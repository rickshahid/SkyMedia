var _childGridId, _childPropertyName, _spacingPatterns, _spacingInserts;
function LoadSubGrid(parentRowId, parentRowKey) {
    var parentRow = $(this).jqGrid("getLocalRow", parentRowKey);
    var childRows = parentRow[_childPropertyName];
    var columns = GetColumns(_childGridId, "Name");
    $("#" + parentRowId).html("<table id='" + _childGridId + "'></table>");
    $("#" + _childGridId).jqGrid({
        colModel: columns,
        datatype: "local",
        data: childRows,
        loadComplete: ClearTitles
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
function SetRowEdit(gridId, rowData) {
    var row = JSON.parse(decodeURIComponent(rowData));
    $("#name").val(row.name);
    $("#description").val(row["properties.description"]);
    switch (gridId) {
        case "transforms":
            for (var i = 0; i < 3; i++) {
                $("#presetEnabled" + i).prop("checked", false);
                $("#relativePriority" + i + " option").prop("selected", function () {
                    return this.defaultSelected;
                });
                $("#onErrorMode" + i + " option").prop("selected", function () {
                    return this.defaultSelected;
                });
            }
            var outputs = row["properties.outputs"];
            for (var i = 0; i < outputs.length; i++) {
                var presetIndex;
                var output = outputs[i];
                var preset = output.preset;
                if (preset.presetName != null) {
                    presetIndex = 0;
                    $("#presetName0").val(preset.presetName);
                } else if (preset.audioInsightsOnly != null) {
                    presetIndex = 1;
                } else {
                    presetIndex = 2;
                }
                $("#presetEnabled" + presetIndex).prop("checked", true);
                $("#relativePriority" + presetIndex).val(output.relativePriority);
                $("#onErrorMode" + presetIndex).val(output.onError);
            }
            break;
    }
}
function ClearTitles(grid) {
    var tdElements = document.getElementsByTagName("td");
    for (var i = 0; i < tdElements.length; i++) {
        tdElements[i].title = "";
    }
}