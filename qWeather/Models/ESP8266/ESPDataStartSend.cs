using Quartz;
using Quartz.Impl;

namespace qWeather.Models.ESP8266
{
    public class ESPDataStartSend
    {
        public static async void Start()
        {
            IScheduler scheduler = await StdSchedulerFactory.GetDefaultScheduler();
            await scheduler.Start();

            IJobDetail job = JobBuilder.Create<ESPDataSender>().Build();

            ITrigger trigger = TriggerBuilder.Create()
                    .WithIdentity("trigger1", "group1")
                    .StartNow()
                    .WithSimpleSchedule(x => x.WithIntervalInMinutes(10).RepeatForever())
                    .Build();

            await scheduler.ScheduleJob(job, trigger);
        }
    }
}