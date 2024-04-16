using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;

namespace Blockovator
{
    public partial class Page1 : System.Windows.Controls.Page
    {
        static string appPath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "\\AppData\\Local\\Blockovator";
        class Config
        {
            public string? vybrany { get; set; }
            public string? des { get; set; }
            public string? output { get; set; }
        }
        Config config = System.Text.Json.JsonSerializer.Deserialize<Config>(System.IO.File.ReadAllText(appPath+"\\config.json"));
        public Page1()
        {
            InitializeComponent();
            System.Windows.Controls.Button button0 = new System.Windows.Controls.Button();
            System.Windows.Controls.Button button1 = new System.Windows.Controls.Button();
            System.Windows.Controls.Button button2 = new System.Windows.Controls.Button();
            System.Windows.Controls.Button button3 = new System.Windows.Controls.Button();
            System.Windows.Controls.Button button4 = new System.Windows.Controls.Button();
            System.Windows.Controls.Button button5 = new System.Windows.Controls.Button();
            System.Windows.Controls.Button button6 = new System.Windows.Controls.Button();
            System.Windows.Controls.Button button7 = new System.Windows.Controls.Button();
            System.Windows.Controls.Button button8 = new System.Windows.Controls.Button();
            System.Windows.Controls.Button button9 = new System.Windows.Controls.Button();
            int modulesCount = System.IO.Directory.GetFiles(".\\modules").Length;
            button.Content = System.IO.Path.GetFileNameWithoutExtension(config.vybrany).Replace("_", " ");
            Description.Text = System.IO.File.ReadLines(".\\modules\\" + System.IO.Path.GetFileName(config.vybrany)).First().Replace("<!--","").Replace("-->","");
            if(modulesCount > 0)
            {
                button0.Click += (sender, EventArgs) => { ChooseModule(sender, EventArgs, System.IO.Directory.GetFiles(".\\modules")[0]); };
                button0.Content = System.IO.Path.GetFileNameWithoutExtension(System.IO.Directory.GetFiles(".\\modules")[0]).Replace("_", " ");
                Modules.Children.Add(button0);
            }
            if (modulesCount>1)
            {
                button1.Click += (sender, EventArgs) => { ChooseModule(sender, EventArgs, System.IO.Directory.GetFiles(".\\modules")[1]); };
                button1.Content = System.IO.Path.GetFileNameWithoutExtension(System.IO.Directory.GetFiles(".\\modules")[1]).Replace("_", " ");
                Modules.Children.Add(button1);
            }
            if (modulesCount>2)
            {
                button2.Click += (sender, EventArgs) => { ChooseModule(sender, EventArgs, System.IO.Directory.GetFiles(".\\modules")[2]); };
                button2.Content = System.IO.Path.GetFileNameWithoutExtension(System.IO.Directory.GetFiles(".\\modules")[2]).Replace("_", " ");
                Modules.Children.Add(button2);
            }
            if (modulesCount>3)
            {
                button3.Click += (sender, EventArgs) => { ChooseModule(sender, EventArgs, System.IO.Directory.GetFiles(".\\modules")[3]); };
                button3.Content = System.IO.Path.GetFileNameWithoutExtension(System.IO.Directory.GetFiles(".\\modules")[3]).Replace("_", " ");
                Modules.Children.Add(button3);
            }
            if (modulesCount>4)
            {
                button4.Click += (sender, EventArgs) => { ChooseModule(sender, EventArgs, System.IO.Directory.GetFiles(".\\modules")[4]); };
                button4.Content = System.IO.Path.GetFileNameWithoutExtension(System.IO.Directory.GetFiles(".\\modules")[4]).Replace("_", " ");
                Modules.Children.Add(button4);
            }
            if (modulesCount>5)
            {
                button5.Click += (sender, EventArgs) => { ChooseModule(sender, EventArgs, System.IO.Directory.GetFiles(".\\modules")[5]); };
                button5.Content = System.IO.Path.GetFileNameWithoutExtension(System.IO.Directory.GetFiles(".\\modules")[5]).Replace("_", " ");
                Modules.Children.Add(button5);
            }
            if (modulesCount>6)
            {
                button6.Click += (sender, EventArgs) => { ChooseModule(sender, EventArgs, System.IO.Directory.GetFiles(".\\modules")[6]); };
                button6.Content = System.IO.Path.GetFileNameWithoutExtension(System.IO.Directory.GetFiles(".\\modules")[6]).Replace("_", " ");
                Modules.Children.Add(button6);
            }
            if (modulesCount>7)
            {
                button7.Click += (sender, EventArgs) => { ChooseModule(sender, EventArgs, System.IO.Directory.GetFiles(".\\modules")[7]); };
                button7.Content = System.IO.Path.GetFileNameWithoutExtension(System.IO.Directory.GetFiles(".\\modules")[7]).Replace("_", " ");
                Modules.Children.Add(button7);
            }
            if (modulesCount>8)
            {
                button8.Click += (sender, EventArgs) => { ChooseModule(sender, EventArgs, System.IO.Directory.GetFiles(".\\modules")[8]); };
                button8.Content = System.IO.Path.GetFileNameWithoutExtension(System.IO.Directory.GetFiles(".\\modules")[8]).Replace("_", " ");
                Modules.Children.Add(button8);
            }
            if (modulesCount>9)
            {
                button9.Click += (sender, EventArgs) => { ChooseModule(sender, EventArgs, System.IO.Directory.GetFiles(".\\modules")[9]); };
                button9.Content = System.IO.Path.GetFileNameWithoutExtension(System.IO.Directory.GetFiles(".\\modules")[9]).Replace("_", " ");
                Modules.Children.Add(button9);
            }
            if (modulesCount>10)
            {
                zoznam.Visibility = Visibility.Visible;
                foreach(string modul in System.IO.Directory.GetFiles(".\\modules"))
                {
                    zoznam.Items.Add(System.IO.Path.GetFileNameWithoutExtension(modul).Replace("_"," "));
                }
            }
        }
        private void BtnClickWindow(object sender, RoutedEventArgs e)
        {
            Window1 win = new Window1();
            win.Show();
        }
        private async void ChooseModule(object sender, RoutedEventArgs e, string chosenModule)
        {
            button.Content = System.IO.Path.GetFileNameWithoutExtension(chosenModule);
            config.vybrany = chosenModule;
            System.IO.File.WriteAllText(appPath+"\\config.json", System.Text.Json.JsonSerializer.Serialize<Config>(config));
            Description.Text = System.IO.File.ReadLines(".\\modules\\" + System.IO.Path.GetFileName(chosenModule)).First().Replace("<!--", "").Replace("-->", "");
        }
        private void zoznamChanged(object sender, RoutedEventArgs e)
        {
            string vybranyModul = ((string?)zoznam.SelectedItem).Replace(" ", "_");
            button.Content = vybranyModul;
            config.vybrany = ".\\modules\\"+vybranyModul+".xslt";
            System.IO.File.WriteAllText(appPath + "\\config.json", System.Text.Json.JsonSerializer.Serialize<Config>(config));
            Description.Text = System.IO.File.ReadLines(".\\modules\\" + System.IO.Path.GetFileName(vybranyModul)+".xslt").First().Replace("<!--", "").Replace("-->", "");

        }
    }
}
