using qWeather.Models;
using System;

namespace TelegramService
{
    static class Program
    {
        private static readonly Logging logging = new Logging();
        private static TelegramBot telegramBot = new TelegramBot();
        private static readonly TelegramService telegramService = new TelegramService(logging, telegramBot);

        static void Main()
        {
            if (Environment.UserInteractive)
            {
                try
                {
                    logging.WriteLog("Starting telegramBot as app");

                    telegramService.OnDebug();

                    Console.WriteLine("Press any key to stop");
                    Console.ReadKey();

                    telegramService.Stop();

                    logging.WriteLog("Stopped telegramBot");
                }
                catch (Exception ex)
                {
                    logging.WriteLog(ex.Message);
                    throw new Exception(ex.Message + " " + ex.InnerException);
                }
            }
            else
            {
                try
                {
                    logging.WriteLog("Starting telegramBot as service");

                    telegramService.OnDebug();

                    System.Threading.Thread.Sleep(System.Threading.Timeout.Infinite);

                    telegramService.Stop();

                    logging.WriteLog("Stopped telegramBot");
                }
                catch (Exception ex)
                {
                    logging.WriteLog(ex.Message);
                    throw new Exception(ex.Message + " " + ex.InnerException);
                }
            }
        }
    }
}
