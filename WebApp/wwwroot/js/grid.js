var _storageCdnUrl, _accountEntityType;
function ClearTitles(grid) {
    for (var i = 0; i < grid.rows.length; i++) {
        var rowId = grid.rows[i].id;
        var row = document.getElementById(rowId);
        for (var x = 0; x < row.cells.length; x++) {
            row.cells[x].title = "";
        }
    }
}
function LoadGrid(gridId, columns, rows, storageCdnUrl, accountEntityType) {
    _storageCdnUrl = storageCdnUrl;
    _accountEntityType = accountEntityType;
    $("#" + gridId).jqGrid({
        colModel: columns,
        datatype: "local",
        data: rows,
        loadComplete: ClearTitles,
        viewsortcols: [false, "horizontal", true],
        sortname: "name",
        height: 500
    });
}
function FormatColumn(value, grid, row) {
    var displayValue = "";
    switch (grid.colModel.name) {
        case "name":
            if (value == null) {
                value = row.id;
            }
            break;
        case "contentKeyType":
            switch (value) {
                case 0:
                    displayValue = "Common Encryption";
                    break;
                case 1:
                    displayValue = "Storage Encryption";
                    break;
                case 2:
                    displayValue = "Configuration Encryption";
                    break;
                case 3:
                    displayValue = "Url Encryption";
                    break;
                case 4:
                    displayValue = "Envelope Encryption";
                    break;
                case 6:
                    displayValue = "Common Encryption CBCS";
                    break;
                case 7:
                    displayValue = "FairPlay Application Secret Key";
                    break;
                case 8:
                    displayValue = "FairPlay PFX Password";
                    break;
            }
            break;
        case "keyDeliveryType":
            switch (value) {
                case 0:
                    displayValue = "None";
                    break;
                case 1:
                    displayValue = "PlayReady DRM";
                    break;
                case 2:
                    displayValue = "AES / Baseline HTTP";
                    break;
                case 3:
                    displayValue = "Widevine DRM";
                    break;
                case 4:
                    displayValue = "FairPlay DRM";
                    break;
            }
            break;
        case "endPointType":
            switch (value) {
                case 0:
                    displayValue = "None";
                    break;
                case 1:
                    displayValue = "Azure Queue";
                    break;
                case 2:
                    displayValue = "Azure Table";
                    break;
                case 3:
                    displayValue = "Web Hook";
                    break;
            }
            break;
    }
    var title = grid.colModel.label;
    var message = value;
    if (row.description != null && row.description != row.name) {
        message = message + "<br><br>" + row.description;
    }
    message = message + "<br><br>" + row.id;
    displayValue = displayValue != "" ? displayValue + " (" + value + ")" : value;
    return "<span onclick=\"DisplayMessage('" + title + "', '" + message + "')\">" + displayValue + "</span>";
}
function FormatDelete(value, grid, row) {
    var itemName = row.name == null ? row.id : row.name;
    var onClick = "DeleteEntity('" + grid.gid + "','" + row.id + "','" + encodeURIComponent(itemName) + "')";
    var buttonHtml = "<button class='siteButton' onclick=" + onClick + ">";
    return buttonHtml + "<img src='" + _storageCdnUrl + "/MediaDelete.png'></button>";
}
function DeleteEntity(entityGrid, entityId, entityName) {
    var title = "Confirm Permanent Delete";
    var message = "Are you sure you want to permanently delete the '" + entityName + "' " + _accountEntityType + "?";
    var onConfirm = function () {
        SetCursor(true);
        $.post("/account/deleteEntity",
            {
                "entityGrid": entityGrid,
                "entityId": entityId
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