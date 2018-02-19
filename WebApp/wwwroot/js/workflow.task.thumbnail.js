function ShowThumbnailGeneration(checkbox) {
    var encoderThumbnailGenerationRowId = checkbox.id.replace("encoderThumbnailGeneration", "encoderThumbnailGenerationRow");
    if (checkbox.checked) {
        $("#" + encoderThumbnailGenerationRowId).show();
    } else {
        $("#" + encoderThumbnailGenerationRowId).hide();
    }
    ScrollToBottom();
}
function GetThumbnailGeneration(taskNumber, encoderConfig) {
    var thumbnailGeneration = null;
    var idPrefix = "#encoderThumbnailGeneration";
    if ($(idPrefix + taskNumber).prop("checked") || encoderConfig.indexOf("Thumbnail Generation") > -1) {
        var format = $(idPrefix + "Format" + taskNumber).val();
        var width = $(idPrefix + "WidthPercent" + taskNumber).val();
        if (width == 0) {
            width = $(idPrefix + "WidthPixel" + taskNumber).val() + "px";
        } else {
            width = width + "%";
        }
        var height = $(idPrefix + "HeightPercent" + taskNumber).val();
        if (height == 0) {
            height = $(idPrefix + "HeightPixel" + taskNumber).val() + "px";
        } else {
            height = height + "%";
        }
        var start = $(idPrefix + "StartPercent" + taskNumber).val();
        if (start == 0) {
            var startHour = $(idPrefix + "StartHour" + taskNumber).val();
            var startMinute = $(idPrefix + "StartMinute" + taskNumber).val();
            var startSecond = $(idPrefix + "StartSecond" + taskNumber).val();
            start = startHour + ":" + startMinute + ":" + startSecond;
        } else {
            start = start + "%";
        }
        var step = $(idPrefix + "StepPercent" + taskNumber).val();
        if (step == 0) {
            var stepHour = $(idPrefix + "StepHour" + taskNumber).val();
            var stepMinute = $(idPrefix + "StepMinute" + taskNumber).val();
            var stepSecond = $(idPrefix + "StepSecond" + taskNumber).val();
            step = stepHour + ":" + stepMinute + ":" + stepSecond;
        } else {
            step = step + "%";
        }
        var range = $(idPrefix + "RangePercent" + taskNumber).val();
        if (range == 0) {
            var rangeHour = $(idPrefix + "RangeHour" + taskNumber).val();
            var rangeMinute = $(idPrefix + "RangeMinute" + taskNumber).val();
            var rangeSecond = $(idPrefix + "RangeSecond" + taskNumber).val();
            range = rangeHour + ":" + rangeMinute + ":" + rangeSecond;
        } else {
            range = range + "%";
        }
        thumbnailGeneration = {
            Format: format,
            Best: $(idPrefix + "Best" + taskNumber).prop("checked"),
            Single: $(idPrefix + "Single" + taskNumber).prop("checked"),
            Sprite: $(idPrefix + "Sprite" + taskNumber).prop("checked"),
            Columns: $(idPrefix + "SpriteColumns" + taskNumber).val(),
            Width: width,
            Height: height,
            Start: start,
            Step: step,
            Range: range
        };
    }
    return thumbnailGeneration;
}
function SetThumbnailFormat(input) {
    var encoderThumbnailGenerationSpriteId = input.id.replace("encoderThumbnailGenerationFormat", "encoderThumbnailGenerationSprite");
    switch (input.value) {
        case "Jpg":
            $("#" + encoderThumbnailGenerationSpriteId).prop("checked", false);
            $("#" + encoderThumbnailGenerationSpriteId).change();
            $("#" + encoderThumbnailGenerationSpriteId).prop("disabled", true);
            break;
        case "Sprite":
            $("#" + encoderThumbnailGenerationSpriteId).prop("disabled", false);
            $("#" + encoderThumbnailGenerationSpriteId).prop("checked", true);
            $("#" + encoderThumbnailGenerationSpriteId).change();
            break;
        default:
            $("#" + encoderThumbnailGenerationSpriteId).prop("disabled", false);
            break;
    }
}
function SetThumbnailBest(checkbox) {
    var encoderThumbnailGenerationSingleId = checkbox.id.replace("encoderThumbnailGenerationBest", "encoderThumbnailGenerationSingle");
    $("#" + encoderThumbnailGenerationSingleId).prop("disabled", checkbox.checked);
}
function SetThumbnailSingle(checkbox) {
    var encoderThumbnailGenerationBestId = checkbox.id.replace("encoderThumbnailGenerationSingle", "encoderThumbnailGenerationBest");
    $("#" + encoderThumbnailGenerationBestId).prop("disabled", checkbox.checked);
}
function SetThumbnailSprite(checkbox) {
    var enableDisable = checkbox.checked ? "enable" : "disable";
    var encoderThumbnailGenerationSpriteColumnsId = checkbox.id.replace("encoderThumbnailGenerationSprite", "encoderThumbnailGenerationSpriteColumns");
    $("#" + encoderThumbnailGenerationSpriteColumnsId).spinner(enableDisable);
}
function ClearWidthPercent(input) {
    var encoderThumbnailGenerationWidthPercentId = input.id.replace("encoderThumbnailGenerationWidthPixel", "encoderThumbnailGenerationWidthPercent");
    $("#" + encoderThumbnailGenerationWidthPercentId).val(0);
}
function ClearWidthPixel(input) {
    var encoderThumbnailGenerationWidthPixelId = input.id.replace("encoderThumbnailGenerationWidthPercent", "encoderThumbnailGenerationWidthPixel");
    $("#" + encoderThumbnailGenerationWidthPixelId).val(0);
}
function ClearHeightPercent(input) {
    var encoderThumbnailGenerationHeightPercentId = input.id.replace("encoderThumbnailGenerationHeightPixel", "encoderThumbnailGenerationHeightPercent");
    $("#" + encoderThumbnailGenerationHeightPercentId).val(0);
}
function ClearHeightPixel(input) {
    var encoderThumbnailGenerationHeightPixelId = input.id.replace("encoderThumbnailGenerationHeightPercent", "encoderThumbnailGenerationHeightPixel");
    $("#" + encoderThumbnailGenerationHeightPixelId).val(0);
}
function ClearStartPercent(input, timeId) {
    var encoderThumbnailGenerationStartPercentId = input.id.replace(timeId, "encoderThumbnailGenerationStartPercent");
    $("#" + encoderThumbnailGenerationStartPercentId).val(0);
}
function ClearStartTime(input) {
    var encoderThumbnailGenerationStartHourId = input.id.replace("encoderThumbnailGenerationStartPercent", "encoderThumbnailGenerationStartHour");
    var encoderThumbnailGenerationStartMinuteId = input.id.replace("encoderThumbnailGenerationStartPercent", "encoderThumbnailGenerationStartMinute");
    var encoderThumbnailGenerationStartSecondId = input.id.replace("encoderThumbnailGenerationStartPercent", "encoderThumbnailGenerationStartSecond");
    $("#" + encoderThumbnailGenerationStartHourId).spinnerEx("value", 0);
    $("#" + encoderThumbnailGenerationStartMinuteId).spinnerEx("value", 0);
    $("#" + encoderThumbnailGenerationStartSecondId).spinnerEx("value", 0);
}
function ClearStepPercent(input, timeId) {
    var encoderThumbnailGenerationStepPercentId = input.id.replace(timeId, "encoderThumbnailGenerationStepPercent");
    $("#" + encoderThumbnailGenerationStepPercentId).val(0);
}
function ClearStepTime(input) {
    var encoderThumbnailGenerationStepHourId = input.id.replace("encoderThumbnailGenerationStepPercent", "encoderThumbnailGenerationStepHour");
    var encoderThumbnailGenerationStepMinuteId = input.id.replace("encoderThumbnailGenerationStepPercent", "encoderThumbnailGenerationStepMinute");
    var encoderThumbnailGenerationStepSecondId = input.id.replace("encoderThumbnailGenerationStepPercent", "encoderThumbnailGenerationStepSecond");
    $("#" + encoderThumbnailGenerationStepHourId).spinnerEx("value", 0);
    $("#" + encoderThumbnailGenerationStepMinuteId).spinnerEx("value", 0);
    $("#" + encoderThumbnailGenerationStepSecondId).spinnerEx("value", 0);
}
function ClearRangePercent(input, timeId) {
    var encoderThumbnailGenerationRangePercentId = input.id.replace(timeId, "encoderThumbnailGenerationRangePercent");
    $("#" + encoderThumbnailGenerationRangePercentId).val(0);
}
function ClearRangeTime(input) {
    var encoderThumbnailGenerationRangeHourId = input.id.replace("encoderThumbnailGenerationRangePercent", "encoderThumbnailGenerationRangeHour");
    var encoderThumbnailGenerationRangeMinuteId = input.id.replace("encoderThumbnailGenerationRangePercent", "encoderThumbnailGenerationRangeMinute");
    var encoderThumbnailGenerationRangeSecondId = input.id.replace("encoderThumbnailGenerationRangePercent", "encoderThumbnailGenerationRangeSecond");
    $("#" + encoderThumbnailGenerationRangeHourId).spinnerEx("value", 0);
    $("#" + encoderThumbnailGenerationRangeMinuteId).spinnerEx("value", 0);
    $("#" + encoderThumbnailGenerationRangeSecondId).spinnerEx("value", 0);
}