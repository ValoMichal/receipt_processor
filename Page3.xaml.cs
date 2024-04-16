using System;

namespace Blockovator
{
    public partial class Page3 : System.Windows.Controls.Page
    {
        string appPath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "\\AppData\\Local\\Blockovator";
        public Page3()
        {
            InitializeComponent();
            logDisplay.Text = System.IO.File.ReadAllText(appPath+"\\log.txt");
            scrollViewer.ScrollToBottom();
        }
    }
}
