function ReplaceAllText(currentText, oldText, newText) {
    var regExp = new RegExp(oldText, "g");
    return currentText.replace(regExp, newText);
}
function GetNewTaskRowHtml(lastTaskRow, lastTaskNumber, newTaskNumber) {
    var taskRowHtml = ReplaceAllText(lastTaskRow.outerHTML, "mediaWorkflowTaskRow" + lastTaskNumber, "mediaWorkflowTaskRow" + newTaskNumber);
    taskRowHtml = ReplaceAllText(taskRowHtml, "Job Task #" + lastTaskNumber, "Job Task #" + newTaskNumber);
    taskRowHtml = ReplaceAllText(taskRowHtml, "mediaProcessor" + lastTaskNumber, "mediaProcessor" + newTaskNumber);
    taskRowHtml = ReplaceAllText(taskRowHtml, "taskParent" + lastTaskNumber, "taskParent" + newTaskNumber);
    taskRowHtml = ReplaceAllText(taskRowHtml, "encoderConfigRow" + lastTaskNumber, "encoderConfigRow" + newTaskNumber);
    taskRowHtml = ReplaceAllText(taskRowHtml, "encoderConfig" + lastTaskNumber, "encoderConfig" + newTaskNumber);
    taskRowHtml = ReplaceAllText(taskRowHtml, "encoderConfigFileRow" + lastTaskNumber, "encoderConfigFileRow" + newTaskNumber);
    taskRowHtml = ReplaceAllText(taskRowHtml, "encoderConfigFile" + lastTaskNumber, "encoderConfigFile" + newTaskNumber);
    taskRowHtml = ReplaceAllText(taskRowHtml, "encoderOptionsRow" + lastTaskNumber, "encoderOptionsRow" + newTaskNumber);
    taskRowHtml = ReplaceAllText(taskRowHtml, "encoderContentProtection" + lastTaskNumber, "encoderContentProtection" + newTaskNumber);
    taskRowHtml = ReplaceAllText(taskRowHtml, "encoderFragmentOutput" + lastTaskNumber, "encoderFragmentOutput" + newTaskNumber);
    taskRowHtml = ReplaceAllText(taskRowHtml, "encoderContentProtectionRow" + lastTaskNumber, "encoderContentProtectionRow" + newTaskNumber);
    taskRowHtml = ReplaceAllText(taskRowHtml, "encoderContentProtectionAes" + lastTaskNumber, "encoderContentProtectionAes" + newTaskNumber);
    taskRowHtml = ReplaceAllText(taskRowHtml, "encoderContentProtectionDrmPlayReady" + lastTaskNumber, "encoderContentProtectionDrmPlayReady" + newTaskNumber);
    taskRowHtml = ReplaceAllText(taskRowHtml, "encoderContentProtectionDrmWidevine" + lastTaskNumber, "encoderContentProtectionDrmWidevine" + newTaskNumber);
    taskRowHtml = ReplaceAllText(taskRowHtml, "encoderContentProtectionAuthTypeToken" + lastTaskNumber, "encoderContentProtectionAuthTypeToken" + newTaskNumber);
    taskRowHtml = ReplaceAllText(taskRowHtml, "encoderContentProtectionAuthTypeAddress" + lastTaskNumber, "encoderContentProtectionAuthTypeAddress" + newTaskNumber);
    taskRowHtml = ReplaceAllText(taskRowHtml, "encoderContentProtectionAuthAddressRange" + lastTaskNumber, "encoderContentProtectionAuthAddressRange" + newTaskNumber);
    taskRowHtml = ReplaceAllText(taskRowHtml, "indexerConfigRow" + lastTaskNumber, "indexerConfigRow" + newTaskNumber);
    taskRowHtml = ReplaceAllText(taskRowHtml, "indexerSpokenLanguages" + lastTaskNumber, "indexerSpokenLanguages" + newTaskNumber);
    taskRowHtml = ReplaceAllText(taskRowHtml, "indexerCaptionWebVtt" + lastTaskNumber, "indexerCaptionWebVtt" + newTaskNumber);
    taskRowHtml = ReplaceAllText(taskRowHtml, "indexerCaptionTtml" + lastTaskNumber, "indexerCaptionTtml" + newTaskNumber);
    taskRowHtml = ReplaceAllText(taskRowHtml, "faceDetectionConfigRow" + lastTaskNumber, "faceDetectionConfigRow" + newTaskNumber);
    taskRowHtml = ReplaceAllText(taskRowHtml, "faceDetectionMode" + lastTaskNumber, "faceDetectionMode" + newTaskNumber);
    taskRowHtml = ReplaceAllText(taskRowHtml, "faceRedactionConfigRow" + lastTaskNumber, "faceRedactionConfigRow" + newTaskNumber);
    taskRowHtml = ReplaceAllText(taskRowHtml, "faceRedactionMode" + lastTaskNumber, "faceRedactionMode" + newTaskNumber);
    taskRowHtml = ReplaceAllText(taskRowHtml, "motionConfigRow" + lastTaskNumber, "motionConfigRow" + newTaskNumber);
    taskRowHtml = ReplaceAllText(taskRowHtml, "motionSensitivityLevel" + lastTaskNumber, "motionSensitivityLevel" + newTaskNumber);
    taskRowHtml = ReplaceAllText(taskRowHtml, "motionDetectLightChange" + lastTaskNumber, "motionDetectLightChange" + newTaskNumber);
    taskRowHtml = ReplaceAllText(taskRowHtml, "hyperlapseConfigRow" + lastTaskNumber, "hyperlapseConfigRow" + newTaskNumber);
    taskRowHtml = ReplaceAllText(taskRowHtml, "hyperlapseStartFrame" + lastTaskNumber, "hyperlapseStartFrame" + newTaskNumber);
    taskRowHtml = ReplaceAllText(taskRowHtml, "hyperlapseFrameCount" + lastTaskNumber, "hyperlapseFrameCount" + newTaskNumber);
    taskRowHtml = ReplaceAllText(taskRowHtml, "hyperlapseSpeed" + lastTaskNumber, "hyperlapseSpeed" + newTaskNumber);
    taskRowHtml = ReplaceAllText(taskRowHtml, "summaryConfigRow" + lastTaskNumber, "summaryConfigRow" + newTaskNumber);
    taskRowHtml = ReplaceAllText(taskRowHtml, "summaryDurationSeconds" + lastTaskNumber, "summaryDurationSeconds" + newTaskNumber);
    taskRowHtml = ReplaceAllText(taskRowHtml, "outputAssetName" + lastTaskNumber, "outputAssetName" + newTaskNumber);
    taskRowHtml = ReplaceAllText(taskRowHtml, "taskOptions" + lastTaskNumber, "taskOptions" + newTaskNumber);
    taskRowHtml = ReplaceAllText(taskRowHtml, "this, " + lastTaskNumber, "this, " + newTaskNumber);
    return taskRowHtml;
}
function ResetProcessorConfig(encoderConfigOptions, encoderConfigFileId, indexerLanguageOptions, hideRowIds, unbindDocIds) {
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
    for (var i = 0; i < unbindDocIds.length; i++) {
        $("#" + unbindDocIds[i]).unbind("click");
    }
}
function SetProcessorConfig(mediaProcessor, taskNumber) {
    var encoderConfigRowId = mediaProcessor.id.replace("mediaProcessor", "encoderConfigRow");
    var encoderConfigDocId = mediaProcessor.id.replace("mediaProcessor", "encoderConfigDoc");
    var encoderConfigId = mediaProcessor.id.replace("mediaProcessor", "encoderConfig");
    var encoderConfigFileRowId = mediaProcessor.id.replace("mediaProcessor", "encoderConfigFileRow");
    var encoderConfigFileDocId = mediaProcessor.id.replace("mediaProcessor", "encoderConfigFileDoc");
    var encoderConfigFileId = mediaProcessor.id.replace("mediaProcessor", "encoderConfigFile");
    var encoderOptionsRowId = mediaProcessor.id.replace("mediaProcessor", "encoderOptionsRow");
    var encoderContentProtectionRowId = mediaProcessor.id.replace("mediaProcessor", "encoderContentProtectionRow");
    var indexerConfigRowId = mediaProcessor.id.replace("mediaProcessor", "indexerConfigRow");
    var indexerLanguagesId = mediaProcessor.id.replace("mediaProcessor", "indexerSpokenLanguages");
    var indexerLanguageOptions = $("#" + indexerLanguagesId)[0].options;
    var faceDetectionConfigRowId = mediaProcessor.id.replace("mediaProcessor", "faceDetectionConfigRow");
    var faceRedactionConfigRowId = mediaProcessor.id.replace("mediaProcessor", "faceRedactionConfigRow");
    var motionConfigRowId = mediaProcessor.id.replace("mediaProcessor", "motionConfigRow");
    var hyperlapseConfigRowId = mediaProcessor.id.replace("mediaProcessor", "hyperlapseConfigRow");
    var summaryConfigRowId = mediaProcessor.id.replace("mediaProcessor", "summaryConfigRow");
    var encoderConfigOptions = $("#" + encoderConfigId)[0].options;
    var hideRowIds = [encoderConfigRowId, encoderConfigFileRowId, encoderOptionsRowId, encoderContentProtectionRowId, indexerConfigRowId, faceDetectionConfigRowId, faceRedactionConfigRowId, motionConfigRowId, hyperlapseConfigRowId, summaryConfigRowId];
    var unbindDocIds = [encoderConfigDocId, encoderConfigFileDocId];
    ResetProcessorConfig(encoderConfigOptions, encoderConfigFileId, indexerLanguageOptions, hideRowIds, unbindDocIds);
    if (mediaProcessor.value != "None") {
        switch (mediaProcessor.value) {
            case "EncoderStandard":
                $("#" + encoderConfigDocId).click(function () {
                    window.open("http://docs.microsoft.com/en-us/azure/media-services/media-services-mes-presets-overview");
                });
                encoderConfigOptions[encoderConfigOptions.length] = new Option("H.264 MBR Adaptive Streaming Bitrate Ladder", "Adaptive Streaming");
                encoderConfigOptions[encoderConfigOptions.length] = new Option("H.264 MBR 4K (12 levels, 1000 - 20000 kbps) AAC 5.1", "H264 Multiple Bitrate 4K Audio 5.1");
                encoderConfigOptions[encoderConfigOptions.length] = new Option("H.264 MBR 4K (12 levels, 1000 - 20000 kbps) AAC", "H264 Multiple Bitrate 4K");
                encoderConfigOptions[encoderConfigOptions.length] = new Option("H.264 MBR 1080p (8 levels, 400 - 6000 kbps) AAC 5.1", "H264 Multiple Bitrate 1080p Audio 5.1");
                encoderConfigOptions[encoderConfigOptions.length] = new Option("H.264 MBR 1080p (8 levels, 400 - 6000 kbps) AAC", "H264 Multiple Bitrate 1080p");
                encoderConfigOptions[encoderConfigOptions.length] = new Option("H.264 MBR 720p (6 levels, 400 - 3400 kbps) AAC 5.1", "H264 Multiple Bitrate 720p Audio 5.1");
                encoderConfigOptions[encoderConfigOptions.length] = new Option("H.264 MBR 720p (6 levels, 400 - 3400 kbps) AAC", "H264 Multiple Bitrate 720p");
                encoderConfigOptions[encoderConfigOptions.length] = new Option("H.264 MBR 16x9 for iOS (8 levels, 200 - 8500 kbps) AAC", "H264 Multiple Bitrate 16x9 for iOS");
                encoderConfigOptions[encoderConfigOptions.length] = new Option("H.264 MBR 16x9 SD (5 levels, 400 - 1900 kbps) AAC 5.1", "H264 Multiple Bitrate 16x9 SD Audio 5.1");
                encoderConfigOptions[encoderConfigOptions.length] = new Option("H.264 MBR 16x9 SD (5 levels, 400 - 1900 kbps) AAC", "H264 Multiple Bitrate 16x9 SD");
                encoderConfigOptions[encoderConfigOptions.length] = new Option("H.264 MBR 4x3 for iOS (8 levels, 200 - 8500 kbps) AAC", "H264 Multiple Bitrate 4x3 for iOS");
                encoderConfigOptions[encoderConfigOptions.length] = new Option("H.264 MBR 4x3 SD (5 levels, 400 - 1600 kbps) AAC 5.1", "H264 Multiple Bitrate 4x3 SD Audio 5.1");
                encoderConfigOptions[encoderConfigOptions.length] = new Option("H.264 MBR 4x3 SD (5 levels, 400 - 1600 kbps) AAC", "H264 Multiple Bitrate 4x3 SD");
                encoderConfigOptions[encoderConfigOptions.length] = new Option("H.264 SBR 4K (18000 kbps) AAC 5.1", "H264 Single Bitrate 4K Audio 5.1");
                encoderConfigOptions[encoderConfigOptions.length] = new Option("H.264 SBR 4K (18000 kbps) AAC", "H264 Single Bitrate 4K");
                encoderConfigOptions[encoderConfigOptions.length] = new Option("H.264 SBR 1080p (6750 kbps) AAC 5.1", "H264 Single Bitrate 1080p Audio 5.1");
                encoderConfigOptions[encoderConfigOptions.length] = new Option("H.264 SBR 1080p (6750 kbps) AAC", "H264 Single Bitrate 1080p");
                encoderConfigOptions[encoderConfigOptions.length] = new Option("H.264 SBR 720p (4500 kbps) AAC 5.1", "H264 Single Bitrate 720p Audio 5.1");
                encoderConfigOptions[encoderConfigOptions.length] = new Option("H.264 SBR 720p (4500 kbps) AAC", "H264 Single Bitrate 720p");
                encoderConfigOptions[encoderConfigOptions.length] = new Option("H.264 SBR 720p for Android (2000 kbps) AAC", "H264 Single Bitrate 720p for Android");
                encoderConfigOptions[encoderConfigOptions.length] = new Option("H.264 SBR 16x9 SD (2200 kbps) AAC 5.1", "H264 Single Bitrate 16x9 SD Audio 5.1");
                encoderConfigOptions[encoderConfigOptions.length] = new Option("H.264 SBR 16x9 SD (2200 kbps) AAC", "H264 Single Bitrate 16x9 SD");
                encoderConfigOptions[encoderConfigOptions.length] = new Option("H.264 SBR 4x3 SD (1600 kbps) AAC 5.1", "H264 Single Bitrate 4x3 SD Audio 5.1");
                encoderConfigOptions[encoderConfigOptions.length] = new Option("H.264 SBR 4x3 SD (1600 kbps) AAC", "H264 Single Bitrate 4x3 SD");
                encoderConfigOptions[encoderConfigOptions.length] = new Option("H.264 SBR High Quality SD for Android (500 kbps) AAC", "H264 Single Bitrate High Quality SD for Android");
                encoderConfigOptions[encoderConfigOptions.length] = new Option("H.264 SBR Low Quality SD for Android (56 kbps) AAC", "H264 Single Bitrate Low Quality SD for Android");
                encoderConfigOptions[encoderConfigOptions.length] = new Option("Thumbnails (PNG Format, 5% Intervals, 640 x 360 px)", "Thumbnails");
                encoderConfigOptions[encoderConfigOptions.length] = new Option("Custom Configuration File (JSON)", "Custom");
                $("#" + encoderOptionsRowId).show();
                break;
            case "EncoderPremium":
            case "EncoderUltra":
                $("#" + encoderConfigDocId).click(function () {
                    window.open("http://docs.microsoft.com/en-us/azure/media-services/media-services-encode-with-premium-workflow");
                });
                encoderConfigOptions[encoderConfigOptions.length] = new Option("", "None");
                encoderConfigOptions[encoderConfigOptions.length] = new Option("Custom Configuration File (XML)", "Custom");
                $("#" + encoderOptionsRowId).show();
                break;
            case "Indexer":
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
                SetJobTaskWidgets(taskNumber);
                $("#" + indexerConfigRowId).show();
                break;
            case "FaceDetection":
                $("#" + faceDetectionConfigRowId).show();
                break;
            case "FaceRedaction":
                $("#" + faceRedactionConfigRowId).show();
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
    EnsureVisibility();
}
function SetEncoderConfigOptions(encoderConfig) {
    var mediaProcessorId = encoderConfig.id.replace("encoderConfig", "mediaProcessor");
    var encoderConfigFileRowId = encoderConfig.id.replace("encoderConfig", "encoderConfigFileRow");
    var encoderConfigFileDocId = encoderConfig.id.replace("encoderConfig", "encoderConfigFileDoc");
    if (encoderConfig.value == "Custom") {
        $("#" + encoderConfigFileRowId).show();
        switch ($("#" + mediaProcessorId).val()) {
            case "EncoderStandard":
                $("#" + encoderConfigFileDocId).click(function () {
                    window.open("http://docs.microsoft.com/en-us/azure/media-services/media-services-mes-schema");
                });
                break;
            case "EncoderPremium":
            case "EncoderUltra":
                $("#" + encoderConfigFileDocId).click(function () {
                    window.open("http://docs.microsoft.com/en-us/azure/media-services/media-services-media-encoder-premium-workflow-multiplefilesinput");
                });
                break;
        }
    } else {
        $("#" + encoderConfigFileRowId).hide();
    }
    EnsureVisibility();
}
