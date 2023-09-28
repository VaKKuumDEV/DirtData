using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace DD.Models
{
    public class Account
    {
        public int Id { get; }
        public string Login { get; }
        public DateTime RegDate { get; }
        public int Status { get; }

        public Account(JObject jsonAccount) {
            Login = jsonAccount.Value<string>("login");

            CultureInfo culture = new CultureInfo("ru");
            RegDate = DateTime.ParseExact(jsonAccount.Value<string>("reg_date"), "yyyy-MM-dd HH:mm:ss", culture);

            Status = int.Parse(jsonAccount.Value<string>("status"));
            Id = int.Parse(jsonAccount.Value<string>("id"));
        }
    }
}
