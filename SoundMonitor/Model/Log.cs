using System;
using System.Diagnostics;
using System.IO;

namespace SoundMonitor.Model
{
    public class Log
    {
        private string lastWriteString;
        private string fileName;
        private readonly Settings Settings;

        public Log(Settings settings)
        {
            this.Settings = settings;
            this.lastWriteString = string.Empty;
            this.fileName = DateTime.Now.ToString("yyyy-MM-dd");
        }

        public void Write()
        {
            if (!Directory.Exists(this.Settings.DirectoryPath))
                Directory.CreateDirectory(this.Settings.DirectoryPath);

            string currentDate = DateTime.Now.ToString("yyyy-MM-dd");

            if (this.fileName != currentDate)
                this.fileName = currentDate;

            string fullPath = this.Settings.DirectoryPath + "\\" + this.fileName + this.Settings.FileExtention;

            if (!File.Exists(fullPath))
                File.Create(fullPath);

            try
            {
                string currentWriteString = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");

                using (StreamWriter sw = new StreamWriter(fullPath, true, System.Text.Encoding.Default))
                {
                    sw.WriteLine(currentWriteString);
                    sw.Close();
                }

                this.lastWriteString = currentWriteString;
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }
        }
    }
}
