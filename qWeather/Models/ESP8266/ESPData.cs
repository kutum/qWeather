using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace qWeather.Models.ESP8266
{
    /// <summary>
    /// Класс данных с контроллера
    /// </summary>
    public class ESPData
    {
        /// <summary>
        /// Датчики
        /// </summary>
        public ESPSensors variables { get; set; }
        /// <summary>
        /// ID контроллера
        /// </summary>
        public int id { get; set; }
        /// <summary>
        /// Название контроллера
        /// </summary>
        public string name { get; set; }
        /// <summary>
        /// Наимеование чипа контроллера
        /// </summary>
        public string hardware { get; set; }
        /// <summary>
        /// Статус подключения
        /// </summary>
        public string connected { get; set; }

        /// <summary>
        /// Получение данных с датчика
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
                    client.Timeout = new TimeSpan (0, 5, 0);
                    client.DefaultRequestHeaders.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    HttpResponseMessage Res = await client.GetAsync("");

                    string T_InResponse = Res.Content.ReadAsStringAsync().Result;

                    return JsonConvert.DeserializeObject<ESPData>(T_InResponse);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("ESPData.GetAsync() "+ ex.Message, ex.InnerException);
            };
        }
    }
}