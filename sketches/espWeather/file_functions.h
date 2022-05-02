#ifndef FILE_FUNCTIONS
#define FILE_FUNCTIONS
#include <FS.h>                                 

void writeFile(String message, String date);
String removeFile(String _name);
void removeAllFiles();
String files();

#endif
