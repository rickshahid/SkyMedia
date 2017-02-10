function LoadGrid(columns, rows) {
    $("#gridView").jqGrid({
        datatype: "local",
        colModel: columns,
        loadComplete: function () {
            for (var i = 0; i < rows.length; i++) {
                var rowId = rows[i].id;
                var row = document.getElementById(rowId);
                if (row != null) {
                    for (var x = 0; x < row.cells.length; x++) {
                        row.cells[x].title = "";
                    }
                }
            }
        },
        sortname: "name",
        sortorder: "asc",
        height: 380
    });
    for (var i = 0; i < rows.length; i++) {
        $("#gridView").jqGrid("addRowData", rows[i].id, rows[i]);
    }
    $("#gridView").jqGrid("sortGrid", "name", false, "asc");
}
function FormatColumnName(value, grid, row) {
    var title = row.name;
    var message = row.id;
    if (row.description != null && row.description != row.name) {
        message = message + "<br /><br />" + row.description;
    }
    return "<span class=\"gridColumn\" onclick=\"DisplayMessage('" + title + "', '" + message + "', null, 600, null)\">" + value + "</span>";
}
function FormatColumnType(value, grid, row) {
    switch (value) {
        case 1:
            value = "1 (Azure Queue)";
            break;
        case 2:
            value = "2 (Azure Table)";
            break;
        case 3:
            value = "3 (Web Hook)";
            break;
    }
    return value;
}
