using qWeather.Context;
using qWeather.Hubs;
using qWeather.Models;
using System;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;

namespace qWeather.SignalR
{
    public class SignalRepository
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string GetAllMessages(WeatherDbContext context, Logging logging)
        {
            var query = context.Weather as DbQuery<Weather>;
            var commandtext = query.ToString();
            var connectionString = context.Database.Connection.ConnectionString;

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    using (SqlCommand command = new SqlCommand(commandtext, connection))
                    {
                        connection.Open();

                        var sqlDependency = new SqlDependency(command);

                        sqlDependency.OnChange += new OnChangeEventHandler(dependency_OnChange);

                        // NOTE: You have to execute the command, or the notification will never fire.
                        using (SqlDataReader reader = command.ExecuteReader()) { }

                        return "DB changed";
                    }
                }
            }
            catch (Exception ex)
            {
                logging.WriteLog(ex);
                throw new Exception(ex.Message + " " + ex.InnerException);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dependency_OnChange(object sender, SqlNotificationEventArgs e)
        {
            if (e.Type == SqlNotificationType.Change && e.Info == SqlNotificationInfo.Insert)
            {
                WeathersHub.SendMessages();
            }
        }
    }
}