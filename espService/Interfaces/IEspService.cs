using qWeather.Models.ESP8266;
using System.ServiceModel;
using System.ServiceModel.Web;

namespace espService.Interfaces
{
    /// <summary>
    /// Интерфейс класса сервиса опроса контроллера
    /// </summary>
    [ServiceContract]
    public interface IEspService
    {
        /// <summary>
        /// Статус доступности контроллера
        /// </summary>
        /// <returns>Статус</returns>
        [OperationContract]
        [WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Json)]
        string Status();

        [OperationContract]
        [WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Json)]
        ESPData Now();
    }
}
