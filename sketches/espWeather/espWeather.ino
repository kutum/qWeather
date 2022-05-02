#include "defines.h"

void getTemperature() {                                 //Функция получения температуры с датчиков
  do {
    DS18B20_DT.requestTemperatures();                   //Запрос к датчику DT11
    T_IN = DS18B20_DT.getTempCByIndex(0);               //Записываем температуру в целисиях
  } while (T_IN == 85.0);

  Humidity = dht.readHumidity();                        //Получаем влажность с датчика DS18B20
  T_OUT = dht.readTemperature();                        //Получаем температуру с датчика DS18B20
}

void setup() {                                          //Инициализация при запуске контроллера
  Serial.begin(115200);

  dht.begin();                                          //Инициализация датчика комнатной температуры
  DS18B20_DT.begin();                                   //Инициализация датчика улицы
  
  lcd.init();                                           //Включение дисплея
  lcd.backlight();                                      //Включение подсветки дисплея
  lcd.setCursor(0, 0);                                  //Задание начала "курсора" экрана (0(1) строка, 0(1) столбец)
  lcd.print("     ESP8266    ");                        //Вывод текста на экран
  lcd.setCursor(0, 1);
  lcd.print("  METEOSTATION  ");
  delay(5000);                                          //Задержка

  lcd.clear();
  lcd.setCursor(0, 0);
  lcd.print("CONNECTING WIFI ");
  lcd.setCursor(0, 1);
  lcd.print(ssid);                                      //Вывод на экран названия сети WiFi, к которой подлючаемся
  lcd.print(" ");
  delay(2000);

  WiFi.mode(WIFI_STA); 
  WiFi.begin(ssid, pass);                               //Инициализируем подключение к сети с именем ssid и паролем pass
  while (WiFi.status() != WL_CONNECTED) {                //Пытаемся подключиться к сети, если не выходит печатаем точку каждые 0,1с на каждую попытку
    lcd.print(".");
    delay(1000);
  }

  lcd.setCursor(0, 0);
  lcd.print(" WIFI CONNECTED ");                        //Если подключились сигнализируем об этом
  delay(1000);
  lcd.clear();                                          //Очищаем дисплей
  
  timeClient.begin();                                         //Запуск клиента даты и времени
  timeClient.setTimeOffset(10800);                            //Установка часового пояса (в данном случае GMT+3, где одна единица GMT равна 3600 итого 3600*3=10800)
 
  if (!SPIFFS.begin()) {
    Serial.println("Error mounting the file system");
    return;
  }
  
  lcd.setCursor(0, 0);
  lcd.print("DEVICE IP");
  lcd.setCursor(0, 1);
  lcd.print(WiFi.localIP());                                  //Выводим присвоенный IP адрес веб-сервера, по которому можно будет обратиться к контроллеру
  delay(3000);
  lcd.clear();

  lcd.createChar(1, sun);                                     //Создаём символ "улица"
  lcd.createChar(2, house);                                   //Создаём символ "дом"
  lcd.createChar(3, humidity);                                //Создаём символ "капля"
  lcd.createChar(4, celsium);                                 //Создаём символ "градус цельсия"
  
  //FOR DEBUGGING
  //removeAllFiles();

  server.on("/fdata", [](){  
    String _date = server.arg("date");  
    File file = SPIFFS.open(_date, "r");
    server.streamFile(file, "text/plain");    
    file.close();   
  });

  server.on("/files", [](){
      server.send(200, "text/plain", files());
  });

  server.on("/delete", [](){
    String _name = server.arg("name");
    server.send(200, "text/plain", removeFile(_name));
  });

  server.on("/now", [](){
    String date = convertTimeToString(timeClient.getEpochTime());
    String _data = date+" "+timeClient.getFormattedTime()+","+String(T_IN)+","+String(T_OUT)+","+String(Humidity);
    server.send(200, "text/plain", _data);
  });

  httpUpdater.setup(&server, OTAPATH, OTAUSER, OTAPASSWORD);
  server.begin();
}

void loop() {
    if (WiFi.status() == WL_CONNECTED)
    {
      timeClient.update();                                        //Обновляем показания даты и времени от сервера в сети   
      getTemperature();                                           //Получаем показания с датчиков в глобальные переменные                                    
      writeLCD(convertTimeToString(timeClient.getEpochTime()), 
                                   timeClient.getHours(), 
                                   timeClient.getMinutes());      //Функция вывода показаний на экран 
      
      int minutes = timeClient.getMinutes();
      if((minutes == 30 || minutes == 0) && minutes != lastInsert) {
            String message = timeClient.getFormattedTime()+","+String(T_IN)+","+String(T_OUT)+","+String(Humidity)+";";
            String date = convertTimeToString(timeClient.getEpochTime());
            writeFile(message, date);
            lastInsert = minutes;
      }
      server.handleClient();
    }
    else {
          lcd.clear();
          lcd.setCursor(0, 0);
          lcd.print("WIFI ERROR!");
          lcd.setCursor(0, 1);
          delay(3000);
    }
  //delay(1000);
}

void writeLCD(String date, int hh, int mm){                    //Функция вывода значений на экран

  lcd.setCursor(0, 0);
  if (hh < 10) {                                               //Добавляем к часу впереди "0" если часов меньше 10
    lcd.print("0");
  }
  lcd.print(hh);                                               //Выводим час

  if (blinker == false) {                                      //Этим условием заставляем мигать у часов двоеточие, чтобы создать иллюзию того что часы "идут"
    lcd.print(":");
    blinker = true;
  }
  else {
    lcd.print(" ");
    blinker = false;
  }

  if (mm < 10) {                                              //Добавляем к минутам впереди "0" если минут меньше 10
    lcd.print("0");
  }
  lcd.print(mm);                                              //Выводим минуты
  lcd.print(" ");
  lcd.print(date);  //Получаем дату и сразу выводим её отступая на пробел от времени
 
  lcd.setCursor(0, 1);                                        //Переходим на вторую строку
  lcd.write(byte(3));                                         //Печатаем "каплю"
  lcd.print((int)Humidity);                                   //Печатаем процент влажности воздуха
  lcd.print("%");
  lcd.setCursor(6, 1);
  lcd.write(byte(1));                                         //Печатаем символ "улица"
  lcd.print((int)T_OUT);                                      //Показатель температуры на улице

  if ((int)T_OUT >= 0) {
    if ((int)T_OUT < 10) {                                               //Этим условием заставляем символ градуса цельсия "прилипнуть" к значению. Если цифра в числе одна, то печатаем в 8 ячейке и стираем в 9
      lcd.setCursor(8, 1);
      lcd.write(byte(4));
      lcd.print(" ");
    }
    else {
      lcd.setCursor(9, 1);                                     //Если две цифры в числе, то в 9
      lcd.write(byte(4));
    }
  }
  else {
    if ((int)T_OUT > -10) {                                               //Этим условием заставляем символ градуса цельсия "прилипнуть" к значению. Если цифра в числе одна, то печатаем в 9 ячейке и стираем в 10
      lcd.setCursor(9, 1);
      lcd.write(byte(4));
      lcd.print(" ");
    }
    else {
      lcd.setCursor(10, 1);                                     //Если две цифры в числе, то в 10
      lcd.write(byte(4));
    }
  }

  lcd.setCursor(12, 1);
  lcd.write(byte(2));                                        //Печатаем символ "дом"
  lcd.print((int)T_IN);                                      //Печатаем температуру в комнате

  if (T_IN < 10) {                                               //Смотри аналогичное условие как и для внешней температуры
    lcd.setCursor(14, 1);
    lcd.write(byte(4));
    lcd.print(" ");
  }
  else {
    lcd.setCursor(15, 1);
    lcd.write(byte(4));
  }
}
