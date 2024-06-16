using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using ToyoMcMfg.Staging.DataBaseConfig;
using StageServerRemote;
using ServerControlCenterApplication;
using SasaLib;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using System.Reflection;
using SharedClassLibrary;
using System.Text;
using ToyoMcMfg.Staging.RemoteObjects;
using ClientApp.Forms;
using System.Linq;
using System.Runtime.Versioning;

/// <summary>
/// ステージサーバーを検索し結果をListViewに表示。選択した図面をPictureBoxに表示する
/// </summary>
#if NETCOREAPP
[SupportedOSPlatform("windows")]
#endif
public class SearchDrawingSetPictureBox
{

    /// <summary>
    /// 
    /// </summary>
    public SasaLibDelegateWriteLine WriteLine;

    /// <summary>
    /// 
    /// </summary>
    private bool InSearchWork;

    /// <summary>
    /// 
    /// </summary>
    private List<FieldValueSet> DBresultList = new List<FieldValueSet>();// フィールド変数
    /// <summary>
    /// 
    /// </summary>
    private System.Windows.Forms.ListView listview;

    /// <summary>
    /// 
    /// </summary>
    private System.Windows.Forms.Label label;

    /// <summary>
    /// 
    /// </summary>
    private PictureBox pictureBox;

    /// <summary>
    /// 
    /// </summary>
    private PreviewImageForm previewArcSuiteForm;

    private RemoteClientDataBase rMCdataBaseTestmd;

    public bool SearchCance = false;

    /// <summary>
    /// ■コンストラクタ
    /// </summary>
    /// <param name="listview"></param>
    /// <param name="pixturebox"></param>
    /// <param name="WriteLine"></param>
    public SearchDrawingSetPictureBox(System.Windows.Forms.ListView listview, System.Windows.Forms.Label label, PreviewImageForm previewArcSuiteForm, SasaLibDelegateWriteLine WriteLine)
    {
        if (WriteLine == null)
            this.WriteLine = Console.WriteLine;
        else
            this.WriteLine = WriteLine;

        this.listview = listview;
        this.label = label;

        this.previewArcSuiteForm = previewArcSuiteForm;
        setListViewHeadder();
    }

    public void clear()
    {
        listview.Items.Clear();
        if (DBresultList != null)
            DBresultList.Clear();
    }

    /// <summary>
    /// ListViewのヘッダーを設定
    /// </summary>
    private void setListViewHeadder()
    {
        listview.GridLines = true;   // グリッド線を表示
        listview.View = View.Details;  // 詳細ビュー;

        ColumnHeader clmnNumber;
        ColumnHeader clmnID;
        ColumnHeader clmnVARIANT;
        ColumnHeader clmnPARTNUMBER;
        ColumnHeader clmnREV;
        ColumnHeader clmnSANITIZEDPARTNUMBER;
        ColumnHeader clmnTICKETCODE;
        ColumnHeader clmnCOMMITUSERANDHOST;
        ColumnHeader clmnTIMESTAMP;

        ColumnHeader clmnREQUESTPRINTER;
        ColumnHeader clmnPRINTINGTIME;

        ColumnHeader clmnAUTHOR;
        ColumnHeader clmnAUTHORDATE;

        ColumnHeader clmnDESIGNER;
        ColumnHeader clmnCHECKDATE;

        ColumnHeader clmnAPPROVEDUSER;
        ColumnHeader clmnAPPROVEDDATE;
        ColumnHeader clmnAPPROVEDHOST;
        ColumnHeader clmnAPPROVEDPCUSER;

        ColumnHeader clmnREGISTEDTIME;
        ColumnHeader clmnARCSUITEID;
        ColumnHeader clmnCREATESOFTWARE;
        ColumnHeader clmnPARTSNAME;
        ColumnHeader clmnTITLE;
        ColumnHeader clmnDESCRIPTION;

        // 列ヘッダをリストに追加
        listview.Columns.AddRange(new ColumnHeader[] {
                clmnNumber = new ColumnHeader()
                { Text = "No." , Name = "Number", Width = 50 },
                clmnID = new ColumnHeader()
                { Text = "ID" , Name = "ID", Width = 50  },
                clmnVARIANT = new ColumnHeader()
                { Text = "VARIANT" , Name = "VARIANT", Width = 20  },
                clmnPARTNUMBER = new ColumnHeader()
                { Text = "図面番号(PARTNUMBER)", Name = "PARTNUMBER", Width = 80  },
                clmnREV = new ColumnHeader()
                { Text = "図面REV(REV)", Name = "REV", Width = 20  },
                clmnSANITIZEDPARTNUMBER = new ColumnHeader()
                { Text = "図面番号(SANITIZEDPARTNUMBER)", Name = "SANITIZEDPARTNUMBER", Width = 130  },
                clmnTICKETCODE= new ColumnHeader()
                { Text = "ﾁｹｯﾄｺｰﾄﾞ(TICKETCODE)", Name = "TICKETCODE", Width = 160  },
                clmnCOMMITUSERANDHOST = new ColumnHeader()
                { Text = "ｺﾐｯﾄした人(COMMITUSER)", Name = "COMMITUSER", Width = 80  },
                clmnTIMESTAMP = new ColumnHeader()
                { Text = "ｺﾐｯﾄ日時(TIMESTAMP)", Name = "TIMESTAMP", Width = 160  },

                clmnREQUESTPRINTER = new ColumnHeader()
                { Text = "印刷先(REQUESTPRINTER)", Name = "REQUESTPRINTER", Width = 160  },
                clmnPRINTINGTIME = new ColumnHeader()
                { Text = "印刷日時(PRINTINGTIME)", Name = "PRINTINGTIME", Width = 160  },

                clmnAUTHOR = new ColumnHeader()
                { Text = "作図者(AUTHOR)", Name = "AUTHOR", Width = 80  },
                clmnAUTHORDATE = new ColumnHeader()
                { Text = "作図日(AUTHORDATE)", Name = "AUTHORDATE", Width = 160  },

                clmnDESIGNER = new ColumnHeader()
                { Text = "設計者(DESIGNER)", Name = "DESIGNER", Width = 80  },
                clmnCHECKDATE = new ColumnHeader()
                { Text = "設計日(CHECKDATE)", Name = "CHECKDATE", Width = 160  },

                clmnAPPROVEDUSER = new ColumnHeader()
                { Text = "承認者(APPROVEDUSER)", Name = "APPROVEDUSER", Width = 130  },
                clmnAPPROVEDDATE = new ColumnHeader()
                { Text = "承認日時(APPROVEDDATE)", Name = "APPROVEDDATE", Width = 200  },
                clmnAPPROVEDHOST = new ColumnHeader()
                { Text = "承認操作PC(APPROVEDHOST)", Name = "APPROVEDHOST", Width = 100  },
                clmnAPPROVEDPCUSER = new ColumnHeader()
                { Text = "承認操作実施者(APPROVEDPCUSER)", Name = "APPROVEDPCUSER", Width = 130  },


                clmnREGISTEDTIME = new ColumnHeader()
                { Text = "最終承認日時(REGISTEDTIME)", Name = "REGISTEDTIME", Width = 200  },
                clmnARCSUITEID = new ColumnHeader()
                { Text = "ArcSuiteｵﾌﾞｼﾞｪｸﾄID(ARCSUITEID)", Name = "ARCSUITEID" , Width = 200 },
                clmnCREATESOFTWARE = new ColumnHeader()
                { Text = "作成元ｿﾌﾄｳｪｱ(CREATESOFTWARE)", Name = "CREATESOFTWARE", Width = 80  },
                clmnPARTSNAME = new ColumnHeader()
                { Text = "部品名(PARTSNAME)", Name = "PARTSNAME", Width = 200  },
                clmnTITLE = new ColumnHeader()
                {Text = "標題(TITLE)", Name = "TITLE", Width = 200  },
                clmnDESCRIPTION = new ColumnHeader()
                {Text = "説明(DESCRIPTION)", Name = "DESCRIPTION", Width = 200  }
            });

    }


    /// <summary>
    /// リストビューにデータをセットします。
    /// </summary>
    /// <param name="DBresultList"></param>
    private void setDataToListView(List<FieldValueSet> DBresultList)
    {
        int count = DBresultList.Count;
        int gage = 0;

        WriteLine($"DebugListView2にデータをセット開始　{count}件");
        #region  【ListViewにデータをセット】

        var DBresultList_ListCopy = DBresultList.ToList(); // コピーコレクションを作成

        foreach (var result in DBresultList_ListCopy)
        {
            ++gage;

            //WriteLine($"{gage} / {count}");
            SasaLib.DoEvents.Run();

            ListViewItem lvi = new ListViewItem(gage.ToString())
            {
                UseItemStyleForSubItems = false
            };


            lvi.SubItems.Add(result.SearchKey("ID"));
            lvi.SubItems[1].Name = "ID";

            lvi.SubItems.Add(result.SearchKey("VARIANT"));
            lvi.SubItems[2].Name = "VARIANT";
            lvi.SubItems.Add(result.SearchKey("PARTNUMBER"));
            lvi.SubItems[3].Name = "PARTNUMBER";
            lvi.SubItems.Add(result.SearchKey("REV"));
            lvi.SubItems[4].Name = "REV";
            lvi.SubItems.Add(result.SearchKey("SANITIZEDPARTNUMBER"));
            lvi.SubItems[5].Name = "SANITIZEDPARTNUMBER";
            lvi.SubItems.Add(result.SearchKey("TICKETCODE"));
            lvi.SubItems[6].Name = "TICKETCODE";
            lvi.SubItems.Add(result.SearchKey("COMMITUSER") + "@" + result.SearchKey("COMMITHOST"));
            lvi.SubItems[7].Name = "COMMITUSER";
            lvi.SubItems.Add(result.SearchKey("TIMESTAMP"));
            lvi.SubItems[8].Name = "TIMESTAMP";

            lvi.SubItems.Add(result.SearchKey("REQUESTPRINTER"));
            lvi.SubItems[9].Name = "REQUESTPRINTER";
            lvi.SubItems.Add(result.SearchKey("PRINTINGTIME"));
            lvi.SubItems[10].Name = "PRINTINGTIME";

            lvi.SubItems.Add(result.SearchKey("AUTHOR"));
            lvi.SubItems[11].Name = "AUTHOR";
            lvi.SubItems.Add(result.SearchKey("AUTHORDATE"));
            lvi.SubItems[12].Name = "AUTHORDATE";

            lvi.SubItems.Add(result.SearchKey("DESIGNER"));
            lvi.SubItems[13].Name = "DESIGNER";
            lvi.SubItems.Add(result.SearchKey("CHECKDATE"));
            lvi.SubItems[14].Name = "CHECKDATE";


            lvi.SubItems.Add(result.SearchKey("APPROVEDUSER"));
            lvi.SubItems[15].Name = "APPROVEDUSER";

            lvi.SubItems.Add(MyDataTimeMethod.ConvertDateTimeStr(result.SearchKey("APPROVEDDATE")));
            {
                lvi.SubItems.Add(result.SearchKey("APPROVEDHOST"));
                string APPROVEDPCUSER = result.SearchKey("APPROVEDPCUSER");
                if (Environment.UserName == APPROVEDPCUSER)
                {
                    FontStyle fs = lvi.Font.Style | FontStyle.Regular;
                    Font f = new Font(lvi.Font.Name, lvi.Font.Size, fs);
                    lvi.SubItems.Add(APPROVEDPCUSER, Color.Black, Color.White, f);
                }
                else
                {
                    FontStyle fs = lvi.Font.Style | FontStyle.Bold;
                    Font f = new Font(lvi.Font.Name, lvi.Font.Size, fs);
                    lvi.SubItems.Add(APPROVEDPCUSER, Color.Red, Color.White, f);
                }
            }

            lvi.SubItems.Add(MyDataTimeMethod.ConvertDateTimeStr(result.SearchKey("REGISTEDTIME")));
            lvi.SubItems.Add(result.SearchKey("ARCSUITEID"));
            lvi.SubItems.Add(result.SearchKey("CREATESOFTWARE"));
            lvi.SubItems.Add(result.SearchKey("PARTSNAME"));
            lvi.SubItems.Add(result.SearchKey("TITLE"));
            lvi.SubItems.Add(result.SearchKey("DESCRIPTION"));

            listview.Items.Add(lvi);
        }

        #endregion
        WriteLine($"DebugListView2にデータをセット完了　{count}件");
    }


    /// <summary>
    /// ■指定条件で検索開始
    /// </summary>
    /// <param name="sqlSearchStringValues">SQL問い合わせ文</param>
    /// <param name="SearchTitle">検索内容を文字列で設定</param>
    /// <param name="topcount">最大検索数</param>
    /// <param name="ORDDERBY">並べ替えを指定するORDYER BY 文字列</param>
    public async void Search(List<SqlSearchStringValue> sqlSearchStringValues, string SearchTitle, int topcount = 1000, string ORDDERBY = "ORDER BY ID DESC")
    {
        if (InSearchWork == true)
        {
            WriteLine($"以前の検索が終っていません");
            return;
        }
        InSearchWork = true;


        label.Text = SearchTitle;



        SearchMsg(SearchTitle);


        rMCdataBaseTestmd = new RemoteClientDataBase(SccConfig.Config.ClientDomainName, SccConfig.Config.ClientUserName, SccConfig.Config.ClientUserPassword, SccConfig.Config.ClsLogon, SccConfig.Config.StageServerHost, SccConfig.Config.PipeNameDR);

        WriteLine($"{SearchTitle} 検索開始");

        await Task.Run(() =>
        {

            if (SearchCance == false)
            {
                DBresultList = rMCdataBaseTestmd.DataBaseSearch4a(sqlSearchStringValues, topcount, ORDDERBY);

            }
            else
            {
                DBresultList.Clear();

            }

            SearchCance = false;
        });

        if (DBresultList != null && DBresultList.Count > 0)
        {
            WriteLine($"{SearchTitle} 検索完了 {DBresultList.Count}件");
            label.Text = SearchTitle + $" 検索完了 {DBresultList.Count}件";

            WriteLine($"{SearchTitle} リストビューセット開始");
            setDataToListView(DBresultList);
            WriteLine($"{SearchTitle} リストビューセット完了");
        }
        else
        {
            WriteLine($"{SearchTitle} 検索完了 結果なし");

            label.Text = SearchTitle + " 結果なし";

        }
        InSearchWork = false;

    }

    /// <summary>
    /// 検索実行中のメッセージを一定間隔でﾛｸﾞｳｨﾝﾄﾞｳに表示させる
    /// </summary>
    /// <param name="Title"></param>
    private async void SearchMsg(string Title, int spanmsec = 1000)
    {
        int sec = 0;
        while (InSearchWork)
        {
            if (DBresultList != null)
            {
                var count = DBresultList.Count;
                var count2 = listview.Items.Count;

                int count3 = 0;
                if (rMCdataBaseTestmd != null && rMCdataBaseTestmd.DBresultList != null)
                {
                    count3 = rMCdataBaseTestmd.DBresultList.Count;
                }


                WriteLine($"{Title} 検索実行中 {sec}秒経過   DBresultList.Coun={count}件 listview.Items.Count={count2}件");


                sec = sec + spanmsec / 1000;
                await Task.Delay(spanmsec);

            }
            else
            {
                DebugConsole.WriteLine($"※DBresultListがNULLです");
            }


        }
    }

    /// <summary>
    /// ■リストビューが選択された時
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public void GetAndViewDRAWINGimage(System.Drawing.Image WaitImage, PictureBox pictureBox = null)
    {

        if (pictureBox == null)
            pictureBox = this.pictureBox;

        if (listview.SelectedItems.Count == 0)
            return;

        if (listview.SelectedItems.Count == 1)
        {
            RemoteClientDRAWREGIST remoteClient = new RemoteClientDRAWREGIST(SccConfig.Config.ClientDomainName,
                SccConfig.Config.ClientUserName,
                SccConfig.Config.ClientUserPassword,
                SccConfig.Config.ClsLogon,
                SccConfig.Config.StageServerHost,
                SccConfig.Config.PipeNameDR);

            int listViewSelectedLineNo = listview.SelectedIndices[0];

            if (DBresultList.Count > listViewSelectedLineNo)
            {

                FieldValueSet selectedFieldValueSet = (FieldValueSet)DBresultList[listViewSelectedLineNo];

                string TICKETCODE = selectedFieldValueSet.SearchKey("TICKETCODE");


                previewArcSuiteForm.SetImage(WaitImage);
                var task = Task.Run(() =>
                {
                    //var resultImage = remoteClient.GetImageFromPIPE_Type2(selectedFieldValueSet.SearchKey("GUIDBASE64"), binaryConvertTYPE);
                    BinaryConvertTYPE binaryConvertTYPE = BinaryConvertTYPE.IFormatter;
                    //BinaryConvertTYPE binaryConvertTYPE = BinaryConvertTYPE.IFormatter;
                    var resultImage = remoteClient.GetImageFromPIPE_Type2(selectedFieldValueSet.SearchKey("GUIDBASE64"), WriteLine, binaryConvertTYPE);

                    if (resultImage != null)
                    {
                        WriteLine($"イメージを受信");

                        System.Drawing.Bitmap bitmap = new Bitmap(resultImage);

                        try
                        {
                            previewArcSuiteForm.SetImage(bitmap);

                        }
                        catch (Exception ex)
                        {
                            WriteLine($"{ex.Message}");
                        }
                    }
                    else
                    {
                        WriteLine($"イメージを受信できませんでした");
                    }

                });

                WriteLine($"イメージ受信を待っています");

                Clipboard.SetText(TICKETCODE);

                WriteLine($"チケットコード {TICKETCODE}");
            }

        }
        else
        {
            var task = Task.Run(() =>
            {
                pictureBox.Image = null;
                previewArcSuiteForm.SetImage(null);
            });

        }


    }

    public string GetSelectedTicketCode()
    {
        if (listview.SelectedItems.Count == 0)
            return null;

        if (listview.SelectedItems.Count == 1)
        {
            RemoteClientDRAWREGIST remoteClient = new RemoteClientDRAWREGIST(SccConfig.Config.ClientDomainName,
                SccConfig.Config.ClientUserName,
                SccConfig.Config.ClientUserPassword,
                SccConfig.Config.ClsLogon,
                SccConfig.Config.StageServerHost,
                SccConfig.Config.PipeNameDR);

            FieldValueSet selectedFieldValueSet = (FieldValueSet)DBresultList[listview.SelectedIndices[0]];

            string TICKETCODE = selectedFieldValueSet.SearchKey("TICKETCODE");


            WriteLine($"{TICKETCODE}");

            return TICKETCODE;
        }
        else
        {
            return null;
        }

    }

    public string GetSelectedData(string FIELDNAME)
    {
        if (listview.SelectedItems.Count == 0)
            return null;

        if (listview.SelectedItems.Count == 1)
        {
            RemoteClientDRAWREGIST remoteClient = new RemoteClientDRAWREGIST(SccConfig.Config.ClientDomainName,
                SccConfig.Config.ClientUserName,
                SccConfig.Config.ClientUserPassword,
                SccConfig.Config.ClsLogon,
                SccConfig.Config.StageServerHost,
                SccConfig.Config.PipeNameDR);
            int listViewSelectedLineNo = listview.SelectedIndices[0];
            if (DBresultList.Count > listViewSelectedLineNo)
            {
                FieldValueSet selectedFieldValueSet = (FieldValueSet)DBresultList[listViewSelectedLineNo];

                string value = selectedFieldValueSet.SearchKey(FIELDNAME);


                WriteLine($"{FIELDNAME} = {value}");

                return value;
            }
            else
                return null;
        }
        else
        {
            return null;
        }

    }

    public void DeleteSelectedDBresultListRecordOne()
    {
        var selected = listview.SelectedIndices[0];

        FieldValueSet fieldValueSet = (FieldValueSet)DBresultList[selected];

        string ticketcode = fieldValueSet.SearchKey("TICKETCODE");
        string GUIDBASE64 = fieldValueSet.SearchKey("GUIDBASE64");

        RemoteClientMaintenance rMmaintenance = new RemoteClientMaintenance(SccConfig.Config.ClientDomainName, SccConfig.Config.ClientUserName, SccConfig.Config.ClientUserPassword, SccConfig.Config.ClsLogon, SccConfig.Config.StageServerHost, SccConfig.Config.PipeNameDR);

        var ans = MessageBox.Show($"削除します。続行しますか？\r\n{ticketcode}", "", MessageBoxButtons.YesNo);
        if (ans == DialogResult.Yes)
        {
            var ans2 = MessageBox.Show($"ほんとうに続行しますか？\r\n{ticketcode}", "", MessageBoxButtons.YesNo);
            if (ans2 == DialogResult.Yes)
            {
                var deleteAns = rMmaintenance.RecordAndEntityfileDelete(GUIDBASE64);
                MessageBox.Show($"結果{deleteAns}");

                if (deleteAns)
                    listview.Items.RemoveAt(selected);

            }
        }

    }

    public void Delete_SelectedDBresultListRecords()
    {
        RemoteClientMaintenance rMmaintenance = new RemoteClientMaintenance(SccConfig.Config.ClientDomainName, SccConfig.Config.ClientUserName, SccConfig.Config.ClientUserPassword, SccConfig.Config.ClsLogon, SccConfig.Config.StageServerHost, SccConfig.Config.PipeNameDR);


        foreach (int index in listview.SelectedIndices)
        {
            FieldValueSet fieldValueSet = (FieldValueSet)DBresultList[index];
            string ticketcode = fieldValueSet.SearchKey("TICKETCODE");
            string GUIDBASE64 = fieldValueSet.SearchKey("GUIDBASE64");
            string PARTNUMBER = fieldValueSet.SearchKey("PARTNUMBER");
            var deleteAns = rMmaintenance.RecordAndEntityfileDelete(GUIDBASE64);

            if (deleteAns)
            {
                WriteLine($"削除対象：{PARTNUMBER} {ticketcode} ({GUIDBASE64}) 成功");

                //listview.Items.Remove(listview.Items[index]); 失敗。要再検討
            }
            else
                WriteLine($"削除対象：{PARTNUMBER} 失敗");
        }
    }

    internal void ApprovdReset_SelectedDBresultListRecordOne()
    {
        var selected = listview.SelectedIndices[0];

        FieldValueSet fieldValueSet = (FieldValueSet)DBresultList[selected];

        string ticketcode = fieldValueSet.SearchKey("TICKETCODE");
        string GUIDBASE64 = fieldValueSet.SearchKey("GUIDBASE64");

        RemoteClientMaintenance rMmaintenance = new RemoteClientMaintenance(SccConfig.Config.ClientDomainName, SccConfig.Config.ClientUserName, SccConfig.Config.ClientUserPassword, SccConfig.Config.ClsLogon, SccConfig.Config.StageServerHost, SccConfig.Config.PipeNameDR);

        var ans = MessageBox.Show($"承認情報をリセットします。続行しますか？\r\n{ticketcode}", "", MessageBoxButtons.YesNo);
        if (ans == DialogResult.Yes)
        {
            var ans2 = MessageBox.Show($"ほんとうに続行しますか？\r\n{ticketcode}", "", MessageBoxButtons.YesNo);
            if (ans2 == DialogResult.Yes)
            {
                var deleteAns = rMmaintenance.ApprovedCancel2(ticketcode);
                MessageBox.Show($"結果{deleteAns}");

            }
        }


    }

    internal void FinalApprovdReset_SelectedDBresultListRecordOne()
    {
        var selected = listview.SelectedIndices[0];

        FieldValueSet fieldValueSet = (FieldValueSet)DBresultList[selected];

        string ticketcode = fieldValueSet.SearchKey("TICKETCODE");
        string GUIDBASE64 = fieldValueSet.SearchKey("GUIDBASE64");

        RemoteClientMaintenance rMmaintenance = new RemoteClientMaintenance(SccConfig.Config.ClientDomainName, SccConfig.Config.ClientUserName, SccConfig.Config.ClientUserPassword, SccConfig.Config.ClsLogon, SccConfig.Config.StageServerHost, SccConfig.Config.PipeNameDR);

        var ans = MessageBox.Show($"最終承認情報のみをリセットします。続行しますか？\r\n{ticketcode}", "", MessageBoxButtons.YesNo);
        if (ans == DialogResult.Yes)
        {
            var ans2 = MessageBox.Show($"ほんとうに続行しますか？\r\n{ticketcode}", "", MessageBoxButtons.YesNo);
            if (ans2 == DialogResult.Yes)
            {

                Command_MAINCOMMAND.IsTICKETCODEexist(ticketcode, out fieldValueSet, WriteLine);

                // 削除リスト
                List<ApprovedCancel> CancelList = new List<ApprovedCancel>()
                {
                    new ApprovedCancel()
                    {
                        FieldValueSet = fieldValueSet,
                        AUTHOR = false,
                        DESIGNER = false,
                        APPROVED = true,
                        APPROVEDPCUSER = true
                    }
                };


                // キャンセル指示発動
                RemoteClientDataBase rMdataBase = new RemoteClientDataBase(SccConfig.Config.ClientDomainName,
                    SccConfig.Config.ClientUserName,
                    SccConfig.Config.ClientUserPassword,
                    SccConfig.Config.ClsLogon,
                    SccConfig.Config.StageServerHost,
                    SccConfig.Config.PipeNameDR);

                StringBuilder sb = new StringBuilder();
                foreach (var item in CancelList)
                {
                    var TICKETCODE = item.FieldValueSet.SearchKey("TICKETCODE");
                    var PARTNUMBER = item.FieldValueSet.SearchKey("PARTNUMBER");
                    sb.AppendLine($"{TICKETCODE}{PARTNUMBER}");
                }

                var CancelErrorGUIDBASE64List = new List<string>();
                bool result = rMdataBase.ApprovedCancels3(CancelList, out CancelErrorGUIDBASE64List);
                if (result == true)
                {
                    GlovalValues.Mylog.WriteLine($"次の押印キャンセルが成功しています\n{sb.ToString()}");

                    MessageBox.Show($"次の押印キャンセルが成功しています\n{sb.ToString()}");
                }
                else
                {
                    GlovalValues.Mylog.WriteLine($"次の押印キャンセルはいずれかまたはすべて失敗しました\n{sb.ToString()}");
                    MessageBox.Show($"次の押印キャンセルはいずれかまたはすべて失敗しました\n{sb.ToString()}");
                }


            }
        }


    }

    internal void ApprovdReset_SelectedDBresultListRecords()
    {
        RemoteClientMaintenance rMmaintenance = new RemoteClientMaintenance(SccConfig.Config.ClientDomainName, SccConfig.Config.ClientUserName, SccConfig.Config.ClientUserPassword, SccConfig.Config.ClsLogon, SccConfig.Config.StageServerHost, SccConfig.Config.PipeNameDR);

        foreach (int index in listview.SelectedIndices)
        {
            FieldValueSet fieldValueSet = (FieldValueSet)DBresultList[index];
            string ticketcode = fieldValueSet.SearchKey("TICKETCODE");
            string GUIDBASE64 = fieldValueSet.SearchKey("GUIDBASE64");
            string PARTNUMBER = fieldValueSet.SearchKey("PARTNUMBER");

            var deleteAns = rMmaintenance.ApprovedCancel2(ticketcode);

            if (deleteAns)
                WriteLine($"承認情報削除対象：{PARTNUMBER} 成功");
            else
                WriteLine($"承認情報削除対象：{PARTNUMBER} 失敗");

        }
    }
}

