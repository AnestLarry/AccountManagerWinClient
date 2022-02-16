using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.DirectoryServices;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Newtonsoft.Json;

namespace WPFDotNetCoreClient
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public class Item
        {
            public string address { get; set; }
            public string account { get; set; }
            public string password { get; set; }

            public string email { get; set; }
            public string date { get; set; }
            public string text { get; set; }
            public Dictionary<string, string> toDictionary()
            {
                Dictionary<string, string> d = new Dictionary<string, string>();
                d.Add("address", address);
                d.Add("account", account);
                d.Add("password", password);
                d.Add("email", email);
                d.Add("date", date);
                d.Add("text", text);
                return d;
            }
        }
        class gets_v1__item_methods__getItems
        {
            public string message;
            public class Result
            {
                public string address, account, password_lv1, password_lv2, password_lv3, password_lv_max, email, date, text;
            };
            public Result value;
        };
        class posts_v1__item_methods__searchItem
        {
            public string message;
            public Item[] result;
        };
        class respondChanged
        {
            public string message { get; set; }
            public uint changed { get; set; }
        }
        class MessageResponse
        {
            public string message { get; set; }
        }
        public string __Window_Title = "Account Manager Win Client";
        public string __Author = "Anest";
        public string __API_Version = "v1";
        public string __GUI_Version = "Spring, 2022.";
        public string __url; // http ://127.0.0.1:8000
        public Item temp_item = null;
        public string AccountStr = "";
        public string PasswordStr = "";
        public string GetHTTPRequest(string url)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "GET";
            request.ContentType = "application/json";
            request.Timeout = 4000;
            HttpWebResponse response = null;
            try
            {
                response = (HttpWebResponse)request.GetResponse();
            }
            catch (Exception)
            {
                throw;
            }
            Stream myResponseStream = response.GetResponseStream();
            StreamReader myStreamReader = new StreamReader(myResponseStream, Encoding.GetEncoding("utf-8"));
            string retString = myStreamReader.ReadToEnd();
            myStreamReader.Close();
            myResponseStream.Close();
            return retString;
        }
        public static string PostHttpRequest(string url, Dictionary<string, string> param)
        {
            string result = null;
            using (HttpClient httpClient = new HttpClient(new HttpClientHandler
            {
                AutomaticDecompression = DecompressionMethods.None,
                ClientCertificateOptions = ClientCertificateOption.Automatic
            }))
                try
                {
                    {
                        httpClient.BaseAddress = new Uri(url);
                        FormUrlEncodedContent content = new FormUrlEncodedContent(param);
                        result = httpClient.PostAsync(url, content).Result.Content.ReadAsStringAsync().Result;
                    }
                }
                catch (Exception)
                {
                    throw;
                }
            return result;
        }

        private static bool CheckValidationResult(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors errors)
        {
            return true;
        }

        //https
        public static HttpWebResponse CreatePostHttpResponse(string url, IDictionary<string, string> parameters, Encoding charset)
        {
            HttpWebRequest request = null;
            ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(CheckValidationResult);
            request = WebRequest.Create(url) as HttpWebRequest;
            request.ProtocolVersion = HttpVersion.Version10;
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            if (!(parameters == null || parameters.Count == 0))
            {
                StringBuilder buffer = new StringBuilder();
                int i = 0;
                foreach (string key in parameters.Keys)
                {
                    if (i > 0)
                    {
                        buffer.AppendFormat("&{0}={1}", key, parameters[key]);
                    }
                    else
                    {
                        buffer.AppendFormat("{0}={1}", key, parameters[key]);
                    }
                    i++;
                }
                byte[] data = charset.GetBytes(buffer.ToString());
                using (Stream stream = request.GetRequestStream())
                {
                    stream.Write(data, 0, data.Length);
                }
            }
            return request.GetResponse() as HttpWebResponse;
        }
        public MainWindow()
        {
            InitializeComponent();
            __url = ServerLink_btn.Text;
        }
        private void About_btn_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(
                string.Format(@"Author: {0}
API Version:{1}
GUI Version:{2}", __Author, __API_Version, __GUI_Version),
                __Window_Title);
        }

        private void ServerLink_btn_LostFocus(object sender, RoutedEventArgs e)
        {
            __url = ServerLink_btn.Text;
        }

        private async void Generate_btn_ClickAsync(object sender, RoutedEventArgs e)
        {
            Generator_Generate_btn.IsEnabled = false;
            try
            {
                string retString = GetHTTPRequest(string.Format("{0}/gets_v1/item_methods/getItems", __url));

                gets_v1__item_methods__getItems r = JsonConvert.DeserializeObject<gets_v1__item_methods__getItems>(retString);
                if (r.message != "succ")
                {
                    MessageBox.Show(string.Format(@"Fail:", r.message), __Window_Title);
                    return;
                }
                await Dispatcher.BeginInvoke(new Action(() =>
                {
                    if (Generator_IsFixedAccount.IsChecked.Value == false)
                    {
                        Generator_Account.Text = r.value.account;
                    }
                    Generator_Password_lv1.Text = r.value.password_lv1;
                    Generator_Password_lv2.Text = r.value.password_lv2;
                    Generator_Password_lv3.Text = r.value.password_lv3;
                    Generator_Password_lv_max.Text = r.value.password_lv_max;
                }));
            }
            catch (Exception exceptionxc)
            {
                MessageBox.Show(exceptionxc.ToString());
            }
            finally
            {
                Generator_Generate_btn.IsEnabled = true;

            }
        }
        private async void Generator_SaveResult_btn_ClickAsync(object sender, RoutedEventArgs e)
        {
            Generator_SaveResult_btn.IsEnabled = false;
            temp_item = new Item();
            temp_item.address = Generator_Address.Text;
            temp_item.account = Generator_Account.Text;
            temp_item.text = Generator_Text.Text;
            temp_item.email = Generator_Email.Text;
            RadioButton checkBtn = Generator_Select.Children.OfType<RadioButton>().FirstOrDefault(r => r.IsChecked.Value);
            switch (checkBtn.Name[checkBtn.Name.Length - 1])
            {
                case '1':
                    temp_item.password = Generator_Password_lv1.Text;
                    break;
                case '2':
                    temp_item.password = Generator_Password_lv2.Text;
                    break;
                case '3':
                    temp_item.password = Generator_Password_lv3.Text;
                    break;
                case 'x':
                    temp_item.password = Generator_Password_lv_max.Text;
                    break;
            }
            await Task.Run(new Action(Generator_SaveResult_btn_Click__Work));
            Generator_SaveResult_btn.IsEnabled = true;
        }

        public void Generator_SaveResult_btn_Click__Work()
        {
            try
            {
                string rs = PostHttpRequest(string.Format("{0}/posts_v1/item_methods/saveItem", __url), temp_item.toDictionary());
                respondChanged r = JsonConvert.DeserializeObject<respondChanged>(rs);
                MessageBox.Show(string.Format("status:{0}\nsave {1} record.", r.message, r.changed), __Window_Title);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString(), __Window_Title);
            }
        }

        private async void Seach_Search_btn_ClickAsync(object sender, RoutedEventArgs e)
        {
            Seach_Search_btn.IsEnabled = false;
            temp_item = new Item();
            RadioButton checkBtn = Search_Select.Children.OfType<RadioButton>().FirstOrDefault(r => r.IsChecked.Value);
            switch (checkBtn.Name[checkBtn.Name.Length - 1])
            {
                case '0':
                    temp_item.address = Search_Address.Text;
                    break;
                case '1':
                    temp_item.account = Search_Account.Text;
                    break;
                case '2':
                    temp_item.password = Search_Password.Text;
                    break;
                case '3':
                    temp_item.email = Search_Email.Text;
                    break;
                case '4':
                    temp_item.text = Search_Text.Text;
                    break;
            }
            try
            {
                await Task.Run(new Action(Seach_Search_btn_Click__Work));
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.ToString(), __Window_Title);
            }
            Seach_Search_btn.IsEnabled = true;
        }
        public void Seach_Search_btn_Click__Work()
        {
            string rs = null;
            try
            {
                rs = PostHttpRequest(string.Format("{0}/posts_v1/item_methods/searchItem", __url), temp_item.toDictionary());
            }
            catch (Exception)
            {
                throw;
            }
            posts_v1__item_methods__searchItem r = JsonConvert.DeserializeObject<posts_v1__item_methods__searchItem>(rs);
            ObservableCollection<Item> searchItems = new ObservableCollection<Item>();
            for (int i = 0; i < r.result.Length; i++)
            {
                searchItems.Add((new Item()
                {
                    address = r.result[i].address,
                    account = r.result[i].account,
                    password = r.result[i].password,
                    email = r.result[i].email,
                    date = r.result[i].date,
                    text = r.result[i].text
                }));
            }
            searchItems.Add(new Item() { address = "Update Date", account = DateTime.Now.ToString(), password = " ", date = " ", text = " " });
            Dispatcher.BeginInvoke(new Action(() => Search_Search_Result.ItemsSource = searchItems));
        }

        private async void Update_btn_ClickAsync(object sender, RoutedEventArgs e)
        {
            temp_item = new Item();
            temp_item.date = Update_Date.Text;
            temp_item.text = Update_Text.Text;
            try
            {
                await Task.Run(new Action(Update_btn_Click__Work));
            }
            catch (Exception e2)
            {
                MessageBox.Show(e2.ToString(), __Window_Title);
            }
        }
        public void Update_btn_Click__Work()
        {
            try
            {
                string rs = PostHttpRequest(string.Format("{0}/posts_v1/item_methods/updateItem", __url), temp_item.toDictionary());
                MessageResponse mr = JsonConvert.DeserializeObject<MessageResponse>(rs);
                MessageBox.Show(string.Format("{0}", mr.message), __Window_Title);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString(), __Window_Title);
            }
        }
        private async void Delete_btn_ClickAsync(object sender, RoutedEventArgs e)
        {
            temp_item = new Item();
            temp_item.date = Delete_Date.Text;
            try
            {
                await Task.Run(new Action(Delete_btn_ClickAsync__Work));
            }
            catch (Exception e2)
            {
                MessageBox.Show(e2.ToString(), __Window_Title);
            }
        }
        public void Delete_btn_ClickAsync__Work()
        {
            string rs = PostHttpRequest(string.Format("{0}/posts_v1/item_methods/deleteItem", __url), temp_item.toDictionary());
            respondChanged rc = JsonConvert.DeserializeObject<respondChanged>(rs);
            MessageBox.Show(string.Format("status:{0}\ndelete {1} record.", rc.message, rc.changed), __Window_Title);
        }
    }
}