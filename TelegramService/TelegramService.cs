using System;
using System.ServiceProcess;
using System.Timers;
using qWeather.Models;

namespace TelegramService
{
    partial class TelegramService : ServiceBase
    {
        private Logging logging;
        private Timer timer;
        private TelegramBot telegramBot;

        public TelegramService(Logging _logging, TelegramBot _telegramBot)
        {
            InitializeComponent();
            timer = new Timer { Interval = TimeSpan.FromMinutes(0.5).TotalMilliseconds };
            logging = _logging;
            telegramBot = _telegramBot;
        }

        protected override void OnStart(string[] args)
        {
            logging.WriteLog("Service started");
            telegramBot.Bot.OnMessage += telegramBot.Bot_OnMessage;
            telegramBot.Bot.StartReceiving();

        }

        protected override void OnStop()
        {
            timer.Stop();
            logging.WriteLog("Service stopped");
        }

        public void OnDebug()
        {
            OnStart(null);
        }
    }
}
