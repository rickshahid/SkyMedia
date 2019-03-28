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
    var title = "Delete " + entityType;
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
function DisplayInsight(fileName, assetName, insightId) {
    SetCursor(true);
    $.get("/insight/data",
        {
            assetName: decodeURIComponent(assetName),
            fileName: decodeURIComponent(fileName),
            insightId: insightId
        },
        function (insight) {
            SetCursor(false);
            var title = fileName == null ? "Video Indexer Insight (" + insightId + ")" : fileName;
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
function ReindexVideo(insightId, videoName) {
    var title = "Reindex Video";
    var message = "Are you sure you want to reindex the '" + FormatValue(videoName) + "' video?";
    var onConfirm = function () {
        SetCursor(true);
        $.post("/insight/reindex",
            {
                insightId: insightId
            },
            function () {
                SetCursor(false);
                DisplayMessage("Reindex Video Initiated", insightId);
            }
        );
        $(this).dialog("close");
    };
    ConfirmMessage(title, message, onConfirm);
}
function PublishContent(entityType, entityName, parentName, unpublish) {
    var action = unpublish ? "Unpublish" : "Publish";
    var actionUrl = entityType == "Asset" ? "/asset/publish" : "/job/publish";
    var title = action + " " + entityType;
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
                $("#presetType" + i).prop("checked", false).change();
                $("input[name=relativePriority" + i + "][value=Normal]").prop("checked", true);
                $("#onError" + i).prop("checked", false);
            }
            var outputs = row["properties.outputs"];
            for (var i = 0; i < outputs.length; i++) {
                var output = outputs[i];
                var preset = output.preset;
                var presetIndex = GetPresetIndex(preset);
                $("#presetType" + presetIndex).prop("checked", true).change();
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
            var outputPublish = JSON.parse(jobData["outputPublish"]);
            var streamingPolicyName = outputPublish["streamingPolicyName"];
            $("#streamingPolicies").val(streamingPolicyName);
            break;
        case "liveEvents":
            $("input[name=inputProtocol][value=" + row["properties.input"]["streamingProtocol"] + "]").prop("checked", true);
            $("input[name=encodingType][value=" + row["properties.encoding"]["encodingType"] + "]").prop("checked", true);
            $("#encodingPresetName").val(row["properties.encoding"]["presetName"]);
            $("#streamingPolicies").val(row["properties.preview"]["streamingPolicyName"]);
            if (row["properties.streamOptions"].indexOf("LowLatency") > -1) {
                $("#lowLatency").prop("checked", true);
            } else {
                $("#lowLatency").prop("checked", false);
            }
            _jsonEditor.set(row["tags"]);
           break;
    }
}
function UpdateEvent(eventName, eventAction) {
    var actionUrl;
    switch (eventAction) {
        case "Start":
            actionUrl = "/live/startEvent";
            break;
        case "Stop":
            actionUrl = "/live/stopEvent";
            break;
        case "Reset":
            actionUrl = "/live/resetEvent";
            break;
    }
    var title = eventAction + " Live Event";
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