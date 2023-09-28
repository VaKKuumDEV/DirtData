using DD.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace DD
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AuthPage : ContentPage
    {
        public AuthPage()
        {
            Title = "Авторизация";

            InitializeComponent();
            CheckAuth();

            LoginButton.Clicked += new EventHandler(async (sender, args) => {
                await Navigation.PushAsync(new LoginPage(), true);
            });

            RegistrationButton.Clicked += new EventHandler(async (sender, args) => {
                await Navigation.PushAsync(new RegistrationPage(), true);
            });
        }

        public void ShowMessage(string message)
        {
            DependencyService.Get<IMessage>().ShortAlert(message);
        }

        public void CheckAuth() {
            if (Utils.GetPropValue("hash").Length > 0) {
                ShowMessage("Вы уже авторизированы");
                Navigation.PopAsync(true);
            }
        }
    }
}