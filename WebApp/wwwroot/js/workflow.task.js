function GetTaskRowIndex(taskButton) {
    return $(taskButton).parents("tr")[1].rowIndex;
}
function GetLastTaskNumber(lastTaskRow) {
    return parseInt(lastTaskRow.id.replace("mediaWorkflowTask", ""));
}
function GetNewTaskRowHtml(lastTaskRow, lastTaskNumber, newTaskNumber) {
    var taskRowHtml = ReplaceAll(lastTaskRow.outerHTML, "mediaWorkflowTask" + lastTaskNumber, "mediaWorkflowTask" + newTaskNumber);
    taskRowHtml = ReplaceAll(taskRowHtml, "Job Task #" + lastTaskNumber, "Job Task #" + newTaskNumber);
    taskRowHtml = ReplaceAll(taskRowHtml, "mediaProcessor" + lastTaskNumber, "mediaProcessor" + newTaskNumber);
    taskRowHtml = ReplaceAll(taskRowHtml, "taskParent" + lastTaskNumber, "taskParent" + newTaskNumber);
    taskRowHtml = ReplaceAll(taskRowHtml, "encoderConfigRow" + lastTaskNumber, "encoderConfigRow" + newTaskNumber);
    taskRowHtml = ReplaceAll(taskRowHtml, "encoderConfig" + lastTaskNumber, "encoderConfig" + newTaskNumber);
    taskRowHtml = ReplaceAll(taskRowHtml, "encoderConfigFileRow" + lastTaskNumber, "encoderConfigFileRow" + newTaskNumber);
    taskRowHtml = ReplaceAll(taskRowHtml, "encoderConfigFile" + lastTaskNumber, "encoderConfigFile" + newTaskNumber);
    taskRowHtml = ReplaceAll(taskRowHtml, "indexerConfigRow" + lastTaskNumber, "indexerConfigRow" + newTaskNumber);
    taskRowHtml = ReplaceAll(taskRowHtml, "indexerSpokenLanguages" + lastTaskNumber, "indexerSpokenLanguages" + newTaskNumber);
    taskRowHtml = ReplaceAll(taskRowHtml, "indexerCaptionWebVtt" + lastTaskNumber, "indexerCaptionWebVtt" + newTaskNumber);
    taskRowHtml = ReplaceAll(taskRowHtml, "indexerCaptionTtml" + lastTaskNumber, "indexerCaptionTtml" + newTaskNumber);
    taskRowHtml = ReplaceAll(taskRowHtml, "faceConfigRow" + lastTaskNumber, "faceConfigRow" + newTaskNumber);
    taskRowHtml = ReplaceAll(taskRowHtml, "faceEmotionDetect" + lastTaskNumber, "faceEmotionDetect" + newTaskNumber);
    taskRowHtml = ReplaceAll(taskRowHtml, "faceEmotionWindowMilliseconds" + lastTaskNumber, "faceEmotionWindowMilliseconds" + newTaskNumber);
    taskRowHtml = ReplaceAll(taskRowHtml, "faceEmotionIntervalMilliseconds" + lastTaskNumber, "faceEmotionIntervalMilliseconds" + newTaskNumber);
    taskRowHtml = ReplaceAll(taskRowHtml, "motionConfigRow" + lastTaskNumber, "motionConfigRow" + newTaskNumber);
    taskRowHtml = ReplaceAll(taskRowHtml, "motionSensitivityLevel" + lastTaskNumber, "motionSensitivityLevel" + newTaskNumber);
    taskRowHtml = ReplaceAll(taskRowHtml, "motionDetectLightChange" + lastTaskNumber, "motionDetectLightChange" + newTaskNumber);
    taskRowHtml = ReplaceAll(taskRowHtml, "hyperlapseConfigRow" + lastTaskNumber, "hyperlapseConfigRow" + newTaskNumber);
    taskRowHtml = ReplaceAll(taskRowHtml, "hyperlapseStartFrame" + lastTaskNumber, "hyperlapseStartFrame" + newTaskNumber);
    taskRowHtml = ReplaceAll(taskRowHtml, "hyperlapseFrameCount" + lastTaskNumber, "hyperlapseFrameCount" + newTaskNumber);
    taskRowHtml = ReplaceAll(taskRowHtml, "hyperlapseSpeed" + lastTaskNumber, "hyperlapseSpeed" + newTaskNumber);
    taskRowHtml = ReplaceAll(taskRowHtml, "summaryConfigRow" + lastTaskNumber, "summaryConfigRow" + newTaskNumber);
    taskRowHtml = ReplaceAll(taskRowHtml, "summaryDurationSeconds" + lastTaskNumber, "summaryDurationSeconds" + newTaskNumber);
    taskRowHtml = ReplaceAll(taskRowHtml, "outputAssetName" + lastTaskNumber, "outputAssetName" + newTaskNumber);
    taskRowHtml = ReplaceAll(taskRowHtml, "taskOptions" + lastTaskNumber, "taskOptions" + newTaskNumber);
    taskRowHtml = ReplaceAll(taskRowHtml, "this, " + lastTaskNumber, "this, " + newTaskNumber);
    return taskRowHtml;
}
function AddJobTaskLink(workflowTable, taskRowIndex) {
    var taskLinkRow = workflowTable.insertRow(taskRowIndex);
    var rowCell0 = taskLinkRow.insertCell(0);
    var rowCell1 = taskLinkRow.insertCell(1);
    taskLinkRow.className = "mediaWorkflowTaskLink";
    rowCell1.innerHTML = "<hr />";
}
function AddJobTask(taskButton) {
    var workflowTable = document.getElementById("mediaWorkflow");
    var taskRowIndex = GetTaskRowIndex(taskButton);
    var lastTaskRow = workflowTable.rows[taskRowIndex - 1];
    var lastTaskNumber = GetLastTaskNumber(lastTaskRow);
    var newTaskRow = workflowTable.insertRow(taskRowIndex);
    var newTaskNumber = lastTaskNumber + 1;
    ClearJobTaskWidgets(lastTaskNumber);
    newTaskRow.outerHTML = GetNewTaskRowHtml(lastTaskRow, lastTaskNumber, newTaskNumber);
    AddJobTaskLink(workflowTable, taskRowIndex);
    SetJobTaskParents(newTaskNumber);
    SetJobTaskWidgets(newTaskNumber, false);
    SetJobTaskWidgets(lastTaskNumber, false);
    var mediaProcessor = $("#mediaProcessor" + newTaskNumber)[0];
    SetProcessorConfig(mediaProcessor, newTaskNumber);
    $("#mediaWorkflowTaskRemove").show();
    if (lastTaskNumber == 8) {
        var marginLeft = $("#mediaWorkflowTaskAdd").css("margin-left");
        $("#mediaWorkflowTaskRemove").css("margin-left", marginLeft);
        $("#mediaWorkflowTaskAdd").hide();
    }
}
function RemoveJobTask(taskButton) {
    var workflowTable = document.getElementById("mediaWorkflow");
    var taskRowIndex = GetTaskRowIndex(taskButton);
    var lastTaskRow = workflowTable.rows[taskRowIndex - 1];
    var lastTaskNumber = GetLastTaskNumber(lastTaskRow);
    workflowTable.deleteRow(lastTaskRow.rowIndex - 1);
    workflowTable.deleteRow(lastTaskRow.rowIndex);
    $("#mediaWorkflowTaskAdd").show();
    $("#mediaWorkflowTaskRemove").css("margin-left", "");
    if (lastTaskNumber == 2) {
        $("#mediaWorkflowTaskRemove").hide();
    }
}
function EncoderSelected() {
    var taskNumber = 1;
    var encoderSelected = false;
    do {
        var mediaProcessor = $("#mediaProcessor" + taskNumber).val();
        if (mediaProcessor == "EncoderStandard" || mediaProcessor == "EncoderPremium") {
            encoderSelected = true;
        }
        taskNumber = taskNumber + 1;
    } while (mediaProcessor != null)
    return encoderSelected;
}
function ResetProcessorConfig(encoderConfigOptions, encoderConfigFileId, indexerLanguageOptions, hideRowIds) {
    $("#" + encoderConfigFileId).val("");
    if (encoderConfigOptions != null) {
        encoderConfigOptions.length = 0;
    }
    if (indexerLanguageOptions != null) {
        indexerLanguageOptions.length = 0;
    }
    for (var i = 0; i < hideRowIds.length; i++) {
        $("#" + hideRowIds[i]).hide();
    }
    if (!EncoderSelected()) {
        $("#mediaWorkflowProtectContent").hide();
        $("#mediaWorkflowArchiveInput").hide();
    }
}
function SetProcessorConfig(mediaProcessor, taskNumber) {
    var encoderConfigRowId = mediaProcessor.id.replace("mediaProcessor", "encoderConfigRow");
    var encoderConfigId = mediaProcessor.id.replace("mediaProcessor", "encoderConfig");
    var encoderConfigFileRowId = mediaProcessor.id.replace("mediaProcessor", "encoderConfigFileRow");
    var encoderConfigFileId = mediaProcessor.id.replace("mediaProcessor", "encoderConfigFile");
    var encoderConfigOptions = $("#" + encoderConfigId)[0].options;
    var indexerConfigRowId = mediaProcessor.id.replace("mediaProcessor", "indexerConfigRow");
    var indexerLanguagesId = mediaProcessor.id.replace("mediaProcessor", "indexerSpokenLanguages");
    var indexerLanguageOptions = $("#" + indexerLanguagesId)[0].options;
    var faceConfigRowId = mediaProcessor.id.replace("mediaProcessor", "faceConfigRow");
    var motionConfigRowId = mediaProcessor.id.replace("mediaProcessor", "motionConfigRow");
    var hyperlapseConfigRowId = mediaProcessor.id.replace("mediaProcessor", "hyperlapseConfigRow");
    var summaryConfigRowId = mediaProcessor.id.replace("mediaProcessor", "summaryConfigRow");
    var hideRowIds = [encoderConfigRowId, encoderConfigFileRowId, indexerConfigRowId, faceConfigRowId, motionConfigRowId, hyperlapseConfigRowId, summaryConfigRowId];
    ResetProcessorConfig(encoderConfigOptions, encoderConfigFileId, indexerLanguageOptions, hideRowIds);
    if (mediaProcessor.value != "None") {
        switch (mediaProcessor.value) {
            case "EncoderStandard":
                encoderConfigOptions[encoderConfigOptions.length] = new Option("Preset - H264 MBR 1080p (8 MP4s, 400 to 6000 kbps) AAC 5.1", "H264 Multiple Bitrate 1080p Audio 5.1");
                encoderConfigOptions[encoderConfigOptions.length] = new Option("Preset - H264 MBR 1080p (8 MP4s, 400 to 6000 kbps) AAC", "H264 Multiple Bitrate 1080p");
                encoderConfigOptions[encoderConfigOptions.length] = new Option("Preset - H264 MBR 16x9 for iOS (8 MP4s, 200 to 8500 kbps) AAC", "H264 Multiple Bitrate 16x9 for iOS");
                encoderConfigOptions[encoderConfigOptions.length] = new Option("Preset - H264 MBR 16x9 SD (5 MP4s, 400 to 1900 kbps) AAC 5.1", "H264 Multiple Bitrate 16x9 SD Audio 5.1");
                encoderConfigOptions[encoderConfigOptions.length] = new Option("Preset - H264 MBR 16x9 SD (5 MP4s, 400 to 1900 kbps) AAC", "H264 Multiple Bitrate 16x9 SD");
                encoderConfigOptions[encoderConfigOptions.length] = new Option("Preset - H264 MBR 4K (12 MP4s, 1000 to 20000 kbps) AAC 5.1", "H264 Multiple Bitrate 4K Audio 5.1");
                encoderConfigOptions[encoderConfigOptions.length] = new Option("Preset - H264 MBR 4K (12 MP4s, 1000 to 20000 kbps) AAC", "H264 Multiple Bitrate 4K");
                encoderConfigOptions[encoderConfigOptions.length] = new Option("Preset - H264 MBR 4x3 for iOS (8 MP4s, 200 to 8500 kbps) AAC", "H264 Multiple Bitrate 4x3 for iOS");
                encoderConfigOptions[encoderConfigOptions.length] = new Option("Preset - H264 MBR 4x3 SD (5 MP4s, 400 to 1600 kbps) AAC 5.1", "H264 Multiple Bitrate 4x3 SD Audio 5.1");
                encoderConfigOptions[encoderConfigOptions.length] = new Option("Preset - H264 MBR 4x3 SD (5 MP4s, 400 to 1600 kbps) AAC", "H264 Multiple Bitrate 4x3 SD");
                encoderConfigOptions[encoderConfigOptions.length] = new Option("Preset - H264 MBR 720p (6 MP4s, 400 to 3400 kbps) AAC 5.1", "H264 Multiple Bitrate 720p Audio 5.1");
                encoderConfigOptions[encoderConfigOptions.length] = new Option("Preset - H264 MBR 720p (6 MP4s, 400 to 3400 kbps) AAC", "H264 Multiple Bitrate 720p");
                encoderConfigOptions[encoderConfigOptions.length] = new Option("Custom - Media Processor Configuration File (JSON)", "Custom");
                break;
            case "EncoderPremium":
                encoderConfigOptions[encoderConfigOptions.length] = new Option("None - No Custom Media Processor Configuration File", "None");
                encoderConfigOptions[encoderConfigOptions.length] = new Option("Custom - Media Processor Configuration File (XML)", "Custom");
                break;
            case "IndexerV1":
                $("#indexerSpokenLanguages" + taskNumber).multiselect("destroy");
                indexerLanguageOptions[indexerLanguageOptions.length] = new Option("English", "EnUs");
                indexerLanguageOptions[indexerLanguageOptions.length - 1].selected = true;
                indexerLanguageOptions[indexerLanguageOptions.length] = new Option("Spanish", "EsEs");
                SetJobTaskWidgets(taskNumber, true);
                $("#" + indexerConfigRowId).show();
                break;
            case "IndexerV2":
                $("#indexerSpokenLanguages" + taskNumber).multiselect("destroy");
                indexerLanguageOptions[indexerLanguageOptions.length] = new Option("English", "EnUs");
                indexerLanguageOptions[indexerLanguageOptions.length - 1].selected = true;
                indexerLanguageOptions[indexerLanguageOptions.length] = new Option("Spanish", "EsEs");
                indexerLanguageOptions[indexerLanguageOptions.length] = new Option("Arabic (Egyptian)", "ArEg");
                indexerLanguageOptions[indexerLanguageOptions.length] = new Option("Chinese", "ZhCn");
                indexerLanguageOptions[indexerLanguageOptions.length] = new Option("French", "FrFr");
                indexerLanguageOptions[indexerLanguageOptions.length] = new Option("German", "DeDe");
                indexerLanguageOptions[indexerLanguageOptions.length] = new Option("Italian", "ItIt");
                indexerLanguageOptions[indexerLanguageOptions.length] = new Option("Japanese", "JaJp");
                indexerLanguageOptions[indexerLanguageOptions.length] = new Option("Portuguese", "PtBr");
                SetJobTaskWidgets(taskNumber, false);
                $("#" + indexerConfigRowId).show();
                break;
            case "FaceDetection":
                $("#" + faceConfigRowId).show();
                break;
            case "MotionDetection":
                $("#" + motionConfigRowId).show();
                break;
            case "MotionHyperlapse":
                $("#" + hyperlapseConfigRowId).show();
                break;
            case "VideoSummarization":
                $("#" + summaryConfigRowId).show();
                break;
        }
        if (encoderConfigOptions.length > 0) {
            $("#" + encoderConfigRowId).show();
            $("#" + encoderConfigId).change();
        }
    }
    if (EncoderSelected()) {
        $("#mediaWorkflowProtectContent").show();
        $("#mediaWorkflowArchiveInput").show();
    }
}
function SetEncoderConfigFile(encoderConfig) {
    var encoderConfigFileRowId = encoderConfig.id.replace("encoderConfig", "encoderConfigFileRow");
    if (encoderConfig.value == "Custom") {
        $("#" + encoderConfigFileRowId).show();
    } else {
        $("#" + encoderConfigFileRowId).hide();
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
function SetJobTaskWidgets(taskNumber, indexerV1) {
    var languageCount = $("#indexerSpokenLanguages" + taskNumber + " option").length;
    $("#indexerSpokenLanguages" + taskNumber).multiselect({
        noneSelectedText: "0 of " + languageCount + " Languages Enabled",
        selectedText: "# of " + languageCount + " Languages Enabled",
        classes: "multiSelect mediaProcessor" + (indexerV1 ? " indexerLanguages" : ""),
        header: false
    });
    $("#faceEmotionWindowMilliseconds" + taskNumber).spinner({
        min: 250,
        max: 2000
    });
    $("#faceEmotionIntervalMilliseconds" + taskNumber).spinner({
        min: 250,
        max: 1000
    });
    $("#hyperlapseStartFrame" + taskNumber).spinner({
        min: 0,
        max: 9999
    });
    $("#hyperlapseFrameCount" + taskNumber).spinner({
        min: 0,
        max: 10000
    });
    $.widget("ui.spinnerEx", $.ui.spinner, {
        _format: function (value) {
            return value + "x";
        },
        _parse: function (value) {
            return parseFloat(value);
        }
    });
    $("#hyperlapseSpeed" + taskNumber).spinnerEx({
        min: 1,
        max: 9
    });
    $("#summaryDurationSeconds" + taskNumber).slider({
        min: 15,
        max: 180,
        step: 1,
        slide: function (event, ui) {
            var dateTime = new Date(null);
            dateTime.setSeconds(ui.value);
            var seconds = dateTime.getSeconds();
            if (seconds < 10) {
                seconds = "0" + seconds;
            }
            var duration = dateTime.getMinutes() + ":" + seconds;
            $("#summaryDurationSecondsLabel" + taskNumber).text(duration);
        }
    });
    $("#taskOptions" + taskNumber).multiselect({
        noneSelectedText: "0 of 3 Task Options Enabled",
        selectedText: "# of 3 Task Options Enabled",
        classes: "multiSelect jobTaskOptions",
        header: false
    });
}
function ClearJobTaskWidgets(lastTaskNumber) {
    $("#indexerSpokenLanguages" + lastTaskNumber).multiselect("destroy");
    $("#faceEmotionWindowMilliseconds" + lastTaskNumber).spinner("destroy");
    $("#faceEmotionIntervalMilliseconds" + lastTaskNumber).spinner("destroy");
    $("#hyperlapseStartFrame" + lastTaskNumber).spinner("destroy");
    $("#hyperlapseFrameCount" + lastTaskNumber).spinner("destroy");
    $("#hyperlapseSpeed" + lastTaskNumber).spinnerEx("destroy");
    $("#taskOptions" + lastTaskNumber).multiselect("destroy");
}
function GetJobTaskOptions(taskNumber) {
    var taskOptions = 0;
    var selectedOptions = $("#taskOptions" + taskNumber).val();
    for (var i = 0; i < selectedOptions.length; i++) {
        taskOptions = taskOptions + parseInt(selectedOptions[i]);
    }
    return taskOptions;
}
function GetJobTask(taskNumber) {
    var jobTask = null;
    var mediaProcessor = $("#mediaProcessor" + taskNumber).val();
    if (mediaProcessor != null && mediaProcessor != "None") {
        var taskParent = $("#taskParent" + taskNumber).val();
        jobTask = {
            ParentIndex: (taskParent == null) ? null : taskParent - 1,
            OutputAssetName: $("#outputAssetName" + taskNumber).val(),
            Options: GetJobTaskOptions(taskNumber),
            MediaProcessor: mediaProcessor
        };
        switch (jobTask.MediaProcessor) {
            case "EncoderStandard":
            case "EncoderPremium":
                var encoderConfig = $("#encoderConfig" + taskNumber).val();
                if (encoderConfig == "Custom") {
                    var fileReader = new FileReader();
                    fileReader.onload = function (e) {
                        jobTask.ProcessorConfig = e.target.result;
                    };
                    var encoderConfigFile = $("#encoderConfigFile" + taskNumber)[0].files[0];
                    fileReader.readAsText(encoderConfigFile);
                    alert("Custom encoder configuration loaded.");
                } else {
                    jobTask.ProcessorConfig = $("#encoderConfig" + taskNumber).val();
                }
                break;
            case "IndexerV1":
                jobTask.IndexerSpokenLanguages = new Array();
                $("#indexerSpokenLanguages" + taskNumber + " :selected").each(function (i, o) {
                    jobTask.IndexerSpokenLanguages[i] = $(o).text();
                });
                jobTask.IndexerCaptionWebVtt = $("#indexerCaptionWebVtt" + taskNumber).prop("checked");
                jobTask.IndexerCaptionTtml = $("#indexerCaptionTtml" + taskNumber).prop("checked");
                break
            case "IndexerV2":
                jobTask.IndexerSpokenLanguages = $("#indexerSpokenLanguages" + taskNumber).val();
                jobTask.IndexerCaptionWebVtt = $("#indexerCaptionWebVtt" + taskNumber).prop("checked");
                jobTask.IndexerCaptionTtml = $("#indexerCaptionTtml" + taskNumber).prop("checked");
                break;
            case "FaceDetection":
                jobTask.FaceEmotionDetect = $("#faceEmotionDetect" + taskNumber).prop("checked");
                jobTask.FaceEmotionWindowMilliseconds = $("#faceEmotionWindowMilliseconds" +taskNumber).val();
                jobTask.FaceEmotionIntervalMilliseconds = $("#faceEmotionIntervalMilliseconds" + taskNumber).val();
                break;
            case "MotionDetection":
                jobTask.MotionSensitivityLevel = $("#motionSensitivityLevel" + taskNumber).val();
                jobTask.MotionDetectLightChange = $("#motionDetectLightChange" + taskNumber).prop("checked");
                break;
            case "MotionHyperlapse":
                jobTask.HyperlapseStartFrame = $("#hyperlapseStartFrame" + taskNumber).val();
                jobTask.HyperlapseFrameCount = $("#hyperlapseFrameCount" + taskNumber).val();
                jobTask.HyperlapseSpeed = $("#hyperlapseSpeed" + taskNumber).val().substr(0, 1);
                break;
            case "VideoSummarization":
                var durationLabel = $("#summaryDurationSecondsLabel" + taskNumber).text();
                var durationInfo = durationLabel.split(":");
                var durationSeconds = (parseInt(durationInfo[0]) * 60) + parseInt(durationInfo[1]);
                jobTask.SummaryDurationSeconds = durationSeconds;
                break;
        }
    }
    return jobTask;
}
function GetJobTasks() {
    var taskNumber = 1;
    var jobTasks = new Array();
    do {
        var jobTask = GetJobTask(taskNumber);
        if (jobTask != null) {
            jobTasks.push(jobTask);
        }
        taskNumber = taskNumber + 1;
    } while (jobTask != null)
    return jobTasks;
}
