using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace DD.Models
{
    public class CategoryCell : ViewCell
    {
        public CategoryCell() {
            Frame gridFrame = new Frame() {
                BorderColor = (Color)Application.Current.Resources["ColorButton"],
                Padding = new Thickness(1),
                BackgroundColor = Color.Transparent,
                HorizontalOptions = LayoutOptions.Fill,
                CornerRadius = 0,
            };

            Grid mainGrid = new Grid
            {
                HorizontalOptions = LayoutOptions.Fill,
                Padding = new Thickness(1),
                ColumnDefinitions = new ColumnDefinitionCollection() {
                    new ColumnDefinition(){ Width = new GridLength(70, GridUnitType.Star) },
                    new ColumnDefinition(){ Width = new GridLength(30, GridUnitType.Star) },
                },
            };

            Label catLabel = new Label()
            {
                HorizontalOptions = LayoutOptions.Fill,
                VerticalOptions = LayoutOptions.Fill,
                VerticalTextAlignment = TextAlignment.Center,
            };
            catLabel.SetBinding(Label.TextProperty, "Label");

            Frame entryFrame = new Frame() {
                BorderColor = (Color)Application.Current.Resources["ColorAccent"],
                Padding = new Thickness(1),
                BackgroundColor = Color.Transparent,
                HorizontalOptions = LayoutOptions.Fill,
                CornerRadius = 0,
            };

            Entry catEntry = new Entry()
            {
                HorizontalOptions = LayoutOptions.Fill,
                VerticalOptions = LayoutOptions.Fill,
                FontSize = 14,
            };
            catEntry.SetBinding(Entry.TextProperty, "Value");
            entryFrame.Content = catEntry;
            entryFrame.SetBinding(Frame.IsVisibleProperty, "HasEntry");

            mainGrid.Children.Add(catLabel, 0, 0);
            mainGrid.Children.Add(entryFrame, 1, 0);

            gridFrame.Content = mainGrid;
            View = gridFrame;
            ForceUpdateSize();
        }

        protected override void OnBindingContextChanged()
        {
            base.OnBindingContextChanged();
        }
    }
}
