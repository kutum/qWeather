using qWeather.Models.ESP8266;
using System;
using System.Collections.Generic;

namespace qWeather.Models
{
    public interface ILogging
    {
        void WriteLog(ESPData espdata);
        void WriteLog(Exception exception);
        void WriteLog(string Message);
        void WriteLog(string[] Message);
        void WriteLog(List<Weather> espdatas);
    }
}