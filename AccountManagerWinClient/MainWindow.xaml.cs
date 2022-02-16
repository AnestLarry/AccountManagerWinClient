using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
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
        struct gets_v1__item_methods__getItems
        {
            public string Message = "";
            public GeneratorResult Value = new GeneratorResult();
        };
        struct posts_v1__item_methods__searchItem
        {
            public string Message = "";
            public Item[] Result = new Item[] { };
        };
        struct respondChanged
        {
            public string message { get; set; }
            public uint changed { get; set; }
        }
        public string __Window_Title = "Account Manager Win Client";
        public string __Author = "Anest";
        public string __API_Version = "v1";
        public string __GUI_Version = "Spring, 2022.";
        public string __url; // http ://127.0.0.1:8000
        public Item temp_item = null;
        public string AccountStr = "";
        public string PasswordStr = "";
        private readonly AMHttpUtils AMHttp = new AMHttpUtils();
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
                string retString = AMHttp.GetHTTPRequest(string.Format("{0}/gets_v1/item_methods/getItems", __url));

                gets_v1__item_methods__getItems r = JsonConvert.DeserializeObject<gets_v1__item_methods__getItems>(retString);
                if (r.Message != "succ")
                {
                    MessageBox.Show(string.Format(@"Fail:", r.Message), __Window_Title);
                    return;
                }
                await Dispatcher.BeginInvoke(new Action(() =>
                {
                    if (Generator_IsFixedAccount.IsChecked.Value == false)
                    {
                        Generator_Account.Text = r.Value.Account;
                    }
                    Generator_Password_lv1.Text = r.Value.Password_lv1;
                    Generator_Password_lv2.Text = r.Value.Password_lv2;
                    Generator_Password_lv3.Text = r.Value.Password_lv3;
                    Generator_Password_lv_max.Text = r.Value.Password_lv_max;
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
                string rs = AMHttp.PostHttpRequest(string.Format("{0}/posts_v1/item_methods/saveItem", __url), temp_item.toDictionary());
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
                rs = AMHttp.PostHttpRequest(string.Format("{0}/posts_v1/item_methods/searchItem", __url), temp_item.toDictionary());
            }
            catch (Exception)
            {
                throw;
            }
            posts_v1__item_methods__searchItem r = JsonConvert.DeserializeObject<posts_v1__item_methods__searchItem>(rs);
            ObservableCollection<Item> searchItems = new ObservableCollection<Item>();
            for (int i = 0; i < r.Result.Length; i++)
            {
                searchItems.Add((new Item()
                {
                    address = r.Result[i].address,
                    account = r.Result[i].account,
                    password = r.Result[i].password,
                    email = r.Result[i].email,
                    date = r.Result[i].date,
                    text = r.Result[i].text
                }));
            }
            searchItems.Add(new Item() { address = "Update Date", account = DateTime.Now.ToString(), password = " ", date = " ", text = " " });
            Dispatcher.BeginInvoke(new Action(() => Search_Search_Result.ItemsSource = searchItems));
        }

        private async void Update_btn_ClickAsync(object sender, RoutedEventArgs e)
        {
            temp_item = new Item();
            temp_item.date = UpdateDelete_Date.Text;
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
                string rs = AMHttp.PostHttpRequest(string.Format("{0}/posts_v1/item_methods/updateItem", __url), temp_item.toDictionary());
                respondChanged mr = JsonConvert.DeserializeObject<respondChanged>(rs);
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
            temp_item.date = UpdateDelete_Date.Text;
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
            string rs = AMHttp.PostHttpRequest(string.Format("{0}/posts_v1/item_methods/deleteItem", __url), temp_item.toDictionary());
            respondChanged rc = JsonConvert.DeserializeObject<respondChanged>(rs);
            MessageBox.Show(string.Format("status:{0}\ndelete {1} record.", rc.message, rc.changed), __Window_Title);
        }
    }
}