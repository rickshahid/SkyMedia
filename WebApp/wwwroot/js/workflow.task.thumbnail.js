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
        var columns = null;
        var format = $(idPrefix + "Format" + taskNumber).val();
        if (format == "Sprite") {
            columns = $(idPrefix + "SpriteColumns" + taskNumber).val();
            format = "Jpg";
        }
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
            Columns: columns,
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
    var encoderThumbnailGenerationBestId = input.id.replace("encoderThumbnailGenerationFormat", "encoderThumbnailGenerationBest");
    var encoderThumbnailGenerationSingleId = input.id.replace("encoderThumbnailGenerationFormat", "encoderThumbnailGenerationSingle");
    var encoderThumbnailGenerationSpriteColumnsId = input.id.replace("encoderThumbnailGenerationFormat", "encoderThumbnailGenerationSpriteColumns");
    if (input.value == "Sprite") {
        $("#" + encoderThumbnailGenerationBestId).prop("checked", false);
        $("#" + encoderThumbnailGenerationBestId).change();
        $("#" + encoderThumbnailGenerationSingleId).prop("checked", false);
        $("#" + encoderThumbnailGenerationSingleId).change();
        $("#" + encoderThumbnailGenerationBestId).prop("disabled", true);
        $("#" + encoderThumbnailGenerationSingleId).prop("disabled", true);
        $("#" + encoderThumbnailGenerationSpriteColumnsId).spinner("enable");
    } else {
        $("#" + encoderThumbnailGenerationBestId).prop("disabled", false);
        $("#" + encoderThumbnailGenerationSingleId).prop("disabled", false);
        $("#" + encoderThumbnailGenerationSpriteColumnsId).spinner("disable");
    }
}
function SetThumbnailBest(checkbox) {
    var disableEnable = checkbox.checked ? "disable" : "enable";
    var encoderThumbnailGenerationSingleId = checkbox.id.replace("encoderThumbnailGenerationBest", "encoderThumbnailGenerationSingle");
    var encoderThumbnailGenerationStartPercentId = checkbox.id.replace("encoderThumbnailGenerationBest", "encoderThumbnailGenerationStartPercent");
    var encoderThumbnailGenerationStartHourId = checkbox.id.replace("encoderThumbnailGenerationBest", "encoderThumbnailGenerationStartHour");
    var encoderThumbnailGenerationStartMinuteId = checkbox.id.replace("encoderThumbnailGenerationBest", "encoderThumbnailGenerationStartMinute");
    var encoderThumbnailGenerationStartSecondId = checkbox.id.replace("encoderThumbnailGenerationBest", "encoderThumbnailGenerationStartSecond");
    var encoderThumbnailGenerationStepPercentId = checkbox.id.replace("encoderThumbnailGenerationBest", "encoderThumbnailGenerationStepPercent");
    var encoderThumbnailGenerationStepHourId = checkbox.id.replace("encoderThumbnailGenerationBest", "encoderThumbnailGenerationStepHour");
    var encoderThumbnailGenerationStepMinuteId = checkbox.id.replace("encoderThumbnailGenerationBest", "encoderThumbnailGenerationStepMinute");
    var encoderThumbnailGenerationStepSecondId = checkbox.id.replace("encoderThumbnailGenerationBest", "encoderThumbnailGenerationStepSecond");
    var encoderThumbnailGenerationRangePercentId = checkbox.id.replace("encoderThumbnailGenerationBest", "encoderThumbnailGenerationRangePercent");
    var encoderThumbnailGenerationRangeHourId = checkbox.id.replace("encoderThumbnailGenerationBest", "encoderThumbnailGenerationRangeHour");
    var encoderThumbnailGenerationRangeMinuteId = checkbox.id.replace("encoderThumbnailGenerationBest", "encoderThumbnailGenerationRangeMinute");
    var encoderThumbnailGenerationRangeSecondId = checkbox.id.replace("encoderThumbnailGenerationBest", "encoderThumbnailGenerationRangeSecond");
    $("#" + encoderThumbnailGenerationSingleId).prop("disabled", checkbox.checked);
    $("#" + encoderThumbnailGenerationStartPercentId).spinner(disableEnable);
    $("#" + encoderThumbnailGenerationStartHourId).spinnerEx(disableEnable);
    $("#" + encoderThumbnailGenerationStartMinuteId).spinnerEx(disableEnable);
    $("#" + encoderThumbnailGenerationStartSecondId).spinnerEx(disableEnable);
    $("#" + encoderThumbnailGenerationStepPercentId).spinner(disableEnable);
    $("#" + encoderThumbnailGenerationStepHourId).spinnerEx(disableEnable);
    $("#" + encoderThumbnailGenerationStepMinuteId).spinnerEx(disableEnable);
    $("#" + encoderThumbnailGenerationStepSecondId).spinnerEx(disableEnable);
    $("#" + encoderThumbnailGenerationRangePercentId).spinner(disableEnable);
    $("#" + encoderThumbnailGenerationRangeHourId).spinnerEx(disableEnable);
    $("#" + encoderThumbnailGenerationRangeMinuteId).spinnerEx(disableEnable);
    $("#" + encoderThumbnailGenerationRangeSecondId).spinnerEx(disableEnable);
}
function SetThumbnailSingle(checkbox) {
    var disableEnable = checkbox.checked ? "disable" : "enable";
    var encoderThumbnailGenerationBestId = checkbox.id.replace("encoderThumbnailGenerationSingle", "encoderThumbnailGenerationBest");
    var encoderThumbnailGenerationStepPercentId = checkbox.id.replace("encoderThumbnailGenerationSingle", "encoderThumbnailGenerationStepPercent");
    var encoderThumbnailGenerationStepHourId = checkbox.id.replace("encoderThumbnailGenerationSingle", "encoderThumbnailGenerationStepHour");
    var encoderThumbnailGenerationStepMinuteId = checkbox.id.replace("encoderThumbnailGenerationSingle", "encoderThumbnailGenerationStepMinute");
    var encoderThumbnailGenerationStepSecondId = checkbox.id.replace("encoderThumbnailGenerationSingle", "encoderThumbnailGenerationStepSecond");
    var encoderThumbnailGenerationRangePercentId = checkbox.id.replace("encoderThumbnailGenerationSingle", "encoderThumbnailGenerationRangePercent");
    var encoderThumbnailGenerationRangeHourId = checkbox.id.replace("encoderThumbnailGenerationSingle", "encoderThumbnailGenerationRangeHour");
    var encoderThumbnailGenerationRangeMinuteId = checkbox.id.replace("encoderThumbnailGenerationSingle", "encoderThumbnailGenerationRangeMinute");
    var encoderThumbnailGenerationRangeSecondId = checkbox.id.replace("encoderThumbnailGenerationSingle", "encoderThumbnailGenerationRangeSecond");
    $("#" + encoderThumbnailGenerationBestId).prop("disabled", checkbox.checked);
    $("#" + encoderThumbnailGenerationStepPercentId).spinner(disableEnable);
    $("#" + encoderThumbnailGenerationStepHourId).spinnerEx(disableEnable);
    $("#" + encoderThumbnailGenerationStepMinuteId).spinnerEx(disableEnable);
    $("#" + encoderThumbnailGenerationStepSecondId).spinnerEx(disableEnable);
    $("#" + encoderThumbnailGenerationRangePercentId).spinner(disableEnable);
    $("#" + encoderThumbnailGenerationRangeHourId).spinnerEx(disableEnable);
    $("#" + encoderThumbnailGenerationRangeMinuteId).spinnerEx(disableEnable);
    $("#" + encoderThumbnailGenerationRangeSecondId).spinnerEx(disableEnable);
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