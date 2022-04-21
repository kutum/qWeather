using Newtonsoft.Json;
using qWeather.Models.ESP8266.Interfaces;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace qWeather.Models.ESP8266
{
    public class ESPMethods : IESPMethods
    {
        /// <summary>
        /// Получение данных с датчика асинхронно
        /// </summary>
        /// <param name="URL"></param>
        /// <returns></returns>
        public async Task<ESPData> GetAsync(Uri URL)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = URL;
                    client.Timeout = new TimeSpan(0, 1, 0);
                    client.DefaultRequestHeaders.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    HttpResponseMessage Res = await client.GetAsync("");

                    string T_InResponse = Res.Content.ReadAsStringAsync().Result;

                    return JsonConvert.DeserializeObject<ESPData>(T_InResponse);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("ESPData.GetAsync() " + ex.Message, ex.InnerException);
            };
        }

        /// <summary>
        /// Получение данных с датчика синхронно
        /// </summary>
        /// <param name="URL"></param>
        /// <returns></returns>
        public ESPData Get(Uri URL)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = URL;
                    client.Timeout = new TimeSpan(0, 5, 0);
                    client.DefaultRequestHeaders.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    var task = Task.Run(async () => await client.GetAsync(""));

                    HttpResponseMessage Res = task.Result;

                    string T_InResponse = Res.Content.ReadAsStringAsync().Result;

                    return JsonConvert.DeserializeObject<ESPData>(T_InResponse);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("ESPData.GetAsync() " + ex.Message, ex.InnerException);
            };
        }
    }
}