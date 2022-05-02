#include "date_functions.h"

String convertTimeToString(time_t rawtime) { //Функция получения даты в формате ГГГГ-ММ-ДД
    struct tm* ti;
    ti = localtime(&rawtime);

    uint16_t year = ti->tm_year + 1900;
    String yearStr = String(year);

    uint8_t month = ti->tm_mon + 1;
    String monthStr = month < 10 ? "0" + String(month) : String(month);

    uint8_t day = ti->tm_mday;
    String dayStr = day < 10 ? "0" + String(day) : String(day);

    return yearStr + "-" + monthStr + "-" + dayStr;
}
