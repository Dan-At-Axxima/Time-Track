function GetActivityList() {
    var baseURL = $('#TestBaseURL').val();
    var activityListURL = $('#ActivityListURL').val();
    var fullUrl = baseURL + activityListURL;
    $.ajax({
        url: fullURL,
        type: 'GET',
        contentType: "application/json",
        success: function (data) {
            //            displayMessage("Prouducts Retrieve");
            console.log(JSON.stringify(data));
        },
        error: function (error) {
            console.log("error");
        },
        complete: function () {
            console.log("In the complete function");
        }

    });
}

function onGridKeydown(e) {
    console.log('key down');
    var grid = $('#grid').data("kendoGrid");
    console.log(grid);
    if (e.keyCode >= 37 && e.keyCode <= 40) {
        console.log(this);
            var row = $(this).closest("tr");
            var rowIndex = $("tr", grid.tbody).index(row);
        var colIndex = $("td", row).index(this);
        console.log(rowIndex);
        console.log(colIndex);
        rowIndex = 0;
        colIndex = 1;
            var direction = "";
            grid.closeCell();

            if (e.keyCode == 37) {//left
                colIndex--;
                direction = "h";
            }
            if (e.keyCode == 38) {//up
                rowIndex--;
                direction = "v";
            }
            if (e.keyCode == 39) {//right
                colIndex++;
                direction = "h";
            }
            if (e.keyCode == 40) {//down
                rowIndex++;
                direction = "v";
            }

        var cell = $("#grid tbody tr:eq(" + rowIndex + ") td:eq(" + colIndex + ")");

            if (cell.length > 0) {
                grid.editCell(cell);
            }
            else {
                var cellSelector = "";

                if (direction == "h") {
                    cellSelector = "#grid tbody tr:eq(" + rowIndex + ") td:eq(0)";
                }
                else {
                    cellSelector = "#grid tbody tr:eq(0) td:eq(" + colIndex + ")";
                }
                console.log(cellSelector);
            }
        
    };

}

function onGridKeydown2(e) {
    if (e.keyCode === kendo.keys.TAB) {
        console.log("key down");
        var grid = $(this).closest("[data-role=grid]").data("kendoGrid");
        var current = grid.current();
        if (!current.hasClass("editable-cell")) {
            var nextCell;
            if (e.shiftKey) {
                nextCell = current.prevAll(".editable-cell");
                if (!nextCell[0]) {
                    //search the next row
                    var prevRow = current.parent().prev();
                    var nextCell = prevRow.children(".editable-cell:last");
                }
            } else {
                nextCell = current.nextAll(".editable-cell");
                if (!nextCell[0]) {
                    //search the next row
                    var nextRow = current.parent().next();
                    var nextCell = nextRow.children(".editable-cell:first");
                }
            }
            grid.current(nextCell);
            grid.editCell(nextCell[0]);
        }
    };
}