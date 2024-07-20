using SasaLib;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
#if NETCOREAPP
using System.Runtime.Versioning;
#endif

namespace ClientApp.Forms
{
#if NETCOREAPP
    [SupportedOSPlatform("windows")]
#endif
    public partial class PreviewImageForm : Form
    {
        static bool EnableLeftButtonDrag = false;
        static bool EnableMiddleButtonDrag = true;
        static bool EnableRightButtonDrag = false;


        /// <summary>
        /// デバッグ用
        /// </summary>
        public bool DebugMode { get; set; } = false;

        /// <summary>
        /// フォームが表示されているかを保持
        /// </summary>
        public static bool FormShow;

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
        /// 図面イメージのソースBitmapオブジェクト
        /// </summary>        
        private Bitmap sourceBitmap = null;
        private static readonly object sourceBitmap_LockHandler = new object();


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
        SasaLibDelegateWriteLine WriteLine;


        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="CallDestination"></param>
        public PreviewImageForm(SasaLibDelegateWriteLine CallDestination = null)
        {
            // デバッグメッセージデリゲート先選択
            if (CallDestination == null) this.WriteLine = DebugConsole.Write; else this.WriteLine = CallDestination;

            InitializeComponent();

            // マウスホイールイベント関連
            PreviewArcSuitePictureBox.MouseWheel += pictureBox1_MouseWheel;
        }

        private void PreviewArcSuiteForm_Shown(object sender, EventArgs e)
        {
            FormShow = true;
            // ウィンド位置を指定
            this.Location = new Point(this.Owner.Location.X + (this.Owner.Width - this.Width) / 4 + 300, this.Owner.Location.Y + (this.Owner.Height - this.Height) / 4);

        }

        private void PreviewArcSuiteForm_Load(object sender, EventArgs e)
        {

        }

        private void PreviewArcSuiteForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;
            }
        }



        private void Hide_button_Click(object sender, EventArgs e)
        {
            this.Hide();
        }


        /// <summary>
        /// ■マウス ボタンプッシュダウンイベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BigPreviewPictureBox_MouseDown(object sender, MouseEventArgs e)
        {
            //if (DebugMode)
            //    showTestlabel();

            // ビットマップが未設定の場合戻る
            if (sourceBitmap == null) return;

            // 右ボタン押下
            //if (EnableRightButtonDrag)
            //{
            //    if (e.Button == MouseButtons.Right)
            //    {
            //        // フォーカスの設定（おまじない)
            //        PreviewArcSuitePictureBox.Focus();

            //        // マウスをクリックした位置の記録  
            //        oldMouseRightButtonClickPoint.X = e.X;
            //        oldMouseRightButtonClickPoint.Y = e.Y;

            //        // オリジナルビットマップの座標を退避
            //        oldSoureBitmapMouseRightButtonClickPoint = GetOrginalImagePoint(e.X, e.Y, _sourceMatAffine);

            //        // マウス移動フラグを立てる  
            //        isRightButtonPushMouseMoving = true;
            //    }
            //}

            // 中央ボタン押下
            if (EnableMiddleButtonDrag)
            {
                if (e.Button == MouseButtons.Middle)
                {
                    // フォーカスの設定（おまじない)
                    PreviewArcSuitePictureBox.Focus();

                    // マウスをクリックした位置の記録  
                    oldMouseMiddleClickPoint.X = e.X;
                    oldMouseMiddleClickPoint.Y = e.Y;

                    // オリジナルビットマップの座標を退避
                    oldSoureBitmapMouseMiddleButtonClickPoint = GetOrginalImagePoint(e.X, e.Y, _sourceMatAffine);

                    // マウス移動フラグを立てる  
                    isMiddleButtonPushMouseMoving = true;
                }
            }

            //// 左ボタン押下
            //if (EnableLeftButtonDrag)
            //{
            //    if (e.Button == MouseButtons.Left)
            //    {
            //        // マウスをクリックした位置の記録  
            //        oldMouseLeftButtonClickPoint.X = e.X;
            //        oldMouseLeftButtonClickPoint.Y = e.Y;

            //        // オリジナルビットマップの座標を退避
            //        oldSoureBitmapMouseLeftButtonClickPoint = GetOrginalImagePoint(e.X, e.Y, _sourceMatAffine);

            //        // マウス移動フラグを立てる  
            //        isLeftButtonPushMouseMoving = true;
            //    }
            //}

        }

        /// <summary>
        /// ■マウス移動イベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BigPreviewPictureBox_MouseMove(object sender, MouseEventArgs e)
        {

            //if (DebugMode)
            //    showTestlabel();

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
                    //lblDst.Text = $"マウスポインタ座標:({e.X},{e.Y}) 実イメージ上の座標値:({oldSoureBitmapMouseLeftButtonClickPoint.X:#.#}, {oldSoureBitmapMouseLeftButtonClickPoint.Y:#.#}) ";
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
                    //lblDst.Text = $"マウスポインタ座標:({e.X},{e.Y}) 実イメージ上の座標値:({oldSoureBitmapMouseRightButtonClickPoint.X:#.#}, {oldSoureBitmapMouseRightButtonClickPoint.Y:#.#}) ";
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
        private void BigPreviewPictureBox_MouseUp(object sender, MouseEventArgs e)
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

            //InvokeRequired_Control_Text(label1, $"解像度 Width{sourceBitmap.VerticalResolution} / Height{sourceBitmap.HorizontalResolution} | (Width,Height) = ( {sourceBitmap.Width} , {sourceBitmap.Height} )");
            //InvokeRequired_Control_Text(label2, $"左上:{points[0].X},{points[0].Y} | 右上:{points[1].X},{points[1].Y} | 左下:{points[2].X},{points[2].Y} ");
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
                pictureBoxImageGraphics.Clear(PreviewArcSuitePictureBox.BackColor);

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
                if (pictureBoxImageGraphics != null)
                    pictureBoxImageGraphics.InterpolationMode = interpolationMode;
            }
            catch (Exception ex)
            {
                SasaLib.Eventlog.Log.WriteEntry("SasaLibArcSuitePreview", EventLogEntryType.Error, 0, $"※ArcSuitePreviewForm.DrawImage(..) graphics.InterpolationMode = interpolationMode; にて例外 {ex.Message}");
            }

            // 描画
            //pictureBoxImageGraphics.DrawImage(sourceBitmap, 0, 0);
            InvokeRequired_Graphics_DrawImage(pictureBoxImageGraphics, sourceBitmap, 0, 0);

            //Debug_panel.BackColor = Color.Transparent;

            // ピクチャボックスをリフレッシュし再描画          
            if (InvokeRequired)
            {
                Invoke(new Action(() =>
                {
                    /// UIを操作する処理
                    PreviewArcSuitePictureBox.Refresh();
                }));
            }
            else
            {
                PreviewArcSuitePictureBox.Refresh();
            }

        }

        /// <summary>
        /// ■フォームがリサイズされた
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BigPreviewForm_Resize(object sender, EventArgs e)
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
                if (PreviewArcSuitePictureBox.Image != null) PreviewArcSuitePictureBox.Image.Dispose();

                // PictureBoxに描画領域用の空のビットマップを割り当てる。
                PreviewArcSuitePictureBox.Image = new Bitmap(PreviewArcSuitePictureBox.Width, PreviewArcSuitePictureBox.Height);

                // PictureBoxから新たなGraphicsオブジェクトを取得し退避  
                pictureBoxImageGraphics = Graphics.FromImage(PreviewArcSuitePictureBox.Image);

                // 補間モードの設定（NearestNeighbor）  
                pictureBoxImageGraphics.InterpolationMode = InterpolationMode.NearestNeighbor;

                // 画像の描画  
                DrawImage();

            }
            catch (Exception ex)
            {
                Console.WriteLine($"※PreviewArcSuiteForm_Resize(..)にて例外 {ex.Message}");
            }


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
                if (InvokeRequired)
                {
                    Invoke(new Action(() =>
                    {
                        /// UIを操作する処理
                        try
                        {
                            graphics.Transform = matrix;
                        }
                        catch { }
                    }));
                }
                else
                {
                    graphics.Transform = matrix;
                }
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
                if (InvokeRequired)
                {
                    Invoke(new Action(() =>
                    {
                        /// UIを操作する処理
                        graphics.DrawImage(image, x, y);
                    }));
                }
                else
                {
                    graphics.DrawImage(image, x, y);
                }
            }
            catch (Exception ex)
            {
                this.WriteLine($"※InvokeRequired_Graphics_DrawImage(...)にて例外{ex.Message}");
            }
        }

        /// <summary>
        /// ●ピクチャボックスにもnullを指定します
        /// </summary>
        public void PreviewMsgClear()
        {
            PreviewArcSuitePictureBox.Image = null;


            SasaLib.DoEvents.Run();
        }

        private void PreviewImageCacheClear()
        {
            if (string.IsNullOrWhiteSpace(this.recent_temporalyDrawingImageFullFileName) == false)
            {
                string removeFolder = System.IO.Path.GetDirectoryName(this.recent_temporalyDrawingImageFullFileName);
                var result = SasaLib.FileFolder.RemoveFolder(removeFolder, true);
                if (result == true)
                    DebugConsole.WriteLine($"■ArcSuiteイメージﾌﾟﾚﾋﾞｭｰﾌｧｲﾙ {this.recent_temporalyDrawingImageFullFileName}をフォルダごと削除しました");
                else
                    DebugConsole.WriteLine($"※ArcSuiteイメージﾌﾟﾚﾋﾞｭｰﾌｧｲﾙ {this.recent_temporalyDrawingImageFullFileName}をフォルダごと削除に失敗しました");
            }
        }

        /// <summary>
        /// ●ピクチャーボックスにフィットさせる
        /// </summary>
        private void ViewFit()
        {
            lock (sourceBitmap_LockHandler)
            {

                try
                {

                    //this.X_numericUpDown.ValueChanged -= new System.EventHandler(this.numericUpDown_ValueChanged);
                    //this.Y_numericUpDown.ValueChanged -= new System.EventHandler(this.numericUpDown_ValueChanged);
                    //this.SCALE_numericUpDown.ValueChanged -= new System.EventHandler(this.SCALE_numericUpDown_ValueChanged);

                    if (PreviewArcSuitePictureBox.Image == null) return;

                    int PictureBoxSizeWidth = PreviewArcSuitePictureBox.Size.Width;
                    int PictureBoxSizeHeight = PreviewArcSuitePictureBox.Size.Height;
                    //Console.WriteLine($"{PictureBoxSizeWidth},{PictureBoxSizeHeight}");

                    int ImageSizeWidth = sourceBitmap.Width;
                    int ImageSizeHeight = sourceBitmap.Height;
                    //Console.WriteLine($"{ImageSizeWidth},{ImageSizeHeight}");


                    float scaleWidth = ((float)ImageSizeWidth / sourceBitmap.HorizontalResolution) / (PictureBoxSizeWidth / PreviewArcSuitePictureBox.Image.HorizontalResolution);
                    float scaleHeigth = ((float)ImageSizeHeight / sourceBitmap.VerticalResolution) / (PictureBoxSizeHeight / PreviewArcSuitePictureBox.Image.VerticalResolution);

                    float _scale = (float)(1 / System.Math.Max(scaleWidth, scaleHeigth));

                    // アフィン変換行列をリセット 
                    _sourceMatAffine.Reset();

                    _sourceMatAffine.Scale(_scale, _scale);

                }
                catch { }

                // 画像の描画  
                DrawImage();

                //this.X_numericUpDown.ValueChanged += new System.EventHandler(this.numericUpDown_ValueChanged);
                //this.Y_numericUpDown.ValueChanged += new System.EventHandler(this.numericUpDown_ValueChanged);
                //this.SCALE_numericUpDown.ValueChanged += new System.EventHandler(this.SCALE_numericUpDown_ValueChanged);
            }
        }

        /// <summary>
        /// ■イメージをセットする
        /// </summary>
        /// <param name="img"></param>
        public void SetImage(Image img)
        {
            lock (sourceBitmap_LockHandler)
            {



                sourceBitmap = (Bitmap)img;

                // イメージの読み込みとセット
                orignalResolution = System.Math.Max(sourceBitmap.HorizontalResolution, sourceBitmap.VerticalResolution);
                // インデックス付き対策のため
                sourceBitmap = (Bitmap)sourceBitmap.GetThumbnailImage(sourceBitmap.Width, sourceBitmap.Height, new Image.GetThumbnailImageAbort(_dummy), IntPtr.Zero);
                {
                    var Width = sourceBitmap.Width;
                    var Height = sourceBitmap.Height;
                    var VerticalResolution = sourceBitmap.VerticalResolution;
                    var HorizontalResolution = sourceBitmap.HorizontalResolution;
                    WriteLine($"Width,Height = ({Width}{Height}) VerticalResolution={VerticalResolution} , HorizontalResolution={HorizontalResolution}");
                }
            }
            // 初期化の為リサイズイベントを強制的に実行
            BigPreviewForm_Resize(null, null);

            // ウィンドに合わせる
            ViewFit();


            bool _dummy()
            {
                return false; // このメソッドの内容は何でもよい
            }

        }

        /// <summary>
        /// ｱｰｸｽｲｰﾄ登録図面および一部属性値をﾌﾟﾚﾋﾞｭｰイメージにセット
        /// </summary>
        /// <param name="TiffTempFullfile"></param>
        /// <param name="ArcsuitePreview"></param>
        //internal void PreviewSet(string TiffTempFullfile, ArcsuitePreviewData ArcsuitePreview)
        //{
        //    if (string.IsNullOrWhiteSpace(TiffTempFullfile)==false)
        //    {
        //        var arcsuiteImg = SasaLib.ImageUtil.FromFile(TiffTempFullfile);

        //        SetImage(arcsuiteImg);


        //        ArcSuiteEditionNumber_textBox.Text = ArcsuitePreview.system_editionNumber;
        //        if (ArcsuitePreview.user_partname != null)
        //        {
        //            ArcSuitePARTNAMEandDESCRIPTION_textBox.Text = ArcsuitePreview.user_partname;
        //        }
        //        else if (ArcsuitePreview.user_description != null)
        //        {
        //            ArcSuitePARTNAMEandDESCRIPTION_textBox.Text = ArcsuitePreview.user_description;
        //        }
        //        else
        //            ArcSuitePARTNAMEandDESCRIPTION_textBox.Text = "";
        //    }
        //}


        public void PreviewClear()
        {
            PreviewArcSuitePictureBox.Image = null;
        }

    }
}
