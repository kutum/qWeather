function getTodayString(date) {
    return "?start=" + getDateSting(date) + "T00:00:00&end=" + getDateSting(date) + "T23:59:59";
}

function getDateSting(date) {
    return date.getFullYear().toString() + "-" + (date.getMonth() + 1).toString() + "-" + date.getDate().toString();
}

function formatTimeDigit(item) {
    return item < 10 ? "0" + item.toString() : item.toString();
}

function getDateTimeSting(date) {

    return date.getFullYear().toString() + "-" + (date.getMonth() + 1).toString() + "-" + date.getDate().toString()
        + "T" + formatTimeDigit(date.getHours()) + ":" + formatTimeDigit(date.getMinutes()) + ":" + formatTimeDigit(date.getSeconds());
}

Date.prototype.addHours = function (h) {
    this.setHours(this.getHours() + h);
    return this;
}

function getDateHoursDiffString(h) {

    var diffdatetime = new Date().addHours(-h);
    var thisdatetime = new Date();

    return "?start=" + getDateTimeSting(diffdatetime) + "&end=" + getDateTimeSting(thisdatetime);
}

function getLastHours(hours) {
    sendAjaxRequest("GET", function (data) {
        model.weather.removeAll();
        for (var i = 0; i < data.length; i++) {
            model.weather.push(data[i]);
        }
    }, getDateHoursDiffString(hours));
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
        model.datetimeLast((data.DateTimeFormattedRus));
        model.outsideLast((data.VAL1).toFixed(1));
        model.insideLast((data.VAL2).toFixed(1));
        model.humidityLast(data.HUMIDITY);
    }, "last");
}

function getAverage(datesting) {
    sendAjaxRequest("GET", function (data) {
        model.outsideAverage((data.insideTemp).toFixed(1));
        model.insideAverage((data.outsideTemp).toFixed(1));
        model.humidityAverage((data.humidity).toFixed(1));
    }, "average" + datesting);
}

function getNow() {
    sendAjaxRequest("GET", function (data) {
        model.datetimeNow((data.DateTimeFormattedRus));
        model.outsideNow((data.VAL1).toFixed(1));
        model.insideNow((data.VAL2).toFixed(1));
        model.humidityNow(data.HUMIDITY);
    }, "now");
}