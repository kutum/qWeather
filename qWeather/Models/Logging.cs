using qWeather.Models.ESP8266;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace qWeather.Models
{
    /// <summary>
    /// Запись в лог
    /// </summary>
    public class Logging : ILogging
    {
        /// <summary>
        /// Конструктор класса записи в лог
        /// </summary>
        public Logging()
        {
            if (!Directory.Exists(FolderPath))
                Directory.CreateDirectory(FolderPath);
        }


        /// <summary>
        /// Путь к файлу
        /// </summary>
        public string Filepath
        {
            get => Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
                                    "Logs",
                                    "Log_" + DateTime.Now.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture) + ".txt"
                                    );
            set => Filepath = Filepath;
        }
        
        /// <summary>
        /// Путь к папке
        /// </summary>
        public string FolderPath
        {
            get => Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
                                    "Logs");
            set => FolderPath = FolderPath;
        }

        /// <summary>
        /// Дата и время записи в лог-файл
        /// </summary>
        private string DateTimeLog
        {
            get => "#" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
            set => DateTimeLog = DateTimeLog;
        }

        /// <summary>
        /// Запись текста в лог
        /// </summary>
        /// <param name="espdata"></param>
        public void WriteLog(ESPData espdata)
        {
            try
            {
                var Message = new string[]
                {
                    DateTimeLog + "T_OUT: " + espdata.T_OUT.ToString(),
                    "T_IN: " + espdata.T_IN.ToString(),
                    "humidity: " + espdata.Humidity.ToString()
                };

                WriteStream(Message);
            }
            catch (Exception ex)
            {
                throw new Exception("Logging.Writelog(ESPData eSPData) " + ex.Message + "###" + ex.InnerException);
            }
        }

        /// <summary>
        /// Запись текста в лог
        /// </summary>
        /// <param name="ex"></param>
        public void WriteLog(Exception exception)
        {
            try
            {
                var Message = new string[]
                { 
                    DateTimeLog + " " + exception.Message,
                    exception?.InnerException?.Message
                };

                WriteStream(Message);
            }
            catch (Exception ex)
            {
                throw new Exception("Logging.Writelog(Exception exception) " + ex.Message + "###" + ex.InnerException);
            }
        }

        /// <summary>
        /// Запись текста в лог
        /// </summary>
        /// <param name="Message"></param>
        public void WriteLog(string Message)
        {
            try
            {
                WriteStream(new string[] { DateTimeLog + " " + Message });
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
                Message[0] = DateTimeLog + ": " + Message[0];
                WriteStream(Message);
            }
            catch (Exception ex)
            {
                throw new Exception("Logging.Writelog(string [] Message) " + ex.Message + "###" + ex.InnerException);
            }
        }

        public void WriteLog(List<Weather> espdatas)
        {
            try
            {
                string[] data = new string[espdatas.Count];

                for (var i = 0; i < espdatas.Count; i++)
                {
                    data[i] = espdatas[i].DATETIME + "," + espdatas[i].VAL2 + "," + espdatas[i].VAL1 + "," + espdatas[i].HUMIDITY;
                }
             
                WriteStream(data);
            }
            catch(Exception ex)
            {
                throw new Exception("Logging.Writelog(List<Weather> espdatas) " + ex.Message + "###" + ex.InnerException);
            }
        }

        /// <summary>
        /// Запись сообщения в файл
        /// </summary>
        /// <param name="message"></param>
        private void WriteStream(string [] message)
        {
            try
            {
                if (!File.Exists(Filepath))
                {
                    using (var streamCreate = File.CreateText(Filepath))
                    {
                        foreach (var row in message)
                        {
                            streamCreate.WriteLine(row);
                        }
                    }
                }
                else
                {
                    using (var streamWriter = File.AppendText(Filepath))
                    {
                        foreach (var row in message)
                        {
                            streamWriter.WriteLine(row);
                        }
                    }
                }

                if (Environment.UserInteractive)
                {
                    foreach (var row in message)
                    {
                        Console.WriteLine(row);
                    }
                }
                
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message + " " + ex.InnerException);
            }
        }
    }
}