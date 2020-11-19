using System.ComponentModel;

namespace SoundMonitor.Model
{
    public class Device : INotifyPropertyChanged
    {
        private int id;
        private string name;

        public int Id
        {
            get
            {
                return this.id;
            }
            set
            {
                if (this.id == value)
                    return;

                this.id = value;
                this.OnPropertyChanged(nameof(this.Id));
            }
        }

        public string Name
        {
            get
            {
                return this.name;
            }
            set
            {
                if (this.name == value)
                    return;

                this.name = value;
                this.OnPropertyChanged(nameof(this.Name));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            if (this.PropertyChanged is null)
                return;

            this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        public Device(int id, string name)
        {
            this.Id = id;
            this.Name = name;
        }

        public override string ToString()
        {
            return $"#{this.Id} - {this.Name}";
        }
    }
}
