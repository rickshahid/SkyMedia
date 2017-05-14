function GetTaskRowIndex(taskButton) {
    return $(taskButton).parents("tr")[1].rowIndex;
}
function GetLastTaskNumber(lastTaskRow) {
    return parseInt(lastTaskRow.id.replace("mediaWorkflowTaskRow", ""));
}
function SetJobTaskParents(taskNumber) {
    var taskParent = $("#taskParent" + taskNumber)[0];
    taskParent.options.length = 0;
    taskParent.options[taskParent.options.length] = new Option("", "");
    for (var i = 1; i < taskNumber; i++) {
        taskParent.options[taskParent.options.length] = new Option(i, i);
    }
    if (taskParent.options.length > 1) {
        taskParent.disabled = false;
    }
}
function SetJobTaskWidgets(taskNumber) {
    $.widget("ui.spinnerEx1", $.ui.spinner, {
        _format: function (value) {
            if (value < 10) {
                value = "0" + value;
            }
            return value;
        },
        _parse: function (value) {
            return parseInt(value);
        }
    });
    $.widget("ui.spinnerEx2", $.ui.spinner, {
        _format: function (value) {
            return value + "x";
        },
        _parse: function (value) {
            return parseInt(value);
        }
    });
    $("#summarizationDurationMinutes" + taskNumber).spinnerEx1({
        min: 0,
        max: 99
    });
    $("#summarizationDurationSeconds" + taskNumber).spinnerEx1({
        min: 0,
        max: 59
    });
    $("#hyperlapseStartFrame" + taskNumber).spinner({
        min: 0,
        max: 9999999
    });
    $("#hyperlapseFrameCount" + taskNumber).spinner({
        min: 0,
        max: 9999999
    });
    $("#hyperlapseSpeed" + taskNumber).spinnerEx2({
        min: 1,
        max: 9
    });
    $("#taskOptions" + taskNumber).multiselect({
        noneSelectedText: "0 of 3 Job Task Options",
        selectedText: "# of 3 Job Task Options",
        classes: "multiSelectOptions jobTaskOptions",
        header: false
    });
}
function ClearJobTaskWidgets(lastTaskNumber) {
    $("#summarizationDurationMinutes" + lastTaskNumber).spinnerEx1("destroy");
    $("#summarizationDurationSeconds" + lastTaskNumber).spinnerEx1("destroy");
    $("#hyperlapseStartFrame" + lastTaskNumber).spinner("destroy");
    $("#hyperlapseFrameCount" + lastTaskNumber).spinner("destroy");
    $("#hyperlapseSpeed" + lastTaskNumber).spinnerEx2("destroy");
    $("#taskOptions" + lastTaskNumber).multiselect("destroy");
}
function SetEncoderConfig(fileInput) {
    var fileReader = new FileReader();
    fileReader.onload = function (e) {
        _encoderConfig = e.target.result;
    };
    var file = fileInput.files[0];
    fileReader.readAsText(file);
}
function GetJobTaskOptions(taskNumber) {
    var taskOptions = 0;
    var selectedOptions = $("#taskOptions" + taskNumber).val();
    if (selectedOptions != null) {
        for (var i = 0; i < selectedOptions.length; i++) {
            taskOptions = taskOptions + parseInt(selectedOptions[i]);
        }
    }
    return taskOptions;
}
function GetJobTask(taskNumber) {
    var jobTask = null;
    var mediaProcessor = $("#mediaProcessor" + taskNumber).val();
    if (mediaProcessor != null && mediaProcessor != "None") {
        var taskParent = $("#taskParent" + taskNumber).val();
        jobTask = {
            ProcessorType: mediaProcessor,
            ParentIndex: (taskParent == "") ? null : taskParent - 1,
            OutputAssetName: $("#outputAssetName" + taskNumber).val(),
            Options: GetJobTaskOptions(taskNumber)
        };
        switch (jobTask.ProcessorType) {
            case "EncoderStandard":
            case "EncoderPremium":
            case "EncoderUltra":
                var encoderConfig = $("#encoderConfig" + taskNumber).val();
                if (encoderConfig == "Custom") {
                    jobTask.ProcessorConfig = _encoderConfig;
                } else {
                    jobTask.ProcessorConfig = encoderConfig;
                }
                jobTask.GenerateThumbnails = $("#encoderGenerateThumbnails" + taskNumber).prop("checked");
                if ($("#encoderFragmentOutput" + taskNumber).prop("checked")) {
                    jobTask.OutputAssetFormat = 1; // AssetFormatOption.AdaptiveStreaming
                }
                jobTask.ContentProtection = GetContentProtection(taskNumber);
                break;
            case "SpeechToText":
                jobTask.SpeechToTextLanguages = $("#speechToTextLanguages" + taskNumber).val();
                jobTask.SpeechToTextCaptionWebVtt = $("#speechToTextcaptionWebVtt" + taskNumber).prop("checked");
                jobTask.SpeechToTextCaptionTtml = $("#speechToTextcaptionTtml" + taskNumber).prop("checked");
                break;
            case "FaceDetection":
                jobTask.FaceDetectionMode = $("#faceDetectionMode" + taskNumber + ":checked").val();
                break;
            case "FaceRedaction":
                jobTask.FaceRedactionMode = $("#faceRedactionMode" + taskNumber + ":checked").val();
                break;
            case "VideoSummarization":
                var durationMinutes = $("#summarizationDurationMinutes" + taskNumber).val();
                var durationSeconds = $("#summarizationDurationSeconds" + taskNumber).val();
                jobTask.SummarizationDurationSeconds = (durationMinutes * 60) + durationSeconds;
                break;
            case "MotionDetection":
                jobTask.MotionDetectionSensitivityLevel = $("#motionDetectionSensitivityLevel" + taskNumber).val();
                jobTask.MotionDetectionLightChange = $("#motionDetectionLightChange" + taskNumber).prop("checked");
                break;
            case "MotionHyperlapse":
                jobTask.MotionHyperlapseStartFrame = $("#hyperlapseStartFrame" + taskNumber).val();
                jobTask.MotionHyperlapseFrameCount = $("#hyperlapseFrameCount" + taskNumber).val();
                jobTask.MotionHyperlapseSpeed = $("#hyperlapseSpeed" + taskNumber).val().substr(0, 1);
                break;
        }
    }
    return jobTask;
}
