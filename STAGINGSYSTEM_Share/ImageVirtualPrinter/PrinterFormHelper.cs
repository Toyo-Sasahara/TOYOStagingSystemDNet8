using System.Runtime.InteropServices;

namespace SasaImagePrinter;

/// <summary>
/// Windows プリンターフォーム（用紙サイズ）を登録・管理するヘルパークラス
/// ISO A0、A1 などの大判サイズ対応
/// </summary>
internal static class PrinterFormHelper
{
    [StructLayout(LayoutKind.Sequential)]
    private struct FORM_INFO_1
    {
        public uint Flags;

        [MarshalAs(UnmanagedType.LPStr)]
        public string pName;

        public SIZEL Size;
        public RECTL ImageableArea;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct SIZEL
    {
        public int cx;
        public int cy;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct RECTL
    {
        public int left;
        public int top;
        public int right;
        public int bottom;
    }

    [DllImport("winspool.drv", SetLastError = true, CharSet = CharSet.Ansi)]
    private static extern bool AddForm(IntPtr hPrinter, uint Level, ref FORM_INFO_1 pForm);

    [DllImport("winspool.drv", SetLastError = true, CharSet = CharSet.Ansi)]
    private static extern bool OpenPrinter(string? pPrinterName, out IntPtr phPrinter, IntPtr pDefault);

    [DllImport("winspool.drv", SetLastError = true)]
    private static extern bool ClosePrinter(IntPtr hPrinter);

    /// <summary>
    /// 指定されたプリンターフォーム（用紙サイズ）を追加します
    /// </summary>
    /// <param name="formName">フォーム名（例："ISO A0"）</param>
    /// <param name="widthMm">幅（ミリメートル単位）</param>
    /// <param name="heightMm">高さ（ミリメートル単位）</param>
    /// <exception cref="InvalidOperationException">フォーム追加に失敗した場合</exception>
    private static void AddIsoForm(string formName, double widthMm, double heightMm)
    {
        if (!OpenPrinter(null, out IntPtr hPrinter, IntPtr.Zero))
        {
            throw new InvalidOperationException(
                "Failed to open printer. Ensure you have admin rights and Print Spooler is running.");
        }

        try
        {
            // mm を 1/10 mm 単位に変換
            // Win32 API は FORM_INFO_1.Size を 1/10 mm 単位で期待
            int widthTenthMm = (int)Math.Round(widthMm * 10);
            int heightTenthMm = (int)Math.Round(heightMm * 10);

            var form = new FORM_INFO_1
            {
                Flags = 0,
                pName = formName,
                Size = new SIZEL { cx = widthTenthMm, cy = heightTenthMm },
                ImageableArea = new RECTL
                {
                    left = 0,
                    top = 0,
                    right = widthTenthMm,
                    bottom = heightTenthMm
                }
            };

            if (!AddForm(hPrinter, 1, ref form))
            {
                int errorCode = Marshal.GetLastWin32Error();

                // エラーコード 5 = ERROR_ACCESS_DENIED / 形式が既に存在
                if (errorCode == 5)
                {
                    // フォーム既存の場合は非致命的
                    return;
                }

                throw new InvalidOperationException(
                    $"Failed to add form '{formName}'. Windows error code: {errorCode}");
            }

            System.Diagnostics.Debug.WriteLine(
                $"[PrinterFormHelper] Successfully added form: {formName} ({widthMm}mm x {heightMm}mm)");
        }
        finally
        {
            ClosePrinter(hPrinter);
        }
    }

    /// <summary>
    /// ISO A0・A1 フォームを登録します（スタートアップ時に自動呼び出し）
    /// 既に存在する場合はスキップします
    /// </summary>
    public static void RegisterIsoAForms()
    {
        try
        {
            // ISO A0: 841 × 1189 mm
            AddIsoForm("ISO A0", 841, 1189);
        }
        catch (InvalidOperationException ex)
        {
            System.Diagnostics.Debug.WriteLine($"[PrinterFormHelper] ISO A0 registration: {ex.Message}");
        }

        try
        {
            // ISO A1: 594 × 841 mm
            AddIsoForm("ISO A1", 594, 841);
        }
        catch (InvalidOperationException ex)
        {
            System.Diagnostics.Debug.WriteLine($"[PrinterFormHelper] ISO A1 registration: {ex.Message}");
        }
    }
}
