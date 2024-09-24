#if NETCOREAPP
using CommonCommitLogicDNet8.Properties;
#else
using CommonCommitLogic.Properties;
#endif
using SasaLib;
using SasaLib.ArcSuitePreview;
using SasaLib.NumberingSupport;
using StageServerRemote;
using System;
using System.Collections.Generic;
#if NETCOREAPP
using System.Data;
#endif
using System.Drawing;
using System.Globalization;
using System.Linq;
#if NETCOREAPP
using System.Runtime.Versioning;
#endif
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using static CommonCommitLogic.ArcSuiteSearchResult;
#if NETCOREAPP
using EnvDTE;
#endif

namespace CommonCommitLogic
{
    /// <summary>
    /// コミットダイアログクラス
    /// </summary>
#if NETCOREAPP
    [SupportedOSPlatform("windows")]
#endif
    public partial class CommitDialogForm : Form
    {
        /// <summary>
        /// コミット処理に必要な共通設定
        /// </summary>
        private CommitCommonSettings commmitCommonSettings;

        /// <summary>
        /// コミット対象CADファイルから収集したチケットファイルを作成する属性・値データ
        /// </summary>
        private CommonTicket ticketXml;

        /// <summary>
        /// このダイアログが表図面に対応した処理に切り替えることが可能かを設定するフラグ
        /// </summary>
        private bool CanUseVariantTypeDrawing { get; set; }

        /// <summary>
        /// 実行時印刷のみモードでダイアログを設定
        /// </summary>
        private bool PrintOutOnly { get; set; }

        /// <summary>
        /// 
        /// </summary>
        private string AtiveDocFullFilename { get; set; }

        /// <summary>
        /// 表図面形式の範囲指定枝番を持っている場合true
        /// </summary>
        private bool isVariantTypeTitleBlockPARTNUMBER;

        /// <summary>
        /// ログ出力先のデフォルトを設定
        /// </summary>
        private SasaLibDelegateWriteLine WriteLine = DebugConsole.WriteLine;

        /// <summary>
        /// 
        /// </summary>
        private CancellationTokenSource cts = new CancellationTokenSource();

        /// <summary>
        /// コミット最大化プレビュー用オブジェクト
        /// </summary>
        private BigPreviewForm bigPreviewForm;

        /// <summary>
        /// 
        /// </summary>
        private CommitHelperCadDrawingFile supportCadDrawingFile;

        /// <summary>
        /// nullの場合はまだ確認されていない
        /// </summary>
        private TypeNumber typeNumber;

        /// <summary>
        /// nullの場合はまだ確認されていない
        /// </summary>
        private ReserveNumber reserveNumber;

        /// <summary>
        /// nullの場合はまだ確認されていない
        /// </summary>
        private ArcSuiteSearchResult arcSuiteSearchResult;

        /// <summary>
        /// 検索結果が1件のみに通用するアークスイート検索図
        /// </summary>
        private ArcsuitePreview arcsuitePreview
        {
            get
            {
                if (arcSuiteSearchResult.arcSuitePreviews != null && arcSuiteSearchResult.arcSuitePreviews.Count == 1 && arcSuiteSearchResult.arcSuitePreviews[0].Found == true)
                {

                    WriteLine($"[プロパティ:CommitDialogForm.ArcsuitePreview]が要求されました ,1件の検索結果を返します");
                    return arcSuiteSearchResult.arcSuitePreviews[0];
                }
                else
                {
                    WriteLine($"[プロパティ:CommitDialogForm.ArcsuitePreview]が要求されました , List<ArcsuitePreview> 'ArcSuiteSearchResult.arcSuitePreviews'にアクセスされました。１件以外の結果 {arcSuiteSearchResult.arcSuitePreviews.Count}件 のデータを保持していますので new ArcsuitePreview()を返します");
                    return new ArcsuitePreview();
                }
            }
        }

        /// <summary>
        /// 類番検索に使う追加ｻﾌｨｯｸｽ
        /// </summary>
        private List<string> arcSuitePARTNUMBER_ContainSuffixs = new List<string>() { "RL", "R", "L" };

        /// <summary>
        /// ダイアログを閉じてよいかを保持
        /// </summary>
        private bool close_permition = true;

        /// <summary>
        /// 採番サーバーとの通信をつかさどる
        /// </summary>
        private CommitHelperNumbering supportNumbering;

        /// <summary>
        /// アークスイートとの通信をつかさどる
        /// </summary>
        private CommitHelperArcSuite supportArcSuite;

        /// <summary>
        /// 
        /// </summary>
        private VariantDrawingNumberSupport variantDrawingNumberSupport = new VariantDrawingNumberSupport();

        /// <summary>
        /// 表形式の時、コミット対象図面としてユーザーが入力した枝番号を元に構築された図面番号
        /// </summary>
        private string variantofOnePartnumber;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public CommitDialogForm(CommitCommonSettings Config, CommitHelperCadDrawingFile supportCadDrawingFile,
            CommitHelperArcSuite supportArcSuite, CommitHelperNumbering supportNumbering,
            SasaLibDelegateWriteLine WriteLine, bool PrintOutOnly = false, bool CanUseVariantTypeDrawing = false)
        {
            this.commmitCommonSettings = Config;
            this.PrintOutOnly = PrintOutOnly;
            this.WriteLine = WriteLine;

            this.supportCadDrawingFile = supportCadDrawingFile;
            this.supportNumbering = supportNumbering;
            this.supportArcSuite = supportArcSuite;
            this.CanUseVariantTypeDrawing = CanUseVariantTypeDrawing;

            InitializeComponent();

#if NETCOREAPP
            this.DoubleBuffered = true;
#endif
            // CADファイル関連表示・ボタン 初期化

            InvokeRequired_Control_Text(CadFileWarningIgnore_button, "調査中", default, default);
            InvokeRequired_Control_Enabled(CadFileInformation_label, false, false);


            NumberingWebServer_button.Image = default;

            // 採番関連表示・ボタン 初期化

            InvokeRequired_Control_Enabled(NumberingInformation_label, true, true);
            InvokeRequired_Control_Text(NumberingInformation_label, "--", default, default);

            InvokeRequired_Control_Text(NumberingWarningIgnore_button, "調査中", default, default);
            InvokeRequired_Control_Enabled(NumberingWarningIgnore_button, true, true);

            InvokeRequired_Control_Enabled(NumberingWebServer_button, false, false);
            InvokeRequired_Control_Enabled(NumberingReCheck_button, false, false);

            NumberingWebServer_button.Image = default;


            // ArcSuite関連表示・ボタン 初期化

            InvokeRequired_Control_Enabled(ArcSuiteInformation_label, false, false);
            InvokeRequired_Control_Text(ArcSuiteInformation_label, "--", default, default);

            InvokeRequired_Control_Text(ArcSuiteWarningIgnore_button, "調査中", default, default);
            InvokeRequired_Control_Enabled(ArcSuiteWarningIgnore_button, true, true);

            InvokeRequired_Control_Enabled(ArcSuiteDrawingShow_button, false, false);

            // ArcSuite関連 Rev比較警告ボタン初期化
            InvokeRequired_Control_Text(SameRevWarningIgnore_button, " -- ", default, default);
            InvokeRequired_Control_Enabled(SameRevWarningIgnore_button, false, false);

            InvokeRequired_Control_Enabled(CommitExecute_Button, false); // コミット開始ボタンを開始直後にディスエイブル

            ArcSuiteDrawingShow_button.Image = default;

            ErrorOccurred_Label.Visible = false; // コミットロック中ラベルを非表示へ


            Variant_panel1.Visible = false;
            Variant_panel1.Enabled = false;
            Variant_panel2.Visible = false;
            Variant_panel2.Enabled = false;
            PARTNUMBER_CAUTION_label.Text = $"";

        }


        /// <summary>
        /// コミット実行を許可不許可するﾒｯｾｰｼﾞとボタンの状態を初期化
        /// </summary>
        private void ResetNumberingCheckArcSuiteRegistCheck()
        {
            // ArcSuite関連 Rev比較警告ボタン初期化
            InvokeRequired_Control_Text(SameRevWarningIgnore_button, " -- ", default, default);
            InvokeRequired_Control_Enabled(SameRevWarningIgnore_button, false, false);

            InvokeRequired_Control_Enabled(CommitExecute_Button, false); // コミット開始ボタンを開始直後にディスエイブル

            ArcSuiteDrawingShow_button.Image = default;

            InvokeRequired_Control_Text(NumberingInformation_label, "【表形式】図面です。追加する枝番号の入力を待っています", Color.Red, Color.Yellow);
            InvokeRequired_Control_Text(ArcSuiteInformation_label, "【表形式】図面です。追加する枝番号の入力を待っています", Color.Red, Color.Yellow);

        }

        /// <summary>
        /// SupportCadDrawingFile に値がセット・変更されたら呼び出される
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="cadDrawingFile"></param>
        /// <exception cref="NotImplementedException"></exception>
        private void SupportCadDrawingFile_CadDrawingFileChanged(object sender, CadDrawingFile cadDrawingFile)
        {
            DebugConsole.WriteLine(@"CadDrawingFileChanged イベントがキックされました");

            if (cadDrawingFile.WarrningButtonEnabled)
            {
                InvokeRequired_Control_Text(CadFileInformation_label, supportCadDrawingFile.CadDrawingFile.CompareResultMsg, Color.Red);
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
        /// SupportNumbering に値がセット・変更されたら呼び出される
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="typeNumber"></param>
        private void SupportNumbering_TypeNumberChanged(Object sender, TypeNumber typeNumber)
        {
            DebugConsole.WriteLine(@"TypeNumberChanged イベントがキックされました");

            this.typeNumber = typeNumber;


            // 図面種類をコントロールへ指定
            DrawingTypeTextBox.Text = typeNumber.TypeName;

            if (CanUseVariantTypeDrawing)
            {
                if (typeNumber.isVariant)
                {
                    Variant_panel1.Enabled = true;
                    Variant_panel1.Visible = true;
                    Variant_panel2.Enabled = true;
                    Variant_panel2.Visible = true;
                }
                else
                {
                    Variant_panel1.Enabled = false;
                    Variant_panel1.Visible = false;
                    Variant_panel2.Enabled = false;
                    Variant_panel2.Visible = false;
                }

            }
            else
            {
                if (typeNumber.isVariant)
                {

                    CommitExecute_Button.Text = "【表形式】図面はコミット非対応";

                    Variant_panel1.Enabled = false;
                    Variant_panel1.Visible = false;
                    Variant_panel2.Enabled = false;
                    Variant_panel2.Visible = false;

                    MessageBox.Show("【表形式】図面はコミット操作ができません。\r\n登録担当者による手動スキャニングおよび登録依頼が必要です", "重要");

                    this.Close();
                    return;
                }
            }



            CommitButtonEnableJudge();

        }

        /// <summary>
        /// ReserveNumber に値がセット・変更されたら呼び出される
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="reserveNumber"></param>
        private void SupportNumbering_ReserveNumberChanged(Object sender, ReserveNumber reserveNumber)
        {
            DebugConsole.WriteLine(@"ReserveNumberChanged イベントがキックされました");

            this.reserveNumber = reserveNumber;

            /// 採番システムからの結果を反映させる
            if (supportNumbering.ReserveNumber.CheckAcquiredNumberedNormal == true)
            {
                WriteLine($"■CommitDialogForm.OnEvent_ReserveNumberChanged(..) 採番システム {commmitCommonSettings.NumberingServerName} との通信は既に成功しています");

                if (supportNumbering.ReserveNumber.NumberingRecordAvailable == true)
                {
                    if (isVariantTypeTitleBlockPARTNUMBER == true)
                    {
                        if (supportNumbering.ReserveNumber.IsHyou == true)
                        {
                            InvokeRequired_Control_Text(NumberingWebServer_button, "Web採番システムを開く", default);
                            InvokeRequired_Control_Text(NumberingReCheck_button, "再ﾁｪｯｸ", default);


                            InvokeRequired_Control_Enabled(NumberingInformation_label, true, true);
                            InvokeRequired_Control_Text(NumberingInformation_label, supportNumbering.ReserveNumber.CheckNumberdHistoryAnser, default);

                            this.WriteLine($"■CommitDialogForm.OnEvent_ReserveNumberChanged(..) {this.reserveNumber.commitTarget_partnumber}の採番情報は見つかっています。 警告ボタンを非表示にします");
                            InvokeRequired_Control_Enabled(NumberingWarningIgnore_button, false, false);

                        }
                        else
                        {
                            InvokeRequired_Control_Text(NumberingWebServer_button, "Web採番システムを開く", default);
                            InvokeRequired_Control_Text(NumberingReCheck_button, "再ﾁｪｯｸ", default);


                            InvokeRequired_Control_Enabled(NumberingInformation_label, true, true);
                            string labelText = $"\"{supportNumbering.ReserveNumber.commitTarget_partnumber}\" は、採番システムに【表形式】として採番されていません。確認が必要です";
                            InvokeRequired_Control_Text(NumberingInformation_label, labelText, Color.Red, Color.Yellow);

                            this.WriteLine($"■CommitDialogForm.OnEvent_ReserveNumberChanged(..) {this.reserveNumber.commitTarget_partnumber}は【表形式】として指定されていますが、採番情報は非表形式です！！");

                            InvokeRequired_Control_Text(NumberingWarningIgnore_button, "警告無視\r\nできません", DefaultForeColor, DefaultBackColor);
                            InvokeRequired_Control_Enabled(NumberingWarningIgnore_button, false, true);

                        }

                    }
                    else
                    {
                        if (supportNumbering.ReserveNumber.IsHyou == true)
                        {
                            InvokeRequired_Control_Text(NumberingWebServer_button, "Web採番システムを開く", default);
                            InvokeRequired_Control_Text(NumberingReCheck_button, "再ﾁｪｯｸ", default);

                            InvokeRequired_Control_Enabled(NumberingInformation_label, true, true);
                            InvokeRequired_Control_Text(NumberingInformation_label, "採番システムには【表形式】で採番されていますがこの図面は一般図面番号形式です", Color.Red);

                            this.WriteLine($"■CommitDialogForm.OnEvent_ReserveNumberChanged(..) {this.reserveNumber.commitTarget_partnumber}の採番情報は【表形式】として見つかっていますが、表題欄図面番号は一般図面形式です");
                            InvokeRequired_Control_Text(NumberingWarningIgnore_button, "警告無視\r\nできません", DefaultForeColor, DefaultBackColor);
                            InvokeRequired_Control_Enabled(NumberingWarningIgnore_button, false, true);
                        }
                        else
                        {
                            InvokeRequired_Control_Enabled(NumberingInformation_label, true, true);
                            InvokeRequired_Control_Text(NumberingInformation_label, supportNumbering.ReserveNumber.CheckNumberdHistoryAnser, default);
                            this.WriteLine($"■CommitDialogForm.OnEvent_ReserveNumberChanged(..) {this.reserveNumber.commitTarget_partnumber}の採番情報は見つかっています。 警告ボタンを非表示にします");
                            InvokeRequired_Control_Enabled(NumberingWarningIgnore_button, false, false);
                        }

                    }
                }
                else
                {
                    WriteLine($"■CommitDialogForm.OnEvent_ReserveNumberChanged(..) 採番システムには {supportNumbering.ReserveNumber.commitTarget_partnumber} は採番されておりません");

                    InvokeRequired_Control_Text(NumberingWebServer_button, "Web採番システムを開く", default);
                    InvokeRequired_Control_Text(NumberingReCheck_button, "再ﾁｪｯｸ", default);

                    if (isVariantTypeTitleBlockPARTNUMBER == true)
                    {
                        InvokeRequired_Control_Text(this.NumberingInformation_label, $"{this.reserveNumber.commitTarget_partnumber} は 【表形式】としては採番情報が見つかりません。採番システムを確認してください.", System.Drawing.Color.Red);

                        InvokeRequired_Control_Text(NumberingWarningIgnore_button, "警告無視\r\nできません", DefaultForeColor, DefaultBackColor);
                        InvokeRequired_Control_Enabled(NumberingWarningIgnore_button, false, true);
                    }
                    else
                    {
                        InvokeRequired_Control_Text(this.NumberingInformation_label, $"{this.reserveNumber.commitTarget_partnumber} の採番情報が見つかりません。採番システムを確認してください.", System.Drawing.Color.Red);

                        InvokeRequired_Control_Text(NumberingWarningIgnore_button, "警告無視", Color.Red, Color.Yellow);
                    }
                }
            }
            else
            {
                MethodInvoker method = () =>
                {

                    InvokeRequired_Control_Text(NumberingInformation_label, "採番システムと通信できません。", System.Drawing.Color.Red);

                    InvokeRequired_Control_Text(NumberingWarningIgnore_button, "警告無視", Color.Red, Color.Yellow);

                    ButtonBackgroundImageWarrningSet(NumberingWebServer_button, "採番システムへ接続不能");  // 検索結果異常時のＵＩをセット
                    InvokeRequired_Control_Text(NumberingReCheck_button, "不可", default);


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

            this.arcSuiteSearchResult = arcSuiteSearchResult;

            string commitTarget_partnumber = arcSuiteSearchResult.commitTarget_partnumber; // コミットターゲット図番

            if (arcSuiteSearchResult.arcSuitePreviews != null)
            {
                if (isVariantTypeTitleBlockPARTNUMBER == false)
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

                            InvokeRequired_Control_Text(ArcSuiteWarningIgnore_button, "警告無視\r\nできません", System.Drawing.Color.Red);
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
                            string hit_zubans = string.Join(" , ", this.arcSuiteSearchResult.arcSuitePreviews.Select(item => $"\"{item.user_zuban}\""));
                            InvokeRequired_Control_Text(ArcSuiteInformation_label, $"ArcSuiteに {commitTarget_partnumber} は見つかりませんが、類番検索の結果 {hit_zubans} が存在します コミットは継続できません", System.Drawing.Color.Red);

                            InvokeRequired_Button_ImageSet(ArcSuiteDrawingShow_button, null);
                            InvokeRequired_Control_Text(ArcSuiteDrawingShow_button, "ｸﾘｯｸしてArcuSuiteの\r\n図面を検索", Color.Red);
                            InvokeRequired_Control_Enabled(ArcSuiteDrawingShow_button, true, true);// ArcSuite検索結果表示ボタンをイネーブル

                            InvokeRequired_Control_Text(ArcSuiteWarningIgnore_button, $"警告無視\r\nできません", System.Drawing.Color.Red);
                            InvokeRequired_Control_Enabled(ArcSuiteWarningIgnore_button, false, true);// ArcSuite検索結果表示ボタンをイネーブル
                            break;

                        default:
                            InvokeRequired_Control_Enabled(ArcSuiteDrawingShow_button, false);// ArcSuite検索結果表示ボタンをディスエイブル

                            InvokeRequired_Control_Enabled(ArcSuiteWarningIgnore_button, true, true);// ArcSuite検索結果無視ボタンをイネーブル

                            throw new Exception($"ArcSuiteSearchResult.Variant_CheckResult_MessagetypeEnum がシステム範囲外");

                    }
                }
                else
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

                            InvokeRequired_Control_Text(ArcSuiteWarningIgnore_button, "警告無視\r\nできません", System.Drawing.Color.Red);
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
                            string hit_zubans = string.Join(" , ", this.arcSuiteSearchResult.arcSuitePreviews.Select(item => $"\"{item.user_zuban}\""));
                            InvokeRequired_Control_Text(ArcSuiteInformation_label, $"ArcSuiteに {commitTarget_partnumber} は見つかりませんが、類番検索の結果 {hit_zubans} が存在します コミットは継続できません", System.Drawing.Color.Red);

                            InvokeRequired_Button_ImageSet(ArcSuiteDrawingShow_button, null);
                            InvokeRequired_Control_Text(ArcSuiteDrawingShow_button, "ｸﾘｯｸしてArcuSuiteの\r\n図面を検索", Color.Red);
                            InvokeRequired_Control_Enabled(ArcSuiteDrawingShow_button, true, true);// ArcSuite検索結果表示ボタンをイネーブル

                            InvokeRequired_Control_Text(ArcSuiteWarningIgnore_button, $"警告無視\r\nできません", System.Drawing.Color.Red);
                            InvokeRequired_Control_Enabled(ArcSuiteWarningIgnore_button, false, true);// ArcSuite検索結果表示ボタンをイネーブル
                            break;

                        default:
                            InvokeRequired_Control_Enabled(ArcSuiteDrawingShow_button, false);// ArcSuite検索結果表示ボタンをディスエイブル

                            InvokeRequired_Control_Enabled(ArcSuiteWarningIgnore_button, true, true);// ArcSuite検索結果無視ボタンをイネーブル

                            throw new Exception($"ArcSuiteSearchResult.Variant_CheckResult_MessagetypeEnum がシステム範囲外");

                    }
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
            if (supportCadDrawingFile.CadDrawingFile == null || typeNumber == null || reserveNumber == null || arcSuiteSearchResult == null)
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
        /// ■チケットデータをダイアログへセット.フォームを表示する直前に実行する必要があります
        /// </summary>
        /// <param name="ticketXml"></param>
        public bool SetData(CommonTicket ticketXml, out string ErrMsg)
        {
            this.ticketXml = ticketXml;

            bool result = true;
            ErrMsg = null;

            StageServerHostLabel.Text = commmitCommonSettings.StageServerHost;



            //用紙種別
            PaperSizeLabel.Text = ticketXml.PAPERSIZE;
            toolTip1.SetToolTip(PaperSizeLabel, $"印刷レイアウトにおける用紙サイズと向き:{ticketXml.PLOTPAPERSIZE}");

            //チケットコード
            TicketCodeLinkLabel.Text = ticketXml.TICKETCODE;

            // 材質 材質コード
            TitleBlock_MATERIAL_And_MATERIALCODE_label.Text = $"{(string)ticketXml.GetParamKeyValue("MATERIAL")} , {(string)ticketXml.GetParamKeyValue("MATERIALCODE")}";

            // 機種
            TitleBlock_MACHINETYPE_label.Text = (string)ticketXml.GetParamKeyValue("MACHINETYPE");

            // 顧客
            string firstcustomer = (string)ticketXml.GetParamKeyValue("FIRSTCUSTOMER");
            string CUSTOMER = (string)ticketXml.GetParamKeyValue("CUSTOMER");

            if (string.IsNullOrWhiteSpace(firstcustomer) == false)
                lFIRSTCUSTOMERlabe.Text = firstcustomer;
            if (string.IsNullOrWhiteSpace(CUSTOMER) == false)
                lFIRSTCUSTOMERlabe.Text = CUSTOMER;

            // 表題
            TitleBlock_TITLE_label.Text = (string)ticketXml.GetParamKeyValue("TITLE");

            // 図面番号
            TitleBlock_PARTNUMBER_linklabel.Text = (string)ticketXml.GetParamKeyValue("PARTNUMBER");

            // 改定番号
            TitleBlock_REV_textBox.Text = (string)ticketXml.GetParamKeyValue("REV");

            // 製図者
            TitleBlock_AUTHOR_label.Text = (string)ticketXml.GetParamKeyValue("AUTHOR");
            if (string.IsNullOrWhiteSpace(TitleBlock_AUTHOR_label.Text) == false)
            {
                CultureInfo provider = CultureInfo.CurrentCulture;
                // 製図日
                string strAUTHORDATE = (string)ticketXml.GetParamKeyValue("AUTHORDATE");
                try
                {
                    DateTime authorDatetime;
                    //bool result1 = StringUtil.TryParseExactMultiple_CurrentCulture(strAUTHORDATE, new List<string> { "yyyy.MM.dd", "yyyy/MM/dd", "yyyy/MM/dd[ddd] H:mm:ss" }.ToArray(), out authorDatetime);
                    bool result1 = StringUtil.ConvertFromDateTimeStringToDateTime(strAUTHORDATE, out authorDatetime);
                    if (result)
                        TitleBlock_AUTHORDATE_label.Text = authorDatetime.ToString("yyyy/MM/dd");
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
            TitleBlock_DESIGNER_label.Text = (string)ticketXml.GetParamKeyValue("DESIGNER");

            // 設計日
            if (string.IsNullOrWhiteSpace(TitleBlock_DESIGNER_label.Text) == false)
            {
                string strCHECKDATE = (string)ticketXml.GetParamKeyValue("CHECKDATE");
                try
                {
                    DateTime checkDatetime;
                    // bool result1 = StringUtil.TryParseExactMultiple_CurrentCulture(strCHECKDATE, new List<string> { "yyyy.MM.dd", "yyyy/MM/dd", "yyyy/MM/dd[ddd] H:mm:ss" }.ToArray(), out checkDatetime);
                    bool result1 = StringUtil.ConvertFromDateTimeStringToDateTime(strCHECKDATE, out checkDatetime);
                    if (result)
                        TitleBlock_CHECKDATE_label.Text = checkDatetime.ToString("yyyy/MM/dd");
                    else
                        throw new Exception();
                }
                catch (Exception ex)
                {
                    ErrMsg = $"設計日の日付フォーマットに異常があります -> \"{strCHECKDATE}\" {ex.Message} {ex.InnerException}";
                    result = false;
                }
            }

            if (commmitCommonSettings.PrinterDriverName != "")
            {
                PlotFileCreate_label.Text = $"出力用紙ｻｲｽﾞと方向の設定は, ﾌﾟﾘﾝﾀ【{commmitCommonSettings.PrinterDriverName}】のﾍﾟｰｼﾞ設定にて決まります";
            }
            else if (commmitCommonSettings.PC3FileName != "")
            {
                PlotFileCreate_label.Text = $"出力用紙ｻｲｽﾞと方向の設定は, ﾌﾟﾘﾝﾀ【{commmitCommonSettings.PC3FileName}】のﾍﾟｰｼﾞ設定にて決まります";

            }
            else
            {
                PlotFileCreate_label.Text = $"";
            }




            // 表図面か否かを図番のみでチェックします。
            bool checkResult = variantDrawingNumberSupport.ParseToyoVariantDrawingNumberString(TitleBlock_PARTNUMBER_linklabel.Text, out string baseNumber, out isVariantTypeTitleBlockPARTNUMBER, out string rangePart, out string rangeStart, out string rangeEnd, out string suffixCode);

            if (checkResult)
            {
                // 表図面判定 表図面の場合は専用コントロールを表示させます
                if (CanUseVariantTypeDrawing && isVariantTypeTitleBlockPARTNUMBER)
                {
                    Variant_panel1.Enabled = isVariantTypeTitleBlockPARTNUMBER;
                    Variant_panel2.Enabled = isVariantTypeTitleBlockPARTNUMBER;
                    VariantNumber_MIN_label1.Enabled = isVariantTypeTitleBlockPARTNUMBER;
                    VariantNumber_MIN_label1.Text = rangeStart;
                    VariantNumber_MIN_label2.Enabled = isVariantTypeTitleBlockPARTNUMBER;
                    VariantNumber_MIN_label2.Text = rangeStart;
                    VariantNumber_MAX_label.Enabled = isVariantTypeTitleBlockPARTNUMBER;
                    VariantNumber_MAX_label.Text = rangeEnd;
                    VariantNumber_Prefix_label.Text = baseNumber;
                    VariantNumber_Suffix_label.Text = suffixCode;
                    ActiveVariantEnd_textBox.Enabled = isVariantTypeTitleBlockPARTNUMBER;
                    ActiveVariantEnd_textBox.BackColor = Color.MistyRose;
                    VariantOnlyMemo_textBox.Enabled = isVariantTypeTitleBlockPARTNUMBER;
                    VariantOnlyMemo_textBox.BackColor = Color.MistyRose;
                }
            }
            else
            {
                throw new Exception($"図面番号 \"{TitleBlock_PARTNUMBER_linklabel.Text}\" を ParseToyoVariantDrawingNumberString(..)メソッドで解析に失敗");

            }
            return result;
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

                supportCadDrawingFile.CheckCadFileName(TitleBlock_PARTNUMBER_linklabel.Text, this);

                CheckReserveNumberStart(); // 採番サーバーにチェック指示

                var resultCheckNumberType = supportNumbering.CheckNumberType(TitleBlock_PARTNUMBER_linklabel.Text, this);
                if (resultCheckNumberType)
                {
                    WriteLine($"■CommitDialogForm.CommitDialogForm_Load(..)　PARTNUMBER_linklabel.Text = 【{TitleBlock_PARTNUMBER_linklabel.Text}】の図面種類および表図面か否かをチェックしました。.結果 NumberTypeConfig.drawingType = {supportNumbering.TypeNumber.drawingType}");

                    if (isVariantTypeTitleBlockPARTNUMBER == false) // //表図面判定を受けていない場合は、即アークスイートに問い合わせを開始します
                    {
                        // アークスイート登録確認開始
                        supportArcSuite.CheckArcSuiteRegisted(TitleBlock_PARTNUMBER_linklabel.Text, TitleBlock_TITLE_label.Text, arcSuitePARTNUMBER_ContainSuffixs, ct);
                        InvokeRequired_Control_Text(ArcSuiteInformation_label, "サーバーからの返答を待機しています・・・", Color.Red, default);
                        InvokeRequired_Control_Text(ArcSuiteWarningIgnore_button, "調査中", default, default);
                    }
                    else
                    {
                        InvokeRequired_Control_Text(ArcSuiteInformation_label, "【表形式】図面です。追加する枝番号の入力を待っています", Color.Red, Color.Yellow);
                        InvokeRequired_Control_Text(ArcSuiteWarningIgnore_button, "待機中", default, default);
                        InvokeRequired_Control_Enabled(ArcSuiteWarningIgnore_button, false, true);

                    }
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
                InvokeRequired_Control_Text(CadFileWarningIgnore_button, "---", default, default);
                InvokeRequired_Control_Text(NumberingWarningIgnore_button, "---", default, default);
                InvokeRequired_Control_Text(ArcSuiteWarningIgnore_button, "---", default, default);

                CADTITLE_groupBox.Enabled = false;
                CadFileInformation_groupBox.Enabled = false;
                NUMBERING_groupBox.Enabled = false;
                ArcSuite_groupBox.Enabled = false;
                CommitExecute_Button.Enabled = true;
                CommitExecute_Button.Text = "ﾊﾞｰｺｰﾄﾞ無し印刷開始";
            }
        }

        /// <summary>
        /// ■採番サーバーへ採番チェック開始
        /// </summary>
        private void CheckReserveNumberStart()
        {

            if (isVariantTypeTitleBlockPARTNUMBER == false) //表図面判定を受けていない場合は、即採番システムに問合せを開始します
            {
                supportNumbering.CheckReserveNumber(TitleBlock_PARTNUMBER_linklabel.Text, this);
                InvokeRequired_Control_Text(NumberingInformation_label, "サーバーからの返答を待機しています・・・", Color.Red, default);
                InvokeRequired_Control_Text(NumberingWarningIgnore_button, "調査中", default, default);
            }
            else
            {
                InvokeRequired_Control_Text(NumberingInformation_label, "【表形式】図面です。追加する枝番号の入力を待っています", Color.Red, Color.Yellow);
                InvokeRequired_Control_Text(NumberingWarningIgnore_button, "待機中", default, default);
                InvokeRequired_Control_Enabled(NumberingWarningIgnore_button, false, true);
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
            if (arcSuiteSearchResult != null && arcSuiteSearchResult.arcSuitePreviews != null)
            {

                if (string.IsNullOrWhiteSpace(arcsuitePreview.temporalyDrawingImageFullFileName) == false)
                {
                    string fullfilename = arcsuitePreview.temporalyDrawingImageFullFileName;
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
            Clipboard.SetText(TitleBlock_PARTNUMBER_linklabel.Text);
            MessageBox.Show($"図面番号 \"{TitleBlock_PARTNUMBER_linklabel.Text}\"をｸﾘｯﾌﾟﾎﾞｰﾄﾞにコピーしました");
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

            WriteLine($"※【重要】\"{TitleBlock_PARTNUMBER_linklabel.Text}\"  (\"{ticketXml.TICKETCODE}\") CADファイルのエラーを無視するボタンが押されました。この時のエラー情報:\"{CadFileInformation_label.Text}\"");

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

            WriteLine($"※【重要】\"{TitleBlock_PARTNUMBER_linklabel.Text}\"  (\"{ticketXml.TICKETCODE}\") 採番時のエラーを無視するボタンが押されました。この時のエラー情報:\"{NumberingInformation_label.Text}\"");

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

            WriteLine($"※【重要】\"{TitleBlock_PARTNUMBER_linklabel.Text}\"  (\"{ticketXml.TICKETCODE}\") ArcSuiteに関するエラーを無視するボタンが押されました。この時のエラー情報:\"{ArcSuiteInformation_label.Text}\"");

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
            {
                InvokeRequired_Control_Text(ArcSuiteInformation_label, $"表題欄Rev番号についての警告を無視すると指示されました", Color.BlueViolet);
                InvokeRequired_Control_Enabled(ArcSuiteDrawingShow_button, false, false);
            }
            WriteLine($"※【重要】\"{TitleBlock_PARTNUMBER_linklabel.Text}\"  (\"{ticketXml.TICKETCODE}\") ArcSuiteの既存最新図の表題欄ﾘﾋﾞｼﾞｮﾝ番号と同じ表題欄ﾘﾋﾘﾋﾞｼﾞｮﾝ番号でコミットされたことへの警告ボタンがを無視するように指示されました。この時のエラー情報:\"{ArcSuiteInformation_label.Text}\"");

            CommitButtonEnableJudge();

        }

        /// <summary>
        /// ●採番サーバー（ウェブ版）を開くボタン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void NumberingWebServer_button_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(TitleBlock_PARTNUMBER_linklabel.Text)) return;

            try
            {
                NumberingWebHelper.OpenNumberingWebServer(TitleBlock_PARTNUMBER_linklabel.Text,
                    commmitCommonSettings.NumberingServerWebAddr,
                    commmitCommonSettings.NumberingPartDrawingUpdateAddress,
                    commmitCommonSettings.NumberingAssyDrawingUpdateAddress,
                    commmitCommonSettings.NumberingLayoutDrawingUpdateAddress, WriteLine);
            }
            catch (Exception ex)
            {
                WriteLine($"Web採番システムオープン時に例外検知 {ex.Message} {ex.InnerException}");
            }
        }

        /// <summary>
        /// ●採番済みﾁｪｯｸをリトライ
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void NumberingReCheck_button_Click(object sender, EventArgs e)
        {
            VariantOnlyMemo_textBox.Focus();
            ActiveVariantEnd_textBox.Focus();
            CheckReserveNumberStart(); // 採番サーバーにチェック指示
        }

        /// <summary>
        /// ●登録済みアークスイート最新図・検索表示ボタン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void SearchRegistedArcSuiteDrawing_button_Click(object sender, EventArgs e)
        {
            if (arcSuiteSearchResult.arcSuitePreviews != null)
            {
                if (arcSuiteSearchResult.arcSuitePreviews.Count == 1)
                {
                    if (arcsuitePreview.Normality == false)
                    {
                        MessageBox.Show("ArcSuteに接続出来ていないか、接続の途中です", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    ArcSuiteDrawingShow_button.Enabled = false; // ArcSuiteプレビュー表示ボタンを一時無効化

                    if (arcsuitePreview.Found == true)
                    {

                        // ArcSuiteに問い合わせるURL
                        string ArcSuiteDrawingSearchURL = commmitCommonSettings.ArcSuiteSearchURL.Replace("{SANITIZEDPARTNUMBER}", arcsuitePreview.user_zuban);

                        // コミット対象のCADファイル
                        string activeDocumentFullFileName = AtiveDocFullFilename;

                        if (commmitCommonSettings.ArcSuiteDrawingDownloadMode)
                        {
                            ///
                            ArcSuitePreviewOnlyForm arcSuitePreviewOnlyForm = null;

                            // ボタン文字列退避
                            string SearchRegistedArcSuiteDrawing_button_Text = ArcSuiteDrawingShow_button.Text;

                            ArcSuiteDrawingShow_button.Text = "ArcSuiteへ図面検索中\r\n暫くお待ちださい・・・";

                            // 一度ダウンロードした状態かを確認。ない場合は取出し

                            Encryption sasaLibencryptionPipeConnection = new Encryption("SasaAuth3.1");
                            string PipeClientPlanePass = sasaLibencryptionPipeConnection.Decoding(commmitCommonSettings.PipeConnection31Password); //復号化

                            await Task.Run(() =>
                            {
                                if (string.IsNullOrWhiteSpace(arcsuitePreview.temporalyDrawingImageFullFileName) == true ||
                                System.IO.File.Exists(arcsuitePreview.temporalyDrawingImageFullFileName) == false)
                                {
                                    WriteLine($"■CommitDialogForm.SearchRegistedArcSuiteDrawing_button_Click(..) ArcSuite登録図ﾌﾟﾚﾋﾞｭｰ用TIFFファイルをダウンロードします");
                                    /// リモート操作をするオブジェクトを生成
                                    RemoteClientDRAWREGIST remoteClientDR = new RemoteClientDRAWREGIST(
                                        commmitCommonSettings.ClientDomainName,
                                        commmitCommonSettings.ClientUserName,
                                        PipeClientPlanePass,
                                        commmitCommonSettings.ClsLogon,
                                        commmitCommonSettings.StageServerHost,
                                        commmitCommonSettings.PipeNameDR
                                        );

                                    ArcsuitePreview originalStruct = arcsuitePreview;

                                    var resule = CheckArcSuiteData.GetArcSuiteImagePipe(remoteClientDR, arcsuitePreview.user_zuban, out originalStruct.temporalyDrawingImageFullFileName, objectConvNew: Commit.objectConvNew, WriteLine);
                                    arcSuiteSearchResult.arcSuitePreviews[0] = originalStruct;
                                }
                                else
                                {
                                    WriteLine($"■CommitDialogForm.SearchRegistedArcSuiteDrawing_button_Click(..) ArcSuite登録図ﾌﾟﾚﾋﾞｭｰ用TIFFファイルはすでに取得済みでした。{arcsuitePreview.temporalyDrawingImageFullFileName} ");
                                }
                            });


                            Encryption sasaLibencryptionArcSuite = new Encryption("SasaAuth3.1");
                            string ArcSuiteUserPlanePass = sasaLibencryptionArcSuite.Decoding(commmitCommonSettings.ArcSuiteCrypt31UserPass); //復号化

                            //テンポラリﾌｫﾙﾀﾞに表示すべきﾌｧｲﾙがあるか確認
                            if (System.IO.File.Exists(arcsuitePreview.temporalyDrawingImageFullFileName) == true)
                            {
                                arcSuitePreviewOnlyForm = new ArcSuitePreviewOnlyForm(this, WriteLine);
                                arcSuitePreviewOnlyForm.Text = $"ｱｰｸｽｲｰﾄに登録済みの図面 {arcsuitePreview.user_zuban} {arcsuitePreview.createdOnMessage}";
                                arcSuitePreviewOnlyForm.PreviewSet(arcsuitePreview, ArcSuiteDrawingSearchURL, activeDocumentFullFileName);
                                arcSuitePreviewOnlyForm.MessageSet("");
                                arcSuitePreviewOnlyForm.SetUnsetCadTypeFlagControlDatas(
                                        commmitCommonSettings.StageServerHost,
                                        commmitCommonSettings.PipeNameDR,
                                        commmitCommonSettings.ClientDomainName,
                                        commmitCommonSettings.ClientUserName,
                                        PipeClientPlanePass,
                                        commmitCommonSettings.ClsLogon,
                                        RemoteClientCADtype.CadType.InventorModel,
                                        commmitCommonSettings.ArcSuiteUserName,
                                        ArcSuiteUserPlanePass
                                );

                                try
                                {
                                    arcSuitePreviewOnlyForm.ShowDialog(this);
                                }
                                catch (Exception ex)
                                {
                                    WriteLine($"※アークスイートﾌﾟﾚﾋﾞｭｰダイアログのShowDialog()niteにて例外検知 {ex.Message}");
                                }
                            }
                            else
                            {
                                WriteLine($"※CommitDialogForm.SearchRegistedArcSuiteDrawing_button_Click(..) ｱｰｸｽｲｰﾄ問合せ後の temporalyDrawingImageFullFileName がnullまたは空白にもかかわらずﾌﾟﾚﾋﾞｭｰしようとしました");
                            }



                        } //  Config.ArcSuiteDrawingDownloadMode が true の場合
                        else
                        {
                            if (string.IsNullOrWhiteSpace(arcsuitePreview.createdOnMessage) != true)
                            {
                                //string ArcSuiteURL = Config.ArcSuiteSearchAndContentOpenURL.Replace("{SANITIZEDPARTNUMBER}", CommitDialogForm.stArcSuitePreview.user_zuban);
                                string ArcSuiteURL = commmitCommonSettings.ArcSuiteSearchAndContentOpenURL.Replace("{SANITIZEDPARTNUMBER}", arcsuitePreview.user_zuban);
                                System.Diagnostics.Process.Start(ArcSuiteURL);
                            }
                        } // WebでArcSuite図面検索
                    }
                    else
                    {
                        //InventorAddInServer.logSystem.WriteLine($"■ｱｰｸｽｲｰﾄ問合せ結果を受信する前か、同名図面が存在しないのに表示ボタンが押されました CommitDialogForm.stArcSuitePreview.Found={CommitDialogForm.stArcSuitePreview.Found}");
                        WriteLine($"※CommitDialogForm.SearchRegistedArcSuiteDrawing_button_Click(..) ｱｰｸｽｲｰﾄ問合せ結果を受信する前か、同名図面が存在しないのに表示ボタンが押されました CommitDialogForm.stArcSuitePreview.Found={arcsuitePreview.Found}");
                    }

                    ArcSuiteDrawingShow_button.Enabled = true; // ArcSuiteプレビュー表示ボタンを有効化

                }
                else if (arcSuiteSearchResult.arcSuitePreviews.Count > 1)
                {
                    //                     string suffixsStr = string.Join(", ", suffixs.Select(item => $"\"{item}\""));

                    string hit_zubans = string.Join(" , ", arcSuiteSearchResult.arcSuitePreviews.Select(item => $"\"{item.user_zuban}\""));
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
            if (isVariantTypeTitleBlockPARTNUMBER)
            {
                if (string.IsNullOrWhiteSpace(VariantOnlyMemo_textBox.Text))
                {
                    MessageBox.Show(this, "表図面では備考欄に文字列が必要です", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }
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

                            bool checkResult = variantDrawingNumberSupport.ParseToyoVariantDrawingNumberString(variant.PRARTNUMBER, out string prefix, out bool isVariant, out string rangePart, out string rangeStart, out string rangeEnd, out string suffixCode);

                            bool success = int.TryParse(rangeStart, out int number);

                            bool success2 = int.TryParse(ActiveVariantEnd_textBox.Text, out int number2);

                            if (success && success2)
                            {
                                // 変換成功した場合の処理
                                if (number <= number2)
                                {
                                    if (variantofOnePartnumber == variant.PRARTNUMBER)
                                        newVariants.Add(new CommonTicket.Variant { PRARTNUMBER = variant.PRARTNUMBER, IsActive = true, COMMENTS = VariantOnlyMemo_textBox.Text });
                                    else
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

                    DialogResult dialogResult = MessageBox.Show("※【表形式】です！！現時点は非対応。印刷したら手動承認後、表図面登録棚に提出してください", $"", MessageBoxButtons.YesNo);


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
            string PipeClientPlanePass = sasaLibencryptionPipeConnection.Decoding(commmitCommonSettings.PipeConnection31Password); //復号化

            RemoteClientDRAWCAPTURE remoteClientDRAWCAPTURE = new RemoteClientDRAWCAPTURE(
                commmitCommonSettings.ClientDomainName, commmitCommonSettings.ClientUserName,
                PipeClientPlanePass, commmitCommonSettings.ClsLogon,
                commmitCommonSettings.StageServerHost, commmitCommonSettings.PipeNameDC);


            var printerNames = remoteClientDRAWCAPTURE.GetCommitPrinterShortCutName(objectConvNew: Commit.objectConvNew);
            var printerAlias = remoteClientDRAWCAPTURE.GetCommitPrinterNameAndAlias(objectConvNew: Commit.objectConvNew);
            var printerFailStatus = remoteClientDRAWCAPTURE.GetCommitPrinterIsFailStatus(objectConvNew: Commit.objectConvNew);
            var printerSettingFromPaperSize = remoteClientDRAWCAPTURE.GetCommitPrinterSettingFromPaperSize(objectConvNew: Commit.objectConvNew);

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
                WriteLine($"■ArcsuitePreview.user_drawingrevision の値は 文字列:\"{arcsuitePreview.user_drawingrevision}\" です");

                string cadRev = ticketXml.Params.FindLast(a => a.Key == "REV").Value;

                WriteLine($"■ticketXml.Params.FindLast(a => a.Key == \"REV\").Value の値は 文字列:\"{cadRev}\" です");

                int cadRevInt;
                int arcSuiteUserDrawingRev;
                if (string.IsNullOrWhiteSpace(cadRev) == false)
                {
                    bool result2 = int.TryParse(cadRev, out cadRevInt);
                    if (result2)
                    {
                        if (string.IsNullOrWhiteSpace(arcsuitePreview.user_drawingrevision) == false)
                        {
                            bool result1 = int.TryParse(arcsuitePreview.user_drawingrevision, out arcSuiteUserDrawingRev);
                            if (result1)
                            {
                                if (arcSuiteUserDrawingRev >= cadRevInt)
                                {
                                    // MessageBox.Show(this, $"警告.CAD側の表題欄Rev番号が {cadRev} です。ArcSuite側の表題欄Rev番号は {arcSuiteUserDrawingRev} です", "■東陽ｱﾄﾞｲﾝ警告", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    WriteLine($"※【警告】\"{TitleBlock_PARTNUMBER_linklabel.Text}\" CAD側の表題欄Rev番号が {cadRev}は取替図にふさわしくありません。ArcSuite側の図面は {arcSuiteUserDrawingRev} です。ダイアログは閉じられます");

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
                                WriteLine($"※【警告】\"{TitleBlock_PARTNUMBER_linklabel.Text}\" .CAD側の表題欄Rev番号が {cadRev} , ArcSuite側表題欄Rev番号は \"{arcsuitePreview.user_drawingrevision}\"");

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
                    WriteLine($"※【警告】\"{TitleBlock_PARTNUMBER_linklabel.Text}\" CAD側の表題欄Rev番号が空文字化null ダイアログは閉じます");
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
        /// <param name="control">true:コントロール Enable </param>
        /// <param name="Enabled">true:コントロール Visible</param>
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


        /// <summary>
        /// 表図面でｺﾐｯﾄする枝番号を入力した時に発生するイベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ActiveVariantEnd_textBox_TextChanged(object sender, EventArgs e)
        {
            ResetNumberingCheckArcSuiteRegistCheck();

            if (_IsThreeDigitInt(ActiveVariantEnd_textBox.Text))
            {
                string baseNumber; string rangePart; string rangeStart; string rangeEnd; string suffixCode;
                bool isVariant;
                bool checkResult = variantDrawingNumberSupport.ParseToyoVariantDrawingNumberString(TitleBlock_PARTNUMBER_linklabel.Text, out baseNumber, out isVariant, out rangePart, out rangeStart, out rangeEnd, out suffixCode);

                int.TryParse(ActiveVariantEnd_textBox.Text, out int _ActiveVarianInt);
                int.TryParse(rangeStart, out int _suffixStartInt);
                int.TryParse(rangeEnd, out int _suffixEndInt);

                if (_ActiveVarianInt < _suffixStartInt || _ActiveVarianInt > _suffixEndInt)
                {
                    MessageBox.Show(this, $"表の範囲外です。{rangeStart} から {rangeEnd} までの必要があります", caption: "範囲外", buttons: MessageBoxButtons.OK, icon: MessageBoxIcon.Error);
                    ActiveVariantEnd_textBox.Text = null;
                    Variant_End_Input_label.ForeColor = Color.Red;
                    return;
                }
                else
                {
                    Variant_End_Input_label.ForeColor = DefaultForeColor;
                }

                variantofOnePartnumber = baseNumber + "-" + ActiveVariantEnd_textBox.Text + suffixCode;
                ActiveVariantEnd_abel.Text = ActiveVariantEnd_textBox.Text;

                // 採番サーバに検索開始
                supportNumbering.CheckReserveNumber(variantofOnePartnumber, this);

                CancellationToken ct = cts.Token;
                // アークスイートに検索開始
                supportArcSuite.CheckArcSuiteRegisted(variantofOnePartnumber, TitleBlock_TITLE_label.Text, arcSuitePARTNUMBER_ContainSuffixs, ct);

                InvokeRequired_Control_Enabled(ArcSuiteInformation_label, true, true);
                InvokeRequired_Control_Text(ArcSuiteInformation_label, "ArcSuiteへ問合せ中です・・", Color.Red, Color.Yellow);

                var range = variantDrawingNumberSupport.GenerateSuffixRange(rangeStart, rangeEnd);

                // アークスイートに表のすべての範囲（未登録部も含む）を検索
                //supportArcSuite.CheckArcSuiteRegistedForVariant(prefix, range, suffix, ActiveVariantEnd_textBox.Text, TITLE_Label_label.Text, arcSuitePARTNUMBER_ContainSuffixs, ct);


                // 備考入力欄へフォーカス変更
                VariantOnlyMemo_textBox.Focus();
                VariantOnlyMemo_textBox.SelectAll();

                ActiveVariantEnd_textBox.BackColor = Color.LightCyan;

            } // 数値として認識できる３ケタが入力された場合
            else
            {
                Variant_End_Input_label.ForeColor = Color.Red;
                ActiveVariantEnd_textBox.BackColor = Color.MistyRose;

            }

            // 長さが３ケタで数値変換可能ならtrueを返す
            bool _IsThreeDigitInt(string s)
            {
                // 長さが3であるかを確認
                if (s.Length != 3)
                {
                    return false;
                }

                // 数字かどうかを確認
                foreach (char c in s)
                {
                    if (!char.IsDigit(c))
                    {
                        return false;
                    }
                }

                return true;
            }
        }

        private void ActiveVariantEnd_textBox_Enter(object sender, EventArgs e)
        {
            ActiveVariantEnd_textBox.Clear();
        }

        private void VariantOnlyMemo_textBox_Enter(object sender, EventArgs e)
        {
            VariantOnlyMemo_textBox.Clear();
        }

        private void VariantOnlyMemo_textBox_TextChanged(object sender, EventArgs e)
        {
            if (VariantOnlyMemo_textBox.Text.Length > 0)
                VariantOnlyMemo_textBox.BackColor = Color.LightCyan;
            else
                VariantOnlyMemo_textBox.BackColor = Color.MistyRose;

        }

        private void Variant_panel1_VisibleChanged(object sender, EventArgs e)
        {
            if (Variant_panel1.Visible)
                ActiveVariantEnd_textBox.Focus();
        }
    }
}
