using System;
using System.Globalization;
using System.IO;

namespace qWeather.Models
{
    /// <summary>
    /// Запись в лог
    /// </summary>
    public class Logging
    {
        /// <summary>
        /// Путь к файлу
        /// </summary>
        public string Filepath
        {
            get => Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
                                    "Logs",
                                    "Log_" + DateTime.Now.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture) + ".txt"
                                    );
            set => Filepath = this.Filepath;
        }
        
        /// <summary>
        /// Путь к папке
        /// </summary>
        public string FolderPath
        {
            get => Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
                                    "Logs");
            set => FolderPath = this.FolderPath;
        }

        /// <summary>
        /// Дата и время записи в лог-файл
        /// </summary>
        private string DateTimeLog
        {
            get => "#" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
            set => DateTimeLog = this.DateTimeLog;
        }

        /// <summary>
        /// Запись текста в лог
        /// </summary>
        /// <param name="Message"></param>
        public void WriteLog(string Message)
        {
            try
            {
                if (!Directory.Exists(FolderPath))
                    Directory.CreateDirectory(FolderPath);

                if (!File.Exists(Filepath))
                    File.Create(Filepath);

                File.AppendAllText(Filepath, DateTimeLog + ": " + Message);
            }
            catch (Exception ex)
            {
                throw new Exception("Logging.Writelog(string Message) " + ex.Message + "###" + ex.InnerException);
            }
        }

        /// <summary>
        /// Запись одной или более строк в лог
        /// </summary>
        /// <param name="Message"></param>
        public void WriteLog(string [] Message)
        {
            try
            {
                if (!Directory.Exists(FolderPath))
                    Directory.CreateDirectory(FolderPath);

                if (!File.Exists(Filepath))
                    File.Create(Filepath);

                Message[0] = DateTimeLog + ": " + Message[0];
                File.AppendAllLines(Filepath, Message);
            }
            catch (Exception ex)
            {
                throw new Exception("Logging.Writelog(string [] Message) " + ex.Message + "###" + ex.InnerException);
            }
        }
    }
}