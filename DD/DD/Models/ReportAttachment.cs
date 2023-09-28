using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace DD.Models
{
    public class ReportAttachment
    {
        public int Id { get; set; }
        public string PhotoUrl { get; set; }
        public Types Type { get; }
        public bool IsLocal { get; set; } = false;
        public string Status { get; set; } = "";

        public enum Types {
            PHOTO,
            NOT_FOUND,
        };

        public ReportAttachment(JObject obj) {
            Id = int.Parse(obj.Value<string>("id"));
            PhotoUrl = obj.Value<string>("photo");

            if (obj.Value<string>("type") == "photo") Type = Types.PHOTO;
            else Type = Types.NOT_FOUND;
        }

        public ReportAttachment(FileInfo file) {
            PhotoUrl = file.FullName;
            IsLocal = true;
        }

        public ReportAttachment(string image)
        {
            PhotoUrl = image;
            IsLocal = true;
            Type = Types.PHOTO;
        }
    }
}
