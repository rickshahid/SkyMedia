function ReplaceAllText(currentText, oldText, newText) {
    var regExp = new RegExp(oldText, "g");
    return currentText.replace(regExp, newText);
}
function GetNewTaskRowHtml(lastTaskRow, lastTaskNumber, newTaskNumber) {
    var taskRowHtml = ReplaceAllText(lastTaskRow.outerHTML, "taskRow" + lastTaskNumber, "taskRow" + newTaskNumber);
    taskRowHtml = ReplaceAllText(taskRowHtml, ">#" + lastTaskNumber, ">#" + newTaskNumber);
    taskRowHtml = ReplaceAllText(taskRowHtml, "mediaProcessor" + lastTaskNumber, "mediaProcessor" + newTaskNumber);
    taskRowHtml = ReplaceAllText(taskRowHtml, "taskParent" + lastTaskNumber, "taskParent" + newTaskNumber);
    taskRowHtml = ReplaceAllText(taskRowHtml, "encoderConfigRow" + lastTaskNumber, "encoderConfigRow" + newTaskNumber);
    taskRowHtml = ReplaceAllText(taskRowHtml, "encoderConfigDoc" + lastTaskNumber, "encoderConfigDoc" + newTaskNumber);
    taskRowHtml = ReplaceAllText(taskRowHtml, "encoderConfigDocSchema" + lastTaskNumber, "encoderConfigDocSchema" + newTaskNumber);
    taskRowHtml = ReplaceAllText(taskRowHtml, "encoderConfig" + lastTaskNumber, "encoderConfig" + newTaskNumber);
    taskRowHtml = ReplaceAllText(taskRowHtml, "encoderConfigFileRow" + lastTaskNumber, "encoderConfigFileRow" + newTaskNumber);
    taskRowHtml = ReplaceAllText(taskRowHtml, "encoderConfigFile" + lastTaskNumber, "encoderConfigFile" + newTaskNumber);
    taskRowHtml = ReplaceAllText(taskRowHtml, "encoderOptionsRow" + lastTaskNumber, "encoderOptionsRow" + newTaskNumber);
    taskRowHtml = ReplaceAllText(taskRowHtml, "encoderFragmentOutput" + lastTaskNumber, "encoderFragmentOutput" + newTaskNumber);
    taskRowHtml = ReplaceAllText(taskRowHtml, "encoderContentProtection" + lastTaskNumber, "encoderContentProtection" + newTaskNumber);
    taskRowHtml = ReplaceAllText(taskRowHtml, "encoderContentProtectionRow" + lastTaskNumber, "encoderContentProtectionRow" + newTaskNumber);
    taskRowHtml = ReplaceAllText(taskRowHtml, "encoderContentProtectionAes" + lastTaskNumber, "encoderContentProtectionAes" + newTaskNumber);
    taskRowHtml = ReplaceAllText(taskRowHtml, "encoderContentProtectionDrmPlayReady" + lastTaskNumber, "encoderContentProtectionDrmPlayReady" + newTaskNumber);
    taskRowHtml = ReplaceAllText(taskRowHtml, "encoderContentProtectionDrmWidevine" + lastTaskNumber, "encoderContentProtectionDrmWidevine" + newTaskNumber);
    taskRowHtml = ReplaceAllText(taskRowHtml, "encoderContentProtectionDrmFairPlay" + lastTaskNumber, "encoderContentProtectionDrmFairPlay" + newTaskNumber);
    taskRowHtml = ReplaceAllText(taskRowHtml, "encoderContentProtectionAuthTypeToken" + lastTaskNumber, "encoderContentProtectionAuthTypeToken" + newTaskNumber);
    taskRowHtml = ReplaceAllText(taskRowHtml, "encoderContentProtectionAuthTypeAddress" + lastTaskNumber, "encoderContentProtectionAuthTypeAddress" + newTaskNumber);
    taskRowHtml = ReplaceAllText(taskRowHtml, "encoderContentProtectionAuthAddressRange" + lastTaskNumber, "encoderContentProtectionAuthAddressRange" + newTaskNumber);
    taskRowHtml = ReplaceAllText(taskRowHtml, "indexerRow" + lastTaskNumber, "indexerRow" + newTaskNumber);
    taskRowHtml = ReplaceAllText(taskRowHtml, "indexerLanguageId" + lastTaskNumber, "indexerLanguageId" + newTaskNumber);
    taskRowHtml = ReplaceAllText(taskRowHtml, "indexerSearchPartition" + lastTaskNumber, "indexerSearchPartition" + newTaskNumber);
    taskRowHtml = ReplaceAllText(taskRowHtml, "indexerVideoDescription" + lastTaskNumber, "indexerVideoDescription" + newTaskNumber);
    taskRowHtml = ReplaceAllText(taskRowHtml, "indexerVideoMetadata" + lastTaskNumber, "indexerVideoMetadata" + newTaskNumber);
    taskRowHtml = ReplaceAllText(taskRowHtml, "indexerVideoPublic" + lastTaskNumber, "indexerVideoPublic" + newTaskNumber);
    taskRowHtml = ReplaceAllText(taskRowHtml, "indexerAudioOnly" + lastTaskNumber, "indexerAudioOnly" + newTaskNumber);
    taskRowHtml = ReplaceAllText(taskRowHtml, "summarizationRow" + lastTaskNumber, "summarizationRow" + newTaskNumber);
    taskRowHtml = ReplaceAllText(taskRowHtml, "summarizationDurationMinutes" + lastTaskNumber, "summarizationDurationMinutes" + newTaskNumber);
    taskRowHtml = ReplaceAllText(taskRowHtml, "summarizationDurationSeconds" + lastTaskNumber, "summarizationDurationSeconds" + newTaskNumber);
    taskRowHtml = ReplaceAllText(taskRowHtml, "summarizationFadeTransitions" + lastTaskNumber, "summarizationFadeTransitions" + newTaskNumber);
    taskRowHtml = ReplaceAllText(taskRowHtml, "summarizationIncludeAudio" + lastTaskNumber, "summarizationIncludeAudio" + newTaskNumber);
    taskRowHtml = ReplaceAllText(taskRowHtml, "faceDetectionRow" + lastTaskNumber, "faceDetectionRow" + newTaskNumber);
    taskRowHtml = ReplaceAllText(taskRowHtml, "faceDetectionMode" + lastTaskNumber, "faceDetectionMode" + newTaskNumber);
    taskRowHtml = ReplaceAllText(taskRowHtml, "faceRedactionRow" + lastTaskNumber, "faceRedactionRow" + newTaskNumber);
    taskRowHtml = ReplaceAllText(taskRowHtml, "faceRedactionBlurMode" + lastTaskNumber, "faceRedactionBlurMode" + newTaskNumber);
    taskRowHtml = ReplaceAllText(taskRowHtml, "faceEmotionRow" + lastTaskNumber, "faceEmotionRow" + newTaskNumber);
    taskRowHtml = ReplaceAllText(taskRowHtml, "faceEmotionMode" + lastTaskNumber, "faceEmotionMode" + newTaskNumber);
    taskRowHtml = ReplaceAllText(taskRowHtml, "faceEmotionAggregateRow" + lastTaskNumber, "faceEmotionAggregateRow" + newTaskNumber);
    taskRowHtml = ReplaceAllText(taskRowHtml, "faceEmotionAggregateWindow" + lastTaskNumber, "faceEmotionAggregateWindow" + newTaskNumber);
    taskRowHtml = ReplaceAllText(taskRowHtml, "faceEmotionAggregateInterval" + lastTaskNumber, "faceEmotionAggregateInterval" + newTaskNumber);
    taskRowHtml = ReplaceAllText(taskRowHtml, "speechAnalyzerRow" + lastTaskNumber, "speechAnalyzerRow" + newTaskNumber);
    taskRowHtml = ReplaceAllText(taskRowHtml, "speechAnalyzerLanguageId" + lastTaskNumber, "speechAnalyzerLanguageId" + newTaskNumber);
    taskRowHtml = ReplaceAllText(taskRowHtml, "speechAnalyzerTimedTextFormatTtml" + lastTaskNumber, "speechAnalyzerTimedTextFormatTtml" + newTaskNumber);
    taskRowHtml = ReplaceAllText(taskRowHtml, "speechAnalyzerTimedTextFormatWebVtt" + lastTaskNumber, "speechAnalyzerTimedTextFormatWebVtt" + newTaskNumber);
    taskRowHtml = ReplaceAllText(taskRowHtml, "motionDetectionRow" + lastTaskNumber, "motionDetectionRow" + newTaskNumber);
    taskRowHtml = ReplaceAllText(taskRowHtml, "motionDetectionSensitivityLevel" + lastTaskNumber, "motionDetectionSensitivityLevel" + newTaskNumber);
    taskRowHtml = ReplaceAllText(taskRowHtml, "motionDetectionLightChange" + lastTaskNumber, "motionDetectionLightChange" + newTaskNumber);
    taskRowHtml = ReplaceAllText(taskRowHtml, "outputAssetName" + lastTaskNumber, "outputAssetName" + newTaskNumber);
    taskRowHtml = ReplaceAllText(taskRowHtml, "taskOptions" + lastTaskNumber, "taskOptions" + newTaskNumber);
    taskRowHtml = ReplaceAllText(taskRowHtml, "this, " + lastTaskNumber, "this, " + newTaskNumber);
    return taskRowHtml;
}
function ResetProcessorConfig(encoderConfigOptions, encoderConfigFileId, hideIds, unbindIds) {
    if (encoderConfigOptions != null) {
        encoderConfigOptions.length = 0;
    }
    $("#" + encoderConfigFileId).val("");
    for (var i = 0; i < hideIds.length; i++) {
        var hideItem = $("#" + hideIds[i]);
        if (hideItem != null) {
            hideItem.hide();
        }
    }
    for (var i = 0; i < unbindIds.length; i++) {
        var unbindItem = $("#" + unbindIds[i]);
        if (unbindItem != null) {
            unbindItem.unbind("click");
        }
    }
}
function SetProcessorConfig(mediaProcessor, taskNumber, encoderStandardPresets, encoderPremiumPresets) {
    var encoderConfigRowId = mediaProcessor.id.replace("mediaProcessor", "encoderConfigRow");
    var encoderConfigDocId = mediaProcessor.id.replace("mediaProcessor", "encoderConfigDoc");
    var encoderConfigDocSchemaId = mediaProcessor.id.replace("mediaProcessor", "encoderConfigDocSchema");
    var encoderConfigId = mediaProcessor.id.replace("mediaProcessor", "encoderConfig");
    var encoderConfigFileRowId = mediaProcessor.id.replace("mediaProcessor", "encoderConfigFileRow");
    var encoderConfigFileId = mediaProcessor.id.replace("mediaProcessor", "encoderConfigFile");
    var encoderOptionsRowId = mediaProcessor.id.replace("mediaProcessor", "encoderOptionsRow");
    var encoderContentProtectionRowId = mediaProcessor.id.replace("mediaProcessor", "encoderContentProtectionRow");
    var indexerRowId = mediaProcessor.id.replace("mediaProcessor", "indexerRow");
    var annotationRowId = mediaProcessor.id.replace("mediaProcessor", "annotationRow");
    var summarizationRowId = mediaProcessor.id.replace("mediaProcessor", "summarizationRow");
    var faceDetectionRowId = mediaProcessor.id.replace("mediaProcessor", "faceDetectionRow");
    var faceDetectionModeId = mediaProcessor.id.replace("mediaProcessor", "faceDetectionMode");
    var speechAnalyzerRowId = mediaProcessor.id.replace("mediaProcessor", "speechAnalyzerRow");
    var motionDetectionRowId = mediaProcessor.id.replace("mediaProcessor", "motionDetectionRow");
    var contentModerationRowId = mediaProcessor.id.replace("mediaProcessor", "contentModerationRow");
    var characterRecognitionRowId = mediaProcessor.id.replace("mediaProcessor", "characterRecognitionRow");
    var encoderConfigOptions = $("#" + encoderConfigId)[0].options;
    var hideIds = [encoderConfigRowId, encoderConfigDocSchemaId, encoderConfigFileRowId, encoderOptionsRowId, encoderContentProtectionRowId, indexerRowId, annotationRowId, summarizationRowId, faceDetectionRowId, speechAnalyzerRowId, motionDetectionRowId, contentModerationRowId, characterRecognitionRowId];
    var unbindIds = [encoderConfigDocId, encoderConfigDocSchemaId];
    ResetProcessorConfig(encoderConfigOptions, encoderConfigFileId, hideIds, unbindIds);
    switch (mediaProcessor.value) {
        case "EncoderStandard":
            $("#" + encoderConfigDocId).click(function () {
                window.open("http://docs.microsoft.com/azure/media-services/media-services-mes-presets-overview");
            });
            SetEncoderPresetOptions(encoderConfigOptions, encoderStandardPresets);
            encoderConfigOptions[encoderConfigOptions.length] = new Option("Custom Configuration File (JSON)", "Custom");
            $("#" + encoderOptionsRowId).show();
            break;
        case "EncoderPremium":
            $("#" + encoderConfigDocId).click(function () {
                window.open("http://docs.microsoft.com/azure/media-services/media-services-encode-with-premium-workflow");
            });
            SetEncoderPresetOptions(encoderConfigOptions, encoderPremiumPresets);
            encoderConfigOptions[encoderConfigOptions.length] = new Option("Custom Configuration File (XML)", "Custom");
            $("#" + encoderOptionsRowId).show();
            break;
        case "VideoIndexer":
            $("#" + indexerRowId).show();
            break;
        case "VideoAnnotation":
            $("#" + annotationRowId).show();
            break;
        case "VideoSummarization":
            $("#" + summarizationRowId).show();
            break;
        case "FaceDetection":
            $("#" + faceDetectionRowId).show();
            $("#" + faceDetectionModeId).change();
            break;
        case "SpeechAnalyzer":
            $("#" + speechAnalyzerRowId).show();
            break;
        case "MotionDetection":
            $("#" + motionDetectionRowId).show();
            break;
        case "ContentModeration":
            $("#" + contentModerationRowId).show();
            break;
        case "CharacterRecognition":
            $("#" + characterRecognitionRowId).show();
            break;
    }
    if (encoderConfigOptions.length > 0) {
        $("#" + encoderConfigRowId).show();
        $("#" + encoderConfigId).change();
    }
    ScrollToBottom();
}
function SetEncoderPresetOptions(encoderConfigOptions, encoderPresets) {
    for (var i = 0; i < encoderPresets.length; i++) {
        var encoderPreset = encoderPresets[i];
        encoderConfigOptions[encoderConfigOptions.length] = new Option(encoderPreset.text, encoderPreset.value);
        encoderConfigOptions[encoderConfigOptions.length - 1].selected = encoderPreset.selected;
    }
}
function SetEncoderConfigOptions(encoderConfig) {
    var encoderConfigDocSchemaId = encoderConfig.id.replace("encoderConfig", "encoderConfigDocSchema");
    var encoderConfigFileRowId = encoderConfig.id.replace("encoderConfig", "encoderConfigFileRow");
    var mediaProcessorId = encoderConfig.id.replace("encoderConfig", "mediaProcessor");
    $("#" + encoderConfigDocSchemaId).hide();
    $("#" + encoderConfigFileRowId).hide();
    switch (encoderConfig.value) {
        case "Custom":
            $("#" + encoderConfigDocSchemaId).show();
            $("#" + encoderConfigFileRowId).show();
            switch ($("#" + mediaProcessorId).val()) {
                case "EncoderStandard":
                    $("#" + encoderConfigDocSchemaId).click(function () {
                        window.open("http://docs.microsoft.com/azure/media-services/media-services-mes-schema");
                    });
                    break;
                case "EncoderPremium":
                    $("#" + encoderConfigDocSchemaId).click(function () {
                        window.open("http://docs.microsoft.com/azure/media-services/media-services-workflow-designer");
                    });
                    break;
            }
            break;
    }
    ScrollToBottom();
}