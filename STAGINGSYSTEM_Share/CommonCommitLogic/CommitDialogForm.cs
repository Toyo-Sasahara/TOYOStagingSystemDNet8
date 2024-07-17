using SasaLib.ArcSuitePreview;
using SasaLib.NumberingSupport;
using StageServerRemote;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SasaLib;
using System.Threading;
using System.Security.Cryptography;
using System.Runtime.Versioning;
using CommonCommitLogicDNet8.Properties;
using System.Globalization;
using System.Text.RegularExpressions;
using EnvDTE;

namespace CommonCommitLogic
{
    /// <summary>
    /// コミットダイアログクラス
    /// </summary>
    [SupportedOSPlatform("windows")]
    public partial class CommitDialogForm : Form
    {
        /// <summary>
        /// 
        /// </summary>
        private CommitParam commitParam;

        /// <summary>
        /// 
        /// </summary>
        bool PrintOutOnly { get; set; }

        /// <summary>
        /// 
        /// </summary>
        string AtiveDocFullFilename { get; set; }

        /// <summary>
        /// 
        /// </summary>
        string ActiveDocumentWithoutPathAndExtension { get; set; }

        /// <summary>
        /// 
        /// </summary>
        SasaLibDelegateWriteLine WriteLine = DebugConsole.WriteLine;

        /// <summary>
        /// 
        /// </summary>
        private CancellationTokenSource cts = new CancellationTokenSource();

        /// <summary>
        /// 
        /// </summary>
        private CommonTicket ticketXml;

        /// <summary>
        /// コミット最大化プレビュー用オブジェクト
        /// </summary>
        private BigPreviewForm bigPreviewForm;

        /// <summary>
        /// nullの場合はまだ確認されていない
        /// </summary>
        private CadDrawingFile _cadDrwingFile;

        /// <summary>
        /// nullの場合はまだ確認されていない
        /// </summary>
        private TypeNumber _typeNumber;

        /// <summary>
        /// nullの場合はまだ確認されていない
        /// </summary>
        private ReserveNumber _reserveNumber;

        /// <summary>
        /// nullの場合はまだ確認されていない
        /// </summary>
        private ArcSuiteSearchResult _arcSuiteSearchResult;

        /// <summary>
        /// 検索結果が1件のみに通用するアークスイート検索図
        /// </summary>
        private ArcsuitePreview ArcsuitePreview
        {
            get
            {
                if (_arcSuiteSearchResult.arcSuitePreviews != null && _arcSuiteSearchResult.arcSuitePreviews.Count == 1 && _arcSuiteSearchResult.arcSuitePreviews[0].Found == true)
                {

                    WriteLine($"[プロパティ:CommitDialogForm.ArcsuitePreview]が要求されました ,1件の検索結果を返します");
                    return _arcSuiteSearchResult.arcSuitePreviews[0];
                }
                else
                {
                    WriteLine($"[プロパティ:CommitDialogForm.ArcsuitePreview]が要求されました , List<ArcsuitePreview> 'ArcSuiteSearchResult.arcSuitePreviews'にアクセスされました。１件以外の結果 {_arcSuiteSearchResult.arcSuitePreviews.Count}件 のデータを保持していますので new ArcsuitePreview()を返します");
                    return new ArcsuitePreview();
                }
            }
        }

        /// <summary>
        /// 類番検索に使う追加ｻﾌｨｯｸｽ
        /// </summary>
        private List<string> ArcSuitePARTNUMBER_ContainSuffixs = new List<string>() { "RL", "R", "L" };

        /// <summary>
        /// ダイアログを閉じてよいかを保持
        /// </summary>
        private bool close_permition = true;


        /// <summary>
        /// 
        /// </summary>
        private CommitSupportCadDrawingFile supportCadDrawingFile;

        /// <summary>
        /// 採番サーバーとの通信をつかさどる
        /// </summary>
        private CommitSupportNumbering supportNumbering;

        /// <summary>
        /// アークスイートとの通信をつかさどる
        /// </summary>
        private CommitSupportArcSuite supportArcSuite;

        private bool CanUseVariantTypeDrawing { get; set; }

        private Helper helper = new Helper();

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public CommitDialogForm(CommitParam Config, CommitSupportCadDrawingFile supportCadDrawingFile,
            CommitSupportArcSuite supportArcSuite, CommitSupportNumbering supportNumbering,
            SasaLibDelegateWriteLine WriteLine, bool PrintOutOnly = false, bool CanUseVariantTypeDrawing = false)
        {
            this.commitParam = Config;
            this.PrintOutOnly = PrintOutOnly;
            this.WriteLine = WriteLine;

            this.supportCadDrawingFile = supportCadDrawingFile;
            this.supportNumbering = supportNumbering;
            this.supportArcSuite = supportArcSuite;
            this.CanUseVariantTypeDrawing = CanUseVariantTypeDrawing;

            //this.AtiveDocFullFilename = supportCadDrawingFile.CadDocumentFullfileName;
            ////string activeDocName = System.IO.Path.GetFileName(activeDocFullFileName);
            //string activeDocName = supportCadDrawingFile.CadDocumentFileName;


            InitializeComponent();

            // CADファイル関連表示・ボタン 初期化

            InvokeRequired_Control_Text(CadFileWarningIgnore_button, "調査中", default, default);
            InvokeRequired_Control_Enabled(CadFileInformation_label, false, false);

            InvokeRequired_Control_Text(CadFileWarningIgnore_button, "調査中", default, default);
            InvokeRequired_Control_Enabled(CadFileWarningIgnore_button, false, false);

            NumberingWebServer_button.Image = default;

            // 採番関連表示・ボタン 初期化

            InvokeRequired_Control_Enabled(NumberingInformation_label, false, false);

            InvokeRequired_Control_Text(NumberingWarningIgnore_button, "調査中", default, default);
            InvokeRequired_Control_Enabled(NumberingWarningIgnore_button, true, true);

            InvokeRequired_Control_Enabled(NumberingWebServer_button, false, false);

            NumberingWebServer_button.Image = default;


            // ArcSuite関連表示・ボタン 初期化

            InvokeRequired_Control_Enabled(ArcSuiteInformation_label, false, false);

            InvokeRequired_Control_Text(ArcSuiteWarningIgnore_button, "調査中", default, default);
            InvokeRequired_Control_Enabled(ArcSuiteWarningIgnore_button, true, true);

            InvokeRequired_Control_Enabled(ArcSuiteDrawingShow_button, false, false);

            // ArcSuite関連 Rev比較警告ボタン初期化
            InvokeRequired_Control_Text(SameRevWarningIgnore_button, " -- ", default, default);
            InvokeRequired_Control_Enabled(SameRevWarningIgnore_button, false, false);

            InvokeRequired_Control_Enabled(CommitExecute_Button, false); // コミット開始ボタンを開始直後にディスエイブル

            ArcSuiteDrawingShow_button.Image = default;

            ErrorOccurred_Label.Visible = false; // コミットロック中ラベルを非表示へ

            Variant_panel.Visible = false;
            Variant_panel.Enabled = false;
            PARTNUMBER_CAUTION_label.Text = $"";

        }

        /// <summary>
        /// 値がセット・変更されたら呼び出される
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="cadDrawingFile"></param>
        /// <exception cref="NotImplementedException"></exception>
        private void SupportCadDrawingFile_CadDrawingFileChanged(object sender, CadDrawingFile cadDrawingFile)
        {
            DebugConsole.WriteLine(@"CadDrawingFileChanged イベントがキックされました");

            _cadDrwingFile = cadDrawingFile;

            ActiveDocumentWithoutPathAndExtension = cadDrawingFile.CadDrawingFileNameWithoutExtension;

            if (cadDrawingFile.WarrningButtonEnabled)
            {
                InvokeRequired_Control_Text(CadFileInformation_label, cadDrawingFile.CompareResultMsg, Color.Red);
                InvokeRequired_Control_Text(CadFileWarningIgnore_button, "警告無視", Color.Red, Color.Yellow);
                InvokeRequired_Control_Enabled(CadFileWarningIgnore_button, true, true);
            }
            else
            {
                InvokeRequired_Control_Text(CadFileInformation_label, cadDrawingFile.CompareResultMsg, default);
                InvokeRequired_Control_Enabled(CadFileWarningIgnore_button, false, false);
            }

            CommitButtonEnableJudge();

        }

        /// <summary>
        /// 値がセット・変更されたら呼び出される
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="typeNumber"></param>
        private void SupportNumbering_TypeNumberChanged(Object sender, TypeNumber typeNumber)
        {
            DebugConsole.WriteLine(@"TypeNumberChanged イベントがキックされました");

            _typeNumber = typeNumber;


            // 図面種類をコントロールへ指定
            DrawingTypeTextBox.Text = typeNumber.TypeName;

            if (CanUseVariantTypeDrawing)
            {
                if (typeNumber.isVariant)
                {
                    Variant_panel.Enabled = true;
                    Variant_panel.Visible = true;
                }
                else
                {
                    Variant_panel.Enabled = false;
                    Variant_panel.Visible = false;
                }

            }
            else
            {
                if (typeNumber.isVariant)
                {

                    CommitExecute_Button.Text = "表形式図はコミット非対応";

                    Variant_panel.Enabled = false;
                    Variant_panel.Visible = false;

                    MessageBox.Show("表形式図はコミット操作ができません。\r\n登録担当者による手動スキャニングおよび登録依頼が必要です", "重要");
                }
                else
                {
                    Variant_panel.Enabled = false;
                    Variant_panel.Visible = false;

                }
            }


            CommitButtonEnableJudge();

        }

        /// <summary>
        /// 値がセット・変更されたら呼び出される
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="reserveNumber"></param>
        private void SupportNumbering_ReserveNumberChanged(Object sender, ReserveNumber reserveNumber)
        {
            DebugConsole.WriteLine(@"ReserveNumberChanged イベントがキックされました");

            _reserveNumber = reserveNumber;

            /// 採番システムからの結果を反映させる
            if (supportNumbering.ReserveNumber.CheckAcquiredNumberedNormal == true)
            {
                WriteLine($"■CommitDialogForm.OnEvent_ReserveNumberChanged(..) 採番システム {commitParam.NumberingServerName} との通信は既に成功しています");

                if (supportNumbering.ReserveNumber.NumberingRecordAvailable == false)
                {
                    InvokeRequired_Control_Text(NumberingInformation_label, $"{_reserveNumber.commitTarget_partnumber} の採番情報が見つかりません。採番システムを確認してください.", System.Drawing.Color.Red);
                    InvokeRequired_Control_Text(NumberingWebServer_button, "Web採番システムを開く", default);

                    WriteLine($"■CommitDialogForm.OnEvent_ReserveNumberChanged(..) {_reserveNumber.commitTarget_partnumber}の採番情報は見つかりませんでした。。警告ボタンを表示します\"");
                    InvokeRequired_Control_Text(NumberingWarningIgnore_button, "警告無視", Color.Red, Color.Yellow);
                }
                else
                {
                    InvokeRequired_Control_Enabled(NumberingInformation_label, true, true);
                    InvokeRequired_Control_Text(NumberingInformation_label, supportNumbering.ReserveNumber.CheckNumberdHistoryAnser, default);

                    WriteLine($"■CommitDialogForm.OnEvent_ReserveNumberChanged(..) {_reserveNumber.commitTarget_partnumber}の採番情報は見つかっています。 警告ボタンを非表示にします");
                    InvokeRequired_Control_Enabled(NumberingWarningIgnore_button, false, false);
                }
            }
            else
            {
                MethodInvoker method = () =>
                {

                    InvokeRequired_Control_Text(NumberingInformation_label, "採番システムと通信できません。", System.Drawing.Color.Red);

                    InvokeRequired_Control_Text(NumberingWarningIgnore_button, "警告無視", Color.Red, Color.Yellow);

                    ButtonBackgroundImageWarrningSet(NumberingWebServer_button, "採番システムへ接続不能");  // 検索結果異常時のＵＩをセット


                }; if (InvokeRequired) { Invoke(method); } else { method(); }

            }// ArcSuiteの接続に失敗している場合


            CommitButtonEnableJudge();
        }

        /// <summary>
        /// ArcSuiteControl.ArcSuitePreviewsプロパティに値がセット・変更されたら呼び出される
        /// </summary>
        /// <exception cref="Exception"></exception>
        private void SupportArcSuite_ArcSuiteSearchResultChanged(Object sender, ArcSuiteSearchResult arcSuiteSearchResult)
        {
            DebugConsole.WriteLine(@"ArcSuiteSearchResultChanged イベントがキックされました");

            _arcSuiteSearchResult = arcSuiteSearchResult;

            string commitTarget_partnumber = arcSuiteSearchResult.commitTarget_partnumber; // コミットターゲット図番

            if (arcSuiteSearchResult.arcSuitePreviews != null)
            {

                switch (arcSuiteSearchResult.dESCRIPTION_ComparResult_MessagetypeEnum)
                {
                    case ArcSuiteSearchResult.DESCRIPTION_ComparResult_MessagetypeEnum.ArcSuiteに同番図面なし類番図面もなし:

                        string searchTargetStr = string.Join(", ", arcSuiteSearchResult.searchTargets);// ダブルクォーテーションで各要素を囲んで、カンマでつなげる
                        InvokeRequired_Control_Text(ArcSuiteInformation_label, $"{searchTargetStr} の図面はArcSuiteでは見つかりませんでした。新図とみなされます", default);

                        InvokeRequired_Button_ImageSet(ArcSuiteDrawingShow_button, null);
                        InvokeRequired_Control_Text(ArcSuiteDrawingShow_button, "", default);
                        InvokeRequired_Control_Enabled(ArcSuiteDrawingShow_button, false, false);// ArcSuite検索結果表示ボタンをディスエイブル

                        InvokeRequired_Control_Enabled(ArcSuiteWarningIgnore_button, false, false);// ArcSuite検索結果無視ボタンをディスエイブル

                        break;

                    case ArcSuiteSearchResult.DESCRIPTION_ComparResult_MessagetypeEnum.検索結果複数ArcSuiteに同番図面あり類番図面あり:
                        InvokeRequired_Control_Text(ArcSuiteInformation_label, $"【危険】ArcSuite検索結果複数・ {commitTarget_partnumber} が見つかりましたが類番図面も存在します！！", System.Drawing.Color.Red);

                        InvokeRequired_Button_ImageSet(ArcSuiteDrawingShow_button, Resources.エマージェンシーバックグラウンド);
                        InvokeRequired_Control_Text(ArcSuiteDrawingShow_button, "ｸﾘｯｸして類番を確認", Color.Red);
                        InvokeRequired_Control_Enabled(ArcSuiteDrawingShow_button, true, true); // アークスイート結果表示ボタンイネーブル

                        InvokeRequired_Control_Text(ArcSuiteWarningIgnore_button, "警告無視不可", System.Drawing.Color.Red);
                        InvokeRequired_Control_Enabled(ArcSuiteWarningIgnore_button, false, true);// ArcSuite検索結果無視ボタンをイネーブル

                        break;

                    case ArcSuiteSearchResult.DESCRIPTION_ComparResult_MessagetypeEnum.検索結果１件ArcSuiteに同番図面有り表題がはどちらも無し:
                        InvokeRequired_Control_Text(ArcSuiteInformation_label, $"【注意】ArcSuite検索結果１件・ {commitTarget_partnumber} が見つかりました。名称・説明が無いためこれ以上の照査は不可能です！！", System.Drawing.Color.Red);

                        InvokeRequired_Button_ImageSet(ArcSuiteDrawingShow_button, null);
                        InvokeRequired_Control_Text(ArcSuiteDrawingShow_button, "ｸﾘｯｸしてArcuSuiteの\r\n図面を表示", Color.Red);
                        InvokeRequired_Control_Enabled(ArcSuiteDrawingShow_button, true, true); // アークスイート結果表示ボタンイネーブル

                        InvokeRequired_Control_Text(ArcSuiteWarningIgnore_button, "警告無視", System.Drawing.Color.Red);
                        InvokeRequired_Control_Enabled(ArcSuiteWarningIgnore_button, true, true);// ArcSuite検索結果無視ボタンをイネーブル
                        break;

                    case ArcSuiteSearchResult.DESCRIPTION_ComparResult_MessagetypeEnum.検索結果１件ArcSuiteに同番図面有り表題は合致:
                        InvokeRequired_Control_Text(ArcSuiteInformation_label, $"【注意】ArcSuite検索結果１件・ {commitTarget_partnumber} が見つかりました。『表題は一致しています』。取替図ですか？", System.Drawing.Color.Red);
                        InvokeRequired_Control_Tag(ArcSuiteInformation_label, "取替図であると指示しました");

                        InvokeRequired_Button_ImageSet(ArcSuiteDrawingShow_button, null);
                        InvokeRequired_Control_Text(ArcSuiteDrawingShow_button, "ｸﾘｯｸしてArcuSuiteの\r\n図面を表示", Color.Red);
                        InvokeRequired_Control_Enabled(ArcSuiteDrawingShow_button, true, true); // アークスイート結果表示ボタンイネーブル

                        InvokeRequired_Control_Text(ArcSuiteWarningIgnore_button, "取替図である", Color.Red, Color.Yellow);
                        InvokeRequired_Control_Enabled(ArcSuiteWarningIgnore_button, true, true);// ArcSuite検索結果無視ボタンをイネーブル
                        break;

                    case ArcSuiteSearchResult.DESCRIPTION_ComparResult_MessagetypeEnum.検索結果１件ArcSuiteに同番図面有り表題は相違:
                        InvokeRequired_Control_Text(ArcSuiteInformation_label, $"【危険】ArcSuite検索結果１件・ {commitTarget_partnumber} が見つかりました。しかし名称・説明が違います！！", System.Drawing.Color.Red);

                        InvokeRequired_Button_ImageSet(ArcSuiteDrawingShow_button, Resources.エマージェンシーバックグラウンド);
                        InvokeRequired_Control_Text(ArcSuiteDrawingShow_button, "ｸﾘｯｸしてArcuSuiteの\r\n図面を表示", Color.Red);
                        InvokeRequired_Control_Enabled(ArcSuiteDrawingShow_button, true, true); // アークスイート結果表示ボタンイネーブル

                        InvokeRequired_Control_Text(ArcSuiteWarningIgnore_button, "警告無視", System.Drawing.Color.Red);
                        InvokeRequired_Control_Enabled(ArcSuiteWarningIgnore_button, true, true);// ArcSuite検索結果無視ボタンをイネーブル
                        break;

                    case ArcSuiteSearchResult.DESCRIPTION_ComparResult_MessagetypeEnum.検索結果１件ArcSuiteに同番図面有り表題がArcSuite側に無し:
                        InvokeRequired_Control_Text(ArcSuiteInformation_label, $"【注意】ArcSuite検索結果１件・ {commitTarget_partnumber} が見つかりました。『ArcSuite側に表題はありません』。取替図か確認してください", System.Drawing.Color.Red);

                        InvokeRequired_Button_ImageSet(ArcSuiteDrawingShow_button, null);
                        InvokeRequired_Control_Text(ArcSuiteDrawingShow_button, "ｸﾘｯｸしてArcuSuiteの\r\n図面を表示", Color.Red);
                        InvokeRequired_Control_Enabled(ArcSuiteDrawingShow_button, true, true); // アークスイート結果表示ボタンイネーブル

                        InvokeRequired_Control_Text(ArcSuiteWarningIgnore_button, "警告無視", System.Drawing.Color.Red);
                        InvokeRequired_Control_Enabled(ArcSuiteWarningIgnore_button, true, true);
                        break;

                    case ArcSuiteSearchResult.DESCRIPTION_ComparResult_MessagetypeEnum.検索結果１件ArcSuiteに同番図面有り表題がCAD側に無し:
                        InvokeRequired_Control_Text(ArcSuiteInformation_label, $"【注意】ArcSuite検索結果１件・ {commitTarget_partnumber} が見つかりました。『CAD側に表題はありません』。取替図か確認してください", System.Drawing.Color.Red);

                        InvokeRequired_Button_ImageSet(ArcSuiteDrawingShow_button, null);
                        InvokeRequired_Control_Text(ArcSuiteDrawingShow_button, "ｸﾘｯｸしてArcuSuiteの\r\n図面を表示", Color.Red);
                        InvokeRequired_Control_Enabled(ArcSuiteDrawingShow_button, true, true); // アークスイート結果表示ボタンイネーブル

                        InvokeRequired_Control_Text(ArcSuiteWarningIgnore_button, "警告無視", System.Drawing.Color.Red);
                        InvokeRequired_Control_Enabled(ArcSuiteWarningIgnore_button, true, true);
                        break;

                    case ArcSuiteSearchResult.DESCRIPTION_ComparResult_MessagetypeEnum.検索結果１件ArcSuiteに同番図面なし類番図面あり:
                        string hit_zubans = string.Join(" , ", _arcSuiteSearchResult.arcSuitePreviews.Select(item => $"\"{item.user_zuban}\""));
                        InvokeRequired_Control_Text(ArcSuiteInformation_label, $"ArcSuiteに {commitTarget_partnumber} は見つかりませんが、類番検索の結果 {hit_zubans} が存在します コミットは継続できません", System.Drawing.Color.Red);

                        InvokeRequired_Button_ImageSet(ArcSuiteDrawingShow_button, null);
                        InvokeRequired_Control_Text(ArcSuiteDrawingShow_button, "ｸﾘｯｸしてArcuSuiteの\r\n図面を検索", Color.Red);
                        InvokeRequired_Control_Enabled(ArcSuiteDrawingShow_button, true, true);// ArcSuite検索結果表示ボタンをイネーブル

                        InvokeRequired_Control_Text(ArcSuiteWarningIgnore_button, $"警告無視不可", System.Drawing.Color.Red);
                        InvokeRequired_Control_Enabled(ArcSuiteWarningIgnore_button, false, true);// ArcSuite検索結果表示ボタンをイネーブル




                        break;
                    default:
                        InvokeRequired_Control_Enabled(ArcSuiteDrawingShow_button, false);// ArcSuite検索結果表示ボタンをディスエイブル

                        InvokeRequired_Control_Enabled(ArcSuiteWarningIgnore_button, true, true);// ArcSuite検索結果無視ボタンをイネーブル

                        throw new Exception($"arcSuitePreviews.Count がシステム範囲外 {arcSuiteSearchResult.arcSuitePreviews.Count}");

                }

            }
            else
            {
                MethodInvoker method = () =>
                {
                    InvokeRequired_Control_Enabled(ArcSuiteDrawingShow_button, false);// ArcSuite検索結果表示ボタンをディスエイブル

                    InvokeRequired_Control_Text(ArcSuiteInformation_label, "ArcSuiteに接続できません。", System.Drawing.Color.Pink);

                    InvokeRequired_Control_Text(ArcSuiteWarningIgnore_button, "警告無視", System.Drawing.Color.Red);

                    ButtonBackgroundImageWarrningSet(ArcSuiteDrawingShow_button, "ArcSuiteへ接続不能\r\n図面の有無は確認できません");  // 検索結果異常時のＵＩをセット


                }; if (InvokeRequired) { Invoke(method); } else { method(); }

            }// ArcSuiteの接続に失敗している場合

            CommitButtonEnableJudge();
        }

        /// <summary>
        /// ■コミットボタンを警告表示に合わせてディスエイブル・イネーブル
        /// </summary>
        private void CommitButtonEnableJudge()
        {
            if (_cadDrwingFile == null || _typeNumber == null || _reserveNumber == null || _arcSuiteSearchResult == null)
                return;

            if (CadFileWarningIgnore_button.Visible || NumberingWarningIgnore_button.Visible || ArcSuiteWarningIgnore_button.Visible || SameRevWarningIgnore_button.Visible)
                InvokeRequired_Control_Enabled(CommitExecute_Button, false);
            else
                InvokeRequired_Control_Enabled(CommitExecute_Button, true);

            if (
                (CadFileWarningIgnore_button.Visible && CadFileWarningIgnore_button.Enabled) ||
                (NumberingWarningIgnore_button.Visible && NumberingWarningIgnore_button.Enabled) ||
                (ArcSuiteWarningIgnore_button.Visible && ArcSuiteWarningIgnore_button.Enabled)
                )
                InvokeRequired_Control_Text(ErrorOccurred_Label, "警告無視を選択するか、警告に従って対応しない限りコミットできません", Color.Red, Color.Yellow);
            else if ((NumberingWarningIgnore_button.Visible == true && NumberingWarningIgnore_button.Enabled == false) || (ArcSuiteWarningIgnore_button.Visible == true && ArcSuiteWarningIgnore_button.Enabled == false))
                InvokeRequired_Control_Text(ErrorOccurred_Label, "このウィンドウをキャンセルし、警告を処理しない限りコミットできません", Color.Red, Color.Yellow);
            else
                InvokeRequired_Control_Text(ErrorOccurred_Label, "", default, default);

        }

        /// <summary>
        /// CommitDialogForm フォームロード時
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CommitDialogForm_Load(object sender, EventArgs e)
        {
            if (PrintOutOnly == false)
            {
                ArcSuiteDrawingShow_button.Enabled = false;

                NumberingWarningIgnore_button.Enabled = false;

                ArcSuiteWarningIgnore_button.Enabled = false;

                if (supportCadDrawingFile != null)
                    supportCadDrawingFile.CadDrwingFileChanged += SupportCadDrawingFile_CadDrawingFileChanged;

                if (supportNumbering != null)
                {
                    supportNumbering.ReserveNumberChanged += SupportNumbering_ReserveNumberChanged; // ｲﾍﾞﾝﾄはこの後にキックされることを確認すること
                    supportNumbering.TypeNumberChanged += SupportNumbering_TypeNumberChanged; // ｲﾍﾞﾝﾄはこの後にキックされることを確認すること

                }

                if (supportArcSuite != null)
                    supportArcSuite.ArcSuiteSearchResultChanged += SupportArcSuite_ArcSuiteSearchResultChanged; // ｲﾍﾞﾝﾄはこの後にキックされることを確認すること


                #region 採番チェックおよびArcSuite登録済みかのチェック。この指令後にプロパティチェンジｲﾍﾞﾝﾄはキックされる

                CancellationToken ct = cts.Token;

                supportCadDrawingFile.CheckCadFileName(PARTNUMBER_linklabel.Text, this);

                supportNumbering.CheckReserveNumber(PARTNUMBER_linklabel.Text, this);

                var resultCheckNumberType = supportNumbering.CheckNumberType(PARTNUMBER_linklabel.Text, this);
                if (resultCheckNumberType)
                {
                    WriteLine($"■CommitDialogForm.CommitDialogForm_Load(..)　PARTNUMBER_linklabel.Text = 【{PARTNUMBER_linklabel.Text}】の図面種類および表図面か否かをチェックしました。.結果 NumberTypeConfig.drawingType = {supportNumbering.TypeNumber.drawingType}");

                    // アークスイート登録確認開始
                    supportArcSuite.CheckArcSuiteRegisted(PARTNUMBER_linklabel.Text, TITLE_Label_label.Text, ArcSuitePARTNUMBER_ContainSuffixs, ct);

                    #endregion

                    CommitButtonEnableJudge();

                }
                else
                {
                    this.Close();
                }
            }
            else
            {
                CADTITLE_groupBox.Enabled = false;
                CadFileInformation_groupBox.Enabled = false;
                NUMBERING_groupBox.Enabled = false;
                ArcSuite_groupBox.Enabled = false;
                CommitExecute_Button.Enabled = true;
                CommitExecute_Button.Text = "ﾊﾞｰｺｰﾄﾞ無し印刷開始";
            }
        }



        /// <summary>
        /// CommitDialogForm フォームが閉じる時
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CommitDialogForm_FormClosing(object sender, FormClosingEventArgs e)
        {

            if (!close_permition)
            {
                close_permition = true;
                e.Cancel = true;
            }
        }

        /// <summary>
        /// フォームが閉じるたび、フォームが閉じた後
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CommitDialogForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (_arcSuiteSearchResult != null && _arcSuiteSearchResult.arcSuitePreviews != null)
            {

                if (string.IsNullOrWhiteSpace(ArcsuitePreview.temporalyDrawingImageFullFileName) == false)
                {
                    string fullfilename = ArcsuitePreview.temporalyDrawingImageFullFileName;
                    if (System.IO.Directory.Exists(System.IO.Path.GetDirectoryName(fullfilename)))
                    {
                        bool result = SasaLib.FileFolder.RemoveFolder(System.IO.Path.GetDirectoryName(fullfilename), true);
                        if (result)
                            WriteLine($"■ｱｰｸｽｲｰﾄﾌﾟﾚﾋﾞｭｰｷｬｯｼｭﾌｫﾙﾀﾞ {System.IO.Path.GetDirectoryName(fullfilename)}の削除に成功しました");
                        else
                            WriteLine($"※ｱｰｸｽｲｰﾄﾌﾟﾚﾋﾞｭｰｷｬｯｼｭﾌｫﾙﾀﾞ {System.IO.Path.GetDirectoryName(fullfilename)}の削除に失敗しました");
                    }
                    else
                        WriteLine($"※ｱｰｸｽｲｰﾄﾌﾟﾚﾋﾞｭｰｷｬｯｼｭﾌｫﾙﾀﾞ {System.IO.Path.GetDirectoryName(fullfilename)}はすでにありませんでした。");
                }

            }
        }

        /// <summary>
        /// ■コミットするCADイメージをダイアログへロード
        /// </summary>
        /// <param name="FullImagePath"></param>
        internal void SetImage(string FullImagePath)
        {
            //MiniPreviewPictureBox.Load(FullImagePath);
            //WriteLine($"画像ﾌｧｲﾙ{FullImagePath}をPicutreBoxへロード。 DPI:{MiniPreviewPictureBox.Image.HorizontalResolution}");

            commitPreviewImage.LoadImage(FullImagePath);
            WriteLine($"■CommitDialogForm.SetImage(..) 画像ﾌｧｲﾙ \"{FullImagePath}\" を CommitPreviewImageへロード。 DPI:{commitPreviewImage.Image.HorizontalResolution}");

            ImageRezolutonInfo_label.Text = $"印刷イメージ：解像度 {commitPreviewImage.OrignalResolution}[DPI] (推奨値400[DPI])";

            commitPreviewImage.TitleBlockFit();
            TitleFit_button.Tag = true;
        }

        /// <summary>
        /// ■チケットデータをダイアログへセット
        /// </summary>
        /// <param name="ticketXml"></param>
        public bool SetData(CommonTicket ticketXml, out string ErrMsg)
        {
            this.ticketXml = ticketXml;

            bool result = true;
            ErrMsg = null;

            StageServerHostLabel.Text = commitParam.StageServerHost;



            //用紙種別
            PaperSizeLabel.Text = ticketXml.PAPERSIZE;
            toolTip1.SetToolTip(PaperSizeLabel, $"印刷レイアウトにおける用紙サイズと向き:{ticketXml.PLOTPAPERSIZE}");

            //チケットコード
            TicketCodeLinkLabel.Text = ticketXml.TICKETCODE;

            // 材質 材質コード
            MATERIALlabel.Text = $"{(string)ticketXml.GetParamKeyValue("MATERIAL")} , {(string)ticketXml.GetParamKeyValue("MATERIALCODE")}";

            // 機種
            MachineTypelabel.Text = (string)ticketXml.GetParamKeyValue("MACHINETYPE");

            // 顧客
            string firstcustomer = (string)ticketXml.GetParamKeyValue("FIRSTCUSTOMER");
            string CUSTOMER = (string)ticketXml.GetParamKeyValue("CUSTOMER");

            if (string.IsNullOrWhiteSpace(firstcustomer) == false)
                lFIRSTCUSTOMERlabe.Text = firstcustomer;
            if (string.IsNullOrWhiteSpace(CUSTOMER) == false)
                lFIRSTCUSTOMERlabe.Text = CUSTOMER;

            // 表題
            TITLE_Label_label.Text = (string)ticketXml.GetParamKeyValue("TITLE");

            // 図面番号
            PARTNUMBER_linklabel.Text = (string)ticketXml.GetParamKeyValue("PARTNUMBER");

            // 改定番号
            REVNUMBERtextBox.Text = (string)ticketXml.GetParamKeyValue("REV");

            // 製図者
            AUTHORlabel.Text = (string)ticketXml.GetParamKeyValue("AUTHOR");
            if (string.IsNullOrWhiteSpace(AUTHORlabel.Text) == false)
            {
                CultureInfo provider = CultureInfo.CurrentCulture;
                // 製図日
                string strAUTHORDATE = (string)ticketXml.GetParamKeyValue("AUTHORDATE");
                try
                {
                    DateTime authorDatetime;
                    bool result1 = StringUtil.TryParseExactMultiple_CurrentCulture(strAUTHORDATE, new List<string> { "yyyy/MM/dd", "yyyy/MM/dd[ddd] H:mm:ss" }.ToArray(), out authorDatetime);
                    if (result)
                        AUTHORDATElabel.Text = authorDatetime.ToString("yyyy/MM/dd");
                    else
                        throw new Exception();
                }
                catch (Exception ex)
                {
                    ErrMsg = $"製図日の日付フォーマットに異常があります -> \"{strAUTHORDATE}\" {ex.Message} {ex.InnerException}";
                    result = false;
                }
            }
            // 設計者
            DESIGNERlabel.Text = (string)ticketXml.GetParamKeyValue("DESIGNER");
            // 設計日
            if (string.IsNullOrWhiteSpace(DESIGNERlabel.Text) == false)
            {
                string strCHECKDATE = (string)ticketXml.GetParamKeyValue("CHECKDATE");
                try
                {
                    DateTime checkDatetime;
                    bool result1 = StringUtil.TryParseExactMultiple_CurrentCulture(strCHECKDATE, new List<string> { "yyyy/MM/dd", "yyyy/MM/dd[ddd] H:mm:ss" }.ToArray(), out checkDatetime);
                    if (result)
                        CHECKDATElabel.Text = checkDatetime.ToString("yyyy/MM/dd");
                    else
                        throw new Exception();
                }
                catch (Exception ex)
                {
                    ErrMsg = $"設計日の日付フォーマットに異常があります -> \"{strCHECKDATE}\" {ex.Message} {ex.InnerException}";
                    result = false;
                }
            }


            if (commitParam.PrinterDriverName != "")
            {
                PlotFileCreate_label.Text = $"出力用紙ｻｲｽﾞと方向の設定は, ﾌﾟﾘﾝﾀ【{commitParam.PrinterDriverName}】のﾍﾟｰｼﾞ設定にて決まります";
            }
            else if (commitParam.PC3FileName != "")
            {
                PlotFileCreate_label.Text = $"出力用紙ｻｲｽﾞと方向の設定は, ﾌﾟﾘﾝﾀ【{commitParam.PC3FileName}】のﾍﾟｰｼﾞ設定にて決まります";

            }
            else
            {
                PlotFileCreate_label.Text = $"";
            }

            return result;
        }

        /// <summary>
        /// ●チケットコードをｸﾘｯﾌﾟﾎﾞｰﾄﾞへ
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TicketCodeLinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            /// ｸﾘｯﾌﾟﾎﾞｰﾄﾞにチケット名をコピー
            Clipboard.SetText(TicketCodeLinkLabel.Text);
            MessageBox.Show("チケットコードをｸﾘｯﾌﾟﾎﾞｰﾄﾞにコピーしました");
        }

        /// <summary>
        /// ●図面番号をｸﾘｯﾌﾟﾎﾞｰﾄﾞへ
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PARTNUMBER_linklabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Clipboard.SetText(PARTNUMBER_linklabel.Text);
            MessageBox.Show($"図面番号 \"{PARTNUMBER_linklabel.Text}\"をｸﾘｯﾌﾟﾎﾞｰﾄﾞにコピーしました");
        }

        /// <summary>
        /// ●CADファイルに関する警告を無視するボタン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CadFileWarningIgnore_button_Click(object sender, EventArgs e)
        {
            InvokeRequired_Control_Enabled(CadFileWarningIgnore_button, false, false);

            if (string.IsNullOrWhiteSpace((string)CadFileInformation_label.Tag))
                InvokeRequired_Control_Text(CadFileInformation_label, CadFileInformation_label.Text + $" →  警告無視を選択しました", Color.BlueViolet);
            else
                InvokeRequired_Control_Text(CadFileInformation_label, CadFileInformation_label.Text + $" → {CadFileInformation_label.Tag}", Color.BlueViolet);

            WriteLine($"※【重要】\"{PARTNUMBER_linklabel.Text}\"  (\"{ticketXml.TICKETCODE}\") CADファイルのエラーを無視するボタンが押されました。この時のエラー情報:\"{CadFileInformation_label.Text}\"");

            CommitButtonEnableJudge();

        }

        /// <summary>
        /// ●採番システムに関する警告を無視するボタン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void NumberingWarningIgnore_button_Click(object sender, EventArgs e)
        {
            InvokeRequired_Control_Enabled(NumberingWarningIgnore_button, false, false);

            if (string.IsNullOrWhiteSpace((string)NumberingInformation_label.Tag))
                InvokeRequired_Control_Text(NumberingInformation_label, NumberingInformation_label.Text + $" →  警告無視を選択しました", Color.BlueViolet);
            else
                InvokeRequired_Control_Text(NumberingInformation_label, NumberingInformation_label.Text + $" → {NumberingInformation_label.Tag}", Color.BlueViolet);

            WriteLine($"※【重要】\"{PARTNUMBER_linklabel.Text}\"  (\"{ticketXml.TICKETCODE}\") 採番時のエラーを無視するボタンが押されました。この時のエラー情報:\"{NumberingInformation_label.Text}\"");

            CommitButtonEnableJudge();

        }

        /// <summary>
        /// ●ArcSuiteシステムに関する警告を無視するボタン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ArcSuiteWarningIgnore_button_Click(object sender, EventArgs e)
        {

            InvokeRequired_Control_Enabled(ArcSuiteWarningIgnore_button, false, false);

            if (string.IsNullOrWhiteSpace((string)ArcSuiteInformation_label.Tag))
                InvokeRequired_Control_Text(ArcSuiteInformation_label, ArcSuiteInformation_label.Text + $" →  警告無視を選択しました", Color.BlueViolet);
            else
                InvokeRequired_Control_Text(ArcSuiteInformation_label, ArcSuiteInformation_label.Text + $" → {ArcSuiteInformation_label.Tag}", Color.BlueViolet);

            WriteLine($"※【重要】\"{PARTNUMBER_linklabel.Text}\"  (\"{ticketXml.TICKETCODE}\") ArcSuiteに関するエラーを無視するボタンが押されました。この時のエラー情報:\"{ArcSuiteInformation_label.Text}\"");

            CheckRevNumberForUpdate();

            CommitButtonEnableJudge();

        }

        /// <summary>
        /// ●ArcSuiteの既存最新図の表題欄ﾘﾋﾞｼﾞｮﾝ番号と同じ表題欄ﾘﾋﾘﾋﾞｼﾞｮﾝ番号でコミットされた警告を無視するボタン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SameRevWarningIgnore_button_Click(object sender, EventArgs e)
        {
            InvokeRequired_Control_Enabled(SameRevWarningIgnore_button, false, false);

            if (string.IsNullOrWhiteSpace(ArcSuiteInformation_label.Text))
                InvokeRequired_Control_Text(ArcSuiteInformation_label, ArcSuiteInformation_label.Text + $" →  Rev番号が同じでも再コミットすることを指示しました", Color.BlueViolet);
            else
                InvokeRequired_Control_Text(ArcSuiteInformation_label, $"表題欄Rev番号についての警告を無視すると指示されました", Color.BlueViolet);

            WriteLine($"※【重要】\"{PARTNUMBER_linklabel.Text}\"  (\"{ticketXml.TICKETCODE}\") ArcSuiteの既存最新図の表題欄ﾘﾋﾞｼﾞｮﾝ番号と同じ表題欄ﾘﾋﾘﾋﾞｼﾞｮﾝ番号でコミットされたことへの警告ボタンがを無視するように指示されました。この時のエラー情報:\"{ArcSuiteInformation_label.Text}\"");

            CommitButtonEnableJudge();

        }

        /// <summary>
        /// ●採番サーバー（ウェブ版）を開くボタン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void NumberingWebServer_button_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(PARTNUMBER_linklabel.Text)) return;

            try
            {
                NumberingSupport.DrawingTypeEnum drawingTypeEnum;
                string NumberingUpdateAddress = null;
                string numberingString = NumberingSupport.RemoveSuffixNumber(PARTNUMBER_linklabel.Text, out drawingTypeEnum);
                string NumberingServerWebAddr = commitParam.NumberingServerWebAddr;

                NumberTypeConfig.DrawingType drawingType;
                bool isVariant = true;
                string suffixMIN = null;
                string suffixMAX = null;
                string DRAWINGTYPEMSG;
                if (ToyoDrawingTypeClassify.CheckNumber(numberingString, out drawingType, ref isVariant, ref suffixMIN, ref suffixMAX, out DRAWINGTYPEMSG) == false)
                {
                    MessageBox.Show($"※ｺﾐｯﾄ受付ｻｰﾊﾞｰ{commitParam.StageServerHost}からの警告\n図面番号{PARTNUMBER_linklabel.Text}は対応していない図番形式です。図面種類が定まりませんでした");
                    WriteLine($"※ｺﾐｯﾄ受付ｻｰﾊﾞｰ{commitParam.StageServerHost}からの警告\n図面番号{PARTNUMBER_linklabel.Text}は対応していない図番形式です。図面種類が定まりませんでした");
                }

                if (numberingString != null)
                {
                    if (drawingTypeEnum == NumberingSupport.DrawingTypeEnum.Part)
                    {
                        NumberingUpdateAddress = commitParam.NumberingPartDrawingUpdateAddress.Replace("{NUMBERINGWEBADD}", NumberingServerWebAddr).Replace("{SEARCHNUMBER}", numberingString);
                    }
                    else if (drawingTypeEnum == NumberingSupport.DrawingTypeEnum.Assy)
                    {
                        NumberingUpdateAddress = commitParam.NumberingAssyDrawingUpdateAddress.Replace("{NUMBERINGWEBADD}", NumberingServerWebAddr).Replace("{SEARCHNUMBER}", numberingString);
                    }
                    else if (drawingTypeEnum == NumberingSupport.DrawingTypeEnum.Layout)
                    {
                        NumberingUpdateAddress = commitParam.NumberingLayoutDrawingUpdateAddress.Replace("{NUMBERINGWEBADD}", NumberingServerWebAddr).Replace("{SEARCHNUMBER}", numberingString);
                    }
                    else if (drawingTypeEnum == NumberingSupport.DrawingTypeEnum.Other)
                    {

                    }
                    if (string.IsNullOrWhiteSpace(NumberingUpdateAddress) == false)
                    {
                        System.Diagnostics.Process.Start(NumberingUpdateAddress);
                    }
                }

            }
            catch (Exception ex)
            {
                WriteLine($"Web採番システムオープン時に例外検知 {ex.Message} {ex.InnerException}");
            }
        }

        /// <summary>
        /// ●登録済みアークスイート最新図・検索表示ボタン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void SearchRegistedArcSuiteDrawing_button_Click(object sender, EventArgs e)
        {
            if (_arcSuiteSearchResult.arcSuitePreviews != null)
            {
                if (_arcSuiteSearchResult.arcSuitePreviews.Count == 1)
                {
                    if (ArcsuitePreview.Normality == false)
                    {
                        MessageBox.Show("ArcSuteに接続出来ていないか、接続の途中です", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    ArcSuiteDrawingShow_button.Enabled = false; // ArcSuiteプレビュー表示ボタンを一時無効化

                    if (ArcsuitePreview.Found == true)
                    {

                        // ArcSuiteに問い合わせるURL
                        string ArcSuiteDrawingSearchURL = commitParam.ArcSuiteSearchURL.Replace("{SANITIZEDPARTNUMBER}", ArcsuitePreview.user_zuban);

                        // コミット対象のCADファイル
                        string activeDocumentFullFileName = AtiveDocFullFilename;

                        if (commitParam.ArcSuiteDrawingDownloadMode)
                        {
                            ///
                            ArcSuitePreviewOnlyForm arcSuitePreviewOnlyForm = null;

                            // ボタン文字列退避
                            string SearchRegistedArcSuiteDrawing_button_Text = ArcSuiteDrawingShow_button.Text;

                            ArcSuiteDrawingShow_button.Text = "ArcSuiteへ図面検索中\r\n暫くお待ちださい・・・";

                            // 一度ダウンロードした状態かを確認。ない場合は取出し

                            Encryption sasaLibencryptionPipeConnection = new Encryption("SasaAuth3.1");
                            string PipeClientPlanePass = sasaLibencryptionPipeConnection.Decoding(commitParam.PipeConnection31Password); //復号化

                            await Task.Run(() =>
                            {
                                if (string.IsNullOrWhiteSpace(ArcsuitePreview.temporalyDrawingImageFullFileName) == true ||
                                System.IO.File.Exists(ArcsuitePreview.temporalyDrawingImageFullFileName) == false)
                                {
                                    WriteLine($"■CommitDialogForm.SearchRegistedArcSuiteDrawing_button_Click(..) ArcSuite登録図ﾌﾟﾚﾋﾞｭｰ用TIFFファイルをダウンロードします");
                                    /// リモート操作をするオブジェクトを生成
                                    RemoteClientDRAWREGIST remoteClientDR = new RemoteClientDRAWREGIST(
                                        commitParam.ClientDomainName,
                                        commitParam.ClientUserName,
                                        PipeClientPlanePass,
                                        commitParam.ClsLogon,
                                        commitParam.StageServerHost,
                                        commitParam.PipeNameDR
                                        );

                                    ArcsuitePreview originalStruct = ArcsuitePreview;

                                    var resule = CheckArcSuiteData.GetArcSuiteImagePipe(remoteClientDR, ArcsuitePreview.user_zuban, out originalStruct.temporalyDrawingImageFullFileName, WriteLine);
                                    _arcSuiteSearchResult.arcSuitePreviews[0] = originalStruct;
                                }
                                else
                                {
                                    WriteLine($"■CommitDialogForm.SearchRegistedArcSuiteDrawing_button_Click(..) ArcSuite登録図ﾌﾟﾚﾋﾞｭｰ用TIFFファイルはすでに取得済みでした。{ArcsuitePreview.temporalyDrawingImageFullFileName} ");
                                }
                            });


                            Encryption sasaLibencryptionArcSuite = new Encryption("SasaAuth3.1");
                            string ArcSuiteUserPlanePass = sasaLibencryptionArcSuite.Decoding(commitParam.ArcSuiteCrypt31UserPass); //復号化

                            //テンポラリﾌｫﾙﾀﾞに表示すべきﾌｧｲﾙがあるか確認
                            if (System.IO.File.Exists(ArcsuitePreview.temporalyDrawingImageFullFileName) == true)
                            {
                                arcSuitePreviewOnlyForm = new ArcSuitePreviewOnlyForm(this, WriteLine);
                                arcSuitePreviewOnlyForm.Text = $"ｱｰｸｽｲｰﾄに登録済みの図面 {ArcsuitePreview.user_zuban} {ArcsuitePreview.createdOnMessage}";
                                arcSuitePreviewOnlyForm.PreviewSet(ArcsuitePreview, ArcSuiteDrawingSearchURL, activeDocumentFullFileName);
                                arcSuitePreviewOnlyForm.MessageSet("");
                                arcSuitePreviewOnlyForm.SetUnsetCadTypeFlagControlDatas(
                                        commitParam.StageServerHost,
                                        commitParam.PipeNameDR,
                                        commitParam.ClientDomainName,
                                        commitParam.ClientUserName,
                                        PipeClientPlanePass,
                                        commitParam.ClsLogon,
                                        RemoteClientCADtype.CadType.InventorModel,
                                        commitParam.ArcSuiteUserName,
                                        ArcSuiteUserPlanePass
                                );

                                arcSuitePreviewOnlyForm.ShowDialog(this);
                            }
                            else
                            {
                                WriteLine($"※CommitDialogForm.SearchRegistedArcSuiteDrawing_button_Click(..) ｱｰｸｽｲｰﾄ問合せ後の temporalyDrawingImageFullFileName がnullまたは空白にもかかわらずﾌﾟﾚﾋﾞｭｰしようとしました");
                            }



                        } //  Config.ArcSuiteDrawingDownloadMode が true の場合
                        else
                        {
                            if (string.IsNullOrWhiteSpace(ArcsuitePreview.createdOnMessage) != true)
                            {
                                //string ArcSuiteURL = Config.ArcSuiteSearchAndContentOpenURL.Replace("{SANITIZEDPARTNUMBER}", CommitDialogForm.stArcSuitePreview.user_zuban);
                                string ArcSuiteURL = commitParam.ArcSuiteSearchAndContentOpenURL.Replace("{SANITIZEDPARTNUMBER}", ArcsuitePreview.user_zuban);
                                System.Diagnostics.Process.Start(ArcSuiteURL);
                            }
                        } // WebでArcSuite図面検索
                    }
                    else
                    {
                        //InventorAddInServer.logSystem.WriteLine($"■ｱｰｸｽｲｰﾄ問合せ結果を受信する前か、同名図面が存在しないのに表示ボタンが押されました CommitDialogForm.stArcSuitePreview.Found={CommitDialogForm.stArcSuitePreview.Found}");
                        WriteLine($"※CommitDialogForm.SearchRegistedArcSuiteDrawing_button_Click(..) ｱｰｸｽｲｰﾄ問合せ結果を受信する前か、同名図面が存在しないのに表示ボタンが押されました CommitDialogForm.stArcSuitePreview.Found={ArcsuitePreview.Found}");
                    }

                    ArcSuiteDrawingShow_button.Enabled = true; // ArcSuiteプレビュー表示ボタンを有効化

                }
                else if (_arcSuiteSearchResult.arcSuitePreviews.Count > 1)
                {
                    //                     string suffixsStr = string.Join(", ", suffixs.Select(item => $"\"{item}\""));

                    string hit_zubans = string.Join(" , ", _arcSuiteSearchResult.arcSuitePreviews.Select(item => $"\"{item.user_zuban}\""));
                    MessageBox.Show($"類番の図面が複数ありますコミット操作は許可されません。\n{hit_zubans}\nArcSuiteを確認してください", "対処が必要です", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

        }

        /// <summary>
        /// ●コミット実行ボタン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CommitExecute_Button_Click(object sender, EventArgs e)
        {
            if (CanUseVariantTypeDrawing)
            {
                if (ticketXml.Variants != null && ticketXml.Variants.Count > 0)
                {
                    if (string.IsNullOrWhiteSpace(ActiveVariantEnd_textBox.Text))
                    {
                        close_permition = false;

                        DialogResult dialogResult = MessageBox.Show("※表図面の場合は有効な最大枝番の指定が必要です", $"", MessageBoxButtons.YesNo);

                        this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
                        return;
                    }
                    else
                    {
                        //ticketXml.Variants;
                        List<CommonTicket.Variant> newVariants = new List<CommonTicket.Variant>();
                        foreach (CommonTicket.Variant variant in ticketXml.Variants)
                        {
                            string prefix;
                            string rangePart;
                            string rangeStart;
                            string rangeEnd;
                            string suffix;

                            helper.ParseString2(variant.PRARTNUMBER, out prefix, out rangePart, out rangeStart, out rangeEnd, out suffix);

                            int number;
                            bool success = int.TryParse(rangeStart, out number);

                            int number2;
                            bool success2 = int.TryParse(ActiveVariantEnd_textBox.Text, out number2);

                            if (success && success2)
                            {
                                // 変換成功した場合の処理
                                if (number <= number2)
                                {
                                    newVariants.Add(new CommonTicket.Variant { PRARTNUMBER = variant.PRARTNUMBER, IsActive = true });
                                }
                                else
                                {
                                    newVariants.Add(new CommonTicket.Variant { PRARTNUMBER = variant.PRARTNUMBER, IsActive = false });
                                }

                            }
                            else
                            {
                                // 変換失敗した場合の処理
                                throw new Exception("表図面の範囲文字列の取得・変換に失敗");
                            }
                        }
                        ticketXml.Variants = newVariants;
                    }
                }
                this.DialogResult = System.Windows.Forms.DialogResult.OK;
                this.Close();
            }
            else
            {
                if (ticketXml.Variants != null && ticketXml.Variants.Count > 0)
                {
                    close_permition = false;

                    DialogResult dialogResult = MessageBox.Show("※表形式です！！現時点は非対応。印刷したら手動承認後、表図面登録棚に提出してください", $"", MessageBoxButtons.YesNo);


                    this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
                    return;
                }
                this.DialogResult = System.Windows.Forms.DialogResult.OK;
                this.Close();
            }
        }

        /// <summary>
        /// ●キャンセルボタン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Cancel_Button_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        /// <summary>
        /// ●全体表示・右下表示ボタン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TitleFit_button_Click(object sender, EventArgs e)
        {
            if ((bool)TitleFit_button.Tag == false)
            {
                commitPreviewImage.TitleBlockFit();
                TitleFit_button.Tag = true;
                TitleFit_button.Text = "全体表示";
            }
            else
            {
                commitPreviewImage.ViewFit();
                TitleFit_button.Tag = false;
                TitleFit_button.Text = "右下拡大";
            }
        }

        /// <summary>
        /// ●コミットプレビュー拡大ボタンをクリック
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PreviewButton_Click(object sender, EventArgs e)
        {
            bigPreviewForm = new BigPreviewForm
            {
                WindowState = FormWindowState.Maximized
            };
            //bigPreviewForm.SetImage(MiniPreviewPictureBox.Image);
            bigPreviewForm.SetImage(commitPreviewImage.Image);
            bigPreviewForm.ShowDialog(this);
        }

        /// <summary>
        /// ●印刷機状態把握ボタン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Printer_Info_button_Click(object sender, EventArgs e)
        {
            Encryption sasaLibencryptionPipeConnection = new Encryption("SasaAuth3.1");
            string PipeClientPlanePass = sasaLibencryptionPipeConnection.Decoding(commitParam.PipeConnection31Password); //復号化

            RemoteClientDRAWCAPTURE remoteClientDRAWCAPTURE = new RemoteClientDRAWCAPTURE(
                commitParam.ClientDomainName, commitParam.ClientUserName,
                PipeClientPlanePass, commitParam.ClsLogon,
                commitParam.StageServerHost, commitParam.PipeNameDC);


            var printerNames = remoteClientDRAWCAPTURE.GetCommitPrinterShortCutName(objectConvNew: true);
            var printerAlias = remoteClientDRAWCAPTURE.GetCommitPrinterNameAndAlias(objectConvNew: true);
            var printerFailStatus = remoteClientDRAWCAPTURE.GetCommitPrinterIsFailStatus(objectConvNew: true);
            var printerSettingFromPaperSize = remoteClientDRAWCAPTURE.GetCommitPrinterSettingFromPaperSize(objectConvNew: true);

            StringBuilder sb = new StringBuilder();


            sb.AppendLine($"** 故障状態 (Ｔｒｕｅが故障中) ************************");
            foreach (var a in printerFailStatus)
            {
                sb.AppendLine($"{a.Key} {a.Value}");
            }

            sb.AppendLine($"");

            sb.AppendLine($"--------------------------------------------------------------");

            sb.AppendLine($"");
            sb.AppendLine($"** プリンタ設定名と別名 ***************************");
            sb.AppendLine($"");
            foreach (var a in printerAlias)
            {
                sb.AppendLine($"{a.Key} {a.Value}");
            }

            sb.AppendLine($"");

            sb.AppendLine($"--------------------------------------------------------------");

            sb.AppendLine($"");

            sb.AppendLine($"**コミット０(ゼロ)コマンド実行時における用紙サイズ別出力先***********");
            sb.AppendLine($"");
            foreach (var a in printerSettingFromPaperSize)
            {
                sb.AppendLine($"{a.Key} {a.Value}");
            }
            sb.AppendLine($"--------------------------------------------------------------");

            var outtext = sb.ToString();

            MessageBox.Show(this, outtext);

        }

        /// <summary>
        /// ■エマージェンシーイメージを指定ボタンに表示します。
        /// </summary>
        private void ButtonBackgroundImageWarrningSet(System.Windows.Forms.Button button, string buttonMsg)
        {
            ArcSuiteDrawingShow_button.Enabled = true;

            InvokeRequired_Control_Text(button, buttonMsg);

            button.Image = Resources.エマージェンシーバックグラウンド;
        }

        /// <summary>
        /// アークスイート側の情報とCAD表題欄情報のＲＥＶを比較し 取替図として有効なREVであるかを簡易チェックする
        /// </summary>
        private void CheckRevNumberForUpdate()
        {
            try
            {
                WriteLine($"■ArcsuitePreview.user_drawingrevision の値は 文字列:\"{ArcsuitePreview.user_drawingrevision}\" です");

                string cadRev = ticketXml.Params.FindLast(a => a.Key == "REV").Value;

                WriteLine($"■ticketXml.Params.FindLast(a => a.Key == \"REV\").Value の値は 文字列:\"{cadRev}\" です");

                int cadRevInt;
                int arcSuiteUserDrawingRev;
                if (string.IsNullOrWhiteSpace(cadRev) == false)
                {
                    bool result2 = int.TryParse(cadRev, out cadRevInt);
                    if (result2)
                    {
                        if (string.IsNullOrWhiteSpace(ArcsuitePreview.user_drawingrevision) == false)
                        {
                            bool result1 = int.TryParse(ArcsuitePreview.user_drawingrevision, out arcSuiteUserDrawingRev);
                            if (result1)
                            {
                                if (arcSuiteUserDrawingRev >= cadRevInt)
                                {
                                    // MessageBox.Show(this, $"警告.CAD側の表題欄Rev番号が {cadRev} です。ArcSuite側の表題欄Rev番号は {arcSuiteUserDrawingRev} です", "■東陽ｱﾄﾞｲﾝ警告", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    WriteLine($"※【警告】\"{PARTNUMBER_linklabel.Text}\" CAD側の表題欄Rev番号が {cadRev}は取替図にふさわしくありません。ArcSuite側の図面は {arcSuiteUserDrawingRev} です。ダイアログは閉じられます");

                                    InvokeRequired_Control_Text(SameRevWarningIgnore_button, "警告解除", Color.Red, Color.Yellow);
                                    InvokeRequired_Control_Enabled(SameRevWarningIgnore_button, true, true);

                                    InvokeRequired_Control_Text(ArcSuiteInformation_label, $"コミットしようとする図面の表題欄Rev番号は '{cadRevInt}' ですが、" +
                                        $"ArcSuite登録済み最新既存図の方は '{arcSuiteUserDrawingRev}' です。取替図にふさわしくありませんが問題無いでしょうか？", Color.Red, default);

                                }
                            }
                        }
                        else
                        {
                            if (cadRevInt == 0)
                            {
                                //MessageBox.Show(this, $"警告.CAD側の表題欄Rev番号が {cadRev} です。取替図にふさわしくありません。ダイアログは閉じられます", "■東陽ｱﾄﾞｲﾝ警告", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                WriteLine($"※【警告】\"{PARTNUMBER_linklabel.Text}\" .CAD側の表題欄Rev番号が {cadRev} , ArcSuite側表題欄Rev番号は \"{ArcsuitePreview.user_drawingrevision}\"");

                                InvokeRequired_Control_Text(SameRevWarningIgnore_button, "警告解除", Color.Red, Color.Yellow);
                                InvokeRequired_Control_Enabled(SameRevWarningIgnore_button, true, true);

                                InvokeRequired_Control_Text(ArcSuiteInformation_label, $"コミットしようとする図面の表題欄Rev番号は '{cadRevInt}' です." +
                                    $"取替図にふさわしくありませんが問題無いでしょうか？", Color.Red, default);

                            }
                            else
                            {
                                WriteLine($"注意.ArcSuie側の表題欄Rev情報は記録なし。コミット対象側の表題欄Rev情報との比較は行いません。");
                            }
                        }
                    }
                }
                else
                {
                    MessageBox.Show(this, $"警告.CAD側の表題欄Rev番号が空文字がnullです。コミットダイアログは強制終了します", "■東陽ｱﾄﾞｲﾝ警告", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    WriteLine($"※【警告】\"{PARTNUMBER_linklabel.Text}\" CAD側の表題欄Rev番号が空文字化null ダイアログは閉じます");
                    this.Close();

                }
            }
            catch (Exception ex)
            {
                WriteLine($"※{ex.Message}");
            }

        }

        /// <summary>
        /// ■スレッド対応のコントロールのtextﾌﾟﾛﾊﾟﾃｨのセット
        /// </summary>
        /// <param name="control"></param>
        /// <param name="text"></param>
        [System.Diagnostics.DebuggerStepThrough]
        internal void InvokeRequired_Control_Text(Control control, string text, System.Drawing.Color foreColor = default, System.Drawing.Color backColor = default)
        {
            MethodInvoker method = () =>
            {
                // コントロールに対する処理
                if (string.IsNullOrWhiteSpace(text) == false)
                {
                    control.Enabled = true;
                    control.Visible = true;
                }
                control.Text = text;
                control.ForeColor = foreColor;
                control.BackColor = backColor;
            };
            if (InvokeRequired) { Invoke(method); } else { method(); }

        }

        /// <summary>
        /// ■スレッド対応のTagプロパティのセット
        /// </summary>
        /// <param name="control"></param>
        /// <param name="tagdata"></param>
        internal void InvokeRequired_Control_Tag(Control control, object tagdata)
        {
            MethodInvoker method = () =>
            {
                // コントロールに対する処理
                control.Tag = tagdata;
            };
            if (InvokeRequired) { Invoke(method); } else { method(); }

        }

        /// <summary>
        /// ■スレッド対応のボタンコントロールのイメージのセット
        /// </summary>
        /// <param name="control"></param>
        /// <param name="image"></param>
        /// <param name="backColor"></param>
        [System.Diagnostics.DebuggerStepThrough]
        internal void InvokeRequired_Button_ImageSet(System.Windows.Forms.Button control, Image image, System.Drawing.Color backColor = default)
        {
            MethodInvoker method = () =>
            {
                // コントロールに対する処理
                control.Image = image;
                control.BackColor = backColor;
            };
            if (InvokeRequired) { Invoke(method); } else { method(); }

        }

        /// <summary>
        /// ■スレッド対応のコントロールのEnabledﾌﾟﾛﾊﾟﾃｨのセット
        /// </summary>
        /// <param name="control"></param>
        /// <param name="Enabled"></param>
        [System.Diagnostics.DebuggerStepThrough]
        private void InvokeRequired_Control_Enabled(Control control, bool Enabled, bool Visible = true)
        {

            MethodInvoker method = () =>
            {
                // コントロールに対する処理
                control.Enabled = Enabled;
                control.Visible = Visible;
            };
            if (InvokeRequired) { Invoke(method); } else { method(); }

        }

        private void ChangeNormalOrVariant_button_Click(object sender, EventArgs e)
        {

        }

    }
}
