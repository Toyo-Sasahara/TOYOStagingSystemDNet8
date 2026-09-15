using System;
using System.ComponentModel;
using System.Runtime.Versioning;
using System.Windows.Forms;
using Windows.UI.Xaml.Controls;

namespace SasaLib
{

    [SupportedOSPlatform("windows")]
    public partial class SasaLibBasicPageControl: System.Windows.Forms.UserControl
    {
        public event EventHandler<EventArgs> CurrentPageChanged;

        /// <summary>
        /// チェック状態が変更された場合に発生します
        /// </summary>
        /// <param name="e"></param>
        [Browsable(true)]
        [Description("カレントページが変更された時に発生")]
        protected virtual void OnCurrentPageChanged(EventArgs e)
        {
            EventHandler<EventArgs> eventHandler = CurrentPageChanged;

            if (eventHandler != null)
            {
                eventHandler(this, e);
            }
        }

        public int MaxPage
        {
            get; set; 
        }

        public int CurrentPage { get; set; }


        public SasaLibBasicPageControl()
        {
            InitializeComponent();
        }

        private void XPlus_button_Click(object sender, EventArgs e)
        {
            if (CurrentPage < MaxPage)
                CurrentPage++;
            updatePageNumber();
        }

        private void XMinus_button_Click(object sender, EventArgs e)
        {
            if (CurrentPage > 0)
                CurrentPage--;
            updatePageNumber();
        }

        private void updatePageNumber()
        {
            PageNumberTextBox.Text = $"{CurrentPage} / {MaxPage}";

            // チェック状態が変更されたのでCheckedChangedイベントを発生させる
            OnCurrentPageChanged(EventArgs.Empty);
        }


    }
}
