using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WoW.Client.Shared;
using WoW.Client.Shared.Web.Model;

namespace WoW.WPF.Launcher.Pages
{
    /// <summary>
    /// Interaction logic for SignIn.xaml
    /// </summary>
    public partial class SignUp : Page
    {
        private HttpClient _webClient;

        public SignUp()
        {
            InitializeComponent();

            _webClient = new HttpClient();
        }

        public void OnRegisterClicked(object sender, EventArgs e)
        {
            var newAccountRegistration = new AccountRegistration()
            {
                AccountName = textBoxUsername.Text.Trim(),
                Password = Utils.ToSHA256(textBoxPassword.Text.Trim()),
                Locale = System.Globalization.CultureInfo.CurrentCulture.ToString().Replace("-", "")
            };

            // todo: obviously add async work here.
            var content = new StringContent(JsonConvert.SerializeObject(newAccountRegistration), Encoding.UTF8, "application/json");
            var resp = _webClient.PostAsync("https://localhost:7159/api/authentication", content);
            var str = resp.Result.Content.ReadAsStringAsync().Result;

            Debug.WriteLine(str);
        }
    }
}
