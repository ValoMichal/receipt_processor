using System;

namespace Blockovator
{
    public partial class Window1 : System.Windows.Window
    {
        string appPath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "\\AppData\\Local\\Blockovator";
        class Config
        {
            public string? vybrany { get; set; }
            public string? des { get; set; }
            public string? output { get; set; }
        }
        public Window1()
        {
            InitializeComponent();
        }
        private void pridatModul(object sender, EventArgs e)
        {
            Microsoft.WindowsAPICodePack.Dialogs.CommonOpenFileDialog dialog = new Microsoft.WindowsAPICodePack.Dialogs.CommonOpenFileDialog();
            dialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile)+"\\Downloads"; 
            dialog.IsFolderPicker = false;
            if (dialog.ShowDialog() == Microsoft.WindowsAPICodePack.Dialogs.CommonFileDialogResult.Ok)
            {
                System.IO.Compression.ZipFile.ExtractToDirectory(dialog.FileName, ".\\modules", true);
            }
        }
        private void zmenitProstredie(object sender, EventArgs e)
        {
            Config config = System.Text.Json.JsonSerializer.Deserialize<Config>(System.IO.File.ReadAllText(appPath + "\\config.json"));
            Microsoft.WindowsAPICodePack.Dialogs.CommonOpenFileDialog dialog = new Microsoft.WindowsAPICodePack.Dialogs.CommonOpenFileDialog();
            dialog.InitialDirectory = config.des;
            dialog.IsFolderPicker = true;
            if (dialog.ShowDialog() == Microsoft.WindowsAPICodePack.Dialogs.CommonFileDialogResult.Ok)
            {
                config.des=dialog.FileName;
                System.IO.File.WriteAllText(appPath+"\\config.json",System.Text.Json.JsonSerializer.Serialize<Config>(config));
            }
        }
    }
}
