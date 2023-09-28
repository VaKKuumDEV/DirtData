using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace DD.Models
{
    public class SpecialFrame : Frame
    {
        public static readonly BindableProperty ReportIdProperty = BindableProperty.Create("ReportId", typeof(string), typeof(SpecialFrame), "", propertyChanged: ReportIdPropertyChanged, defaultBindingMode: BindingMode.TwoWay);
        public string ReportId {
            get { return (string)GetValue(ReportIdProperty); }
            set { SetValue(ReportIdProperty, value); }
        }

        private static void ReportIdPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var control = (SpecialFrame)bindable;
        }
    }
}
