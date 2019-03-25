function SetIntelligenceOptions(videoPreset) {
    var audioPresetId;
    switch (videoPreset.id) {
        case "videoAnalyzer":
            audioPresetId = "audioAnalyzer";
            break;
        case "videoIndexer":
            audioPresetId = "audioIndexer";
            break;
        case "presetType2":
            audioPresetId = "presetType3";
            break;
    }
    $("#" + audioPresetId).prop("checked", false);
    $("#" + audioPresetId).prop("disabled", videoPreset.checked);
}
function SetThumbnailSprite(thumbnailPreset) {
    if (thumbnailPreset.checked) {
        $("#thumbnailSpriteRow").show();
    } else {
        $("#thumbnailSprite").prop("checked", false);
        $("#thumbnailSpriteRow").hide();
    }
}
function GetTransformOutputs() {
    var transformOutputs = new Array();
    for (var i = 0; i < 4; i++) {
        if ($("#presetType" + i).prop("checked")) {
            var transformOutput = {
                PresetType: $("#presetType" + i).val(),
                PresetName: $("#presetName" + i).val(),
                RelativePriority: $("#relativePriority" + i + ":checked").val(),
                OnError: $("#onError" + i).prop("checked") ? $("#onError" + i).val() : null
            };
            transformOutputs.push(transformOutput);
        }
    }
    return transformOutputs;
}
function GetThumbnailSpriteColumns() {
    var thumbnailSpriteColumns;
    if ($("#presetType1").prop("checked") && $("#thumbnailSprite").prop("checked")) {
        thumbnailSpriteColumns = $("#thumbnailSpriteColumns").val();
    }
    return thumbnailSpriteColumns;
}
function SaveTransform() {
    var title = "Save Transform";
    var message = "Are you sure you want to save the current transform?";
    var transformOutputs = GetTransformOutputs();
    if (transformOutputs.length == 0) {
        message = "At least 1 transform output preset must be selected before a transform can be saved.";
        DisplayMessage(title, message);
    } else {
        var thumbnailSpriteColumns = GetThumbnailSpriteColumns();
        var onConfirm = function () {
            SetCursor(true);
            $.post("/transform/create",
                {
                    transformName: $("#name").val(),
                    transformDescription: $("#description").val(),
                    transformOutputs: transformOutputs,
                    thumbnailSpriteColumns: thumbnailSpriteColumns
                },
                function (transform) {
                    SetCursor(false);
                    var transformName = FormatValue(transform.name);
                    DisplayMessage("Media Transform Saved", transformName);
                }
            );
            $(this).dialog("close");
        };
        ConfirmMessage(title, message, onConfirm);
    }
}