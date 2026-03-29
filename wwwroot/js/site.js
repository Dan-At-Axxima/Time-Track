
   
function userChanged(e) {
    var ddl = e.sender;
    var selectedMonth = $('#selectedMonth').val();
    var selectedYear = $('#selectedYear').val();
    var selectedUser = ddl.value();
    var d1 = $('#startDateLabel').val();
    var d2 = $('#endDateLabel').val();


    $('#beginningDate').val(d1);
    $('#endingDate').val(d2);
    $('#monthSelection').val(selectedMonth);
    $('#yearSelection').val(selectedYear);
    $('#thisSelectedUser').val(selectedUser);
    $("#changeUserForm").submit();

};


function makeJavaScriptDate(dateString) {
    var values = dateString.split('-');
    if (values.length != 3) {
        values = dateString.split(',');
    }
    if (values.length == 3) {
        let year = values[0];
        let month = values[1];
        let day = values[2]
        let date = new Date(year, month-1, day, 0, 0, 0);
        return date;
    }
}


function isValidCell(row, col, spreadsheet) {
    let extraFreezeColumns = parseInt($('#extraFreezeColumns').val());
    let firstRow = $('#firstRow').val();
    let firstColumn = $('#firstColumn').val();
    if (typeof spreadsheet !== 'undefined') {
        var sheet = spreadsheet.activeSheet(),
        selection = sheet.selection();
        row = selection._ref.topLeft.row;
        col = selection._ref.topLeft.col;

    }
    var validCell = false;
    if (row > (firstRow - 1) && col > (firstColumn - 1 + extraFreezeColumns)) {
        validCell = true;
    }
    console.log('valid cell ', validCell);
    return validCell;
};


function onChanging(arg) {
    arg.sender_controller_preventNavigation = true;
    number: col = arg.range._sheet._editSelection._activeCell.bottomRight.col; //._activeCell._bottomRight.col;
    number: row = arg.range._sheet._editSelection._activeCell.bottomRight.row; //._activeCell._bottomRight.col
    var loading = $('#isLoading').val();
        var spreadsheet = $("#spreadsheet").data("kendoSpreadsheet");
    var sheet = spreadsheet.activeSheet();
};

function onChange(arg) {
    var loading = $('#isLoading').val();
    var columns = ["A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z", "AA", "AB", "AC", "AD", "AE", "AF", "AG", "AH"];

    if (loading == 'false') {
        var spreadsheet = $("#spreadsheet").data("kendoSpreadsheet");
        var sheet = spreadsheet.activeSheet();
        var selectedMonth = $("#selectedMonth").val();
        var selectedYear = $("#selectedYear").val();
        var selectedUser = $("#currentUser").val();
        arg.sender._view.editor.one("deactivate", function (e) {
            var range = e.sender._range;
            e.preventDefault();
            number: col = e.sender._range._ref.bottomRight.col;
            var colLabel = columns[col].concat(":");
            var row = e.sender._range._ref.bottomRight.row;
            var val = e.sender._value;
            var comment = range.comment();
            var originalVal = $("#originalCellValue").val();
            var client = sheet.range(row, 0).values();
            var project = sheet.range(row, 1).values();
            var activity = sheet.range(row, 2).values();
            var day = sheet.range(0, col).values();
            var values = range.values();
            var oldCell = colLabel.concat(row.toString());
            

            $("#activeCellValue").val("Column  " + col + " Row " + row);
            saveActivity(client[0][0], project[0][0], activity[0][0], selectedUser, val, selectedYear, selectedMonth, col- 2,arg);
        });
    }
    else {
      //  console.log('loading');
    }


}

function changeFrozenDate(date) {
    var baseUrl = $('#baseURL').val();
    var changeFrozenDateUrl = $('#changeFrozenDateURL').val();
    var fullURL = baseUrl + changeFrozenDateUrl;
    var updateValue = {
        "date": date
    };
    updateValue = JSON.stringify(updateValue);
   // console.log('update value ', updateValue);
    $.ajax({
        url: fullURL,
        type: 'POST',
        contentType: "application/json",
        data: updateValue
    }).done(function (data) {
            //console.log('it worked');
    }).fail(function (error) {
            //console.log('save error');
    }).always(function () {
            //console.log('done')
    })
 }



//function saveData() {
//    $.ajax({
//        url: URL,
//        type: 'GET',
//        contentType: "application/json",
//        success: function (data) {
//            //console.log(JSON.stringify(data));
//        },
//        error: function (error) {
//            //console.log("error");
//        },
//        complete: function () {
//            //console.log("In the complete function");
//        }

//    });
//}

function getActivityList() {
    $.ajax({
        url: URL,
        type: 'GET',
        contentType: "application/json",
        success: function (data) {
            //console.log(JSON.stringify(data));
        },
        error: function (error) {
            //console.log("error");
        },
        complete: function () {
            //console.log("In the complete function");
        }

    });
}

function onSelect(arg) {
    console.log('I was selected');
    return;
    var loading = $('#isLoading').val();
    if (loading == 'false') {
      //  arg.preventDefault();
        var columns = ["A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z", "AA", "AB", "AC", "AD", "AE", "AF", "AG", "AH"];
        var spreadsheet = $("#spreadsheet").data("kendoSpreadsheet");
        var sheet = spreadsheet.activeSheet();
        var range = sheet.range();
        var cell = sheet.activeCell();
        var error = $("#editError").val();
    }

}



function getCurrentRange() {
    var spreadsheet = $("#spreadsheet").data("kendoSpreadsheet");
    var sheet = spreadsheet.activeSheet();
    var cell = sheet.activeCell();
    var range = sheet.range(cell);
    return range;
}

function getCellValues(e) {
 //   console.log('get cell values: ', e);
    var range = '';
    if (e == 'undefined' || e == undefined) {
        range = getActiveRange();
    }
    else {
        range = e.range;
    }
    
    var comment = range.comment() == undefined ? '' : range.comment() ;
    var value = range.value() == undefined ? '' : range.value();
  //  console.log('com is: ', comment, 'value is: ', value);

    return {
            'comment': comment,
            'value': value
    }
}


function testTime(time) {

    // Step 1: Check if the string has a valid time format

    var isValidTimeFormat = /^((?:[1-9][0-9]?|0)?:[0-5][0-9]|9:60)$/.test(time);
    console.log('is this a valid time format ', isValidTimeFormat);

    if (isValidTimeFormat) {
        // Step 2: Separate hours and minutes and perform additional checks
        var [hours, minutes] = time.split(":");
        var hrs = parseInt(hours);
        var isValidHours = false;
        var isValidMinutes = false;
        if (isNaN(hrs) || hrs == 0) {
            isValidHours = hours === "" || (parseInt(hours, 10) >= 0 && parseInt(hours, 10) <= 99);
            isValidMinutes = parseInt(minutes, 10) > 0 && parseInt(minutes, 10) <= 60;
        }
        else {
            isValidHours = hours === "" || (parseInt(hours, 10) >= 0 && parseInt(hours, 10) <= 99);
            isValidMinutes = parseInt(minutes, 10) >= 0 && parseInt(minutes, 10) <= 60;
        }
    //    console.log('minutes ', minutes, ' hours ',hours);
        if (isValidHours && isValidMinutes) {
      //      console.log("Valid format.");
        } else {
        //    console.log("Invalid format.");
            isValidTimeFormat = false;
        }
    } else {
      //  console.log("Invalid format.");
    }

    return isValidTimeFormat;
}

function checkCellContent(comment, time) {
    var timeMatched = testTime(time);
 //   var isNormal = time.indexOf(':');
 //   var regString = '';
 //   isNormal = parseInt(isNormal);
  //  console.log('new test regex ', trueorfalse, 'and the value is ',time);
 //   const myReg = new RegExp('(^[0-9]{0,2}:[0-5]{1}[0-9]{1}$)');
    //    const myReg = new RegExp('(^\d*:[0-5]?\d:[0-5][0-9]$)');
//    var timeMatched = myReg.test(time);
    //  console.log('did time match? ',timeMatched);
    //  console.log('time = ',time);
    var isThereAComment = true;
    if (comment == null || comment == 'undefined' || comment.trim()==='') {
        isThereAComment = false;
    };
    return { 'commentExists': isThereAComment, 'timeMatched': timeMatched };
}

function checkContent() {
    var comment = $("#commentText").val();
    var time = $("#timeText").val();
    var isNormal = time.indexOf(':');
    var regString = '';
    isNormal = parseInt(isNormal);
    
    const myReg = new RegExp('(^[0-9]{0,2}:[0-5]{1}[0-9]{1}$)');
//    const myReg = new RegExp('(^\d*:[0-5]?\d:[0-5][0-9]$)');
    var timeMatched = myReg.test(time);
  //  console.log('did time match? ',timeMatched);
  //  console.log('time = ',time);
    var isThereAComment = true;
    if (comment == 'undefined' || comment.length < 1) {
        isThereAComment = false;
    };
    return { 'comment': isThereAComment, 'timeMatched': timeMatched };
}

function userChanged(e) {
    var ddl = e.sender;
    var selectedMonth = $('#selectedMonth').val();
    var selectedYear = $('#selectedYear').val();
    var selectedUser = ddl.value();
    var d1 = $('#startDateLabel').val();
    var d2 = $('#endDateLabel').val();


    $('#beginningDate').val(d1);
    $('#endingDate').val(d2);
    $('#monthSelection').val(selectedMonth);
    $('#yearSelection').val(selectedYear);
    $('#thisSelectedUser').val(selectedUser);
    $("#changeUserForm").submit();

};

function saveData(col, row,comment,time) {
    var res = checkCellContent(comment,time);
    var com = res.commentExists;
    var timeMatched = res.timeMatched;
    if (com == false && timeMatched == false) { // if both are empty it's ok.
        removeComment();
        return true;
    }
    if (com == false || timeMatched == false) { // if only one is empty, it's not ok
        return res;
    }
    var spreadsheet = $("#spreadsheet").data("kendoSpreadsheet");
    var sheet = spreadsheet.activeSheet();

    var allDates = $('#newAllDates').val();
    var allDates = allDates.split(',');


    var client = sheet.range(row, 0).values();
    var project = sheet.range(row, 1).values();
    var activity = sheet.range(row, 2).values();
    var selectedMonth = $("#selectedMonth").val();
    var selectedYear = $("#selectedYear").val();
    var currentUser = $("#currentUser").val();
    console.log('current user is ' + currentUser);
    if (!isValidUser(currentUser)) {
        alert('System cannot save: Invalid User Number ' + currentUser + ' -- please call admin');
        range.value('');
        range.comment('');
        totalRow(sheet);
        return false;
    }
    var range = sheet.range(row, col, 1, 1);
    range.comment(comment);
    range.value(time);
    //saveCellComment(client[0][0], project[0][0], activity[0][0], selectedUser, comment, hours, selectedYear, selectedMonth, myCol - 4);
    saveCellData(client[0][0], project[0][0], activity[0][0], currentUser, comment, time, col,range);
    // weird but I have to force Row Totals/Columns Total to recalculate every time a value changes
    totalRow(sheet);
    //var string = "=" + "AddRows(" + 1 + ")";
    //range = sheet.range(1, 3, 1, 1);
    //range.formula(string);
    return true;
}

function totalRow(sheet) {
    var string = "=" + "AddRows(" + 1 + ")";
    range = sheet.range(1, 3, 1, 1);
    range.formula(string);
}

function isValidUser(value) {
    console.log('type of user is ' + typeof value) 
    const number = Number(value);
    if (isNaN(number)) {
        return false;
    }
    else {
        if (number >= 1000 && number <= 9999)
            return true
        else {
            return false;
        }
    }
}

function saveNewData(col, row) {
    var comment = $('#commentText').val();
    var time = $('#timeText').val();
    var res = checkCellContent(comment, time);
    var com = res.comment;
    var time = res.timeMatched;
    if (com == false && time == false) { // if both are empty it's ok.
        removeComment();
        return true;
    }
    if (com == false || time == false) { // if only one is empty, it's not ok
        return res;
    }
    var spreadsheet = $("#spreadsheet").data("kendoSpreadsheet");
    var sheet = spreadsheet.activeSheet();

    var allDates = $('#newAllDates').val();
    var allDates = allDates.split(',');


    var client = sheet.range(row, 0).values();
    var project = sheet.range(row, 1).values();
    var activity = sheet.range(row, 2).values();
    var selectedMonth = $("#selectedMonth").val();
    var selectedYear = $("#selectedYear").val();
    var selectedUser = $("#currentUser").val();
    var range = sheet.range(row, col, 1, 1);
    range.comment(comment);
    range.value(time);
    //saveCellComment(client[0][0], project[0][0], activity[0][0], selectedUser, comment, hours, selectedYear, selectedMonth, myCol - 4);
    saveCellData(client[0][0], project[0][0], activity[0][0], selectedUser, comment, hours, col);
    // weird but I have to force Row Totals/Columns Total to recalculate every time a value changes
    var string = "=" + "AddRows(" + 1 + ")";
    range = sheet.range(1, 3, 1, 1);
    range.formula(string);
    return true;
}

function saveComment(e) {
    var res = checkContent();
    var com = res.comment;
    var time = res.timeMatched;
    if (com == false && time == false) { // if both are empty it's ok.
        removeComment();
        return true;
    }
    if (com == false || time == false) { // if only one is empty, it's not ok
        return res;
    }
    var spreadsheet = $("#spreadsheet").data("kendoSpreadsheet");
    var sheet = spreadsheet.activeSheet();
    var cell = sheet.activeCell();
    var columnIdx = cell.bottomRight.col;
    var myRow = cell.bottomRight.row;

    var allDates = $('#newAllDates').val();
    var allDates = allDates.split(',');


    var client = sheet.range(myRow, 0).values();
    var project = sheet.range(myRow, 1).values();
    var activity = sheet.range(myRow, 2).values();
    var comment = $('#commentText').val();
    var hours = $('#timeText').val();
    var selectedMonth = $("#selectedMonth").val();
    var selectedYear = $("#selectedYear").val();
    var selectedUser = $("#currentUser").val();
    var range = getCurrentRange();
    number: col = 0;
    number: row1 = 0;
    range.comment(comment);
    range.value(hours);
    //saveCellComment(client[0][0], project[0][0], activity[0][0], selectedUser, comment, hours, selectedYear, selectedMonth, myCol - 4);
    saveCellData(client[0][0], project[0][0], activity[0][0], selectedUser, comment, hours, columnIdx);
    // weird but I have to force Row Totals/Columns Total to recalculate every time a value changes
    var string = "=" + "AddRows(" + 1 + ")";
    range = sheet.range(1, 3, 1, 1);
    range.formula(string);
    return true;
}

function removeComment() {
    console.log('this is removecomment');
    var range = getCurrentRange();
    var sheet = getCurrentSheet();
    var cell = sheet.activeCell();
    var myCol = cell.bottomRight.col;
    var myRow = cell.bottomRight.row;
    range.comment('');
    range.value('');
    var date = GetDateFromColumn(myCol)
    var client = sheet.range(myRow, 0).values();
    var project = sheet.range(myRow, 1).values();
    var activity = sheet.range(myRow, 2).values();
    var selectedDay = date.getDate()
    var selectedMonth = date.getMonth() + 1;
    var selectedYear = date.getFullYear();
    deleteCellContents(client[0][0], project[0][0], activity[0][0], selectedUser, selectedYear, selectedMonth, selectedDay);

}

function removeCellData(sheet, col, row) {
    console.log('this is removecelldata');

    var client = sheet.range(row, 0).values();
    var project = sheet.range(row, 1).values();
    var activity = sheet.range(row, 2).values();
    var selectedMonth = $("#selectedMonth").val();
    var selectedYear = $("#selectedYear").val();
    var selectedUser = $("#currentUser").val();

    deleteCellContents(client[0][0], project[0][0], activity[0][0], selectedUser, selectedYear, selectedMonth, col - 2);

}

function removeCellDataWithDate(sheet, row, date) {
    console.log('this is removecelldatawithDate');
    var client = sheet.range(row, 0).values();
    var project = sheet.range(row, 1).values();
    var activity = sheet.range(row, 2).values();
    dateArray = [];
    dateArray = date.split('-');
    console.log('dateArray is ', dateArray);
    var selectedDay = dateArray[2];
    var selectedMonth = dateArray[1];
    var selectedYear = dateArray[0];
    console.log('Day ', selectedDay);
    console.log('Month ', selectedMonth);
    console.log('Year ', selectedYear);
    var selectedUser = $("#currentUser").val();
    deleteCellContents(client[0][0], project[0][0], activity[0][0], selectedUser, selectedYear.toString(), selectedMonth.toString(), selectedDay.toString());

}

function getCurrentSheet() {
    var spreadsheet = $("#spreadsheet").data("kendoSpreadsheet");
    var sheet = spreadsheet.activeSheet();
    return sheet;
}

function addData() {
    var test = $('#daysOfTheMonth').val();
    sheet = getCurrentSheet();
    range = sheet.range("A1:AE1");
}

function makeNewDialog(col, row, newparm, oldparm) {
    //  console.log('make dialog');
   var dialog =  $("#dialog").kendoDialog({
        title: "Enter Data",
        close: function (e) {
            //var spreadsheet = $("#spreadsheet").data("kendoSpreadsheet");
            //console.log('e is equal to',e);
            //var sheet = spreadsheet.activeSheet();
            //let row = parseInt($('#cellInitialRow').val());
            //let col = parseInt($('#cellInitialCol').val());
            //console.log('row is ', row, ' and column is ', col);
            //var allDates = $('#newAllDates').val();
            //allDates = allDates.split(',');
            //var client = sheet.range(row, 0).values();
            //var project = sheet.range(row, 1).values();
            //var activity = sheet.range(row, 2).values();
            //var selectedMonth = $("#selectedMonth").val();
            //var selectedYear = $("#selectedYear").val();
            //var selectedUser = $("#newSelectedUser").val();
            //reloadCellData(client[0][0], project[0][0], activity[0][0], selectedUser, col,row, selectedYear, selectedMonth);
            //let range = sheet.range(row, col, 1, 1);
            //let comment = $('#cellInitialComment').val()
            //let value = $('#cellInitialValue').val()
            //console.log('row is ',row,' and column is',col,' and range is ',range);
            //console.log('value is ', value, ' and comment is', comment, ' and range is ', range);
            //range.value('wtf is going on');
            //range.comment('shit man');
            //return true;
            //let value = range.value();
            //$("#commentErrorMsg").css("visibility", "hiden");
            //$("#timeErrorMsg").css("visibility", "hidden");
            //let comm = $('#commentText').val();
            //let time = $('#timeText').val();
            //if ((comm != null && comm.length > 0) && (time != null && time.length > 0)) {
            //    return true;
            //}
            //if ((time == null || time.length == 0) && (comm!=null && comm.length > 0)) {
            //    $("#timeErrorMsg").css("visibility", "visible");
            //    e.preventDefault();
            //}
            //if ((comm == null || comm.length == 0) && (time!=null && time.length > 0)) {
            //    $("#commentErrorMsg").css("visibility", "visible");
            //    e.preventDefault();
            //}

       },
       open: function () {
           $('#timeText').prop("readonly", false);
           console.log('we just opened');
       },

       show: function () {
           console.log('we just showed');
           $('#commentText').select();
//           $('#timeText').select();
           console.log('the dialog is showing');
       //    $('#timeText').prop("readonly", false);
       //    $('#timeText').focus();
           let time = $('#timeText').val();
           console.log('time is ', time);
       //    var dialog = $('#dialog');
           let comment = $('#commentText').val();
           console.log('comment is ', comment);
       //    console.log('dialog is open and time value is ', time);
       //    var test = comment.length;
       //    console.log('test is ', test);
       //    if (time.length > 0 && comment.length > 0) {
       //        $('#timeText').prop("readonly", false);
       //        $('#timeText').focus();
       //    }
            if (time.length > 0 && comment.length < 1) {
                $('#commentText').prop("readonly", false);
                console.log('this should work');
               $('#commentText').focus();

            };
       },
       //    else {
       //        $('#timeText').prop("readonly", false);
       //        $('#timeText').focus();
       //    }
        content: '<div class="k-editor-dialog k-popup-edit-form k-edit-form-container" style="width:auto;">' +
            '<div class="row">' +
            '<div class=col-md-1>' +
            '<p>Time</p>' +
            '</div>' +
            '<div class=col-md-8>' +
            '<div id="timeErrorMsg" style="visibility:hidden; color:red" >Error: Time must be of the format: HH:MM HH is optional </div > ' +
            '<p><textarea id="timeText" placeholder="hh:mm" title="textAreaTitle" cols="60" rows="1" style="width:30%"></textarea></p>' +
            '</div>' +
            '<div class="row">' +
            '<div class=col-md-1>' +
            '<p>Comment</p>' +
            '</div>' +
            '<div class=col-md-8>' +
            '<div id="commentErrorMsg" style="visibility:hidden; color:red" >Error: Missing comment</div > ' +
            '<p><textarea id="commentText" title="textAreaTitle" cols="60" rows="2" style="width:95%"></textarea></p>' +
            '</div>' +
            '</div>',
        visible: false, actions: [
            {
                text: 'Save ', action: function (e) {
                    $("#commentErrorMsg").css("visibility", "hiden");
                    $("#timeErrorMsg").css("visibility", "hidden");
                    var baseUrl = $('#baseURL').val();
                    var updateCellUrl = $('#saveCellDataURL').val();
                    var fullURL = baseUrl + updateCellUrl;
                    var comment = $('#commentText').val();
                    var time = $('#timeText').val();
                    var res = saveData(col, row, comment, time);
                    var com = res.commentExists;
                    var time = res.timeMatched;
                    if (com == false || time == false) {
                        if (com == false) {
                            $("#commentErrorMsg").css("visibility", "visible");
                        }
                        if (time == false) {
                            $("#timeErrorMsg").css("visibility", "visible");
                        }
                        return false;
                    }
                    else {
                        return true;
                    }
                }
            }
        ]
    });
    //var test = $("#dialog");
    //var titlebar = test.find('.k-dialog-titlebar').find('a[name="Close"]').click(function () { console.log('yabadabado') });
 
}

function checkInput() {
    let value = $('#timeText').val();
    if (value != null && value != undefined && value.length < 3 && value.length>0 && value.indexOf(":") == -1) {
        value = value + ":00";
        console.log('value is ', value);
        $('#timeText').val(value);
        return;
    }
    if (value != null && value != undefined && value.indexOf(":") > -1) {

        let values = value.split(":");
        if (values.length == 1) {
            let thisValue = values[0];
            if (thisValue.length == 1) {
                value = ":" + thisValue + "0";
            }
        }
        if (values.length == 2) {
            let thisValue = values[1];
            if (thisValue.length == 1) {
                value = values[0] + ":" + thisValue + "0";
            }

        }
        //e.preventDefault();
        $('#timeText').val(value);
    }
}

function adjustSize() {
    const textarea = document.getElementById("commentText");
    textarea.style.height = "auto";
    textarea.style.height = Math.max(textarea.scrollHeight, 100) + "px";
}


function makeDialog() {
    $("#dialog").kendoDialog({
        title: "Enter Data",
//        show: function (e) {
 //           console.log('the dialog is showing');
 //       },
        //close: function (e) {
        //    let comm = $('#commentText').val();
        //    adjustSize();
        //    let time = $('#timeText').val();
        //    if ((comm != null && comm.length > 0) && (time != null && time.length > 0)) {
        //        return true;
        //    }
        //    if ( (comm==null ||  comm.length == 0) && time.length > 0 || (time == null || time.length == 0) && comm.length > 0) {
        //        alert('you canno do that dude');
        //        return false;
        //    }

        //},
        content: '<div class="k-editor-dialog k-popup-edit-form k-edit-form-container" style="width:auto;height:auto">' +
            '<div class="row">' +
            '<div class=col-md-1>' +
            '<p>Time</p>' +
            '</div>' +
            '<div class=col-md-8>' +
            '<div id="timeErrorMsg" style="visibility:hidden; color:red" >Error: Time must be of the format: HH:MM HH is optional </div > ' +
            '<p><textarea id="timeText"  onblur="checkInput()"  placeholder="hh:mm" title="textAreaTitle" cols="60" rows="1" style="width:30%"></textarea></p>' +
            '</div>' +
            '<div class="row">' +
            '<div class=col-md-1>' +
            '<p>Comment</p>' +
            '</div>' +
            '<div class=col-md-8>' +
            '<div id="commentErrorMsg" style="visibility:hidden; color:red" >Error: Missing comment</div > ' +
            '<p><textarea id="commentText"  min-height = "150px" title="textAreaTitle" cols="60"  style="width:95%"></textarea></p>' +
            '</div>' +
            '</div>',
        open: function () {
        },
        show: function () {
            console.log('the dialog is showing');
            let time = $('#timeText').val();
            var dialog = $('#dialog');
            let comment = $('#commentText').val();
            var test = comment.length;
            if (time.length > 0 && comment.length > 0) {
                $('#timeText').prop("readonly", false);
                $('#timeText').focus();
            }
            else if (time.length > 0 && comment.length < 1) {
                $('#commentText').prop("readonly", false);
                $('#commentText').focus();

            }
            else {
                $('#timeText').prop("readonly", false);
                $('#timeText').focus();
            }
        },
        visible: false, actions: [
            {
                text: 'Save ', action: function (e) {
                    $("#commentErrorMsg").css("visibility", "hiden");
                    $("#timeErrorMsg").css("visibility", "hidden");
                    var baseUrl = $('#baseURL').val();
                    var updateCellUrl = $('#saveCellDataURL').val();
                    var fullURL = baseUrl + updateCellUrl;
                    var res = saveComment(e);
                    var com = res.comment;
                    var time = res.timeMatched;

                    if (com == false || time == false) {
                        if (com == false) {
                            $("#commentErrorMsg").css("visibility", "visible");
                        }
                        if (time == false) {
                            $("#timeErrorMsg").css("visibility", "visible");
                        }
                        return false;
                    }
                    else {
                        return true;
                    }
                }
            }
        ]
    });
}

function CheckInputAndSave() {

}

function openDialog(e) {
  //  console.log('here is opoendialog e value: ',e);
    let range = getActiveRange();
    $('#cellInitialCol').val(range._ref.topLeft.col);
    $('#cellInitialRow').val(range._ref.topLeft.row);

    var dialog = $("#dialog").data("kendoDialog");

    var cellValues = getCellValues(e);
    $('#commentText').val(cellValues.comment);
    adjustSize();
    $('#timeText').val(cellValues.value);
    dialog.open();
    var button = $(dialog.wrapper).find("a[title='Close']").click(function (e) {
        let col = parseInt($('#cellInitialCol').val());
        let row = parseInt($('#cellInitialRow').val());
        let selectedMonth = $('#selectedMonth').val();
        let selectedYear = $('#selectedYear').val();
        var spreadsheet = $("#spreadsheet").data("kendoSpreadsheet");
        var sheet = spreadsheet.activeSheet();
        let range = sheet.range(row, 0, 1, 1);
        let client = range.value();
        range = sheet.range(row, 1, 1, 1);
        let project = range.value();
        range = sheet.range(row, 2, 1, 1);
        let activity = range.value();
        reloadCellData(client, project, activity, "", col, row, selectedYear, selectedMonth);

    });

  //  var d1 = $("#dialog").click(function (e) { e.preventDefault();  });

}

function getActiveRange() {
    var spreadsheet = $("#spreadsheet").data("kendoSpreadsheet");
    var sheet = spreadsheet.activeSheet();
    var cell = sheet.activeCell();
    var range = sheet.range(cell);
    return range;
}


function loadSpreadSheeet() {
    console.log('we never get here');
    var spreadsheet = $("#spreadsheet").kendoSpreadsheet({
        //select: function () {
        //    makeDialog();
        //    openDialog();
        //}, 
        change: onchange,
        select: (function (e) {console.log('focused') }),
        toolbar: {
            home: ["borders", {
                type: "button",
                text: "Comments",
                showText: "both",
                icon: "k-icon k-i-cog",
                click: function () {
                    makeDialog();
                    openDialog();
                },
            },
                {
                    type: "splitButton",
                    text: "Select Year",
                    menuButtons: [
                        { id: "1", text: "2002" },
                        { id: "2", text: "2003" }
                    ]
                },

            ]
        },
    });
    var spreadsheet = $("#spreadsheet").data("kendoSpreadsheet"); 

    var contextMenu = spreadsheet._view.cellContextMenu;

    contextMenu.append(
        [{
            type: "button",
            text: "MyButton",
            id: "foo",
            click: function (e) {
                console.log("click", e.target.text());
            }
        },
        ])

    contextMenu.bind("select", function (e) {
        var command = $(e.item).text();
        if (command == "MyButton") {
            makeDialog();
            openDialog();
        }
    })

    loadColumnTitles();

    var sheet = spreadsheet.activeSheet();
    sheet.rowHeight(0, 40);
    //var list = $('.k-spreadsheet-cell-context-menu li:first-child');
    //var list = $('.k-spreadsheet-cell-context-menu li:first-child').remove();
    //var list = $('.k-spreadsheet-cell-context-menu li:first-child').remove();
    //var list = $('.k-spreadsheet-cell-context-menu li:first-child').remove();
    //var list = $('.k-spreadsheet-cell-context-menu li:first-child').remove();
    //var list = $('.k-spreadsheet-cell-context-menu li:first-child').remove();

}



function addColumnTitles(sheet) {
    var values2 = [];
    values = $('#columnTitles').val();
    var arr = values.split(',');
    let len = arr.length;
    sheet.resize(2, len);
    range = sheet.range(0, 0, 1, len);
    values2.push(arr);
    range.values(values2);
    range = sheet.range(0, 4, 1, len - 4);
    range.textAlign("right");

}

function newAddColumnTitles(sheet) {
    values = $('#columnTitles').val();
    var arr = values.split(',');
    let len = arr.length;
    sheet.resize(2, len);

    range = sheet.range(0, 0, 1, 4);
    var values2 = [];
    for (let i = 0; i < 4; i++) {
        range = sheet.range(0, i, 1, 1);
        range.value(arr[i]);
        range.textAlign("right");
    }


    for (let i = 4; i < arr.length; i++) {
        let javaDate1 = makeJavaScriptDate(arr[i]);
        arr[i] = javaDate1;
        range = sheet.range(0, i, 1, 1);
        range.value(javaDate1);
        range.format("ddd\n\MMM-dd");
        range.textAlign("center");
    //    let test = range.value();
    //    let day = javaDate1.getDay();
    //    if (day==0 || day == 6) {
    //        range = sheet.range(0, i, 1, 1);
    //        range.background("#F5F5F5");
    //    }
    }
   
//    range = sheet.range(0, 6, 1, 1);
//    let date1 = arr[9];
//    console.log('date1 is ', date1);
//    let javaDate1 = makeJavaScriptDate(date1);
//    console.log('javeDate1 is: ', javaDate1);
//    console.log('titles are: ', values);
//    range.value(javaDate1);
}


function GetRange(firstColumn, lastColumn, firstRow, rows) {
    var spreadsheet = $("#spreadsheet").data("kendoSpreadsheet");
    var r1 = firstColumn + firstRow.toString() + ":" + lastColumn + (firstRow + rows - 1).toString();
    var sheet = spreadsheet.activeSheet();
    var range = sheet.range(r1);
    return range;
}

function saveActivity(client, project, activity, user, value, year, month, day,arg) {

    var spreadsheet = $("#spreadsheet").data("kendoSpreadsheet");
    var sheet = spreadsheet.activeSheet();
    var cell = sheet.activeCell();
    var range = sheet.range(cell);

    var baseUrl = $('#baseURL').val();
    var updateCellUrl = $('#updateCellValueURL').val();
    var fullURL = baseUrl + updateCellUrl;
    var loading = $('#isLoading').val();
    if (loading == 'false') {

        var updateValue = {"client": client, "project": project, "activity": activity, "user": user, "value": value.toString(), "year": year, "month": month, "day": day.toString()};
        updateValue = JSON.stringify(updateValue);
        console.log('save values are:', updateValue);
        $.ajax({
            url: fullURL,
            type: 'POST',
            contentType: "application/json",
            data: updateValue,
        })
            .done(function (data) {
                console.log('it worked');
            })
            .fail(function (error) {
                console.log('save error');
            }).
            always(function () {
                console.log('done')
            })
    }
    else {
        console.log(loading);
    }

}

function executeDeleteCell(spreadsheet) {
    let sheet = spreadsheet.activeSheet();
    console.log('yup this is deletecell');
    selection = sheet.selection();
    console.log('cell being deleted is: ', selection._ref);
    let firstRow = selection._ref.topLeft.row;
    let firstColumn = selection._ref.topLeft.col;
    values = $('#columnTitles').val();
    var titles = [];
    titles = values.split(',');
    console.log('titles are ', titles);
    let dateToDelete = titles[firstColumn];
    console.log('date to delete is: ', dateToDelete);
    let lastRow = selection._ref.bottomRight.row;
    let lastColumn = selection._ref.bottomRight.col;
    let numberOfRows = lastRow - firstRow + 1;
    let numberOfColumns = lastColumn - firstColumn + 1;
    for (var i = 0; i < numberOfRows; i++) {
        for (var c = 0; c < numberOfColumns; c++) {
            let range = sheet.range(firstRow + i, firstColumn + c, 1, 1);
            range.value(null);
            range.comment(null);
            var date = getDateFromColumn(firstColumn + c);
            console.log('date to remove is: ', date);
            console.log('first row is ', firstRow)
            removeCellDataWithDate(sheet,firstRow + i,date);
        }
    }
}

function getDateFromColumn(column) {
    values = $('#columnTitles').val();
    var titles = [];
    titles = values.split(',');
    console.log('titles are ', titles);
    let dateToDelete = titles[column];

    return dateToDelete;
}

function executeMoveCell(spreadsheet) {
    var res = executeCopyCell(spreadsheet);
    console.log("the return value is ", res);
    if (res) {
        executeDeleteCell(spreadsheet);
    }
}

function  executeCopyCell(spreadsheet) {
    var sheet = spreadsheet.activeSheet(),
    selection = sheet.selection();
    let firstRow = selection._ref.topLeft.row;
    let firstColumn = selection._ref.topLeft.col;
    let lastRow = selection._ref.bottomRight.row;
    let lastColumn = selection._ref.bottomRight.col;
    let numberOfRows = lastRow - firstRow + 1;
    let numberOfColumns = lastColumn - firstColumn + 1;
    if (numberOfRows > 1 || numberOfColumns > 1) {
        alert('You can only copy from 1 cell but can copy to many.');
        return false;
    }
    let row = selection._ref.bottomRight.row;
    let column = selection._ref.bottomRight.col;
    let range = sheet.range(row, column, 1, 1);
    let text = range.value();
    let comm = range.comment();
    if (text.length > 0 && comm.length > 0) {
        $('#copiedText').val(text);
        $('#copiedComment').val(comm);
    }
    return true;
}

function executePaste(spreadsheet){
    var text = $('#copiedText').val();
    var comm = $('#copiedComment').val();
    if ((text != null && text != 'undefinded' && text.length > 0) && (comm != null && comm != 'undefinded' && comm.length > 0)) {
        var sheet = spreadsheet.activeSheet(),
            selection = sheet.selection();
        let firstRow = selection._ref.topLeft.row;
        let firstColumn = selection._ref.topLeft.col;
        let lastRow = selection._ref.bottomRight.row;
        let lastColumn = selection._ref.bottomRight.col;
        let numberOfRows = lastRow - firstRow + 1;
        let numberOfColumns = lastColumn - firstColumn + 1;
        for (var i = 0; i < numberOfRows; i++) {
            for (var c = 0; c < numberOfColumns; c++) {
                let range = sheet.range(firstRow + i, firstColumn + c, 1, 1);
                range.value(text);
                range.comment(comm);
                saveData(firstColumn + c, firstRow + i, comm, text);
            }
        }
    }

    if (text.length > 0 && comm.length > 0) {
        $('#copiedText').val(text);
        $('#copiedComment').val(comm);
    }
}

function reloadCellData(client, project, activity, user, col,row, year, month) {
    console.log('reloading');
    var spreadsheet = $("#spreadsheet").data("kendoSpreadsheet");
    var sheet = spreadsheet.activeSheet();
 //   var cell = sheet.activeCell();
    var range = sheet.range(row, col, 1, 1);
    //range.comment("");
    //range.value("");
    var allDates = $('#newAllDates').val();
    var datesArray = allDates.split(',');
//    let col = cell.bottomRight.col;
////    var col = parseInt(column);
     var thisDate = datesArray[col];

     var baseUrl = $('#baseURL').val();
     var updateCellUrl = $('#getCellDataURL').val();
     var fullURL = baseUrl + updateCellUrl;
//     var updateValue = { "client": client[0], "project": project[0], "activity": activity[0], "comment": "", "time": "",  "date": thisDate };
    var updateValue = { "client": client, "project": project, "activity": activity, "user": user, "date": thisDate.toString() };
    let newUpdateValue = JSON.stringify(updateValue);
//    console.log(newUpdateValue);

    $.ajax({
        url: fullURL,
        type: 'POST',
        contentType: "application/json",
        data: newUpdateValue,
    })
        .done(function (data) {
            if (data != null) {
                let data1 = JSON.parse(data);
                let t1 = data1.Time;
                console.log('data 1 is ', data1);
                console.log('t1 is ', t1);
                range.value(data1.Time==null ? "" : data1.Time);
                range.comment(data1.Comment == null ? "" : data1.Comment);
            //    if (data.time == null) {
            //        console.log('we are not where we should be ',data);
            //        range.value("");
            //        range.comment("");
            //    }
            //    else {
            //        console.log('we are where we should be');
            //        rage.value(data.time);
            //        range.comment(data.comment);
            //    }
            }
            else {
                console.log('apparently it is null');
            }
        })
        .fail(function (error) {
            console.log('save error');
        }).
        always(function (data) {
        })

}

function saveCellData(client, project, activity, user, comment, time,column,range) {

    var spreadsheet = $("#spreadsheet").data("kendoSpreadsheet");
    var sheet = spreadsheet.activeSheet();
    var allDates = $('#newAllDates').val();
    var datesArray = allDates.split(',');
    var col = parseInt(column);
    var thisDate = datesArray[col];

 //   console.log('the new date value is: ', thisDate);


    var baseUrl = $('#baseURL').val();
    var updateCellUrl = $('#saveCellDataURL').val();
    var fullURL = baseUrl + updateCellUrl;
    var loading = $('#isLoading').val();
    if (loading == 'false') {
        console.log('full url is ', fullURL);
        var updateValue = { "client": client, "project": project, "activity": activity, "user": user, "comment": comment.toString(), "time": time.toString(), "date": thisDate };
        updateValue = JSON.stringify(updateValue);
        $.ajax({
            url: fullURL,
            type: 'POST',
            contentType: "application/json",
            data: updateValue,
            success: function (response) {
                if (response.success) {
                }
                else {
                    alert('Update failed to save. Contact admin or try again later ');
                    location.reload();
                }
            },
            error: function (xhr, status, error) {
                alert('Update failed to save. If this continues, please contact Admin: ');
                location.reload();
            },
            //statusCode: {
            //    404: function () {
            //        alert('Update failed to save. Website did not respond: ');
            //        location.reload();
            //    }
            //}

        })

    //        .done(function (data) {
    //            console.log('it worked');
    //        })
    //        .fail(function (error) {
    //            console.log('save error');
    //            range.comment("");
    //            range.value("");
    //            alert('Update failed to save. Contact admin or try again later ');

    //        }).
    //        always(function () {
    //          //  console.log('done')
    //        })
    }
    else {
        console.log(loading);
    }

}


function saveCellComment(client, project, activity, user, comment, time, year, month, day) {

    var spreadsheet = $("#spreadsheet").data("kendoSpreadsheet");
    var sheet = spreadsheet.activeSheet();

    var baseUrl = $('#baseURL').val();
    var updateCellUrl = $('#updateCellCommentURL').val();
    var fullURL = baseUrl + updateCellUrl;
    var loading = $('#isLoading').val();
    if (loading == 'false') {
 
        var updateValue = { "client": client, "project": project, "activity": activity, "user": user, "comment": comment.toString(), "time": time.toString(), "year": year, "month": month, "day": day.toString() };
        updateValue = JSON.stringify(updateValue);
        $.ajax({
            url: fullURL,
            type: 'POST',
            contentType: "application/json",
            data: updateValue,
        })
            .done(function (data) {
                console.log('it worked');
            })
            .fail(function (error) {
                console.log('save error');
            }).
            always(function () {
                console.log('done')
            })
    }
    else {
        console.log(loading);
    }

}

function deleteCellContents(client, project, activity, user, year, month, day) {

    var spreadsheet = $("#spreadsheet").data("kendoSpreadsheet");
    var sheet = spreadsheet.activeSheet();

    var baseUrl = $('#baseURL').val();
    var deleteCellContentUrl = $('#deleteCellContentURL').val();
    var fullURL = baseUrl + deleteCellContentUrl;

    var updateValue = { "client": client, "project": project, "activity": activity, "user": user, "comment": '', "time": '', "year": year.toString(), "month": month.toString(), "day": day.toString() };
    updateValue = JSON.stringify(updateValue);
    console.log('full url for delete is ', fullURL);
    $.ajax({
        url: fullURL,
        type: 'POST',
        contentType: "application/json",
        data: updateValue,
    })
        .done(function (data) {
            console.log('it worked ', data);
        })
        .fail(function (error) {
            console.log('save error');
        }).
        always(function () {
            console.log('done')
        })
}












