using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace DD.Models
{
    public class ReportCategory
    {
        public string Label { get; }
        public bool HasEntry { get; }
        public int Level { get; }
        public string OriginalLabel { get; }
        public string Value { get; set; } = "";
        public bool Blocked { get; } = false;
        public int LocalId { get; set; }
        public bool IsReal { get; } = true;
        public ObservableCollection<KeyValuePair<ReportCategory, ReportCategory>> Children { get; }
        public bool HasChildren { get; set; } = false;
        public List<ReportCategory> ChildrenList { 
            get 
            {
                List<ReportCategory> cats = new List<ReportCategory>();
                foreach (var kv in Children) {
                    if (kv.Key != null && kv.Key.IsReal) cats.Add(kv.Key);
                    if (kv.Value != null && kv.Value.IsReal) cats.Add(kv.Value);
                }

                return cats;
            }
            set 
            {
                List<KeyValuePair<ReportCategory, ReportCategory>> children = new List<KeyValuePair<ReportCategory, ReportCategory>>();
                for (int a = 0; a < (value.Count / 2) + 1; a++)
                {
                    ReportCategory key = (a * 2) < value.Count ? value[a * 2] : new ReportCategory();
                    ReportCategory val = ((a * 2) + 1) < value.Count ? value[a * 2 + 1] : new ReportCategory();
                    if (key != null || val != null)
                    {
                        children.Add(new KeyValuePair<ReportCategory, ReportCategory>(key, val));
                    }
                }

                Children.Clear();
                foreach (var kv in children) Children.Add(kv);
            }
        }

        public ReportCategory(string label, bool hasEntry = true, int level = 0, int localId = 0)
        {
            OriginalLabel = label;
            HasEntry = hasEntry;
            Level = level;
            LocalId = localId;

            string tabs = "";
            for (int i = 0; i < level; i++) tabs += "    ";

            string suffix = "";
            if (label.ToLower() == "анализ воды")
            {
                suffix = " - находится в стадии разработки";
                HasEntry = false;
                Blocked = true;
            }

            Label = tabs + label + suffix;
            Children = new ObservableCollection<KeyValuePair<ReportCategory, ReportCategory>>();

            Children.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler((sender, args) => {
                if (Children.Count > 0) HasChildren = true;
            });
        }

        public ReportCategory() {
            IsReal = false;
        }
    }
}
