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
function OnGridLoad(grid) {
    ClearTitles();
    if (_childGridType == "transformJobOutputs") {
        var rows = grid.rows;
        for (var i = 0; i < rows.length; i++) {
            var row = rows[i];
            if (row["properties.state"] == "Processing") {
                $(this).expandSubGridRow(row.id);
            }
        }
    }
}
function LoadGrid(gridId, rows) {
    var columns = GetParentColumns(gridId);
    SetParentRowIds(gridId, rows);
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
    if (gridId == "transformJobs") {
        var refreshJobs = function () {
            RefreshJobs(gridId);
        };
        setInterval(refreshJobs, 10000);
    }
}
function LoadSubGrid(parentRowId, parentRowKey) {
    var parentRow = $(this).jqGrid("getLocalRow", parentRowKey);
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
function RefreshJobs(gridId) {
    var transformNames = new Array();
    var jobNames = new Array();
    var rows = $("#" + gridId).jqGrid("getRowData");
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
                SetParentRowIds(gridId, jobs);
                $("#" + gridId).jqGrid("setGridParam", {
                    datatype: "local",
                    data: jobs
                });
                $("#" + gridId).trigger("reloadGrid");
            }
        );
    }
}