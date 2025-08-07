using StageServerRemote;
using STAGINGSYSTEM_COMMANDS;
using StreamCommandBridge.StreamBasedClient;
using StreamCommandExecutorClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;

#if NETCOREAPP
using System.Runtime.Versioning;
#endif

namespace SasaLib.NumberingSupport
{
    /// <summary>
    /// 
    /// </summary>
#if NETCOREAPP
    [SupportedOSPlatform("windows")]
#endif
    public class NumberingSuppportConfigs
    {
        /// <summary>
        /// 図面種類判別用設定XMLﾌｧｲﾙ
        /// (ｻｰﾊﾞｰ側設定ﾌｧｲﾙ取得先ﾌﾙﾊﾟｽ)
        /// </summary>
        public string SourceNumberTypeConfigXMLfullpath { get; private set; } = @"C:\ProgramData\TOYOCOMMON\NumberTypeConfig.XML";

        /// <summary>
        ///  図面種類判別用設定XMLﾌｧｲﾙ
        /// </summary>
        public string NumberTypeConfigXMLfullpath { get; private set; } = @"C:\Users\Public\Documents\TOYOCOMMON\NumberTypeConfig.XML";

        /// <summary>
        /// 図番ﾊﾟﾀｰﾝとArcSuite登録図面番号対応表ﾌｧｲﾙ
        /// (ｻｰﾊﾞｰ側設定ﾌｧｲﾙ取得先ﾌﾙﾊﾟｽ)
        /// </summary>
        public string SourceConversionFormulaNumberConfigfullpath { get; private set; } = @"C:\ProgramData\TOYOCOMMON\ConversionFormulaNumberConfig.XML";

        /// <summary>
        /// 図番ﾊﾟﾀｰﾝとArcSuite登録図面番号対応表ﾌｧｲﾙ
        ///  (ﾛｰｶﾙ側ﾌﾙﾊﾟｽ名)
        /// </summary>
        public string ConversionFormulaNumberConfigfullpath { get; private set; } = @"C:\Users\Public\Documents\TOYOCOMMON\ConversionFormulaNumberConfig.XML";

        /// <summary>
        /// 
        /// (ｻｰﾊﾞｰ側設定ﾌｧｲﾙ取得先ﾌﾙﾊﾟｽ)
        /// </summary>
        public string SourceStageServerDatabaseConfigXMLfullpath { get; private set; } = @"C:\ProgramData\TOYOCOMMON\StageServerDatabaseConfig.XML";

        /// <summary>
        /// 共通属性名とアークスイート属性名を紐づける XMLﾌｧｲﾙ
        /// (ﾛｰｶﾙ側ﾌﾙﾊﾟｽ名)
        /// </summary>
        public string StageServerDatabaseConfigXMLfullpath { get; private set; } = @"C:\Users\Public\Documents\TOYOCOMMON\StageServerDatabaseConfig.XML";

        /// <summary>
        /// 材質コード・材質名対応 XMLﾌｧｲﾙ
        /// (ｻｰﾊﾞｰ側設定ﾌｧｲﾙ取得先ﾌﾙﾊﾟｽ)
        /// </summary>
        public string SourceMaterialCodeConfigXMLfullpath { get; private set; } = @"C:\ProgramData\TOYOCOMMON\MaterialCodeConfig.XML";

        /// <summary>
        /// 材質コード・材質名対応 XMLﾌｧｲﾙ
        /// (ﾛｰｶﾙ側ﾌﾙﾊﾟｽ名)
        /// </summary>
        public string MaterialCodeConfigXMLfullpath { get; private set; } = @"C:\Users\Public\Documents\TOYOCOMMON\MaterialCodeConfig.XML";

        /// <summary>
        /// 購入先コード・購入先名対応 XMLﾌｧｲﾙ
        /// (ｻｰﾊﾞｰ側設定ﾌｧｲﾙ取得先ﾌﾙﾊﾟｽ)
        /// </summary>
        public string SourcePurchasingManufacturerCodeConfigXMLfullpath { get; private set; } = @"C:\ProgramData\TOYOCOMMON\PurchasingManufacturerCodeConfig.XML";

        /// <summary>
        /// 購入先コード・購入先名対応 XMLﾌｧｲﾙ
        /// (ﾛｰｶﾙ側ﾌﾙﾊﾟｽ名)
        /// </summary>
        public string PurchasingManufacturerCodeConfigXMLfullpath { get; private set; } = @"C:\Users\Public\Documents\TOYOCOMMON\PurchasingManufacturerCodeConfig.XML";

        /// <summary>
        /// 注意ワード情報をダウンロード、デシリアライズ
        /// (ｻｰﾊﾞｰ側設定ﾌｧｲﾙ取得先ﾌﾙﾊﾟｽ)
        /// </summary>
        public string SourcePhrasesToBeAwareXMLfullpath { get; private set; } = @"C:\Users\Public\Documents\TOYOCOMMON\PhrasesToBeAwareConfig.XML";

        /// <summary>
        /// 統合ストリームによるデータ送受信
        /// </summary>
        public bool IsNewStreamMode { get; private set; }

        string ClientDomainName;
        string ClientUserName;
        string ClientPassword;
        bool ClsLogon;
        string StageServerHost;
        string PipeNameDC;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="ClientDomainName"></param>
        /// <param name="ClientUserName"></param>
        /// <param name="ClientPassword"></param>
        /// <param name="ClsLogon"></param>
        /// <param name="StageServerHost"></param>
        /// <param name="PipeNameDC"></param>
        /// <param name="WriteLine"></param>
        public NumberingSuppportConfigs(string ClientDomainName, string ClientUserName, string ClientPassword, bool ClsLogon,
            string StageServerHost, string PipeNameDC, bool isNewStreamMode = false, Action<string> WriteLine = null)
        {
            if (WriteLine == null) WriteLine = Console.WriteLine;

            // 各フィールドの値を検証
            if (string.IsNullOrWhiteSpace(StageServerHost))
                throw new ArgumentNullException(nameof(StageServerHost), "NumberingSuppportConfigs コンストラクタエラー PipeServerName cannot be null or empty.");
            if (string.IsNullOrWhiteSpace(PipeNameDC))
                throw new ArgumentNullException(nameof(PipeNameDC), "NumberingSuppportConfigs コンストラクタエラー PipeName cannot be null or empty.");

            this.ClientDomainName = ClientDomainName;
            this.ClientUserName = ClientUserName;
            this.ClientPassword = ClientPassword;
            this.ClsLogon = ClsLogon;
            this.StageServerHost = StageServerHost;
            this.PipeNameDC = PipeNameDC;

            this.IsNewStreamMode = IsNewStreamMode;

        }

        /// <summary>
        /// ■表題欄検証データセット（NumberTypeConfig.XML , ConversionFormulaNumberConfig.XML , StageServerDatabaseConfig.XML , MaterialCodeConfig.XML） を準備
        /// </summary>
        /// <param name="importantWriteLine">ログメッセージのデリゲート</param>
        /// <param name="WriteLine"></param>
        /// <returns></returns>
        public async Task<bool> DataSetDownloadAsync(Action<string> importantWriteLine = null, Action<string> WriteLine = null)
        {
            if (importantWriteLine == null) importantWriteLine = DebugConsole.WriteLine;
            if (WriteLine == null) WriteLine = DebugConsole.WriteLine;

            WriteLine($"■図面種類を特定するデータベース・マテリアルコードを判別するデータベースの受信を開始します");

            bool result = true;

            bool downloadNumberTypeConfiSucess; /// ①NumberTypeConfig.XML をサーバーから複製結果

            bool downloadSuffixZeroPadVariantNumberTypeConfigSucess;　/// ②SuffixZeroPadVariantNumberTypeConfig.XML をサーバーから複製結果

            bool downloadStageServerDatabaseConfigSucess;　/// ③StageServerDatabaseConfig.XML をサーバーから複製結果

            bool downloadMaterialCodeConfigSucess; /// ④MaterialCodeConfig.XML をサーバーから複製結果

            /// ①NumberTypeConfig.XML をサーバーから複製します
            downloadNumberTypeConfiSucess = await LoadConfigFileFromStageServerAsync(SourceNumberTypeConfigXMLfullpath, NumberTypeConfigXMLfullpath, WriteLine);
            if (downloadNumberTypeConfiSucess == false)
                importantWriteLine($"※ExecuteDownloadAndDeserialize(..) ｻｰﾊﾞｰ側ﾌｧｲﾙ \"{SourceNumberTypeConfigXMLfullpath}\" を ﾛｰｶﾙPC \"{NumberTypeConfigXMLfullpath}\" へのダウンロードに失敗しました。");
            else
                WriteLine($"■NumberingSuppportConfigs.ExecuteDownloadAndDeserialize(..)ｻｰﾊﾞｰ側ﾌｧｲﾙ \"{SourceNumberTypeConfigXMLfullpath}\" を ﾛｰｶﾙPC \"{NumberTypeConfigXMLfullpath}\" へダウンロードしました。");

            /// ②SuffixZeroPadVariantNumberTypeConfig.XML をサーバーから複製します
            downloadSuffixZeroPadVariantNumberTypeConfigSucess = await LoadConfigFileFromStageServerAsync(SourceConversionFormulaNumberConfigfullpath, ConversionFormulaNumberConfigfullpath, WriteLine);
            if (downloadSuffixZeroPadVariantNumberTypeConfigSucess == false)
                importantWriteLine($"※ExecuteDownloadAndDeserialize(..) ｻｰﾊﾞｰ側ﾌｧｲﾙ \"{SourceConversionFormulaNumberConfigfullpath}\" を ﾛｰｶﾙPC \"{ConversionFormulaNumberConfigfullpath}\" へのダウンロードに失敗しました。");
            else
                WriteLine($"■NumberingSuppportConfigs.ExecuteDownloadAndDeserialize(..) ｻｰﾊﾞｰ側ﾌｧｲﾙ \"{SourceConversionFormulaNumberConfigfullpath}\" を ﾛｰｶﾙPC \"{ConversionFormulaNumberConfigfullpath}\" へダウンロードしました。");

            /// ③StageServerDatabaseConfig.XML をサーバーから複製します
            downloadStageServerDatabaseConfigSucess = await LoadConfigFileFromStageServerAsync(SourceStageServerDatabaseConfigXMLfullpath, StageServerDatabaseConfigXMLfullpath, WriteLine);
            if (downloadStageServerDatabaseConfigSucess == false)
                importantWriteLine($"※ExecuteDownloadAndDeserialize(..) ｻｰﾊﾞｰ側ﾌｧｲﾙ \"{SourceStageServerDatabaseConfigXMLfullpath}\" を ﾛｰｶﾙPC \"{StageServerDatabaseConfigXMLfullpath}\" へのダウンロードに失敗しました。");
            else
                WriteLine($"■NumberingSuppportConfigs.ExecuteDownloadAndDeserialize(..) ｻｰﾊﾞｰ側ﾌｧｲﾙ \"{SourceStageServerDatabaseConfigXMLfullpath}\" を ﾛｰｶﾙPC \"{StageServerDatabaseConfigXMLfullpath}\"へダウンロードしました。");

            /// ④MaterialCodeConfig.XML をサーバーから複製します
            downloadMaterialCodeConfigSucess = await LoadConfigFileFromStageServerAsync(SourceMaterialCodeConfigXMLfullpath, MaterialCodeConfigXMLfullpath, WriteLine);
            if (downloadMaterialCodeConfigSucess == false)
                importantWriteLine($"※NumberingSuppportConfigs.ExecuteDownloadAndDeserialize(..) ｻｰﾊﾞｰ側ﾌｧｲﾙ \"{SourceMaterialCodeConfigXMLfullpath}\" を ﾛｰｶﾙPC \"{MaterialCodeConfigXMLfullpath}\" へのダウンロードに失敗しました。");
            else
                WriteLine($"■NumberingSuppportConfigs.ExecuteDownloadAndDeserialize(..) ｻｰﾊﾞｰ側ﾌｧｲﾙ \"{SourceMaterialCodeConfigXMLfullpath}\" を ﾛｰｶﾙPC \"{MaterialCodeConfigXMLfullpath}\" へダウンロードしました。");
            return result;
        }

        /// <summary>
        ///　表題欄検証データセットをデシリアライズします
        /// </summary>
        /// <param name="importantWriteLine"></param>
        /// <param name="WriteLine"></param>
        /// <returns></returns>
        public bool ExecuteDeserialize(Action<string> importantWriteLine = null, Action<string> WriteLine = null)
        {
            if (importantWriteLine == null) importantWriteLine = Console.WriteLine;
            if (WriteLine == null) WriteLine = Console.WriteLine;

            WriteLine($"■図面種類を特定するデータベース・マテリアルコードを判別するデータベースのデシリアライズを開始します");

            bool result = true;


            bool preparationNumberTypeConfiSucess; /// ①NumberTypeConfig.XML のデシリアライズ結果

            bool preparationSuffixZeroPadVariantNumberTypeConfigSucess;　/// ②SuffixZeroPadVariantNumberTypeConfig.XML のデシリアライズ結果

            bool preparationStageServerDatabaseConfigSucess;　/// ③StageServerDatabaseConfig.XML のデシリアライズ結果

            bool preparationMaterialCodeConfigSucess; /// ④MaterialCodeConfig.XML のデシリアライズ結果

            /// ①NumberTypeConfig.XML をメモリへﾃﾞシリアライズを行う
            preparationNumberTypeConfiSucess = NumberTypeConfigWork.PreparationConfigData(NumberTypeConfigXMLfullpath, false, WriteLine);
            if (preparationNumberTypeConfiSucess == false)
            {
                importantWriteLine($"※NumberingSuppportConfigs.ExecuteDownloadAndDeserialize(..) ﾛｰｶﾙPC \"{NumberTypeConfigXMLfullpath}\" のデシリアライズに失敗しました。開発初期値データを使用します");
                preparationNumberTypeConfiSucess = NumberTypeConfigWork.PreparationConfigData(NumberTypeConfigXMLfullpath, true, WriteLine);
                result = false;
            }
            else
                WriteLine($"■NumberingSuppportConfigs.ExecuteDownloadAndDeserialize(..) \"{NumberTypeConfigXMLfullpath}\" のデシリアライズに成功しました。");



            /// ②SuffixZeroPadVariantNumberTypeConfig.XMLL をメモリへﾃﾞシリアライズを行う 指定ﾊﾞｰｼﾞｮﾝ未満の場合は新規作成される
            preparationSuffixZeroPadVariantNumberTypeConfigSucess = ConversionFormulaNumberConfigWork.PreparationConfigData(ConversionFormulaNumberConfigfullpath, false, importantWriteLine);
            if (preparationSuffixZeroPadVariantNumberTypeConfigSucess == false)
            {
                importantWriteLine($"※NumberingSuppportConfigs.ExecuteDownloadAndDeserialize(..) ﾛｰｶﾙPC \"{ConversionFormulaNumberConfigfullpath}\" のデシリアライズに失敗しました。開発初期値データを使用します");
                preparationSuffixZeroPadVariantNumberTypeConfigSucess = ConversionFormulaNumberConfigWork.PreparationConfigData(ConversionFormulaNumberConfigfullpath, true, importantWriteLine);
                result = false;
            }
            else
            {
                WriteLine($"■NumberingSuppportConfigs.ExecuteDownloadAndDeserialize(..) \"{ConversionFormulaNumberConfigfullpath}\" のデシリアライズに成功しました。");
            }



            /// ③StageServerDatabaseConfig.XML をメモリへﾃﾞシリアライズを行う
            preparationStageServerDatabaseConfigSucess = StageServerDatabaseConfigWork.PreparationConfigData(StageServerDatabaseConfigXMLfullpath, false, importantWriteLine);
            if (preparationStageServerDatabaseConfigSucess == false)
            {
                importantWriteLine($"※NumberingSuppportConfigs.ExecuteDownloadAndDeserialize(..) ﾛｰｶﾙPC \"{StageServerDatabaseConfigXMLfullpath}\" のデシリアライズに失敗しました。開発初期値データを使用します");
                preparationStageServerDatabaseConfigSucess = StageServerDatabaseConfigWork.PreparationConfigData(StageServerDatabaseConfigXMLfullpath, true, importantWriteLine);
                result = false;
            }
            else
            {
                WriteLine($"■NumberingSuppportConfigs.ExecuteDownloadAndDeserialize(..) \"{StageServerDatabaseConfigXMLfullpath}\" のデシリアライズに成功しました。");
            }

            /// ④MaterialCodeConfig.XMLL をメモリへﾃﾞシリアライズを行う 指定ﾊﾞｰｼﾞｮﾝ未満の場合は新規作成される
            preparationMaterialCodeConfigSucess = MaterialCodeConfigWork.PreparationConfigData(MaterialCodeConfigXMLfullpath, false, 1.5d, importantWriteLine);
            if (preparationMaterialCodeConfigSucess == false)
            {
                importantWriteLine($"※NumberingSuppportConfigs.ExecuteDownloadAndDeserialize(..) ﾛｰｶﾙPC \"{MaterialCodeConfigXMLfullpath}\" のデシリアライズに失敗しました。開発初期値データを使用します");
                preparationMaterialCodeConfigSucess = MaterialCodeConfigWork.PreparationConfigData(MaterialCodeConfigXMLfullpath, true, 1.5d, importantWriteLine);
                result = false;
            }
            else
            {
                WriteLine($"■NumberingSuppportConfigs.ExecuteDownloadAndDeserialize(..) \"{MaterialCodeConfigXMLfullpath}\" のデシリアライズに成功しました。");
            }

            return result;
        }

        /// <summary>
        /// ■購入品メーカーコード検証データ PurchasingManufacturerCodeConfig.XML をサーバーからダウンロード＆メモリへ展開
        /// </summary>
        /// <param name="DebugWriteLine"></param>
        /// <param name="NumberingSuppportConfigsFileLocalOnly"></param>
        /// <returns></returns>
        public async Task<bool> ExecuteDownloadAndDeserialize_PurchasingManufacturerCodeConfigAsync(Action<string> WriteLine = null, Action<string> DebugWriteLine = null, bool UseLocalConfigFileOnly = false)
        {
            if (DebugWriteLine == null) DebugWriteLine = DebugConsole.WriteLine;
            if (WriteLine == null) WriteLine = DebugConsole.WriteLine;

            bool result = true;

            if (UseLocalConfigFileOnly)
                DebugWriteLine($"□サーバーからPurchasingManufacturerCodeConfig.XMLを読み出さずローカルファイルのみ使用します");
            else
                DebugWriteLine($"■サーバーからPurchasingManufacturerCodeConfig.XMLを読み出しﾛｰｶﾙﾌｧｲﾙを上書きします。");


            bool downloadSucess; /// ⑤PurchasingManufacturerCodeConfig.XML をサーバーから複製結果
            bool preparatonSucess; /// ⑤PurchasingManufacturerCodeConfig.XML のデシリアライズ結果

            /// ⑤PurchasingManufacturerCodeConfig.XML をサーバーから複製します
            if (UseLocalConfigFileOnly == false)
            {
                downloadSucess = await LoadConfigFileFromStageServerAsync(SourcePurchasingManufacturerCodeConfigXMLfullpath, PurchasingManufacturerCodeConfigXMLfullpath, DebugWriteLine);
                if (downloadSucess == false)
                    WriteLine($"※ExecuteDownloadAndDeserialize_PurchasingManufacturerCodeConfig(..) \"{SourcePurchasingManufacturerCodeConfigXMLfullpath}\" のダウンロードに失敗しました。");
                else
                    DebugWriteLine($"■ｻｰﾊﾞｰ側ﾌｧｲﾙ \"{SourcePurchasingManufacturerCodeConfigXMLfullpath}\" を ﾛｰｶﾙPC \"{PurchasingManufacturerCodeConfigXMLfullpath}\"へダウンロードしました。");
            }

            /// ⑤PurchasingManufacturerCodeConfig.XML をメモリへﾃﾞシリアライズを行う
            preparatonSucess = PurchasingManufacturerCodeConfigWork.PreparationConfigData(PurchasingManufacturerCodeConfigXMLfullpath, false, 1.5d, DebugWriteLine);

            if (preparatonSucess == false)
            {
                WriteLine($"※{PurchasingManufacturerCodeConfigXMLfullpath} のデシリアライズに失敗しました。開発初期値データを使用します");
                preparatonSucess = PurchasingManufacturerCodeConfigWork.PreparationConfigData(PurchasingManufacturerCodeConfigXMLfullpath, true, 1.5d, DebugWriteLine);
                result = false;
            }
            else
            {
                DebugWriteLine($"■{PurchasingManufacturerCodeConfigXMLfullpath} のデシリアライズに成功しました。");
            }

            return result;
        }

        /// <summary>
        /// ■PhrasesToBeAwareConfig.XML をサーバーからダウンロード＆メモリへ展開
        /// </summary>
        /// <param name="WriteLine"></param>
        /// <param name="DebugWriteLine"></param>
        /// <param name="UseLocalConfigFileOnly"></param>
        /// <returns></returns>
        public async Task<bool> ExecuteDownloadAndDeserialize_PhrasesToBeAwareConfigAsync(Action<string> WriteLine = null, Action<string> DebugWriteLine = null, bool UseLocalConfigFileOnly = false)
        {
            if (DebugWriteLine == null) DebugWriteLine = DebugConsole.WriteLine;
            if (WriteLine == null) WriteLine = DebugConsole.WriteLine;

            bool result = true;

            if (UseLocalConfigFileOnly)
                DebugWriteLine($"□ExecuteDownloadAndDeserialize_PhrasesToBeAwareConfig(..) サーバーから PhrasesToBeAwareConfig.XMLを読み出さずローカルファイルのみ使用します");
            else
                DebugWriteLine($"■ExecuteDownloadAndDeserialize_PhrasesToBeAwareConfig(..) サーバーから PhrasesToBeAwareConfig.XMLを読み出しﾛｰｶﾙﾌｧｲﾙを上書きします。");


            bool downloadSucess; /// ⑤PhrasesToBeAware.XML をサーバーから複製結果
            bool preparatonSucess; /// ⑤PhrasesToBeAware.XML のデシリアライズ結果

            /// ⑤PhrasesToBeAwareConfig.XML をサーバーから複製します
            if (UseLocalConfigFileOnly == false)
            {
                downloadSucess = await LoadConfigFileFromStageServerAsync(SourcePhrasesToBeAwareXMLfullpath, SourcePhrasesToBeAwareXMLfullpath, DebugWriteLine);
                if (downloadSucess == false)
                    WriteLine($"※ExecuteDownloadAndDeserialize_PhrasesToBeAwareConfig(..) \"{SourcePhrasesToBeAwareXMLfullpath}\" のダウンロードに失敗しました。");
                else
                    WriteLine($"■ExecuteDownloadAndDeserialize_PhrasesToBeAwareConfig(..) ｻｰﾊﾞｰ側ﾌｧｲﾙ \"{SourcePhrasesToBeAwareXMLfullpath}\" を ﾛｰｶﾙPC \"{SourcePhrasesToBeAwareXMLfullpath}\"へダウンロードしました。");
            }

            /// ⑤PhrasesToBeAwareConfig.XML をメモリへﾃﾞシリアライズを行う
            preparatonSucess = PhrasesToBeAwareConfigWork.PreparationConfigData(SourcePhrasesToBeAwareXMLfullpath, false, DebugWriteLine);

            if (preparatonSucess == false)
            {
                WriteLine($"※{SourcePhrasesToBeAwareXMLfullpath} のデシリアライズに失敗しました。開発初期値データを使用します");
                preparatonSucess = PhrasesToBeAwareConfigWork.PreparationConfigData(SourcePhrasesToBeAwareXMLfullpath, true, DebugWriteLine);
                result = false;
            }
            else
            {
                DebugWriteLine($"■ExecuteDownloadAndDeserialize_PhrasesToBeAwareConfig(..) \"{SourcePhrasesToBeAwareXMLfullpath}\" のデシリアライズに成功しました。");
            }

            return result;
        }

        /// <summary>
        /// サーバーからファイルを受信
        /// </summary>
        /// <param name="SouceFile"></param>
        /// <param name="DistnationFile"></param>
        /// <param name="WriteLine"></param>
        /// <returns></returns>
        async Task<bool> LoadConfigFileFromStageServerAsync(string SouceFile, string DistnationFile, Action<string> WriteLine = null)
        {
            if (IsNewStreamMode == false)
            {
                if (WriteLine == null) WriteLine = Console.WriteLine;

                RemoteClientDRAWCAPTURE remoteClientDC = new RemoteClientDRAWCAPTURE(
                        this.ClientDomainName,
                        this.ClientUserName,
                        this.ClientPassword,
                        this.ClsLogon,
                        this.StageServerHost,
                        this.PipeNameDC);

                //var ans = remoteClientDC.GetTextFileFromPIPE(SouceFile, DistnationFile, WriteLine);
                string resultMsg;
                var ans = await remoteClientDC.FileRecvAsync(SouceFile, DistnationFile, objectConvNew: true, WriteLine: WriteLine, debugMode: false);
                if (ans.Sucess)
                {
                    WriteLine($"LoadConfigFileFromStageServer(..) ｽﾃｰｼﾞｻｰﾊﾞｰ {StageServerHost}から \"{SouceFile}\" の複製に成功 {ans.ResultMsg}");
                    return ans.Sucess;
                }
                else
                {
                    WriteLine($"※LoadConfigFileFromStageServer(..) ｽﾃｰｼﾞｻｰﾊﾞｰ {StageServerHost}から \"{SouceFile}\" の複製に失敗しました {ans.ResultMsg}");
                    return ans.Sucess;
                }
            }
            else
            {

                var tcpClient = new StreamBasedClient(
                new StreamProvider_Tcp(StageServerHost, 12345, WriteLine),WriteLine);

                CMD_ServerControl_FileRecv_Client.Param param = new CMD_ServerControl_FileRecv_Client.Param()
                {
                    ServerSourceFullFileName = SouceFile,
                    LocalDistFullFileName = DistnationFile,
                    debugMode = true,
                    WriteLine = WriteLine,
                };

                var result = await tcpClient.SendCommandAsync(
                        commandName: CMDS.DC_ServerControl_FileRecv, subCommandName: null,
                        ExecuteCommandByNameAsync: CMD_ServerControl_FileRecv_Client.ExecuteAsync, commandParam: param,
                        userName: Environment.UserName
                    );

            }
            return true;
        }


        internal void WrilteLine2(string value)
        {
            DebugConsole.Write(value);

        }

    }
}
