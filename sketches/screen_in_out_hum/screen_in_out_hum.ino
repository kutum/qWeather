#include <LiquidCrystal_I2C.h>                          //Библиотека поддержки дисплея A1602
#include "DHT.h"                                        //Библиотека для работы с датчиком температуры DHT11 (датчик на корпусе)
#include <OneWire.h>                                    //Библиотека шина связи для датчика DHT11
#include <DallasTemperature.h>                          //Библиотека для работы с датчиком температуры и влажности DS18B20              
#include <ESP8266WiFi.h>                                //Библиотека для подключения к WiFi 
#include <aREST.h>                                      //Библиотека для обработки REST запросов
#include <NTPClient.h>                                  //Библиотека для получения даты и времени через интернет
#include <WiFiUdp.h>                                    //Библиотека для работы с UDP-пакетами

#define DT 14                                           //Датчик на корпусе DHT11 
#define DHTPIN 2                                        //Выносной датчик температуры и влажности DS18B20 

OneWire oneWire_DT(DT);                                 //Инициализируем шину связи для DHT11
DallasTemperature DS18B20_DT(&oneWire_DT);              //Инициализируем датчик DS18B20
DHT dht(DHTPIN, DHT11);                                 //Инициализируем датчик DS18B20 на 2 пине
LiquidCrystal_I2C lcd(0x27, 16, 2);                     //Инициализируем экран с размером 16 "ячеек" в ширину, 2 в высоту с адресом 0x27
aREST rest = aREST();                                   //Инициализируем сервис REST для обработки запросов по API
WiFiUDP ntpUDP;                                         //Инициализируем обработчика UDP пакетов для получения и расшифровки даты и времени
NTPClient timeClient(ntpUDP,"pool.ntp.org");            //Инициализируем "клиента времени" который берёт дату и время с сервера pool.ntp.org
WiFiServer server(80);                                  //Инициализируем веб-сервер, на котором будет крутиться REST сервис

float T_IN;                                             //Переменная со значением температуры с DHT11 (комнатная)
float Humidity;                                         //Переменная со значением влажности
float T_OUT;                                            //Переменная со значением температуры с DS18B20

char ssid[] = "xiaomi_kutuck";                          //Имя WiFi сети к которой подключаемся
char pass[] = "myofrene";                               //Пароль WiFi сети

unsigned long timing;                                   //Переменная "времени", для выполнения операций циклично в определённое время
bool blinker = false;                                   //Переменная для "мигания" двоеточием на часах


byte sun[8] = {B00111, B00011, B00001, B01000, B11000, B01100, B01000, B11111};       //Символ "улица" для внешней температуры
byte house[8] = {B00100, B01010, B10001, B11111, B10001, B10101, B11111, B00000};     //Символ "домик" для комнатной температуры
byte humidity[8] = {B00000, B00100, B01110, B01110, B11111, B11111, B01110, B00000};  //Символ "капля" для влажности
byte celsium[8] = {B00001, B00000, B01110, B10000, B10000, B10000, B01110, B00000};   //Символ градуса цельсия 

void getTemperature() {                                 //Функция получения температуры с датчиков
  do {
    DS18B20_DT.requestTemperatures();                   //Запрос к датчику DT11
    T_IN = DS18B20_DT.getTempCByIndex(0);               //Записываем температуру в целисиях
  } while (T_IN == 85.0);

  Humidity = dht.readHumidity();                        //Получаем влажность с датчика DS18B20
  T_OUT = dht.readTemperature();                        //Получаем температуру с датчика DS18B20 
}

void setup(){                                           //Инициализация при запуске контроллера
  
  lcd.init();                                           //Включение дисплея
  lcd.backlight();                                      //Включение подсветки дисплея
  
  dht.begin();                                          //Инициализация датчика комнатной температуры
  DS18B20_DT.begin();                                   //Инициализация датчика улицы

  lcd.setCursor(0, 0);                                  //Задание начала "курсора" экрана (0(1) строка, 0(1) столбец)
  lcd.print("     ESP8266    ");                        //Вывод текста на экран
  lcd.setCursor(0, 1);
  lcd.print("  METEOSTATION  ");
  delay(5000);                                          //Задержка
  
  lcd.setCursor(0, 0);                                
  lcd.print("CONNECTING WIFI ");                        
  lcd.setCursor(0, 1);
  lcd.print(ssid);                                      //Вывод на экран названия сети WiFi, к которой подлючаемся
  lcd.print(" ");
  delay(2000);
  
  WiFi.begin(ssid, pass);                               //Инициализируем подключение к сети с именем ssid и паролем pass
  while (WiFi.status() != WL_CONNECTED){                //Пытаемся подключиться к сети, если не выходит печатаем точку каждые 0,1с на каждую попытку                
        lcd.print(".");
        delay(100);
      }
      
  lcd.setCursor(0, 0);
  lcd.print(" WIFI CONNECTED ");                        //Если подключились сигнализируем об этом 
  delay(1000);
  lcd.clear();                                          //Очищаем дисплей
  
                                                              //Инициализация переменных для REST API  
  rest.variable("T_IN", &T_IN);                               //Внутренняя температура                                                                        
  rest.variable("T_OUT", &T_OUT);                             //Наружняя температура
  rest.variable("Humidity", &Humidity);                       //Влажность воздуха
  rest.set_id("1");                                           //ID контроллера (на случай если используется больше одного, нужно их пронумеровать для удобства                                             
  rest.set_name("esp8266");                                   //Имя контроллера
  
  server.begin();                                             //Запускаем веб-сервер             
                      
  lcd.setCursor(0, 0);                                
  lcd.print("REST STARTED");
  lcd.setCursor(0, 1);
  lcd.print(WiFi.localIP());                                  //Выводим присвоенный IP адрес веб-сервера, по которому можно будет обратиться к контроллеру
  delay(3000);
  lcd.clear();

  timeClient.begin();                                         //Запуск клиента даты и времени
  timeClient.setTimeOffset(14400);                            //Установка часового пояса (в данном случае GMT+4, где одна единица GMT равна 3600 итого 3600*4=14400)
  
  lcd.createChar(1, sun);                                     //Создаём символ "улица"
  lcd.createChar(2, house);                                   //Создаём символ "дом"
  lcd.createChar(3, humidity);                                //Создаём символ "капля"
  lcd.createChar(4, celsium);                                 //Создаём символ "градус цельсия"
}

void loop(){                                                  //Функция, которая вызывается циклично на каждый такт процессора

  writeLCD();                                                 //Функция вывода показаний на экран                     
  restapi();                                                  //Функция обработки REST API запросов
                                            
  //delay(500);                                                 //Общий такт контроллера - 0,5 секунды.
}

void writeLCD()                                               //Функция вывода значений на экран
{
  getTemperature();                                           //Получаем показания с датчиков в глобальные переменные
  timeClient.update();                                        //Обновляем показания даты и времени от сервера в сети

  int hh = timeClient.getHours();                             //Часов
  int mm = timeClient.getMinutes();                           //Минут
 
  lcd.setCursor(0, 0);
  if (hh<10){                                                 //Добавляем к часу впереди "0" если часов меньше 10
    lcd.print("0");
  }
  lcd.print(hh);                                              //Выводим час

  if(blinker == false){                                       //Этим условием заставляем мигать у часов двоеточие, чтобы создать иллюзию того что часы "идут"
      lcd.print(":");
      blinker = true;
   }
   else{
      lcd.print(" ");
      blinker = false;
   }

  if (mm<10){                                                 //Добавляем к минутам впереди "0" если минут меньше 10
    lcd.print("0");
  }
  lcd.print(mm);                                              //Выводим минуты
  lcd.print(" ");
  lcd.print(getDate());                                       //Получаем дату и сразу выводим её отступая на пробел от времени
  
  lcd.setCursor(0, 1);                                        //Переходим на вторую строку
  lcd.write(byte(3));                                         //Печатаем "каплю"
  lcd.print((int)Humidity);                                   //Печатаем процент влажности воздуха
  lcd.print("%");
  lcd.setCursor(6, 1);                                        
  lcd.write(byte(1));                                         //Печатаем символ "улица"
  lcd.print((int)T_OUT);                                      //Показатель температуры на улице
  if(T_OUT<10){                                               //Этим условием заставляем символ градуса цельсия "прилипнуть" к значению. Если цифра в числе одна, то печатаем в 8 ячейке и стираем в 9
    lcd.setCursor(8, 1);
    lcd.write(byte(4));
    lcd.print(" ");
  }
  else{
    lcd.setCursor(9, 1);                                     //Если две цифры в числе, то в 9
    lcd.write(byte(4));
  }
 
  lcd.setCursor(12, 1);
  lcd.write(byte(2));                                        //Печатаем символ "дом"
  lcd.print((int)T_IN);                                      //Печатаем температуру в комнате
  if(T_IN<10){                                               //Смотри аналогичное условие как и для внешней температуры
    lcd.setCursor(14, 1);
    lcd.write(byte(4));
    lcd.print(" ");
  }
  else{
    lcd.setCursor(15, 1);
    lcd.write(byte(4));
  }
}

void restapi(){                                             //функция обработки REST API запросов
  
    WiFiClient client = server.available();

    if (!client) {
      return;
    }
    
    if(!client.available()){
      lcd.setCursor(5, 0);
      lcd.print("S");
      return;
    }
    else
    {
      lcd.setCursor(5, 0);
      lcd.print(" ");
    }
    
    rest.handle(client);
}

String getDate() {                                          //Функция получения даты в формате ДД.ММ.ГГГГ
   time_t rawtime = timeClient.getEpochTime();              //Получаем дату
   struct tm * ti;
   ti = localtime (&rawtime);

   uint16_t year = ti->tm_year + 1900;
   String yearStr = String(year);

   uint8_t month = ti->tm_mon + 1;
   String monthStr = month < 10 ? "0" + String(month) : String(month);

   uint8_t day = ti->tm_mday;
   String dayStr = day < 10 ? "0" + String(day) : String(day);

   return dayStr + "." + monthStr + "." + yearStr/*.substring(2,4)*/;
}