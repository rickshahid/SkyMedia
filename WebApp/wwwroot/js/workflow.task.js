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
            Options: taskOptions,
            ProcessorConfigBoolean: {},
            ProcessorConfigInteger: {},
            ProcessorConfigString: {}
        };
        switch (jobTask.MediaProcessor) {
            case "EncoderStandard":
            case "EncoderPremium":
            case "EncoderUltra":
                var encoderConfig = $("#encoderConfig" + taskNumber).val();
                if (encoderConfig == "Custom") {
                    jobTask.ProcessorConfig = _encoderConfig;
                } else {
                    jobTask.ProcessorConfig = encoderConfig;
                }
                if ($("#encoderFragmentOutput" + taskNumber).prop("checked")) {
                    jobTask.OutputAssetFormat = 1; // AssetFormatOption.AdaptiveStreaming
                }
                jobTask.ContentProtection = GetContentProtection(taskNumber);
                break;
            case "VideoIndexer":
                jobTask.ProcessorConfigString["SpokenLanguage"] = $("#indexerLanguage" + taskNumber).val();
                jobTask.ProcessorConfigString["SearchPartition"] = $("#indexerSearchPartition" + taskNumber).val();
                jobTask.ProcessorConfigString["VideoDescription"] = $("#indexerVideoDescription" + taskNumber).val();
                jobTask.ProcessorConfigString["VideoMetadata"] = $("#indexerVideoMetadata" + taskNumber).val();
                jobTask.ProcessorConfigBoolean["VideoPublic"] = $("#indexerVideoPublic" + taskNumber).prop("checked");
                jobTask.ProcessorConfigBoolean["AudioOnly"] = $("#indexerAudioOnly" + taskNumber).prop("checked");
                break;
            case "VideoSummarization":
                var durationMinutes = $("#summarizationDurationMinutes" + taskNumber).val();
                var durationSeconds = $("#summarizationDurationSeconds" + taskNumber).val();
                jobTask.ProcessorConfigInteger["SummarizationDurationSeconds"] = (durationMinutes * 60) + durationSeconds;
                jobTask.ProcessorConfigBoolean["SummarizationFadeTransitions"] = $("#summarizationFadeTransitions" + taskNumber).prop("checked");
                jobTask.ProcessorConfigBoolean["VideoOnly"] = !$("#summarizationIncludeAudio" + taskNumber).prop("checked");
                break;
            case "SpeechAnalyzer":
                jobTask.ProcessorConfigString["SpokenLanguage"] = $("#speechAnalyzerLanguage" + taskNumber).val();
                jobTask.ProcessorConfigBoolean["CaptionFormatTtml"] = $("#speechAnalyzerCaptionTtml" + taskNumber).prop("checked");
                jobTask.ProcessorConfigBoolean["CaptionFormatWebVtt"] = $("#speechAnalyzerCaptionWebVtt" + taskNumber).prop("checked");
                break;
            case "FaceDetection":
                jobTask.ProcessorConfigString["FaceDetectionMode"] = $("#faceDetectionMode" + taskNumber + ":checked").val();
                jobTask.ProcessorConfigString["FaceDetectionTrackingMode"] = $("#faceDetectionTrackingMode" + taskNumber).val();
                jobTask.ProcessorConfigInteger["FaceDetectionAggregateEmotionWindow"] = $("#faceDetectionAggregateEmotionWindow" + taskNumber).val();
                jobTask.ProcessorConfigInteger["FaceDetectionAggregateEmotionInterval"] = $("#faceDetectionAggregateEmotionInterval" + taskNumber).val();
                break;
            case "FaceRedaction":
                jobTask.ProcessorConfigString["FaceRedactionMode"] = $("#faceRedactionMode" + taskNumber + ":checked").val();
                jobTask.ProcessorConfigString["FaceRedactionBlurMode"] = $("#faceRedactionBlurMode" + taskNumber).val();
                break;
            case "MotionDetection":
                jobTask.ProcessorConfigString["MotionDetectionSensitivityLevel"] = $("#motionDetectionSensitivityLevel" + taskNumber).val();
                jobTask.ProcessorConfigBoolean["MotionDetectionLightChange"] = $("#motionDetectionLightChange" + taskNumber).prop("checked");
                break;
            case "MotionHyperlapse":
                jobTask.ProcessorConfigInteger["MotionHyperlapseFrameStart"] = $("#motionHyperlapseFrameStart" + taskNumber).val();
                jobTask.ProcessorConfigInteger["MotionHyperlapseFrameEnd"] = $("#motionHyperlapseFrameEnd" + taskNumber).val();
                jobTask.ProcessorConfigInteger["MotionHyperlapseSpeed"] = $("#motionHyperlapseSpeed" + taskNumber).val().substr(0, 1);
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
    $("#faceDetectionFacesRow" + taskNumber).hide();
    $("#faceDetectionAggregateEmotionRow" + taskNumber).hide();
    switch (radioButton.value) {
        case "Faces":
            $("#faceDetectionFacesRow" + taskNumber).show();
            break;
        case "AggregateEmotion":
            $("#faceDetectionAggregateEmotionRow" + taskNumber).show();
            break;
        case "PerFaceEmotion":
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
    $("#faceDetectionAggregateEmotionWindow" + taskNumber).spinner({
        min: 250,
        max: 2000
    });
    $("#faceDetectionAggregateEmotionInterval" + taskNumber).spinner({
        min: 250,
        max: 1000
    });
    $("#motionHyperlapseFrameStart" + taskNumber).spinner({
        min: 0,
        max: 99999999
    });
    $("#motionHyperlapseFrameEnd" + taskNumber).spinner({
        min: 0,
        max: 99999999
    });
    $("#motionHyperlapseSpeed" + taskNumber).spinnerEx2({
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
function ClearJobTaskWidgets(taskNumber) {
    $("#summarizationDurationMinutes" + taskNumber).spinnerEx1("destroy");
    $("#summarizationDurationSeconds" + taskNumber).spinnerEx1("destroy");
    $("#faceDetectionAggregateEmotionWindow" + taskNumber).spinner("destroy");
    $("#faceDetectionAggregateEmotionInterval" + taskNumber).spinner("destroy");
    $("#motionHyperlapseFrameStart" + taskNumber).spinner("destroy");
    $("#motionHyperlapseFrameEnd" + taskNumber).spinner("destroy");
    $("#motionHyperlapseSpeed" + taskNumber).spinnerEx2("destroy");
    $("#taskOptions" + taskNumber).multiselect("destroy");
}