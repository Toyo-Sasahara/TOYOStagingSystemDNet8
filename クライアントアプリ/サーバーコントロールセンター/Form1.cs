using System;
using System.Diagnostics;
using System.Windows.Forms;
#if NETCOREAPP
using System.Runtime.Versioning;
#endif


namespace ServerControlCenterApplication
{
#if NETCOREAPP
    [SupportedOSPlatform("windows")]
#endif
    public partial class Form1 : Form
    {

        static readonly string AssemblyInternalName = FileVersionInfo.GetVersionInfo(System.Reflection.Assembly.GetExecutingAssembly().Location).InternalName;

        public LogWindow LogWindow;

        // アプリケーション全体で使用するプリンタ情報を保持
        internal CommitPrinters CommitPrinters = new CommitPrinters();


        //SeachTest sh;

        TabControl01 tabControl01;
        TabControl02 tabControl02;
        TabControl03 tabControl03;
        TabControl04 tabControl04;
        TabControl05 tabControl05;
        TabControl06 tabControl06;
        TabControl07 tabControl07;

        // プリンタ情報取得
        //internal CommitPrinters CommitPrinters = new CommitPrinters();

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public Form1()
        {
            InitializeComponent();


            tabControl01 = new TabControl01(this);
            AddTabPages("ｺﾐｯﾄ処理ﾃｽﾄ", tabControl01);

            tabControl02 = new TabControl02(this);
            AddTabPages("ﾁｹｯﾄ処理ﾃｽﾄ(1/2)", tabControl02);

            tabControl03 = new TabControl03(this);
            AddTabPages("ﾁｹｯﾄ処理ﾃｽﾄ(2/2)", tabControl03);

            tabControl04 = new TabControl04(this);
            AddTabPages("ｻｰﾋﾞｽ動作ﾓｰﾄﾞ", tabControl04);

            tabControl05 = new TabControl05(this);
            AddTabPages("ｻｰﾋﾞｽ詳細ﾁｪｯｸ", tabControl05);

            tabControl06 = new TabControl06(this);
            AddTabPages("ｱｰｸｽｲｰﾄ制御ﾃｽﾄ", tabControl06);

            tabControl07 = new TabControl07(this);
            AddTabPages("ﾌｧｲﾙ送受信ﾃｽﾄ", tabControl07);

            LogWindow = new LogWindow(this);

        }


        /// <summary>
        /// ■タブコントロールを追加します
        /// </summary>
        /// <param name="tabname">名前</param>
        /// <param name="userControlTab">ユーザーコントロールからの派生を指定</param>
        private void AddTabPages(string tabname, UserControl userControlTab)
        {

            // タブコントロールにタブページを追加
            var tabPage = new System.Windows.Forms.TabPage(tabname);
            tabControl.Controls.Add(tabPage);

            // タブページにページの内容（UserControl派生）を追加
            tabPage.Controls.Add(userControlTab);

            // タブページのサイズに合わせて広げたい場合はこの設定
            userControlTab.Dock = System.Windows.Forms.DockStyle.Fill;

        }


        private void Form1_Load(object sender, EventArgs e)
        {

        }


        private void tabControl_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

    }
}
