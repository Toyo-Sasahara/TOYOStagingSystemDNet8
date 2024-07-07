/// <summary>
/// クライアントアプリケーション各設定
/// </summary>
public class SccConfig : SasaLib.XmlSettingFile
{

    /// <summary>
    /// ClientAppConfigクラスの 静的オブジェクト config を生成する。
    /// 静的オブジェクトなので、名前空間名称.クラス名.オブジェクト名 で呼びだせる。
    /// 例:  ApprovalClient.ClientApp.ClientConfig.config.StageServerHost = "CS1";
    /// </summary>      
    public static SccConfig Config { set; get; }

    //以下のメンバーはXMLに保存される

    /// <summary>
    /// ４桁の社員番号
    /// </summary>
    public string LastUserID { get; set; }

    public string LastArcSuiteUserID { get; set; }
    public string LastArcSuiteUserPASS { get; set; }

    /// <summary>
    /// StagingServerのホスト名
    /// </summary>
    public string StageServerHost { get; set; } = "CS1";

    /// <summary>
    /// このアプリケーションの規定のログフォルダ
    /// </summary>
    public string LogFolder { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public string ArcSuiteDmsHost { get; set; } = "ASS1";

    /// <summary>
    /// パイプ接続のためアカウントを強制する場合はtrue
    /// </summary>              
    public bool ClsLogon { get; set; } = false;

    /// <summary>
    /// パイプ接続のアカウントを強制する時に使うドメイン名
    /// </summary>
    public string ClientDomainName { get; set; } = "AD";

    /// <summary>
    /// パイプ接続のアカウントを強制する時に使うユーザー名
    /// </summary>
    public string ClientUserName { get; set; } = "";


    /// <summary>
    /// パイプ接続のアカウントを強制する時に使うパスワード
    /// </summary>
    [System.Xml.Serialization.XmlIgnore]
    public string ClientUserPassword { get; set; }

    public string SasaLibEncryptionType = "SasaAuth2.1";

    public string ClientUserCryptUserPass;

    /// <summary>
    /// ToyoDRAWREGISTservice接続用パイプ名
    /// </summary>
    public string PipeNameDR { get; set; } = "ApprovalServer";

    /// <summary>
    ///  ToyoDRAWCAPTUREservice接続用パイプ名
    /// </summary>
    public string PipeNameDC { get; set; } = "CaptureService";

    /// <summary>
    ///  ToyoDRAWATCHservice接続用パイプ名
    /// </summary>
    public string PipeNameSW { get; set; } = "WatchService";

    /// <summary>
    /// コミットフォルダ
    /// </summary>
    public string CommitPath { get; set; } = @"\\ADS1\COMMIT$";

    public string BackupFolderPath { get; set; }

    public string RestoreFolderPath { get; set; }


    [System.Xml.Serialization.XmlIgnore] //保存したくないメンバーは以下に宣言
    public string NotSaved;

    /// <summary>
    /// Read/WriteObjectメソッドにて転送データの状況を詳細
    /// </summary>
    internal bool ReadWriteObjectVerbose { get; set; } = true;


    /// <summary>
    /// 既定の設定情報を生成。
    /// (デフォルトコンストラクタは必ず必要)
    /// </summary>
    public SccConfig() { }

}


