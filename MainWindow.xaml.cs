using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.Linq;
using System.Windows.Forms;

namespace Blockovator
{
    class Config
    {
        public string? vybrany { get; set; }
        public string? des { get; set; }
        public string? output { get; set; }
    }
    public partial class MainWindow : System.Windows.Window
    {
        string appPath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "\\AppData\\Local\\Blockovator";
        public MainWindow()
        {
            System.IO.Directory.CreateDirectory(appPath);
            System.IO.Directory.CreateDirectory(".\\modules");
            System.IO.File.WriteAllText(appPath+"\\log.txt", "");
            System.IO.File.WriteAllText(appPath+"\\ylog.txt", "");
            System.IO.File.WriteAllText(appPath+"\\zlog.txt", "");
            System.IO.File.WriteAllText(appPath+"\\tmp.xml", "");
            if (System.IO.File.Exists(appPath+"\\config.json") == false)
            {
                string defConf= "{\"vybrany\":\""+ System.IO.Directory.GetFiles(".\\modules", "*.xslt")[0] + "\",\"des\":\"" + Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "\\Desktop\",\"output\":\"" + Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "\\Desktop\"}";
                System.IO.File.WriteAllText(appPath + "\\config.json", defConf.Replace("\\", "/"));
            }
            InitializeComponent();
            Main.Content = new Page1();
            Config config = System.Text.Json.JsonSerializer.Deserialize<Config>(System.IO.File.ReadAllText(appPath + "\\config.json"));
            Button.Content = "Zmenit vystup: " + config.output;
            Main.Background = System.Windows.Media.Brushes.LightGray;
        }
        private void BtnClickPage1(object sender, System.Windows.RoutedEventArgs e)
        {
            Main.Content = new Page1();
            Button1.Background = System.Windows.Media.Brushes.Green;
            Button2.Background = System.Windows.Media.Brushes.Gray;
            Button3.Background = System.Windows.Media.Brushes.Gray;
            Button1.Content = "Modul";
            Button2.Content = "Sken";
            Button3.Content = "Spracovat";
        }
        private void BtnClickPage2(object sender, System.Windows.RoutedEventArgs e)
        {
            Config config = System.Text.Json.JsonSerializer.Deserialize<Config>(System.IO.File.ReadAllText(appPath+"\\config.json"));Main.Content = new Page2();
            Button1.Background = System.Windows.Media.Brushes.Gray;
            Button2.Background = System.Windows.Media.Brushes.Green;
            Button3.Background = System.Windows.Media.Brushes.Gray;
            Button1.Content = System.IO.Path.GetFileNameWithoutExtension(config.vybrany).Replace("_", " ");
            Button2.Content = "Sken";
            Button3.Content = "Spracovat";
            Main.Focus();
        }
        private async void BtnClickPage3(object sender, System.Windows.RoutedEventArgs e)
        {
            Config config = System.Text.Json.JsonSerializer.Deserialize<Config>(System.IO.File.ReadAllText(appPath+"\\config.json"));            
            System.IO.File.AppendAllText(appPath+"\\ylog.txt", System.IO.File.ReadAllText(appPath+"\\zlog.txt")+"\n\n\n");
            await ProcessCodes();
            Main.Content = new Page3();
            Button1.Background = System.Windows.Media.Brushes.Gray;
            Button2.Background = System.Windows.Media.Brushes.Gray;
            Button3.Background = System.Windows.Media.Brushes.Green;
            Button1.Content = System.IO.Path.GetFileNameWithoutExtension(config.vybrany).Replace("_", " ");
            Button2.Content = "Sken";
            Button3.Content = "Spracovat";
        }
        async System.Threading.Tasks.Task ProcessCodes()
        {
            Config config = System.Text.Json.JsonSerializer.Deserialize<Config>(System.IO.File.ReadAllText(appPath+"\\config.json"));
            System.Collections.Generic.List<String> qrcodes = new System.Collections.Generic.List<String>();
            qrcodes.Add(config.vybrany);
            foreach (string qrcode in System.IO.File.ReadAllLines(appPath+"\\zlog.txt").Distinct().ToList())
            {
                qrcodes.Add(qrcode);
            }
            System.IO.File.WriteAllText(appPath+"\\zlog.txt", "");
            await ReceiptProcessor(qrcodes);
        }
        private void path(object sender, EventArgs e)
        {
            Config config = System.Text.Json.JsonSerializer.Deserialize<Config>(System.IO.File.ReadAllText(appPath + "\\config.json"));
            CommonOpenFileDialog dialog = new CommonOpenFileDialog();
            dialog.InitialDirectory = config.des;
            dialog.IsFolderPicker = true;
            if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                config.output = dialog.FileName;
                System.IO.File.WriteAllText(appPath + "\\config.json", System.Text.Json.JsonSerializer.Serialize<Config>(config));
                Button.Content = "Zmenit vystup: "+dialog.FileName;
            }
        }
        static async System.Threading.Tasks.Task ReceiptProcessor(System.Collections.Generic.List<String> args)
        {
        //here would be file generation, function calling and other stuff [25 lines]
        }
        public static class Post
        {
            public static async System.Threading.Tasks.Task<string> QrCode(string input)
            {
            //here would be json fetch from api [5 lines]
            }
        }
        //here would be document class [75 lines]
        public static class Converter
        {
            public static string JsonToXml(string json, string currentCode)
            {
            //here would be json to xml conversion [240 lines]
            }
        }
    }
}
