function FormatActions(value, grid, row) {
    var onClick, actionsHtml = "", canDelete = true;
    var entityId = grid.gid == "indexerInsights" ? row.id : row.name;
    switch (grid.gid) {
        case "assets":
            onClick = "PublishContent('Asset','" + encodeURIComponent(entityId) + "','" + encodeURIComponent(row.parentName) + "',false)";
            actionsHtml = "<button id='" + row.id + "_publish' class='siteButton' onclick=" + onClick + ">";
            actionsHtml = actionsHtml + "<img src='" + _storageCdnUrl + "/MediaPublishCreate.png'></button>";
            onClick = "PublishContent('Asset','" + encodeURIComponent(entityId) + "','" + encodeURIComponent(row.parentName) + "',true)";
            actionsHtml = actionsHtml + "<button id='" + row.id + "_unpublish' class='siteButton' onclick=" + onClick + ">";
            actionsHtml = actionsHtml + "<img src='" + _storageCdnUrl + "/MediaPublishDelete.png'></button>";
            break;
        case "transforms":
            onClick = "SetRowEdit('" + grid.gid + "','" + encodeURIComponent(JSON.stringify(row)) + "')";
            actionsHtml = "<button id='" + row.id + "_edit' class='siteButton' onclick=" + onClick + ">";
            actionsHtml = actionsHtml + "<img src='" + _storageCdnUrl + "/MediaEntityEdit.png'></button>";
            break;
        case "transformJobs":
            switch (row["properties.state"]) {
                case "Queued":
                case "Scheduled":
                case "Processing":
                    if (row["properties.state"] == "Queued" || row["properties.state"] == "Scheduled") {
                        onClick = "SetRowEdit('" + grid.gid + "','" + encodeURIComponent(JSON.stringify(row)) + "')";
                        actionsHtml = "<button id='" + row.id + "_edit' class='siteButton' onclick=" + onClick + ">";
                        actionsHtml = actionsHtml + "<img src='" + _storageCdnUrl + "/MediaEntityEdit.png'></button>";
                    }
                    onClick = "CancelJob('" + encodeURIComponent(entityId) + "','" + encodeURIComponent(row.parentName) + "')";
                    actionsHtml = actionsHtml + "<button id='" + row.id + "_cancel' class='siteButton' onclick=" + onClick + ">";
                    actionsHtml = actionsHtml + "<img src='" + _storageCdnUrl + "/MediaJobCancel.png'></button>";
                    break;
                case "Finished":
                    onClick = "PublishContent('Job%20Output','" + encodeURIComponent(entityId) + "','" + encodeURIComponent(row.parentName) + "',false)";
                    actionsHtml = actionsHtml + "<button id='" + row.id + "_publish' class='siteButton' onclick=" + onClick + ">";
                    actionsHtml = actionsHtml + "<img src='" + _storageCdnUrl + "/MediaPublishCreate.png'></button>";
                    onClick = "PublishContent('Job%20Output','" + encodeURIComponent(entityId) + "','" + encodeURIComponent(row.parentName) + "',true)";
                    actionsHtml = actionsHtml + "<button id='" + row.id + "_unpublish' class='siteButton' onclick=" + onClick + ">";
                    actionsHtml = actionsHtml + "<img src='" + _storageCdnUrl + "/MediaPublishDelete.png'></button>";
                    break;
            }
            break;
        case "liveEvents":
            onClick = "UpdateEvent('" + encodeURIComponent(entityId) + "','Start')";
            actionsHtml = "<button id='" + row.id + "_start' class='siteButton' onclick=" + onClick + ">";
            actionsHtml = actionsHtml + "<img src='" + _storageCdnUrl + "/MediaEventStart.png'></button>";
            onClick = "UpdateEvent('" + encodeURIComponent(entityId) + "','Stop')";
            actionsHtml = actionsHtml + "<button id='" + row.id + "_stop' class='siteButton' onclick=" + onClick + ">";
            actionsHtml = actionsHtml + "<img src='" + _storageCdnUrl + "/MediaEventStop.png'></button>";
            onClick = "UpdateEvent('" + encodeURIComponent(entityId) + "','Reset')";
            actionsHtml = actionsHtml + "<button id='" + row.id + "_reset' class='siteButton' onclick=" + onClick + ">";
            actionsHtml = actionsHtml + "<img src='" + _storageCdnUrl + "/MediaEventReset.png'></button>";
            break;
        case "indexerInsights":
            onClick = "DisplayInsight(null,null,'" + row.id + "')";
            actionsHtml = "<button id='" + entityId + "_insight' class='siteButton' onclick=" + onClick + ">";
            actionsHtml = actionsHtml + "<img src='" + _storageCdnUrl + "/MediaInsightShow.png'></button>";
            onClick = "ReindexVideo('" + entityId + "','" + encodeURIComponent(row.name) + "')";
            actionsHtml = actionsHtml + "<button id='" + row.id + "_reindex' class='siteButton' onclick=" + onClick + ">";
            actionsHtml = actionsHtml + "<img src='" + _storageCdnUrl + "/MediaInsightReindex.png'></button>";
            break;
        default:
            switch (_childGridType) {
                case "assetFiles":
                    if (row.name.indexOf(".json") > -1) {
                        onClick = "DisplayInsight('" + encodeURIComponent(entityId) + "','" + encodeURIComponent(row.parentName) + "')";
                        actionsHtml = "<button id='" + row.id + "_insight' class='siteButton' onclick=" + onClick + ">";
                        actionsHtml = actionsHtml + "<img src='" + _storageCdnUrl + "/MediaInsightShow.png'></button>";
                    }
                    onClick = "OpenFile('" + row.downloadUrl + "')";
                    var downloadHtml = "<button id='" + row.id + "_download' class='siteButton' onclick=" + onClick + ">";
                    downloadHtml = downloadHtml + "<img src='" + _storageCdnUrl + "/MediaDownload.png'></button>";
                    actionsHtml = actionsHtml + downloadHtml;
                    break;
            }
            canDelete = false;
            break;
    }
    if (canDelete) {
        onClick = "DeleteEntity('" + grid.gid + "','" + encodeURIComponent(entityId) + "','" + encodeURIComponent(row.parentName) + "')";
        var deleteHtml = "<button id='" + row.id + "_delete' class='siteButton' onclick=" + onClick + ">";
        deleteHtml = deleteHtml + "<img src='" + _storageCdnUrl + "/MediaEntityDelete.png'></button>";
        actionsHtml = actionsHtml + deleteHtml;
    }
    return actionsHtml;
}