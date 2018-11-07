function OpenFile(fileUrl) {
    window.open(fileUrl, "_blank"); 
}
function DisplayInsight(fileName, assetName, indexId) {
    SetCursor(true);
    $.get("/asset/insight",
        {
            assetName: decodeURIComponent(assetName),
            fileName: decodeURIComponent(fileName),
            indexId: indexId
        },
        function (insight) {
            SetCursor(false);
            var title = fileName == null ? "Video Indexer Insight (Index Id " + indexId + ")" : fileName;
            DisplayJson(title, insight);
        }
    );
}
function DisplayJson(title, jsonData) {
    var dialogId = "metadataDialog";
    var containerId = "contentMetadata";
    var onClose = function () {
        _jsonEditor.destroy();
        $("#" + containerId).hide();
    };
    $("#" + containerId).show();
    CreateJsonEditor(containerId, null, jsonData);
    DisplayDialog(dialogId, title, null, null, null, null, onClose);
}
function PlayVideo(fileUrl) {
    alert(fileUrl);
}
function ReindexVideo(indexId, videoName) {
    var title = "Confirm Reindex Video";
    var message = "Are you sure you want to reindex the '" + FormatValue(videoName) + "' video?";
    var onConfirm = function () {
        SetCursor(true);
        $.post("/asset/reindex",
            {
                indexId: indexId
            },
            function () {
                SetCursor(false);
                DisplayMessage("Reindex Video Initiated", indexId);
            }
        );
        $(this).dialog("close");
    };
    ConfirmMessage(title, message, onConfirm);
}
function PublishAsset(assetName) {
    var title = "Confirm Publish Asset";
    var message = "Are you sure you want to publish the '" + FormatValue(assetName) + "' asset?";
    var onConfirm = function () {
        SetCursor(true);
        $.post("/asset/publish",
            {
                assetName: decodeURIComponent(assetName)
            },
            function (playerUrl) {
                SetCursor(false);
                DisplayMessage("Asset Publish Message", playerUrl);
            }
        );
        $(this).dialog("close");
    };
    ConfirmMessage(title, message, onConfirm);
}
function GetPresetIndex(preset) {
    var presetIndex;
    if (preset.presetName != null) {
        presetIndex = 0;
        $("#presetName0").val(preset.presetName);
    } else if (preset.hasOwnProperty("codecs")) {
        presetIndex = 1;
    } else if (preset.hasOwnProperty("insightsToExtract")) {
        presetIndex = 2;
    } else {
        presetIndex = 3;
    }
    return presetIndex;
}
function SetRowEdit(gridId, rowData) {
    var row = JSON.parse(decodeURIComponent(rowData));
    $("#name").val(row.name);
    $("#description").val(row["properties.description"]);
    switch (gridId) {
        case "transforms":
            for (var i = 0; i < 4; i++) {
                $("#presetType" + i).prop("checked", false);
                $("input[name=relativePriority" + i + "][value=Normal]").prop("checked", true);
                $("#onError" + i).prop("checked", false);
            }
            var outputs = row["properties.outputs"];
            for (var i = 0; i < outputs.length; i++) {
                var output = outputs[i];
                var preset = output.preset;
                var presetIndex = GetPresetIndex(preset);
                $("#presetType" + presetIndex).prop("checked", true);
                $("input[name=relativePriority" + presetIndex + "][value=" + output.relativePriority + "]").prop("checked", true);
                if (output.onError == "ContinueJob") {
                    $("#onError" + presetIndex).prop("checked", true);
                }
            }
            break;
        case "transformJobs":
            $("input[name=jobPriority][value=" + row["properties.priority"] + "]").prop("checked", true);
            $("#transforms").val(row.parentEntityName);
            var jobData = row["properties.correlationData"];
            var outputPublish = JSON.parse(jobData["OutputPublish"]);
            var streamingPolicyName = outputPublish["StreamingPolicyName"];
            $("#streamingPolicies").val(streamingPolicyName);
            break;
    }
}
//function FormatActions(value, grid, row) {
//    if (grid.gid == "streamingPolicies" && entityName.indexOf("Predefined_") > -1) {
//        actionsHtml = "N/A";
//    } else if (grid.gid == "assetFiles") {
//        if (row.name.indexOf(".json") > -1) {
//            onClick = "DisplayInsight('" + encodeURIComponent(entityName) + "','" + encodeURIComponent(row.parentEntityName) + "')";
//            actionsHtml = "<button id='" + row.id + "_json' class='siteButton' onclick=" + onClick + ">";
//            actionsHtml = actionsHtml + "<img src='" + _storageCdnUrl + "/MediaInsight.png'></button>";
//        } else if (row.name.indexOf(".mp4") > -1) {
//            onClick = "PlayVideo('" + row.downloadUrl + "')";
//            actionsHtml = "<button id='" + row.id + "_video' class='siteButton' onclick=" + onClick + ">";
//            actionsHtml = actionsHtml + "<img src='" + _storageCdnUrl + "/MediaPlay.png'></button>";
//        } else if (row.name.indexOf(".ism") > -1 || row.name.indexOf(".ismc") > -1) {
//            onClick = "OpenFile('" + row.downloadUrl + "')";
//            actionsHtml = "<button id='" + row.id + "_manifest' class='siteButton' onclick=" + onClick + ">";
//            actionsHtml = actionsHtml + "<img src='" + _storageCdnUrl + "/MediaManifest.png'></button>";
//        } else if (row.name.indexOf(".vtt") > -1 || row.name.indexOf(".ttml") > -1) {
//            onClick = "OpenFile('" + row.downloadUrl + "')";
//            actionsHtml = "<button id='" + row.id + "_transcript' class='siteButton' onclick=" + onClick + ">";
//            actionsHtml = actionsHtml + "<img src='" + _storageCdnUrl + "/MediaTranscript.png'></button>";
//        }
//        onClick = "OpenFile('" + row.downloadUrl + "')";
//        var downloadHtml = "<button id='" + row.id + "_download' class='siteButton' onclick=" + onClick + ">";
//        downloadHtml = downloadHtml + "<img src='" + _storageCdnUrl + "/MediaDownload.png'></button>";
//        actionsHtml = actionsHtml + downloadHtml;
//    } else {
//        var insightHtml = "";
//        var reindexHtml = "";
//        var cancelHtml = "";
//        var publishHtml = "";
//        switch (grid.gid) {
//            case "assets":
//                onClick = "PublishAsset('" + encodeURIComponent(entityName) + "')";
//                publishHtml = "<button id='" + row.id + "_publish' class='siteButton' onclick=" + onClick + ">";
//                publishHtml = publishHtml + "<img src='" + _storageCdnUrl + "/MediaJobPublish.png'></button>";
//                break;
//            case "transformJobs":
//                switch (row["properties.state"]) {
//                    case "Queued":
//                    case "Scheduled":
//                    case "Processing":
//                        onClick = "CancelJob('" + encodeURIComponent(entityName) + "','" + encodeURIComponent(row.parentEntityName) + "')";
//                        cancelHtml = "<button id='" + row.id + "_cancel' class='siteButton' onclick=" + onClick + ">";
//                        cancelHtml = cancelHtml + "<img src='" + _storageCdnUrl + "/MediaJobCancel.png'></button>";
//                        break;
//                    //case "Error":
//                    case "Finished":
//                        onClick = "PublishJobOutput('" + encodeURIComponent(entityName) + "')";
//                        publishHtml = "<button id='" + row.id + "_publish' class='siteButton' onclick=" + onClick + ">";
//                        publishHtml = publishHtml + "<img src='" + _storageCdnUrl + "/MediaJobPublish.png'></button>";
//                        break;
//                }
//                break;
//            case "indexerInsights":
//                onClick = "DisplayInsight(null,null,'" + entityName + "')";
//                insightHtml = "<button id='" + row.id + "_insight' class='siteButton' onclick=" + onClick + ">";
//                insightHtml = insightHtml + "<img src='" + _storageCdnUrl + "/MediaInsight.png'></button>";
//                onClick = "ReindexVideo('" + row.id + "','" + encodeURIComponent(entityName) + "')";
//                reindexHtml = "<button id='" + row.id + "_reindex' class='siteButton' onclick=" + onClick + ">";
//                reindexHtml = reindexHtml + "<img src='" + _storageCdnUrl + "/MediaInsightReindex.png'></button>";
//                break;
//        }
//        var editHtml = "";
//        if (grid.gid == "transforms" || grid.gid == "transformJobs") {
//            editHtml = "<button id='" + row.id + "_edit' class='siteButton' onclick=SetRowEdit('" + grid.gid + "','" + encodeURIComponent(JSON.stringify(row)) + "')>";
//            editHtml = editHtml + "<img src='" + _storageCdnUrl + "/MediaEntityEdit.png'></button>";
//        }
//        onClick = "DeleteEntity('" + grid.gid + "','" + encodeURIComponent(entityName) + "','" + encodeURIComponent(row.parentEntityName) + "')";
//        var deleteHtml = "<button id='" + row.id + "_delete' class='siteButton' onclick=" + onClick + ">";
//        deleteHtml = deleteHtml + "<img src='" + _storageCdnUrl + "/MediaEntityDelete.png'></button>";
//        actionsHtml = insightHtml + reindexHtml + cancelHtml + publishHtml + editHtml + deleteHtml;
//    }
//    return actionsHtml;
//}
function FormatActions(value, grid, row) {
    var onClick, actionsHtml, canDelete = false;
    var entityName = grid.gid == "indexerInsights" ? row.id : row.name;
    switch (grid.gid) {
        case "liveEvents":
            onClick = "UpdateEvent('" + encodeURIComponent(entityName) + "','Start')";
            var startHtml = "<button id='" + row.id + "_start' class='siteButton' onclick=" + onClick + ">";
            startHtml = startHtml + "<img src='" + _storageCdnUrl + "/MediaEventStart.png'></button>";
            onClick = "UpdateEvent('" + encodeURIComponent(entityName) + "','Stop')";
            var stopHtml = "<button id='" + row.id + "_stop' class='siteButton' onclick=" + onClick + ">";
            stopHtml = stopHtml + "<img src='" + _storageCdnUrl + "/MediaEventStop.png'></button>";
            onClick = "UpdateEvent('" + encodeURIComponent(entityName) + "','Reset')";
            var resetHtml = "<button id='" + row.id + "_reset' class='siteButton' onclick=" + onClick + ">";
            resetHtml = resetHtml + "<img src='" + _storageCdnUrl + "/MediaEventReset.png'></button>";
            actionsHtml = startHtml + stopHtml + resetHtml;
            canDelete = true;
            break;
    }
    if (canDelete) {
        onClick = "DeleteEntity('" + grid.gid + "','" + encodeURIComponent(entityName) + "','" + encodeURIComponent(row.parentEntityName) + "')";
        var deleteHtml = "<button id='" + row.id + "_delete' class='siteButton' onclick=" + onClick + ">";
        deleteHtml = deleteHtml + "<img src='" + _storageCdnUrl + "/MediaEntityDelete.png'></button>";
        actionsHtml = actionsHtml + deleteHtml;
    }
    return actionsHtml;
}
function UpdateEvent(eventName, eventAction) {
    var actionUrl;
    switch (eventAction) {
        case "Start":
            actionUrl = "/live/start";
            break;
        case "Stop":
            actionUrl = "/live/stop";
            break;
        case "Reset":
            actionUrl = "/live/reset";
            break;
    }
    var title = "Confirm Live Event " + eventAction;
    var message = "Are you sure you want to " + eventAction.toLowerCase() + " the '" + eventName + "' event?";
    var onConfirm = function () {
        SetCursor(true);
        $.post(actionUrl,
            {
                eventName: decodeURIComponent(eventName)
            },
            function () {
                SetCursor(false);
                window.location = window.location.href;
            }
        );
        $(this).dialog("close");
    };
    ConfirmMessage(title, message, onConfirm);
}
function DeleteEntity(gridId, entityName, parentEntityName) {
    var entityType = GetEntityType(gridId);
    var title = "Confirm " + entityType + " Delete";
    var message = "Are you sure you want to delete the '" + FormatValue(entityName) + "' " + entityType.toLowerCase() + "?";
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
    };
    ConfirmMessage(title, message, onConfirm);
}
function GetEntityType(gridId) {
    var entityType;
    switch (gridId) {
        case "assets":
            entityType = "Asset";
            break;
        case "transforms":
            entityType = "Transform";
            break;
        case "transformJobs":
            entityType = "Job";
            break;
        case "contentKeyPolicies":
            entityType = "Content Key Policy";
            break;
        case "streamingPolicies":
            entityType = "Streaming Policy";
            break;
        case "streamingEndpoints":
            entityType = "Streaming Endpoint";
            break;
        case "streamingLocators":
            entityType = "Streaming Locator";
            break;
        case "liveEvents":
            entityType = "Live Event";
            break;
        case "liveEventOutputs":
            entityType = "Live Event Output";
            break;
        case "indexerInsights":
            entityType = "Video Indexer Insight";
            break;
    }
    return entityType;
}