using Quartz;
using Quartz.Impl;
using System;
using System.Web.Configuration;

namespace qWeather.Models.ESP8266
{
    public class ESPDataStartSend
    {
        public static async void Start()
        {
            IScheduler scheduler = await StdSchedulerFactory.GetDefaultScheduler();
            await scheduler.Start();

            IJobDetail job = JobBuilder.Create<ESPDataSender>().Build();

            var interval = int.Parse(WebConfigurationManager.AppSettings["InsertIntervalMinutes"]);

            ITrigger trigger = TriggerBuilder.Create()
                    .WithIdentity("trigger1", "group1")
                    .StartNow()
                    .WithSimpleSchedule(x => x.WithIntervalInMinutes(interval).RepeatForever())
                    .Build();

            await scheduler.ScheduleJob(job, trigger);
        }
    }
}