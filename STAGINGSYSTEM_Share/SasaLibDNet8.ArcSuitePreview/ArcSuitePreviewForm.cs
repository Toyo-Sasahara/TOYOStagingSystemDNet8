using SasaLib.NumberingSupport;
using SharedClassLibrary;
using StageServerRemote;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Runtime.Versioning;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SasaLib.ArcSuitePreview
{
    /// <summary>
    /// デリゲートの宣言（手動検索）
    /// </summary>
    /// <param name="SearchNumber"></param>
    /// <param name="ComponentDocumentFullFileName"></param>
    /// <param name="SearchNumber_Sub"></param>
    public delegate void Delegate_ManualSearchMethod(string SearchNumber, string ComponentDocumentFullFileName, string SearchNumber_Sub);

    /// <summary>
    /// デリゲートの宣言（Vault/コンテンツセンタ 手動検索）
    /// </summary>
    /// <param name="SearchNumber"></param>
    public delegate void Delegate_VaultManualSearchMethod(string SearchNumber);

    /// <summary>
    /// デリゲートの宣言（Vault/Inventor図面検索）
    /// </summary>
    /// <param name="SearchNumber"></param>
    public delegate void Delegate_FindDrawingfromVaultMethod(string SearchNumber);

    /// <summary>
    /// デリゲートの宣言（Vault DWG 手動検索）
    /// </summary>
    /// <param name="SearchNumber"></param>
    public delegate void Delegate_FindDWGfromVaultMethod(string SearchNumber);

    /// <summary>
    /// デリゲートの宣言 (引数なしコマンドボタン)
    /// </summary>
    public delegate void Delegate_ExternalFunctionMethod();

    /// <summary>
    /// デリゲートの宣言（Vaultにｲﾒｰｼﾞﾌｧｲﾙを送る）
    /// </summary>
    /// <param name="imageFileFullPath"></param>
    public delegate bool Delegate_SendImageToVaultMethod(string PARTNUMBER, string imageFileFullPath);

    /// <summary>
    /// デリゲートの宣言（Vaultからｲﾒｰｼﾞﾌｧｲﾙをロード）
    /// </summary>
    /// <param name="filename"></param>
    /// <param name="imageFileFullPath"></param>
    /// <returns></returns>
    public delegate bool Delegate_LoadImageFromVaultMethod(string filename, string imageFileFullPath);

    /// <summary>
    /// デリゲートの宣言（リストイラスト表示メソッド）
    /// </summary>
    /// <param name="PARTNUMBER"></param>
    /// <returns></returns>
    public delegate Image Delegate_GetPartListIllust(string PARTNUMBER);
    public delegate void Delegate_PartListIllustPictureBoxClick(string CadFullFileName);


    /// <summary>
    /// ArcSuite Previewフォームクラス
    /// </summary>
#if NETCOREAPP
    [SupportedOSPlatform("windows")]
#endif
    public partial class ArcSuitePreviewForm : Form
    {
        /// <summary>
        /// ArcSuite検索イメージを保存するフォルダ
        /// </summary>
        static string _draftCacheFullFolderName = System.IO.Path.Combine(System.Environment.GetFolderPath(Environment.SpecialFolder.Personal), "_ArcSuite図面メモ");

        /// <summary>
        /// 
        /// </summary>
        Color Set1LeftButtonDragPenColor = Color.Yellow;
        float Set1LeftButtonPenWidth = 10.0f;

        /// <summary>
        /// 
        /// </summary>
        Color Set1RightButtonDragPenColor = Color.Red;
        float Set1RightButtonPenWidth = 10.0f;

        /// <summary>
        /// 
        /// </summary>
        Color Set2LeftButtonDragPenColor = Color.Cyan;
        float Set2LeftButtonPenWidth = 10.0f;

        /// <summary>
        /// 
        /// </summary>
        Color Set2RightButtonDragPenColor = Color.Green;
        float Set2RightButtonPenWidth = 10.0f;

        /// <summary>
        /// このフォームのウィンドハンドルを保持するのに使用する
        /// </summary>
        internal static NativeWindow thisNativeWindow;

        /// <summary>
        ///  ArcsuitePreview 構造体を定義
        /// </summary>
        internal static ArcSuitePreview.ArcsuitePreview stArcSuitePreview = new ArcSuitePreview.ArcsuitePreview();

        /// <summary>
        /// フォームが表示されているかを保持
        /// </summary>
        public static bool FormShow;

        /// <summary>
        /// 
        /// </summary>
        NativeWindow parentNativeWindow;

        /// <summary>
        /// 
        /// </summary>
        public void ManualSearch_textBox_SetForcus()
        {
            ManualSearch_textBox.Focus();
        }

        /// <summary>
        /// デバッグ用
        /// </summary>
        public bool DebugMode { get; set; } = false;

        /// <summary>
        /// デリゲートメソッド 手動検索
        /// </summary>
        private Delegate_ManualSearchMethod Delegate_ManualSearchMethod;

        /// <summary>
        /// デリゲートメソッド アクティブドキュメント検索ボタン
        /// </summary>
        private Delegate_ExternalFunctionMethod Delegate_ActiveDocmentSearchMethod;

        /// <summary>
        /// デリゲートメソッド 指定したCADの計測コマンド
        /// </summary>
        private Delegate_ExternalFunctionMethod Delegate_MeasureToolMethod;

        /// <summary>
        /// デリゲートメソッド 
        /// </summary>
        private Delegate_ExternalFunctionMethod Delegate_SectionViewStartMethod;

        /// <summary>
        /// デリゲートメソッド 
        /// </summary>
        private Delegate_ExternalFunctionMethod Delegate_SectionViewEndMethod;

        /// <summary>
        /// デリゲートメソッド 部品リストイラストを表示
        /// </summary>
        private Delegate_GetPartListIllust Delegate_GetPartListIllust;

        /// <summary>
        /// デリゲートメソッド ｳｨﾝﾄﾞｳ最大化ボタン
        /// </summary>
        private Delegate_ExternalFunctionMethod Delegate_MaximizeWindowMethod;

        /// <summary>
        /// デリゲートメソッド VaultへPNGｲﾒｰｼﾞをチェックインするボタン 
        /// </summary>
        private Delegate_SendImageToVaultMethod Delegate_SendImageToVaultMethod;

        /// <summary>
        /// デリゲートメソッド Vaultから保存したPNGｲﾒｰｼﾞをロードするボタン
        /// </summary>
        private Delegate_LoadImageFromVaultMethod Delegate_LoadImageFromVaultMethod;

        /// <summary>
        /// デリゲートメソッド イラストピクチャをクリックしたときに実行させるメソッド
        /// </summary>
        private Delegate_PartListIllustPictureBoxClick Delegate_PartListIllustPictureBoxClickMethod;

        /// <summary>
        /// デリゲートメソッド Vault・コンテンツセンタから検索ボタンをクリックしたときに実行させるメソッド
        /// </summary>
        private Delegate_VaultManualSearchMethod Delegate_SearchVaultButtonClickMethod;

        /// <summary>
        /// デリゲートメソッド Vaultから Drawing ファイルを開くボタンをクリックしたときに実行させるメソッド
        /// </summary>
        private Delegate_FindDrawingfromVaultMethod Delegate_FindDrawingfromVaultButtonClickMethod;

        /// <summary>
        /// デリゲートメソッド Vaultから DWG ファイルを開くボタンをクリックしたときに実行させるメソッド
        /// </summary>
        private Delegate_FindDWGfromVaultMethod Delegate_FindAutoCADDWGfromVaultButtonClickMethod;

        /// <summary>
        /// 
        /// </summary>
        public string ArcSuiteDrawinFind_Template_URL { get; set; }

        /// <summary>
        /// Web版で検索するため
        /// </summary>
        private string ArcSuiteDrawinFindURL;

        /// <summary>
        /// ｱｰｸｽｲｰﾄ図番
        /// </summary>
        private string User_zuban;

        /// <summary>
        /// 検索を要求したCADファイル
        /// </summary>
        private string CadDataFullFileName;

        /// <summary>
        /// ArcSuiteイメージ表示ファイル名
        /// </summary>
        private string recent_temporalyDrawingImageFullFileName;

        /// <summary>
        /// TIFFｲﾒｰｼﾞﾌｧｲﾙの解像度
        /// </summary>
        private float orignalResolution;

        /// <summary>
        /// 
        /// </summary>
        //private string StageServerHost { get; set; }

        ///// <summary>
        ///// 
        ///// </summary>
        //private string PipeNameDR;

        ///// <summary>
        ///// 
        ///// </summary>
        //private string ClientDomainName;

        ///// <summary>
        ///// 
        ///// </summary>
        //private string ClientUserName;

        ///// <summary>
        ///// 
        ///// </summary>
        //private string ClientUserPassword;

        ///// <summary>
        ///// 
        ///// </summary>
        //private bool ClsLogon;

        //private bool NewStreamMode;

        //private int NewStreamModeTcpPort;

        /// <summary>
        /// アークスイートログイン名
        /// </summary>
        //private string ArcSuiteUserName;

        /// <summary>
        /// アークスイートログインパスワード（平文）
        /// </summary>
        //private string ArcSuiteUserPass;

        public ConnectionDataSet ConnectionDataSet { get; private set; }

        /// <summary>
        /// ArcSuite側へ設定・取得するCadType 列挙型 それぞれのＣＡＤアドインにて設定される
        /// </summary>
        private RemoteClientCadType.CadType SetUnsetCadType;

        /// <summary>
        /// 図面イメージの現在のページを保持するオブジェクト
        /// </summary>        
        private Bitmap sourceBitmap = null;

        /// <summary>
        /// 図面イメージのソースBitmapオブジェクト
        /// </summary>
        MultiPage drawingPreview = null;

        // 5件格納するバッファーを作成
        private RingBuffer<Bitmap> imageUndoBuffer = new RingBuffer<Bitmap>(10);

        /// <summary>
        /// 
        /// </summary>
        private int imageUndoBufferIndex = 0;

        /// <summary>
        /// pictureBoxImage描画オブジェクト
        /// </summary>        
        private Graphics pictureBoxImageGraphics = null;

        /// <summary>
        /// イメージを拡大または回転するときのアルゴリズムを指定
        /// </summary>
        private System.Drawing.Drawing2D.InterpolationMode interpolationMode = System.Drawing.Drawing2D.InterpolationMode.Low;

        /// <summary>
        /// アフィン変換行列
        /// </summary>        
        private System.Drawing.Drawing2D.Matrix _sourceMatAffine = null;

        /// <summary>
        /// 中央ボタンを押してドラッグ中
        /// </summary>
        private bool isMiddleButtonPushMouseMoving = false;

        /// <summary>
        /// 左ボタンを押してドラッグ中
        /// </summary>
        private bool isLeftButtonPushMouseMoving = false;

        /// <summary>
        /// 右ボタンを押してドラッグ中
        /// </summary>
#pragma warning disable IDE0052 // 読み取られていないプライベート メンバーを削除
        private bool isRightButtonPushMouseMoving = false;
#pragma warning restore IDE0052 // 読み取られていないプライベート メンバーを削除

        /// <summary>
        /// 中央ボタンクリック時のコントロール座標値
        /// </summary>      
        private System.Drawing.PointF oldMouseMiddleClickPoint;

        /// <summary>
        /// 左ボタンクリック時のコントロール座標値
        /// </summary>
        private System.Drawing.PointF oldMouseLeftButtonClickPoint;

        /// <summary>
        /// 右ボタンクリック時のコントロール座標値
        /// </summary>
        private System.Drawing.PointF oldMouseRightButtonClickPoint;

        /// <summary>
        /// 中央ボタンクリック時のソースビットマップ上の座標値
        /// </summary>
        private PointF oldSoureBitmapMouseMiddleButtonClickPoint;

        /// <summary>
        /// 左ボタンクリック時のソースビットマップ上の座標値
        /// </summary>
        private PointF oldSoureBitmapMouseLeftButtonClickPoint;

        /// <summary>
        /// 右ボタンクリック時のソースビットマップ上の座標値
        /// </summary>
        private PointF oldSoureBitmapMouseRightButtonClickPoint;

        /// <summary>
        /// 
        /// </summary>
        internal bool WriteTextMode;

        /// <summary>
        /// 
        /// </summary>
        private TextDrawForm textDrawForm = new TextDrawForm(null);

        /// <summary>
        /// デバッグ用
        /// </summary>
        float test_cur_x;

        /// <summary>
        /// デバッグ用
        /// </summary>
        float test_cur_y;

        /// <summary>
        /// デバッグ用
        /// </summary>
        float test_cur_scale;

        /// <summary>
        /// デバッグ出力用
        /// </summary>
        Action<string> WriteLine;


        /// <summary>
        /// ■コンストラクタ
        /// </summary>
        /// <param name="parentNativeWindow"></param>
        /// <param name="WriteLine"></param>
        public ArcSuitePreviewForm(System.Windows.Forms.NativeWindow parentNativeWindow, ConnectionDataSet connectionDataSet, Action<string> WriteLine = null)
        {
            this.parentNativeWindow = parentNativeWindow;

            ConnectionDataSet = connectionDataSet;

            // デバッグメッセージデリゲート先選択
            if (WriteLine == null) this.WriteLine = DebugConsole.Write; else this.WriteLine = WriteLine;

            //
            InitializeComponent();

            ArcSuitePreviewPictureBox.Controls.Add(ArcsuitePreviewForm_Msg_label);
            ArcSuitePreviewPictureBox.Controls.Add(ArcSuite_Status_label);

            VaultCheckInPngSuffix_textBox.Text = Properties.Resources.ArcSuiteImageSuffix;

            // マウスホイールイベント関連
            ArcSuitePreviewPictureBox.MouseWheel += pictureBox1_MouseWheel;

            // ﾍﾟｰｼﾞコントロールは通常非表示
            sasaLibBasicPageControl.Visible = false;
        }

        /// <summary>
        /// ■フォームがロードされたとき
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ArcSuitePreviewForm_Load(object sender, EventArgs e)
        {
            ArcSuite_Status_label.Text = "---";

            OnOrderClear_button.Enabled = false;

            ImageReLoad_button.Enabled = false;

            TitleBlockScale_button.Tag = false;

            button1.Tag = false;

            OP_PartListIllust_groupbox.Visible = false;

            //コンボボックスに列挙型の文字列を入れておく
            foreach (System.Drawing.Drawing2D.InterpolationMode interpolatinMode in Enum.GetValues(typeof(System.Drawing.Drawing2D.InterpolationMode)))
            {
                QualityMode_comboBox.Items.Add(interpolatinMode.ToString());
            }

            // オプション扱いのコントロールをディスエーブル
            EnabledOptionContrlol(false);

            thisNativeWindow = new System.Windows.Forms.NativeWindow();

            currentStageServer_label.Text = ConnectionDataSet.StageServerHost;

            Enable_AplicationOpenFile_button(false);

        }

        /// <summary>
        /// ■フォームが表示されたとき
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ArcSuitePreviewForm_Shown(object sender, EventArgs e)
        {
            FormShow = true;

            // タイトルバーをﾓｰﾄﾞにより変更
            if (DebugMode)
                this.Text = $"■東陽ﾂｰﾙ TESTﾓｰﾄﾞ{this.Text}";
            else
                this.Text = $"■東陽ArcSuteプレビュー";

            if (DebugMode)
            {
                Debug_panel.Enabled = true;
                Debug_panel.Visible = true;
            }
            else
            {
                Debug_panel.Enabled = false;
                Debug_panel.Visible = false;
            }

            // フォームロード時のアクティブコントロールを設定
            this.ActiveControl = this.ManualSearch_textBox;
        }

        private void flowLayoutPanel1_Resize(object sender, EventArgs e)
        {
            WindowFit();
        }

        private void WindowFit()
        {
            if (splitContainer1 != null)
                if (splitContainer1.Size.Width > 1300)
                {
                    splitContainer1.SplitterDistance = splitContainer1.Size.Height - 230;

                }
                else
                    splitContainer1.SplitterDistance = splitContainer1.Size.Height - 420;
        }

        /// <summary>
        /// ■オプション扱いのコントロールをイネーブル・ディスエーブル
        /// </summary>
        /// <param name="sw"></param>
        private void EnabledOptionContrlol(bool sw)
        {
            InvokeRequired_Control_Enabled(OP_ManualSearchPanel, sw);
            InvokeRequired_Control_Enabled(OP_CAD_MeasureTool_Panel, sw);
            InvokeRequired_Control_Enabled(OP_PartListIllust_groupbox, sw);
            InvokeRequired_Control_Enabled(OP_ActiveDocmentSearch_Button, sw);
            InvokeRequired_Control_Enabled(OP_MaximizeWindow_button, sw);
            InvokeRequired_Control_Enabled(OP_SendVault_button, sw);
            InvokeRequired_Control_Enabled(OP_LoadVault_button, sw);
            InvokeRequired_Control_Enabled(VaultCheckInPngSuffix_textBox, sw);
            InvokeRequired_Control_Enabled(Vault_ContentCenter_ComponentSearch_button, sw);
            InvokeRequired_Control_Enabled(Vault_DrawingSearch_button, sw);
            InvokeRequired_Control_Enabled(Vault_AutoCAD_DWGSearch_button, sw);
        }

        /// <summary>
        /// ■フォームが閉じようとするとき
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ArcSuitePreviewForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            //if (e.CloseReason == CloseReason.UserClosing)
            //{
            //    e.Cancel = true;
            //}

            PreviewImageCacheClear();
        }

        /// <summary>
        /// ★閉じるボタンを表示するか
        /// </summary>
        /// <param name="flag"></param>
        public void Close_Button_Enable(bool flag)
        {
            Close_panel.Visible = flag;
        }

        /// <summary>
        /// ★手動検索パネルを設定
        /// </summary>
        /// <param name="flag"></param>
        public void Enable_ManualSearchFunction(Delegate_ManualSearchMethod method)
        {
            //マニュアル検索ボタンのデリゲート先を設定
            Delegate_ManualSearchMethod = method;

            InvokeRequired_Control_Enabled(OP_ManualSearchPanel, true);
        }

        /// <summary>
        /// ★アクティブドキュメントを検索ボタンを設定 
        /// </summary>
        /// <param name="flag"></param>
        public void Enable_ActiveDocmentSearchFunction(Delegate_ExternalFunctionMethod method)
        {
            Delegate_ActiveDocmentSearchMethod = method;

            InvokeRequired_Control_Enabled(OP_ActiveDocmentSearch_Button, true);
        }

        /// <summary>
        /// ★ドキュメントをVaultから検索ボタンを設定
        /// </summary>
        /// <param name="method"></param>
        public void Enable_DocmentSearchFromVaultFunction(Delegate_VaultManualSearchMethod method)
        {
            Delegate_SearchVaultButtonClickMethod = method;
            InvokeRequired_Control_Enabled(Vault_ContentCenter_ComponentSearch_button, true);
        }

        /// <summary>
        /// ★Inventorドローイングを検索
        /// </summary>
        /// <param name="method"></param>
        public void Enable_FindDrawingfromVaultFunction(Delegate_FindDrawingfromVaultMethod method)
        {
            Delegate_FindDrawingfromVaultButtonClickMethod = method;
            InvokeRequired_Control_Enabled(Vault_DrawingSearch_button, true);
        }

        /// <summary>
        /// ★AutoCADDWGをVaultから検索して開くボタンを設定
        /// </summary>
        /// <param name="method"></param>
        public void Enable_FindDWGfromVaultFunction(Delegate_FindDWGfromVaultMethod method)
        {
            Delegate_FindAutoCADDWGfromVaultButtonClickMethod = method;
            InvokeRequired_Control_Enabled(Vault_AutoCAD_DWGSearch_button, true);
        }

        /// <summary>
        ///  ★ｳｨﾝﾄﾞｳ最大化を検索ボタンを設定
        /// </summary>
        /// <param name="method"></param>
        public void Enable_MaximizeWindowFunction(Delegate_ExternalFunctionMethod method)
        {
            Delegate_MaximizeWindowMethod = method;

            InvokeRequired_Control_Enabled(OP_MaximizeWindow_button, true);
        }

        /// <summary>
        /// ★ｲﾒｰｼﾞをVaultに送るボタンを押した時
        /// </summary>
        /// <param name="method"></param>
        public void Enable_SendImageToVaultFunction(Delegate_SendImageToVaultMethod method)
        {
            Delegate_SendImageToVaultMethod = method;

            InvokeRequired_Control_Enabled(OP_SendVault_button, true);
        }

        /// <summary>
        /// ★ｲﾒｰｼﾞをVaultからロードボタンを押した時
        /// </summary>
        /// <param name="method"></param>
        public void Enable_LoadImageFromVaultFunction(Delegate_LoadImageFromVaultMethod method)
        {
            Delegate_LoadImageFromVaultMethod = method;

            InvokeRequired_Control_Enabled(OP_LoadVault_button, true);
        }


        /// <summary>
        /// ★イラスト表示機能を設定
        /// </summary>
        /// <param name="flag"></param>
        public void Enable_ListIllustFunction(Delegate_GetPartListIllust method, Delegate_PartListIllustPictureBoxClick method2)
        {
            Delegate_GetPartListIllust = method;
            Delegate_PartListIllustPictureBoxClickMethod = method2;
            InvokeRequired_Control_Enabled(OP_PartListIllust_groupbox, true);
            InvokeRequired_Control_Visible(OP_PartListIllust_groupbox, true);
        }

        /// <summary>
        /// ★測定機能を設定
        /// </summary>
        /// <param name="MeasureToolMethod">測定ボタンのデリゲート先</param>
        /// <param name="SelectFeaturesPriorityMethod">ﾌｨｰﾁｬのみ選択ボタンのデリゲート先</param>
        /// <param name="SelectFacesAndEdgePriorityMethod">面とエッジのみ選択ボタンのデリゲート先</param>
        /// <param name="SectionViewStartMethod">断面開始ボタンのデリゲート先</param>
        /// <param name="SectionViewEndMethod">断面終了ボタンのデリゲート先</param>
        public void Enable_MeasureToolFuncton(
            Delegate_ExternalFunctionMethod MeasureToolMethod,
            Delegate_ExternalFunctionMethod SectionViewStartMethod,
            Delegate_ExternalFunctionMethod SectionViewEndMethod)
        {
            Delegate_MeasureToolMethod = MeasureToolMethod; // 

            Delegate_SectionViewStartMethod = SectionViewStartMethod; //
            Delegate_SectionViewEndMethod = SectionViewEndMethod; //

            InvokeRequired_Control_Enabled(OP_CAD_MeasureTool_Panel, true);
            InvokeRequired_Control_Visible(OP_CAD_MeasureTool_Panel, true);
            InvokeRequired_Control_Enabled(MeasureTool_Button, true);
            InvokeRequired_Control_Visible(MeasureTool_Button, true);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="sw"></param>
        public void Enable_AplicationOpenFile_button(bool sw)
        {
            InvokeRequired_Control_Visible(AplicationOpenFile_button, sw);
            InvokeRequired_Control_Enabled(AplicationOpenFile_button, sw);
        }

        /// <summary>
        /// ■マウス ボタンプッシュダウンイベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            if (DebugMode)
                showTestlabel();

            // ビットマップが未設定の場合戻る
            if (sourceBitmap == null) return;

            // フォーカスの設定（おまじない)
            ArcSuitePreviewPictureBox.Focus();

            // 中央ボタン
            if (e.Button == MouseButtons.Middle)
            {
                // マウスをクリックした位置の記録  
                oldMouseMiddleClickPoint.X = e.X;
                oldMouseMiddleClickPoint.Y = e.Y;

                // マウス移動フラグを立てる  
                isMiddleButtonPushMouseMoving = true;

                // オリジナルビットマップの座標を退避
                oldSoureBitmapMouseMiddleButtonClickPoint = GetOrginalImagePoint(e.X, e.Y, _sourceMatAffine);
            }

            // 左ボタン押下
            if (e.Button == MouseButtons.Left)
            {
                // マウスをクリックした位置の記録  
                oldMouseLeftButtonClickPoint.X = e.X;
                oldMouseLeftButtonClickPoint.Y = e.Y;

                // マウス移動フラグを立てる  
                isLeftButtonPushMouseMoving = true;

                // オリジナルビットマップの座標を退避
                oldSoureBitmapMouseLeftButtonClickPoint = GetOrginalImagePoint(e.X, e.Y, _sourceMatAffine);

                // 文字列描画モードがtrueの時はｸﾘｯｸした場所に描画
                if (textDrawForm != null && WriteTextMode == true)
                {
                    string text = textDrawForm.drawString_textBox.Text;
                    float emsize = (float)textDrawForm.FontSize_numericUpDown.Value;
                    DrawText(text, oldSoureBitmapMouseLeftButtonClickPoint.X, oldSoureBitmapMouseLeftButtonClickPoint.Y, Brushes.Red, "MS UI Gothic", emsize);
                }
            }

            // 右ボタンがクリックされたとき
            if (e.Button == MouseButtons.Right)
            {
                // マウスをクリックした位置の記録  
                oldMouseRightButtonClickPoint.X = e.X;
                oldMouseRightButtonClickPoint.Y = e.Y;

                // マウス移動フラグを立てる  
                isRightButtonPushMouseMoving = true;

                // オリジナルビットマップの座標を退避
                oldSoureBitmapMouseRightButtonClickPoint = GetOrginalImagePoint(e.X, e.Y, _sourceMatAffine);
            }

            InvokeRequired_Control_Text(MidLabel2_label, $"{imageUndoBuffer.Count} / {imageUndoBuffer.MaxCapacity} index = {imageUndoBufferIndex}");
        }

        /// <summary>
        /// ■マウス移動イベント.描画も実行
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>        
        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            if (DebugMode)
                showTestlabel();

            // ビットマップが未設定の場合戻る
            if (sourceBitmap == null) return;

            // マウスホイールをクリックしながら移動中のとき  
            if (isMiddleButtonPushMouseMoving == true)
            {
                // 以前の座標から移動量を算出
                var xx = e.X - oldMouseMiddleClickPoint.X;
                var yy = e.Y - oldMouseMiddleClickPoint.Y;


                // 画像の移動  
                _sourceMatAffine.Translate(xx, yy, MatrixOrder.Append);
                test_cur_x = xx;
                test_cur_y = yy;

                // ポインタ位置を退避します  
                oldMouseMiddleClickPoint.X = e.X;
                oldMouseMiddleClickPoint.Y = e.Y;
            }

            // 左またはボタン押下されながら移動中の場合
            if (isLeftButtonPushMouseMoving == true || isRightButtonPushMouseMoving == true)
            {
                int curXX = GetOrginalImagePoint(e.X, e.Y, _sourceMatAffine).X;
                int curYY = GetOrginalImagePoint(e.X, e.Y, _sourceMatAffine).Y;

                Graphics g = Graphics.FromImage(sourceBitmap);

                Pen pen = null;
                if (ColorSet2_checkBox.Checked)
                {
                    if (isLeftButtonPushMouseMoving)
                        pen = new Pen(Set2LeftButtonDragPenColor, Set2LeftButtonPenWidth);
                    else if (isRightButtonPushMouseMoving)
                        pen = new Pen(Set2RightButtonDragPenColor, Set2RightButtonPenWidth);
                }
                else
                {
                    if (isLeftButtonPushMouseMoving)
                        pen = new Pen(Set1LeftButtonDragPenColor, Set1LeftButtonPenWidth);
                    else if (isRightButtonPushMouseMoving)
                        pen = new Pen(Set1RightButtonDragPenColor, Set1RightButtonPenWidth);
                }

                if (isLeftButtonPushMouseMoving)
                    g.DrawLine(pen, curXX, curYY, oldSoureBitmapMouseLeftButtonClickPoint.X, oldSoureBitmapMouseLeftButtonClickPoint.Y);
                if (isRightButtonPushMouseMoving)
                    g.DrawLine(pen, curXX, curYY, oldSoureBitmapMouseRightButtonClickPoint.X, oldSoureBitmapMouseRightButtonClickPoint.Y);

                g.Dispose();
                pen.Dispose();


                if (DebugMode)
                {
                    label3.Text = $"マウスポインタ座標:({e.X},{e.Y}) 実イメージ上の座標値:({oldSoureBitmapMouseLeftButtonClickPoint.X:#.#}, {oldSoureBitmapMouseLeftButtonClickPoint.Y:#.#}) ";
                }

                // 現在のソースびっとまぷ上のマウスの位置を退避する
                if (isLeftButtonPushMouseMoving)
                    oldSoureBitmapMouseLeftButtonClickPoint = GetOrginalImagePoint(e.X, e.Y, _sourceMatAffine);
                else if (isRightButtonPushMouseMoving)
                    oldSoureBitmapMouseRightButtonClickPoint = GetOrginalImagePoint(e.X, e.Y, _sourceMatAffine);
                else if (isMiddleButtonPushMouseMoving)
                    oldSoureBitmapMouseMiddleButtonClickPoint = GetOrginalImagePoint(e.X, e.Y, _sourceMatAffine);

                SavedMsg_label.Text = null;
            }

            // 画像の描画  
            DrawImage();
        }

        /// <summary>
        /// ■マウスアップイベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            // ビットマップが未設定の場合戻る
            if (sourceBitmap == null) return;

            if (e.Button == MouseButtons.Middle)
            {
                // マウス移動フラグを解放  
                isMiddleButtonPushMouseMoving = false;
            }
            if (e.Button == MouseButtons.Left)
            {
                // マウス移動フラグを解放  
                isLeftButtonPushMouseMoving = false;
            }
            if (e.Button == MouseButtons.Right)
            {
                // マウス移動フラグを解放  
                isRightButtonPushMouseMoving = false;
            }

            imageUndoBuffer.Add(new Bitmap(sourceBitmap));
            imageUndoBufferIndex = imageUndoBuffer.Count - 1;


        }

        /// <summary>
        /// ■描画をひとつ前に戻す
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UNDO_button_Click(object sender, EventArgs e)
        {
            InvokeRequired_Control_Text(MidLabel2_label, $"{imageUndoBuffer.Count} / {imageUndoBuffer.MaxCapacity} index = {imageUndoBufferIndex}");
            if (imageUndoBuffer.Count > 0)
            {
                if (imageUndoBuffer.Count > imageUndoBufferIndex && imageUndoBufferIndex >= 0)
                {
                    if (imageUndoBufferIndex > 0)
                        imageUndoBufferIndex--;
                    sourceBitmap = imageUndoBuffer[imageUndoBufferIndex];
                    DrawImage();
                }
            }
            InvokeRequired_Control_Text(MidLabel2_label, $"{imageUndoBuffer.Count} / {imageUndoBuffer.MaxCapacity} index = {imageUndoBufferIndex}");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void REDO_button_Click(object sender, EventArgs e)
        {
            InvokeRequired_Control_Text(MidLabel2_label, $"{imageUndoBuffer.Count} / {imageUndoBuffer.MaxCapacity} index = {imageUndoBufferIndex}");

            if (imageUndoBuffer.Count > 0)
            {
                if (imageUndoBuffer.Count - 1 > imageUndoBufferIndex && imageUndoBufferIndex >= 0)
                {
                    if (imageUndoBufferIndex < imageUndoBuffer.Count)
                        imageUndoBufferIndex++;
                    sourceBitmap = imageUndoBuffer[imageUndoBufferIndex];
                    DrawImage();
                }
            }
            InvokeRequired_Control_Text(MidLabel2_label, $"{imageUndoBuffer.Count} / {imageUndoBuffer.MaxCapacity} index = {imageUndoBufferIndex}");
        }


        /// <summary>
        /// ■マウスホイールイベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>        
        private void pictureBox1_MouseWheel(object sender, MouseEventArgs e)
        {
            // ビットマップが未設定の場合戻る
            if (sourceBitmap == null) return;

            // 一旦ポインタの位置→原点へ移動  
            _sourceMatAffine.Translate(-e.X, -e.Y, MatrixOrder.Append);
            test_cur_x = -e.X;
            test_cur_y = -e.Y;


            // ホイールの回転回数が、
            if (e.Delta > 0)
            {
                // 正の場合拡大
                if (_sourceMatAffine.Elements[0] > 0.01)
                {
                    // 最小倍率以内の場合、1.0/1.5縮小
                    _sourceMatAffine.Scale(1.0f / 1.5f, 1.0f / 1.5f, MatrixOrder.Append);
                }
            }
            else
            {
                // 負の場合縮小  
                if (_sourceMatAffine.Elements[0] < 100)
                {
                    // 最大倍率以内の場合、1.5拡大  
                    _sourceMatAffine.Scale(1.5f, 1.5f, MatrixOrder.Append);
                }
            }

            // 原点→ポインタの位置へ移動(元の位置へ戻す)  
            _sourceMatAffine.Translate(e.X, e.Y, MatrixOrder.Append);
            test_cur_x = e.X;
            test_cur_y = e.Y;


            // 画像の描画  
            DrawImage();
        }

        /// <summary>
        /// ●コントロールの座標から元のビットマップの座標を変換する
        /// </summary>
        /// <param name="X"></param>
        /// <param name="Y"></param>
        /// <returns></returns>
        private System.Drawing.Point GetOrginalImagePoint(float X, float Y, System.Drawing.Drawing2D.Matrix sourceMattAffine)
        {
            // 元の座標を求めるテスト
            using (var matInvert = sourceMattAffine.Clone())
            {
                // アフィン変換行列の逆行列を求める
                matInvert.Invert();

                // コントロールの座標
                var points = new PointF[]
                    {
                        new PointF(X, Y)
                    };

                // 元の座標(画像上の座標）を求める
                matInvert.TransformPoints(points);

                return new Point((int)points[0].X, (int)points[0].Y);
            }
        }

        /// <summary>
        /// ●右下に文字列の描画
        /// </summary>
        /// <param name="Text"></param>
        /// <param name="fontname"></param>
        /// <param name="emSize"></param>
        private void DrawTextRightButtom(string Text, string fontname = "MS UI Gothic", float emSize = 40)
        {
            if (sourceBitmap == null) return;
            Graphics g = Graphics.FromImage(sourceBitmap);

            //フォントオブジェクトの作成
            Font fnt = new Font(fontname, emSize);

            float sourceBitmapWidth = sourceBitmap.Width;
            float sourceBitmapHeight = sourceBitmap.Height;

            float fx;
            float fy;

            var size = g.MeasureString(Text, fnt);

            fx = sourceBitmapWidth - size.Width - 2;
            fy = sourceBitmapHeight - size.Height - 2;

            g.FillRectangle(Brushes.White, fx, fy, size.Width, size.Height);

            //文字列を位置(0,0)、青色で表示
            g.DrawString(Text, fnt, Brushes.Blue, fx, fy);


            //リソースを解放する
            fnt.Dispose();
            g.Dispose();

            // 画像の描画  
            DrawImage();
        }

        /// <summary>
        /// ●指定座標に文字列を描画
        /// </summary>
        /// <param name="Text"></param>
        /// <param name="fx"></param>
        /// <param name="fy"></param>
        /// <param name="brush"></param>
        /// <param name="fontname"></param>
        /// <param name="emSize"></param>
        internal void DrawText(string Text, float fx, float fy, Brush brush, string fontname = "MS UI Gothic", float emSize = 40)
        {
            if (sourceBitmap == null) return;
            Graphics g = Graphics.FromImage(sourceBitmap);

            //フォントオブジェクトの作成
            Font fnt = new Font(fontname, emSize);


            var size = g.MeasureString(Text, fnt);


            g.FillRectangle(Brushes.White, fx, fy, size.Width, size.Height);

            //文字列を位置(0,0)、青色で表示
            g.DrawString(Text, fnt, brush, fx, fy);


            //リソースを解放する
            fnt.Dispose();
            g.Dispose();

            // 画像の描画  
            DrawImage();

        }

        /// <summary>
        /// ●デバッグ用メソッド
        /// </summary>
        private void TestCheckImageData()
        {
            /// <summary>
            /// 画像の左上、右上、左下の座標
            /// </summary>
            System.Drawing.PointF[] points;

            // 画像の画素の外側の領域
            var sorceBitmapRect = new RectangleF(
                    -0.5f,
                    -0.5f,
                    sourceBitmap.Width,
                    sourceBitmap.Height
            );
            // 画像の左上、右上、左下の座標
            points = new PointF[] {
                    new PointF(sorceBitmapRect.Left, sorceBitmapRect.Top),  // 左上
                    new PointF(sorceBitmapRect.Right, sorceBitmapRect.Top), // 右上
                    new PointF(sorceBitmapRect.Left, sorceBitmapRect.Bottom)// 左下
                };
            var pointsstr = $"左上 {points[0].X},{points[0].Y} 右上 {points[1].X},{points[1].Y} 左下 {points[2].X},{points[2].Y} ";
            //Console.WriteLine(pointsstr);

            InvokeRequired_Control_Text(label1, $"解像度 Width{sourceBitmap.VerticalResolution} / Height{sourceBitmap.HorizontalResolution} | (Width,Height) = ( {sourceBitmap.Width} , {sourceBitmap.Height} )");
            InvokeRequired_Control_Text(label2, $"左上:{points[0].X},{points[0].Y} | 右上:{points[1].X},{points[1].Y} | 左下:{points[2].X},{points[2].Y} ");
        } //情報のみ取得デバッグ用

        /// <summary>
        /// ●ビットマップの描画
        /// </summary>        
        private void DrawImage()
        {
            // ビットマップが未設定の場合戻る
            if (sourceBitmap == null) return;

            if (DebugMode)
                TestCheckImageData();

            // アフィン変換行列をセット
            if (_sourceMatAffine != null)
            {
                //pictureBoxImageGraphics.Transform = _sourceMatAffine;
                InvokeRequired_Graphics_Transform(pictureBoxImageGraphics, _sourceMatAffine);
            }
            if (pictureBoxImageGraphics != null)
            {
                // ピクチャボックスを背景色でクリア  
                pictureBoxImageGraphics.Clear(ArcSuitePreviewPictureBox.BackColor);
            }

            // 画素補完モード指定
            //     System.Drawing.Drawing2D.QualityMode.Invalid 列挙体の要素 System.Drawing.Drawing2D.QualityMode
            //     と等価。
            //Invalid = -1,
            //     既定のモードを指定します。
            //Default = 0,
            //     低品質補間を指定します。
            //Low = 1,
            //     高品質補間を指定します。
            //High = 2,
            //     双一次補間を指定します。 事前フィルター処理は実行されません。 このモードは、イメージを元のサイズの 50% 以下にするような縮小処理には適していません。
            //Bilinear = 3,
            //     双三次補間を指定します。 事前フィルター処理は実行されません。 このモードは、イメージを元のサイズの 25% 以下にするような縮小処理には適していません。
            //Bicubic = 4,
            //     最近傍補間を指定します。
            //NearestNeighbor = 5,
            //     高品質双一次補間を指定します。 事前フィルター処理が適用され、高品質の縮小処理が実行されます。
            //HighQualityBilinear = 6,
            //     高品質双三次補間を指定します。 事前フィルター処理が適用され、高品質の縮小処理が実行されます。 このモードを使用すると、変換後のイメージが高品質になります。
            //HighQualityBicubic = 7

            try
            {
                pictureBoxImageGraphics.InterpolationMode = interpolationMode;
            }
            catch (Exception ex)
            {
                Eventlog.Log.WriteEntry("SasaLibArcSuitePreview", EventLogEntryType.Error, 0, $"※ArcSuitePreviewForm.DrawImage(..) graphics.InterpolationMode = interpolationMode; にて例外 {ex.Message}");
            }

            // 描画
            //pictureBoxImageGraphics.DrawImage(sourceBitmap, 0, 0);
            InvokeRequired_Graphics_DrawImage(pictureBoxImageGraphics, sourceBitmap, 0, 0);

            Debug_panel.BackColor = Color.Transparent;


            // ピクチャボックスをリフレッシュし再描画          
            //if (InvokeRequired)
            //{
            //    Invoke(new Action(() =>
            //    {
            //        /// UIを操作する処理
            //        ArcSuitePreviewPictureBox.Refresh();
            //    }));
            //}
            //else
            //{
            //    ArcSuitePreviewPictureBox.Refresh();
            //}

            MethodInvoker method = () =>
            {
                // コントロールに対する処理
                ArcSuitePreviewPictureBox.Refresh();
            };
            if (InvokeRequired) { Invoke(method); } else { method(); }



        }

        /// <summary>
        /// ■フォームがリサイズされた
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ArcSuitePreviewForm_Resize(object sender, EventArgs e)
        {
            pictureBoxRedraw();

            ///*
            //フォームのウィンドウサイズが変更されると、フォーム上のPictureBoxのサイズも変更される。
            //PictreuBoxのサイズ一杯に画像表示領域を確保する場合、サイズ変更のイベントと合わせて、
            //表示領域用ビットマップオブジェクトの再作成が必要になる。
            //*/
            //try
            //{
            //    // Graphicsオブジェクトが取得済みの場合
            //    if (pictureBoxImageGraphics != null)
            //    {
            //        // Graphicsオブジェクトからアフィン変換行列を取得し退避
            //        _sourceMatAffine = pictureBoxImageGraphics.Transform;

            //        // Graphicsオブジェクトの解放  
            //        pictureBoxImageGraphics.Dispose(); pictureBoxImageGraphics = null;
            //    }
            //    else
            //    {
            //        _sourceMatAffine = new System.Drawing.Drawing2D.Matrix();
            //    }

            //    // Imageを破棄
            //    if (ArcSuitePreviewPictureBox.Image != null) ArcSuitePreviewPictureBox.Image.Dispose();

            //    // PictureBoxに描画領域用の空のビットマップを割り当てる。
            //    ArcSuitePreviewPictureBox.Image = new Bitmap(ArcSuitePreviewPictureBox.Width, ArcSuitePreviewPictureBox.Height);

            //    // PictureBoxから新たなGraphicsオブジェクトを取得し退避  
            //    pictureBoxImageGraphics = Graphics.FromImage(ArcSuitePreviewPictureBox.Image);

            //    // 補間モードの設定（NearestNeighbor）  
            //    pictureBoxImageGraphics.InterpolationMode = InterpolationMode.NearestNeighbor;

            //    // 画像の描画  
            //    DrawImage();

            //}
            //catch (Exception ex)
            //{
            //    Console.WriteLine($"※ArcSuitePreviewForm_Resize(..)にて例外 {ex.Message}");
            //}

        }

        /// <summary>
        /// ■スプリットバーが移動された
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void splitContainer1_SplitterMoved(object sender, SplitterEventArgs e)
        {
            pictureBoxRedraw();
        }

        /// <summary>
        /// ■InterpolationModeコンボボックスの選択が変更されたら、再描画
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void QualityMode_comboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            interpolationMode = (System.Drawing.Drawing2D.InterpolationMode)Enum.Parse(typeof(System.Drawing.Drawing2D.InterpolationMode), QualityMode_comboBox.SelectedItem.ToString());
            DrawImage();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="url"></param>
        public void SetArcSuiteDrawinFindURL(string url)
        {
            this.ArcSuiteDrawinFindURL = url;
        }

        /// <summary>
        /// ■Webブラウザでｱｰｸｽｲｰﾄ図面を検索
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ArcSuiteWebSearchAndView_button_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(ArcSuiteDrawinFindURL) != true)
            {
                try
                {

#if NETCOREAPP
                    System.Diagnostics.Process.Start(new ProcessStartInfo
                    {
                        FileName = ArcSuiteDrawinFindURL,
                        UseShellExecute = true // システムのデフォルトアプリケーションを使用
                    });
#else
                            System.Diagnostics.Process.Start(ArcSuiteDrawinFindURL);
#endif

                }
                catch (Exception ex)
                {
                    Eventlog.Log.WriteEntry("SasaLibArcSuitePreview", EventLogEntryType.Error, 0, $"※ArcSuiteSearchAndVew_button_Click(..)  にて例外 {ex.Message}");
                }
            }
            else if (string.IsNullOrWhiteSpace(ArcSuiteDrawinFind_Template_URL) != true)
            {
                ArcSuiteDrawinFindURL = ArcSuiteDrawinFind_Template_URL.Replace("{SANITIZEDPARTNUMBER}", ManualSearch_textBox.Text);

                try
                {

#if NETCOREAPP
                    System.Diagnostics.Process.Start(new ProcessStartInfo
                    {
                        FileName = ArcSuiteDrawinFindURL,
                        UseShellExecute = true // システムのデフォルトアプリケーションを使用
                    });
#else
                            System.Diagnostics.Process.Start(ArcSuiteDrawinFindURL);
#endif

                }
                catch (Exception ex)
                {
                    Eventlog.Log.WriteEntry("SasaLibArcSuitePreview", EventLogEntryType.Error, 0, $"※ArcSuiteSearchAndVew_button_Click(..)  にて例外 {ex.Message}");
                }

            }
        }

        /// <summary>
        ///  ■閉じるボタンが押されたとき
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Hide_button_Click(object sender, EventArgs e)
        {
            this.Hide();
            FormShow = false;

            GC.Collect();
        }


        /// <summary>
        /// ●スレッド対応・システムメッセージを表示
        /// </summary>
        /// <param name="msg"></param>
        public void MessageSet(string msg)
        {
            MethodInvoker method = () =>
            {
                // コントロールに対する処理
                ArcsuitePreviewForm_Msg_label.Text = msg;
            };
            if (InvokeRequired) { Invoke(method); } else { method(); }
        }

        /// <summary>
        /// ●スレッド対応・AcsSuiteStatusLabeラベル(ｱｰｸｽｲｰﾄ属性：状態)にメッセージセット
        /// </summary>
        /// <param name="msg"></param>
        public void AcsSuiteStatusLabelMessageSet(string msg, System.Drawing.Color color = default)
        {
            MethodInvoker method = () =>
            {
                // コントロールに対する処理
                ArcSuite_Status_label.Text = msg;
                ArcSuite_Status_label.ForeColor = color;
            };
            if (InvokeRequired) { Invoke(method); } else { method(); }
        }

        /// <summary>
        /// ●スレッド対応のArcSuiteCreatedOnラベル(ｱｰｸｽｲｰﾄ属性：登録日時)にメッセージセット.カラー指示あり
        /// </summary>
        /// <param name="ArcSuiteCreatedOn_label_Text"></param>
        /// <param name="foregroundColor"></param>
        /// <param name="ArcSuiteCreatedOn_label_ToolTip"></param>
        public void ArcSuiteCreatedOn_label_MessageSet(string ArcSuiteCreatedOn_label_Text, Color foregroundColor, string ArcSuiteCreatedOn_label_ToolTip)
        {
            MethodInvoker method = () =>
            {
                // コントロールに対する処理
                ArcSuiteCreatedOn_label.ForeColor = foregroundColor;
                ArcSuiteCreatedOn_label.Text = ArcSuiteCreatedOn_label_Text;
                toolTip1.SetToolTip(this.ArcSuiteCreatedOn_label, $"{ArcSuiteCreatedOn_label_ToolTip}");
            };
            if (InvokeRequired) { Invoke(method); } else { method(); }
        }

        /// <summary>
        /// ●スレッド対応のコントロールのVisibleﾌﾟﾛﾊﾟﾃｨのセット
        /// </summary>
        /// <param name="control"></param>
        /// <param name="Visible"></param>
        private void InvokeRequired_Control_Visible(Control control, bool Visible)
        {
            MethodInvoker method = () =>
            {
                // コントロールに対する処理
                control.Visible = Visible;
            };
            if (InvokeRequired) { Invoke(method); } else { method(); }
        }

        /// <summary>
        /// ●スレッド対応のコントロールのtextﾌﾟﾛﾊﾟﾃｨのセット
        /// </summary>
        /// <param name="control"></param>
        /// <param name="text"></param>
        private void InvokeRequired_Control_Text(Control control, string text)
        {
            MethodInvoker method = () =>
            {
                // コントロールに対する処理
                control.Text = text;
            };
            if (InvokeRequired) { Invoke(method); } else { method(); }
        }

        /// <summary>
        /// ●スレッド対応のコントロールのtextﾌﾟﾛﾊﾟﾃｨのセット。文字色指定あり
        /// </summary>
        /// <param name="control"></param>
        /// <param name="text"></param>
        /// <param name="ForColor"></param>
        private void InvokeRequired_Control_Text(Control control, string text, Color ForColor)
        {
            MethodInvoker method = () =>
            {
                // コントロールに対する処理
                control.Text = text;
                control.ForeColor = ForColor;
            };
            if (InvokeRequired) { Invoke(method); } else { method(); }
        }

        /// <summary>
        /// ●スレッド対応のコントロールのイネーブルのセット
        /// </summary>
        /// <param name="control"></param>
        /// <param name="Enabled"></param>
        private void InvokeRequired_Control_Enabled(Control control, bool Enabled)
        {
            MethodInvoker method = () =>
            {
                // コントロールに対する処理
                control.Enabled = Enabled;
            };
            if (InvokeRequired) { Invoke(method); } else { method(); }
        }

        /// <summary>
        /// ●スレッド対応のGraphics.Transform
        /// </summary>
        /// <param name="graphics"></param>
        /// <param name="matrix"></param>
        private void InvokeRequired_Graphics_Transform(Graphics graphics, System.Drawing.Drawing2D.Matrix matrix)
        {
            try
            {

                MethodInvoker method = () =>
                {
                    // コントロールに対する処理
                    graphics.Transform = matrix;
                };
                if (InvokeRequired) { Invoke(method); } else { method(); }

            }
            catch (Exception ex)
            {
                this.WriteLine($"※InvokeRequired_Graphics_Transform(...)にて例外{ex.Message}");
            }

        }

        /// <summary>
        /// ●スレッド対応のGraphics.DrawImage
        /// </summary>
        /// <param name="graphics"></param>
        /// <param name="image"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        private void InvokeRequired_Graphics_DrawImage(Graphics graphics, System.Drawing.Image image, int x, int y)
        {
            try
            {

                MethodInvoker method = () =>
                {
                    // コントロールに対する処理
                    graphics.DrawImage(image, x, y);
                };
                if (InvokeRequired) { Invoke(method); } else { method(); }

            }
            catch (Exception ex)
            {
                this.WriteLine($"※InvokeRequired_Graphics_DrawImage(...)にて例外{ex.Message}");
            }
        }

        /// <summary>
        /// ●メッセージクリア
        /// </summary>
        public void PreviewMsgClear()
        {
            ArcSuitePreviewPictureBox.Image = null;
            PartListIllust_pictureBox.Image = null;

            Enable_AplicationOpenFile_button(false);

            InvokeRequired_Control_Enabled(ArcSuiteAttr_groupBox, true);

            MessageSet("");

            InvokeRequired_Control_Text(FindTimeStamp_label, "");

            InvokeRequired_Control_Text(modelcreationonorder_label, "");

            InvokeRequired_Control_Text(DrawingInfoLabel2, "");

            InvokeRequired_Control_Text(DrawingInfoLabel3, "");

            InvokeRequired_Control_Text(DrawingInfoLabel4, "");

            ArcSuiteDrawinFindURL = "";

            InvokeRequired_Control_Text(ArcSuite_Status_label, "");

            InvokeRequired_Control_Text(ArcSuiteCreatedOn_label, "");

            InvokeRequired_Control_Text(modelcreationonorder_label, "");

            InvokeRequired_Control_Text(SavedMsg_label, "");

            DoEvents.Run();
        }

        /// <summary>
        /// ■ｲﾒｰｼﾞをクリア
        /// </summary>
        public void ClearSourceBitmapAndPreviewImage()
        {
            sourceBitmap = null;
            ArcSuitePreviewPictureBox.Image = null;
        }

        /// <summary>
        /// ●ArcSuiteイメージﾌﾟﾚﾋﾞｭｰﾌｧｲﾙをフォルダごと削除
        /// </summary>
        private void PreviewImageCacheClear()
        {
            if (string.IsNullOrWhiteSpace(this.recent_temporalyDrawingImageFullFileName) == false)
            {
                string removeFolder = System.IO.Path.GetDirectoryName(this.recent_temporalyDrawingImageFullFileName);
                var result = FileFolder.RemoveFolder(removeFolder, true);
                if (result == true)
                    DebugConsole.WriteLine($"■ArcSuiteイメージﾌﾟﾚﾋﾞｭｰﾌｧｲﾙ {this.recent_temporalyDrawingImageFullFileName}をフォルダごと削除しました");
                else
                    DebugConsole.WriteLine($"※ArcSuiteイメージﾌﾟﾚﾋﾞｭｰﾌｧｲﾙ {this.recent_temporalyDrawingImageFullFileName}をフォルダごと削除に失敗しました");
            }
        }

        /// <summary>
        /// ■CADタイプボタン押下
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UserCadType_AddRemove_button_Click(object sender, EventArgs e)
        {
            // SetUnsetCadType が NotSetの場合は この機能は使わないことにする
            if (this.SetUnsetCadType == RemoteClientCadType.CadType.NotSet)
            {
                MessageBox.Show("このCADアドインでは対応していません", "■ごめんなさい");
                return;
            }

            string recentTtile = UserCadType_AddRemove_button.Text;
            UserCadType_AddRemove_button.Text = "処理中";

            InvokeRequired_Control_Enabled(UserCadType_AddRemove_button, false);

            CadTypeArcSuiteControl cadSetControl = new CadTypeArcSuiteControl(ConnectionDataSet);
            Task.Run(async () =>
            {
                //cadSetControl.SetUnsetCadTypeFlag(User_zuban, ref msg, this.SetUnsetCadType, thisNativeWindow, false);
                var result = await cadSetControl.SetUnsetCadTypeFlagAsync(User_zuban, this.SetUnsetCadType, thisNativeWindow, MsgBoxShow: false, objectConvNew: true, WriteLine: WriteLine);

                InvokeRequired_Control_Text(UserCadType_AddRemove_button, recentTtile);

                InvokeRequired_Control_Text(DrawingInfoLabel4, $"CADﾀｲﾌﾟ:{cadSetControl.CurrentCadTypeString}");

                InvokeRequired_Control_Enabled(UserCadType_AddRemove_button, true);
            });
        }

        /// <summary>
        /// ●CADタイプを設定または解除
        /// </summary>
        /// <param name="StageServerHost"></param>
        /// <param name="PipeNameDR"></param>
        /// <param name="ClientDomainName"></param>
        /// <param name="ClientUserName"></param>
        /// <param name="ClientUserPassword"></param>
        /// <param name="ClsLogon"></param>
        /// <param name="cadType"></param>
        /// <param name="ArcSuiteUserName"></param>
        /// <param name="ArcSuiteUserPass"></param>
        public void SetUnsetCadTypeFlagControlDatas(RemoteClientCadType.CadType cadType)
        {


            this.SetUnsetCadType = cadType;
            this.UserCadType_AddRemove_button.Enabled = true;

            this.Update();
        }

        /// <summary>
        /// ●指定した文字列でヒットする図面番号の ArcSuiteオブジェクトについて指定した属性の属性値を削除する
        /// </summary>
        /// <param name="inputString">検索する文字列（ArcSuiteでの図面番号）</param>
        /// <param name="attributeName">削除する属性名</param>
        /// <param name="nativeWindow">このメソッドでの結果をどのｳｨﾝﾄﾞｳの子として表示するか</param>
        /// <param name="WrteLine"></param>
        public bool ArcSuieAttributeStringErace(string inputString, string attributeName, NativeWindow nativeWindow = null)
        {

            if (string.IsNullOrWhiteSpace(inputString) == false)
            {
                string arcSuiteZuban = ArcSuiteSupport.ConvertSanitaizedPartnumber(inputString);

                string attributeString = @"\0";

                WriteLine($"■ArcSuite図面属性変更開始 {arcSuiteZuban} Attr = \"{attributeName}\" Value = \"{attributeString}\"");

                RemoteClientCadType rmcCadType = new RemoteClientCadType(ConnectionDataSet);

                // 処理１ 検索
                bool result = rmcCadType.MergeArcSuiteAttribute1(arcSuiteZuban, attributeName, attributeString, ConnectionDataSet.ArcSuiteUserName, ConnectionDataSet.ArcSuiteUserPass, objectConvNew: true);

                if (result)
                {
                    //MessageBox.Show(nativeWindow, $"ArcSuite図面：{arcSuiteZuban} 属性：{attributeName} の文字列値を削除完了", "■ArcSuite図面 指定属性値 削除結果");
                    return true;
                }
                else
                {
                    MessageBox.Show(nativeWindow, $"ArcSuite図面：{arcSuiteZuban} 属性：{attributeName} の文字列値の削除に失敗しました\nﾕｰｻﾞｰｱｶｳﾝﾄが違う,ArcSuite制御ｻｰﾊﾞｰに以上が発生している可能性があります", "■ArcSuite図面 指定属性値 削除結果");
                    return false;
                }
            }
            else
            {
                MessageBox.Show($"ArcSuite図面が指定されていません", "■ArcSuite図面 指定属性値 削除結果");
                return false;
            }
        }

        /// <summary>
        /// ■3D作成発注中をクリア
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnOrderClear_button_Click(object sender, EventArgs e)
        {
            string recentTtile = OnOrderClear_button.Text;
            OnOrderClear_button.Text = "処理中";

            InvokeRequired_Control_Enabled(OnOrderClear_button, false);

            //削除実行
            bool ret = ArcSuieAttributeStringErace(User_zuban, "user:modelcreationonorder", thisNativeWindow);
            if (ret)
            {
                modelcreationonorder_label.Text = $"3D作成発注中: 値はありません";
                InvokeRequired_Control_Enabled(OnOrderClear_button, false);
            }
            else
            {
                InvokeRequired_Control_Enabled(OnOrderClear_button, true);
            }

            InvokeRequired_Control_Text(OnOrderClear_button, recentTtile);
        }

        /// <summary>
        /// ■
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ArcSuitePreviewForm_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                WriteLine($"■テストコード ArcSuitePreviewForm_KeyUp(..) Escapeキー押下検知");
                this.Close();
            }
        }

        /// <summary>
        /// ●文字列描画ウィンドを表示
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DrawText_button_Click(object sender, EventArgs e)
        {
            textDrawForm = new TextDrawForm(this);
            textDrawForm.Show();
            textDrawForm.TopMost = true;
        }

        /// <summary>
        /// ●ピクチャーボックスの図面を マイドキュメントに保存する
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SaveCurrent_button_Click(object sender, EventArgs e)
        {
            WriteLine($"■ArcSuitePreviewForm.SaveCurrent_button_Click ボタンが押されました user_zuban = {User_zuban}");
            if (sourceBitmap == null)
            {
                WriteLine($"sourceBitmap は null");

                return;
            }

            if (string.IsNullOrWhiteSpace(User_zuban) == true) return;

            string machine = Environment.MachineName;
            string user = Environment.UserName;

            DrawTextRightButtom($"{user}\\{machine} {DateTime.Now}");

            string saveFullFileName = System.IO.Path.Combine(_draftCacheFullFolderName, User_zuban + ".png");

            try
            {

                if (System.IO.Directory.Exists(_draftCacheFullFolderName) == false)
                    System.IO.Directory.CreateDirectory(_draftCacheFullFolderName);

                if (System.IO.File.Exists(saveFullFileName))
                {
                    //　強制的に書き込み可能へ
                    FileFolder.SetReadOnly(saveFullFileName, false);

                    FileFolder.RemoveFile(saveFullFileName);
                }

                sourceBitmap.Save(saveFullFileName, ImageFormat.Png);

                if (System.IO.File.Exists(saveFullFileName))
                {
                    InvokeRequired_Control_Text(SavedMsg_label, $"保存済み");
                    //MessageBox.Show($"{saveFullFileName} を保存しました", "イメージ保存結果");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"※{saveFullFileName} を保存できません {ex.Message}", "エラー：例外発生");
            }
        }

        /// <summary>
        /// ■図面メモボタンクリック
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Load_button_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(User_zuban) == true) return;

            string fileName = User_zuban + ".png";

            string saveFullFileName = System.IO.Path.Combine(_draftCacheFullFolderName, fileName);

            if (System.IO.File.Exists(saveFullFileName))
            {
                // イメージファイルの読み込みとセット
                sourceBitmap = (Bitmap)ImageUtil.FromFile(saveFullFileName);
                //orignalResolution = System.Math.Max(sourceBitmap.HorizontalResolution, sourceBitmap.VerticalResolution);

                // 初期化の為リサイズイベントを強制的に実行
                //ArcSuitePreviewForm_Resize(null, null);
                pictureBoxRedraw();

                DrawImage();
            }
        }

        /// <summary>
        /// ■Vaultに送る
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OP_SendVault_button_Click(object sender, EventArgs e)
        {
            // このフォームが表示中の図面番号が存在するか調査
            if (string.IsNullOrWhiteSpace(User_zuban) == true) return;

            SaveCurrent_button_Click(sender, e); // マイドキュメントに保存します

            string machine = Environment.MachineName;
            string user = Environment.UserName;

            // タイムスタンプを右下の描画
            DrawTextRightButtom($"{user}\\{machine} {DateTime.Now}");

            // Vaultにチェックインする時のﾌｧｲﾙ名ｻﾌｨｯｸｽを決定
            string suffix;
            if (string.IsNullOrWhiteSpace(VaultCheckInPngSuffix_textBox.Text) == false)
                suffix = VaultCheckInPngSuffix_textBox.Text;
            else
                suffix = "";

            // Vaultにチェックインするファイルのフルファイル名
            string saveFullFileName = System.IO.Path.Combine(_draftCacheFullFolderName, User_zuban + suffix + ".png");

            try
            {
                // 図面メモのためのフォルダを作成
                if (System.IO.Directory.Exists(_draftCacheFullFolderName) == false)
                    System.IO.Directory.CreateDirectory(_draftCacheFullFolderName);

                // 既存ﾌｧｲﾙは上書きするため強制削除
                if (System.IO.File.Exists(saveFullFileName))
                {
                    //　強制的に書き込み可能へ
                    FileFolder.SetReadOnly(saveFullFileName, false);

                    // 既存図削除
                    FileFolder.RemoveFile(saveFullFileName);
                }

                // チェックインするためのファイルを生成
                sourceBitmap.Save(saveFullFileName, ImageFormat.Png);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"※{saveFullFileName} を保存できません {ex.Message}", "エラー：例外発生");
            }

            // Vaultにチェックインするために 外部アセンブリのメソッドをデリゲート
            var result = Delegate_SendImageToVaultMethod(User_zuban, saveFullFileName);
            if (result)
                DebugConsole.WriteLine("■チェックインに成功しました");
            else
                DebugConsole.WriteLine("※チェックインに失敗しました");
        }

        /// <summary>
        /// ■Vaultから図面メモエリアへロード
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OP_LoadVault_button_Click(object sender, EventArgs e)
        {
            // このフォームが表示中の図面番号が存在するか調査
            if (string.IsNullOrWhiteSpace(User_zuban) == true) return;

            // Vaultからファイルを取り出す為にﾌｧｲﾙ名ｻﾌｨｯｸｽを決定
            string suffix;
            if (string.IsNullOrWhiteSpace(VaultCheckInPngSuffix_textBox.Text) == false)
                suffix = VaultCheckInPngSuffix_textBox.Text;
            else
                suffix = "";

            // Vaultから検索するファイル名
            string findFileName = System.IO.Path.ChangeExtension(User_zuban + suffix, "png");

            // Vaultから読み出したファイルの保存先フルファイル名
            string savefullFileName = System.IO.Path.Combine(_draftCacheFullFolderName, findFileName);

            // Vaultから検索するために 外部アセンブリのメソッドをデリゲート
            var result = Delegate_LoadImageFromVaultMethod(findFileName, savefullFileName);

            if (result)
            {
                DebugConsole.WriteLine("■ダウンロードに成功しました");

                if (System.IO.File.Exists(savefullFileName))
                {
                    // イメージファイルの読み込みとセット
                    sourceBitmap = (Bitmap)ImageUtil.FromFile(savefullFileName);

                    // 初期化の為リサイズイベントを強制的に実行
                    //ArcSuitePreviewForm_Resize(null, null);
                    pictureBoxRedraw();

                    DrawImage();
                }
            }
            else
            {
                DebugConsole.WriteLine("※ダウンロードに失敗しました");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sourceBitmap"></param>
        /// <returns></returns>
        private Bitmap LoadIndexedImage(Bitmap sourceBitmap)
        {
            orignalResolution = System.Math.Max(sourceBitmap.HorizontalResolution, sourceBitmap.VerticalResolution);
            // インデックス付き対策のため
            sourceBitmap = (Bitmap)sourceBitmap.GetThumbnailImage(sourceBitmap.Width, sourceBitmap.Height, new Image.GetThumbnailImageAbort(_dummy), IntPtr.Zero);
            {
                var Width = sourceBitmap.Width;
                var Height = sourceBitmap.Height;
                var VerticalResolution = sourceBitmap.VerticalResolution;
                var HorizontalResolution = sourceBitmap.HorizontalResolution;
                WriteLine($"■ArcSuitePreviewForm.ArcSuitePreviewSet(..) Width,Height = ({Width} , {Height}) VerticalResolution={VerticalResolution} , HorizontalResolution={HorizontalResolution}");
            }

            bool _dummy()
            {
                return false; // このメソッドの内容は何でもよい
            }

            return sourceBitmap;
        }

        /// <summary>
        /// ●ピクチャーボックスにフィットさせる
        /// </summary>
        private void ViewFit()
        {
            this.X_numericUpDown.ValueChanged -= new System.EventHandler(this.numericUpDown_ValueChanged);
            this.Y_numericUpDown.ValueChanged -= new System.EventHandler(this.numericUpDown_ValueChanged);
            this.SCALE_numericUpDown.ValueChanged -= new System.EventHandler(this.SCALE_numericUpDown_ValueChanged);

            if (ArcSuitePreviewPictureBox.Image == null) return;

            int PictureBoxSizeWidth = ArcSuitePreviewPictureBox.Size.Width;
            int PictureBoxSizeHeight = ArcSuitePreviewPictureBox.Size.Height;
            //Console.WriteLine($"{PictureBoxSizeWidth},{PictureBoxSizeHeight}");


            int ImageSizeWidth = sourceBitmap.Width;
            int ImageSizeHeight = sourceBitmap.Height;
            //Console.WriteLine($"{ImageSizeWidth},{ImageSizeHeight}");


            float scaleWidth = ((float)ImageSizeWidth / sourceBitmap.HorizontalResolution) / (PictureBoxSizeWidth / ArcSuitePreviewPictureBox.Image.HorizontalResolution);
            float scaleHeigth = ((float)ImageSizeHeight / sourceBitmap.VerticalResolution) / (PictureBoxSizeHeight / ArcSuitePreviewPictureBox.Image.VerticalResolution);

            float _scale = (float)(1 / System.Math.Max(scaleWidth, scaleHeigth));

            // アフィン変換行列をリセット 
            _sourceMatAffine.Reset();

            _sourceMatAffine.Scale(_scale, _scale);

            // 画像の描画  
            DrawImage();

            this.X_numericUpDown.ValueChanged += new System.EventHandler(this.numericUpDown_ValueChanged);
            this.Y_numericUpDown.ValueChanged += new System.EventHandler(this.numericUpDown_ValueChanged);
            this.SCALE_numericUpDown.ValueChanged += new System.EventHandler(this.SCALE_numericUpDown_ValueChanged);
        }

        /// <summary>
        /// ■右下部を拡大表示
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TitleBlockScale_button_Click(object sender, EventArgs e)
        {
            if (sourceBitmap == null)
                return;

            if ((bool)button1.Tag == false)
            {

                button1.Tag = true;
                button1.Text = "元に戻す";

                //右下のワールド座標を求める
                int PictureBoxSizeWidth = ArcSuitePreviewPictureBox.Size.Width;
                int PictureBoxSizeHeight = ArcSuitePreviewPictureBox.Size.Height;

                int ImageSizeWidth = sourceBitmap.Width;
                int ImageSizeHeight = sourceBitmap.Height;
                float ImageResolution = orignalResolution;

                var scale = PictureBoxSizeWidth / (210.0f * ImageResolution / 25.4f);

                // 右下を原点位置へ移動・拡縮・戻す
                _sourceMatAffine.Reset();
                _sourceMatAffine.Translate(-ImageSizeWidth, -ImageSizeHeight);
                _sourceMatAffine.Scale(scale, scale, MatrixOrder.Append);
                _sourceMatAffine.Translate(PictureBoxSizeWidth, PictureBoxSizeHeight, MatrixOrder.Append);

                DrawImage();
            }
            else
            {
                button1.Tag = false;
                button1.Text = "表題欄\r\n拡大";

                // ﾌｨｯﾄへ
                ViewFit();
            }
        }

        /// <summary>
        /// ■ｳｨﾝﾄﾞｳ最大化
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MaximizeWindow_button_Click(object sender, EventArgs e)
        {
            Delegate_MaximizeWindowMethod();
        }

        /// <summary>
        /// ●ｱｸﾃｨﾌﾞﾄﾞｷｭﾒﾝﾄを検索ボタン押下
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ActiveDocmentSearch_Button_Click(object sender, EventArgs e)
        {
            WriteLine($"●ArcSuitePreviewForm.ActiveDocmentSearch_Button が押されました");

            PreviewMsgClear();

            try
            {
                Delegate_ActiveDocmentSearchMethod();
            }
            catch (Exception ex)
            {
                this.WriteLine($"ActiveDocmentSearch_Button_Click(..)にて例外 {ex.Message}");
            }

        }

        /// <summary>
        /// ●手動検索ボタン押下
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ManualSearch_button_Click(object sender, EventArgs e)
        {
            WriteLine($"●ArcSuitePreviewForm.ManualSearch_button が押されました 検索対象 \"{ManualSearch_textBox.Text}\"");
            PreviewMsgClear();

            string ConvertedNumber;
            NumberTypeConfig.DrawingType drawingType;
            string DRAWINGTYPEMSG;

            try
            {
                ManualSearch_textBox.Text = System.IO.Path.GetFileNameWithoutExtension(ManualSearch_textBox.Text).TrimStart(null).TrimEnd(null).Replace("*", "").Replace("?", "").ToUpper();

                if (string.IsNullOrWhiteSpace(ManualSearch_textBox.Text) == false)
                {
                    string sanitizedPartnumber = ArcSuiteSupport.GetArcSuiteSpecealCovertedPARTNUMBER(ManualSearch_textBox.Text, out ConvertedNumber, out drawingType, out DRAWINGTYPEMSG).ToUpper(); ;


                    Delegate_ManualSearchMethod(sanitizedPartnumber, "", "");
                }
            }
            catch (Exception ex)
            {
                DebugConsole.WriteLine($"ManualSearch_button_Click(..)にて例外 {ex.Message}");
            }
        }

        /// <summary>
        /// ●Vaultサーチ開始ボタン押下自
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Vault_ContentCenter_ComponentSearch_button_Click(object sender, EventArgs e)
        {
            if (Delegate_SearchVaultButtonClickMethod != null)
                Delegate_SearchVaultButtonClickMethod(ManualSearch_textBox.Text);
            WriteLine($"●ArcSuitePreviewForm.Vault_ContentCenter_Search_button が押されました 検索対象 \"{ManualSearch_textBox.Text}\"");
        }

        /// <summary>
        /// ●
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Vault_DrawingSearch_button_Click(object sender, EventArgs e)
        {
            if (Delegate_FindDrawingfromVaultButtonClickMethod != null)
                Delegate_FindDrawingfromVaultButtonClickMethod(ManualSearch_textBox.Text);
            WriteLine($"●ArcSuitePreviewForm.FindDWGfromVault_button が押されました 検索対象 \"{ManualSearch_textBox.Text}\"");
        }

        /// <summary>
        /// ●VaultからDWGファイルを検索して開く
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Vault_AutoCAD_DWGSearch_button_Click(object sender, EventArgs e)
        {
            if (Delegate_FindAutoCADDWGfromVaultButtonClickMethod != null)
                Delegate_FindAutoCADDWGfromVaultButtonClickMethod(ManualSearch_textBox.Text);
            WriteLine($"●ArcSuitePreviewForm.FindDWGfromVault_button が押されました 検索対象 \"{ManualSearch_textBox.Text}\"");
        }

        /// <summary>
        /// ●ｸﾘｯﾌﾟﾎﾞｰﾄﾞから検索押下
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ClipBoardTextSearch_button_Click(object sender, EventArgs e)
        {
            IDataObject data = Clipboard.GetDataObject();
            if (data.GetDataPresent(DataFormats.Text))
            {
                string str = (string)data.GetData(DataFormats.Text);

                ManualSearch_textBox.Text = str;
                ManualSearch_button_Click(sender, e);
            }
            WriteLine($"●ArcSuitePreviewForm.ClipBoardTextSearch_button が押されました 検索対象 \"{ManualSearch_textBox.Text}\"");
        }

        /// <summary>
        /// ●図番入力のテキストボックスがフォーカスを持っており、キーボードが最初に押されたとき
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ManualSearch_textBox_KeyDown(object sender, KeyEventArgs e)
        {

            if (e.KeyCode == Keys.Enter)
            {
                bool ShiftPressed = e.Modifiers == Keys.Shift;

                e.Handled = true;

                ManualSearch_button_Click(sender, e);
            }
        }

        private void ManualSearch_textBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                e.Handled = true;

                ManualSearch_button_Click(sender, e);
            }

        }

        /// <summary>
        /// ■
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ManualSearch_textBox_Enter(object sender, EventArgs e)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ManualSearch_textBox_TextChanged(object sender, EventArgs e)
        {
            ArcSuiteDrawinFindURL = null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ManualSearch_textBox_Click(object sender, EventArgs e)
        {
            ManualSearch_textBox.SelectAll();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="user_zuban"></param>
        public void ManualSearch_textBox_Set(string user_zuban)
        {
            InvokeRequired_Control_Text(ManualSearch_textBox, user_zuban);
        }

        /// <summary>
        /// ■指定されたCADの計測機能を実行させる押下
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MeasureTool_Button_Click(object sender, EventArgs e)
        {
            Delegate_MeasureToolMethod();
        }

        /// <summary>
        /// ■断面表示開始ボタン押下
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SectionViewStart_button_Click(object sender, EventArgs e)
        {
            Delegate_SectionViewStartMethod();
        }

        /// <summary>
        /// ■断面表示終了ボタン押下
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SectionViewEnd_button_Click(object sender, EventArgs e)
        {
            Delegate_SectionViewEndMethod();
        }


        /// <summary>
        /// ■現在の表示をｸﾘｯﾌﾟﾎﾞｰﾄﾞへ送信 ボタン押下
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SendClipBord_Button_Click(object sender, EventArgs e)
        {
            if (sourceBitmap != null)
                Clipboard.SetDataObject(sourceBitmap);
        }

        /// <summary>
        /// パーツイラストを取得しピクチャボックスへ
        /// </summary>
        /// <param name="PARTNUMBER"></param>
        /// <param name="WriteLineMethod"></param>
        private void PartListIllust_Process(string PARTNUMBER, Action<string> WriteLineMethod = null)
        {
            try
            {
                if (WriteLineMethod == null) WriteLineMethod = this.WriteLine;
                this.WriteLine = WriteLineMethod;

                if (Delegate_GetPartListIllust == null) return;

                PartListIllust_pictureBox.Image = Delegate_GetPartListIllust(PARTNUMBER);
            }
            catch (Exception ex)
            {
                this.WriteLine($"※PartListIllust_Process(..)にて例外発生 {ex.Message}");
            }
        }


        /// <summary>
        /// ■パーツイラストピクチャボックスをダブルクリックしたとき
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PartListIllust_pictureBox_DoubleClick(object sender, EventArgs e)
        {
            if (CadDataFullFileName == null)
                return;

            if (Delegate_PartListIllustPictureBoxClickMethod != null)
                Delegate_PartListIllustPictureBoxClickMethod(CadDataFullFileName);

            if (Delegate_GetPartListIllust == null) return;

            Delegate_ActiveDocmentSearchMethod();

            if (string.IsNullOrWhiteSpace(stArcSuitePreview.user_zuban) == true) return;

            PartListIllust_pictureBox.Image = Delegate_GetPartListIllust(stArcSuitePreview.user_zuban);
        }

        /// <summary>
        /// ■デバッグ用
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void numericUpDown_ValueChanged(object sender, EventArgs e)
        {
            if (sourceBitmap == null)
                return;


            test_cur_x = (float)X_numericUpDown.Value;
            test_cur_y = (float)Y_numericUpDown.Value;

            test_scale();
        }

        /// <summary>
        /// ■デバッグ用
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SCALE_numericUpDown_ValueChanged(object sender, EventArgs e)
        {
            if (sourceBitmap == null)
                return;
            test_cur_scale = (float)SCALE_numericUpDown.Value;

            test_scale();
        }

        /// <summary>
        /// ■デバッグ用
        /// </summary>
        private void test_scale()
        {

            //右下のワールド座標を求める
            var x = sourceBitmap.Width;
            var y = sourceBitmap.Height;

            // 右下に移動し拡縮
            _sourceMatAffine.Reset();
            _sourceMatAffine.Translate(-x, -y, MatrixOrder.Append);
            test_cur_x = -x;
            test_cur_y = -y;

            if (test_cur_scale != 0)
                _sourceMatAffine.Scale(test_cur_scale, test_cur_scale, MatrixOrder.Append);

            // 戻す
            _sourceMatAffine.Translate(sourceBitmap.Width - test_cur_x, sourceBitmap.Height - test_cur_y, MatrixOrder.Append);
            test_cur_x = sourceBitmap.Width - test_cur_x;
            test_cur_y = sourceBitmap.Height - test_cur_y;

            DrawImage();
        }

        /// <summary>
        /// ■デバッグ用
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {
            if (sourceBitmap == null)
                return;

            //右下のワールド座標を求める
            var x = sourceBitmap.Width * -1f;
            var y = sourceBitmap.Height * -1f;

            // 右下に移動し拡縮
            _sourceMatAffine.Reset();
            _sourceMatAffine.Translate(x, y);
            test_cur_x = x;
            test_cur_y = y;

            if (test_cur_scale != 0)
                _sourceMatAffine.Scale(test_cur_scale, test_cur_scale, MatrixOrder.Append);

            DrawImage();
        }

        /// <summary>
        /// ■デバッグ用
        /// </summary>
        private void showTestlabel()
        {
            DebugConsole.WriteLine($"test_cur_x:{test_cur_x} test_cur_y:{test_cur_y} test_cur_scale:{test_cur_scale}");
        }

        private void currentStageServer_label_Paint(object sender, PaintEventArgs e)
        {
            currentStageServer_label.Text = ConnectionDataSet.StageServerHost;
        }

        private void sasaLibBasicPageControl_CurrentPageChanged(object sender, EventArgs e)
        {
            drawingPreview.CurrentPage = sasaLibBasicPageControl.CurrentPage;
            //// イメージファイルの読み込みとセット
            sourceBitmap = LoadIndexedImage((Bitmap)drawingPreview.GetCurrentImage());

            // 初期化の為リサイズイベントを強制的に実行
            //ArcSuitePreviewForm_Resize(null, null);
            pictureBoxRedraw();

            // ウィンドに合わせる
            ViewFit();

        }

        /// <summary>
        /// 
        /// </summary>
        private void pictureBoxRedraw()
        {
            /*
            フォームのウィンドウサイズが変更されると、フォーム上のPictureBoxのサイズも変更される。
            PictreuBoxのサイズ一杯に画像表示領域を確保する場合、サイズ変更のイベントと合わせて、
            表示領域用ビットマップオブジェクトの再作成が必要になる。
            */
            try
            {
                // Graphicsオブジェクトが取得済みの場合
                if (pictureBoxImageGraphics != null)
                {
                    // Graphicsオブジェクトからアフィン変換行列を取得し退避
                    _sourceMatAffine = pictureBoxImageGraphics.Transform;

                    // Graphicsオブジェクトの解放  
                    pictureBoxImageGraphics.Dispose(); pictureBoxImageGraphics = null;
                }
                else
                {
                    _sourceMatAffine = new System.Drawing.Drawing2D.Matrix();
                }

                // Imageを破棄
                if (ArcSuitePreviewPictureBox.Image != null) ArcSuitePreviewPictureBox.Image.Dispose();

                // PictureBoxに描画領域用の空のビットマップを割り当てる。
                ArcSuitePreviewPictureBox.Image = new Bitmap(ArcSuitePreviewPictureBox.Width, ArcSuitePreviewPictureBox.Height);

                // PictureBoxから新たなGraphicsオブジェクトを取得し退避  
                pictureBoxImageGraphics = Graphics.FromImage(ArcSuitePreviewPictureBox.Image);

                // 補間モードの設定（NearestNeighbor）  
                pictureBoxImageGraphics.InterpolationMode = InterpolationMode.NearestNeighbor;

                // 画像の描画  
                DrawImage();

            }
            catch (Exception ex)
            {
                Console.WriteLine($"※pictureBoxRedraw(..)にて例外 {ex.Message}");
            }

        }

        /// <summary>
        /// ●●ｱｰｸｽｲｰﾄ登録図面および一部属性値をﾌﾟﾚﾋﾞｭｰイメージにセット
        /// </summary>
        /// <param name="st_arcSuitePreview">ArcSuite検索結果構造体</param>
        /// <param name="ArcSuiteDrawingSearchURL">ArcSuiteにWeb検索するときのURL</param>
        /// <param name="CadDataFullFileName">検索元になったCADネイティブファイルフルパス</param>
        public void PreviewSet(ArcsuitePreview st_arcSuitePreview, string ArcSuiteDrawingSearchURL = null, string CadDataFullFileName = null)
        {
            WriteLine($"■ArcSuitePreviewForm.PreviewSet() 実行開始 \"{st_arcSuitePreview.temporalyDrawingImageFullFileName}\"");

            InvokeRequired_Control_Enabled(ArcSuiteAttr_groupBox, true);

            InvokeRequired_Control_Text(currentStageServer_label, ConnectionDataSet.StageServerHost); // 現在のステージングサーバーホスト名をラベルにセット

            // 検索対象のCAD側ドキュメントフルファイル名を取得
            this.CadDataFullFileName = CadDataFullFileName;

            if (string.IsNullOrWhiteSpace(st_arcSuitePreview.user_zuban) == false)
            {
                this.User_zuban = st_arcSuitePreview.user_zuban;
                InvokeRequired_Control_Text(ManualSearch_textBox, this.User_zuban);
            } // 構造体メンバ user_zuban が IsNullOrWhiteSpace でなければ ManualSeac_textBox に値をセット.または・・・
            else if (string.IsNullOrWhiteSpace(st_arcSuitePreview.temporalyDrawingImageFullFileName) == false)
            {
                this.User_zuban = System.IO.Path.GetFileNameWithoutExtension(st_arcSuitePreview.temporalyDrawingImageFullFileName);
                InvokeRequired_Control_Text(ManualSearch_textBox, this.User_zuban);
            } // 構造体メンバ temporalyDrawingImageFullFileName が IsNullOrWhiteSpace でなければManualSeac_textBox に値をセット.または・・・
            else
            {
                WriteLine($"※ArcSuitePreviewForm.PreviewSet() エラー 検索すべき 図面番号 が特定できない。終了します");
                return;
            }

            string saveFullFileName = System.IO.Path.Combine(_draftCacheFullFolderName, User_zuban + ".png");

            if (System.IO.File.Exists(saveFullFileName))
                InvokeRequired_Control_Enabled(ImageReLoad_button, true);
            else
                InvokeRequired_Control_Enabled(ImageReLoad_button, false);

            try
            {
                if (ArcSuiteDrawingSearchURL == null)
                {
                    InvokeRequired_Control_Enabled(ArcSuiteWebSearchAndView_button, false);
                    InvokeRequired_Control_Text(ArcSuiteWebSearchAndView_button, "非対応");
                } // ArcSuiteDrawingSearchURL が null なら非対応とする
                else
                {
                    InvokeRequired_Control_Enabled(ArcSuiteWebSearchAndView_button, true);
                    InvokeRequired_Control_Text(ArcSuiteWebSearchAndView_button, "Web版検索");
                }

                PreviewMsgClear();

                InvokeRequired_Control_Text(FindTimeStamp_label, "");

                SetArcSuiteDrawinFindURL(ArcSuiteDrawingSearchURL);

                if (st_arcSuitePreview.system_contentType.ToUpper() == "image/tiff".ToUpper() ||
                    st_arcSuitePreview.system_contentType.ToUpper() == "image/png".ToUpper() ||
                    st_arcSuitePreview.system_contentType.ToUpper() == "image/bmp".ToUpper() ||
                    st_arcSuitePreview.system_contentType.ToUpper() == "image/jpeg".ToUpper())
                {

                    if (string.IsNullOrWhiteSpace(st_arcSuitePreview.temporalyDrawingImageFullFileName) == false)
                    {
                        // stArcSuitePreview.user_description か stArcSuitePreview.user_partname に文字列があればそれを採用する
                        string ArcSuiteDrawingNameOrDescription = ArcsuitePreview.SelectArcSuitePreviewDESCRITIONorPARTNAME(st_arcSuitePreview);

                        // アークスイート属性 "状態" の属性値を取得
                        string SystemStatusDisplayName = ArcSuiteSupport.GetArcSuiteSystemStatusDisplayName(st_arcSuitePreview.sysmte_status);

                        //  アークスイート属性 "登録日時" の属性値を取得
                        DateTime dateTime_system_createdOn = ArcSuiteSupport.GetSyssmteCreatedOnTime(st_arcSuitePreview.system_createdon);

                        if (st_arcSuitePreview.sysmte_status != "system:editable")
                            AcsSuiteStatusLabelMessageSet($"状態:{SystemStatusDisplayName}", System.Drawing.Color.Red);
                        else
                            AcsSuiteStatusLabelMessageSet($"");

                        // 3Dﾓﾃﾞﾙ作成発注状態を示す文字列
                        string UserMoodelcreationonorderDiplayName;
                        if (string.IsNullOrWhiteSpace(st_arcSuitePreview.user_modelcreationonorder) == true)
                        {
                            UserMoodelcreationonorderDiplayName = $"3D作成発注中: 値はありません";
                            InvokeRequired_Control_Enabled(OnOrderClear_button, false);
                        }
                        else
                        {
                            UserMoodelcreationonorderDiplayName = $"3D作成発注中:『{st_arcSuitePreview.user_modelcreationonorder}』";
                            InvokeRequired_Control_Enabled(OnOrderClear_button, true);
                        }

                        // テキストラベルｺﾝﾎﾟｰﾈﾝﾄに反映
                        InvokeRequired_Control_Text(modelcreationonorder_label, UserMoodelcreationonorderDiplayName);

                        // 登録日時をテキストラベルに反映
                        string dateTime_system_createdOn_String = dateTime_system_createdOn.ToString("yyyy/MM/dd , HH:mm");

                        // ファイルのタイムスタンプを取得
                        if (System.IO.File.Exists(CadDataFullFileName) == true)
                        {
                            //DateTime CADfileCreatedTime = default;
                            //DateTime CadfileLastTime = default;

                            DateTime CADfileCreatedTime = System.IO.File.GetCreationTime(CadDataFullFileName);
                            string CADfileCreatedTime_String = CADfileCreatedTime.ToString("yyyy/MM/dd , HH:mm");
                            //CadfileLastTime = System.IO.File.GetLastWriteTime(CadDataFullFileName);

                            string ArcSuiteCreatedOn_label_ToolTip = $"CADﾌｧｲﾙ【{CadDataFullFileName}】更新時刻:{CADfileCreatedTime_String}";


                            string msg;
                            bool isCadDataOld = st_arcSuitePreview.IsCadDataOlderThanArcSuite(System.IO.File.GetCreationTime(CadDataFullFileName), System.IO.File.GetLastWriteTime(CadDataFullFileName), SetUnsetCadType, st_arcSuitePreview.user_cadtype, dateTime_system_createdOn, out msg);

                            // 現在開いているCADタイプが、アークスイートから検索した図面のCADタイプに含まれておらず、日付の比較の結果ｱｰｸｽｲｰﾄが新しければ警告します
                            if (isCadDataOld) // true なら CADファイル側が古くなっているので警告表示
                            {
                                ArcSuiteCreatedOn_label_MessageSet($"ArcSuite登録日時:{dateTime_system_createdOn_String} ※図面変更されています。確認必須！！", Color.Red, ArcSuiteCreatedOn_label_ToolTip);
                            }
                            else
                            {

                                ArcSuiteCreatedOn_label_MessageSet($"ArcSuite登録日時:{dateTime_system_createdOn}", Color.Black, ArcSuiteCreatedOn_label_ToolTip);
                            }

                        }
                        else
                        {
                            string ArcSuiteCreatedOn_label_ToolTip = $"CADﾌｧｲﾙは見つかりません";
                            ArcSuiteCreatedOn_label_MessageSet($"登録日時:{dateTime_system_createdOn}", Color.Black, ArcSuiteCreatedOn_label_ToolTip);
                        }

                        InvokeRequired_Control_Text(DrawingInfoLabel2, $"図面番号:{st_arcSuitePreview.user_zuban} , 改版:{st_arcSuitePreview.system_editionNumber} ", Color.Black);

                        InvokeRequired_Control_Text(DrawingInfoLabel3, $"部品名 / 説明:{ArcSuiteDrawingNameOrDescription} , 状態:{SystemStatusDisplayName}", Color.Black);

                        InvokeRequired_Control_Text(DrawingInfoLabel4, $"CADﾀｲﾌﾟ:{st_arcSuitePreview.user_cadtype_string}", Color.Black);

                        /// リストイラスト機能が有効のとき
                        try
                        {
                            if (OP_PartListIllust_groupbox.Enabled == true)
                                PartListIllust_Process(st_arcSuitePreview.user_zuban, WriteLine);
                        }
                        catch (Exception ex)
                        {
                            WriteLine($"※例外検知 ArcSuitePreviewForm.PreviewSet(..)部品リストイラスト読み出しにて例外検知 {ex.Message}");
                        }

                        string SearchTimeString = DateTime.Now.ToString("yyyy/MM/dd , HH:mm:ss");
                        InvokeRequired_Control_Text(FindTimeStamp_label, $"検索実行日時：{SearchTimeString}", Color.Black);
                    }

                    if (System.IO.File.Exists(st_arcSuitePreview.temporalyDrawingImageFullFileName) == true)
                    {

                        // ArcSuiteイメージﾌﾟﾚﾋﾞｭｰﾌｧｲﾙをフォルダごと削除（前回のがあれば？）
                        PreviewImageCacheClear();

                        // ArcSuiteイメージ表示ファイル名
                        this.recent_temporalyDrawingImageFullFileName = st_arcSuitePreview.temporalyDrawingImageFullFileName;

                        drawingPreview = new MultiPage(st_arcSuitePreview.temporalyDrawingImageFullFileName);

                        st_arcSuitePreview.numberOfPages = drawingPreview.Images.Length - 1;

                        MethodInvoker method = () =>
                        {
                            // コントロールに対する処理
                            if (drawingPreview.Images.Length == 1)
                            {
                                sasaLibBasicPageControl.Visible = false;
                            }
                            else if (drawingPreview.Images.Length > 1)
                            {
                                sasaLibBasicPageControl.MaxPage = st_arcSuitePreview.numberOfPages;
                                sasaLibBasicPageControl.Visible = true;
                            }
                            else
                            {
                                throw new Exception("ｲﾒｰｼﾞが空です");
                            }
                        };
                        if (InvokeRequired) { Invoke(method); } else { method(); }


                        //// イメージファイルの読み込みとセット
                        sourceBitmap = LoadIndexedImage((Bitmap)drawingPreview.Images[0]);

                        imageUndoBuffer.Add(new Bitmap(sourceBitmap));
                        imageUndoBufferIndex = imageUndoBuffer.Count - 1;

                        InvokeRequired_Control_Text(MidLabel2_label, $"{imageUndoBuffer.Count} / {imageUndoBuffer.MaxCapacity} index = {imageUndoBufferIndex}");

                        // 初期化の為リサイズイベントを強制的に実行
                        //ArcSuitePreviewForm_Resize(null, null);
                        pictureBoxRedraw();

                        // ウィンドに合わせる
                        ViewFit();
                    }
                }
                else
                {
                    // ArcSuiteイメージ表示ファイル名
                    this.recent_temporalyDrawingImageFullFileName = st_arcSuitePreview.temporalyDrawingImageFullFileName;

                    InvokeRequired_Control_Enabled(ArcSuiteAttr_groupBox, false);

                    MessageSet("表示できないファイルです。アプリケーションから開く ボタンを押してください");
                    Enable_AplicationOpenFile_button(true);
                }

            }
            catch (Exception ex)
            {
                WriteLine($"※※例外検知 ArcSuitePreviewForm.PreviewSet()のどこかで。{ex.Message}");
            }
            WriteLine($"■ArcSuitePreviewForm.PreviewSet() 正常終了 \"{st_arcSuitePreview.temporalyDrawingImageFullFileName}\"");

        }

        /// <summary>
        /// ■ｱﾌﾟﾘｹｰｼｮﾝから開くボタン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AplicationOpenFile_button_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(recent_temporalyDrawingImageFullFileName))
                return;

            if (System.IO.File.Exists(recent_temporalyDrawingImageFullFileName))
            {


#if NETCOREAPP
                System.Diagnostics.Process.Start(new ProcessStartInfo
                {
                    FileName = recent_temporalyDrawingImageFullFileName,
                    UseShellExecute = true // システムのデフォルトアプリケーションを使用
                });
#else
                            System.Diagnostics.Process.Start(recent_temporalyDrawingImageFullFileName);
#endif

            }

        }


        /// <summary>
        /// ■再描画ボタン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Redraw_button_Click(object sender, EventArgs e)
        {
            pictureBoxRedraw();
        }

    }
}
