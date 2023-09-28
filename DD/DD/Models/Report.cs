using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Newtonsoft.Json.Linq;

namespace DD.Models
{
    public class Report
    {
        public int Id { get; }
        public double Longitude { get; }
        public double Latitude { get; }
        public ReportAttachment[] Attachments { get; }
        public JObject Content { get; }
        public DateTime Date { get; }
        public string FormatedDate { get; }
        public string FormatedTime { get; }
        public bool IsReal { get; } = true;
        public ReportAttachment Image { get; }

        public Report(JObject reportJson) {
            Id = int.Parse(reportJson.Value<string>("id"));
            Longitude = double.Parse(reportJson.Value<string>("longitude"), CultureInfo.InvariantCulture);
            Latitude = double.Parse(reportJson.Value<string>("latitude"), CultureInfo.InvariantCulture);

            List<ReportAttachment> atts = new List<ReportAttachment>();
            foreach (JObject attJson in reportJson.Value<JArray>("attachments")) {
                ReportAttachment att = new ReportAttachment(attJson);
                atts.Add(att);
            }
            Attachments = atts.ToArray();

            Image = new ReportAttachment("icon_image_aqua.png");
            if (Attachments.Length > 0) {
                Image = Attachments[0];
            }

            Content = reportJson.Value<JObject>("content");
            Date = DateTime.ParseExact(reportJson.Value<string>("date"), "yyyy-MM-dd HH:mm:ss", new CultureInfo("ru"));

            FormatedDate = Date.ToString("dd.MM.yyyy", new CultureInfo("ru"));
            FormatedTime = Date.ToString("HH:mm", new CultureInfo("ru"));
        }

        public Report() {
            IsReal = false;
        }
    }
}
