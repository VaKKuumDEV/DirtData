using DD.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace DD
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LoginPage : ContentPage
    {
        public LoginPage()
        {
            Title = "Войти в систему";
            InitializeComponent();

            LoginButton.Clicked += new EventHandler((sender, args) => {
                LoginUser();
            });
        }

        public void ShowMessage(string message)
        {
            DependencyService.Get<IMessage>().ShortAlert(message);
        }

        private void LoginUser() {
            string login = LoginEntry.Text;
            string password = PasswordEntry.Text;

            if (string.IsNullOrEmpty(login) || string.IsNullOrEmpty(password)) return;

            Thread thread = new Thread(new ThreadStart(() => {
                try
                {
                    string hash = WebApi.LoginUser(login, password);
                    Utils.SavePropValue("hash", hash);

                    MainThread.BeginInvokeOnMainThread(() => ShowMessage("Успешная авторизация"));
                    MainThread.BeginInvokeOnMainThread(() => Navigation.PopToRootAsync(true));
                }
                catch (ApiException apiEx)
                {
                    MainThread.BeginInvokeOnMainThread(() => ShowMessage(apiEx.Message));
                }
                catch (Exception ex) 
                {
                    MainThread.BeginInvokeOnMainThread(() => ShowMessage("Внутренняя ошибка: " + ex.Message));
                }
            }));
            thread.IsBackground = false;
            thread.Start();
        }
    }
}