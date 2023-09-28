using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using DD.Models;
using DD.Services;
using System.Globalization;

namespace DD
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AccountPage : ContentPage
    {
        private Account UserAccount { get; }

        public AccountPage(Account account)
        {
            UserAccount = account;
            Title = "Ваш аккаунт";

            InitializeComponent();

            LoginSpan.Text = UserAccount.Login;
            RegDateSpan.Text = UserAccount.RegDate.ToString("dd.MM.yyyy", new CultureInfo("ru"));
            RegTimeSpan.Text = UserAccount.RegDate.ToString("HH:mm", new CultureInfo("ru"));
            ExitButton.Clicked += new EventHandler(async (sender, args) => {
                bool action = await DisplayAlert("Подтвердите", "Вы действительно хотите выйти из аккаунта? Все ваши данные будут сохранены.", "Выйти", "Отмена");
                if (action) {
                    Utils.SavePropValue("hash", "");
                    await Navigation.PopToRootAsync(true);
                }
            });
        }
    }
}