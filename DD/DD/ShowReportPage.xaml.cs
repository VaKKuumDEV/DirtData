using DD.Models;
using Newtonsoft.Json.Linq;
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
    public partial class ShowReportPage : ContentPage
    {
        private Report Report { get; }
        private readonly ObservableCollection<ReportCategory> ReportCategories = new ObservableCollection<ReportCategory>();
        private readonly ObservableCollection<ReportAttachment> ReportAttachments = new ObservableCollection<ReportAttachment>();
        private ReportCategory[] RealCategories;
        
        public ShowReportPage(Report report)
        {
            Report = report;

            Thread loadThread = new Thread(new ThreadStart(() =>
            {
                List<ReportCategory> categories = LoadCategories();
                RealCategories = categories.ToArray();

                foreach (var cat in categories) {
                    MainThread.BeginInvokeOnMainThread(() => ReportCategories.Add(cat));
                    Thread.Sleep(100);
                }
            }))
            {
                IsBackground = false,
            };
            loadThread.Start();

            InitializeComponent();

            CategoriesCollection.ItemsSource = ReportCategories;

            if (Report.Attachments.Length > 0) 
            {
                ImagesCollection.ItemsSource = ReportAttachments;
                Thread imagesThread = new Thread(new ThreadStart(() =>
                {
                    foreach (var att in Report.Attachments)
                    {
                        MainThread.BeginInvokeOnMainThread(() => ReportAttachments.Add(att));
                        Thread.Sleep(150);
                    }
                }))
                {
                    IsBackground = false
                };
                imagesThread.Start();
            }
            else ImagesCollection.IsVisible = false;

            Geocoding.GetPlacemarksAsync(Report.Latitude, Report.Longitude).ContinueWith((x) =>
            {
                var placemark = x.Result?.FirstOrDefault();

                string locationName = "Не удалось определить";
                if (placemark != null)
                {
                    locationName = placemark.Locality;
                }

                LocationNameLabel.Text = locationName;
                LatitudeSpan.Text = Report.Latitude.ToString("##0.000000");
                LongitudeSpan.Text = Report.Longitude.ToString("##0.000000");
            });

            SearchBarEntry.SearchButtonPressed += new EventHandler((sender, args) => {
                string text = SearchBarEntry.Text;
                MakeSearch(text);
            });

            SearchBarEntry.TextChanged += new EventHandler<TextChangedEventArgs>((sender, args) => {
                if (args.NewTextValue.Length == 0) {
                    MakeSearch("");
                }
            });
        }

        private List<ReportCategory> LoadCategories() {
            List<ReportCategory> categories = new List<ReportCategory>();
            void recur(JObject cats, int level = 0)
            {
                foreach (var cat in cats)
                {
                    bool hasChilren = false;
                    JObject catArray = null;
                    string value = null;
                    if (cat.Value.Type == JTokenType.Object)
                    {
                        hasChilren = true;
                        catArray = (JObject)cat.Value;
                    }
                    else if (cat.Value.Type == JTokenType.String) 
                    {
                        value = cat.Value.Value<string>();
                    }

                    ReportCategory category = new ReportCategory(cat.Key, !hasChilren, level, categories.Count);
                    if (cat.Key.ToLower() == "анализ воды") hasChilren = false;

                    if (hasChilren)
                    {
                        categories.Add(category);
                        recur(catArray, level + 1);
                    }
                    else if(value != null) 
                    {
                        category.Value = value;
                        categories.Add(category);
                    }
                }
            }

            recur(Report.Content);

            List<ReportCategory> newCategories = new List<ReportCategory>();
            for (int i = 0; i < categories.Count; i++)
            {
                ReportCategory parentCat = categories[i];
                if (parentCat.HasEntry) continue;
                List<ReportCategory> bufferChildren = new List<ReportCategory>();

                for (int j = i + 1; j < categories.Count; j++)
                {
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

            return newCategories;
        }

        private void MakeSearch(string text) {
            if (text.Length > 0)
            {
                List<ReportCategory> newCategories = new List<ReportCategory>();
                for(int i = 0; i < RealCategories.Length; i++)
                {
                    ReportCategory cat = RealCategories[i];
                    if (cat.HasChildren)
                    {
                        List<ReportCategory> childrenList = new List<ReportCategory>();
                        foreach (var childCat in cat.ChildrenList)
                        {
                            if (childCat.OriginalLabel.ToLower().Contains(text.ToLower())) childrenList.Add(childCat);
                        }
                        
                        if (childrenList.Count > 0) {
                            cat.ChildrenList = childrenList;
                            newCategories.Add(cat);
                        }
                    }
                    else newCategories.Add(cat);
                }

                ReportCategories.Clear();
                foreach (var cat in newCategories) ReportCategories.Add(cat);
            }
            else 
            {
                ReportCategories.Clear();

                RealCategories = LoadCategories().ToArray();
                foreach (var cat in RealCategories) ReportCategories.Add(cat);
            }
        }
    }
}