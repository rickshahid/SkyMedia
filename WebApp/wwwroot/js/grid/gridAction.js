function DownloadFile(fileUrl) {
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
            $("#insightData").jsonBrowse(insight);
            var title = fileName == null ? indexId : fileName;
            DisplayDialog("insightDialog", title, null, {}, 800, 1200);
        }
    );
}
function DisplayManifest(fileUrl) {
    alert(fileUrl);
}
function DisplayTranscript(fileUrl) {
    alert(fileUrl);
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
function PublishJob(jobName) {
    var title = "Confirm Job Publish";
    var message = "Are you sure you want to publish the '" + FormatValue(jobName) + "' job?";
    var onConfirm = function () {
        SetCursor(true);
        $.post("/job/publish",
            {
                jobName: decodeURIComponent(jobName)
            },
            function (message) {
                SetCursor(false);
                DisplayMessage("Job Publish Message", message);
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
            actionsHtml = "<button id='" + row.id + "_insight' class='siteButton' onclick=" + onClick + ">";
            actionsHtml = actionsHtml + "<img src='" + _storageCdnUrl + "/MediaInsight.png'></button>";
        } else if (row.name.indexOf(".mp4") > -1) {
            onClick = "PlayVideo('" + row.downloadUrl + "')";
            actionsHtml = "<button id='" + row.id + "_video' class='siteButton' onclick=" + onClick + ">";
            actionsHtml = actionsHtml + "<img src='" + _storageCdnUrl + "/MediaPlayerFile.png'></button>";
        } else if (row.name.indexOf(".ism") > -1 || row.name.indexOf(".ismc") > -1) {
            onClick = "DisplayManifest('" + row.downloadUrl + "')";
            actionsHtml = "<button id='" + row.id + "_manifest' class='siteButton' onclick=" + onClick + ">";
            actionsHtml = actionsHtml + "<img src='" + _storageCdnUrl + "/MediaManifest.png'></button>";
        } else if (row.name.indexOf(".vtt") > -1 || row.name.indexOf(".ttml") > -1) {
            onClick = "DisplayTranscript('" + row.downloadUrl + "')";
            actionsHtml = "<button id='" + row.id + "_transcript' class='siteButton' onclick=" + onClick + ">";
            actionsHtml = actionsHtml + "<img src='" + _storageCdnUrl + "/MediaTranscript.png'></button>";
        }
        onClick = "DownloadFile('" + row.downloadUrl + "')";
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
                    case "Error":
                    case "Finished":
                        onClick = "PublishJob('" + encodeURIComponent(entityName) + "')";
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