var _childGridId, _childPropertyName, _spacingPatterns, _spacingInserts;
function GetColumns(gridId, nameLabel) {
    var columns;
    switch (gridId) {
        case "assetFiles":
            columns = [
                {
                    label: "File Name",
                    name: "name",
                    formatter: FormatName,
                    align: "center",
                    width: 330
                }
            ];
            break;
        case "transformOutputs":
            columns = [
                {
                    label: "Preset Name",
                    name: "preset.presetName",
                    formatter: FormatName,
                    align: "center",
                    width: 330
                },
                {
                    label: "Priority",
                    name: "relativePriority",
                    align: "center",
                    width: 100
                },
                {
                    label: "On Error Mode",
                    name: "onError",
                    formatter: FormatValue,
                    align: "center",
                    width: 200
                }
            ];
            break;
        case "transformJobOutputs":
            columns = [
                {
                    label: "Asset Name",
                    name: "assetName",
                    formatter: FormatName,
                    align: "center",
                    width: 325
                },
                {
                    label: "State",
                    name: "state",
                    align: "center",
                    width: 100
                },
                {
                    label: "Progress",
                    name: "progress",
                    formatter: FormatProgress,
                    align: "center",
                    width: 100
                }
            ];
            break;
        default:
            columns = [
                {
                    label: nameLabel,
                    name: "name",
                    formatter: FormatName,
                    align: "center",
                    width: 325
                },
                {
                    label: "Created",
                    name: "properties.created",
                    formatter: FormatDateTime,
                    align: "center",
                    width: 100
                },
                {
                    label: "Actions",
                    formatter: FormatActions,
                    align: "center",
                    sortable: false,
                    width: 100
                }
            ];
            if (gridId != "streamingPolicies" && gridId != "streamingLocators") {
                var modified = {
                    label: "Modified",
                    name: "properties.lastModified",
                    formatter: FormatDateTime,
                    align: "center",
                    width: 100
                };
                columns.splice(2, 0, modified);
            }
            switch (gridId) {
                case "transformJobs":
                    var priority = {
                        label: "Priority",
                        name: "properties.priority",
                        align: "center",
                        width: 100
                    };
                    var state = {
                        label: "State",
                        name: "properties.state",
                        align: "center",
                        width: 100
                    };
                    columns.splice(1, 0, priority, state);
                    break;
                case "streamingLocators":
                    var streamingPolicyName = {
                        label: "Streaming Policy Name",
                        name: "properties.streamingPolicyName",
                        align: "center",
                        width: 300
                    };
                    var startTime = {
                        label: "Start Time",
                        name: "properties.startTime",
                        formatter: FormatDateTime,
                        align: "center",
                        width: 120
                    };
                    var endTime = {
                        label: "End Time",
                        name: "properties.endTime",
                        formatter: FormatDateTime,
                        align: "center",
                        width: 120
                    };
                    columns.splice(1, 0, streamingPolicyName, startTime, endTime);
                    break;
            }
            break;
    }
    return columns;
}
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
function LoadGrid(gridId, columns, rows, childGridId, childPropertyName, spacingPatterns, spacingInserts, storageCdnUrl) {
    _childGridId = childGridId;
    _childPropertyName = childPropertyName;
    _spacingPatterns = spacingPatterns;
    _spacingInserts = spacingInserts;
    _storageCdnUrl = storageCdnUrl;
    SetRowIds(rows);
    $("#" + gridId).jqGrid({
        colModel: columns,
        datatype: "local",
        data: rows,
        loadComplete: ClearTitles,
        subGrid: childGridId != null,
        subGridRowExpanded: LoadSubGrid,
        pager: "gridPager",
        sortname: "name",
        height: "auto",
        rowNum: 10
    });
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
function FormatName(value, grid, row) {
    if (row.preset != null && row.preset.audioLanguage != null) {
        value = row.preset.audioInsightsOnly == null ? "Audio Analyzer" : "Video Analyzer";
    }
    value = FormatValue(value, grid, row);
    var description = row["properties.description"];
    if (description != null) {
        value = "<span onclick=DisplayMessage('Description','" + description + "')>" + value + "</span>";
    }
    return value;
}
function FormatValue(value, grid, row) {
    for (var i = 0; i < _spacingPatterns.length; i++) {
        var expression = new RegExp(_spacingPatterns[i], "g");
        value = value.replace(expression, _spacingInserts[i]);
    }
    return value;
}
function FormatProgress(value, grid, row) {
    return value + "%";
}
function FormatDateTime(value, grid, row) {
    if (value == null) {
        value = "N/A";
    } else {
        value = value.slice(11, 19) + "<br>" + value.slice(0, 10);
    }
    return value; 
}
function FormatActions(value, grid, row) {
    var actionsHtml = "";
    var entityName = row.name;
    if (entityName.indexOf("Predefined_") > -1) {
        actionsHtml = "N/A";
    } else {
        var editHtml = "";
        if (grid.gid != "streamingLocators") {
            editHtml = "<button class='siteButton' onclick=SetRowEdit('" + grid.gid + "','" + encodeURIComponent(JSON.stringify(row)) + "')>";
            editHtml = editHtml + "<img src='" + _storageCdnUrl + "/MediaEntityEdit.png'></button>";
        }
        var onDelete = "DeleteEntity('" + grid.gid + "','" + encodeURIComponent(entityName) + "','" + encodeURIComponent(row.parentEntityName) + "')";
        var deleteHtml = "<button class='siteButton' onclick=" + onDelete + ">";
        deleteHtml = deleteHtml + "<img src='" + _storageCdnUrl + "/MediaEntityDelete.png'></button>";
        actionsHtml = editHtml + deleteHtml;
    }
    return actionsHtml
}
function DeleteEntity(gridId, entityName, parentEntityName) {
    var title = "Confirm Delete";
    var message = "Are you sure you want to delete the '" + FormatValue(entityName) + "' entity?";
    var onConfirm = function () {
        SetCursor(true);
        $.post("/account/deleteEntity",
            {
                gridId: gridId,
                entityName: decodeURIComponent(entityName),
                parentEntityName: decodeURIComponent(parentEntityName)
            },
            function () {
                SetCursor(false);
                window.location = window.location.href;
            }
        );
        $(this).dialog("close");
    }
    ConfirmMessage(title, message, onConfirm);
}