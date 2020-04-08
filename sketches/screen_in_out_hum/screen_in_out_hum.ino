// Include LIBS
#include <LiquidCrystal_I2C.h> //LCD lib
#include "DHT.h"  //DHT11 lib
#include <OneWire.h> 
#include <DallasTemperature.h> //DT802 lib
#include <MySQL_Connection.h> //SQL Connection lib
#include <MySQL_Cursor.h> //SQL query lib
#include <ESP8266WiFi.h> //WIFI ESP8266 lib
#include <WiFiClient.h> //WIFI Client lib
#include <aREST.h>
#include <NTPClient.h>
#include <WiFiUdp.h>

#define DT 14 //IN Temperature
#define DHTPIN 2 //OUT Temperature
#define DHTTYPE DHT11 

DHT dht(DHTPIN, DHTTYPE);
OneWire oneWire_DT(DT);
DallasTemperature DS18B20_DT(&oneWire_DT);

LiquidCrystal_I2C lcd(0x27, 16, 2); 
 
aREST rest = aREST(); 

WiFiUDP ntpUDP;
//NTPClient timeClient(ntpUDP, "europe.pool.ntp.org", 3600, 60000);
NTPClient timeClient(ntpUDP,"pool.ntp.org");
String date_time;

float T_IN;
float Humidity;
float T_OUT;

char ssid[] = "xiaomi_kutuck";                 // Network Name
char pass[] = "myofrene";                 // Network Password
byte mac[6];

WiFiServer server(80);
WiFiClient client;

MySQL_Connection conn((Client *)&client);
char query[128];
IPAddress mysql_ip(192, 168 ,1, 5);          // MySQL server IP
char mysql_user[] = "kutum";           // MySQL user
char mysql_password[] = "myofrene";       // MySQL password
unsigned long timing;
bool first;
bool blinker = false;


void getTemperature() {
  do {
    DS18B20_DT.requestTemperatures();
    T_IN = DS18B20_DT.getTempCByIndex(0); //Inside temperature
  } while (T_IN == 85.0);

  Humidity = dht.readHumidity(); //Humidity
  T_OUT = dht.readTemperature();   //Outside temperature
}

byte sun[8] = {B00111, B00011, B00001, B01000, B11000, B01100, B01000, B11111};
byte house[8] = {B00100, B01010, B10001, B11111, B10001, B10101, B11111, B00000};
byte humidity[8] = {B00000, B00100, B01110, B01110, B11111, B11111, B01110, B00000};
byte celsium[8] = {B00001, B00000, B01110, B10000, B10000, B10000, B01110, B00000};

void setup(){
  
  /*
   First initialize serial, lcd screen, DT11, DS18B20, WIFI Connection, SQL Connection
   */

  Serial.begin(115200);
  
  lcd.init();                  
  lcd.backlight();
  
  dht.begin(); 
  DS18B20_DT.begin();  

  lcd.setCursor(0, 0);
  lcd.print("    ESP8266");
  lcd.setCursor(0, 1);
  lcd.print("  METEOSTATION");
  delay(5000);
  lcd.clear();

  lcd.setCursor(0, 0);
  lcd.print("CONNECTING WIFI");
  lcd.setCursor(0, 1);
  lcd.print(ssid);
  delay(1000);
  lcd.clear();
  
  WiFi.begin(ssid, pass);
  lcd.setCursor(0, 0);
  lcd.print("WIFI CONNECTED");
  delay(1000);
  lcd.clear();
  
  lcd.setCursor(0, 0);
  lcd.print("CONNECTING TO DB");
  lcd.setCursor(0, 1);
  lcd.print(mysql_ip);
  while (conn.connect(mysql_ip, 3306, mysql_user, mysql_password) != true) {
    delay(100);
    lcd.print(".");
  }

  delay(1000);
  lcd.setCursor(0, 1);
  lcd.clear();
  first = true;
  lcd.setCursor(0, 0);
  lcd.print("STARTING");

  rest.variable("T_IN", &T_IN);                    
  rest.variable("T_OUT", &T_OUT); 
  rest.variable("Humidity", &Humidity); 
  rest.set_id("1");                                              
  rest.set_name("esp8266");    
  
  server.begin(); 
  lcd.setCursor(0, 0);
  lcd.print("REST STARTED");
  lcd.setCursor(0, 1);
  lcd.print(WiFi.localIP());
  delay(2000);
  lcd.clear();

  timeClient.begin();
  timeClient.setTimeOffset(14400);
  
  lcd.createChar(1, sun);
  lcd.createChar(2, house);
  lcd.createChar(3, humidity);
  lcd.createChar(4, celsium);
}

void loop(){
  
  
  if (first == true){
    sendquery();
  }
  
  if(millis() - timing >= 600000) { //10 min = 600000

   sendquery();
  }
  first = false;
   
  delay(1000);
  writeLCD();
  restapi();
}

void writeLCD()
{
  /*
   Function write info to LCD screen
  */
  getTemperature();
  timeClient.update();

  int hh = timeClient.getHours();
  int mm = timeClient.getMinutes();
 
  lcd.setCursor(0, 0);

  if (hh<10){
    lcd.print("0");
  }
  
  lcd.print(hh);

  if(blinker == false){
      lcd.print(":");
      blinker = true;
   }
   else{
      lcd.print(" ");
      blinker = false;
   }
  
  //lcd.print(":");
  
  if (mm<10){
    lcd.print("0");
  }
  lcd.print(mm);
  lcd.print(" ");
  lcd.print(getDate());
  
  lcd.setCursor(0, 1);

  lcd.write(byte(3));
  lcd.print((int)Humidity);
  lcd.print("%");

  lcd.setCursor(6, 1);
  lcd.write(byte(1));
  lcd.print((int)T_OUT);
  if(T_OUT<10){
    lcd.setCursor(8, 1);
    lcd.write(byte(4));
    lcd.print(" ");
  }
  else{
    lcd.setCursor(9, 1);
    lcd.write(byte(4));
  }
  
  
  lcd.setCursor(12, 1);
  lcd.write(byte(2));
  lcd.print((int)T_IN);
  if(T_IN<10){
    lcd.setCursor(14, 1);
    lcd.write(byte(4));
    lcd.print(" ");
  }
  else{
    lcd.setCursor(15, 1);
    lcd.write(byte(4));
  }
  

}

void sendquery(){

/*
  Function write info to DataBase over WiFi connection
*/

 if(WiFi.status() != WL_CONNECTED){
        return;
      }
      
if(conn.connect(mysql_ip, 3306, mysql_user, mysql_password) != true){
        return;
      }

    timing=millis();
    
    getTemperature();
    
    char INSERT_SQL[] = "INSERT INTO ESP8266.WEATHER (DATETIME,VAL1,VAL2,HUMIDITY) VALUES (CURRENT_TIMESTAMP(),%f,%f,%f)";
    sprintf(query, INSERT_SQL, T_OUT,T_IN,Humidity);
    Serial.println(query);
    
    MySQL_Cursor *cur_mem = new MySQL_Cursor(&conn);
    cur_mem->execute(query);

    delete cur_mem;
    
    writeLCD();
}

void restapi(){

  WiFiClient client = server.available();
  if (!client) {
    return;
  }
  while (!client.available()) {
    delay(1);
  }
  rest.handle(client);
}

String getDate() {
   time_t rawtime = timeClient.getEpochTime();
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