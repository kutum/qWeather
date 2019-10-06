#include <MySQL_Connection.h>
#include <MySQL_Cursor.h>
#include <ESP8266WiFi.h>
#include <WiFiClient.h>
#include <OneWire.h>
#include <DallasTemperature.h>

#define DT1 5 //D1 temperature #1
#define DT2 4 //D2 temperature #2
#define LED1 0 //D3 write to DB
#define LED2 2 //D4 wifi connection status
#define LED3 14 //D5 db connection status

OneWire oneWire_DT1(DT1);
OneWire oneWire_DT2(DT2);

DallasTemperature DS18B20_DT1(&oneWire_DT1);
float T_C_DT1;

DallasTemperature DS18B20_DT2(&oneWire_DT2);
float T_C_DT2;

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
    DS18B20_DT1.requestTemperatures();
    T_C_DT1 = DS18B20_DT1.getTempCByIndex(0);
  } while (T_C_DT1 == 85.0);

  do {
    DS18B20_DT2.requestTemperatures();
    T_C_DT2 = DS18B20_DT2.getTempCByIndex(0);
  } while (T_C_DT2 == 85.0);
}
 
void setup() {
  
  Serial.begin(115200);
  
  DS18B20_DT1.begin();
  DS18B20_DT2.begin();

  pinMode(LED1, OUTPUT);
  pinMode(LED2, OUTPUT);
  pinMode(LED3, OUTPUT);

  digitalWrite(LED1, LOW); 
  digitalWrite(LED2, LOW); 
  digitalWrite(LED3, LOW); 
  
  Serial.println("Connecting to ");
  Serial.println(ssid);
  
  WiFi.begin(ssid, pass);
  
  while (WiFi.status() != WL_CONNECTED) {
      delay(200);
      Serial.print(".");
  }
  Serial.println("WiFi Connected");
  digitalWrite(LED2, HIGH); 
  Serial.print("Assigned IP: ");
  Serial.println(WiFi.localIP());

  Serial.println("Connecting to db");
  while (conn.connect(mysql_ip, 3306, mysql_user, mysql_password) != true) {
    delay(200);
    Serial.print ( "." );
  }
  
  digitalWrite(LED3, HIGH); 
  Serial.println("Connected to SQL Server!");  

  first = true;
}

void loop() {

  if (first == true){
    sendquery();
  }
  
  if(millis() - timing >= 600000) { //10 min = 600000

   sendquery();
  }
  first = false;
}

void sendquery(){

 if(WiFi.status() != WL_CONNECTED){
        digitalWrite(LED2, LOW);
        return;
      }
      else{
        digitalWrite(LED2, HIGH);
      }
    
      if(conn.connect(mysql_ip, 3306, mysql_user, mysql_password) != true){
        digitalWrite(LED3, LOW);
        return;
      }
      else{
        digitalWrite(LED3, HIGH);
      }
    
    digitalWrite(LED1, HIGH); 
    timing=millis();
    
    getTemperature();
    
    char INSERT_SQL[] = "INSERT INTO ESP8266.WEATHER (DATETIME,VAL1,VAL2) VALUES (CURRENT_TIMESTAMP(),%f,%f)";
    sprintf(query, INSERT_SQL, T_C_DT1,T_C_DT2);
    Serial.println(query);
    
    MySQL_Cursor *cur_mem = new MySQL_Cursor(&conn);
    cur_mem->execute(query);

    delete cur_mem;

    digitalWrite(LED1, LOW); 
}
