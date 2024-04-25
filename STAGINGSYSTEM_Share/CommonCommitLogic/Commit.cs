//
// Autodesk Inventor 用 コミット実行クラス
//
using SasaLib;
using SasaLib.PrintConfig;
using System;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Xml.Serialization;
using SIP = System.IO.Path;
using SasaLib.NumberingSupport;
using System.Linq;
using SasaLib.ArcSuitePreview;
using System.Threading;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Runtime.Versioning;

namespace CommonCommitLogic
{
    /// <summary>
    /// ticketオブジェクトを各CADの機能を使って更新するためのデリゲート
    /// </summary>
    /// <param name="ticket"></param>
    /// <returns></returns>
    public delegate bool Delegate_ModifyAttrObject_Method(ref CommonTicket ticket);

    /// <summary>
    /// 各CADの機能を使ってコミット用イメージ(tiff形式ファイル)を作成するためのデリゲート
    /// </summary>
    /// <param name="tempBaseFileFullpathWithoutExt"></param>
    /// <param name="ImagePositionConfigXML"></param>
    /// <param name="currentPaperSize"></param>
    /// <returns></returns>
    public delegate bool Delegate_ExecuteTiffExport_Method(string tempBaseFileFullpathWithoutExt, string ImagePositionConfigXML, out CommonPaperSize currentPaperSize);

    /// <summary>
    /// コミット実行
    /// UIからのイベント発生時に呼ばれる
    /// </summary>
    [SupportedOSPlatform("windows")]
    public class Commit
    {
        /// <summary>
        /// コミット動作の再入を防止するフラグ インスタンス間共通
        /// </summary>
        private static bool CommitBlock = false;

        private IWin32Window nativeWindow;

        private SasaLibDelegateWriteLine WriteLine = DebugConsole.WriteLine;

        private Delegate_ModifyAttrObject_Method Delegate_ModifyAttrObject_Method;
        private Delegate_ExecuteTiffExport_Method Delegate_ExecuteTiffExport_Method;

        private string CADDocumentFullFileName;

        private GUIDExtensions guid;
        private string TICKETCODE;

        /// <summary>
        /// コミットシステムに対する固定的な設定を保持（コミットの都度変化するデータは含まない）
        /// </summary>
        private CommitParam commitParam;

        private CommitSupport commitSupport;

        #region フィールド

        // テンプレートチケットファイルのフルパス名
        private string TemplateTicketFullpath;

        // イメージポジション修正設定ファイルのフルパス名
        private string ImagePosXMLfileFullPath;

        // チケットテンプレート解読後の説明を保持
        private string analyzeMsg;
        private NativeWindow nativeWindow1;
        private CommitParam commitParam1;
        private object autoCadModifyAttrObject;
        private object executeTiffExport;
        private string v1;
        private string v2;
        private Action<string> writeLine;

        #endregion

        #region メソッド

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="nativeWindow">CADウィンドウ</param>
        /// <param name="commitParam">Commitシステムパラメータ</param>
        /// <param name="Delegate_ModifyAttrObject_Method">デリゲート・CAD側がチケットオブジェクトを作成するメソッド</param>
        /// <param name="Delegate_ExecuteTiffExport_Method">デリゲート・CAD側がｺﾐｯﾄ用Tiffイメージを生成するメソッド</param>
        /// <param name="TemplateTicketFullpath">CAD側のプロパティと共通チケットフォーマットを結びつけるテンプレートファイルのフルパス</param>
        /// <param name="ImagePosXMLfileFullPath">書き出すTIFFイメージの作成ついて定義</param>
        /// <param name="debugWriteLine"></param>
        public Commit(IWin32Window nativeWindow, CommitParam commitParam, Delegate_ModifyAttrObject_Method Delegate_ModifyAttrObject_Method, Delegate_ExecuteTiffExport_Method Delegate_ExecuteTiffExport_Method, string TemplateTicketFullpath, string ImagePosXMLfileFullPath, SasaLibDelegateWriteLine debugWriteLine)
        {
            this.nativeWindow = nativeWindow;
            this.Delegate_ModifyAttrObject_Method = Delegate_ModifyAttrObject_Method;
            this.commitParam = commitParam;
            this.Delegate_ExecuteTiffExport_Method = Delegate_ExecuteTiffExport_Method;
            this.TemplateTicketFullpath = TemplateTicketFullpath;
            this.ImagePosXMLfileFullPath = ImagePosXMLfileFullPath;
            this.WriteLine = debugWriteLine;

            if (commitParam == null)
                throw new Exception("Commit.Commit(..) CommitParamがnull");

            var objectCheck = ObjectInitializeCheck(commitParam);
            if (objectCheck.Count > 0)
            {
                string nullNamesStr = string.Join(",", objectCheck);

                MessageBox.Show($"CommitParam の フィールドまたはプロパティの次のものが初期化されていません\r\n{nullNamesStr}","東陽ｱﾄﾞｲﾝ エラー",MessageBoxButtons.OK, MessageBoxIcon.Error);
                throw new Exception($"Commit.Commit(..) CommitParam の フィールド・プロパティの次のものが初期化されていません {nullNamesStr}");
            }

            commitSupport = new CommitSupport(this.commitParam);
        }

        public Commit(NativeWindow nativeWindow1, CommitParam commitParam1, object autoCadModifyAttrObject, object executeTiffExport, string v1, string v2, Action<string> writeLine)
        {
            this.nativeWindow1 = nativeWindow1;
            this.commitParam1 = commitParam1;
            this.autoCadModifyAttrObject = autoCadModifyAttrObject;
            this.executeTiffExport = executeTiffExport;
            this.v1 = v1;
            this.v2 = v2;
            this.writeLine = writeLine;
        }

        private List<string> ObjectInitializeCheck(object myObject)
        {
            List<string>nullName = new List<string>();

            // オブジェクトの型を取得
            Type objectType = myObject.GetType();

            // クラスのフィールドを取得
            FieldInfo[] fields = objectType.GetFields();

            // クラスのプロパティを取得
            PropertyInfo[] properties = objectType.GetProperties();

            // フィールドの初期化を確認
            foreach (FieldInfo field in fields)
            {
                object value = field.GetValue(myObject);
                if (value == null)
                {
                    WriteLine($"※ObjectInitializeCheck(..) フィールド  '{field.Name}'が 初期化されていません");
                    nullName.Add(field.Name);
                }
            }

            // プロパティの初期化を確認
            foreach (PropertyInfo property in properties)
            {
                object value = property.GetValue(myObject);
                if (value == null)
                {
                    WriteLine($"※ObjectInitializeCheck(..) プロパティ '{property.Name}'が 初期化されていません");
                    nullName.Add(property.Name);
                }
            }

            return nullName;
        }

        /// <summary>
        /// ■コミットスタート
        /// </summary>
        /// <param name="RequestPrinterStr">プリンタ設定XMLファイル名　例:"PRINTER1.XML"(ショートカット可)</param>
        /// <returns></returns>
        public bool CommitStart(string CADDocumentFullFileName, string RequestPrinterStr, ref string RecentTicketCode, ref bool RecentCommitStatus)
        {
            if (string.IsNullOrWhiteSpace(RequestPrinterStr))
            {
                WriteLine($"■Commit.CommitStart(..)[1] 開始。 RequestPrinterStr は null またはスペース でした \"\"に変更します");

                RequestPrinterStr = "";
            }
            else
            {
                WriteLine($"■Commit.CommitStart(..)[1] 開始。RequestPrinterStr は \"{RequestPrinterStr}\" です");
            }

            if (CommitBlock == false)
                CommitBlock = true;
            else
            {
                WriteLine($"※Commit.CommitStart(..) 一つ前のコミットがまだ実行中.メッセージボックスにてＯＫ待ち");
                MessageBox.Show($"一つ前のコミットがまだ実行中です\r\nこの操作は受付られません。OKを押して前のコミットを終了させてください。", "■東陽ﾂｰﾙ ※警告※");
                WriteLine($"※Commit.CommitStart(..) CommitStart(..)を false にて終了します");
                return false;
            }

            try
            {
                bool commitResult = false;

                this.CADDocumentFullFileName = CADDocumentFullFileName;

                /// 一時TIFFファイルのフルパスを生成（拡張子無し）
                string tempBaseFileFullpathWithoutExt = System.IO.Path.Combine(SIP.GetTempPath(), SIP.GetRandomFileName());


                /// コミット受付可能かをサーバーに問い合わせる。
                string Message;
                if (commitSupport.CheckCommitRecepitonState(out Message, "呼出し元, CommitExecute.CommitStart(..)", WriteLine) == false)
                {
                    MessageBox.Show(nativeWindow, $"{Message}");
                    WriteLine($"※Commit.CommitStart(..)[2] コミットサーバー \"{commitParam.StageServerHost}\"。Commit.CommitStart(..) コミットサーバー \"{commitParam.StageServerHost}\"。コミット先は受付不能(受信メッセージ\"{Message}\")。コミット先は受付不可(受信メッセージ\"{Message}\")。終了します");
                    return false;
                }
                else
                {
                    WriteLine($"■Commit.CommitStart(..)[2]  コミットサーバー \"{commitParam.StageServerHost}\"。コミット先は受付可能(受信メッセージ\"{Message}\")。続行します");
                }

                if (string.IsNullOrWhiteSpace(RequestPrinterStr) == false)
                {
                    if (commitSupport.IsCommitPrinterFiler(RequestPrinterStr, WriteLine))
                    {
                        WriteLine($"※Commit.CommitStart(..)[2-1] リクエストされたコミットプリンター\"{RequestPrinterStr}\"は故障中です");

                        MessageBox.Show(nativeWindow, $"リクエストされたコミットプリンター\"{RequestPrinterStr}\"は故障中です");
                        return false;
                    }
                }

                // 一度も保存されていない場合はメッセージを表示して正常終了とする
                if (CADDocumentFullFileName == null)
                {
                    MessageBox.Show(nativeWindow, "このCAD図面ファイルは一度も保存されていません。コミットは保存されたファイルのみ実行可能です。");
                    WriteLine($"※Commit.CommitStart(..)[2-1]  このCAD図面ファイルは一度も保存されていませんでした。コミットはここで終了させます");
                    return true;
                }


                // コミットフォルダがあるかチェック
                if (FileFolder.DirExists(commitParam.CommitPath) != true)
                {
                    WriteLine($"※Commit.CommitStart(..)[3] コミット先フォルダ {commitParam.CommitPath} に接続できません。");
                    MessageBox.Show(nativeWindow, "コミット先フォルダ [" + commitParam.CommitPath + "]に接続できません。ステージサーバーが稼働していない？");
                    return false;
                }
                WriteLine($"■Commit.CommitStart(..)[3] コミット先フォルダ {commitParam.CommitPath} に接続成功");

                // コミット待ち数を調査。閾値より多い場合はコミット中止
                int remainCommit = commitSupport.GetGemainingDrawing();
                if (remainCommit > commitParam.CommitQueueThreshold)
                {
                    MessageBox.Show(nativeWindow, $"※ステージサーバーでコミット待ちファイルが{remainCommit}件以上待機中です。コミットを中止します");
                    WriteLine($"※Commit.CommitStart(..)[3-1] ステージサーバーでコミット待ちファイルが{remainCommit}件以上待機中です。コミットを中止します\n");
                    return false;
                }


                // チケットコード生成
                SetNewTicketName();
                WriteLine($"■Commit.CommitStart(..)[4] チケットコードを生成しました 【{TICKETCODE}】");

                // チケットテンプレート存在確認・なければ作成・指定したバージョンより前なら問答無用で書き戻し
                UpdateTicketTemplateFile(TemplateTicketFullpath, 1.6d);

                // チケットテンプレートからオブジェクトを生成する。
                CommonTicket ticketXml = CreateNewTicketData(TemplateTicketFullpath, TemplateTicketFullpath);

                if (ticketXml == null)
                {
                    WriteLine("※Commit.CommitStart(..)[5] エラー：チケットオブジェクトのメモリ展開に失敗.処理を中止します");
                    MessageBox.Show("エラー：チケットオブジェクトのメモリ展開に失敗.処理を中止します");
                    return false;
                }

                // CommonTicket ticketXml をCAD側プロパティからの情報を得て修正する
                WriteLine($"■Commit.CommitStart(..)[5]  処理対象の図面を調べチケットオブジェクトを更新します。ticketXmlは以降に必要な要素で再構築されます");

                bool modifyticketAnser = Delegate_ModifyAttrObject_Method(ref ticketXml);
                if (modifyticketAnser == false)
                {
                    WriteLine($"※Commit.CommitStart(..)[6]  チケットオブジェクトの更新に失敗しました");
                    MessageBox.Show(nativeWindow, $"チケットオブジェクトの更新に失敗しました。コミットに必要な表題情報を得られませんでした");
                    return false;
                }

                WriteLine($"■Commit.CommitStart(..)[6]  チケットデータParamで必要なキーが抜けて入れば追加します");
                // SANITIZEDPARTNUMBER は必ず必要
                if (ticketXml.Params.FindLast(a => a.Key == "SANITIZEDPARTNUMBER").Key == null)
                {
                    ticketXml.Params.Add(new CommonTicket.Param { Key = "SANITIZEDPARTNUMBER" });
                    WriteLine($"※Commit.CommitStart(..)[6-1]  SANITIZEDPARTNUMBERがParamsに無いため追加しました");
                }

                // DRAWINGTYPE は必ず必要
                if (ticketXml.Params.FindLast(a => a.Key == "DRAWINGTYPE").Key == null)
                {
                    ticketXml.Params.Add(new CommonTicket.Param { Key = "DRAWINGTYPE" });
                    WriteLine($"※Commit.CommitStart(..)[6-2]  DRAWINGTYPEがParamsに無いため追加しました");
                }

                //// チケットデータの妥当性を確認します
                //if (CheckCommitData(ticketXml) == false)
                //{
                //    WriteLine("※Commit.CommitStart(..)[6-3]  エラー：図面番号・表題などの属性を読み取れません.処理を中止します");
                //    MessageBox.Show(nativeWindow, "エラー：図面番号・表題などの属性を読み取れません.処理を中止します");
                //    return false;
                //}

                #region 部品番号のサニタイズ
                string _partnumber = (string)ticketXml.GetParamKeyValue("PARTNUMBER");

                // 部品番号に漢字・ひらがな・かたかなが混じっていないかチェック
                if (StringUtil.CheckKanji(_partnumber) == true)
                {
                    MessageBox.Show(nativeWindow, "図面番号に全角のひらがな・カタカナ・漢字は使えません");
                    WriteLine("※Commit.CommitStart(..)[6-4]  エラー：図面番号に全角のひらがな・カタカナ・漢字は使えません\n");
                    return false;
                }


                // 部品番号、全角は半角化
                _partnumber = StringUtil.Zen2Han(_partnumber);

                // 部品番号、大文字化
                _partnumber = _partnumber.ToUpper();

                // 改定番号 が 数値文字列だけで構成されているか
                string _revnumber = (string)ticketXml.GetParamKeyValue("REV");

                if (_revnumber != null)
                {
                    if (_revnumber.All(char.IsDigit) == false)
                    {
                        MessageBox.Show(nativeWindow, "改定番号に全角のひらがな・カタカナ・漢字は使えません");
                        WriteLine("※Commit.CommitStart(..)[6-5]  エラー：改定番号に全角のひらがな・カタカナ・漢字は使えません\n");
                        return false;
                    }

                    // 改訂番号、全角は半角化
                    _revnumber = _revnumber.ToUpper();
                }



                #endregion

                // TIFFイメージ書き出し------------------------------------------------------------------------------------------------------------------------------------------------------------
                CommonPaperSize cuurenetPaperSize;
                WriteLine($"■Commit.CommitStart(..)[7]  Delegate_ExecuteTiffExport_Method(..)メソッドを呼び出します。 Tiffファイルを生成開始");
                if (Delegate_ExecuteTiffExport_Method(tempBaseFileFullpathWithoutExt, ImagePosXMLfileFullPath, out cuurenetPaperSize) == false)
                {
                    WriteLine($"※Commit.CommitStart(..)[7]   Delegate_ExecuteTiffExport_Method(..)の戻り値がfalseです。メソッドをfalseで抜けます");
                    return false;
                }
                else
                    WriteLine($"■Commit.CommitStart(..)[8]  Tiffファイルの生成に成功しました tempBaseFileFullpathWithoutExt =  {tempBaseFileFullpathWithoutExt}");

                //// チケットオブジェクトの残りを更新：PLOTPAPERSIZEをセット

                ticketXml.PLOTPAPERSIZE = cuurenetPaperSize.ToString();
                WriteLine($"■Commit.CommitStart(..)[9]  チケット共通項目の更新 PLOTPAPERSIZE = \"{ticketXml.PLOTPAPERSIZE}\"");

                ticketXml.PAPERSIZE = cuurenetPaperSize.ToString();
                WriteLine($"■Commit.CommitStart(..)[10]  チケット共通項目の更新 PAPERSIZE = \"{ticketXml.PAPERSIZE}\"");

                // ⑤チケットオブジェクトを更新(共通項目)：印刷先プリンタ名をセット
                ticketXml.REQUESTPRINTER = RequestPrinterStr;
                WriteLine($"■Commit.CommitStart(..)[11]  チケット共通項目の更新 REQUESTPRINTER = \"{ticketXml.REQUESTPRINTER}\"");

                // ⑥チケットオブジェクトを更新(共通項目)：印刷日時をセット。PrintingTimeStr が"" の時は現在時刻とする
                ticketXml.PRINTINGTIME = DateTime.Now;
                WriteLine($"■Commit.CommitStart(..)[12]  チケット共通項目の更新 PRINTINGTIMEを更新 = \"{ticketXml.PRINTINGTIME}\"");


                WriteLine($"■Commit.CommitStart(..)[12.1] チケットのPramsアレイの確認");
                foreach (var a in ticketXml.Params)
                {
                    if (a.Value !=null)
                    {
                        WriteLine($"\t{a.Key} = \"{a.Value}\"");
                    }
                }

                // チケットデータの妥当性を確認します
                if (CheckCommitData(ticketXml) == false)
                {
                    WriteLine("※Commit.CommitStart(..)[6-3]  エラー：図面番号・表題などの属性を読み取れません.処理を中止します");
                    MessageBox.Show(nativeWindow, "エラー：図面番号・表題などの属性を読み取れません.処理を中止します");
                    return false;
                }

                /// ticketXmlの準備はここで終了

                // CAD Drawingファイルに対する正当性識別オブジェクトの準備
                CommitSupportCadDrawingFile supportCadDrwingFile = new CommitSupportCadDrawingFile(this.commitParam,CADDocumentFullFileName, this.WriteLine);

                // 採番・図番タイプ識別オブジェクトの準備
                CommitSupportNumbering supportNumbering = new CommitSupportNumbering(this.commitParam, this.WriteLine);
    
                // アークスイート登録確認オブジェクトの準備
                CommitSupportArcSuite supportArcSuite = new CommitSupportArcSuite(this.commitParam, this.WriteLine);

                // コミット実行直前ダイアログの準備
                WriteLine($"■Commit.CommitStart(..)[13]  {_partnumber} コミットダイアログの準備");
                //CommitDialogForm commitDialogForm = new CommitDialogForm(commitParam, CADDocumentFullFileName, supportArcSuite, supportNumbering, WriteLine: WriteLine);
                CommitDialogForm commitDialogForm = new CommitDialogForm(commitParam, supportCadDrwingFile, supportArcSuite, supportNumbering, WriteLine: WriteLine);


                WriteLine($"■Commit.CommitStart(..)[14]  {_partnumber} 実行直前ダイアログに値をセットします CADDocumentFullFileName={CADDocumentFullFileName}");
                string errMsg;
                /// チケットデータをダイアログへセット
                bool resultSetData = commitDialogForm.SetData(ticketXml, out errMsg);
                if (resultSetData == false)
                {
                    WriteLine($"※Commit.CommitStart(..)[14-1]  commitDialogForm.SetData(..)の戻り値が false メッセージボックスにてエラーを報告 \"{errMsg}\"");
                    MessageBox.Show($"表題欄の情報に不備があります \"{errMsg}\"", "エラー");
                    return false;
                }
                /// コミットするイメージをダイアログへロード
                commitDialogForm.SetImage(tempBaseFileFullpathWithoutExt + @".tif");


                // 表図面判定の場合　実装途中　　　　　　　　　　　　　　　　　　　　　　　　　　　　　　　　　　　　　　　　　　　　　　　　　　　　　　　　　　　　　　　　　　　　　　　　　　　　　　　　　　　　　　　　　　　　　　　　　　　　　　　　　　　　　　　
                //commitDialogForm.Variant_panel.Enabled = isVariant;
                //commitDialogForm.VariantNumber_MIN_textBox.Enabled = isVariant;
                //commitDialogForm.VariantNumber_MIN_textBox.Text = VariantSuffixMIN;
                //commitDialogForm.VariantNumber_MAX_textBox.Enabled = isVariant;
                //commitDialogForm.VariantNumber_MAX_textBox.Text = VariantSuffixMAX;
                //commitDialogForm.VariantNew_textBox.Enabled = isVariant;

                /// 材質名から適する材質コードが選択されているかを比較する。正しい材質―ド または 材質名・材質コードのいずれかに入力が無い場合はtrue
                bool materialcodeans = SasaLib.NumberingSupport.MaterialCodeCheck.ConfirmDiscrepancy((string)ticketXml.GetParamKeyValue("MATERIAL"), (string)ticketXml.GetParamKeyValue("MATERIALCODE"), (NativeWindow)nativeWindow, WriteLine);
                if (materialcodeans == false)
                {
                    WriteLine(
                        $"※Commit.CommitStart(..)  材質コード不一致警告に対して \"処理を中止\"　が選択されました:　材質 {(string)ticketXml.GetParamKeyValue("MATERIAL")} に対して" +
                        $"材質コードは {(string)ticketXml.GetParamKeyValue("MATERIALCODE")} と指定されていました");
                    return true;
                }

                /// ダイアログ表示のための前準備完了

                /// コミット実行直前ダイアログ表示開始
                WriteLine($"■Commit.CommitStart(..)[15]  コミット実行直前ダイアログを表示 図面番号:{_partnumber}");
                DialogResult dialogResult = commitDialogForm.ShowDialog(nativeWindow);

                if (dialogResult != DialogResult.OK)
                {
                    WriteLine($"※Commit.CommitStart(..)[15-1]  コミットダイアログはキャンセルされました。 {_partnumber} コミットは直前でキャンセルされました");
                    commitResult = false;
                }
                else if (dialogResult == DialogResult.OK)
                {
                    // コミット最終実行処理
                    WriteLine($"■Commit.CommitStart(..)[15-2]   コミットダイアログはOKにて閉じられました。");

                    if (supportNumbering.TypeNumber.isVariant && commitDialogForm.AllVariantNumberRegistMode_checkbox.Checked)
                    {
                        WriteLine($"■Commit.CommitStart(..)[15-3]   図面番号:{_partnumber}は表形式図面として処理するオーダーを受領しました");
                    }


                    WriteLine($"■Commit.CommitStart(..)[16]   チケットファイル【{tempBaseFileFullpathWithoutExt}】とTIFFファイルを所定のフォルダへコミットします");
                    if (TicketFinalPhase(ticketXml, tempBaseFileFullpathWithoutExt) == false)
                    {
                        WriteLine($"※Commit.CommitStart(..)[16-1]  チケットファイル【{tempBaseFileFullpathWithoutExt}】とTIFFファイルを所定のフォルダへコミットに失敗しました");
                        return false;
                    }

                    if (supportNumbering.ReserveNumber.CheckAcquiredNumberedNormal == false)
                        WriteLine($"■Commit.CommitStart(..)[17] ※コミットは \"採番実績なしで強行\" されます。ファイル:{CADDocumentFullFileName} , 図面番号{_partnumber} , チケットコード: {TICKETCODE}");
                    else
                        WriteLine($"■Commit.CommitStart(..)[17] ■コミットは\"採番実績あり\" 継続されます。:{supportNumbering.ReserveNumber.CheckNumberdHistoryAnser}）.ファイル:{CADDocumentFullFileName} , 図面番号{_partnumber} , チケットコード: {TICKETCODE}");

                    // 最後のチケットコードを退避
                    RecentTicketCode = ticketXml.TICKETCODE;
                    RecentCommitStatus = true;

                    commitResult = true;
                }

                // ダイアログの解放
                commitDialogForm.Dispose();
                WriteLine($"■Commit.CommitStart(..)[18] コミットダイアログをDisposeしました");

                // テンポラリフォルダ清掃
                bool ans = RemoveTemporaryFiles(tempBaseFileFullpathWithoutExt);
                WriteLine($"■Commit.CommitStart(..)[19] コミット後テンポラリファイル{tempBaseFileFullpathWithoutExt}の消去結果{ans}");

                CommitBlock = false;

                return commitResult;
                // これにてコミット作業は終了です。
            }
            catch (Exception ex)
            {
                WriteLine($"※Commit.CommitStart(..)  にて例外検知 {ex.Message} {ex.InnerException}");

                return false;
            }
            finally
            {
                CommitBlock = false;
                WriteLine($"■ommit.CommitStart(..)[20]   finaly句により CommitBlock を falseにしました");
            }
        }

        /// <summary>
        /// ■ノーコミットスタート
        /// </summary>
        /// <param name="RequestPrinterStr"></param>
        /// <returns></returns>
        public bool NoCommitStart(string CADDocumentFullFileName, string RequestPrinterStr, IWin32Window nativeWindow, SasaLibDelegateWriteLine debugWriteLine)
        {
            if (string.IsNullOrWhiteSpace(RequestPrinterStr))
            {
                WriteLine($"RequestPrinterStr が null またはスペース でした \"\"に変更します");

                RequestPrinterStr = "";
            }

            if (CommitBlock == false)
                CommitBlock = true;
            else
            {
                WriteLine($"※一つ前のコミットがまだ実行中.メッセージボックスにてＯＫ待ち");
                MessageBox.Show($"一つ前のコミットがまだ実行中です\r\nこの操作は受付られません。OKを押して前のコミットを終了させてください。", "■東陽ﾂｰﾙ ※警告※");
                WriteLine($"※CommitStart(..)を false にて終了します");
                return false;
            }

            try
            {
                /// 一時TIFFファイルのフルパスを生成（拡張子無し）
                string tempBaseFileFullpathWithoutExt = System.IO.Path.Combine(SIP.GetTempPath(), SIP.GetRandomFileName());

                WriteLine($"■メソッドNoCommitStart({RequestPrinterStr})開始");

                /// コミット受付可能かをサーバーに問い合わせる。
                string Message;
                if (commitSupport.CheckCommitRecepitonState(out Message, "呼出し元, CommitExecute.NoCommitStart(..)", WriteLine) == false)
                {
                    MessageBox.Show(nativeWindow, $"{Message}");
                    WriteLine($"※コミット先は受付不可(メッセージ{Message})。終了します");
                    return false;
                }

                if (commitSupport.IsCommitPrinterFiler(RequestPrinterStr, WriteLine))
                {
                    MessageBox.Show(nativeWindow, $"リクエストされたコミットプリンター{RequestPrinterStr}は故障中です");
                    return false;
                }

                // コミットフォルダがあるかチェック
                WriteLine($"コミット先フォルダ {commitParam.CommitPath} を確認中です・・・");
                if (FileFolder.DirExists(commitParam.CommitPath) != true)
                {
                    WriteLine($"※コミット先フォルダ {commitParam.CommitPath} に接続できません。");
                    MessageBox.Show(nativeWindow, "コミット先フォルダ [" + commitParam.CommitPath + "]に接続できません。ステージサーバーが稼働していない？");
                    return false;
                }
                WriteLine($"コミット先フォルダ {commitParam.CommitPath} に接続成功");

                // チケットコード生成
                SetNewTicketName();
                WriteLine($"④チケットコードを生成します・・・ {TICKETCODE}");

                // チケットテンプレート存在確認・なければ作成・指定したバージョンより前なら問答無用で書き戻し
                WriteLine($"チケットテンプレートファイル：{TemplateTicketFullpath}");
                UpdateTicketTemplateFile(TemplateTicketFullpath, 1.6d);

                // チケットテンプレートからオブジェクトを生成する。
                WriteLine($"⑥チケットオブジェクト一般情報の生成開始");
                CommonTicket ticketXml = CreateNewTicketData(TemplateTicketFullpath, TemplateTicketFullpath);
                if (ticketXml == null)
                {
                    WriteLine("※エラー：チケットオブジェクトのメモリ展開に失敗.処理を中止します");
                    MessageBox.Show("エラー：チケットオブジェクトのメモリ展開に失敗.処理を中止します");
                    return false;
                }

                ///throw new Exception("ダミー障害発生");

                // Tiffファイルを生成 
                CommonPaperSize cuurenetPaperSize;
                WriteLine($"⑩ExecuteTiffExport(..) Tiffファイルを生成します");
                if (Delegate_ExecuteTiffExport_Method(tempBaseFileFullpathWithoutExt, ImagePosXMLfileFullPath, out cuurenetPaperSize) == false)
                {
                    WriteLine($"⑩ExecuteTiffExport(..)の戻り値がfalseです。CommitStart(..)メソッドをfalseで抜けます");
                    return false;
                }
                else
                    WriteLine($"⑩ExecuteTiffExport(..) Tiffファイルの生成に成功しています tempBaseFileFullpathWithoutExt =  {tempBaseFileFullpathWithoutExt}");

                //// チケットオブジェクトの残りを更新：PLOTPAPERSIZEをセット
                WriteLine($"⑪チケットオブジェクトの残りを更新します");
                ticketXml.PLOTPAPERSIZE = cuurenetPaperSize.ToString();
                WriteLine($"　チケット共通項目 PLOTPAPERSIZEを更新しました {ticketXml.PLOTPAPERSIZE}");

                ticketXml.PAPERSIZE = cuurenetPaperSize.ToString();
                WriteLine($"　チケット共通項目 PAPERSIZEを更新しました {ticketXml.PAPERSIZE}");

                // ⑤チケットオブジェクトを更新(共通項目)：印刷先プリンタ名をセット
                ticketXml.REQUESTPRINTER = RequestPrinterStr;
                WriteLine($"　チケット共通項目 REQUESTPRINTERを更新しました {ticketXml.REQUESTPRINTER}");

                // ⑥チケットオブジェクトを更新(共通項目)：印刷日時をセット。PrintingTimeStr が"" の時は現在時刻とする
                ticketXml.PRINTINGTIME = DateTime.Now;
                WriteLine($"　チケット共通項目 PRINTINGTIMEを更新しました {ticketXml.PRINTINGTIME}");

                // 印刷オンリーフラグ
                ticketXml.PrintOutOnly = true;

                WriteLine($"⑫実行直前ダイアログに値をセットします CADDocumentFullFileName={CADDocumentFullFileName}");

                // CAD Drawingファイルに対する正当性識別オブジェクトの準備
                CommitSupportCadDrawingFile supportCadDrwingFile = new CommitSupportCadDrawingFile(this.commitParam, CADDocumentFullFileName, this.WriteLine);

                // コミット実行直前ダイアログ
                CommitDialogForm commitDialogForm = new CommitDialogForm(commitParam, supportCadDrwingFile, null, null, WriteLine: WriteLine, PrintOutOnly: true);

                string errMsg;
                /// チケットデータをダイアログへセット
                WriteLine($"⑬実行直前ダイアログに値をセットします CADDocumentFullFileName={CADDocumentFullFileName}");
                bool resultSetData = commitDialogForm.SetData(ticketXml, out errMsg);

                /// コミットするイメージをダイアログへロード
                commitDialogForm.SetImage(tempBaseFileFullpathWithoutExt + @".tif");


                /// コミット実行直前ダイアログ表示開始
                WriteLine($"⑭コミット実行直前ダイアログを表示");
                DialogResult dialogResult = commitDialogForm.ShowDialog(nativeWindow);
                if (dialogResult == DialogResult.Cancel)
                {
                    WriteLine($"⑮コミットダイアログはキャンセルされました。");
                }
                else if (dialogResult == DialogResult.OK)
                {
                    // コミット最終実行処理
                    WriteLine($"⑮コミットダイアログはOKにて閉じられました。");

                    WriteLine($"⑮【{tempBaseFileFullpathWithoutExt}】のチケットファイルとTIFFファイルを所定のフォルダへコミットします");
                    if (TicketFinalPhase(ticketXml, tempBaseFileFullpathWithoutExt) == false)
                    {
                        WriteLine($"　コミット最終処理に失敗");
                        return false;
                    }

                    WriteLine($"コミット処理終了");

                    // テンポラリフォルダ清掃
                    bool ans = RemoveTemporaryFiles(tempBaseFileFullpathWithoutExt);
                    WriteLine($"コミット後テンポラリファイル{tempBaseFileFullpathWithoutExt}の消去結果{ans}");

                }
                // ダイアログの解放
                commitDialogForm.Dispose();
                WriteLine($"⑯コミットダイアログをDisposeしました");

                CommitBlock = false;

                return true;
                // これにてNoCommit作業は終了です。
            }

            catch (Exception ex)
            {
                WriteLine($"※CommonCommtLogic.NoCommitStart(..) にて例外検知 {ex.Message} {ex.InnerException}");

                return false;
            }
            finally
            {
                CommitBlock = false;
                WriteLine($"※CommonCommtLogic.NoCommitStart(..) にて例外処理後 finaly句により CommitBlock を falseにしました");
            }
        }

        /// <summary>
        /// TICKETCODEをactiveDocNameとguidB64FnameStringにて生成(GUIDは新たに生成されます)
        /// 'ドキュメント名'-'B64FnameString'
        /// </summary>
        private void SetNewTicketName()
        {
            /// 新しいGUID値を生成
            guid = new GUIDExtensions(true);
            // チケットコードのベースファイル名はGUIDをBase64エンコードし'/'を'_'としたものです。
            //_ticketName = FileFolder.GetFileNameWithoutExtension(activeDocName).Trim() + @"-" + guid.B64FnameString;
            TICKETCODE = guid.B64FnameString;
        }

        /// <summary>
        /// チケットファイルテンプレートを更新
        /// </summary>
        /// <param name="template_ticket_fullpath"></param>
        /// <param name="toriggerDatetime">既存ファイルがこの日時以前ならアップデートする</param>
        public void UpdateTicketTemplateFile(string template_ticket_fullpath, double TrigerVersion)
        {
            // チケットテンプレート存在確認・なければ作成
            if (SasaLib.FileFolder.FileExists(template_ticket_fullpath) != true)
            {
                // チケットテンプレートを作成
                WriteLine($"※Commit.UpdateTicketTemplateFile(..)[1] チケットテンプレートが存在しないため {template_ticket_fullpath} を新規作成しました");
                CommonTicketWork.MakeTicketTemplate(template_ticket_fullpath);
            }
            else
            {
                // ①StreamReader srにチケットテンプレートファイルを読み込む
                System.IO.StreamReader sr = new System.IO.StreamReader(template_ticket_fullpath, new System.Text.UTF8Encoding(false));

                // ②XmlSerializerを使いオブジェクトをCommonTicket ticketへ書き戻す
                XmlSerializer serializer = new XmlSerializer(typeof(CommonTicket));
                CommonTicket ticket = (CommonTicket)serializer.Deserialize(sr);
                sr.Close();
                if (ticket.VERSION < TrigerVersion)
                {
                    // チケットテンプレートを作成
                    CommonTicketWork.MakeTicketTemplate(template_ticket_fullpath);
                    WriteLine($"※Commit.UpdateTicketTemplateFile(..)[1]　チケットテンプレート {template_ticket_fullpath} は バージョンが {TrigerVersion} 以下なのでデフォルト値でを更新しました。");
                }
                else
                {
                    WriteLine($"■Commit.UpdateTicketTemplateFile(..)[1]　チケットテンプレート {template_ticket_fullpath} バージョン {TrigerVersion}でした。このまま使用します。");
                }
            }
        }

        /// <summary>
        /// チケットオブジェクト生成
        /// </summary>
        /// <param name="template_Ticket_fullpath"></param>
        /// <param name="exportedTiffFilename"></param>
        /// <returns></returns>
        private CommonTicket CreateNewTicketData(string template_Ticket_fullpath, string exportedTiffFilename)
        {
            ///チケットオブジェクトのインスタンスを宣言
            CommonTicket ticket;
            // ①StreamReader srにチケットテンプレートファイルを読み込む
            System.IO.StreamReader sr = new System.IO.StreamReader(template_Ticket_fullpath, new System.Text.UTF8Encoding(false));
            WriteLine($"■Commit.CreateNewTicketData(..)[1] チケットテンプレートを読み込みました \"{template_Ticket_fullpath}\"");

            // ②XmlSerializerを使いオブジェクトをCommonTicket ticketへ書き戻す
            XmlSerializer serializer = new XmlSerializer(typeof(CommonTicket));
            ticket = (CommonTicket)serializer.Deserialize(sr);
            sr.Close();
            WriteLine($"■Commit.CreateNewTicketData(..)[2] チケットテンプレートを逆シリアル化しました");

            // ③チケットオブジェクトを更新(共通項目)：タイムスタンプをセット
            ticket.TIMESTAMP = DateTime.Now;
            WriteLine($"■Commit.CreateNewTicketData(..)[3] チケット共通項目 TIMESTAMP を更新しました \"{ticket.TIMESTAMP}\"");

            // ④チケットオブジェクトを更新(共通項目)：チケットコードをセット
            ticket.TICKETCODE = TICKETCODE;
            WriteLine($"■Commit.CreateNewTicketData(..)[4] チケット共通項目 TICKETCODE を更新しました \"{ticket.TICKETCODE}\"");

            // ⑤チケットオブジェクトを更新(共通項目)：GUIDコードをセット
            ticket.GUID = guid.GuidObj; //
            WriteLine($"■Commit.CreateNewTicketData(..)[5] チケット共通項目 GUID を更新しました \"{ticket.GUID}\"");

            // ⑥チケットオブジェクトを更新(共通項目)：GUIDBASE64コードをセット
            ticket.GUIDBASE64 = guid.B64String;
            WriteLine($"■Commit.CreateNewTicketData(..)[6] チケット共通項目 GUIDBASE64 を更新しました \"{ticket.GUIDBASE64}\"");

            // ⑦チケットオブジェクトを更新(共通項目)：CREATESOFTWAREをセット
            ticket.CREATESOFTWARE = commitParam.CREATESOFTWARE;
            WriteLine($"■Commit.CreateNewTicketData(..)[7] チケット共通項目 CREATESOFTWAR を更新しました \"{ticket.CREATESOFTWARE}\"");

            // ⑧チケットオブジェクトを更新(共通項目)：DOCUMENTNAMEをアクティブドキュメントファイル名で設定。ただしnullの場合はexporttedTiffIfilenameを使用
            if (CADDocumentFullFileName == null)
                ticket.DOCUMENTNAME = exportedTiffFilename;
            else
                ticket.DOCUMENTNAME = CADDocumentFullFileName;
            WriteLine($"■Commit.CreateNewTicketData(..)[8] チケット共通項目 DOCUMENTNAME を更新しました \"{ticket.DOCUMENTNAME}\"");

            // ⑩チケットオブジェクトを更新：EXPORTERDLLVERをセット
            ticket.EXPORTERDLLVER = $"Export DLL Version {commitSupport.GetAssemblyVersion()}";
            WriteLine($"■Commit.CreateNewTicketData(..)[9] チケット共通項目 EXPORTERDLLVER を更新しました \"{ticket.EXPORTERDLLVER}\"");

            // ⑪チケットオブジェクトを更新：COMMITHOSTをセット
            ticket.COMMITHOST = Net.GetHOSTNAME();
            WriteLine($"■Commit.CreateNewTicketData(..)[10] チケット共通項目 COMMITHOST を更新しました \"{ticket.COMMITHOST}\"");

            // ⑫チケットオブジェクトを更新：COMMITUSERをセット
            ticket.COMMITUSER = Net.GetLONGONNAME();
            WriteLine($"■Commit.CreateNewTicketData(..)[11] チケット共通項目 COMMITUSER を更新しました \"{ticket.COMMITUSER}\"");

            return ticket;
        }

        /// <summary>
        /// チケットコードの妥当性確認
        /// </summary>
        /// <param name="ticket"></param>
        /// <returns></returns>
        private bool CheckCommitData(CommonTicket ticket)
        {

            bool anser = true;

            if (String.IsNullOrWhiteSpace((string)ticket.GetParamKeyValue("PARTNUMBER")))
            {
                var iprop = (string)ticket.GetINViPropertyName("PARTNUMBER");
                WriteLine($"※Commit.CheckCommitData(..)  iProperty【{iprop}】(図面番号\"DWG NO\") が設定されていない！！");
                MessageBox.Show(nativeWindow, $"iProperty【{iprop}】(図面番号\"DWG NO\") が設定されていない！！\n" +
                    $"「東陽共通属性:PARTNUMBER」を生成できません\n" +
                    $"", "コミット不可能");
                anser = false;
            }
            if (String.IsNullOrWhiteSpace((string)ticket.GetParamKeyValue("TITLE")))
            {
                var iprop = (string)ticket.GetINViPropertyName("TITLE");
                WriteLine($"※Commit.CheckCommitData(..)  iProperty【{iprop}】(タイトル\"TITLE\") が設定されていない！！");
                MessageBox.Show(nativeWindow, $"iProperty【{iprop}】(タイトル\"TITLE\") が設定されていない！！\n" +
                    $"「東陽共通属性:TITLE」を生成できません\n" +
                    $"", "コミット不可能");
                anser = false;
            }

            var authordate = (string)ticket.GetParamKeyValue("AUTHORDATE");
            if (authordate == "1601/01/01 0:00:00")
            {
                var iprop = (string)ticket.GetINViPropertyName("AUTHORDATE");
                WriteLine($"※Commit.CheckCommitData(..)  iProperty【{iprop}】(製図日時\"DRAWの日付N\"に該当) が設定されていない！！");
                MessageBox.Show(nativeWindow, $"iProperty【{iprop}】(製図日時\"DRAWの日付N\"に該当) が設定されていない！！\n" +
                    $"「東陽共通属性:AUTHORDATE」を生成できません\n" +
                    $"", "コミット不可能");
                anser = false;
            }
            if (String.IsNullOrWhiteSpace((string)ticket.GetParamKeyValue("AUTHOR")))
            {
                var iprop = (string)ticket.GetINViPropertyName("AUTHOR");
                WriteLine($"※Commit.CheckCommitData(..)  iProperty【{iprop}】(製図者名\"DRAWN\"に該当) が設定されていない！！");
                MessageBox.Show(nativeWindow, $"iProperty【{iprop}】(製図者名\"DRAWN\"に該当) が設定されていない！！\n" +
                    $"「東陽共通属性:AUTHOR」を生成できません\n" +
                    $"", "コミット不可能");
                anser = false;
            }

            if (anser)
            {
                /// 妥当性の確認は成功とみなす。
                WriteLine($"■Commit.CheckCommitData(..) チケットデータの妥当性の確認は成功とみなしました。PARTNUMBER = {(string)ticket.GetParamKeyValue("PARTNUMBER")}");
                return anser;
            }
            else
            {
                WriteLine($"※Commit.CheckCommitData(..) チケットデータの妥当性の確認で異常を検知しました。PARTNUMBER = {(string)ticket.GetParamKeyValue("PARTNUMBER")}");

                return false;
            }
        }


        /// <summary>
        /// コミットフォルダへ書き出す最終フェーズ
        /// </summary>
        /// <param name="ticketXml"></param>
        /// <param name="tempBaseFileFullpathWithoutExt"></param>
        /// <returns></returns>
        private bool TicketFinalPhase(CommonTicket ticketXml, string tempBaseFileFullpathWithoutExt)
        {
            string tempTIFFfileFullPath = tempBaseFileFullpathWithoutExt + ".tif";
            string tempTicketFileFullPath = tempBaseFileFullpathWithoutExt + ".xml";

            // コミットするチケットファイル名、TIFFファイル名を生成
            string CommitTICKETfilepath = System.IO.Path.Combine(commitParam.CommitPath, TICKETCODE + ".XML");
            string CommitTIFFfilepath = System.IO.Path.Combine(commitParam.CommitPath, TICKETCODE + ".tif");
            string CommitLOCKfilepath = System.IO.Path.Combine(commitParam.CommitPath, TICKETCODE + ".LCK");

            WriteLine($"Commit.TicketFinalPhase(..)　CommitTICKETfilepath = {CommitTICKETfilepath}");
            WriteLine($"Commit.TicketFinalPhase(..)　CommitTIFFfilepath = {CommitTIFFfilepath}");
            WriteLine($"Commit.TicketFinalPhase(..)　CommitLOCKfilepath = {CommitLOCKfilepath}");

            // XMLシリアライザの準備
            XmlSerializer serializerSave = new XmlSerializer(typeof(CommonTicket));

            //チケットファイルを保存
            using (StreamWriter sw = new StreamWriter(tempTicketFileFullPath, false, Encoding.UTF8))
            {
                serializerSave.Serialize(sw, ticketXml);
                WriteLine($"Commit.TicketFinalPhase(..)　チケットファイルを保存しました {tempTicketFileFullPath}");
            }

            bool finulans = false;

            // コミットフォルダへ書き込み可能かをチェック
            if (FileFolder.Touch(CommitLOCKfilepath))
            {
                WriteLine($"Commit.TicketFinalPhase(..)　コミットフォルダ【{CommitLOCKfilepath}】は書き込み可能です");

                //コミットフォルダへTIFFを書き出し
                bool ans1 = FileFolder.CopyFile(tempTIFFfileFullPath, CommitTIFFfilepath, true);
                if (ans1)
                {
                    WriteLine($"Commit.TicketFinalPhase(..)　コミットフォルダへTIFFを書き出しました.{tempTIFFfileFullPath} => {CommitTIFFfilepath}");
                }
                else
                {
                    WriteLine($"※Commit.TicketFinalPhase(..)　コミットフォルダへTIFF書き出し失敗！！.{tempTIFFfileFullPath} => {CommitTIFFfilepath}");
                }

                //コミットフォルダへチケットファイルを書き出し
                bool ans2 = FileFolder.CopyFile(tempTicketFileFullPath, CommitTICKETfilepath, true);
                if (ans2)
                {
                    WriteLine($"Commit.TicketFinalPhase(..)　チケットファイルを書き出しました.{tempTicketFileFullPath} => {CommitTICKETfilepath}");
                }
                else
                {
                    WriteLine($"※Commit.TicketFinalPhase(..)　チケットファイルの書き出し失敗！！.{tempTicketFileFullPath} => {CommitTICKETfilepath}");
                }
                // 実行結果判定
                if ((ans1 == true) && (ans2 == true))
                    finulans = true;
                else
                    finulans = false;
            }
            else
            {
                FileFolder.RemoveFile(CommitLOCKfilepath);
                WriteLine($"※Commit.TicketFinalPhase(..) {CommitLOCKfilepath}が書き込めません");
                return false;
            }

            // 最終結果判定
            if (finulans == false)
            {
                try
                {
                    FileFolder.RemoveFile(CommitTIFFfilepath);
                    FileFolder.RemoveFile(CommitTICKETfilepath);
                    FileFolder.RemoveFile(CommitLOCKfilepath);
                }
                catch (IOException ioe)
                {
                    WriteLine($"※Commit.TicketFinalPhase(..)コミット失敗しています。最終フェーズで \"{CommitTIFFfilepath}\",\"{CommitTICKETfilepath}\",\"{CommitLOCKfilepath}\" これらのファイルの削除で失敗 {ioe.Message}");
                }

                WriteLine($"※Commit.TicketFinalPhase(..)コミット失敗,{tempTIFFfileFullPath},{tempTicketFileFullPath}");
                return false;
            }


            // 最終結果がOKならお掃除お掃除。
            if (finulans == true)
            {
                // Lockファイル削除
                bool removeFileResult = FileFolder.RemoveFile(CommitLOCKfilepath);
                if (removeFileResult)
                    WriteLine($"■コミット処理後のロックファイル \"{CommitLOCKfilepath}\" の削除が正常終了しました.【{TICKETCODE}】");
                else
                {
                    WriteLine($"※コミット処理後のロックファイル \"{CommitLOCKfilepath}\" の削除に失敗しました.【{TICKETCODE}】");
                }
            }
            else
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// テンポラリファイルの削除
        /// </summary>
        /// <param name="inputTicketFileFullPath"></param>
        /// <param name="inputTIFFfileFullPath"></param>
        /// <param name="ticketXml"></param>
        /// <returns></returns>
        private bool RemoveTemporaryFiles(string tempBaseFileFullpathWithoutExt)
        {
            string tempTIFFfileFullPath = tempBaseFileFullpathWithoutExt + ".tif";
            string tempTicketFileFullPath = tempBaseFileFullpathWithoutExt + ".xml";

            bool ansTiff;
            bool ansTicket;
            ansTiff = FileFolder.RemoveFile(tempTIFFfileFullPath);
            ansTicket = FileFolder.RemoveFile(tempTicketFileFullPath);

            if (ansTiff == false || ansTicket == false)
                return false;
            return true;
        }

        #endregion

    }
}
