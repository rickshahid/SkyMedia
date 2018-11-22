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
        CreateTipTop(rowId + "_edit", "Edit");
        CreateTipTop(rowId + "_publish", "Publish");
        CreateTipTop(rowId + "_unpublish", "Unpublish");
        CreateTipTop(rowId + "_insight", "Insight");
        CreateTipTop(rowId + "_reindex", "Reindex");
        CreateTipTop(rowId + "_start", "Start");
        CreateTipTop(rowId + "_stop", "Stop");
        CreateTipTop(rowId + "_reset", "Reset");
        CreateTipTop(rowId + "_cancel", "Cancel");
        CreateTipTop(rowId + "_download", "Download");
        CreateTipTop(rowId + "_delete", "Delete");
    }
}
function SetParentRowIds(gridId, rows) {
    if (gridId != "indexerInsights") {
        for (var i = 0; i < rows.length; i++) {
            var row = rows[i];
            var rowId = row.name;
            rowId = rowId.replace(/ /g, "-");
            rowId = rowId.replace(/,/g, "-");
            rowId = rowId.replace(/\(/g, "-");
            rowId = rowId.replace(/\)/g, "-");
            row.parentName = GetParentName(row);
            row.id = rowId;
        }
    }
}
function SetChildRowIds(parentRow, childRows) {
    for (var i = 0; i < childRows.length; i++) {
        childRows[i].parentName = parentRow.name;
        childRows[i].id = parentRow.id + "-" + i;
   }
}
function OnGridLoad(gridData, gridId, gridRows) {
    ClearTitles();
    if (_childGridType == "transformJobOutputs") {
        if (gridData != null) {
            gridRows = gridData.rows;
        }
        for (var i = 0; i < gridRows.length; i++) {
            var gridRow = gridRows[i];
            if (gridRow["properties.state"] == "Processing") {
                if (gridData != null) {
                    $(this).expandSubGridRow(gridRow.id);
                } else {
                    $("#" + gridId).collapseSubGridRow(gridRow.id);
                    $("#" + gridId).expandSubGridRow(gridRow.id);
                }
            }
        }
    }
}
function LoadGrid(gridId, rows) {
    var columns = GetParentColumns(gridId);
    SetParentRowIds(gridId, rows);
    $("#" + gridId).jqGrid({
        multiselect: gridId == "assets",
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
    switch (gridId) {
        case "transformJobs":
            var refreshJobs = function () {
                RefreshJobs(gridId);
            };
            setInterval(refreshJobs, 10000);
            break;
        case "indexerInsights":
            var refreshInsights = function () {
                RefreshInsights(gridId);
            };
            setInterval(refreshInsights, 10000);
            break;
    }
}
function LoadSubGrid(parentRowId, parentRowKey) {
    var parentRow = $(this).getLocalRow(parentRowKey);
    var childRows = parentRow[_childPropertyName];
    var columns = GetChildColumns(_childGridType);
    var childGridId = parentRowId + "_" + _childPropertyName.replace("properties.", "");
    SetChildRowIds(parentRow, childRows);
    $("#" + parentRowId).html("<table id='" + childGridId + "'></table>");
    $("#" + childGridId).jqGrid({
        loadComplete: OnGridLoad,
        colModel: columns,
        datatype: "local",
        sortname: "name",
        data: childRows,
        rowNum: 100
    });
    CreateTips(childRows);
}
function RefreshGrid(gridId, gridRows) {
    SetParentRowIds(gridId, gridRows);
    for (var i = 0; i < gridRows.length; i++) {
        var gridRow = gridRows[i];
        $("#" + gridId).setRowData(gridRow.id, gridRow);
    }
    $("#" + gridId).jqGrid("setGridParam", {
        datatype: "local",
        data: gridRows
    });
    OnGridLoad(null, gridId, gridRows);
}
function RefreshJobs(gridId) {
    var transformNames = new Array();
    var jobNames = new Array();
    var rows = $("#" + gridId).getRowData();
    for (var i = 0; i < rows.length; i++) {
        var row = rows[i];
        if (row["properties.state"] == "Queued" || row["properties.state"] == "Scheduled" || row["properties.state"] == "Processing") {
            transformNames.push(row.parentName);
            jobNames.push(row.name);
        }
    }
    if (jobNames.length > 0) {
        $.post("/job/refresh",
            {
                transformNames: transformNames,
                jobNames: jobNames
            },
            function (jobs) {
                RefreshGrid(gridId, jobs);
            }
        );
    }
}
function RefreshInsights(gridId) {
    var insightIds = new Array();
    var rows = $("#" + gridId).getRowData();
    for (var i = 0; i < rows.length; i++) {
        var row = rows[i];
        if (row["state"] == "Processing") {
            insightIds.push(row.id);
        }
    }
    if (insightIds.length > 0) {
        $.post("/insight/refresh",
            {
                insightIds: insightIds
            },
            function (insights) {
                RefreshGrid(gridId, insights);
            }
        );
    }
}