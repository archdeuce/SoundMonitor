using NAudio.Wave;
using SoundMonitor.Model;
using SoundMonitor.Model.Constant;
using SoundMonitor.ViewModel.Command;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;

namespace SoundMonitor.ViewModel
{
    class MainWindowViewModel : INotifyPropertyChanged
    {
        public Log Log { get; set; }
        public Settings Settings { get; set; }
        private DebugLog debugLog { get; set; }
        private Audio audio { get; set; }
        private bool buttonAccess;
        private SolidColorBrush stateLabelColor;
        private string stateLabel;

        public DebugLog DebugLog
        {
            get
            {
                return this.debugLog;
            }
            set
            {
                if (this.debugLog == value)
                    return;

                this.debugLog = value;
                this.OnPropertyChanged(nameof(this.DebugLog));
            }
        }

        public Audio Audio
        {
            get
            {
                return this.audio;
            }
            set
            {
                if (this.audio == value)
                    return;

                this.audio = value;
                this.OnPropertyChanged(nameof(this.Audio));
            }
        }

        public bool ButtonAccess
        {
            get
            {
                return this.buttonAccess;
            }
            set
            {
                if (this.buttonAccess == value)
                    return;

                this.buttonAccess = value;
                this.OnPropertyChanged(nameof(this.ButtonAccess));
            }
        }

        public string StateLabel
        {
            get
            {
                return this.stateLabel;
            }
            set
            {
                if (this.stateLabel == value)
                    return;

                this.stateLabel = value;
                this.OnPropertyChanged(nameof(this.StateLabel));
            }
        }

        public SolidColorBrush StateLabelColor
        {
            get
            {
                return this.stateLabelColor;
            }
            set
            {
                if (this.stateLabelColor == value)
                    return;

                this.stateLabelColor = value;
                this.OnPropertyChanged(nameof(this.StateLabelColor));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            if (this.PropertyChanged is null)
                return;

            this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        public ICommand EnableCommand { get; set; }
        public ICommand DisableCommand { get; set; }
        public ICommand LocationCommand { get; set; }
        public ICommand LogCommand { get; set; }
        public ICommand ConfigureCommand { get; set; }
        public ICommand RefreshCommand { get; set; }
        public ICommand SaveCommand { get; set; }
        public ICommand DebugStartCommand { get; set; }
        public ICommand DebugStopCommand { get; set; }

        public MainWindowViewModel()
        {
            this.DebugLog = new DebugLog();
            this.Settings = new Settings();
            this.Log = new Log(this.Settings);
            this.Audio = new Audio(this.Settings, this.Log, this.DebugLog);
            this.StateLabel = DefaultValues.StateOff;
            this.StateLabelColor = new SolidColorBrush(Colors.Red);
            this.ButtonAccess = true;

            this.EnableCommand = new RelayCommand(this.EnableCommandExecuted, this.EnableCommandCanExecute);
            this.DisableCommand = new RelayCommand(this.DisableCommandExecuted, this.DisableCommandCanExecute);
            this.LocationCommand = new RelayCommand(this.LocationCommandExecuted, this.LocationCommandCanExecute);
            this.SaveCommand = new RelayCommand(this.SaveCommandExecuted, this.SaveCommandCanExecute);
            this.RefreshCommand = new RelayCommand(this.RefreshCommandExecuted, this.RefreshCommandCanExecute);
            this.ConfigureCommand = new RelayCommand(this.ConfigureCommandExecuted, this.ConfigureCommandCanExecute);
            this.LogCommand = new RelayCommand(this.LogCommandExecuted, this.LogCommandCanExecute);
            this.DebugStartCommand = new RelayCommand(this.DebugStartCommandExecuted, this.DebugStartCommandCanExecute);
            this.DebugStopCommand = new RelayCommand(this.DebugStopCommandExecuted, this.DebugStopCommandCanExecute);
        }

        public void EnableCommandExecuted(object obj)
        {
            this.Audio = new Audio(this.Settings, this.Log, this.DebugLog);
            this.Audio.StartRecording();

            //переписать на конвертор?
            this.StateLabel = DefaultValues.StateOn;
            this.StateLabelColor = new SolidColorBrush(Colors.Green);

            this.ButtonAccess = !this.ButtonAccess;
        }

        public bool EnableCommandCanExecute(object obj)
        {
            return this.ButtonAccess && WaveIn.DeviceCount > 0;
        }

        public void DisableCommandExecuted(object obj)
        {
            this.Audio.StopRecording();

            //переписать на конвертор?
            this.StateLabel = DefaultValues.StateOff;
            this.StateLabelColor = new SolidColorBrush(Colors.Red);

            this.ButtonAccess = !this.ButtonAccess;
        }

        public bool DisableCommandCanExecute(object obj)
        {
            return !this.ButtonAccess;
        }

        public void LocationCommandExecuted(object obj)
        {
            using (var dialog = new FolderBrowserDialog())
            {
                var result = dialog.ShowDialog();

                if (result == System.Windows.Forms.DialogResult.OK)
                {
                    this.Settings.DirectoryPath = dialog.SelectedPath;
                }
            }
        }

        public bool LocationCommandCanExecute(object obj)
        {
            return this.ButtonAccess;
        }

        public void LogCommandExecuted(object obj)
        {
            ProcessStartInfo startInfo = new ProcessStartInfo()
            {
                FileName = "explorer.exe",
                Arguments = this.Settings.DirectoryPath,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                CreateNoWindow = false
            };

            var process = new Process()
            {
                StartInfo = startInfo
            };

            process.Start();
        }

        public bool LogCommandCanExecute(object obj)
        {
            if (!Directory.Exists(this.Settings.DirectoryPath))
                return false;

            return true;
        }

        public void ConfigureCommandExecuted(object obj)
        {
            ProcessStartInfo startInfo = new ProcessStartInfo()
            {
                FileName = "cmd.exe",
                Arguments = "/c mmsys.cpl,1",
                UseShellExecute = false,
                RedirectStandardOutput = true,
                CreateNoWindow = true
            };

            var process = new Process()
            {
                StartInfo = startInfo
            };

            process.Start();
        }

        public bool ConfigureCommandCanExecute(object obj)
        {
            return true;
        }

        public void RefreshCommandExecuted(object obj)
        {
            this.Audio.GetDeviceList();
        }

        public bool RefreshCommandCanExecute(object obj)
        {
            return this.ButtonAccess;
        }

        public void SaveCommandExecuted(object obj)
        {
            this.Settings.Write();
            this.Settings.Read();
        }

        public bool SaveCommandCanExecute(object obj)
        {
            return this.ButtonAccess;
        }

        public void DebugStartCommandExecuted(object obj)
        {
            this.Audio = new Audio(this.Settings, this.Log, this.DebugLog);
            this.Audio.StartDebugRecording();
            this.ButtonAccess = !this.ButtonAccess;
        }

        public bool DebugStartCommandCanExecute(object obj)
        {
            return this.ButtonAccess && WaveIn.DeviceCount > 0;
        }

        public void DebugStopCommandExecuted(object obj)
        {
            this.Audio.StopDebugRecording();
            this.ButtonAccess = !this.ButtonAccess;
        }

        public bool DebugStopCommandCanExecute(object obj)
        {
            return !this.ButtonAccess;
        }
    }
}
