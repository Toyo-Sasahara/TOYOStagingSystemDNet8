//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace ServerControlCenterApplication
//{
//    public  class Account
//    {
//        public object control { get; private set; }

//        public Account(Object control)
//        {
//            this.control = control;
//        }

//        /// <summary>
//        /// 
//        /// </summary>

//        /// <summary>
//        /// ■コントロール上でアカウント情報が変更された時にキックされる
//        /// </summary>
//        /// <param name="sender"></param>
//        /// <param name="e"></param>
//        //bool flag=false;
//        //public void ControlChanged(object sender, EventArgs e)
//        //{
//        //    if (flag)
//        //        return;
//        //    else
//        //        flag = true;
            
//        //    SccConfig.Config.ClsLogon = ((Form1)control).ClientImpersonationCheckBox.Checked;// 1
//        //    SccConfig.Config.ClientDomainName = ((Form1)control).LogonDomainTextBox.Text;    // 2
//        //    SccConfig.Config.ClientUserName = ((Form1)control).LogonUserTextBox.Text;        // 3
//        //    SccConfig.Config.ClientUserPassword = ((Form1)control).LogonPasswordTextBox.Text;// 4

//        //    SccConfig.Config.StageServerHost = ((Form1)control).StageServerHostName_comboBox.Text;    // 5
//        //    SccConfig.Config.PipeNameDR = ((Form1)control).DrawcapturePIPEnameTextBox.Text;  // 6
//        //    SccConfig.Config.PipeNameDC = ((Form1)control).DrawregistPIPEnameTextBox.Text;   // 7

//        //    SccConfig.Config.CommitPath = @"\\" + ((Form1)control).StageServerHostName_comboBox.Text + @"\" + ((Form1)control).CommitShareNameTextBox.Text; // ⑧
//        //    //form.CommitPathTextBox.Text = SccConfig.Config.CommitPath;

//        //    //form.LogWindow.LogWindowWriteLine($"\r\nアカウント情報更新されました {SccConfig.Config.StageServerHost}");
           

//        //    //control.ReadMMPFAndSetInTheFormContorols();

//        //    //form.LogWindow.LogWindowWriteLine($"メモリマップドファイル情報を更新されました {SccConfig.Config.StageServerHost}");


//        //    //form.LogWindow.LogWindowWriteLine($"プリンタ情報が更新されました {SccConfig.Config.StageServerHost}");

//        //    flag = false;
//        //}
//    }
//}
