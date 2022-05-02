#include "file_functions.h"

void writeFile(String message, String date) {
  File fileRead = SPIFFS.open(date, "r");
  String fileData = "";
  if(fileRead)
  {
    while(fileRead.available())
    {
      fileData += char(fileRead.read());
    }
  }
  fileData += message;
  fileRead.close();
  
  File fileWrite = SPIFFS.open(date, "w");
  if (!fileWrite) {
    Serial.println("Error opening file for writing");
    return;
  }
  
  int bytesWritten = fileWrite.print(fileData);
  if (bytesWritten == 0) {
    Serial.println("File write failed");
    return;
  }
  fileWrite.close();
}

String removeFile(String _name){
    if(SPIFFS.exists(_name)){
      SPIFFS.remove(_name);
      return "OK";
    }
    else{
      return "Not found";
    }
}

void removeAllFiles() {
  Dir dir = SPIFFS.openDir("");
  while (dir.next()) {
    SPIFFS.remove(dir.fileName());
  }
}

String files(){
  String _data;
  Dir dir = SPIFFS.openDir("");
  while (dir.next()) {
    _data += dir.fileName() + ",";
  } return _data.substring(0,_data.length() - 1);
}
