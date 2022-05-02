using Newtonsoft.Json;
using qWeather.Models.ESP8266.Interfaces;
using System;
using System.Globalization;
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
        public ESPData Get(Uri URL)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = URL;
                    client.Timeout = new TimeSpan(0, 1, 0);
                    client.DefaultRequestHeaders.Clear();

                    HttpResponseMessage Res = Task.Run(async () => await client.GetAsync(URL.AbsoluteUri + "now")).Result;

                    if (Res.StatusCode != System.Net.HttpStatusCode.OK)
                        throw new Exception(Res.StatusCode.ToString());

                    string response = Res.Content.ReadAsStringAsync().Result;

                    if (string.IsNullOrEmpty(response))
                        throw new Exception("Response is empty!");

                    var parsedData = response.Split(',');

                    return new ESPData()
                    {
                        DateTime = DateTime.ParseExact(parsedData[0], "yyyy-MM-dd H:mm:ss", CultureInfo.InvariantCulture),
                        T_IN = float.Parse(parsedData[1].Replace('.',',')),
                        T_OUT = float.Parse(parsedData[2].Replace('.', ',')),
                        Humidity = float.Parse(parsedData[3].Replace('.', ','))
                    };
                    
                }
            }
            catch (Exception ex)
            {
                throw new Exception("ESPMethods.Get() " + ex.Message, ex.InnerException);
            };
        }

        public ESPData GetJson(Uri URL)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = URL;
                    client.Timeout = new TimeSpan(0, 1, 0);
                    client.DefaultRequestHeaders.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    HttpResponseMessage Res = Task.Run(async () => await client.GetAsync(URL)).Result;

                    string T_InResponse = Res.Content.ReadAsStringAsync().Result;

                    return JsonConvert.DeserializeObject<ESPData>(T_InResponse);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("ESPData.GetJson() " + ex.Message, ex.InnerException);
            };
        }

        public string GetString(Uri URL)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    client.Timeout = new TimeSpan(0, 5, 0);
                    client.DefaultRequestHeaders.Clear();

                    var task = Task.Run(async () => await client.GetAsync(URL));

                    HttpResponseMessage Res = task.Result;

                    if(Res.StatusCode != System.Net.HttpStatusCode.OK)
                        throw new Exception(Res.StatusCode.ToString());

                    return Res.Content.ReadAsStringAsync().Result;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("string ESPMethods.GetString() " + ex.Message, ex.InnerException);
            }
        }
    }
}