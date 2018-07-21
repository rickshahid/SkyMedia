function SetAudioAnalyzer(videoAnalyzer) {
    $("#audioAnalyzerPreset").prop("checked", false);
    $("#audioAnalyzerPreset").prop("disabled", videoAnalyzer.checked);
}
function GetTransformOutputs() {
    var transformOutputs = new Array();
    for (var i = 0; i < 3; i++) {
        var transformOutput = {
            PresetEnabled: $("#presetEnabled" + i).prop("checked"),
            PresetName: $("#presetName" + i).val(),
            RelativePriority: $("#relativePriority" + i).val(),
            OnError: $("#onError" + i).val()
        }
        transformOutputs.push(transformOutput);
    }
    return transformOutputs
}
function CreateTransform() {
    var title = "Confirm Transform Create / Update";
    var message = "Are you sure you want to create / update the current transform?";
    var onConfirm = function () {
        SetCursor(true);
        $.post("/transform/create",
            {
                transformName: $("#name").val(),
                transformDescription: $("#description").val(),
                transformOutputs: GetTransformOutputs()
            },
            function (entity) {
                SetCursor(false);
                window.location = window.location.href;
            }
        );
        $(this).dialog("close");
    }
    ConfirmMessage(title, message, onConfirm);
}