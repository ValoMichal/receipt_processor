using System;
using System.Windows.Forms;
using System.Windows.Input;

namespace Blockovator
{
    public partial class Page2 : System.Windows.Controls.Page
    {
        string appPath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "\\AppData\\Local\\Blockovator";
        public Page2()
        {
            InitializeComponent();
            qrkody.Text = System.IO.File.ReadAllText(appPath+"\\zlog.txt");
            qrkody.Focus();
        }
        private void saveCode(object sender, System.Windows.RoutedEventArgs e)
        {
            System.IO.File.WriteAllText(appPath+"\\zlog.txt", qrkody.Text);
        }
    }
}
