using System;
using System.Web.Configuration;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using qWeather.Models.ESP8266;

namespace qWeather.Models
{
    public class TelegramBot
    {
        public TelegramBotClient Bot = new TelegramBotClient(WebConfigurationManager.AppSettings["TelegramBotKey"]);

        private ESPData ESPData = new ESPData();

        public void Bot_OnMessage(object sender, MessageEventArgs messageEventArgs)
        {
            var message = messageEventArgs.Message;

            if (message == null || message.Type != MessageType.Text) 
                return;

            switch (message.Text)
            {
                case "/start" :
                    CommandStart(message.Chat.Id);
                    break;
                case "/stop":
                    CommandEnd(message.Chat.Id);
                    break;
                case "outside":
                    CommandOutside(message.Chat.Id);
                    break;
                case "inside":
                    CommandInside(message.Chat.Id);
                    break;
                case "humidity":
                    CommandHumidity(message.Chat.Id);
                    break;
            }
        }

        private void CommandStart (long Id)
        {
            var keyboard = new ReplyKeyboardMarkup
            {
                Keyboard = new[]
                {
                    new[]
                    {
                         new KeyboardButton("outside"),
                         new KeyboardButton("inside"),
                         new KeyboardButton("humidity")
                    },
                },
                ResizeKeyboard = true
            };

            Bot.SendTextMessageAsync(Id, "choose", replyMarkup: keyboard);
        }

        private void CommandEnd (long Id)
        {
            Bot.SendTextMessageAsync(Id, "Stop Recived");
        }

        private async void CommandOutside(long Id)
        {
            ESPData = await ESPData.GetAsync(new Uri(WebConfigurationManager.AppSettings["ESP8266url"]));
            await Bot.SendTextMessageAsync(Id, "<code>outside:</code> <b>" + ESPData.variables.T_OUT + "°C</b>\n", parseMode: ParseMode.Html);
        }

        private async void CommandInside (long Id)
        {
            ESPData = await ESPData.GetAsync(new Uri(WebConfigurationManager.AppSettings["ESP8266url"]));
            await Bot.SendTextMessageAsync(Id, "<code>inside:</code> <b>" + ESPData.variables.T_IN + "°C</b>\n", parseMode: ParseMode.Html);
        }

        private async void CommandHumidity(long Id)
        {
            ESPData = await ESPData.GetAsync(new Uri(WebConfigurationManager.AppSettings["ESP8266url"]));
            await Bot.SendTextMessageAsync(Id, "<code>humidity:</code> <b>" + ESPData.variables.Humidity + "%</b>", parseMode: ParseMode.Html);
        }
    }
}