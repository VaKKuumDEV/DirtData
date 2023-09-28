using DD.Models;
using DD.Services;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace DD
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SendReportPage : ContentPage
    {
        private ObservableCollection<ReportCategory> ReportCategories = new ObservableCollection<ReportCategory>();
        private List<ReportAttachment> ReportAttachments = new List<ReportAttachment>();
        private CancellationTokenSource cts;

        private Location CurrentLocation { get; set; } = null;

        public SendReportPage()
        {
            InitializeComponent();

            CategoriesCollection.ItemsSource = ReportCategories;

            CenterMenuButton.GestureRecognizers.Add(new TapGestureRecognizer() {
                Command = new Command(() => {
                    if (CurrentLocation == null) ShowMessage("Загрузите вашу геопозицию");
                    else
                    {
                        int loadedAttachments = 0;
                        foreach (var att in ReportAttachments) if (!att.IsLocal) loadedAttachments++;
                        if (loadedAttachments == 0) { ShowMessage("Загрузите фотографии"); return; }

                        Dictionary<string, object> jsonData = new Dictionary<string, object>();
                        KeyValuePair<string, object> recur(int index)
                        {
                            Dictionary<string, object> catData = new Dictionary<string, object>();
                            ReportCategory cat = ReportCategories[index];
                            for (int i = index + 1; i < ReportCategories.Count; i++)
                            {
                                ReportCategory childCat = ReportCategories[i];
                                if (childCat.Level > cat.Level)
                                {
                                    if (childCat.Level - cat.Level == 1)
                                    {
                                        if (!childCat.HasEntry)
                                        {
                                            if (childCat.HasChildren)
                                            {
                                                Dictionary<string, string> values = new Dictionary<string, string>();
                                                foreach (ReportCategory catC in childCat.ChildrenList)
                                                {
                                                    if (!catC.IsReal) continue;

                                                    double value = 0;
                                                    if (catC.Value.Length > 0)
                                                    {
                                                        double.TryParse(catC.Value, NumberStyles.Float, CultureInfo.InvariantCulture, out value);
                                                    }

                                                    values[catC.OriginalLabel] = value.ToString("##0.######", CultureInfo.InvariantCulture);
                                                }
                                                catData[childCat.OriginalLabel] = values;
                                            }
                                            else
                                            {
                                                var kv = recur(childCat.LocalId);
                                                catData[kv.Key] = kv.Value;
                                            }
                                        }
                                    }
                                }
                                else break;
                            }

                            return new KeyValuePair<string, object>(cat.OriginalLabel, catData);
                        }

                        foreach (ReportCategory cat in ReportCategories)
                        {
                            if (cat.Level == 0 && !cat.HasEntry)
                            {
                                var kv = recur(cat.LocalId);
                                jsonData[kv.Key] = kv.Value;
                            }
                        }

                        string ss = JsonConvert.SerializeObject(jsonData);
                        SendReport(ss);
                    }
                }),
            });

            LeftMenuButton.GestureRecognizers.Add(new TapGestureRecognizer() {
                Command = new Command(async () => {
                    var options = new PickOptions
                    {
                        PickerTitle = "Добавьте изображения для объявления",
                        FileTypes = FilePickerFileType.Images,
                    };

                    var files = await PickAndShow(options);
                    if (files != null)
                    {
                        foreach (var image in files.ToArray())
                        {
                            if (ReportAttachments.Count >= 6)
                            {
                                ShowMessage("К загрузке доступно не более 6-ти файлов");
                                break;
                            }

                            FileInfo file = new FileInfo(image.FullPath);
                            if (file.Name.EndsWith("jpg", StringComparison.OrdinalIgnoreCase) || file.Name.EndsWith("png", StringComparison.OrdinalIgnoreCase) || file.Name.EndsWith("jpeg", StringComparison.OrdinalIgnoreCase))
                            {
                                ReportAttachments.Add(new ReportAttachment(file));
                            }
                        }

                        UpdateAttachmentsView();
                    }
                }),
            });

            RightMenuButton.GestureRecognizers.Add(new TapGestureRecognizer() {
                Command = new Command(async () => {
                    Location loc = await GetCurrentLocation();
                    if (loc != null)
                    {
                        CurrentLocation = loc;

                        var placemarks = await Geocoding.GetPlacemarksAsync(CurrentLocation.Latitude, CurrentLocation.Longitude);
                        var placemark = placemarks?.FirstOrDefault();

                        string locationName = "Не удалось определить";
                        if (placemark != null) {
                            locationName = placemark.Locality;
                        }

                        LocationNameLabel.Text = locationName;
                        LatitudeSpan.Text = CurrentLocation.Latitude.ToString("#.0000");
                        LongitudeSpan.Text = CurrentLocation.Longitude.ToString("#.0000");
                    }
                }),
            });

            InitReportCategories();
            UpdateAttachmentsView();
        }

        public void ShowMessage(string message)
        {
            DependencyService.Get<IMessage>().ShortAlert(message);
        }

        private async Task<IEnumerable<FileResult>> PickAndShow(PickOptions options)
        {
            try
            {
                var result = await FilePicker.PickMultipleAsync(options);
                if (result != null)
                {
                    return result;
                }
            }
            catch (Exception) { }

            return null;
        }

        private void UpdateAttachmentsView(bool upload = true) {
            if (ReportAttachments.Count == 0) AttachmentsFrame.IsVisible = false;
            else {
                AttachmentsFrame.IsVisible = true;
                AttachmentsLayout.Children.Clear();
                foreach (var att in ReportAttachments) {
                    FileInfo file = new FileInfo(att.PhotoUrl);
                    string suffix = "";
                    if (att.Status.Length > 0) suffix = " - " + att.Status;

                    Label attLabel = new Label() {
                        Text = file.Name + suffix,
                        HorizontalOptions = LayoutOptions.Fill,
                        BackgroundColor = Color.Transparent,
                    };

                    attLabel.GestureRecognizers.Add(new TapGestureRecognizer() {
                        Command = new Command(new Action<object>(async (obj) => {
                            List<string> additionalActions = new List<string>();
                            if (att.IsLocal) additionalActions.Add("Повторить загрузку");

                            string action = await DisplayActionSheet("Выберите действие с изображением", "Отмена", "Удалить", additionalActions.ToArray());
                            if (action == "Удалить")
                            {
                                int index = ReportAttachments.IndexOf(att);
                                ReportAttachments.RemoveAt(index);
                                UpdateAttachmentsView(false);
                            }
                            else if (action == "Повторить загрузку") 
                            {
                                UpdateAttachmentsView();
                            }
                        })),
                    });

                    AttachmentsLayout.Children.Add(attLabel);
                }

                if (upload) {
                    int notLoadCount = 0;
                    ReportAttachments.ForEach((att) => { if (att.IsLocal) notLoadCount++; });

                    if (notLoadCount > 0)
                    {
                        Thread thread = new Thread(new ThreadStart(() => {
                            ReportAttachments = new List<ReportAttachment>(WebApi.LoadAttachments(Utils.GetPropValue("hash"), ReportAttachments.ToArray()));
                            MainThread.BeginInvokeOnMainThread(() => {
                                UpdateAttachmentsView();
                            });
                        }));
                        thread.IsBackground = false;
                        thread.Start();
                    }
                }
            }
        }

        private void InitReportCategories() {
            ReportCategories.Clear();

            Thread thread = new Thread(new ThreadStart(() => {
                JObject catsAnswerJson = WebApi.GetCategories();
                List<ReportCategory> categories = new List<ReportCategory>();
                if (catsAnswerJson.Value<string>("code") == "1")
                {
                    void recur(JObject cats, int level = 0) {
                        foreach (var cat in cats)
                        {
                            bool hasChilren = false;
                            JObject catArray = null;
                            if (cat.Value.GetType() == typeof(JObject)) {
                                hasChilren = true;
                                catArray = (JObject)cat.Value;
                            }

                            categories.Add(new ReportCategory(cat.Key, !hasChilren, level, categories.Count));
                            if (cat.Key.ToLower() == "анализ воды") hasChilren = false;

                            if(hasChilren) recur(catArray, level + 1);
                        }
                    }

                    JObject catsArray = catsAnswerJson.Value<JObject>("categories");
                    recur(catsArray);

                    List<ReportCategory> newCategories = new List<ReportCategory>();
                    for (int i = 0; i < categories.Count; i++) {
                        ReportCategory parentCat = categories[i];
                        if (parentCat.HasEntry) continue;
                        List<ReportCategory> bufferChildren = new List<ReportCategory>();

                        for (int j = i + 1; j < categories.Count; j++) {
                            ReportCategory childCat = categories[j];

                            if (childCat.HasEntry)
                            {
                                bufferChildren.Add(childCat);
                            }
                            else break;
                        }

                        parentCat.ChildrenList = bufferChildren;
                        newCategories.Add(parentCat);
                    }

                    foreach (var cat in newCategories) ReportCategories.Add(cat);
                }
                else 
                {
                    string message = catsAnswerJson.Value<string>("message");
                    MainThread.BeginInvokeOnMainThread(() => ShowMessage(message));
                }
            }));
            thread.IsBackground = false;
            thread.Start();
        }

        private void SendReport(string content) {
            string hash = Utils.GetPropValue("hash");
            double longitude = CurrentLocation.Longitude;
            double latitude = CurrentLocation.Latitude;

            Thread thread = new Thread(new ThreadStart(() => {
                JObject answer = WebApi.SendReport(hash, content, longitude, latitude, ReportAttachments.ToArray());
                if (answer.Value<string>("code") == "1")
                {
                    MainThread.BeginInvokeOnMainThread(async () => {
                        ShowMessage(answer.Value<string>("message"));
                        await Navigation.PopAsync();
                    });
                }
                else 
                {
                    MainThread.BeginInvokeOnMainThread(() => {
                        ShowMessage(answer.Value<string>("message"));
                    });
                }
            }));
            thread.IsBackground = true;
            thread.Start();
        }

        private async Task<Location> GetCurrentLocation()
        {
            try
            {
                var request = new GeolocationRequest(GeolocationAccuracy.Best, TimeSpan.FromSeconds(10));
                cts = new CancellationTokenSource();
                var location = await Geolocation.GetLocationAsync(request, cts.Token);

                if (location != null)
                {
                    return location;
                }
            }
            catch (FeatureNotSupportedException)
            {
                ShowMessage("Данная возможность не поддерживается устройством");
            }
            catch (FeatureNotEnabledException)
            {
                ShowMessage("Геопозиционирование не включено в настройках");
            }
            catch (PermissionException)
            {
                ShowMessage("Не предоставлены необходимые права");
            }
            catch (Exception ex)
            {
                ShowMessage("Непредвиденная ошибка: " + ex.Message);
            }

            return null;
        }

        protected override void OnDisappearing()
        {
            if (cts != null && !cts.IsCancellationRequested)
                cts.Cancel();
            base.OnDisappearing();
        }
    }
}