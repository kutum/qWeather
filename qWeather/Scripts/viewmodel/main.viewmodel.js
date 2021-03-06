﻿var model = {
    weather: ko.observableArray(),

    datetimeLast: ko.observable(),
    outsideLast: ko.observable(),
    insideLast: ko.observable(),
    humidityLast: ko.observable(),

    datetimeNow: ko.observable(),
    outsideNow: ko.observable(),
    insideNow: ko.observable(),
    humidityNow: ko.observable(),

    outsideAverage: ko.observable(),
    insideAverage: ko.observable(),
    humidityAverage: ko.observable(),

    dropdownOptions: ko.observableArray(['Last 4h', 'Last 8h', 'Day', 'Week', 'Month', 'Year', 'All']),
    dropdownValue: ko.observable()
};