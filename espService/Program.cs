using espService.Interfaces;
using qWeather.Models;
using System;
using System.Configuration;
using System.Runtime.InteropServices;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.ServiceModel.Web;

namespace espService
{
    class Program
    {
        /// <summary>
        /// Сервис получения данных с датчиков
        /// </summary>
        private static readonly EspService EspService = new EspService();

        /// <summary>
        /// Логгирование
        /// </summary>
        private static readonly Logging logging = new Logging();

        /// <summary>
        /// HTTP адрес сервиса
        /// </summary>
        private static readonly Uri url = new Uri(ConfigurationManager.AppSettings["ServiceUrl"]);

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main()
        {       
            if (Environment.UserInteractive)
            {
                Console.Title = "espService";

                logging.WriteLog("Starting as application");

                WebServiceHost host = new WebServiceHost(typeof(EspService), url);
                try
                {
                    ServiceEndpoint ep = host.AddServiceEndpoint(typeof(IEspService), new WebHttpBinding(), "");

                    host.Open();

                    using (ChannelFactory<IEspService> channelFactory = new ChannelFactory<IEspService>(new WebHttpBinding(), url.ToString()))
                    {
                        channelFactory.Endpoint.Behaviors.Add(new WebHttpBehavior());

                        IEspService channel = channelFactory.CreateChannel();                        

                        new ConsoleControl(AppConsoleCloseHandler);
                        EspService.OnDebug();
                    }

                    Console.WriteLine("Press any key to stop");
                    Console.ReadKey();

                    host.Close();
                    EspService.Stop();
                }
                catch (CommunicationException ex)
                {
                    logging.WriteLog(ex);
                    host.Abort();
                    throw new Exception(ex.Message + " " + ex.InnerException);
                }
            }
            else
            {
                logging.WriteLog("Starting as service");

                WebServiceHost host = new WebServiceHost(typeof(EspService), url);
                try
                {
                    ServiceEndpoint ep = host.AddServiceEndpoint(typeof(IEspService), new WebHttpBinding(), "");

                    host.Open();

                    using (ChannelFactory<IEspService> channelFactory = new ChannelFactory<IEspService>(new WebHttpBinding(), url.ToString()))
                    {
                        channelFactory.Endpoint.Behaviors.Add(new WebHttpBehavior());

                        IEspService channel = channelFactory.CreateChannel();

                        EspService.OnDebug();
                        System.Threading.Thread.Sleep(System.Threading.Timeout.Infinite);
                    }
                }
                catch (CommunicationException ex)
                {
                    logging.WriteLog(ex);
                    host.Abort();
                    throw new Exception(ex.Message + " " + ex.InnerException);
                }
            }
        }

        public static void AppConsoleCloseHandler(ConsoleControl.ConsoleEvent consoleEvent)
        {
            EspService.Stop();
            Environment.Exit(-1);
        }
    }

    class ConsoleControl
    {
        public delegate void ConsoleCloseEventHandler(ConsoleEvent consoleEvent);

        [DllImport("kernel32.dll")]
        static extern bool SetConsoleCtrlHandler(ConsoleCloseEventHandler e, bool add);

        public enum ConsoleEvent
        {
            CtrlC = 0, CtrlBreak = 1, CtrlClose = 2, CtrlLogoff = 5, CtrlShutdown = 6
        }

        public ConsoleControl(ConsoleCloseEventHandler consoleCloseEvent)
        {
            SetConsoleCtrlHandler(consoleCloseEvent, true);
        }
    }
}
