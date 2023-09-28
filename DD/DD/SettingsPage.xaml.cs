using DD.Models;
using DD.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Xamarin.Essentials;
using System.Collections.ObjectModel;

namespace DD
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SettingsPage : ContentPage
    {
        public ObservableCollection<ApiServer> ApiServers = new ObservableCollection<ApiServer>();

        public SettingsPage()
        {
            Title = "Настройки";

            InitializeComponent();

            ServerPicker.ItemsSource = ApiServers;
            ServerPicker.SelectedIndexChanged += new EventHandler((sender, args) => {
                ApiServer selectedItem = ApiServers[ServerPicker.SelectedIndex];
                if(selectedItem != null) Utils.SavePropValue("server", selectedItem.Name);
            });
        }

        public void ShowMessage(string message)
        {
            DependencyService.Get<IMessage>().ShortAlert(message);
        }

        override protected void OnAppearing() {
            base.OnAppearing();

            InitServers();
        }

        private void InitServers()
        {
            Thread thread = new Thread(new ThreadStart(() =>
            {
                ApiServer[] servers = WebApi.GetApiServers();
                string serverName = Utils.GetPropValue("server");

                ApiServer apiServer = null;
                foreach (var server in servers) if (server.Name == serverName) { apiServer = server; break; }
                if (apiServer == null && servers.Length > 0) apiServer = servers[0];

                WebApi.API_SERVER = apiServer;

                MainThread.BeginInvokeOnMainThread(() => {
                    ApiServers.Clear();
                    foreach (var server in servers) {
                        ApiServers.Add(server);
                    }

                    ServerPicker.SelectedItem = WebApi.API_SERVER;
                });
            }))
            {
                IsBackground = false
            };
            thread.Start();
        }
    }
}