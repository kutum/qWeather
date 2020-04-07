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

#define DT 14 //IN Temperature
#define DHTPIN 2 //OUT Temperature
#define DHTTYPE DHT11 

DHT dht(DHTPIN, DHTTYPE);
OneWire oneWire_DT(DT);
DallasTemperature DS18B20_DT(&oneWire_DT);
LiquidCrystal_I2C lcd(0x27, 16, 2);  
aREST rest = aREST(); 

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


void getTemperature() {
  do {
    DS18B20_DT.requestTemperatures();
    T_IN = DS18B20_DT.getTempCByIndex(0); //Inside temperature
  } while (T_IN == 85.0);

  Humidity = dht.readHumidity(); //Humidity
  T_OUT = dht.readTemperature();   //Outside temperature
}

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
  lcd.print("HELLO <3");
  
  delay(1000);
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
 
  lcd.setCursor(0, 0);
  lcd.print("HUMIDITY:");
  lcd.print((int)Humidity);
  lcd.print("%");

  if(WiFi.status() != WL_CONNECTED){
        lcd.print("!W");
      }
      else{
        lcd.print("  ");
      }
      
      
  lcd.setCursor(0, 1);
  lcd.print("OUT:");
  lcd.print((int)T_OUT);
  lcd.print("C ");
  lcd.print("IN:");
  lcd.print((int)T_IN);
  lcd.print("C ");
   if(conn.connect(mysql_ip, 3306, mysql_user, mysql_password) != true){
        lcd.print("!D");
      }
      else{
        lcd.print("  ");
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