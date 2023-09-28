using DD.Models;
using DD.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace DD
{
    public partial class MainPage : ContentPage
    {
        public Account UserAccount { get; set; } = null;

        public MainPage()
        {
            InitializeComponent();

            SettingsButton.Clicked += new EventHandler(async (sender, args) => {
                await Navigation.PushAsync(new SettingsPage(), true);
            });

            AuthButton.Clicked += new EventHandler(async (sender, args) => {
                await Navigation.PushAsync(new AuthPage(), true);
            });

            AccountButton.Clicked += new EventHandler(async (sender, args) => {
                await Navigation.PushAsync(new AccountPage(UserAccount), true);
            });

            SendReviewButton.Clicked += new EventHandler(async (sender, args) => {
                await Navigation.PushAsync(new SendReportPage(), true);
            });

            MyReviewsButton.Clicked += new EventHandler(async (sender, args) => {
                await Navigation.PushAsync(new MyReportsPage(), true);
            });

            InitServers();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            if(WebApi.API_SERVER != null) InitAccount();

            DateTime dateTime = DateTime.Now;
            int currentHour = dateTime.Hour;
            string welcomeText = "Доброе утро!";
            if (currentHour >= 12 && currentHour < 18) welcomeText = "Добрый день!";
            else if (currentHour >= 18 && currentHour < 23) welcomeText = "Добрый вечер!";
            else if (currentHour >= 0 && currentHour < 6) welcomeText = "Доброй ночи!";
            WelcomeLabel.Text = welcomeText;

            CultureInfo culture = new CultureInfo("ru");
            WelcomeDateLabel.Text = dateTime.ToString("dddd", culture) + ", " + dateTime.ToString("D", culture);
        }

        public void ShowMessage(string message)
        {
            DependencyService.Get<IMessage>().ShortAlert(message);
        }

        private void CheckButtons() {
            AuthButton.IsVisible = Utils.GetPropValue("hash").Length <= 0;
            MyReviewsButton.IsVisible = Utils.GetPropValue("hash").Length > 0;
            SendReviewButton.IsVisible = Utils.GetPropValue("hash").Length > 0;
            AccountButton.IsVisible = Utils.GetPropValue("hash").Length > 0;
        }

        private void InitServers() {
            Thread thread = new Thread(new ThreadStart(() =>
            {
                ApiServer[] servers = WebApi.GetApiServers();
                string serverName = Utils.GetPropValue("server");

                ApiServer apiServer = null;
                foreach (var server in servers) if (server.Name == serverName) { apiServer = server; break; }
                if (apiServer == null && servers.Length > 0) apiServer = servers[0];

                WebApi.API_SERVER = apiServer;
                InitAccount();

                MainThread.BeginInvokeOnMainThread(() => {
                    if (apiServer == null) ShowMessage("Не удалось подключиться к серверу");
                    else ShowMessage("Подключено к серверу");
                });
            }))
            {
                IsBackground = false
            };
            thread.Start();
        }

        private void InitAccount()
        {
            if (Utils.GetPropValue("hash").Length == 0) {
                MainThread.BeginInvokeOnMainThread(() => {
                    CheckButtons();
                });

                return;
            };

            MainThread.BeginInvokeOnMainThread(() => {
                PageContent.IsVisible = false;
                LoadingIndicator.IsVisible = true;
            });

            Thread thread = new Thread(new ThreadStart(() =>
            {
                try
                {
                    Account account = WebApi.UserAccount(Utils.GetPropValue("hash"));
                    if (account.Status == 0) Utils.SavePropValue("hash", "");
                    else UserAccount = account;
                }
                catch(ApiException apiEx)
                {
                    Utils.SavePropValue("hash", "");
                    MainThread.BeginInvokeOnMainThread(() => ShowMessage(apiEx.Message));
                }
                catch (Exception ex)
                {
                    MainThread.BeginInvokeOnMainThread(() => ShowMessage("Ошибка соединения с сервером"));
                }

                MainThread.BeginInvokeOnMainThread(() => {
                    CheckButtons();

                    PageContent.IsVisible = true;
                    LoadingIndicator.IsVisible = false;
                });
            }))
            {
                IsBackground = false
            };
            thread.Start();
        }
    }
}
