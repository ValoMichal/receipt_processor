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
            string appPath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "\\AppData\\Local\\Blockovator";
            System.Globalization.CultureInfo customCulture = (System.Globalization.CultureInfo)System.Threading.Thread.CurrentThread.CurrentCulture.Clone();
            customCulture.NumberFormat.NumberDecimalSeparator = ".";
            System.Threading.Thread.CurrentThread.CurrentCulture = customCulture;
            string xml = "<receipts>";
            for (int i = 1; i < args.Count; i++)
            {
                System.IO.File.AppendAllText(appPath + "\\log.txt", args[i]);
                xml += Converter.JsonToXml(await Post.QrCode(args[i]), args[i]);
            }
            xml += "</receipts>";
            Console.Write(xml);
            Config config = System.Text.Json.JsonSerializer.Deserialize<Config>(System.IO.File.ReadAllText(appPath + "\\config.json"));string outputPath = config.output;
            string fileName = System.IO.Path.GetFileNameWithoutExtension(config.vybrany) + System.DateTime.Now.ToString("yyyyMMddhhmmss");
            xml = xml.Replace("&", "&amp;");
            await System.IO.File.WriteAllTextAsync(appPath + "\\tmp.xml", xml);
            System.Xml.XPath.XPathDocument XmlFile = new System.Xml.XPath.XPathDocument(appPath + "\\tmp.xml");
            System.Xml.Xsl.XslTransform XsltFile = new System.Xml.Xsl.XslTransform();
            XsltFile.Load(args[0]);
            using var FinalFile = new System.Xml.XmlTextWriter(outputPath + "\\" + fileName + ".xml", null);
            XsltFile.Transform(XmlFile, null, FinalFile);
            Console.Write(outputPath + "\\" + fileName + ".xml");
            System.IO.File.AppendAllText(appPath + "\\log.txt", outputPath + "\\" + fileName + ".xml");
        }
        public static class Post
        {
            public static async System.Threading.Tasks.Task<string> QrCode(string input)
            {
                System.Net.Http.HttpClient client = new System.Net.Http.HttpClient();
                var json = "{\"receiptId\":\"" + input + "\"}";
                var content = new System.Net.Http.StringContent(json, System.Text.Encoding.UTF8, "application/json");
                var response = await client.PostAsync("https://ekasa.financnasprava.sk/mdu/api/v1/opd/receipt/find", content);
                var responseString = await response.Content.ReadAsStringAsync();
                return responseString;
            }
        }
        public class Item
        {
            public string? name { get; set; }
            public string? itemType { get; set; }
            public float? quantity { get; set; }
            public float? vatRate { get; set; }
            public float? price { get; set; }
        }

        public class Organization
        {
            public string? buildingNumber { get; set; }
            public string? country { get; set; }
            public string? dic { get; set; }
            public string? icDph { get; set; }
            public string? ico { get; set; }
            public string? municipality { get; set; }
            public string? name { get; set; }
            public string? postalCode { get; set; }
            public object? propertyRegistrationNumber { get; set; }
            public string? streetName { get; set; }
            public bool? vatPayer { get; set; }
        }

        public class Unit
        {
            public string? cashRegisterCode { get; set; }
            public object? buildingNumber { get; set; }
            public string? country { get; set; }
            public string? municipality { get; set; }
            public string? postalCode { get; set; }
            public string? propertyRegistrationNumber { get; set; }
            public string? streetName { get; set; }
            public object? name { get; set; }
            public string? unitType { get; set; }
        }

        public class Receipt
        {
            public string? receiptId { get; set; }
            public string? ico { get; set; }
            public string? cashRegisterCode { get; set; }
            public string? issueDate { get; set; }
            public string? createDate { get; set; }
            public object? customerId { get; set; }
            public string? dic { get; set; }
            public string? icDph { get; set; }
            public object? invoiceNumber { get; set; }
            public string? okp { get; set; }
            public bool? paragon { get; set; }
            public object? paragonNumber { get; set; }
            public string? pkp { get; set; }
            public int? receiptNumber { get; set; }
            public string? type { get; set; }
            public float? taxBaseBasic { get; set; }
            public float? taxBaseReduced { get; set; }
            public float? totalPrice { get; set; }
            public float? freeTaxAmount { get; set; }
            public float? vatAmountBasic { get; set; }
            public float? vatAmountReduced { get; set; }
            public float? vatRateBasic { get; set; }
            public float? vatRateReduced { get; set; }
            public System.Collections.Generic.List<Item>? items { get; set; }
            public Organization? organization { get; set; }
            public Unit? unit { get; set; }
            public bool? exemption { get; set; }
        }

        public class SearchIdentification
        {
            public long? createDate { get; set; }
            public int? bucket { get; set; }
            public string? internalReceiptId { get; set; }
            public string? searchUuid { get; set; }
        }

        public class MyDocument
        {
            public int? returnValue { get; set; }
            public string? errorCode { get; set; }
            public string? errorDescription { get; set; }
            public Receipt? receipt { get; set; }
            public SearchIdentification? searchIdentification { get; set; }
        }

        public static class Converter
        {
            public static string JsonToXml(string json, string currentCode)
            {
                string appPath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "\\AppData\\Local\\Blockovator";
                Console.Write(json);
                MyDocument xmlDoc = System.Text.Json.JsonSerializer.Deserialize<MyDocument>(json);
                string xml = "";
                string log = "";
                System.IO.File.WriteAllText(appPath+"\\json.json", json);
                if (xmlDoc.returnValue != 0)
                {
                    log = " ERROR " + xmlDoc.errorCode + ": " + xmlDoc.errorDescription + "\n";
                    System.IO.File.AppendAllText(appPath + "\\log.txt", log);
                    System.IO.File.AppendAllText(appPath + "\\zlog.txt", currentCode + "\n");
                    return xml;
                }
                if (xmlDoc.returnValue != null)
                {
                    xml += "<returnValue>" + xmlDoc.returnValue + "</returnValue>";
                }
                xml += "<receipt>";
                if (xmlDoc.receipt.receiptId != null)
                {
                    xml += "<receiptId>" + xmlDoc.receipt.receiptId + "</receiptId>";
                }
                if (xmlDoc.receipt.ico != null)
                {
                    xml += "<ico>" + xmlDoc.receipt.ico + "</ico>";
                }
                if (xmlDoc.receipt.cashRegisterCode != null)
                {
                    xml += "<cashRegisterCode>" + xmlDoc.receipt.cashRegisterCode + "</cashRegisterCode>";
                }
                if (xmlDoc.receipt.issueDate != null)
                {
                    xml += "<issueDate>" + xmlDoc.receipt.issueDate + "</issueDate>";
                }
                if (xmlDoc.receipt.createDate != null)
                {
                    xml += "<createDate>" + xmlDoc.receipt.createDate + "</createDate>";
                }
                if (xmlDoc.receipt.customerId != null)
                {
                    xml += "<customerId>" + xmlDoc.receipt.customerId + "</customerId>";
                }
                if (xmlDoc.receipt.dic != null)
                {
                    xml += "<dic>" + xmlDoc.receipt.dic + "</dic>";
                }
                if (xmlDoc.receipt.icDph != null)
                {
                    xml += "<icDph>" + xmlDoc.receipt.icDph + "</icDph>";
                }
                if (xmlDoc.receipt.invoiceNumber != null)
                {
                    xml += "<invoiceNumber>" + xmlDoc.receipt.invoiceNumber + "</invoiceNumber>";
                }
                if (xmlDoc.receipt.okp != null)
                {
                    xml += "<okp>" + xmlDoc.receipt.okp + "</okp>";
                }
                if (xmlDoc.receipt.paragon != null)
                {
                    xml += "<paragon>" + xmlDoc.receipt.paragon + "</paragon>";
                }
                if (xmlDoc.receipt.paragonNumber != null)
                {
                    xml += "<paragonNumber>" + xmlDoc.receipt.paragonNumber + "</paragonNumber>";
                }
                if (xmlDoc.receipt.pkp != null)
                {
                    xml += "<pkp>" + xmlDoc.receipt.pkp + "</pkp>";
                }
                if (xmlDoc.receipt.receiptNumber != null)
                {
                    xml += "<receiptNumber>" + xmlDoc.receipt.receiptNumber + "</receiptNumber>";
                }
                if (xmlDoc.receipt.type != null)
                {
                    xml += "<type>" + xmlDoc.receipt.type + "</type>";
                }
                if (xmlDoc.receipt.taxBaseBasic != null)
                {
                    xml += "<taxBaseBasic>" + xmlDoc.receipt.taxBaseBasic + "</taxBaseBasic>";
                }
                if (xmlDoc.receipt.taxBaseReduced != null)
                {
                    xml += "<taxBaseReduced>" + xmlDoc.receipt.taxBaseReduced + "</taxBaseReduced>";
                }
                if (xmlDoc.receipt.totalPrice != null)
                {
                    xml += "<totalPrice>" + xmlDoc.receipt.totalPrice + "</totalPrice>";
                }
                if (xmlDoc.receipt.freeTaxAmount != null)
                {
                    xml += "<freeTaxAmount>" + xmlDoc.receipt.freeTaxAmount + "</freeTaxAmount>";
                }
                if (xmlDoc.receipt.vatAmountBasic != null)
                {
                    xml += "<vatAmountBasic>" + xmlDoc.receipt.vatAmountBasic + "</vatAmountBasic>";
                }
                if (xmlDoc.receipt.vatAmountReduced != null)
                {
                    xml += "<vatAmountReduced>" + xmlDoc.receipt.vatAmountReduced + "</vatAmountReduced>";
                }
                if (xmlDoc.receipt.vatRateBasic != null)
                {
                    xml += "<vatRateBasic>" + xmlDoc.receipt.vatRateBasic + "</vatRateBasic>";
                }
                if (xmlDoc.receipt.vatRateReduced != null)
                {
                    xml += "<vatRateReduced>" + xmlDoc.receipt.vatRateReduced + "</vatRateReduced>";
                }
                xml += "<items>";
                for (int i = 0; i < xmlDoc.receipt.items.Count; i++)
                {
                    xml += "<item>";
                    if (xmlDoc.receipt.items[i].name != null)
                    {
                        xml += "<name>" + xmlDoc.receipt.items[i].name + "</name>";
                    }
                    if (xmlDoc.receipt.items[i].itemType != null)
                    {
                        xml += "<itemType>" + xmlDoc.receipt.items[i].itemType + "</itemType>";
                    }
                    if (xmlDoc.receipt.items[i].quantity != null)
                    {
                        xml += "<quantity>" + xmlDoc.receipt.items[i].quantity + "</quantity>";
                    }
                    if (xmlDoc.receipt.items[i].vatRate != null)
                    {
                        xml += "<vatRate>" + xmlDoc.receipt.items[i].vatRate + "</vatRate>";
                    }
                    if (xmlDoc.receipt.items[i].price != null)
                    {
                        xml += "<price>" + xmlDoc.receipt.items[i].price + "</price>";
                    }
                    xml += "</item>";
                }
                xml += "</items>";
                xml += "<organization>";
                if (xmlDoc.receipt.organization.buildingNumber != null)
                {
                    xml += "<buildingNumber>" + xmlDoc.receipt.organization.buildingNumber + "</buildingNumber>";
                }
                if (xmlDoc.receipt.organization.country != null)
                {
                    xml += "<country>" + xmlDoc.receipt.organization.country + "</country>";
                }
                if (xmlDoc.receipt.organization.dic != null)
                {
                    xml += "<dic>" + xmlDoc.receipt.organization.dic + "</dic>";
                }
                if (xmlDoc.receipt.organization.icDph != null)
                {
                    xml += "<icDph>" + xmlDoc.receipt.organization.icDph + "</icDph>";
                }
                if (xmlDoc.receipt.organization.ico != null)
                {
                    xml += "<ico>" + xmlDoc.receipt.organization.ico + "</ico>";
                }
                if (xmlDoc.receipt.organization.municipality != null)
                {
                    xml += "<municipality>" + xmlDoc.receipt.organization.municipality + "</municipality>";
                }
                if (xmlDoc.receipt.organization.name != null)
                {
                    xml += "<name>" + xmlDoc.receipt.organization.name + "</name>";
                }
                if (xmlDoc.receipt.organization.postalCode != null)
                {
                    xml += "<postalCode>" + xmlDoc.receipt.organization.postalCode + "</postalCode>";
                }
                if (xmlDoc.receipt.organization.propertyRegistrationNumber != null)
                {
                    xml += "<propertyRegistrationNumber>" + xmlDoc.receipt.organization.propertyRegistrationNumber + "</propertyRegistrationNumber>";
                }
                if (xmlDoc.receipt.organization.streetName != null)
                {
                    xml += "<streetName>" + xmlDoc.receipt.organization.streetName + "</streetName>";
                }
                xml += "</organization>";
                xml += "<unit>";
                if (xmlDoc.receipt.unit.cashRegisterCode != null)
                {
                    xml += "<cashRegistrationCode>" + xmlDoc.receipt.unit.cashRegisterCode + "</cashRegistrationCode>";
                }
                if (xmlDoc.receipt.unit.buildingNumber != null)
                {
                    xml += "<buildingNumber>" + xmlDoc.receipt.unit.buildingNumber + "</buildingNumber>";
                }
                if (xmlDoc.receipt.unit.country != null)
                {
                    xml += "<country>" + xmlDoc.receipt.unit.country + "</country>";
                }
                if (xmlDoc.receipt.unit.municipality != null)
                {
                    xml += "<municipality>" + xmlDoc.receipt.unit.municipality + "</municipality>";
                }
                if (xmlDoc.receipt.unit.postalCode != null)
                {
                    xml += "<postalCode>" + xmlDoc.receipt.unit.postalCode + "</postalCode>";
                }
                if (xmlDoc.receipt.unit.propertyRegistrationNumber != null)
                {
                    xml += "<propertyRegistrationNumber>" + xmlDoc.receipt.unit.propertyRegistrationNumber + "</propertyRegistrationNumber>";
                }
                if (xmlDoc.receipt.unit.streetName != null)
                {
                    xml += "<streetName>" + xmlDoc.receipt.unit.streetName + "</streetName>";
                }
                if (xmlDoc.receipt.unit.name != null)
                {
                    xml += "<name>" + xmlDoc.receipt.unit.name + "</name>";
                }
                if (xmlDoc.receipt.unit.unitType != null)
                {
                    xml += "<unitType>" + xmlDoc.receipt.unit.unitType + "</unitType>";
                }
                xml += "</unit>";
                if (xmlDoc.receipt.exemption != null)
                {
                    xml += "<exemption>" + xmlDoc.receipt.exemption + "</exemption>";
                }
                xml += "</receipt>";
                xml += "<searchIdentification>";
                if (xmlDoc.searchIdentification.createDate != null)
                {
                    xml += "<createDate>" + xmlDoc.searchIdentification.createDate + "</createDate>";
                }
                if (xmlDoc.searchIdentification.bucket != null)
                {
                    xml += "<bucket>" + xmlDoc.searchIdentification.bucket + "</bucket>";
                }
                if (xmlDoc.searchIdentification.internalReceiptId != null)
                {
                    xml += "<internalReceiptId>" + xmlDoc.searchIdentification.internalReceiptId + "</internalReceiptId>";
                }
                if (xmlDoc.searchIdentification.searchUuid != null)
                {
                    xml += "<searchUuid>" + xmlDoc.searchIdentification.searchUuid + "</searchUuid>";
                }
                xml += "</searchIdentification>";
                Console.Write(xml);
                log = " OK\n";
                System.IO.File.AppendAllText(appPath + "\\log.txt", log);
                return xml;
            }
        }
    }
}