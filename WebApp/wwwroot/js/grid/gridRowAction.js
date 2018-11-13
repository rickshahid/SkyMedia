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
    } else if (preset.hasOwnProperty("codecs")) {
        presetIndex = 1;
    } else if (preset.hasOwnProperty("insightsToExtract")) {
        presetIndex = 2;
    } else {
        presetIndex = 3;
    }
    return presetIndex;
}
function OpenFile(fileUrl) {
    window.open(fileUrl, "_blank");
}
function DeleteEntity(gridId, entityName, parentName) {
    var entityType = GetEntityType(gridId);
    var title = "Confirm " + entityType + " Delete";
    var message = "Are you sure you want to delete the '" + FormatValue(entityName) + "' " + entityType.toLowerCase() + "?";
    var onConfirm = function () {
        SetCursor(true);
        $.post("/account/deleteEntity",
            {
                gridId: gridId,
                entityName: decodeURIComponent(entityName),
                parentName: decodeURIComponent(parentName)
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
function PublishContent(entityType, entityName, parentName, unpublish) {
    var action = unpublish ? "Unpublish" : "Publish";
    var actionUrl = entityType == "Asset" ? "/asset/publish" : "/job/publish";
    var title = "Confirm " + action + " " + entityType;
    var message = "Are you sure you want to " + action.toLowerCase() + " the '" + FormatValue(entityName) + "' " + entityType.toLowerCase() + "?";
    var onConfirm = function () {
        SetCursor(true);
        $.post(actionUrl,
            {
                entityName: decodeURIComponent(entityName),
                parentName: decodeURIComponent(parentName),
                unpublish: unpublish
            },
            function (message) {
                SetCursor(false);
                var title = "Content " + action + " Message";
                if (entityType != "Asset") {
                    message = message.userNotification.jobOutputMessage;
                }
                DisplayMessage(title, message);
                window.location = window.location.href;
            }
        );
        $(this).dialog("close");
    };
    ConfirmMessage(title, message, onConfirm);
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
            $("#transforms").val(row.parentName);
            var jobData = row["properties.correlationData"];
            var outputPublish = JSON.parse(jobData["OutputPublish"]);
            var streamingPolicyName = outputPublish["StreamingPolicyName"];
            $("#streamingPolicies").val(streamingPolicyName);
            break;
    }
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