function ReplaceAllText(currentText, oldText, newText) {
    var regExp = new RegExp(oldText, "g");
    return currentText.replace(regExp, newText);
}
function GetNewTaskRowHtml(lastTaskRow, lastTaskNumber, newTaskNumber) {
    var taskRowHtml = ReplaceAllText(lastTaskRow.outerHTML, "taskRow" + lastTaskNumber, "taskRow" + newTaskNumber);
    taskRowHtml = ReplaceAllText(taskRowHtml, ">#" + lastTaskNumber, ">#" + newTaskNumber);
    taskRowHtml = ReplaceAllText(taskRowHtml, "this, " + lastTaskNumber, "this, " + newTaskNumber);
    taskRowHtml = ReplaceAllText(taskRowHtml, "taskParent" + lastTaskNumber, "taskParent" + newTaskNumber);
    taskRowHtml = ReplaceAllText(taskRowHtml, "mediaProcessor" + lastTaskNumber, "mediaProcessor" + newTaskNumber);
    taskRowHtml = ReplaceAllText(taskRowHtml, "encoderConfigRow" + lastTaskNumber, "encoderConfigRow" + newTaskNumber);
    taskRowHtml = ReplaceAllText(taskRowHtml, "encoderConfigDoc" + lastTaskNumber, "encoderConfigDoc" + newTaskNumber);
    taskRowHtml = ReplaceAllText(taskRowHtml, "encoderConfigDocSchema" + lastTaskNumber, "encoderConfigDocSchema" + newTaskNumber);
    taskRowHtml = ReplaceAllText(taskRowHtml, "encoderConfig" + lastTaskNumber, "encoderConfig" + newTaskNumber);
    taskRowHtml = ReplaceAllText(taskRowHtml, "encoderConfigFileRow" + lastTaskNumber, "encoderConfigFileRow" + newTaskNumber);
    taskRowHtml = ReplaceAllText(taskRowHtml, "encoderConfigFile" + lastTaskNumber, "encoderConfigFile" + newTaskNumber);
    taskRowHtml = ReplaceAllText(taskRowHtml, "encoderOptionsRow" + lastTaskNumber, "encoderOptionsRow" + newTaskNumber);
    taskRowHtml = ReplaceAllText(taskRowHtml, "encoderThumbnailGeneration" + lastTaskNumber, "encoderThumbnailGeneration" + newTaskNumber);
    taskRowHtml = ReplaceAllText(taskRowHtml, "encoderThumbnailGenerationRow" + lastTaskNumber, "encoderThumbnailGenerationRow" + newTaskNumber);
    taskRowHtml = ReplaceAllText(taskRowHtml, "encoderThumbnailGenerationFormat" + lastTaskNumber, "encoderThumbnailGenerationFormat" + newTaskNumber);
    taskRowHtml = ReplaceAllText(taskRowHtml, "encoderThumbnailGenerationBest" + lastTaskNumber, "encoderThumbnailGenerationBest" + newTaskNumber);
    taskRowHtml = ReplaceAllText(taskRowHtml, "encoderThumbnailGenerationSingle" + lastTaskNumber, "encoderThumbnailGenerationSingle" + newTaskNumber);
    taskRowHtml = ReplaceAllText(taskRowHtml, "encoderThumbnailGenerationSprite" + lastTaskNumber, "encoderThumbnailGenerationSprite" + newTaskNumber);
    taskRowHtml = ReplaceAllText(taskRowHtml, "encoderThumbnailGenerationSpriteColumns" + lastTaskNumber, "encoderThumbnailGenerationSpriteColumns" + newTaskNumber);
    taskRowHtml = ReplaceAllText(taskRowHtml, "encoderThumbnailGenerationWidthPercent" + lastTaskNumber, "encoderThumbnailGenerationWidthPercent" + newTaskNumber);
    taskRowHtml = ReplaceAllText(taskRowHtml, "encoderThumbnailGenerationWidthPixel" + lastTaskNumber, "encoderThumbnailGenerationWidthPixel" + newTaskNumber);
    taskRowHtml = ReplaceAllText(taskRowHtml, "encoderThumbnailGenerationHeightPercent" + lastTaskNumber, "encoderThumbnailGenerationHeightPercent" + newTaskNumber);
    taskRowHtml = ReplaceAllText(taskRowHtml, "encoderThumbnailGenerationHeightPixel" + lastTaskNumber, "encoderThumbnailGenerationHeightPixel" + newTaskNumber);
    taskRowHtml = ReplaceAllText(taskRowHtml, "encoderThumbnailGenerationStartPercent" + lastTaskNumber, "encoderThumbnailGenerationStartPercent" + newTaskNumber);
    taskRowHtml = ReplaceAllText(taskRowHtml, "encoderThumbnailGenerationStartHour" + lastTaskNumber, "encoderThumbnailGenerationStartHour" + newTaskNumber);
    taskRowHtml = ReplaceAllText(taskRowHtml, "encoderThumbnailGenerationStartMinute" + lastTaskNumber, "encoderThumbnailGenerationStartMinute" + newTaskNumber);
    taskRowHtml = ReplaceAllText(taskRowHtml, "encoderThumbnailGenerationStartSecond" + lastTaskNumber, "encoderThumbnailGenerationStartSecond" + newTaskNumber);
    taskRowHtml = ReplaceAllText(taskRowHtml, "encoderThumbnailGenerationStepPercent" + lastTaskNumber, "encoderThumbnailGenerationStepPercent" + newTaskNumber);
    taskRowHtml = ReplaceAllText(taskRowHtml, "encoderThumbnailGenerationStepHour" + lastTaskNumber, "encoderThumbnailGenerationStepHour" + newTaskNumber);
    taskRowHtml = ReplaceAllText(taskRowHtml, "encoderThumbnailGenerationStepMinute" + lastTaskNumber, "encoderThumbnailGenerationStepMinute" + newTaskNumber);
    taskRowHtml = ReplaceAllText(taskRowHtml, "encoderThumbnailGenerationStepSecond" + lastTaskNumber, "encoderThumbnailGenerationStepSecond" + newTaskNumber);
    taskRowHtml = ReplaceAllText(taskRowHtml, "encoderThumbnailGenerationRangePercent" + lastTaskNumber, "encoderThumbnailGenerationRangePercent" + newTaskNumber);
    taskRowHtml = ReplaceAllText(taskRowHtml, "encoderThumbnailGenerationRangeHour" + lastTaskNumber, "encoderThumbnailGenerationRangeHour" + newTaskNumber);
    taskRowHtml = ReplaceAllText(taskRowHtml, "encoderThumbnailGenerationRangeMinute" + lastTaskNumber, "encoderThumbnailGenerationRangeMinute" + newTaskNumber);
    taskRowHtml = ReplaceAllText(taskRowHtml, "encoderThumbnailGenerationRangeSecond" + lastTaskNumber, "encoderThumbnailGenerationRangeSecond" + newTaskNumber);
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
    taskRowHtml = ReplaceAllText(taskRowHtml, "encoderVideoOverlay" + lastTaskNumber, "encoderVideoOverlay" + newTaskNumber);
    taskRowHtml = ReplaceAllText(taskRowHtml, "audioAnalyzerRow" + lastTaskNumber, "audioAnalyzerRow" + newTaskNumber);
    taskRowHtml = ReplaceAllText(taskRowHtml, "audioAnalyzerLanguageId" + lastTaskNumber, "audioAnalyzerLanguageId" + newTaskNumber);
    taskRowHtml = ReplaceAllText(taskRowHtml, "audioAnalyzerTimedTextFormatTtml" + lastTaskNumber, "audioAnalyzerTimedTextFormatTtml" + newTaskNumber);
    taskRowHtml = ReplaceAllText(taskRowHtml, "audioAnalyzerTimedTextFormatWebVtt" + lastTaskNumber, "audioAnalyzerTimedTextFormatWebVtt" + newTaskNumber);
    taskRowHtml = ReplaceAllText(taskRowHtml, "videoAnalyzerRow" + lastTaskNumber, "videoAnalyzerRow" + newTaskNumber);
    taskRowHtml = ReplaceAllText(taskRowHtml, "videoAnalyzerLanguageId" + lastTaskNumber, "videoAnalyzerLanguageId" + newTaskNumber);
    taskRowHtml = ReplaceAllText(taskRowHtml, "videoAnalyzerSearchPartition" + lastTaskNumber, "videoAnalyzerSearchPartition" + newTaskNumber);
    taskRowHtml = ReplaceAllText(taskRowHtml, "videoAnalyzerVideoDescription" + lastTaskNumber, "videoAnalyzerVideoDescription" + newTaskNumber);
    taskRowHtml = ReplaceAllText(taskRowHtml, "videoAnalyzerVideoMetadata" + lastTaskNumber, "videoAnalyzerVideoMetadata" + newTaskNumber);
    taskRowHtml = ReplaceAllText(taskRowHtml, "videoAnalyzerVideoPublic" + lastTaskNumber, "videoAnalyzerVideoPublic" + newTaskNumber);
    taskRowHtml = ReplaceAllText(taskRowHtml, "videoAnalyzerAudioOnly" + lastTaskNumber, "videoAnalyzerAudioOnly" + newTaskNumber);
    taskRowHtml = ReplaceAllText(taskRowHtml, "videoSummarizationRow" + lastTaskNumber, "videoSummarizationRow" + newTaskNumber);
    taskRowHtml = ReplaceAllText(taskRowHtml, "videoSummarizationDurationMinutes" + lastTaskNumber, "videoSummarizationDurationMinutes" + newTaskNumber);
    taskRowHtml = ReplaceAllText(taskRowHtml, "videoSummarizationDurationSeconds" + lastTaskNumber, "videoSummarizationDurationSeconds" + newTaskNumber);
    taskRowHtml = ReplaceAllText(taskRowHtml, "videoSummarizationFadeTransitions" + lastTaskNumber, "videoSummarizationFadeTransitions" + newTaskNumber);
    taskRowHtml = ReplaceAllText(taskRowHtml, "videoSummarizationIncludeAudio" + lastTaskNumber, "videoSummarizationIncludeAudio" + newTaskNumber);
    taskRowHtml = ReplaceAllText(taskRowHtml, "faceDetectionRow" + lastTaskNumber, "faceDetectionRow" + newTaskNumber);
    taskRowHtml = ReplaceAllText(taskRowHtml, "faceDetectionMode" + lastTaskNumber, "faceDetectionMode" + newTaskNumber);
    taskRowHtml = ReplaceAllText(taskRowHtml, "faceRedactionRow" + lastTaskNumber, "faceRedactionRow" + newTaskNumber);
    taskRowHtml = ReplaceAllText(taskRowHtml, "faceRedactionBlurMode" + lastTaskNumber, "faceRedactionBlurMode" + newTaskNumber);
    taskRowHtml = ReplaceAllText(taskRowHtml, "faceEmotionRow" + lastTaskNumber, "faceEmotionRow" + newTaskNumber);
    taskRowHtml = ReplaceAllText(taskRowHtml, "faceEmotionMode" + lastTaskNumber, "faceEmotionMode" + newTaskNumber);
    taskRowHtml = ReplaceAllText(taskRowHtml, "faceEmotionAggregateRow" + lastTaskNumber, "faceEmotionAggregateRow" + newTaskNumber);
    taskRowHtml = ReplaceAllText(taskRowHtml, "faceEmotionAggregateWindow" + lastTaskNumber, "faceEmotionAggregateWindow" + newTaskNumber);
    taskRowHtml = ReplaceAllText(taskRowHtml, "faceEmotionAggregateInterval" + lastTaskNumber, "faceEmotionAggregateInterval" + newTaskNumber);
    taskRowHtml = ReplaceAllText(taskRowHtml, "motionDetectionRow" + lastTaskNumber, "motionDetectionRow" + newTaskNumber);
    taskRowHtml = ReplaceAllText(taskRowHtml, "motionDetectionSensitivityLevel" + lastTaskNumber, "motionDetectionSensitivityLevel" + newTaskNumber);
    taskRowHtml = ReplaceAllText(taskRowHtml, "motionDetectionLightChange" + lastTaskNumber, "motionDetectionLightChange" + newTaskNumber);
    taskRowHtml = ReplaceAllText(taskRowHtml, "outputAssetName" + lastTaskNumber, "outputAssetName" + newTaskNumber);
    taskRowHtml = ReplaceAllText(taskRowHtml, "taskOptions" + lastTaskNumber, "taskOptions" + newTaskNumber);
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
function ResetEncoderOptions(mediaProcessor) {
    var encoderThumbnailGenerationId = mediaProcessor.id.replace("mediaProcessor", "encoderThumbnailGeneration");
    var encoderContentProtectionId = mediaProcessor.id.replace("mediaProcessor", "encoderContentProtection");
    var encoderFragmentOutputId = mediaProcessor.id.replace("mediaProcessor", "encoderFragmentOutput");
    $("#" + encoderThumbnailGenerationId).prop("checked", false);
    $("#" + encoderContentProtectionId).prop("checked", false);
    $("#" + encoderFragmentOutputId).prop("checked", false);
}
function SetProcessorConfig(mediaProcessor, taskNumber, encoderStandardPresets, encoderPremiumPresets) {
    var encoderConfigRowId = mediaProcessor.id.replace("mediaProcessor", "encoderConfigRow");
    var encoderConfigDocId = mediaProcessor.id.replace("mediaProcessor", "encoderConfigDoc");
    var encoderConfigDocSchemaId = mediaProcessor.id.replace("mediaProcessor", "encoderConfigDocSchema");
    var encoderConfigId = mediaProcessor.id.replace("mediaProcessor", "encoderConfig");
    var encoderConfigFileRowId = mediaProcessor.id.replace("mediaProcessor", "encoderConfigFileRow");
    var encoderConfigFileId = mediaProcessor.id.replace("mediaProcessor", "encoderConfigFile");
    var encoderOptionsRowId = mediaProcessor.id.replace("mediaProcessor", "encoderOptionsRow");
    var encoderThumbnailGenerationRowId = mediaProcessor.id.replace("mediaProcessor", "encoderThumbnailGenerationRow");
    var encoderContentProtectionRowId = mediaProcessor.id.replace("mediaProcessor", "encoderContentProtectionRow");
    var audioAnalyzerRowId = mediaProcessor.id.replace("mediaProcessor", "audioAnalyzerRow");
    var videoAnalyzerRowId = mediaProcessor.id.replace("mediaProcessor", "videoAnalyzerRow");
    var videoSummarizationRowId = mediaProcessor.id.replace("mediaProcessor", "videoSummarizationRow");
    var faceDetectionRowId = mediaProcessor.id.replace("mediaProcessor", "faceDetectionRow");
    var faceDetectionModeId = mediaProcessor.id.replace("mediaProcessor", "faceDetectionMode");
    var motionDetectionRowId = mediaProcessor.id.replace("mediaProcessor", "motionDetectionRow");
    var contentModerationRowId = mediaProcessor.id.replace("mediaProcessor", "contentModerationRow");
    var characterRecognitionRowId = mediaProcessor.id.replace("mediaProcessor", "characterRecognitionRow");
    var encoderConfigOptions = $("#" + encoderConfigId)[0].options;
    var hideIds = [encoderConfigRowId, encoderConfigDocSchemaId, encoderConfigFileRowId, encoderOptionsRowId, encoderThumbnailGenerationRowId, encoderContentProtectionRowId, audioAnalyzerRowId, videoAnalyzerRowId, videoSummarizationRowId, faceDetectionRowId, motionDetectionRowId, contentModerationRowId, characterRecognitionRowId];
    var unbindIds = [encoderConfigDocId, encoderConfigDocSchemaId];
    ResetProcessorConfig(encoderConfigOptions, encoderConfigFileId, hideIds, unbindIds);
    switch (mediaProcessor.value) {
        case "EncoderStandard":
            $("#" + encoderConfigDocId).click(function () {
                window.open("http://docs.microsoft.com/azure/media-services/media-services-mes-presets-overview");
            });
            SetEncoderPresetOptions(encoderConfigOptions, encoderStandardPresets);
            encoderConfigOptions[encoderConfigOptions.length] = new Option("Custom Configuration File (JSON)", "Custom");
            ResetEncoderOptions(mediaProcessor);
            $("#" + encoderOptionsRowId).show();
            break;
        case "EncoderPremium":
            $("#" + encoderConfigDocId).click(function () {
                window.open("http://docs.microsoft.com/azure/media-services/media-services-encode-with-premium-workflow");
            });
            SetEncoderPresetOptions(encoderConfigOptions, encoderPremiumPresets);
            encoderConfigOptions[encoderConfigOptions.length] = new Option("Custom Configuration File (XML)", "Custom");
            ResetEncoderOptions(mediaProcessor);
            $("#" + encoderOptionsRowId).show();
            break;
        case "AudioAnalyzer":
            $("#" + audioAnalyzerRowId).show();
            break;
        case "VideoAnalyzer":
            $("#" + videoAnalyzerRowId).show();
            break;
        case "VideoSummarization":
            $("#" + videoSummarizationRowId).show();
            break;
        case "FaceDetection":
            $("#" + faceDetectionRowId).show();
            $("#" + faceDetectionModeId).change();
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
    var encoderOptionsRowId = encoderConfig.id.replace("encoderConfig", "encoderOptionsRow");
    var encoderThumbnailGenerationId = encoderConfig.id.replace("encoderConfig", "encoderThumbnailGeneration");
    var encoderThumbnailGenerationRowId = encoderConfig.id.replace("encoderConfig", "encoderThumbnailGenerationRow");
    $("#" + encoderConfigDocSchemaId).hide();
    $("#" + encoderConfigFileRowId).hide();
    $("#" + encoderOptionsRowId).show();
    $("#" + encoderThumbnailGenerationId).prop("disabled", false);
    $("#" + encoderThumbnailGenerationRowId).hide();
    switch (encoderConfig.value) {
        case "Adaptive Streaming":
        case "Content Adaptive Multiple Bitrate MP4":
            $("#" + encoderThumbnailGenerationId).prop("disabled", true);
            break;
        case "EncoderStandard_Thumbnail Generation":
            $("#" + encoderOptionsRowId).hide();
            $("#" + encoderThumbnailGenerationRowId).show();
            break;
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