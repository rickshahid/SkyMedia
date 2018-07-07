﻿var _childGridId, _childPropertyName, _spacingPatterns, _spacingInserts;
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
        subGridRowExpanded: LoadSubGrid,
        pager: "gridPager",
        sortname: "name",
        height: "auto",
        rowNum: 10
    });
    for (var i = 0; i < rows.length; i++) {
        var rowId = rows[i].id;
        CreateTipTop(rowId + "_cancel", "Cancel");
        CreateTipTop(rowId + "_edit", "Edit");
        CreateTipTop(rowId + "_delete", "Delete");
    }
}
function SetRowIds(rows) {
    for (var i = 0; i < rows.length; i++) {
        var id = rows[i].id.split("/");
        rows[i].id = id[id.length - 1];
        if (id.length > 12) {
            rows[i].parentEntityName = id[id.length - 3];
        }
        rows[i].id = rows[i].id.replace(/ /g, "_");
        rows[i].id = rows[i].id.replace(/,/g, "");
        rows[i].id = rows[i].id.replace(/\(/g, "");
        rows[i].id = rows[i].id.replace(/\)/g, "");
    }
}
function ClearTitles(grid) {
    var tdElements = document.getElementsByTagName("td");
    for (var i = 0; i < tdElements.length; i++) {
        tdElements[i].title = "";
    }
}