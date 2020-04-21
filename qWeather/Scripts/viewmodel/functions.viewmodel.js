function getTodayString(date) {
    return "?start=" + getcurrentDate(date) + "T00:00:00&end=" + getcurrentDate(date) + "T23:59:59";
}

function getcurrentDate(date) {
    return date.getFullYear().toString() + "-" + (date.getMonth() + 1).toString() + "-" + date.getDate().toString();
}

function getToday() {
    sendAjaxRequest("GET", function (data) {
        model.weather.removeAll();
        for (var i = 0; i < data.length; i++) {
            model.weather.push(data[i]);
        }
    }, getTodayString(new Date()));
}

function getLast() {
    sendAjaxRequest("GET", function (data) {
        model.outside((data.VAL1).toFixed(1));
        model.inside((data.VAL2).toFixed(1));
        model.humidity(data.HUMIDITY);
    }, "last");
}

function getAverage() {
    sendAjaxRequest("GET", function (data) {
        model.outsideAverage((data.insideTemp).toFixed(1));
        model.insideAverage((data.outsideTemp).toFixed(1));
        model.humidityAverage((data.humidity).toFixed(1));
    }, "average" + getTodayString(new Date()));
}