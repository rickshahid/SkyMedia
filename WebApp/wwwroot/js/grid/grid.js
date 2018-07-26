var _childGridId, _childPropertyName, _spacingPatterns, _spacingInserts;
function LoadSubGrid(parentRowId, parentRowKey) {
    var parentRow = $(this).jqGrid("getLocalRow", parentRowKey);
    var childRows = parentRow[_childPropertyName];
    var columns = GetChildColumns(_childGridId);
    $("#" + parentRowId).html("<table id='" + _childGridId + "'></table>");
    $("#" + _childGridId).jqGrid({
        colModel: columns,
        datatype: "local",
        data: childRows,
        loadComplete: ClearTitles,
        sortname: "name"
    });
}
function LoadGrid(gridId, rows, columns) {
    SetRowIds(rows);
    $("#" + gridId).jqGrid({
        colModel: columns,
        datatype: "local",
        data: rows,
        loadComplete: ClearTitles,
        subGrid: _childGridId != null,
        subGridOptions: {
            "openicon": "ui-icon-arrowreturnthick-1-e"
        },
        subGridRowExpanded: LoadSubGrid,
        pager: "gridPager",
        sortname: "name",
        height: "auto",
        rowNum: 10
    });
    for (var i = 0; i < rows.length; i++) {
        var rowId = rows[i].id;
        CreateTipTop(rowId + "_cancel", "Cancel");
        CreateTipTop(rowId + "_publish", "Publish");
        CreateTipTop(rowId + "_edit", "Edit");
        CreateTipTop(rowId + "_delete", "Delete");
    }
}
function SetRowIds(rows) {
    for (var i = 0; i < rows.length; i++) {
        var row = rows[i];
        if (row.id != null) {
            var id = row.id.split("/");
            row.id = id[id.length - 1];
            if (id.length > 12) {
                row.parentEntityName = id[id.length - 3];
            }
            row.id = row.id.replace(/ /g, "_");
            row.id = row.id.replace(/,/g, "");
            row.id = row.id.replace(/\(/g, "");
            row.id = row.id.replace(/\)/g, "");
        }
    }
}
function ClearTitles(grid) {
    var tdElements = document.getElementsByTagName("td");
    for (var i = 0; i < tdElements.length; i++) {
        tdElements[i].title = "";
    }
}