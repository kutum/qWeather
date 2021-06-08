﻿using System;
using System.Web.Configuration;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using qWeather.Models.ESP8266;

namespace qWeather.Models
{
    /// <summary>
    /// Телеграм бот получения данных с датчиков
    /// </summary>
    public class TelegramBot
    {
        /// <summary>
        /// Инициализация телеграм бота по ключу
        /// </summary>
        public TelegramBotClient Bot = new TelegramBotClient(WebConfigurationManager.AppSettings["TelegramBotKey"]);

        /// <summary>
        /// Данные с контроллера
        /// </summary>
        private ESPData ESPData = new ESPData();

        /// <summary>
        /// 
        /// </summary>
        private Logging logging = new Logging();

        /// <summary>
        /// 
        /// </summary>
        private readonly Uri ESPServiceUrl = new Uri(WebConfigurationManager.AppSettings["espServiceUrl"]);

        /// <summary>
        /// Обработка сообщений посылаемых боту
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="messageEventArgs"></param>
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
                default: CommandStart(message.Chat.Id);
                    break;
            }
        }

        /// <summary>
        /// Отображение кнопок по команде "старт"
        /// </summary>
        /// <param name="Id">Id получателя</param>
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

        /// <summary>
        /// Остановка бота
        /// </summary>
        /// <param name="Id">Id получателя</param>
        private void CommandEnd (long Id)
        {
            Bot.SendTextMessageAsync(Id, "Stop Recived");
        }

        /// <summary>
        /// Температура снаружи
        /// </summary>
        /// <param name="Id">Id получателя</param>
        private async void CommandOutside(long Id)
        {
            try
            {
                ESPData = await ESPData.GetAsync(ESPServiceUrl);
                await Bot.SendTextMessageAsync(Id, "<code>outside:</code> <b>" + ESPData.variables.T_OUT + "°C</b>\n", parseMode: ParseMode.Html);
            }
            catch (Exception ex)
            {
                logging.WriteLog(ex);
                await Bot.SendTextMessageAsync(Id, "<code>" + ex.Message + "\n" + ex.InnerException + "</code>", parseMode: ParseMode.Html);
            }

        }

        /// <summary>
        /// Температура комнатная
        /// </summary>
        /// <param name="Id">Id получателя</param>
        private async void CommandInside(long Id)
        {
            try
            {
                ESPData = await ESPData.GetAsync(ESPServiceUrl);
                await Bot.SendTextMessageAsync(Id, "<code>inside:</code> <b>" + ESPData.variables.T_IN + "°C</b>\n", parseMode: ParseMode.Html);

            }
            catch (Exception ex)
            {
                logging.WriteLog(ex);
                await Bot.SendTextMessageAsync(Id, "<code>" + ex.Message + "\n" + ex.InnerException + "</code>", parseMode: ParseMode.Html);
            }
        }

        /// <summary>
        /// Влажность
        /// </summary>
        /// <param name="Id">Id получателя</param>
        private async void CommandHumidity(long Id)
        {
            try 
            {
                ESPData = await ESPData.GetAsync(ESPServiceUrl);
                await Bot.SendTextMessageAsync(Id, "<code>humidity:</code> <b>" + ESPData.variables.Humidity + "%</b>", parseMode: ParseMode.Html);
            }
            catch (Exception ex)
            {
                logging.WriteLog(ex);
                await Bot.SendTextMessageAsync(Id, "<code>" + ex.Message + "\n" + ex.InnerException + "</code>", parseMode: ParseMode.Html);
            }
        }
    }
}