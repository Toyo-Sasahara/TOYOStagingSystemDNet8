//using SasaLib;
//using SasaLib.NumberingSupport;
//using SasaLib.PrintConfig;
//using SasaLibDummy;
//using StageServerRemote;
//using System;
//using System.Collections.Generic;
//using System.Drawing;
//using System.Drawing.Imaging;
//using System.IO;
//using System.Linq;
//using System.Runtime.CompilerServices;
//using System.Text;
//using System.Threading.Tasks;
//using System.Windows.Forms;
//using System.Windows.Forms.VisualStyles;
//using System.Xml.Serialization;
//using ToyoMcMfg.Staging.DataBaseConfig;
/////
//namespace ServerControlCenterApplication
//{
//    public delegate void delegate_SetCommontikcetParams(ref List<CommonTicket.Param> param, string KEY,string VALUE);

//    public static class CommitTest
//    {

//        /// <summary>
//        /// 【コミットコマンド・パートA
//        /// イメージとチケットファイル作成メソッド・スタート
//        /// </summary>
//        /// <param name="CommitFolder"></param>
//        /// <param name="TemplateTiffFilePath"></param>
//        /// <param name="PARTNUMBER"></param>
//        /// <param name="RequestPrinterStr"></param>
//        /// <returns></returns>
//        public static bool CreateTicketAndTiffImage(string CommitFolder, string TemplateTiffFilePath, out string TICKETCODE, List<CommonTicket.Param> prms, List<CommonTicket.Param> variantparams, string ImgePositonConfig, string RequestPrinterStr = "", bool PrintOutOnly = false, SasaLibDelegateWriteLine delegateWriteLine = null)
//        {
//            using (new ClsLogonDummy(SccConfig.Config.ClientDomainName, SccConfig.Config.ClientUserName, SccConfig.Config.ClientUserPassword, SccConfig.Config.ClsLogon))
//            {
//                string WorkingTempForder = Path.GetTempPath() + Path.GetRandomFileName();

//                // ② GUIDを更新
//                StaticCommonVars.guid = new GUIDExtensions(true);
//                // チケットコードのベースファイル名はGUIDをBase64エンコードし'/'を'_'としたものです。
//                StaticCommonVars.TICKETCODE = StaticCommonVars.guid.B64FnameString;
//                TICKETCODE = StaticCommonVars.TICKETCODE;
//                delegateWriteLine($"チケットコードを取得 {StaticCommonVars.TICKETCODE}");

//                // ④ チケットコードのテンプレートファイル名を生成
//                string template_Ticket_File = SccConfigWork.GetAppConfigFolder() + System.IO.Path.DirectorySeparatorChar + @"TEMPLATE_TICKET.XML";

//                // ⑤チケットテンプレートの作成
//                CommonTicketWork.MakeTicketTemplate(template_Ticket_File);

//                // ⑥コミットフォルダがあるかチェック
//                if (FileFolder.DirExists(CommitFolder) != true)
//                {
//                    //MessageBox.Show($"CreateTicketAndTiffImageコミット先フォルダ [{CommitFolder}]に接続できません。");
//                    delegateWriteLine($"コミット先フォルダ [{CommitFolder}]に接続できません。");

//                    return false;
//                }

//                // ⑦ワーキングフォルダ設定、有るなしにかかわらず作成
//                FileFolder.MakeDirectory(WorkingTempForder);

//                // ⑧GUIDstr を取得
//                string GUIDstr = StaticCommonVars.guid.ToString();

//                // チケットコードからテンポラリフォルダに仮保存するチケットファイル名を生成
//                string WorknigTicketFilePath = WorkingTempForder + System.IO.Path.DirectorySeparatorChar + StaticCommonVars.TICKETCODE + @".XML";

//                // チケットコードからテンポラリフォルダに仮保存するTIFFファイル名を生成
//                string WorkingTiffFilePath = WorkingTempForder + System.IO.Path.DirectorySeparatorChar + StaticCommonVars.TICKETCODE + @".TIF";

//                // パーツ番号からネイティブCADファイル名を偽造
//                string NativeCADFilePath = WorkingTempForder + System.IO.Path.DirectorySeparatorChar +  CommitTest.GetCommonTicketParam(prms,"PARTNUMBER") + @".SLDPRT";

//                string PARTNUMBER = CommitTest.GetCommonTicketParam(prms, "PARTNUMBER");
//                // ⑨チケットコードファイル名を生成。フルパス
//                string LastExportTicketFilePath = CommitFolder + System.IO.Path.DirectorySeparatorChar + StaticCommonVars.TICKETCODE + @".XML";
//                string LastExportTiffFilePath = CommitFolder + System.IO.Path.DirectorySeparatorChar + StaticCommonVars.TICKETCODE + @".TIF";

//                string LastExportLockFilePath = CommitFolder + System.IO.Path.DirectorySeparatorChar + StaticCommonVars.TICKETCODE + @".LCK";

//                // 外部ソフトウェアによるTIFFファイル作成をシミュレ－ト
//                bool sucessMakeTIFFimage = OrderToCADsaveTiffImage(NativeCADFilePath, TemplateTiffFilePath, WorkingTiffFilePath);

//                // TIFFの保存に成功した場合
//                if (sucessMakeTIFFimage == true)
//                {
//                    delegateWriteLine($"★TIFFの保存に成功。CreateWorkingTiffFilePath={WorkingTiffFilePath}");

//                    //設定読込
//                    ImagePositonConfigWork.ReadImagePositionConfig(ImgePositonConfig);

//                    /// イメージをオブジェクトへ確保（ファイルをロックしない）
//                    StaticCommonVars.currentImage = SasaLib.ImageUtil.CreateImageFromFile(WorkingTiffFilePath);
//                    var size = ImageUtil.GetPaperSizeMillimeter(StaticCommonVars.currentImage);


//                    /// イメージから用紙サイズと向きを推察
//                    StaticCommonVars.currentPaperSize = PaperCheck.GetJISpaperSize(size.Width, size.Height, 5);
//                    Point p = ImagePositonConfig.Config.GetOffset(StaticCommonVars.currentPaperSize);
//                    StaticCommonVars.Offset = p;

//                    ///大きさと向きを表示
//                    Console.WriteLine($"★イメージの大きさと向き：{StaticCommonVars.currentPaperSize.ToString()}, オフセット ({p.X},{p.Y})");

//                    // イメージのオフセット移動開始。
//                    StaticCommonVars.currentImage = ImageUtil.Move1bppImageMilli(StaticCommonVars.currentImage, p.X, p.Y);

//                    //StaticCommonVars.debugForm.TestImageOffsetValueLabel.Text = $"{StaticCommonVars.currentPaperSize} X={p.X},Y={p.Y}";

//                    #region 図面文字書き込み
//                    ImageUtil.DrawTextImageFromRightButtom(StaticCommonVars.currentImage, PARTNUMBER, 95, 27, 0, 0, 30);

//                    string AUTHORDATE = CommitTest.GetCommonTicketParam(prms, "AUTHORDATE");
//                    ImageUtil.DrawTextImageFromRightButtom(StaticCommonVars.currentImage, AUTHORDATE, 128, 25, 0, 0, 10);

//                    string AUTHOR = CommitTest.GetCommonTicketParam(prms, "AUTHOR");
//                    ImageUtil.DrawTextImageFromRightButtom(StaticCommonVars.currentImage, AUTHOR, 128, 16, 0, 0, 10);

//                    #endregion
//                    // イメージのフォーマット変更
//                    StaticCommonVars.currentImage = ImageUtil.ChangePixelFormat((Bitmap)StaticCommonVars.currentImage, PixelFormat.Format1bppIndexed);

//                    // メモリストリームを用意し、TIFF CCITT4圧縮のストリームデータを保存
//                    MemoryStream tiffStream = new MemoryStream();
//                    ImageUtil.ImageToTIFF1bppCCITT4Stream(StaticCommonVars.currentImage, tiffStream);
//                    // TIFFメモリストリームをファイルに保存
//                    StreamExtensions.StreamToFile(tiffStream, WorkingTiffFilePath);

//                    tiffStream.Dispose();

//                    System.GC.Collect(); // アクセス不可能なオブジェクトを除去
//                    System.GC.WaitForPendingFinalizers(); // ファイナライゼーションが終わるまでスレッド待機
//                    System.GC.Collect(); // ファイナライズされたばかりのオブジェクトに関連するメモリを開放


//                    delegateWriteLine($"★Tiffイメージ {WorkingTiffFilePath} に関する処理は終了・続いてチケットファイルの作成開始");

//                    delegateWriteLine($"★チケットファイル作成開始 GUIDstr:{GUIDstr} NativeCADFilePath:{NativeCADFilePath} WorkingTiffFilePath:{WorkingTiffFilePath} StaticCommonVars.currentPaperSize:{StaticCommonVars.currentPaperSize} template_Ticket_File:{template_Ticket_File} WorknigTicketFilePath:{WorknigTicketFilePath} RequestPrinterStr:{RequestPrinterStr}");

//                    // ⑩　チケットファイルを作成保存する
//                    //チケット設定テンプレート, GUID文字列, Ticket名, 希望印刷先
//                    CommonTicket ans = GetAttrAndTicketSave(template_Ticket_File: template_Ticket_File, GUIDstr: GUIDstr,
//                        NativeCADFilePath: NativeCADFilePath, currentPaperSize: StaticCommonVars.currentPaperSize,
//                        CreateSoftware: "SOLIDWORKS", comticketparams: ref prms, ref variantparams,
//                        WorkingExportTicketFilePath: WorknigTicketFilePath,
//                        RequestPrinter: RequestPrinterStr, PrintingTimeStr: "", 
//                        PrintOutOnly: PrintOutOnly, WriteLine: delegateWriteLine);

//                    if (ans != null)
//                    {
//                        delegateWriteLine($"★正常終了処理");

//                        // ロックファイル生成
//                        SasaLib.FileFolder.Touch(LastExportLockFilePath);

//                        // TIFFファイルをステージサーバーのコミットフォルダへ移動
//                        SasaLib.FileFolder.MoveFile(WorkingTiffFilePath, LastExportTiffFilePath);

//                        // チケットファイルをステージサーバーのコミットフォルダへ移動
//                        SasaLib.FileFolder.MoveFile(WorknigTicketFilePath, LastExportTicketFilePath);

//                        // ロックファイル削除
//                        SasaLib.FileFolder.RemoveFile(LastExportLockFilePath);

//                        // ワーキングフォルダ―削除
//                        SasaLib.FileFolder.RemoveFolder(WorkingTempForder);

//                        return true;
//                    }
//                    else
//                    {
//                        delegateWriteLine($"★ﾁｹｯﾄｺｰﾄﾞ作成失敗");
//                        MessageBox.Show("★チケットコード作成に失敗");
//                        return false;
//                    }
//                }
//                else
//                {
//                    MessageBox.Show("★TIFF保存に失敗");
//                    return false;
//                }
//            }
//        }

//        static void ReEditParams1(ref List<CommonTicket.Param> addParams, string PARTNUMBER)
//        {
//            addParams = new List<CommonTicket.Param>() {
//                //
//                new CommonTicket.Param{
//                    Key = "PARTNUMBER",
//                    Value = PARTNUMBER,
//                },
//                new CommonTicket.Param{
//                    Key = "AUTHOR_STAMP_POS_X",
//                    Value = "120",
//                },
//                new CommonTicket.Param{
//                    Key = "AUTHOR_STAMP_POS_Y",
//                    Value = "0",
//                },
//                new CommonTicket.Param{
//                    Key = "AUTHOR_STAMP_SCALE",
//                    Value = "1.7",
//                },
//                new CommonTicket.Param{
//                    Key = "CHECKED_STAMP_POS_X",
//                    Value = "130",
//                },
//                new CommonTicket.Param{
//                    Key = "CHECKED_STAMP_POS_Y",
//                    Value = "0",
//                },
//                new CommonTicket.Param{
//                    Key = "CHECKED_STAMP_SCALE",
//                    Value = "1.7",
//                },
//                new CommonTicket.Param{
//                    Key = "APPROVED_STAMP_POS_X",
//                    Value = "140",
//                },
//                new CommonTicket.Param{
//                    Key = "APPROVED_STAMP_POS_Y",
//                    Value = "0",
//                },
//                new CommonTicket.Param{
//                    Key = "APPROVED_STAMP_SCALE",
//                    Value = "1.7",
//                },
//            };
//        }

//        public static void UpdateComonTicketParam(ref List<CommonTicket.Param> cticketParam, string KEY, string VALUE)
//        {
//            CommonTicket.Param param = new CommonTicket.Param() {Key=KEY,Value=VALUE };
//            if (cticketParam.Where(p => p.Key == KEY).Count() > 0)
//                cticketParam.RemoveAll(x => x.Key == KEY);
//            cticketParam.Add(param);               
//        }

//        public static string GetCommonTicketParam(List<CommonTicket.Param> cticketParam, string KEY)
//        {
//            IEnumerable<CommonTicket.Param>  param = cticketParam.Where(p => p.Key == KEY);
//            return param.First().Value;
//        }

//        /// <summary>
//        /// 
//        /// </summary>
//        /// <param name="PARTNUMBER"></param>
//        /// <returns></returns>
//        public static string CheckDrawingType(string PARTNUMBER, SasaLibDelegateWriteLine delegateWriteLine = null)
//        {
//            var hostname = SccConfig.Config.StageServerHost;

//            RemoteClientDRAWCAPTURE stageserver = new RemoteClientDRAWCAPTURE(SccConfig.Config.ClientDomainName, SccConfig.Config.ClientUserName, SccConfig.Config.ClientUserPassword, SccConfig.Config.ClsLogon, SccConfig.Config.StageServerHost, SccConfig.Config.PipeNameDC);
//            var DrawingType = stageserver.CHECK_DRAWING_TYPE(hostname, PARTNUMBER);

//            delegateWriteLine($"接続先[{SccConfig.Config.StageServerHost}],パイプ名:[{SccConfig.Config.PipeNameDC}], 調査した図番:{PARTNUMBER} 結果：{DrawingType}");

//            return DrawingType;
//        }

//        /// <summary>
//        /// チケットコードが実在するか
//        /// </summary>
//        /// <param name="TICKETCODE"></param>
//        /// <returns></returns>
//        public static bool IsTICKETCODEexist(string TICKETCODE, out FieldValueSet result, SasaLibDelegateWriteLine WriteLine = null)
//        {
//            SqlFieldValue sqlStr = new SqlFieldValue()
//            {
//                Field = "TICKETCODE",
//                Value = TICKETCODE,
//                SqlDBType = System.Data.SqlDbType.NVarChar
//            };

//            RemoteClientDataBase rMdataBase = new RemoteClientDataBase(SccConfig.Config.ClientDomainName, SccConfig.Config.ClientUserName, SccConfig.Config.ClientUserPassword,
//                SccConfig.Config.ClsLogon, SccConfig.Config.StageServerHost, SccConfig.Config.PipeNameDR);

//            var ans = rMdataBase.DataBaseSearch3(sqlStr);

//            if (ans.Count == 1)
//            {
//                result = ans[0];
//                return true;
//            }
//            else
//            {
//                WriteLine($"検索結果 {ans.Count} 件");
//                result = null;
//                return false;
//            }

//        }


//        //public static void ApprovedMainProcessDebug(string TKCKETCODE, string UserID, SasaLibDelegateWriteLine delegateWriteLine = null)
//        //{
//        //    RemoteClientMaintenance rMmaintenance = new RemoteClientMaintenance(SccConfig.Config.ClientDomainName, SccConfig.Config.ClientUserName, SccConfig.Config.ClientUserPassword, SccConfig.Config.ClsLogon, SccConfig.Config.StageServerHost, SccConfig.Config.PipeNameDR);
//        //    //手動押印テスト
//        //    var ans = rMmaintenance.ApprovedMainProcessDebug(TKCKETCODE, UserID);

//        //    if (ans)
//        //    {

//        //        delegateWriteLine($" rMmaintenance.ApprovedMainProcessDebug({TKCKETCODE}, {UserID}) は成功したようです");
//        //    }
//        //    else
//        //    {
//        //        delegateWriteLine($" rMmaintenance.ApprovedMainProcessDebug({TKCKETCODE}, {UserID}) は失敗しました");

//        //    }
//        //}

//        /// <summary>
//        /// チケットファイルデータ生成＆作成
//        /// チケットテンプレートに指示済みの値は取り込む
//        /// </summary>
//        /// <param name="template_Ticket_File">元になるチケットファイルテンプレート</param>
//        /// <param name="GUIDstr">GUIDチケットコード</param>
//        /// <param name="currentPaperSize">用紙サイズ</param>
//        /// <param name="ExportTiffFilePath">元になった</param>
//        /// <param name="exportTIKECTFilePath">作成するチケットファイル</param>
//        /// <param name="RequestPrinter"></param>
//        /// <param name="PrintingTimeStr"></param>
//        /// <param name="delegateWriteLine"></param>
//        /// <returns></returns>
//        //private static CommonTicket GetAttrAndTicketSave(string GUIDstr, string NativeCADFilePath,
//        //   string CreateWorkingTiffFilePath, CommonPaperSize currentPaperSize,
//        //   string template_Ticket_File, string WorkingExportTicketFilePath,
//        //   string RequestPrinter = "", string PrintingTimeStr = "", bool PrintOutOnly = false, SasaLibDelegateWriteLine delegateWriteLine = null)
//        //{
//        //    ///
//        //    string NativeActiveDocFileNameWithoutExtention = SasaLib.FileFolder.GetFileNameWithoutExtension(NativeCADFilePath);

//        //    // ①Ticketの設定ファイルのデシリアライズ準備
//        //    XmlSerializer serializer = new XmlSerializer(typeof(CommonTicket));
//        //    System.IO.StreamReader sr = new System.IO.StreamReader(template_Ticket_File, new System.Text.UTF8Encoding(false));
//        //    // ②共通属性フォーマットのインスタンスを宣言
//        //    CommonTicket ticket;
//        //    // ③オブジェクトをメモリへ書き戻す
//        //    ticket = (CommonTicket)serializer.Deserialize(sr);
//        //    sr.Close();
//        //    // ④タイムスタンプをセット
//        //    ticket.TIMESTAMP = DateTime.Now;
//        //    // ⑤ チケットコードをセット
//        //    ticket.TICKETCODE = StaticCommonVars.TICKETCODE;
//        //    // ⑥ 
//        //    ticket.REQUESTPRINTER = RequestPrinter;
//        //    // PrintingTimeStr が"" の時は現在時刻とする
//        //    if (PrintingTimeStr == "") { ticket.PRINTINGTIME = DateTime.Now; } else { ticket.PRINTINGTIME = DateTime.Parse(PrintingTimeStr); }
//        //    // ⑦
//        //    ticket.GUID = new Guid(GUIDstr); // 関数へ渡されたGUID文字列からGUIDオブジェクト再生成
//        //    // ⑧
//        //    ticket.GUIDBASE64 = new GUIDExtensions(GUIDstr).B64String; //BASE64にエンコード
//        //    // ⑨
//        //    ticket.CREATESOFTWARE = "SOLIDWORKS";


//        //    ticket.COMMITHOST = Net.GetHOSTNAME();
//        //    ticket.COMMITUSER = Net.GetLONGONNAME();

//        //    ticket.PLOTPAPERSIZE = currentPaperSize.ToString();
//        //    // Solidworksからページ設定のサイズを取得するのはあきらめた。
//        //    ticket.PAPERSIZE = ticket.PLOTPAPERSIZE;

//        //    Console.WriteLine("■コミット送信元・ユーザー名:" + ticket.COMMITHOST + @"\" + ticket.COMMITUSER + "\n");

//        //    // CommonTicket ticket をカスタムプロパティから情報を得て修正する
//        //    bool ans = ModifyAttrObject(ticket, NativeActiveDocFileNameWithoutExtention);

//        //    AddTicketParams(ref ticket.Params);

//        //    ticket.PrintOutOnly = PrintOutOnly;

//        //    if (ans)
//        //    {
//        //        XmlSerializer serializerSave = new XmlSerializer(typeof(CommonTicket));

//        //        //チケットファイルを正規場所へ保存
//        //        using (StreamWriter sw = new StreamWriter(WorkingExportTicketFilePath, false, Encoding.UTF8))
//        //        {
//        //            serializerSave.Serialize(sw, ticket);
//        //        }
//        //    }
//        //    else
//        //    {
//        //        MessageBox.Show("□エラー：図面番号や表題などの属性を読み取れません.処理を中止します\n");
//        //        return null;
//        //    }
//        //    return ticket;
//        //}

//        private static CommonTicket GetAttrAndTicketSave(string template_Ticket_File, string GUIDstr,
//            string NativeCADFilePath, CommonPaperSize currentPaperSize, string CreateSoftware,
//            ref List<CommonTicket.Param> comticketparams, ref List<CommonTicket.Param> variantparams,
//             string WorkingExportTicketFilePath, string RequestPrinter = "", string PrintingTimeStr = "", bool PrintOutOnly = false, SasaLibDelegateWriteLine WriteLine = null)
//        {
//            ///
//            string NativeActiveDocFileNameWithoutExtention = SasaLib.FileFolder.GetFileNameWithoutExtension(NativeCADFilePath);

//            // ①Ticketの設定ファイルのデシリアライズ準備
//            XmlSerializer serializer = new XmlSerializer(typeof(CommonTicket));
//            System.IO.StreamReader sr = new System.IO.StreamReader(template_Ticket_File, new System.Text.UTF8Encoding(false));
//            // ②共通属性フォーマットのインスタンスを宣言
//            CommonTicket ticket;
//            // ③オブジェクトをメモリへ書き戻す
//            ticket = (CommonTicket)serializer.Deserialize(sr);
//            sr.Close();
//            // ④タイムスタンプをセット
//            ticket.TIMESTAMP = DateTime.Now;
//            // ⑤ チケットコードをセット
//            ticket.TICKETCODE = StaticCommonVars.TICKETCODE;
//            // ⑥ 
//            ticket.REQUESTPRINTER = RequestPrinter;
//            // PrintingTimeStr が"" の時は現在時刻とする
//            if (PrintingTimeStr == "") { ticket.PRINTINGTIME = DateTime.Now; } else { ticket.PRINTINGTIME = DateTime.Parse(PrintingTimeStr); }
//            // ⑦
//            ticket.GUID = new Guid(GUIDstr); // 関数へ渡されたGUID文字列からGUIDオブジェクト再生成
//            // ⑧
//            ticket.GUIDBASE64 = new GUIDExtensions(GUIDstr).B64String; //BASE64にエンコード
//            // ⑨
//            ticket.CREATESOFTWARE = CreateSoftware;

//            ticket.COMMITHOST = Net.GetHOSTNAME();
//            ticket.COMMITUSER = Net.GetLONGONNAME();

//            ticket.PLOTPAPERSIZE = currentPaperSize.ToString();
//            // Solidworksからページ設定のサイズを取得するのはあきらめた。
//            ticket.PAPERSIZE = ticket.PLOTPAPERSIZE;

//            Console.WriteLine("■コミット送信元・ユーザー名:" + ticket.COMMITHOST + @"\" + ticket.COMMITUSER + "\n");

//            ticket.DOCUMENTNAME = NativeCADFilePath;
           
//            ticket.PrintOutOnly = PrintOutOnly;

//            ticket.Params = comticketparams;

//            ticket.Variant = variantparams;

//            XmlSerializer serializerSave = new XmlSerializer(typeof(CommonTicket));

//            //チケットファイルを正規場所へ保存
//            using (StreamWriter sw = new StreamWriter(WorkingExportTicketFilePath, false, Encoding.UTF8))
//            {
//                serializerSave.Serialize(sw, ticket);
//            }

//            return ticket;
//        }




//        //public static void AddTicketParams(ref List<CommonTicket.Param> orgParams)
//        //{
//        //    List<CommonTicket.Param> addParams = new List<CommonTicket.Param>() {
//        //        //
//        //        new CommonTicket.Param{
//        //            Key = "AUTHOR_STAMP_POS_X",
//        //            Value = "120",
//        //        },
//        //        new CommonTicket.Param{
//        //            Key = "AUTHOR_STAMP_POS_Y",
//        //            Value = "0",
//        //        },
//        //        new CommonTicket.Param{
//        //            Key = "AUTHOR_STAMP_SCALE",
//        //            Value = "1.7",
//        //        },
//        //        new CommonTicket.Param{
//        //            Key = "CHECKED_STAMP_POS_X",
//        //            Value = "130",
//        //        },
//        //        new CommonTicket.Param{
//        //            Key = "CHECKED_STAMP_POS_Y",
//        //            Value = "0",
//        //        },
//        //        new CommonTicket.Param{
//        //            Key = "CHECKED_STAMP_SCALE",
//        //            Value = "1.7",
//        //        },
//        //        new CommonTicket.Param{
//        //            Key = "APPROVED_STAMP_POS_X",
//        //            Value = "140",
//        //        },
//        //        new CommonTicket.Param{
//        //            Key = "APPROVED_STAMP_POS_Y",
//        //            Value = "0",
//        //        },
//        //        new CommonTicket.Param{
//        //            Key = "APPROVED_STAMP_SCALE",
//        //            Value = "1.7",
//        //        },
//        //    };

//        //    orgParams.AddRange(addParams);
//        //}

//        /// <summary>
//        /// 外部アプリにドキュメントからTiffイメージを作成・指定名で保存させるダミーメソッド
//        /// </summary>
//        /// <param name="NaitiveDocumentFullPath">印刷させるＣＡＤファイル名</param>
//        /// <param name="TEMPLATETIFFFILE">代わりに生成させる用紙別TIFFテンプレートイメージ</param>
//        /// <param name="ExportTiffFullPath">生成させる完成TIFF図面イメージ</param>
//        /// <returns></returns>
//        private static bool OrderToCADsaveTiffImage(string NaitiveDocumentFullPath, string TEMPLATETIFFFILE, string ExportTiffFullPath, SasaLibDelegateWriteLine delegateWriteLine = null)
//        {
//            StaticCommonVars.currentImage = null;
//            System.Drawing.Image org = SasaLib.ImageUtil.GetCCITT4ImageFromFile(TEMPLATETIFFFILE, true);

//            bool ans = ImageUtil.SaveImageToFile(org, "image/tiff", EncoderValue.CompressionCCITT4, ExportTiffFullPath);
//            ///
//            org.Dispose();

//            Console.WriteLine($"偽装した{NaitiveDocumentFullPath}について、");
//            Console.WriteLine($"テンプレートファイル {TEMPLATETIFFFILE} を使用し、");
//            Console.WriteLine($"ファイル名 {ExportTiffFullPath} として書き出しました");

//            return ans;
//        }

//        /// <summary>
//        /// パートAAA
//        /// SolidWorksからカスタムプロパティを取得し、CommonTicket ticket を修正する
//        /// </summary>
//        /// <param name="ticket"></param>
//        /// <returns></returns>
//        //private static bool ModifyAttrObject(CommonTicket ticket, string ActiveDocFile, SasaLibDelegateWriteLine delegateWriteLine = null)
//        //{
//        //    try
//        //    {
//        //        // 
//        //        string DocumentName = default;
//        //        //
//        //        string[] drawPropNames = default;
//        //        string[] drawPropValues = default;
//        //        //
//        //        string[] modelPropNames = default;
//        //        string[] modelPropValues = default;
//        //        //
//        //        CommonPaperSize PaperSize = default;

//        //        // ダミープロパティ生成
//        //        PropertyGetter propertyGetter = new PropertyGetter();
//        //        var ans = propertyGetter.GetCADFileDummyProperty(ref DocumentName, ref PaperSize, ref drawPropNames, ref drawPropValues, ref modelPropNames, ref modelPropValues);
//        //        StaticCommonVars.activeDocName = DocumentName;
//        //        ticket.DOCUMENTNAME = DocumentName;

//        //        // drawPropNames を調査するループ
//        //        Console.WriteLine($"図面側からプロパティリストを{drawPropNames}へ取得");
//        //        Console.WriteLine($"図面側からプロパティの内容を{drawPropValues}へ取得");

//        //        for (int i = 0; i < ((string[])drawPropNames).Length; i++)
//        //        {
//        //            ////    属性名と内容を渡し、attrObjを更新する
//        //            ////     drawPropNames[i] = プロパティ名, drawPropValues[i] = プロパティ名が示す内容
//        //            ///

//        //            Console.WriteLine($"調査対象：図面側プロパティ{drawPropNames[i]}");
//        //            Console.WriteLine($"調査対象：図面側プロパティ{drawPropValues[i]}");

//        //            /// drawPropValues[i] の示す値がNULLやホワイトスペースではない場合
//        //            if (String.IsNullOrWhiteSpace(drawPropValues[i]) != true)
//        //            {
//        //                Console.WriteLine($"図面側プロパティ値：{drawPropNames[i]}");
//        //                Console.WriteLine($"図面側プロパティ値：{drawPropValues[i]}");

//        //                ticket.SetSWprop(drawPropNames[i], drawPropValues[i]);
//        //                Console.WriteLine($"SetSWprop({drawPropNames[i]}, {drawPropNames[i]})図面側から取得");

//        //            }
//        //            /// drawPropValues[i] の示す値がNULLやホワイトスペースの場合、モデル側からプロパティを探す
//        //            else
//        //            {
//        //                Console.WriteLine($"図面側プロパティ{drawPropNames[i]}が空のためモデル側から探します");
//        //                Console.WriteLine($"図面側プロパティ{drawPropValues[i]}が空のためモデル側から探します");
//        //                int ii = Array.IndexOf(modelPropNames, drawPropNames[i]);
//        //                /// モデル側のプロパティにて見つかった場合 そちらを採用する
//        //                if (ii != -1)
//        //                {
//        //                    ticket.SetSWprop(drawPropNames[i], modelPropValues[ii]);
//        //                    Console.WriteLine($"SetSWprop({drawPropNames[i]}, {modelPropValues[ii]})モデル側から取得");
//        //                    Console.WriteLine($"SetSWprop({drawPropNames[i]}, {modelPropValues[ii]})モデル側から取得");

//        //                }
//        //                else
//        //                {
//        //                    ticket.SetSWprop(drawPropNames[i], "");
//        //                    Console.WriteLine($"SetSWprop({drawPropNames[i]}, 図面側・モデル側とも見つからない");
//        //                    Console.WriteLine($"SetSWprop({drawPropNames[i]}, \"\")実行type3");

//        //                }

//        //            }
//        //        }
//        //        ticket.SetSWprop("Number", ActiveDocFile);

//        //        Console.WriteLine($"PARTNUMBER = {(string)ticket.GetParamKeyValue("PARTNUMBER")}");

//        //        // NumberがNull Or White Spaceの場合SLDDRWファイル名から推察する
//        //        if (String.IsNullOrWhiteSpace((string)ticket.GetParamKeyValue("PARTNUMBER")))
//        //        {
//        //            string SlddrwaFileName = SasaLib.FileFolder.GetFileNameWithoutExtension(ticket.DOCUMENTNAME);
//        //            var result = MessageBox.Show($"図面枠から図面番号情報を取りこめません.\n" +
//        //                $"図面ファイル名から推測すると {SlddrwaFileName} です.\n" +
//        //                $"正しいですか？");
//        //            if (result == DialogResult.OK)
//        //            {
//        //                ticket.SetSWprop("Number", SlddrwaFileName);
//        //            }
//        //            else
//        //            {
//        //                MessageBox.Show($"この図面ファイルは処理できません。\n" +
//        //                    $"正しい図面番号がソリッドワークス図面の\n" +
//        //                    $"カスタムプロパティ【Number】に反映されるように修正してください。");
//        //                return false;
//        //            }
//        //        }
//        //        else
//        //        {
//        //            if ((string)ticket.GetParamKeyValue("PARTNUMBER") != ActiveDocFile)
//        //            {
//        //                MessageBox.Show($"TIFFイメージと、図面番号が一致しません。\n" +
//        //                    $"正しい図面番号がソリッドワークス図面の\n" +
//        //                    $"カスタムプロパティ【Number】に反映されるように修正してください。");
//        //                return false;

//        //            }
//        //            else
//        //            {
//        //                Console.WriteLine($"TIFF作成時のActiveDocFile{ActiveDocFile}とGetParamKeyValue(\"PARTNUMBER\")との値が一致しました");

//        //            }

//        //        }
//        //        return true;
//        //    }
//        //    catch (Exception ex)
//        //    {
//        //        Console.WriteLine($"{ex.ToString()}");
//        //        Console.WriteLine($"エラー：ModifyAttrObject()で例外発生 {ex.Message} ");
//        //        return false;
//        //    }
//        //}

//    }

//    /// <summary>
//    /// テストのためCADからプロパティ取得するメソッド。
//    /// </summary>
//    class PropertyGetter
//    {
//        /// <summary>
//        /// テストのためCADからプロパティ取得するメソッド。
//        /// </summary>
//        /// <param name="DocumentName">対象とするドキュメント(SLDDRW)</param>
//        /// <param name="PaperSize">解析された用紙サイズ</param>
//        /// <param name="DrawPropNames">図面側プロパティ名の配列</param>
//        /// <param name="DrawPropValues">図面側プロパティ名配列の対応する値の配列</param>
//        /// <param name="ModelPropNames">モデル側プロパティ名の配列</param>
//        /// <param name="ModelPropValues"></param>
//        public bool GetCADFileDummyProperty(ref string DocumentName, ref CommonPaperSize PaperSize, ref string[] DrawPropNames, ref string[] DrawPropValues, ref string[] ModelPropNames, ref string[] ModelPropValues)
//        {
//            bool ans = false;

//            DocumentName = @"C:\ABC\DEF\XX-12345-001.DOC";

//            PaperSize = CommonPaperSize.A3L;



//            //プロパティ名と値をストリング配列にする

//            string[] property = {
//                "Number",
//                "Description",
//                "顧客",
//                "作図者",
//                "作図日",
//                //"設計者",
//                //"設計日",

//            };
//            string[] value = {
//                "XX-12345-001",
//                "テスト用ダミー図面",
//                "ささは製薬",
//                "笹原裕貴",
//                "2100/01/01",
//                //"笹田裕貴",
//                //"2120/01/01",
//            };

//            DrawPropNames = property;
//            DrawPropValues = value;
//            ModelPropNames = property;
//            ModelPropValues = value;

//            ans = true;

//            return ans;
//        }

//    }

//}
