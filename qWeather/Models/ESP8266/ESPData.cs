using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace qWeather.Models.ESP8266
{
    public class ESPData
    {
        public ESPSensors variables { get; set; }
        public int id { get; set; }
        public string name { get; set; }
        public string hardware { get; set; }
        public string connected { get; set; }

        private ESPData _ESPData { get; set; }

        public async Task<ESPData> GetAsync(Uri URL)
        {
            
            using (var client = new HttpClient())
            {
                client.BaseAddress = URL; 
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                HttpResponseMessage Res = await client.GetAsync("");

                string T_InResponse = Res.Content.ReadAsStringAsync().Result;
                _ESPData = JsonConvert.DeserializeObject<ESPData>(T_InResponse);
            }

            return _ESPData;
        }

    }
}