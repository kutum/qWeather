#ifndef DEFINES
#define DEFINES

  #include <NTPClient.h>                                  //Библиотека для получения даты и времени через интернет
  #include <WiFiUdp.h>                                    //Библиотека для работы с UDP-пакетами
  #include <ESP8266WiFi.h>                                //Библиотека для подключения к WiFi 
  #include <ESP8266WebServer.h>  
  #include "date_functions.h"
  #include "file_functions.h"
  #include <ESP8266HTTPClient.h>
  #include <ESP8266HTTPUpdateServer.h>
  #include <LiquidCrystal_I2C.h>                          //Библиотека поддержки дисплея A1602
  #include "DHT.h"                                        //Библиотека для работы с датчиком температуры DHT11 (датчик на корпусе)
  #include <OneWire.h>                                    //Библиотека шина связи для датчика DHT11
  #include <DallasTemperature.h>                          //Библиотека для работы с датчиком температуры и влажности DS18B20                          

  #define DT 14                                           //Датчик на корпусе DHT11 
  #define DHTPIN 2                                        //Выносной датчик температуры и влажности DS18B20 
  #define OTAUSER         "admin"                         // Логин для входа в OTA
  #define OTAPASSWORD     "admin"                         // Пароль для входа в ОТА
  #define OTAPATH         "/firmware"                     // Путь, который будем дописывать после ip адреса в браузере.
  #define SERVERPORT      80                              // Порт для входа, он стандартный 80 это порт http
  
  WiFiUDP ntpUDP;                                         //Инициализируем обработчика UDP пакетов для получения и расшифровки даты и времени
  NTPClient timeClient(ntpUDP, "pool.ntp.org");           //Инициализируем "клиента времени" который берёт дату и время с сервера pool.ntp.org
  ESP8266WebServer server(SERVERPORT);                    //Инициализируем веб-сервер API
  LiquidCrystal_I2C lcd(0x27, 16, 2);                     //Инициализируем экран с размером 16 "ячеек" в ширину, 2 в высоту с адресом 0x27
  ESP8266HTTPUpdateServer httpUpdater;
  OneWire oneWire_DT(DT);                                 //Инициализируем шину связи для DHT11
  DallasTemperature DS18B20_DT(&oneWire_DT);              //Инициализируем датчик DS18B20
  DHT dht(DHTPIN, DHT11);                                 //Инициализируем датчик DS18B20 на 2 пине
  
  float T_IN;                                         //Переменная со значением температуры с DHT11 (комнатная)
  float Humidity;                                     //Переменная со значением влажности
  float T_OUT;                                        //Переменная со значением температуры с DS18B20

  char ssid[] = "xiaomi_kutuck";                          //Имя WiFi сети к которой подключаемся
  char pass[] = "myofrene";                               //Пароль WiFi сети
  unsigned long timing;                                   //Переменная "времени", для выполнения операций циклично в определённое время
  bool blinker = false;                                   //Переменная для "мигания" двоеточием на часах
  int lastInsert = timeClient.getMinutes();
  
  byte sun[8] = { B00111, B00011, B00001, B01000, B11000, B01100, B01000, B11111 };       //Символ "улица" для внешней температуры
  byte house[8] = { B00100, B01010, B10001, B11111, B10001, B10101, B11111, B00000 };     //Символ "домик" для комнатной температуры
  byte humidity[8] = { B00000, B00100, B01110, B01110, B11111, B11111, B01110, B00000 };  //Символ "капля" для влажности
  byte celsium[8] = { B00001, B00000, B01110, B10000, B10000, B10000, B01110, B00000 };   //Символ градуса цельсия
#endif
