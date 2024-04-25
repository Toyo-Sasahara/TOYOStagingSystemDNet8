using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerControlCenterApplication
{
    //public class LogWindow2
    //{
    //    Form1 form;

    //    public LogWindow2(Form1 mainForm)
    //    {
    //        this.form = mainForm;
    //    }

    //    /// <summary>
    //    /// メインスレッド外からの呼び出しも考慮したﾛｸﾞｳｨﾝﾄﾞｳ変更メソッド
    //    /// </summary>
    //    /// <param name="msg"></param>
    //    public void LogWindowWriteLine(string msg)
    //    {
    //        try
    //        {
    //            if (form.InvokeRequired)
    //            {//https://qiita.com/taiyakisun/items/15b57df979eae7562aef
    //                form.Invoke(new Action<string>(this.UpdateText), msg);
    //            }
    //            else
    //            {
    //                UpdateText($"{msg}\n");
    //            }
    //        }
    //        catch (Exception ex)
    //        {
    //            form.ConsoleTextBox1.AppendText($"例外検知{ex.Message}\r\n");

    //        }

    //    }
    //    private void UpdateText(string msg)
    //    {
    //        form.ConsoleTextBox1.AppendText($"{msg}\r\n");
    //        form.ConsoleTextBox2.AppendText($"{msg}\r\n");
    //        form.ConsoleTextBox3.AppendText($"{msg}\r\n");
    //    }

    //}
}
