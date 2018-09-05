function SetProcessingOptions() {
    switch (processingOption.id) {
        case "videoAnalyzer":
            $("#audioAnalyzer").prop("checked", false);
            $("#audioAnalyzer").prop("disabled", processingOption.checked);
            break;
        case "videoIndexer":
            $("#audioIndexer").prop("checked", false);
            $("#audioIndexer").prop("disabled", processingOption.checked);
            break;
        case "presetType2":
            $("#presetType3").prop("checked", false);
            $("#presetType3").prop("disabled", processingOption.checked);
            break;
        case "presetType4":
            $("#presetType5").prop("checked", false);
            $("#presetType5").prop("disabled", processingOption.checked);
            break;
    }
}
function GetTransformOutputs() {
    var transformOutputs = new Array();
    for (var i = 0; i < 4; i++) {
        if ($("#presetType" + i).prop("checked")) {
            var transformOutput = {
                PresetType: $("#presetType" + i).val(),
                PresetName: $("#presetName" + i).val(),
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
                    transformOutputs: transformOutputs
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