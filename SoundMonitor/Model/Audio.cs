using NAudio.Wave;
using SoundMonitor.Model.Constant;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;

namespace SoundMonitor.Model
{
    public class Audio : INotifyPropertyChanged
    {
        private readonly Log log;
        private readonly WaveIn waveIn;
        private readonly Settings settings;
        public DebugLog debugLog;
        private bool isRecording;
        private int selectedDevice;
        private readonly int channels;
        private readonly int sampleRate;
        private int levelPercent;
        private int sensitivity;
        public ObservableCollection<Device> DeviceList { get; set; }

        public int SelectedDevice
        {
            get
            {
                return this.selectedDevice;
            }
            set
            {
                if (this.selectedDevice == value)
                    return;

                this.selectedDevice = value;
                this.settings.DeviceNumber = value;
                this.OnPropertyChanged(nameof(this.SelectedDevice));
            }
        }

        public int LevelPercent
        {
            get
            {
                return this.levelPercent;
            }
            set
            {
                if (this.levelPercent == value)
                    return;

                this.levelPercent = value;
                this.OnPropertyChanged(nameof(this.LevelPercent));
            }
        }

        public int Sensitivity
        {
            get
            {
                return this.sensitivity;
            }
            set
            {
                if (this.sensitivity == value)
                    return;

                this.sensitivity = value;
                this.settings.Sensitivity = value;
                this.OnPropertyChanged(nameof(this.Sensitivity));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            if (this.PropertyChanged is null)
                return;

            this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        public Audio(Settings settings, Log log, DebugLog debugLog)
        {
            this.debugLog = debugLog;
            this.log = log;
            this.settings = settings;
            this.isRecording = false;
            this.DeviceList = new ObservableCollection<Device>();
            this.Sensitivity = this.settings.Sensitivity;
            this.SelectedDevice = this.settings.DeviceNumber;
            this.channels = DefaultValues.Channels;
            this.sampleRate = DefaultValues.SampleRate;

            this.waveIn = new WaveIn()
            {
                WaveFormat = new WaveFormat(this.sampleRate, this.channels),
                DeviceNumber = this.SelectedDevice
            };

            this.GetDeviceList();
        }

        public void GetDeviceList()
        {
            int devCount = WaveIn.DeviceCount;
            this.DeviceList.Clear();

            for (int i = 0; i < devCount; i++)
            {
                var device = WaveIn.GetCapabilities(i).ProductName;
                this.DeviceList.Add(new Device(i, device));
            }

            if (this.SelectedDevice >= 0 && this.settings.DeviceNumber < this.DeviceList.Count)
                this.SelectedDevice = this.settings.DeviceNumber;
            else
                this.SelectedDevice = 0;
        }

        public void StartRecording()
        {
            if (!this.isRecording)
            {
                this.waveIn.DeviceNumber = this.SelectedDevice;
                this.waveIn.DataAvailable += new EventHandler<WaveInEventArgs>(this.OnWaveDataAvailable);
                this.waveIn.StartRecording();
                this.isRecording = true;
            }
        }

        public void StopRecording()
        {
            if (this.isRecording)
            {
                this.waveIn.DataAvailable -= new EventHandler<WaveInEventArgs>(this.OnWaveDataAvailable);
                this.waveIn.StopRecording();
                this.LevelPercent = 0;
                this.isRecording = false;
            }
        }

        public void OnWaveDataAvailable(object sender, WaveInEventArgs e)
        {
            this.LevelPercent = 0;

            for (int index = 0; index < e.BytesRecorded; index += 2)
            {
                short sample = (short)((e.Buffer[index + 1] << 8) | e.Buffer[index + 0]);
                float amplitude = sample / 32768f;

                float level = Math.Abs(amplitude); // 0..1
                int currentLevel = Convert.ToInt32(level * 100); // 0..100

                if (this.LevelPercent < currentLevel && currentLevel <= 100)
                    this.LevelPercent = currentLevel;
            }

            if (this.LevelPercent > this.Sensitivity)
            {
                this.debugLog.Write(this.GetDateTime());
                this.log.Write();
                //Debug.WriteLine(this.LevelPercent);
            }
        }

        public void StartDebugRecording()
        {
            if (!this.isRecording)
            {
                this.waveIn.DeviceNumber = this.SelectedDevice;
                this.waveIn.DataAvailable += new EventHandler<WaveInEventArgs>(this.OnWaveDataAvailableDebug);
                this.waveIn.StartRecording();
                this.isRecording = true;
            }
        }

        public void StopDebugRecording()
        {
            if (this.isRecording)
            {
                this.waveIn.DataAvailable -= new EventHandler<WaveInEventArgs>(this.OnWaveDataAvailableDebug);
                this.waveIn.StopRecording();
                this.LevelPercent = 0;
                this.isRecording = false;
            }
        }

        public void OnWaveDataAvailableDebug(object sender, WaveInEventArgs e)
        {
            this.LevelPercent = 0;

            for (int index = 0; index < e.BytesRecorded; index += 2)
            {
                short sample = (short)((e.Buffer[index + 1] << 8) | e.Buffer[index + 0]);
                float amplitude = sample / 32768f;

                float level = Math.Abs(amplitude); // 0..1
                int currentLevel = Convert.ToInt32(level * 100); // 0..100

                if (this.LevelPercent < currentLevel && currentLevel <= 100)
                    this.LevelPercent = currentLevel;
            }

            if (this.LevelPercent > this.Sensitivity)
            {
                this.debugLog.Write(this.GetDateTime());
                Debug.WriteLine(this.LevelPercent);
                //Debug.WriteLine(this.LevelPercent);
            }
        }

        private string GetDateTime()
        {
            return DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + " - " + this.LevelPercent.ToString();
        }
    }
}
