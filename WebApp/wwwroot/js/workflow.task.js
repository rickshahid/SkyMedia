function GetTaskNumber(taskRow) {
    var taskNumber = taskRow.id.slice(-1);
    return parseInt(taskNumber);
}
function GetTaskRowIndex(taskButton) {
    var taskRow = $(taskButton).parents("tr");
    return taskRow[1].rowIndex;
}
function GetJobTask(taskNumber) {
    var jobTask = null;
    var mediaProcessor = $("#mediaProcessor" + taskNumber).val();
    if (mediaProcessor != null && mediaProcessor != "") {
        var taskParent = $("#taskParent" + taskNumber).val();
        var taskOptions = GetJobTaskOptions(taskNumber);
        jobTask = {
            MediaProcessor: mediaProcessor,
            ParentIndex: taskParent == "" ? null : taskParent - 1,
            OutputAssetName: $("#outputAssetName" + taskNumber).val(),
            ProcessorConfigString: {},
            ProcessorConfigBoolean: {},
            ProcessorConfigInteger: {},
            Options: taskOptions
        };
        switch (jobTask.MediaProcessor) {
            case "EncoderStandard":
            case "EncoderPremium":
                var encoderConfig = $("#encoderConfig" + taskNumber).val();
                if (encoderConfig == "Custom") {
                    jobTask.ProcessorConfig = _encoderConfig;
                } else {
                    jobTask.ProcessorConfigId = encoderConfig;
                }
                if ($("#encoderFragmentOutput" + taskNumber).prop("checked")) {
                    jobTask.OutputAssetFormat = 1; // AssetFormatOption.AdaptiveStreaming
                }
                jobTask.ContentProtection = GetContentProtection(taskNumber);
                break;
            case "VideoIndexer":
                jobTask.ContentIndexer = {
                    LanguageId: $("#indexerLanguageId" + taskNumber).val(),
                    SearchPartition: $("#indexerSearchPartition" + taskNumber).val(),
                    VideoDescription: $("#indexerVideoDescription" + taskNumber).val(),
                    VideoMetadata: $("#indexerVideoMetadata" + taskNumber).val(),
                    VideoPublic: $("#indexerVideoPublic" + taskNumber).prop("checked"),
                    AudioOnly: $("#indexerAudioOnly" + taskNumber).prop("checked")
                };
                break;
            case "VideoSummarization":
                var durationMinutes = parseInt($("#summarizationDurationMinutes" + taskNumber).val());
                var durationSeconds = parseInt($("#summarizationDurationSeconds" + taskNumber).val());
                jobTask.ProcessorConfigInteger["SummarizationDurationSeconds"] = (durationMinutes * 60) + durationSeconds;
                jobTask.ProcessorConfigBoolean["SummarizationFadeTransitions"] = $("#summarizationFadeTransitions" + taskNumber).prop("checked");
                jobTask.ProcessorConfigBoolean["SummarizationIncludeAudio"] = $("#summarizationIncludeAudio" + taskNumber).prop("checked");
                break;
            case "FaceDetection":
                jobTask.ProcessorConfigString["FaceDetectionMode"] = $("#faceDetectionMode" + taskNumber + ":checked").val();
                switch (jobTask.ProcessorConfigString["FaceDetectionMode"]) {
                    case "Redact":
                        jobTask.ProcessorConfigString["FaceDetectionMode"] = "Combined";
                        jobTask.ProcessorConfigString["FaceRedactionBlurMode"] = $("#faceRedactionBlurMode" + taskNumber).val();
                        break;
                    case "Emotion":
                        jobTask.ProcessorConfigString["FaceDetectionMode"] = $("#faceEmotionMode" + taskNumber + ":checked").val();
                        jobTask.ProcessorConfigInteger["FaceEmotionAggregateWindow"] = $("#faceEmotionAggregateWindow" + taskNumber).val();
                        jobTask.ProcessorConfigInteger["FaceEmotionAggregateInterval"] = $("#faceEmotionAggregateInterval" + taskNumber).val();
                        break;
                }
                break;
            case "SpeechAnalyzer":
                jobTask.ProcessorConfigString["SpeechAnalyzerLanguageId"] = $("#speechAnalyzerLanguageId" + taskNumber).val();
                jobTask.ProcessorConfigBoolean["SpeechAnalyzerTimedTextFormatTtml"] = $("#speechAnalyzerTimedTextFormatTtml" + taskNumber).prop("checked");
                jobTask.ProcessorConfigBoolean["SpeechAnalyzerTimedTextFormatWebVtt"] = $("#speechAnalyzerTimedTextFormatWebVtt" + taskNumber).prop("checked");
                break;
            case "MotionDetection":
                jobTask.ProcessorConfigString["MotionDetectionSensitivityLevel"] = $("#motionDetectionSensitivityLevel" + taskNumber).val();
                jobTask.ProcessorConfigBoolean["MotionDetectionLightChange"] = $("#motionDetectionLightChange" + taskNumber).prop("checked");
                break;
        }
    }
    return jobTask;
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
function SetEncoderConfig(fileInput) {
    var file = fileInput.files[0];
    var fileReader = new FileReader();
    fileReader.onload = function (e) {
        _encoderConfig = e.target.result;
    };
    fileReader.readAsText(file);
}
function SetFaceDetectionConfig(radioButton) {
    var taskNumber = radioButton.id.slice(-1);
    $("#faceRedactionRow" + taskNumber).hide();
    $("#faceEmotionRow" + taskNumber).hide();
    switch (radioButton.value) {
        case "Redact":
            $("#faceRedactionRow" + taskNumber).show();
            break;
        case "Emotion":
            $("#faceEmotionRow" + taskNumber).show();
            break;
    }
}
function SetFaceEmotionConfig(radioButton) {
    var taskNumber = radioButton.id.slice(-1);
    $("#faceEmotionAggregateRow" + taskNumber).hide();
    switch (radioButton.value) {
        case "AggregateEmotion":
            $("#faceEmotionAggregateRow" + taskNumber).show();
            break;
    }
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
function SetJobTaskInputs(taskNumber) {
    $.widget("ui.spinnerEx", $.ui.spinner, {
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
    $("#summarizationDurationMinutes" + taskNumber).spinnerEx({
        min: 0,
        max: 99
    });
    $("#summarizationDurationSeconds" + taskNumber).spinnerEx({
        min: 0,
        max: 59
    });
    $("#faceEmotionAggregateWindow" + taskNumber).spinner({
        min: 250,
        max: 2000
    });
    $("#faceEmotionAggregateInterval" + taskNumber).spinner({
        min: 250,
        max: 1000
    });
    $("#taskOptions" + taskNumber).multiselect({
        noneSelectedText: "0 of 3 Job Task Options",
        selectedText: "# of 3 Job Task Options",
        classes: "multiSelectOptions jobTaskOptions",
        header: false
    });
}
function ClearJobTaskWidgets(taskNumber) {
    $("#summarizationDurationMinutes" + taskNumber).spinnerEx("destroy");
    $("#summarizationDurationSeconds" + taskNumber).spinnerEx("destroy");
    $("#faceEmotionAggregateWindow" + taskNumber).spinner("destroy");
    $("#faceEmotionAggregateInterval" + taskNumber).spinner("destroy");
    $("#taskOptions" + taskNumber).multiselect("destroy");
}