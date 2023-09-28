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
    public partial class RegistrationPage : ContentPage
    {
        public RegistrationPage()
        {
            Title = "Регистрация в системе";
            InitializeComponent();

            RegisterButton.Clicked += new EventHandler((sender, args) => {
                RegisterUser();
            });
        }

        public void ShowMessage(string message)
        {
            DependencyService.Get<IMessage>().ShortAlert(message);
        }

        private void RegisterUser()
        {
            string login = LoginEntry.Text;
            string password = PasswordEntry.Text;
            string passwordConfirm = PasswordConfirmEntry.Text;

            if (string.IsNullOrEmpty(login) || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(passwordConfirm)) return;
            if(password.Trim() != passwordConfirm.Trim()) { ShowMessage("Указанные пароли не совпадают"); return; }

            Thread thread = new Thread(new ThreadStart(() => {
                try
                {
                    string hash = WebApi.RegisterUser(login, password);
                    Utils.SavePropValue("hash", hash);

                    MainThread.BeginInvokeOnMainThread(() => ShowMessage("Успешная регистрация"));
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