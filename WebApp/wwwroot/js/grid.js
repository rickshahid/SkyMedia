function ClearTitles(grid) {
    for (var i = 0; i < grid.rows.length; i++) {
        var rowId = grid.rows[i].id;
        var row = document.getElementById(rowId);
        for (var x = 0; x < row.cells.length; x++) {
            row.cells[x].title = "";
        }
    }
}
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
function FormatColumn(value, grid, row) {
    var title = row.name;
    var message = row.id;
    if (row.description != null && row.description != row.name) {
        message = message + "<br><br>" + row.description;
    }
    var displayValue = "";
    switch (grid.colModel.name) {
        case "name":
            if (value == null) {
                value = row.id;
                title = row.id;
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
    displayValue = displayValue != "" ? displayValue + " (" + value + ")" : value;
    return "<span onclick=\"DisplayMessage('" + title + "', '" + message + "', null, 600)\">" + displayValue + "</span>";
}