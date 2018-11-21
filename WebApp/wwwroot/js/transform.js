function SetIntelligenceOptions(intelligencePreset) {
    switch (intelligencePreset.id) {
        case "videoAnalyzer":
            $("#audioAnalyzer").prop("checked", false);
            $("#audioAnalyzer").prop("disabled", intelligencePreset.checked);
            break;
        case "videoIndexer":
            $("#audioIndexer").prop("checked", false);
            $("#audioIndexer").prop("disabled", intelligencePreset.checked);
            break;
        case "presetType2":
            $("#presetType3").prop("checked", false);
            $("#presetType3").prop("disabled", intelligencePreset.checked);
            break;
    }
}
function SetThumbnailOptions(thumbnailPreset) {
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
                TransformPreset: $("#presetName" + i).val(),
                RelativePriority: $("#relativePriority" + i).val(),
                OnError: $("#onError" + i).val()
            };
            transformOutputs.push(transformOutput);
        }
    }
    return transformOutputs;
}
function CreateTransform() {
    var title = "Save Transform";
    var message = "Are you sure you want to save the current transform?";
    var transformOutputs = GetTransformOutputs();
    if (transformOutputs.length == 0) {
        message = "At least 1 transform output preset must be selected before a transform can be saved.";
        DisplayMessage(title, message);
    } else {
        var onConfirm = function () {
            SetCursor(true);
            $.post("/transform/create",
                {
                    transformName: $("#name").val(),
                    transformDescription: $("#description").val(),
                    transformOutputs: transformOutputs,
                    thumbnailSpriteColumns: $("#thumbnailSpriteColumns").val()
                },
                function (transform) {
                    SetCursor(false);
                    window.location = window.location.href;
                }
            );
            $(this).dialog("close");
        };
        ConfirmMessage(title, message, onConfirm);
    }
}