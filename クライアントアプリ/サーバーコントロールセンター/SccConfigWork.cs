using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using SasaLib;
using SharedClassLibrary;

/// <summary>
/// メイン設定ファイルに関するクラス
/// </summary>    
public class SccConfigWork
{
    public static string GetAppConfigFolder()
    {
        string ApplicationDataFolder = System.Environment.GetFolderPath(Environment.SpecialFolder.CommonDocuments) + @"\TOYOCOMMON\ServerControl";
        return ApplicationDataFolder;
    }

    /// <summary>
    /// "%APPDATA%\CAD承認登録ツール\MainConfig.xml"を読み込み
    /// </summary>
    /// <param name="ConfigFilename"></param>
    public static void ReadConfig(string ConfigFilename)
    {
        /// 設定fileフォルダ組立
        string ApplicationDataFolder = GetAppConfigFolder();

        try
        {
            string configFullPath = System.IO.Path.Combine(ApplicationDataFolder, ConfigFilename);
            if (System.IO.File.Exists(configFullPath))
            {
                DateTime toriggerDatetimeNow = new DateTime(2021, 12, 17, 0, 0, 0);

                // ファイルのCreationTimeが 指定日時より古い場合
                DateTime ConfigFileLastWriteTime = System.IO.File.GetLastWriteTime(configFullPath);
                if (ConfigFileLastWriteTime < toriggerDatetimeNow)
                {
                    // 古い設定ファイルは削除しておく
                    System.IO.File.Delete(configFullPath);
                    //GlovalValues.Mylog.WriteLine($"{configFullPath}のLasttime{ConfigFileLastWriteTime}は日時{toriggerDatetimeNow}より古いため削除しました。");
                    //GlovalValues.Mylog.Flash();
                }
            }

            SccConfig.Config = (SccConfig)XmlSettingFile.Load(ApplicationDataFolder, new SccConfig(), ConfigFilename);

            if (string.IsNullOrWhiteSpace(SccConfig.Config.PipeNameDR))
                SccConfig.Config.PipeNameDR = "ApprovalServer";
            if (string.IsNullOrWhiteSpace(SccConfig.Config.PipeNameDC))
                SccConfig.Config.PipeNameDC = "CaptureService";

            if (string.IsNullOrWhiteSpace(SccConfig.Config.PipeNameSW))
                SccConfig.Config.PipeNameSW = "WatchService";

            //GlovalValues.Mylog.WriteLine($"■ClientAppConfigファイル = {ClientAppConfig.Config.Filename}を読み出しました");

        }
        catch (Exception ex)
        {
            DebugConsole.WriteLine($"CAD図面押印＆自動登録センターClientAppConfig.Config.ReadConfig()で例外発生 {ex.Message}");
        }
    }

}

/*　①　XMLの定義用に次のクラスを準備する
 *　
     /// <summary>
    /// XMLアプリケーション設定サンプル
    /// </summary>
    public class XMLconfigPreparation : SasaLib.XmlSettingFile
    {
        /// <summary>
        /// 既定の設定情報を生成。
        /// (デフォルトコンストラクタは必ず必要)
        /// </summary>
        public XMLconfigPreparation() { }

        //以下のメンバーはXMLに保存される

        public string ListenAddres { get; set; } = "127.0.0.1";
        public string OutputPrinter { get; set; } = "Brother MFC-J6770CDW Printer";
        public int FeatureOption { get; set; } = 0;
        public bool DebugOption { get; set; } = false;

        //保存したくないメンバーは以下のように宣言
        [System.Xml.Serialization.XmlIgnore]
        public string NotSaved;
    }

    ②　XMLの定義をインスタンス化する

    /// <summary>
    /// XML設定ファイルの定義を行う
    /// </summary>
    static public XMLconfigPreparation confSet = new XMLconfigPreparation();


    ③ XML設定を読み込むメソッドを準備する

    /// <summary>
    /// XML設定ファイルを読み込む
    /// </summary>
    static void ReadSeeting()
    {
        try
        {
            // 設定ファイル位置
            // C:\ProgramData\(CompanyName)\(ProductName)\(ProductVersion)\SampleSetting.xml
            // C:\ProgramData\ソケットサーバーconsole1\1.0.0.0
            // Environment.SpecialFolder共用体を参照
            //
            confSet = SasaLib.XmlSettingFile.Load(
                Environment.SpecialFolder.MyDocuments,
                new XMLconfigPreparation()) as XMLconfigPreparation;
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error:{0}", ex.Message);
        }
        finally
        {
            confSet.Save();
        }
    }

    ④ 初期化メソッドからXML設定を読み込むメソッドをコールする
    public static int Main(String[] args)
    {
        ReadSeeting();
    }

 */

