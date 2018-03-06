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
                jobTask.ThumbnailGeneration = GetThumbnailGeneration(taskNumber, encoderConfig);
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
    $("#encoderThumbnailGenerationSpriteColumns" + taskNumber).spinner({
        min: 0,
        max: 99,
        disabled: true
    });
    $("#encoderThumbnailGenerationWidthPercent" + taskNumber).spinner({
        min: 0,
        max: 100,
        spin: function (event, ui) {
            ClearWidthPixel(this);
        }
    });
    $("#encoderThumbnailGenerationWidthPixel" + taskNumber).spinner({
        min: 0,
        max: 999,
        spin: function (event, ui) {
            ClearWidthPercent(this);
        }
    });
    $("#encoderThumbnailGenerationHeightPercent" + taskNumber).spinner({
        min: 0,
        max: 100,
        spin: function (event, ui) {
            ClearHeightPixel(this);
        }
    });
    $("#encoderThumbnailGenerationHeightPixel" + taskNumber).spinner({
        min: 0,
        max: 999,
        spin: function (event, ui) {
            ClearHeightPercent(this);
        }
    });
    $("#encoderThumbnailGenerationStartPercent" + taskNumber).spinner({
        min: 0,
        max: 100,
        spin: function (event, ui) {
            ClearStartTime(this);
        }
    });
    $("#encoderThumbnailGenerationStartHour" + taskNumber).spinnerEx({
        min: 0,
        max: 59,
        spin: function (event, ui) {
            ClearStartPercent(this, "encoderThumbnailGenerationStartHour");
        }
    });
    $("#encoderThumbnailGenerationStartMinute" + taskNumber).spinnerEx({
        min: 0,
        max: 59,
        spin: function (event, ui) {
            ClearStartPercent(this, "encoderThumbnailGenerationStartMinute");
        }
    });
    $("#encoderThumbnailGenerationStartSecond" + taskNumber).spinnerEx({
        min: 0,
        max: 59,
        spin: function (event, ui) {
            ClearStartPercent(this, "encoderThumbnailGenerationStartSecond");
        }
    });
    $("#encoderThumbnailGenerationStepPercent" + taskNumber).spinner({
        min: 0,
        max: 100,
        spin: function (event, ui) {
            ClearStepTime(this);
        }
    });
    $("#encoderThumbnailGenerationStepHour" + taskNumber).spinnerEx({
        min: 0,
        max: 59,
        spin: function (event, ui) {
            ClearStepPercent(this, "encoderThumbnailGenerationStepHour");
        }
    });
    $("#encoderThumbnailGenerationStepMinute" + taskNumber).spinnerEx({
        min: 0,
        max: 59,
        spin: function (event, ui) {
            ClearStepPercent(this, "encoderThumbnailGenerationStepMinute");
        }
    });
    $("#encoderThumbnailGenerationStepSecond" + taskNumber).spinnerEx({
        min: 0,
        max: 59,
        spin: function (event, ui) {
            ClearStepPercent(this, "encoderThumbnailGenerationStepSecond");
        }
    });
    $("#encoderThumbnailGenerationRangePercent" + taskNumber).spinner({
        min: 0,
        max: 100,
        spin: function (event, ui) {
            ClearRangeTime(this);
        }
    });
    $("#encoderThumbnailGenerationRangeHour" + taskNumber).spinnerEx({
        min: 0,
        max: 59,
        spin: function (event, ui) {
            ClearRangePercent(this, "encoderThumbnailGenerationRangeHour");
        }
    });
    $("#encoderThumbnailGenerationRangeMinute" + taskNumber).spinnerEx({
        min: 0,
        max: 59,
        spin: function (event, ui) {
            ClearRangePercent(this, "encoderThumbnailGenerationRangeMinute");
        }
    });
    $("#encoderThumbnailGenerationRangeSecond" + taskNumber).spinnerEx({
        min: 0,
        max: 59,
        spin: function (event, ui) {
            ClearRangePercent(this, "encoderThumbnailGenerationRangeSecond");
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
    $("#encoderThumbnailGenerationSpriteColumns" + taskNumber).spinner("destroy");
    $("#encoderThumbnailGenerationWidthPercent" + taskNumber).spinner("destroy");
    $("#encoderThumbnailGenerationWidthPixel" + taskNumber).spinner("destroy");
    $("#encoderThumbnailGenerationHeightPercent" + taskNumber).spinner("destroy");
    $("#encoderThumbnailGenerationHeightPixel" + taskNumber).spinner("destroy");
    $("#encoderThumbnailGenerationStartPercent" + taskNumber).spinner("destroy");
    $("#encoderThumbnailGenerationStartHour" + taskNumber).spinnerEx("destroy");
    $("#encoderThumbnailGenerationStartMinute" + taskNumber).spinnerEx("destroy");
    $("#encoderThumbnailGenerationStartSecond" + taskNumber).spinnerEx("destroy");
    $("#encoderThumbnailGenerationStepPercent" + taskNumber).spinner("destroy");
    $("#encoderThumbnailGenerationStepHour" + taskNumber).spinnerEx("destroy");
    $("#encoderThumbnailGenerationStepMinute" + taskNumber).spinnerEx("destroy");
    $("#encoderThumbnailGenerationStepSecond" + taskNumber).spinnerEx("destroy");
    $("#encoderThumbnailGenerationRangePercent" + taskNumber).spinner("destroy");
    $("#encoderThumbnailGenerationRangeHour" + taskNumber).spinnerEx("destroy");
    $("#encoderThumbnailGenerationRangeMinute" + taskNumber).spinnerEx("destroy");
    $("#encoderThumbnailGenerationRangeSecond" + taskNumber).spinnerEx("destroy");
    $("#summarizationDurationMinutes" + taskNumber).spinnerEx("destroy");
    $("#summarizationDurationSeconds" + taskNumber).spinnerEx("destroy");
    $("#faceEmotionAggregateWindow" + taskNumber).spinner("destroy");
    $("#faceEmotionAggregateInterval" + taskNumber).spinner("destroy");
    $("#taskOptions" + taskNumber).multiselect("destroy");
}