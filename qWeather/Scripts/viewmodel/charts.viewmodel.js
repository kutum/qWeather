window.chartColors = {
    red: 'rgb(255, 99, 132)',
    orange: 'rgb(255, 159, 64)',
    yellow: 'rgb(255, 205, 86)',
    green: 'rgb(75, 192, 192)',
    blue: 'rgb(54, 162, 235)',
    purple: 'rgb(153, 102, 255)',
    grey: 'rgb(201, 203, 207)'
};

ko.bindingHandlers.ChartTemp = {
    update: function (element, valueAccessor, allBindingsAccesor, viewModel, bindingContext) {

        if (this.charttemp) {
            this.charttemp.destroy();
            delete this.charttemp;
        }

        var dataArrayOut = [];
        var dataArrayIn = [];
        var dataLabels = [];

        for (var i = 0; i < viewModel.weather().length; i++) {
            var thisdate = new Date(viewModel.weather()[i].DateTimeFormatted);
            dataArrayOut.push(viewModel.weather()[i].VAL1);
            dataArrayIn.push(viewModel.weather()[i].VAL2);
            dataLabels.push(thisdate.toLocaleTimeString().substring(0, 5));
        }

        var ctx = element.getContext('2d');
        this.charttemp = new Chart(ctx, {
            type: 'line',
            data: {
                labels: dataLabels,
                datasets: [
                    {
                        label: 'Outside',
                        borderColor: window.chartColors.blue,
                        backgroundColor: window.chartColors.blue,
                        fill: false,
                        data: dataArrayOut
                    },
                    {
                        label: 'Inside',
                        borderColor: window.chartColors.red,
                        backgroundColor: window.chartColors.red,
                        fill: false,
                        data: dataArrayIn
                    }
                ],
                fill: false
            },

            options: {}
        });
    }
}

ko.bindingHandlers.ChartHumidity = {
    update: function (element, valueAccessor, allBindingsAccesor, viewModel, bindingContext) {

        if (this.charthumidity) {
            this.charthumidity.destroy();
            delete this.charthumidity;
        }

        var dataLabels = [];
        var dataArrayHumidity = [];


        for (var i = 0; i < viewModel.weather().length; i++) {
            var thisdate = new Date(viewModel.weather()[i].DateTimeFormatted);
            dataArrayHumidity.push(viewModel.weather()[i].HUMIDITY);
            dataLabels.push(thisdate.toLocaleTimeString().substring(0, 5));
        }

        var ctx = element.getContext('2d');
        this.charthumidity = new Chart(ctx, {
            type: 'line',
            data: {
                labels: dataLabels,
                datasets: [
                    {
                        label: 'Humidity %',
                        borderColor: window.chartColors.purple,
                        backgroundColor: window.chartColors.purple,
                        fill: false,
                        data: dataArrayHumidity
                    }
                ],
                fill: false
            },

            options: {}
        });
    }
}