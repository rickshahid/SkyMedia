var _childGridType, _childPropertyName;
function ClearTitles() {
    var tableCells = document.getElementsByTagName("td");
    for (var i = 0; i < tableCells.length; i++) {
        tableCells[i].title = "";
    }
}
function CreateTips(rows) {
    for (var i = 0; i < rows.length; i++) {
        var rowId = rows[i].id;
        CreateTipTop(rowId + "_video", "Video");
        CreateTipTop(rowId + "_insight", "Insight");
        CreateTipTop(rowId + "_json", "Metadata");
        CreateTipTop(rowId + "_manifest", "Manifest");
        CreateTipTop(rowId + "_transcript", "Transcript");
        CreateTipTop(rowId + "_reindex", "Reindex");
        CreateTipTop(rowId + "_publish", "Publish");
        CreateTipTop(rowId + "_download", "Download");
        CreateTipTop(rowId + "_edit", "Edit");
        CreateTipTop(rowId + "_clip", "Clip");
        CreateTipTop(rowId + "_cancel", "Cancel");
        CreateTipTop(rowId + "_delete", "Delete");
        CreateTipTop(rowId + "_start", "Start");
        CreateTipTop(rowId + "_stop", "Stop");
        CreateTipTop(rowId + "_reset", "Reset");
    }
}
function SetParentRowIds(rows) {
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
function SetChildRowIds(parentRow, childRows) {
    for (var i = 0; i < childRows.length; i++) {
        var childRowId = parentRow.id + "-" + i;
        childRows[i].id = childRowId;
        childRows[i].parentEntityName = parentRow.name;
   }
}
function LoadGrid(gridId, rows, columns) {
    SetParentRowIds(rows);
    $("#" + gridId).jqGrid({
        colModel: columns,
        datatype: "local",
        data: rows,
        loadComplete: OnGridLoad,
        subGrid: _childGridType != null,
        subGridOptions: {
            "openicon": "ui-icon-arrowreturnthick-1-e"
        },
        subGridRowExpanded: LoadSubGrid,
        pager: "gridPager",
        sortname: "name",
        height: "auto",
        rowNum: 5
    });
    CreateTips(rows);
}
function LoadSubGrid(parentRowId, parentRowKey) {
    var parentRow = $(this).jqGrid("getLocalRow", parentRowKey);
    var childRows = parentRow[_childPropertyName];
    var columns = GetChildColumns(_childGridType);
    SetChildRowIds(parentRow, childRows);
    var childGridId = parentRowId + "_" + _childPropertyName.replace("properties.", "");
    $("#" + parentRowId).html("<table id='" + childGridId + "'></table>");
    $("#" + childGridId).jqGrid({
        colModel: columns,
        datatype: "local",
        data: childRows,
        gridComplete: ClearTitles,
        sortname: "name",
        rowNum: 100
    });
    CreateTips(childRows);
}
function ReloadGrid(gridId, relativeUrl, columns) {
    $.get(relativeUrl,
        function (rows) {
            SetParentRowIds(rows);
            $("#" + gridId).jqGrid("clearGridData");
            $("#" + gridId).jqGrid("setGridParam", {
                datatype: "local",
                data: rows
            });
            $("#" + gridId).trigger("reloadGrid");
        }
    );
}
function OnGridLoad(grid) {
    ClearTitles();
    if (_childGridType == "transformJobOutputs") {
        var rows = grid.rows;
        for (var i = 0; i < rows.length; i++) {
            var jobState = rows[i]["properties.state"];
            if (jobState == "Processing") {
                var rowId = rows[i].id;
                $(this).expandSubGridRow(rowId);
            }
        }
    }
}