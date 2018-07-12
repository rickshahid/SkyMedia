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
        case "videoIndexerInsights":
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
function CancelJob(jobName, transformName) {
    var title = "Confirm Cancel Job Request";
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