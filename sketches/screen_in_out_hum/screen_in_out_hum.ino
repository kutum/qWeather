#include <LiquidCrystal_I2C.h>                          //���������� ��������� ������� A1602
#include "DHT.h"                                        //���������� ��� ������ � �������� ����������� DHT11 (������ �� �������)
#include <OneWire.h>                                    //���������� ���� ����� ��� ������� DHT11
#include <DallasTemperature.h>                          //���������� ��� ������ � �������� ����������� � ��������� DS18B20              
#include <ESP8266WiFi.h>                                //���������� ��� ����������� � WiFi 
#include <aREST.h>                                      //���������� ��� ��������� REST ��������
#include <NTPClient.h>                                  //���������� ��� ��������� ���� � ������� ����� ��������
#include <WiFiUdp.h>                                    //���������� ��� ������ � UDP-��������

#define DT 14                                           //������ �� ������� DHT11 
#define DHTPIN 2                                        //�������� ������ ����������� � ��������� DS18B20 

OneWire oneWire_DT(DT);                                 //�������������� ���� ����� ��� DHT11
DallasTemperature DS18B20_DT(&oneWire_DT);              //�������������� ������ DS18B20
DHT dht(DHTPIN, DHT11);                                 //�������������� ������ DS18B20 �� 2 ����
LiquidCrystal_I2C lcd(0x27, 16, 2);                     //�������������� ����� � �������� 16 "�����" � ������, 2 � ������ � ������� 0x27
aREST rest = aREST();                                   //�������������� ������ REST ��� ��������� �������� �� API
WiFiUDP ntpUDP;                                         //�������������� ����������� UDP ������� ��� ��������� � ����������� ���� � �������
NTPClient timeClient(ntpUDP,"pool.ntp.org");            //�������������� "������� �������" ������� ���� ���� � ����� � ������� pool.ntp.org
WiFiServer server(80);                                  //�������������� ���-������, �� ������� ����� ��������� REST ������

float T_IN;                                             //���������� �� ��������� ����������� � DHT11 (���������)
float Humidity;                                         //���������� �� ��������� ���������
float T_OUT;                                            //���������� �� ��������� ����������� � DS18B20

char ssid[] = "xiaomi_kutuck";                          //��� WiFi ���� � ������� ������������
char pass[] = "myofrene";                               //������ WiFi ����

unsigned long timing;                                   //���������� "�������", ��� ���������� �������� �������� � ����������� �����
bool blinker = false;                                   //���������� ��� "�������" ���������� �� �����


byte sun[8] = {B00111, B00011, B00001, B01000, B11000, B01100, B01000, B11111};       //������ "�����" ��� ������� �����������
byte house[8] = {B00100, B01010, B10001, B11111, B10001, B10101, B11111, B00000};     //������ "�����" ��� ��������� �����������
byte humidity[8] = {B00000, B00100, B01110, B01110, B11111, B11111, B01110, B00000};  //������ "�����" ��� ���������
byte celsium[8] = {B00001, B00000, B01110, B10000, B10000, B10000, B01110, B00000};   //������ ������� ������� 

void getTemperature() {                                 //������� ��������� ����������� � ��������
  do {
    DS18B20_DT.requestTemperatures();                   //������ � ������� DT11
    T_IN = DS18B20_DT.getTempCByIndex(0);               //���������� ����������� � ��������
  } while (T_IN == 85.0);

  Humidity = dht.readHumidity();                        //�������� ��������� � ������� DS18B20
  T_OUT = dht.readTemperature();                        //�������� ����������� � ������� DS18B20 
}

void setup(){                                           //������������� ��� ������� �����������
  
  lcd.init();                                           //��������� �������
  lcd.backlight();                                      //��������� ��������� �������
  
  dht.begin();                                          //������������� ������� ��������� �����������
  DS18B20_DT.begin();                                   //������������� ������� �����

  lcd.setCursor(0, 0);                                  //������� ������ "�������" ������ (0(1) ������, 0(1) �������)
  lcd.print("     ESP8266    ");                        //����� ������ �� �����
  lcd.setCursor(0, 1);
  lcd.print("  METEOSTATION  ");
  delay(5000);                                          //��������
  
  lcd.setCursor(0, 0);                                
  lcd.print("CONNECTING WIFI ");                        
  lcd.setCursor(0, 1);
  lcd.print(ssid);                                      //����� �� ����� �������� ���� WiFi, � ������� �����������
  lcd.print(" ");
  delay(2000);
  
  WiFi.begin(ssid, pass);                               //�������������� ����������� � ���� � ������ ssid � ������� pass
  while (WiFi.status() != WL_CONNECTED){                //�������� ������������ � ����, ���� �� ������� �������� ����� ������ 0,1� �� ������ �������                
        lcd.print(".");
        delay(100);
      }
      
  lcd.setCursor(0, 0);
  lcd.print(" WIFI CONNECTED ");                        //���� ������������ ������������� �� ���� 
  delay(1000);
  lcd.clear();                                          //������� �������
  
                                                              //������������� ���������� ��� REST API  
  rest.variable("T_IN", &T_IN);                               //���������� �����������                                                                        
  rest.variable("T_OUT", &T_OUT);                             //�������� �����������
  rest.variable("Humidity", &Humidity);                       //��������� �������
  rest.set_id("1");                                           //ID ����������� (�� ������ ���� ������������ ������ ������, ����� �� ������������� ��� ��������                                             
  rest.set_name("esp8266");                                   //��� �����������
  
  server.begin();                                             //��������� ���-������             
                      
  lcd.setCursor(0, 0);                                
  lcd.print("REST STARTED");
  lcd.setCursor(0, 1);
  lcd.print(WiFi.localIP());                                  //������� ����������� IP ����� ���-�������, �� �������� ����� ����� ���������� � �����������
  delay(3000);
  lcd.clear();

  timeClient.begin();                                         //������ ������� ���� � �������
  timeClient.setTimeOffset(14400);                            //��������� �������� ����� (� ������ ������ GMT+4, ��� ���� ������� GMT ����� 3600 ����� 3600*4=14400)
  
  lcd.createChar(1, sun);                                     //������ ������ "�����"
  lcd.createChar(2, house);                                   //������ ������ "���"
  lcd.createChar(3, humidity);                                //������ ������ "�����"
  lcd.createChar(4, celsium);                                 //������ ������ "������ �������"
}

void loop(){                                                  //�������, ������� ���������� �������� �� ������ ���� ����������

  writeLCD();                                                 //������� ������ ��������� �� �����                     
  restapi();                                                  //������� ��������� REST API ��������
                                            
  //delay(500);                                                 //����� ���� ����������� - 0,5 �������.
}

void writeLCD()                                               //������� ������ �������� �� �����
{
  getTemperature();                                           //�������� ��������� � �������� � ���������� ����������
  timeClient.update();                                        //��������� ��������� ���� � ������� �� ������� � ����

  int hh = timeClient.getHours();                             //�����
  int mm = timeClient.getMinutes();                           //�����
 
  lcd.setCursor(0, 0);
  if (hh<10){                                                 //��������� � ���� ������� "0" ���� ����� ������ 10
    lcd.print("0");
  }
  lcd.print(hh);                                              //������� ���

  if(blinker == false){                                       //���� �������� ���������� ������ � ����� ���������, ����� ������� ������� ���� ��� ���� "����"
      lcd.print(":");
      blinker = true;
   }
   else{
      lcd.print(" ");
      blinker = false;
   }

  if (mm<10){                                                 //��������� � ������� ������� "0" ���� ����� ������ 10
    lcd.print("0");
  }
  lcd.print(mm);                                              //������� ������
  lcd.print(" ");
  lcd.print(getDate());                                       //�������� ���� � ����� ������� � �������� �� ������ �� �������
  
  lcd.setCursor(0, 1);                                        //��������� �� ������ ������
  lcd.write(byte(3));                                         //�������� "�����"
  lcd.print((int)Humidity);                                   //�������� ������� ��������� �������
  lcd.print("%");
  lcd.setCursor(6, 1);                                        
  lcd.write(byte(1));                                         //�������� ������ "�����"
  lcd.print((int)T_OUT);                                      //���������� ����������� �� �����
  if(T_OUT<10){                                               //���� �������� ���������� ������ ������� ������� "����������" � ��������. ���� ����� � ����� ����, �� �������� � 8 ������ � ������� � 9
    lcd.setCursor(8, 1);
    lcd.write(byte(4));
    lcd.print(" ");
  }
  else{
    lcd.setCursor(9, 1);                                     //���� ��� ����� � �����, �� � 9
    lcd.write(byte(4));
  }
 
  lcd.setCursor(12, 1);
  lcd.write(byte(2));                                        //�������� ������ "���"
  lcd.print((int)T_IN);                                      //�������� ����������� � �������
  if(T_IN<10){                                               //������ ����������� ������� ��� � ��� ������� �����������
    lcd.setCursor(14, 1);
    lcd.write(byte(4));
    lcd.print(" ");
  }
  else{
    lcd.setCursor(15, 1);
    lcd.write(byte(4));
  }
}

void restapi(){                                             //������� ��������� REST API ��������
  
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

String getDate() {                                          //������� ��������� ���� � ������� ��.��.����
   time_t rawtime = timeClient.getEpochTime();              //�������� ����
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