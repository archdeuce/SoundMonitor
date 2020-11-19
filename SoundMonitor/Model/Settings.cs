using Microsoft.Win32;
using SoundMonitor.Model.Constant;
using System;
using System.ComponentModel;

namespace SoundMonitor.Model
{
    public class Settings : INotifyPropertyChanged
    {
        private string directoryPath = "D:\\SoundMonitor";
        private string fileExtention = ".log";
        private int deviceNumber = 0;
        private int sensitivity = 20;

        public string DirectoryPath
        {
            get
            {
                return this.directoryPath;
            }
            set
            {
                if (this.directoryPath == value)
                    return;

                this.directoryPath = value;
                this.OnPropertyChanged(nameof(this.DirectoryPath));
            }
        }

        public string FileExtention
        {
            get
            {
                return this.fileExtention;
            }
            set
            {
                if (this.fileExtention == value)
                    return;

                this.fileExtention = value;
                this.OnPropertyChanged(nameof(this.FileExtention));
            }
        }

        public int DeviceNumber
        {
            get
            {
                return this.deviceNumber;
            }
            set
            {
                if (this.deviceNumber == value)
                    return;

                this.deviceNumber = value;
                this.OnPropertyChanged(nameof(this.DeviceNumber));
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

        public Settings()
        {
            this.SetDefaultValues();
            this.Read();
        }

        public void Read()
        {
            RegistryKey regKey = Registry.CurrentUser.OpenSubKey("SoundMonitor");

            if (regKey == null)
            {
                this.Write();
                return;
            }

            string regValueDirectoryPath = regKey.GetValue(nameof(this.DirectoryPath), "NULL").ToString();
            string regValueFileExtention = regKey.GetValue(nameof(this.FileExtention), "NULL").ToString();
            string regValueDeviceNumber = regKey.GetValue(nameof(this.DeviceNumber), "NULL").ToString();
            string regValueSensitivity = regKey.GetValue(nameof(this.Sensitivity), "NULL").ToString();

            if (regValueDirectoryPath != "NULL")
                this.DirectoryPath = regValueDirectoryPath;

            if (regValueFileExtention != "NULL")
                this.FileExtention = regValueFileExtention;

            if (regValueDeviceNumber != "NULL")
                this.DeviceNumber = Convert.ToInt32(regValueDeviceNumber);

            if (regValueSensitivity != "NULL")
                this.Sensitivity = Convert.ToInt32(regValueSensitivity);

            regKey.Close();
        }

        public void Write()
        {
            RegistryKey regKey = Registry.CurrentUser.CreateSubKey("SoundMonitor");

            regKey.SetValue(nameof(this.Sensitivity), this.Sensitivity.ToString());
            regKey.SetValue(nameof(this.DeviceNumber), this.DeviceNumber.ToString());
            regKey.SetValue(nameof(this.DirectoryPath), this.DirectoryPath.ToString());
            regKey.SetValue(nameof(this.FileExtention), this.FileExtention.ToString());

            regKey.Close();
        }

        private void SetDefaultValues()
        {
            this.DirectoryPath = DefaultValues.DirectoryPath;
            this.FileExtention = DefaultValues.FileExtention;
            this.DeviceNumber = DefaultValues.DeviceNumber;
            this.Sensitivity = DefaultValues.Sensitivity;
        }
    }
}
