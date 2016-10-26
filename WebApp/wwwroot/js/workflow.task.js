function GetTaskRowIndex(taskButton) {
    return $(taskButton).parents("tr")[1].rowIndex;
}
function GetLastTaskNumber(lastTaskRow) {
    return parseInt(lastTaskRow.id.replace("mediaWorkflowTask", ""));
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
    $("#spokenLanguages" + lastTaskNumber).multiselect("destroy");
    $("#taskOptions" + lastTaskNumber).multiselect("destroy");
    var newTaskRowHtml = ReplaceAll(lastTaskRow.outerHTML, "mediaWorkflowTask" + lastTaskNumber, "mediaWorkflowTask" + newTaskNumber);
    newTaskRowHtml = ReplaceAll(newTaskRowHtml, "Job Task #" + lastTaskNumber, "Job Task #" + newTaskNumber);
    newTaskRowHtml = ReplaceAll(newTaskRowHtml, "mediaProcessor" + lastTaskNumber, "mediaProcessor" + newTaskNumber);
    newTaskRowHtml = ReplaceAll(newTaskRowHtml, "taskParent" + lastTaskNumber, "taskParent" + newTaskNumber);
    newTaskRowHtml = ReplaceAll(newTaskRowHtml, "processorConfigRow" + lastTaskNumber, "processorConfigRow" + newTaskNumber);
    newTaskRowHtml = ReplaceAll(newTaskRowHtml, "processorConfig" + lastTaskNumber, "processorConfig" + newTaskNumber);
    newTaskRowHtml = ReplaceAll(newTaskRowHtml, "processorConfigFileRow" + lastTaskNumber, "processorConfigFileRow" + newTaskNumber);
    newTaskRowHtml = ReplaceAll(newTaskRowHtml, "processorConfigFile" + lastTaskNumber, "processorConfigFile" + newTaskNumber);
    newTaskRowHtml = ReplaceAll(newTaskRowHtml, "indexerConfigRow" + lastTaskNumber, "indexerConfigRow" + newTaskNumber);
    newTaskRowHtml = ReplaceAll(newTaskRowHtml, "spokenLanguages" + lastTaskNumber, "spokenLanguages" + newTaskNumber);
    newTaskRowHtml = ReplaceAll(newTaskRowHtml, "captionFormatWebVtt" + lastTaskNumber, "captionFormatWebVtt" + newTaskNumber);
    newTaskRowHtml = ReplaceAll(newTaskRowHtml, "captionFormatTtml" + lastTaskNumber, "captionFormatTtml" + newTaskNumber);
    newTaskRowHtml = ReplaceAll(newTaskRowHtml, "outputAssetName" + lastTaskNumber, "outputAssetName" + newTaskNumber);
    newTaskRowHtml = ReplaceAll(newTaskRowHtml, "taskOptions" + lastTaskNumber, "taskOptions" + newTaskNumber);
    newTaskRow.outerHTML = newTaskRowHtml;
    AddJobTaskLink(workflowTable, taskRowIndex);
    SetJobTaskParents(newTaskNumber);
    SetJobTaskOptions(newTaskNumber);
    SetJobTaskOptions(lastTaskNumber);
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
function ResetProcessorConfig(processorConfigRowId, processorConfigOptions, processorConfigFileRowId, processorConfigFileId, indexerConfigRowId) {
    $("#" + processorConfigFileId).val("");
    $("#" + processorConfigFileRowId).hide();
    if (processorConfigOptions != null) {
        processorConfigOptions.length = 0;
    }
    $("#" + processorConfigRowId).hide();
    $("#" + indexerConfigRowId).hide();
    if (!EncoderSelected()) {
        $("#mediaWorkflowContentProtection").hide();
    }
}
function SetProcessorConfig(mediaProcessor) {
    var processorConfigRowId = mediaProcessor.id.replace("mediaProcessor", "processorConfigRow");
    var processorConfigId = mediaProcessor.id.replace("mediaProcessor", "processorConfig");
    var processorConfigFileRowId = mediaProcessor.id.replace("mediaProcessor", "processorConfigFileRow");
    var processorConfigFileId = mediaProcessor.id.replace("mediaProcessor", "processorConfigFile");
    var processorConfigOptions = $("#" + processorConfigId)[0].options;
    var indexerConfigRowId = mediaProcessor.id.replace("mediaProcessor", "indexerConfigRow");
    ResetProcessorConfig(processorConfigRowId, processorConfigOptions, processorConfigFileRowId, processorConfigFileId, indexerConfigRowId);
    if (mediaProcessor.value != "None") {
        switch (mediaProcessor.value) {
            case "EncoderStandard":
                processorConfigOptions[processorConfigOptions.length] = new Option("Preset - H264 MBR 1080p (8 MP4s, 400 to 6000 kbps) AAC 5.1", "H264 Multiple Bitrate 1080p Audio 5.1");
                processorConfigOptions[processorConfigOptions.length] = new Option("Preset - H264 MBR 1080p (8 MP4s, 400 to 6000 kbps) AAC", "H264 Multiple Bitrate 1080p");
                processorConfigOptions[processorConfigOptions.length] = new Option("Preset - H264 MBR 16x9 for iOS (8 MP4s, 200 to 8500 kbps) AAC", "H264 Multiple Bitrate 16x9 for iOS");
                processorConfigOptions[processorConfigOptions.length] = new Option("Preset - H264 MBR 16x9 SD (5 MP4s, 400 to 1900 kbps) AAC 5.1", "H264 Multiple Bitrate 16x9 SD Audio 5.1");
                processorConfigOptions[processorConfigOptions.length] = new Option("Preset - H264 MBR 16x9 SD (5 MP4s, 400 to 1900 kbps) AAC", "H264 Multiple Bitrate 16x9 SD");
                processorConfigOptions[processorConfigOptions.length] = new Option("Preset - H264 MBR 4K (12 MP4s, 1000 to 20000 kbps) AAC 5.1", "H264 Multiple Bitrate 4K Audio 5.1");
                processorConfigOptions[processorConfigOptions.length] = new Option("Preset - H264 MBR 4K (12 MP4s, 1000 to 20000 kbps) AAC", "H264 Multiple Bitrate 4K");
                processorConfigOptions[processorConfigOptions.length] = new Option("Preset - H264 MBR 4x3 for iOS (8 MP4s, 200 to 8500 kbps) AAC", "H264 Multiple Bitrate 4x3 for iOS");
                processorConfigOptions[processorConfigOptions.length] = new Option("Preset - H264 MBR 4x3 SD (5 MP4s, 400 to 1600 kbps) AAC 5.1", "H264 Multiple Bitrate 4x3 SD Audio 5.1");
                processorConfigOptions[processorConfigOptions.length] = new Option("Preset - H264 MBR 4x3 SD (5 MP4s, 400 to 1600 kbps) AAC", "H264 Multiple Bitrate 4x3 SD");
                processorConfigOptions[processorConfigOptions.length] = new Option("Preset - H264 MBR 720p (6 MP4s, 400 to 3400 kbps) AAC 5.1", "H264 Multiple Bitrate 720p Audio 5.1");
                processorConfigOptions[processorConfigOptions.length] = new Option("Preset - H264 MBR 720p (6 MP4s, 400 to 3400 kbps) AAC", "H264 Multiple Bitrate 720p");
                processorConfigOptions[processorConfigOptions.length] = new Option("Custom - Media Processor Configuration File (JSON)", "Custom");
                break;
            case "EncoderPremium":
                processorConfigOptions[processorConfigOptions.length] = new Option("None - No Custom Media Processor Configuration File", "None");
                processorConfigOptions[processorConfigOptions.length] = new Option("Custom - Media Processor Configuration File (XML)", "Custom");
                break;
            case "IndexerV2":
                $("#" + indexerConfigRowId).show();
                break;
        }
        if (processorConfigOptions.length > 0) {
            $("#" + processorConfigRowId).show();
            $("#" + processorConfigId).change();
        }
    }
    if (EncoderSelected()) {
        $("#mediaWorkflowContentProtection").show();
    }
}
function SetProcessorConfigFile(processorConfig) {
    var processorConfigFileRowId = processorConfig.id.replace("processorConfig", "processorConfigFileRow");
    if (processorConfig.value == "Custom") {
        $("#" + processorConfigFileRowId).show();
    } else {
        $("#" + processorConfigFileRowId).hide();
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
function SetJobTaskOptions(taskNumber) {
    var languageCount = $("#spokenLanguages1 option").length;
    $("#spokenLanguages" + taskNumber).multiselect({
        noneSelectedText: "0 of " + languageCount + " Languages Enabled",
        selectedText: "# of " + languageCount + " Languages Enabled",
        classes: "multiSelect mediaProcessor",
        header: false
    });
    $("#taskOptions" + taskNumber).multiselect({
        noneSelectedText: "0 of 3 Task Options Enabled",
        selectedText: "# of 3 Task Options Enabled",
        classes: "multiSelect jobTaskOptions",
        header: false
    });
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
                var processorConfig = $("#processorConfig" + taskNumber).val();
                if (processorConfig == "Custom") {
                    var fileReader = new FileReader();
                    fileReader.onload = function (e) {
                        jobTask.ProcessorConfig = e.target.result;
                    };
                    var processorConfigFile = $("#processorConfigFile" + taskNumber)[0].files[0];
                    fileReader.readAsText(processorConfigFile);
                    alert("Custom encoder configuration loaded.");
                } else {
                    jobTask.ProcessorConfig = $("#processorConfig" + taskNumber).val();
                }
                break;
            case "IndexerV2":
                jobTask.SpokenLanguages = $("#spokenLanguages" + taskNumber).val();
                jobTask.CaptionFormatWebVtt = $("#captionFormatWebVtt" + taskNumber).prop("checked");
                jobTask.CaptionFormatTtml = $("#captionFormatTtml" + taskNumber).prop("checked");
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
