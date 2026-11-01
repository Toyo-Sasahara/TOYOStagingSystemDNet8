using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using SasaLib;
using SasaLib.PrintConfig;
using SharedClassLibrary;
using System.Diagnostics;
using ToyoStageService;

namespace NetworkPrintService;

/// <summary>
/// 印刷要求の入力検証、プリンタ解決、印刷実行、結果生成を統括します。
/// </summary>
public sealed class PrintCoordinator(ILogger<PrintCoordinator> logger)
{
    private readonly SemaphoreSlim _printLock = new(1, 1);
    private readonly object _initLock = new();
    private PrintProcess? _printProcess;
    private bool _initialized;

    /// <summary>
    /// NetworkPrintService 画像ファイルを受け取り、印刷キューへの投入を実行します。
    /// </summary>
    /// <param name="request">フォーム経由で受信した印刷要求です。</param>
    /// <param name="cancellationToken">処理中断を通知するキャンセルトークンです。</param>
    /// <returns>HTTP 応答に変換可能な印刷実行結果を返します。</returns>
    public async Task<PrintExecutionResult> PrintAsync(PrintRequestForm request, CancellationToken cancellationToken)
    {
        logger.LogInformation($"■ネットワークプリントサービス PrintCoordinator.PrintAsync(..)メソッドを開始します。DeviceName：{request.DeviceConfigXMLname} ImageFile:{request.ImageFile?.FileName}");

        if (request.ImageFile is null || request.ImageFile.Length <= 0)
        {
            return PrintExecutionResult.BadRequest("imageFile が未指定です。");
        }


        try
        {
            //  印刷機能で利用する設定とプロセスを一度だけ初期化します。
            EnsureInitialized();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "印刷サービス初期化に失敗しました。");
            return PrintExecutionResult.Failed("印刷サービス初期化に失敗しました。", StatusCodes.Status500InternalServerError);
        }

        if (_printProcess is null)
        {
            return PrintExecutionResult.Failed("印刷プロセスの初期化に失敗しました。", StatusCodes.Status500InternalServerError);
        }

        if (request.DeviceConfigXMLname is null)
        {
            logger.LogInformation("DeviceConfigXMLnameがnullでしたので \"\" に置換します");
            request.DeviceConfigXMLname = "";
        }


        //// XML印刷設定名からPrintInfo印刷対象プリンタを解決します。
        PrinterInfo? printTarget = ResolvePrintTarget(_printProcess, request.DeviceConfigXMLname);

        if (String.IsNullOrWhiteSpace(request.DeviceConfigXMLname)==false)
        {
            if (printTarget is null)
            {
                return PrintExecutionResult.BadRequest($"deviceName '{request.DeviceConfigXMLname}' に一致するプリンタ設定が見つかりません。");
            }
            else
            {
                if (!printTarget.Ready || printTarget.IsPrinterFailure)
                {
                    return PrintExecutionResult.Failed($"指定プリンタは利用不可です。Ready={printTarget.Ready}, IsPrinterFailure={printTarget.IsPrinterFailure}", StatusCodes.Status503ServiceUnavailable);
                }

            }

        }


        var extension = Path.GetExtension(request.ImageFile.FileName);
        if (string.IsNullOrWhiteSpace(extension))
        {
            extension = ".img";
        }

        var tempFolder = Path.Combine(Path.GetTempPath(), "NetworkPrintService");
        Directory.CreateDirectory(tempFolder);
        var tempFilePath = Path.Combine(tempFolder, $"{Guid.NewGuid():N}{extension}");

        await using (var fs = new FileStream(tempFilePath, FileMode.CreateNew, FileAccess.Write, FileShare.None))
        {
            await request.ImageFile.CopyToAsync(fs, cancellationToken);
        }

        await _printLock.WaitAsync(cancellationToken);
        try
        {
            var evt = new EventsSummary();
            var documentName = string.IsNullOrWhiteSpace(request.DocumentName)
                ? Path.GetFileNameWithoutExtension(request.ImageFile.FileName)
                : request.DocumentName;

            // 印刷処理を実行します。
            var ok = _printProcess.PrintStart(tempFilePath, evt, request.DeviceConfigXMLname, documentName ?? string.Empty);
            if (!ok)
            {
                var detail = evt.GetEntry();
                logger.LogError("印刷に失敗しました。device={DeviceName}, config={ConfigFile}, detail={Detail}", request.DeviceConfigXMLname, printTarget.PrinterXmlFileName, detail);
                return PrintExecutionResult.Failed("印刷処理に失敗しました。", StatusCodes.Status500InternalServerError, detail);
            }
            else
            {
                var successDetail = evt.GetEntry();

                logger.LogInformation("印刷を受け付けました。device={DeviceName}, config={ConfigFile}, paper={Paper}, orientation={Orientation}, detail={Detail}", request.DeviceConfigXMLname, successDetail);
                return PrintExecutionResult.Success($"印刷キューへ投入しました。config={request.DeviceConfigXMLname}", successDetail);
            }
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "印刷実行中に例外が発生しました。");
            return PrintExecutionResult.Failed("印刷実行中に例外が発生しました。", StatusCodes.Status500InternalServerError, ex.Message);
        }
        finally
        {
            _printLock.Release();
            TryDelete(tempFilePath);
        }
    }

    /// <summary>
    /// 印刷機能で利用する設定とプロセスを一度だけ初期化します。
    /// </summary>
    private void EnsureInitialized()
    {
        if (_initialized)
        {
            return;
        }

        lock (_initLock)
        {
            if (_initialized)
            {
                return;
            }

            StageServerConfigWork.ReadStageServerConfig(ConfigFilesFullPath.STSConfigFileFullpath, false);
            _printProcess = new PrintProcess();
            _initialized = true;
        }
    }

    /// <summary>
    /// 指定デバイス名と用紙サイズから印刷対象プリンタを解決します。
    /// </summary>
    /// <param name="printProcess">プリンタ情報を提供する印刷プロセスです。</param>
    /// <param name="deviceName">要求されたデバイス名です。</param>
    /// <param name="commonPaperSize">要求から解決済みの共通用紙サイズです。</param>
    /// <returns>一致したプリンタ情報。見つからない場合は <see langword="null"/>。</returns>
    private static PrinterInfo? ResolvePrintTarget(PrintProcess printProcess, string deviceName, CommonPaperSize commonPaperSize)
    {
        var device = deviceName.Trim();
        var printers = printProcess.GetPrinterInfos(false);

        var matched = printers.FirstOrDefault(p =>
            EqualsIgnoreCase(p.PrinterName, device) ||
            EqualsIgnoreCase(p.PrinterAliasName, device) ||
            EqualsIgnoreCase(p.PrinterShortCutName, device) ||
            EqualsIgnoreCase(p.PrinterXmlFileName, device));

        if (matched is not null)
        {
            return matched;
        }

        var fallbackConfigName = printProcess.GetPlotterSettingFromPaperSize(commonPaperSize);
        return printers.FirstOrDefault(p => EqualsIgnoreCase(p.PrinterXmlFileName, fallbackConfigName));
    }

    private static PrinterInfo? ResolvePrintTarget(PrintProcess printProcess, string deviceName)
    {
        var device = deviceName.Trim();
        var printers = printProcess.GetPrinterInfos(false);

        var matched = printers.FirstOrDefault(p =>
            EqualsIgnoreCase(p.PrinterName, device) ||
            EqualsIgnoreCase(p.PrinterAliasName, device) ||
            EqualsIgnoreCase(p.PrinterShortCutName, device) ||
            EqualsIgnoreCase(p.PrinterXmlFileName, device));

        if (matched is not null)
        {
            return matched;
        }

        return null;
    }


    /// <summary>
    /// 2 つの文字列を大文字小文字を無視して比較します。
    /// </summary>
    /// <param name="left">比較元文字列です。</param>
    /// <param name="right">比較先文字列です。</param>
    /// <returns>一致する場合は <see langword="true"/>、それ以外は <see langword="false"/>。</returns>
    private static bool EqualsIgnoreCase(string? left, string? right)
    {
        return string.Equals(left, right, StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// 用紙サイズ文字列と向き文字列から共通用紙サイズを解決します。
    /// </summary>
    /// <param name="paperSize">A0～A4 の用紙サイズ文字列です。</param>
    /// <param name="orientation">縦横を表す向き文字列です。</param>
    /// <param name="commonPaperSize">解決結果の共通用紙サイズです。</param>
    /// <returns>解決に成功した場合は <see langword="true"/>、失敗した場合は <see langword="false"/>。</returns>
    private static bool TryResolveCommonPaperSize(string? paperSize, string? orientation, out CommonPaperSize commonPaperSize)
    {
        commonPaperSize = default;

        if (string.IsNullOrWhiteSpace(paperSize) || string.IsNullOrWhiteSpace(orientation))
        {
            return false;
        }

        var size = paperSize.Trim().ToUpperInvariant();
        var orient = orientation.Trim().ToUpperInvariant();

        var isLandscape = orient is "L" or "LANDSCAPE" or "YOKO" or "横" or "横向き";
        var isPortrait = orient is "P" or "PORTRAIT" or "TATE" or "縦" or "縦向き";

        if (!isLandscape && !isPortrait)
        {
            return false;
        }

        return size switch
        {
            "A0" => SetPaperSize(isLandscape, CommonPaperSize.A0L, CommonPaperSize.A0P, out commonPaperSize),
            "A1" => SetPaperSize(isLandscape, CommonPaperSize.A1L, CommonPaperSize.A1P, out commonPaperSize),
            "A2" => SetPaperSize(isLandscape, CommonPaperSize.A2L, CommonPaperSize.A2P, out commonPaperSize),
            "A3" => SetPaperSize(isLandscape, CommonPaperSize.A3L, CommonPaperSize.A3P, out commonPaperSize),
            "A4" => SetPaperSize(isLandscape, CommonPaperSize.A4L, CommonPaperSize.A4P, out commonPaperSize),
            _ => false,
        };
    }

    /// <summary>
    /// 向きに応じて共通用紙サイズを選択します。
    /// </summary>
    /// <param name="isLandscape">横向きの場合は <see langword="true"/>。</param>
    /// <param name="landscape">横向き時の用紙サイズです。</param>
    /// <param name="portrait">縦向き時の用紙サイズです。</param>
    /// <param name="commonPaperSize">選択結果の共通用紙サイズです。</param>
    /// <returns>常に <see langword="true"/> を返します。</returns>
    private static bool SetPaperSize(bool isLandscape, CommonPaperSize landscape, CommonPaperSize portrait, out CommonPaperSize commonPaperSize)
    {
        commonPaperSize = isLandscape ? landscape : portrait;
        return true;
    }

    /// <summary>
    /// 一時ファイルが存在する場合に削除します。
    /// </summary>
    /// <param name="tempFilePath">削除対象の一時ファイルパスです。</param>
    private static void TryDelete(string tempFilePath)
    {
        try
        {
            if (File.Exists(tempFilePath))
            {
                File.Delete(tempFilePath);
            }
        }
        catch
        {
        }
    }
}

/// <summary>
/// /print エンドポイントで受信するフォーム入力モデルです。
/// </summary>
public sealed class PrintRequestForm
{
    /// <summary>
    /// 印刷対象の画像ファイルです。
    /// </summary>
    public IFormFile? ImageFile { get; set; }

    /// <summary>
    /// 対象プリンタの XML 設定ファイル名を示す名称です。
    /// </summary>
    public string? DeviceConfigXMLname { get; set; }


    /// <summary>
    /// 印刷ドキュメント名です。
    /// </summary>
    public string? DocumentName { get; set; }
}

/// <summary>
/// 印刷処理の成否と HTTP 応答情報を表します。
/// </summary>
public sealed class PrintExecutionResult
{
    private PrintExecutionResult(bool isSuccess, int statusCode, string message, string? detail = null)
    {
        IsSuccess = isSuccess;
        StatusCode = statusCode;
        Message = message;
        Detail = detail;
    }

    /// <summary>
    /// 印刷処理が成功したかどうかを示します。
    /// </summary>
    public bool IsSuccess { get; }

    /// <summary>
    /// 返却する HTTP ステータスコードです。
    /// </summary>
    public int StatusCode { get; }

    /// <summary>
    /// 利用者向けメッセージです。
    /// </summary>
    public string Message { get; }

    /// <summary>
    /// 詳細情報またはエラー詳細です。
    /// </summary>
    public string? Detail { get; }

    /// <summary>
    /// 成功結果を生成します。
    /// </summary>
    /// <param name="message">成功時メッセージです。</param>
    /// <param name="detail">成功時の結果詳細です。</param>
    /// <returns>202 Accepted の結果です。</returns>
    public static PrintExecutionResult Success(string message, string? detail = null)
        => new(true, StatusCodes.Status202Accepted, message, detail);

    /// <summary>
    /// 入力エラー結果を生成します。
    /// </summary>
    /// <param name="message">入力エラー内容です。</param>
    /// <returns>400 BadRequest の結果です。</returns>
    public static PrintExecutionResult BadRequest(string message)
        => new(false, StatusCodes.Status400BadRequest, message);

    /// <summary>
    /// 失敗結果を生成します。
    /// </summary>
    /// <param name="message">失敗メッセージです。</param>
    /// <param name="statusCode">返却する HTTP ステータスコードです。</param>
    /// <param name="detail">追加詳細情報です。</param>
    /// <returns>失敗を表す結果です。</returns>
    public static PrintExecutionResult Failed(string message, int statusCode, string? detail = null)
        => new(false, statusCode, message, detail);
}