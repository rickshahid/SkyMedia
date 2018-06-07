function GetColumns(gridId) {
    var columns;
    switch (gridId) {
        case "transforms":
            columns = [
                {
                    width: 500,
                    name: "name",
                    label: "Transform Name",
                    align: "center"
                },
                {
                    width: 100,
                    label: "Actions",
                    formatter: FormatActions,
                    align: "center"
                }
            ];
            break;
        case "transformJobs":
            columns = [
                {
                    width: 500,
                    name: "name",
                    label: "Transform Job Name",
                    align: "center"
                },
                {
                    width: 150,
                    name: "properties.state",
                    label: "Job State",
                    align: "center"
                },
                {
                    width: 100,
                    label: "Actions",
                    formatter: FormatActions,
                    align: "center"
                }
            ];
            break;
    }
    return columns;
}
function LoadGrid(gridId, columns, rows, storageCdnUrl) {
    _storageCdnUrl = storageCdnUrl;
    $("#" + gridId).jqGrid({
        colModel: columns,
        datatype: "local",
        data: rows,
        loadComplete: ClearTitles,
        viewsortcols: [false, "horizontal", true],
        sortname: "name"
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
function FormatActions(value, grid, row) {
    var deleteHtml = "";
    var entityName = row.name;
    if (entityName.indexOf("Predefined_") == -1) {
        var parentEntityName = GetParentEntityName(row.id);
        var onClick = "DeleteEntity('" + grid.gid + "','" + encodeURIComponent(entityName) + "','" + encodeURIComponent(parentEntityName) + "')";
        deleteHtml = "<button class='siteButton' onclick=" + onClick + ">";
        deleteHtml = deleteHtml + "<img src='" + _storageCdnUrl + "/MediaEntityDelete.png'></button>";
    }
    return deleteHtml;
}
function GetParentEntityName(childEntityId) {
    var parentEntityName = "";
    var idInfo = childEntityId.split("/");
    if (idInfo.length > 12) {
        parentEntityName = idInfo[10];
    }
    return parentEntityName;
}
function DeleteEntity(gridId, entityName, parentEntityName) {
    var title = "Confirm Delete";
    var message = "Are you sure you want to delete the '" + entityName + "' entity?";
    var onConfirm = function () {
        SetCursor(true);
        $.post("/account/deleteEntity",
            {
                gridId: gridId,
                entityName: entityName,
                parentEntityName: parentEntityName
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