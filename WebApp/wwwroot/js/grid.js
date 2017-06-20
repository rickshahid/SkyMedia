function LoadGrid(gridId, columns, rows, height) {
    $("#" + gridId).jqGrid({
        colModel: columns,
        datatype: "local",
        data: rows,
        loadComplete: ClearTitles,
        viewsortcols: [false, "horizontal", true],
        sortname: "name",
        height: height
    });
}
function ClearTitles(grid) {
    for (var i = 0; i < grid.rows.length; i++) {
        var rowId = grid.rows[i].id;
        var row = document.getElementById(rowId);
        for (var x = 0; x < row.cells.length; x++) {
            row.cells[x].title = "";
        }
    }
}
function FormatColumn(value, grid, row) {
    var title = row.name;
    var message = row.id;
    if (row.description != null && row.description != row.name) {
        message = message + "<br /><br />" + row.description;
    }
    if (grid.colModel.name == "endPointType") {
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
    }
    return "<span onclick=\"DisplayMessage('" + title + "', '" + message + "', null, 600)\">" + value + "</span>";
}
