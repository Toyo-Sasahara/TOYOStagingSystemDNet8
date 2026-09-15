using System;
using System.Drawing;
using System.Windows.Forms;
using System.Diagnostics;
using System.Drawing.Drawing2D;
using SasaLib.NumberingSupport;
using StageServerRemote;
using System.Runtime.Versioning;

namespace SasaLib.ArcSuitePreview
{

    [SupportedOSPlatform("windows")]
    public partial class ArcSuitePreviewOnlyForm : Form
    {
        static bool EnableLeftButtonDrag = false;
        static bool EnableMiddleButtonDrag = true;
        static bool EnableRightButtonDrag = false;

        /// <summary>
        /// デリゲートメソッド 再検索ボタン
        /// </summary>
        Delegate_ExternalFunctionMethod Delegate_ClipBoardTextSearchMethod;

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
        object loadmethod;

        /// <summary>
        /// デバッグ用
        /// </summary>
        public bool DebugMode { get; set; } = false;

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
        private string StageServerHost;

        /// <summary>
        /// 
        /// </summary>
        private string PipeNameDR;

        /// <summary>
        /// 
        /// </summary>
        private string ClientDomainName;

        /// <summary>
        /// 
        /// </summary>
        private string ClientUserName;

        /// <summary>
        /// 
        /// </summary>
        private string ClientUserPassword;

        /// <summary>
        /// 
        /// </summary>
        private bool ClsLogon;

        /// <summary>
        /// アークスイートログイン名
        /// </summary>
        private string ArcSuiteUserName;

        /// <summary>
        /// アークスイートログインパスワード（平文）
        /// </summary>
        private string ArcSuiteUserPass;

        /// <summary>
        /// ArcSuite側へ設定・取得するCadType 列挙型
        /// </summary>
        private RemoteClientCadType.CadType SetUnsetCadType;

        /// <summary>
        /// 図面イメージのソースBitmapオブジェクト
        /// </summary>        
        private Bitmap sourceBitmap = null;

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
        private bool isRightButtonPushMouseMoving = false;

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
        /// 中央ボタンクリック時のコントロール座標値
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
        /// デバッグ用
        /// </summary>
        float test_cur_x;

        /// <summary>
        /// デバッグ用
        /// </summary>
        float test_cur_y;


        /// <summary>
        /// デバッグ出力用
        /// </summary>
        Action<string> WriteLine;

        /// <summary>
        /// ■コンストラクタ
        /// </summary>
        /// <param name="loadmethod"></param>
        /// <param name="CallDestination"></param>

        public ArcSuitePreviewOnlyForm(object loadmethod, Action<string> CallDestination = null)
        {
            this.loadmethod = loadmethod;

            // デバッグメッセージデリゲート先選択
            if (CallDestination == null) this.WriteLine = DebugConsole.Write; else this.WriteLine = CallDestination;

            //
            InitializeComponent();

            // タイトルバーをﾓｰﾄﾞにより変更
            if (DebugMode)
                this.Text = $"■東陽ﾂｰﾙ TESTﾓｰﾄﾞ{this.Text}";

            ArcSuitePreviewPictureBox.Controls.Add(ArcsuitePreviewForm_Msg_label);
            ArcSuitePreviewPictureBox.Controls.Add(ArcSuite_Status_label);

            Debug_panel.Visible = DebugMode;

            // マウスホイールイベント関連
            ArcSuitePreviewPictureBox.MouseWheel += pictureBox1_MouseWheel;
        }

        /// <summary>
        /// ■フォームがロードされたとき
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ArcSuitePreviewForm_Load(object sender, EventArgs e)
        {
            InvokeRequired_Control_Enabled(ClipBoardTextSearch_button, false);

            TitleBlockScale_button.Tag = false;
            ArcSuiteWebSearchAndView_button.Tag = false;

            thisNativeWindow = new System.Windows.Forms.NativeWindow();
        }

        /// <summary>
        /// ■フォームが閉じようとするとき
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ArcSuitePreviewForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;
            }

            PreviewImageCacheClear();
        }

        /// <summary>
        /// ■フォームが表示されたとき
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ArcSuitePreviewForm_Shown(object sender, EventArgs e)
        {
            FormShow = true;

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

            // 右ボタン押下
            if (EnableRightButtonDrag)
            {
                if (e.Button == MouseButtons.Right)
                {
                    // フォーカスの設定（おまじない)
                    ArcSuitePreviewPictureBox.Focus();

                    // マウスをクリックした位置の記録  
                    oldMouseRightButtonClickPoint.X = e.X;
                    oldMouseRightButtonClickPoint.Y = e.Y;

                    // オリジナルビットマップの座標を退避
                    oldSoureBitmapMouseRightButtonClickPoint = GetOrginalImagePoint(e.X, e.Y, _sourceMatAffine);

                    // マウス移動フラグを立てる  
                    isRightButtonPushMouseMoving = true;
                }
            }

            // 中央ボタン押下
            if (EnableMiddleButtonDrag)
            {
                if (e.Button == MouseButtons.Middle)
                {
                    // フォーカスの設定（おまじない)
                    ArcSuitePreviewPictureBox.Focus();

                    // マウスをクリックした位置の記録  
                    oldMouseMiddleClickPoint.X = e.X;
                    oldMouseMiddleClickPoint.Y = e.Y;

                    // オリジナルビットマップの座標を退避
                    oldSoureBitmapMouseMiddleButtonClickPoint = GetOrginalImagePoint(e.X, e.Y, _sourceMatAffine);

                    // マウス移動フラグを立てる  
                    isMiddleButtonPushMouseMoving = true;
                }
            }

            // 左ボタン押下
            if (EnableLeftButtonDrag)
            {
                if (e.Button == MouseButtons.Left)
                {
                    // マウスをクリックした位置の記録  
                    oldMouseLeftButtonClickPoint.X = e.X;
                    oldMouseLeftButtonClickPoint.Y = e.Y;

                    // オリジナルビットマップの座標を退避
                    oldSoureBitmapMouseLeftButtonClickPoint = GetOrginalImagePoint(e.X, e.Y, _sourceMatAffine);

                    // マウス移動フラグを立てる  
                    isLeftButtonPushMouseMoving = true;
                }
            }
        }

        /// <summary>
        /// ■マウス移動イベント
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

            // 左ボタン押下されながら移動中の場合
            if (isLeftButtonPushMouseMoving == true)
            {
                int curXX = GetOrginalImagePoint(e.X, e.Y, _sourceMatAffine).X;
                int curYY = GetOrginalImagePoint(e.X, e.Y, _sourceMatAffine).Y;

                Graphics g = Graphics.FromImage(sourceBitmap);
                float penWidth = 10.0f;
                Pen pen = new Pen(Color.Red, penWidth);

                g.DrawLine(pen, curXX, curYY, oldSoureBitmapMouseLeftButtonClickPoint.X, oldSoureBitmapMouseLeftButtonClickPoint.Y);

                g.Dispose();
                pen.Dispose();


                if (DebugMode)
                {
                    lblDst.Text = $"マウスポインタ座標:({e.X},{e.Y}) 実イメージ上の座標値:({oldSoureBitmapMouseLeftButtonClickPoint.X:#.#}, {oldSoureBitmapMouseLeftButtonClickPoint.Y:#.#}) ";
                }

                // 現在のソースびっとまぷ上のマウスの位置を退避する
                oldSoureBitmapMouseLeftButtonClickPoint = GetOrginalImagePoint(e.X, e.Y, _sourceMatAffine);

            }

            // 右ボタン押下されながら移動中の場合
            if (isRightButtonPushMouseMoving == true)
            {
                int curXX = GetOrginalImagePoint(e.X, e.Y, _sourceMatAffine).X;
                int curYY = GetOrginalImagePoint(e.X, e.Y, _sourceMatAffine).Y;

                Graphics g = Graphics.FromImage(sourceBitmap);
                float penWidth = 10.0f;
                Pen pen = new Pen(Color.Red, penWidth);

                g.DrawLine(pen, curXX, curYY, oldSoureBitmapMouseRightButtonClickPoint.X, oldSoureBitmapMouseRightButtonClickPoint.Y);

                g.Dispose();
                pen.Dispose();


                if (DebugMode)
                {
                    lblDst.Text = $"マウスポインタ座標:({e.X},{e.Y}) 実イメージ上の座標値:({oldSoureBitmapMouseRightButtonClickPoint.X:#.#}, {oldSoureBitmapMouseRightButtonClickPoint.Y:#.#}) ";
                }

                // 現在のソースびっとまぷ上のマウスの位置を退避する
                oldSoureBitmapMouseRightButtonClickPoint = GetOrginalImagePoint(e.X, e.Y, _sourceMatAffine);

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
            // ピクチャボックスを背景色でクリア
            if (pictureBoxImageGraphics != null)
                pictureBoxImageGraphics.Clear(ArcSuitePreviewPictureBox.BackColor);

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
                Console.WriteLine($"※ArcSuitePreviewForm_Resize(..)にて例外 {ex.Message}");
            }

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
        /// ●システムメッセージを表示
        /// </summary>
        /// <param name="msg"></param>
        public void MessageSet(string msg)
        {
            //if (InvokeRequired)
            //{
            //    Invoke(new Action(() =>
            //    {
            //        /// UIを操作する処理
            //        ArcsuitePreviewForm_Msg_label.Text = msg;
            //    }));
            //}
            //else
            //{
            //    ArcsuitePreviewForm_Msg_label.Text = msg;
            //}

            MethodInvoker method = () =>
            {
                // コントロールに対する処理
                ArcsuitePreviewForm_Msg_label.Text = msg;
            };
            if (InvokeRequired) { Invoke(method); } else { method(); }
        }

        /// <summary>
        /// ●AcsSuiteStatusLabeラベル(ｱｰｸｽｲｰﾄ属性：状態)にメッセージセット
        /// </summary>
        /// <param name="msg"></param>
        public void AcsSuiteStatusLabelMessageSet(string msg)
        {
            //if (InvokeRequired)
            //{
            //    Invoke(new Action(() =>
            //    {
            //        /// UIを操作する処理
            //        ArcSuite_Status_label.Text = msg;
            //    }));
            //}
            //else
            //{
            //    ArcSuite_Status_label.Text = msg;
            //}

            MethodInvoker method = () =>
            {
                // コントロールに対する処理
                ArcSuite_Status_label.Text = msg;
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
            //if (InvokeRequired)
            //{
            //    Invoke(new Action(() =>
            //    {
            //        /// UIを操作する処理
            //        ArcSuiteCreatedOn_label.ForeColor = foregroundColor;
            //        ArcSuiteCreatedOn_label.Text = ArcSuiteCreatedOn_label_Text;
            //        toolTip1.SetToolTip(this.ArcSuiteCreatedOn_label, $"{ArcSuiteCreatedOn_label_ToolTip}");
            //    }));
            //}
            //else
            //{
            //    ArcSuiteCreatedOn_label.ForeColor = foregroundColor;
            //    ArcSuiteCreatedOn_label.Text = ArcSuiteCreatedOn_label_Text;
            //    toolTip1.SetToolTip(this.ArcSuiteCreatedOn_label, $"{ArcSuiteCreatedOn_label_ToolTip}");
            //}

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
        public void InvokeRequired_Control_Visible(Control control, bool Visible)
        {
            //if (InvokeRequired)
            //{
            //    Invoke(new Action(() =>
            //    {
            //        /// UIを操作する処理
            //        control.Visible = Visible;
            //    }));
            //}
            //else
            //{
            //    control.Visible = Visible;
            //}


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
        public void InvokeRequired_Control_Text(Control control, string text)
        {
            //if (InvokeRequired)
            //{
            //    Invoke(new Action(() =>
            //    {
            //        /// UIを操作する処理
            //        control.Text = text;
            //    }));
            //}
            //else
            //{
            //    control.Text = text;
            //}

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
        public void InvokeRequired_Control_Text(Control control, string text, Color ForColor)
        {
            //if (InvokeRequired)
            //{
            //    Invoke(new Action(() =>
            //    {
            //        /// UIを操作する処理
            //        control.Text = text;
            //        control.ForeColor = ForColor;
            //    }));
            //}
            //else
            //{
            //    control.Text = text;
            //    control.ForeColor = ForColor;
            //}

            MethodInvoker method = () =>
            {
                // コントロールに対する処理
                control.ForeColor = ForColor;
            };
            if (InvokeRequired) { Invoke(method); } else { method(); }

        }

        /// <summary>
        /// ●スレッド対応のコントロールのイネーブルのセット
        /// </summary>
        /// <param name="control"></param>
        /// <param name="Enabled"></param>
        public void InvokeRequired_Control_Enabled(Control control, bool Enabled)
        {
            //if (InvokeRequired)
            //{
            //    Invoke(new Action(() =>
            //    {
            //        /// UIを操作する処理
            //        control.Enabled = Enabled;
            //    }));
            //}
            //else
            //{
            //    control.Enabled = Enabled;
            //}

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
        public void InvokeRequired_Graphics_Transform(Graphics graphics, System.Drawing.Drawing2D.Matrix matrix)
        {
            try
            {

                //if (InvokeRequired)
                //{
                //    Invoke(new Action(() =>
                //    {
                //        /// UIを操作する処理
                //        graphics.Transform = matrix;
                //    }));
                //}
                //else
                //{
                //    graphics.Transform = matrix;
                //}

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
        public void InvokeRequired_Graphics_DrawImage(Graphics graphics, System.Drawing.Image image, int x, int y)
        {
            try
            {
                //if (InvokeRequired)
                //{
                //    Invoke(new Action(() =>
                //    {
                //        /// UIを操作する処理
                //        graphics.DrawImage(image, x, y);
                //    }));
                //}
                //else
                //{
                //    graphics.DrawImage(image, x, y);
                //}

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
        /// ●メッセージクリア.スレッド対応。ピクチャボックスにもnullを指定します
        /// </summary>
        public void PreviewMsgClear()
        {
            ArcSuitePreviewPictureBox.Image = null;

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


            DoEvents.Run();
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
        public void SetUnsetCadTypeFlagControlDatas(string StageServerHost, string PipeNameDR, string ClientDomainName, string ClientUserName, string ClientUserPassword, bool ClsLogon,
            RemoteClientCadType.CadType cadType, string ArcSuiteUserName, string ArcSuiteUserPass)
        {
            this.StageServerHost = StageServerHost;
            this.PipeNameDR = PipeNameDR;

            this.ClientDomainName = ClientDomainName;
            this.ClientUserName = ClientUserName;
            this.ClientUserPassword = ClientUserPassword;
            this.ClsLogon = ClsLogon;

            this.ArcSuiteUserName = ArcSuiteUserName;
            this.ArcSuiteUserPass = ArcSuiteUserPass;
            this.SetUnsetCadType = cadType;
        }

        /// <summary>
        /// ★ｸﾘｯﾌﾟﾎﾞｰﾄﾞで検索ボタンを設定 
        /// </summary>
        /// <param name="flag"></param>
        public void Enable_ClipBoardTextSearchFunction(Delegate_ExternalFunctionMethod method)
        {
            Delegate_ClipBoardTextSearchMethod = method;

            InvokeRequired_Control_Enabled(ClipBoardTextSearch_button, true);
        }

        /// <summary>
        /// ●●ｱｰｸｽｲｰﾄ登録図面および一部属性値をﾌﾟﾚﾋﾞｭｰイメージにセット
        /// </summary>
        /// <param name="TiffTempFullfile"></param>
        /// <param name="stArcSuitePreview"></param>
        public void PreviewSet(ArcsuitePreview stArcSuitePreview, string ArcSuiteDrawingSearchURL = null, string CadDataFullFileName = null)
        {
            WriteLine($"■ArcSuitePreviewOnlyForm.PreviewSet() 実行開始");

            // 検索対象のCAD側ドキュメントフルファイル名を取得
            this.CadDataFullFileName = CadDataFullFileName;

            if (string.IsNullOrWhiteSpace(stArcSuitePreview.user_zuban) == false)
            {
                this.User_zuban = stArcSuitePreview.user_zuban;
            } // 構造体メンバ user_zuban が IsNullOrWhiteSpace でなければ ManualSeac_textBox に値をセット.または・・・
            else if (string.IsNullOrWhiteSpace(stArcSuitePreview.temporalyDrawingImageFullFileName) == false)
            {
                this.User_zuban = System.IO.Path.GetFileNameWithoutExtension(stArcSuitePreview.temporalyDrawingImageFullFileName);
            } // 構造体メンバ temporalyDrawingImageFullFileName が IsNullOrWhiteSpace でなければManualSeac_textBox に値をセット.または・・・
            else
            {
                WriteLine($"※ArcSuitePreviewOnlyForm.PreviewSet() エラー 検索すべき 図面番号 が特定できない。終了します");
                return;
            }

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

                this.ArcSuiteDrawinFindURL = ArcSuiteDrawingSearchURL;

                if (string.IsNullOrWhiteSpace(stArcSuitePreview.temporalyDrawingImageFullFileName) == false)
                {
                    if (System.IO.File.Exists(stArcSuitePreview.temporalyDrawingImageFullFileName) == true)
                    {
                        // ArcSuiteイメージﾌﾟﾚﾋﾞｭｰﾌｧｲﾙをフォルダごと削除
                        PreviewImageCacheClear();

                        // ArcSuiteイメージ表示ファイル名
                        this.recent_temporalyDrawingImageFullFileName = stArcSuitePreview.temporalyDrawingImageFullFileName;

                        // イメージファイルの読み込みとセット
                        sourceBitmap = (Bitmap)ImageUtil.FromFile(stArcSuitePreview.temporalyDrawingImageFullFileName);
                        orignalResolution = System.Math.Max(sourceBitmap.HorizontalResolution, sourceBitmap.VerticalResolution);
                        // インデックス付き対策のため
                        sourceBitmap = (Bitmap)sourceBitmap.GetThumbnailImage(sourceBitmap.Width, sourceBitmap.Height, new Image.GetThumbnailImageAbort(_dummy), IntPtr.Zero);
                        {
                            var Width = sourceBitmap.Width;
                            var Height = sourceBitmap.Height;
                            var VerticalResolution = sourceBitmap.VerticalResolution;
                            var HorizontalResolution = sourceBitmap.HorizontalResolution;
                            WriteLine($"■ArcSuitePreviewOnlyForm.ArcSuitePreviewSet(..) Width,Height = ({Width}{Height}) VerticalResolution={VerticalResolution} , HorizontalResolution={HorizontalResolution}");
                        }

                        // 初期化の為リサイズイベントを強制的に実行
                        ArcSuitePreviewForm_Resize(null, null);

                        // ウィンドに合わせる
                        ViewFit();

                        // stArcSuitePreview.user_description か stArcSuitePreview.user_partname に文字列があればそれを採用する
                        string ArcSuiteDrawingNameOrDescription = ArcsuitePreview.SelectArcSuitePreviewDESCRITIONorPARTNAME(stArcSuitePreview);

                        // アークスイート属性 "状態" の属性値を取得
                        string SystemStatusDisplayName = ArcSuiteSupport.GetArcSuiteSystemStatusDisplayName(stArcSuitePreview.sysmte_status);

                        //  アークスイート属性 "登録日時" の属性値を取得
                        DateTime dateTime_system_createdOn = ArcSuiteSupport.GetSyssmteCreatedOnTime(stArcSuitePreview.system_createdon);

                        if (stArcSuitePreview.sysmte_status != "system:editable")
                            AcsSuiteStatusLabelMessageSet($"状態:{SystemStatusDisplayName}");
                        else
                            AcsSuiteStatusLabelMessageSet($"");

                        // 3Dﾓﾃﾞﾙ作成発注状態を示す文字列
                        string UserMoodelcreationonorderDiplayName;
                        if (string.IsNullOrWhiteSpace(stArcSuitePreview.user_modelcreationonorder) == true)
                        {
                            UserMoodelcreationonorderDiplayName = $"3D作成発注中: 値はありません";
                        }
                        else
                        {
                            UserMoodelcreationonorderDiplayName = $"3D作成発注中:『{stArcSuitePreview.user_modelcreationonorder}』";
                        }

                        // テキストラベルｺﾝﾎﾟｰﾈﾝﾄに反映
                        InvokeRequired_Control_Text(modelcreationonorder_label, UserMoodelcreationonorderDiplayName);

                        // 登録日時をテキストラベルに反映
                        string dateTime_system_createdOn_String = dateTime_system_createdOn.ToString("yyyy/MM/dd , HH:mm");

                        // ファイルのタイムスタンプを取得
                        if (System.IO.File.Exists(CadDataFullFileName) == true)
                        {
                            DateTime CADfileCreatedTime = default;
                            DateTime CadfileLastTime = default;

                            CADfileCreatedTime = System.IO.File.GetCreationTime(CadDataFullFileName);
                            CadfileLastTime = System.IO.File.GetLastWriteTime(CadDataFullFileName);

                            if (CADfileCreatedTime != null)
                            {
                                string CADfileCreatedTime_String = CADfileCreatedTime.ToString("yyyy/MM/dd , HH:mm");

                                string ArcSuiteCreatedOn_label_ToolTip = $"CADﾌｧｲﾙ【{CadDataFullFileName}】更新時刻:{CADfileCreatedTime_String}";

                                bool containSameCadType = stArcSuitePreview.user_cadtype_string.Contains(SetUnsetCadType.ToString());

                                // 現在開いているCADタイプが、アークスイートから検索した図面のCADタイプに含まれておらず、日付の比較の結果ｱｰｸｽｲｰﾄが新しければ警告します
                                if ((containSameCadType == false) && (dateTime_system_createdOn > CADfileCreatedTime))
                                {
                                    ArcSuiteCreatedOn_label_MessageSet($"登録日時:{dateTime_system_createdOn_String} ※CADﾌｧｲﾙの更新日付{CADfileCreatedTime_String}より新しい", Color.Red, ArcSuiteCreatedOn_label_ToolTip);
                                }
                                else
                                {
                                    ArcSuiteCreatedOn_label_MessageSet($"登録日時:{dateTime_system_createdOn}", Color.Black, ArcSuiteCreatedOn_label_ToolTip);
                                }
                            }
                            else
                            {
                                ArcSuiteCreatedOn_label_MessageSet($"登録日時:{dateTime_system_createdOn}", Color.Black, "");
                            }
                        }
                        else
                        {
                            string ArcSuiteCreatedOn_label_ToolTip = $"CADﾌｧｲﾙは見つかりません";
                            ArcSuiteCreatedOn_label_MessageSet($"登録日時:{dateTime_system_createdOn}", Color.Black, ArcSuiteCreatedOn_label_ToolTip);
                        }

                        InvokeRequired_Control_Text(DrawingInfoLabel2, $"図面番号:{stArcSuitePreview.user_zuban} , 改版:{stArcSuitePreview.system_editionNumber} ", Color.Black);

                        InvokeRequired_Control_Text(DrawingInfoLabel3, $"部品名 / 説明:{ArcSuiteDrawingNameOrDescription} , 状態:{SystemStatusDisplayName}", Color.Black);

                        InvokeRequired_Control_Text(DrawingInfoLabel4, $"CADﾀｲﾌﾟ:{stArcSuitePreview.user_cadtype_string}", Color.Black);

                    }
                    string SearchTimeString = DateTime.Now.ToString("yyyy/MM/dd , HH:mm:ss");
                    InvokeRequired_Control_Text(FindTimeStamp_label, $"検索実行日時：{SearchTimeString}", Color.Black);
                }
                WriteLine($"■ArcSuitePreviewForm.PreviewSet() 正常終了");
            }
            catch (Exception ex)
            {
                WriteLine($"※※例外検知 ArcSuitePreviewForm.PreviewSet()のどこかで。{ex.Message}");
            }

            bool _dummy()
            {
                return false; // このメソッドの内容は何でもよい
            }

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

            if ((bool)TitleBlockScale_button.Tag == false)
            {

                TitleBlockScale_button.Tag = true;
                TitleBlockScale_button.Text = "元に戻す";

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
                TitleBlockScale_button.Tag = false;
                TitleBlockScale_button.Text = "表題欄\r\n拡大";

                // ﾌｨｯﾄへ
                ViewFit();
            }
        }

        /// <summary>
        /// ■デバッグ用
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void numericUpDown_ValueChanged(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// ■デバッグ用
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SCALE_numericUpDown_ValueChanged(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// ■デバッグ用
        /// </summary>
        private void showTestlabel()
        {
            //float test_cur_x;
            //float test_cur_y;
            //float test_cur_scale;

            //WriteLine($"test_cur_x:{test_cur_x} test_cur_y:{test_cur_y} test_cur_scale:{test_cur_scale}");
            WriteLine($"test_cur_x:{test_cur_x} test_cur_y:{test_cur_y}");
        }

        private void ClipBoardTextSearch_button_Click(object sender, EventArgs e)
        {
            if (Delegate_ClipBoardTextSearchMethod != null)
            {

                try
                {
                    Delegate_ClipBoardTextSearchMethod();
                }
                catch (Exception ex)
                {
                    this.WriteLine($"ActiveDocmentSearch_Button_Click(..)にて例外 {ex.Message}");
                }

            }

        }
    }
}
