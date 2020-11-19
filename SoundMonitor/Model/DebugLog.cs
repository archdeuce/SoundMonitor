using System;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace SoundMonitor.Model
{
    public class DebugLog : INotifyPropertyChanged
    {
        private ObservableCollection<string> list;
        private readonly int maxItems;

        public ObservableCollection<string> List
        {
            get
            {
                return this.list;
            }
            set
            {
                if (this.list == value)
                    return;

                this.list = value;
                this.OnPropertyChanged(nameof(this.List));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            if (this.PropertyChanged is null)
                return;

            this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        public DebugLog()
        {
            this.maxItems = 4;
            this.List = new ObservableCollection<string>();
        }

        public void Write(string input)
        {
            if (this.List.Count >= this.maxItems)
                this.List.RemoveAt(0);

            this.List.Add(input);
        }
    }
}