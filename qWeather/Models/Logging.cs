using System;
using System.Globalization;
using System.IO;

namespace qWeather.Models
{
    public class Logging
    {
        public string Filepath
        {
            get => Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
                                    "Logs",
                                    "Log_" + DateTime.Now.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture) + ".txt"
                                    );
            set => Filepath = this.Filepath;
        }

        public string FolderPath
        {
            get => Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
                                    "Logs");
            set => FolderPath = this.FolderPath;
        }

        private string DateTimeLog
        {
            get => "#" + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss");
            set => DateTimeLog = this.DateTimeLog;
        }

        public void WriteLog(string Message)
        {
            if (!Directory.Exists(FolderPath))
            {
                Directory.CreateDirectory(FolderPath);
            }

            if (!File.Exists(Filepath))
            {
                File.Create(Filepath);
            }

            File.AppendAllText(Filepath, DateTimeLog + ": " + Message);
        }

        public void WriteLog(string [] Message)
        {
            if (!Directory.Exists(FolderPath))
            {
                Directory.CreateDirectory(FolderPath);
            }

            if (!File.Exists(Filepath))
            {
                File.Create(Filepath);
            }

            Message[0] = DateTimeLog + ": " + Message[0];

            File.AppendAllLines(Filepath, Message);
        }
    }
}