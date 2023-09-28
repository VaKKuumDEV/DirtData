using DD.Models;
using DD.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    public partial class MyReportsPage : ContentPage
    {
        public ObservableCollection<KeyValuePair<Report, Report>> Reports = new ObservableCollection<KeyValuePair<Report, Report>>();

        public MyReportsPage()
        {
            InitializeComponent();

            BindableLayout.SetItemsSource(ReportsCollection, Reports);

            InitReports();
        }

        private void InitReports() {
            Reports.Clear();
            Thread thread = new Thread(new ThreadStart(() =>
            {
                try
                {
                    Report[] myReports = WebApi.UserReports(Utils.GetPropValue("hash"));
                    for (int i = 0; i < (myReports.Length / 2) + 1; i++)
                    {
                        Report key = ((i * 2) < myReports.Length) ? myReports[i * 2] : new Report();
                        Report value = ((i * 2 + 1) < myReports.Length) ? myReports[i * 2 + 1] : new Report();

                        if (key != null || value != null)
                        {
                            MainThread.BeginInvokeOnMainThread(() => Reports.Add(new KeyValuePair<Report, Report>(key, value)));
                            Thread.Sleep(200);
                        }
                    }
                }
                catch (ApiException)
                {
                    MainThread.BeginInvokeOnMainThread(() => EmptyLabel.Text = "Ошибка получения списка отчетов");
                }
                catch (Exception ex)
                {
                    MainThread.BeginInvokeOnMainThread(() => EmptyLabel.Text = "Ошибка: " + ex.Message);
                }
            }))
            {
                IsBackground = false
            };
            thread.Start();
        }

        private async void TapGestureRecognizer_Tapped(object sender, EventArgs e)
        {
            if (sender.GetType() == typeof(SpecialFrame)) {
                SpecialFrame senderFrame = (SpecialFrame)sender;
                int reportId = int.Parse(senderFrame.ReportId);

                Report report = null;
                foreach (var rep in Reports) {
                    if(rep.Key.Id == reportId) { report = rep.Key; break; }
                    else if(rep.Value.Id == reportId) { report = rep.Value; break; }
                }

                if (report != null) {
                    ShowReportPage page = new ShowReportPage(report);
                    await Navigation.PushAsync(page);
                }
            }
        }
    }
}