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
            var title = fileName == null ? indexId : fileName;
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
    }
    ConfirmMessage(title, message, onConfirm);
}
function PublishJobOutput(jobName) {
    var title = "Confirm Job Output Publish";
    var message = "Are you sure you want to publish the '" + FormatValue(jobName) + "' job output?";
    var onConfirm = function () {
        SetCursor(true);
        $.post("/job/publish",
            {
                jobName: decodeURIComponent(jobName)
            },
            function (mediaPublish) {
                SetCursor(false);
                var message = mediaPublish.userContact.notificationMessage;
                DisplayMessage("Job Output Publish Message", message);
            }
        );
        $(this).dialog("close");
    }
    ConfirmMessage(title, message, onConfirm);
}
function CancelJob(jobName, transformName) {
    var title = "Confirm Job Cancel Request";
    var message = "Are you sure you want to cancel the '" + FormatValue(jobName) + "' job?";
    var onConfirm = function () {
        SetCursor(true);
        $.post("/job/cancel",
            {
                jobName: decodeURIComponent(jobName),
                transformName: decodeURIComponent(transformName)
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
function DeleteEntity(gridId, entityName, parentEntityName) {
    var entityType = GetEntityType(gridId);
    var title = "Confirm Delete " + entityType;
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
    }
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
function GetPresetIndex(preset) {
    var presetIndex;
    if (preset.presetName != null) {
        presetIndex = 0;
        $("#presetName0").val(preset.presetName);
    } else if (preset.codecs != null) {
        presetIndex = 1;
    } else if (preset.audioInsightsOnly != null) {
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
    }
}
function FormatActions(value, grid, row) {
    var onClick = "";
    var actionsHtml = "";
    var entityName = grid.gid == "indexerInsights" ? row.id : row.name;
    var entityType = GetEntityType();
    if (grid.gid == "streamingPolicies" && entityName.indexOf("Predefined_") > -1) {
        actionsHtml = "N/A";
    } else if (grid.gid == "assetFiles") {
        if (row.name.indexOf(".json") > -1) {
            onClick = "DisplayInsight('" + encodeURIComponent(entityName) + "','" + encodeURIComponent(row.parentEntityName) + "')";
            actionsHtml = "<button id='" + row.id + "_json' class='siteButton' onclick=" + onClick + ">";
            actionsHtml = actionsHtml + "<img src='" + _storageCdnUrl + "/MediaInsight.png'></button>";
        } else if (row.name.indexOf(".mp4") > -1) {
            onClick = "PlayVideo('" + row.downloadUrl + "')";
            actionsHtml = "<button id='" + row.id + "_video' class='siteButton' onclick=" + onClick + ">";
            actionsHtml = actionsHtml + "<img src='" + _storageCdnUrl + "/MediaPlayerFile.png'></button>";
        } else if (row.name.indexOf(".ism") > -1 || row.name.indexOf(".ismc") > -1) {
            onClick = "OpenFile('" + row.downloadUrl + "')";
            actionsHtml = "<button id='" + row.id + "_manifest' class='siteButton' onclick=" + onClick + ">";
            actionsHtml = actionsHtml + "<img src='" + _storageCdnUrl + "/MediaManifest.png'></button>";
        } else if (row.name.indexOf(".vtt") > -1 || row.name.indexOf(".ttml") > -1) {
            onClick = "OpenFile('" + row.downloadUrl + "')";
            actionsHtml = "<button id='" + row.id + "_transcript' class='siteButton' onclick=" + onClick + ">";
            actionsHtml = actionsHtml + "<img src='" + _storageCdnUrl + "/MediaTranscript.png'></button>";
        }
        onClick = "OpenFile('" + row.downloadUrl + "')";
        var downloadHtml = "<button id='" + row.id + "_download' class='siteButton' onclick=" + onClick + ">";
        downloadHtml = downloadHtml + "<img src='" + _storageCdnUrl + "/MediaDownload.png'></button>";
        actionsHtml = actionsHtml + downloadHtml;
    } else {
        var insightHtml = "";
        var reindexHtml = "";
        var cancelHtml = "";
        var publishHtml = "";
        switch (grid.gid) {
            case "indexerInsights":
                onClick = "DisplayInsight(null,null,'" + entityName + "')";
                insightHtml = "<button id='" + row.id + "_insight' class='siteButton' onclick=" + onClick + ">";
                insightHtml = insightHtml + "<img src='" + _storageCdnUrl + "/MediaInsight.png'></button>";
                onClick = "ReindexVideo('" + row.id + "','" + encodeURIComponent(entityName) + "')";
                reindexHtml = "<button id='" + row.id + "_reindex' class='siteButton' onclick=" + onClick + ">";
                reindexHtml = reindexHtml + "<img src='" + _storageCdnUrl + "/MediaInsightReindex.png'></button>";
                break;
            case "transformJobs":
                switch (row["properties.state"]) {
                    case "Queued":
                    case "Scheduled":
                    case "Processing":
                        onClick = "CancelJob('" + encodeURIComponent(entityName) + "','" + encodeURIComponent(row.parentEntityName) + "')";
                        cancelHtml = "<button id='" + row.id + "_cancel' class='siteButton' onclick=" + onClick + ">";
                        cancelHtml = cancelHtml + "<img src='" + _storageCdnUrl + "/MediaJobCancel.png'></button>";
                        break;
                    //case "Error":
                    case "Finished":
                        onClick = "PublishJobOutput('" + encodeURIComponent(entityName) + "')";
                        publishHtml = "<button id='" + row.id + "_publish' class='siteButton' onclick=" + onClick + ">";
                        publishHtml = publishHtml + "<img src='" + _storageCdnUrl + "/MediaJobPublish.png'></button>";
                        break;
                }
                break;
        }
        var editHtml = "";
        if (window.location.href.indexOf("/account") == -1) {
            if (grid.gid == "transforms") {
                editHtml = "<button id='" + row.id + "_edit' class='siteButton' onclick=SetRowEdit('" + grid.gid + "','" + encodeURIComponent(JSON.stringify(row)) + "')>";
                editHtml = editHtml + "<img src='" + _storageCdnUrl + "/MediaEntityEdit.png'></button>";
            }
        }
        onClick = "DeleteEntity('" + grid.gid + "','" + encodeURIComponent(entityName) + "','" + encodeURIComponent(row.parentEntityName) + "')";
        var deleteHtml = "<button id='" + row.id + "_delete' class='siteButton' onclick=" + onClick + ">";
        deleteHtml = deleteHtml + "<img src='" + _storageCdnUrl + "/MediaEntityDelete.png'></button>";
        actionsHtml = insightHtml + reindexHtml + cancelHtml + publishHtml + editHtml + deleteHtml;
    }
    return actionsHtml;
}