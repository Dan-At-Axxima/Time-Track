function setMinimumDate(e) {
    var ddlContainer = e.container.find("#date");
    var date = new Date(Date.parse($('#defaultDate').val())); 
    var year = date.getFullYear();
    console.log(date);
    console.log(year);
    ddlContainer.kendoDatePicker({
//        value: new Date(2022, 10, 1),
        min: new Date(2022, 10, 1)
    });
}




function hoursdropdown(e) {

    var ddlContainer = e.container.find("#dropDownHours");
    var data = [
        { "hoursID": 0, "hoursValue": "0" },
        { "hoursID": 1, "hoursValue": "1" },
        { "hoursID": 2, "hoursValue": "2" },
        { "hoursID": 3, "hoursValue": "3" },
        { "hoursID": 4, "hoursValue": "4" },
        { "hoursID": 5, "hoursValue": "5" },
        { "hoursID": 6, "hoursValue": "6" },
        { "hoursID": 7, "hoursValue": "7" },
        { "hoursID": 8, "hoursValue": "8" },
        { "hoursID": 9, "hoursValue": "9" },
        { "hoursID": 10, "hoursValue": "10" },
        { "hoursID": 11, "hoursValue": "11" },
        { "hoursID": 12, "hoursValue": "Go Home You Work Too Much" }
    ];
    ddlContainer.kendoDropDownList({
        dataSource: data,
        dataTextField: "hoursValue",
        dataValueField: "hoursID"
    });
}

function minutesdropdown(e) {

    var ddlContainer = e.container.find("#dropDownMinutes");
    var data = [
        { "minutesID": 0, "minutesValue": ":00" },
        { "minutesID": 1, "minutesValue": ":01" },
        { "minutesID": 2, "minutesValue": ":02" },
        { "minutesID": 3, "minutesValue": ":03" },
        { "minutesID": 4, "minutesValue": ":04" },
        { "minutesID": 5, "minutesValue": ":05" },
        { "minutesID": 6, "minutesValue": ":06" },
        { "minutesID": 7, "minutesValue": ":07" },
        { "minutesID": 8, "minutesValue": ":08" },
        { "minutesID": 9, "minutesValue": ":09" },
        { "minutesID": 10, "minutesValue": ":10" },
        { "minutesID": 11, "minutesValue": ":11" },
        { "minutesID": 12, "minutesValue": ":12" },
        { "minutesID": 13, "minutesValue": ":13" },
        { "minutesID": 14, "minutesValue": ":14" },
        { "minutesID": 15, "minutesValue": ":15" },
        { "minutesID": 16, "minutesValue": ":16" },
        { "minutesID": 17, "minutesValue": ":17" },
        { "minutesID": 18, "minutesValue": ":18" },
        { "minutesID": 19, "minutesValue": ":19" },
        { "minutesID": 20, "minutesValue": ":20" },
        { "minutesID": 21, "minutesValue": ":21" },
        { "minutesID": 22, "minutesValue": ":22" },
        { "minutesID": 23, "minutesValue": ":23" },
        { "minutesID": 24, "minutesValue": ":24" },
        { "minutesID": 25, "minutesValue": ":25" },
        { "minutesID": 26, "minutesValue": ":26" },
        { "minutesID": 27, "minutesValue": ":27" },
        { "minutesID": 28, "minutesValue": ":28" },
        { "minutesID": 29, "minutesValue": ":29" },
        { "minutesID": 30, "minutesValue": ":30" },
        { "minutesID": 4, "minutesValue": ":40" },
        { "minutesID": 5, "minutesValue": ":50" }

    ];
    ddlContainer.kendoDropDownList({
        dataSource: data,
        dataTextField: "minutesValue",
        dataValueField: "minutesID"
    });
}


