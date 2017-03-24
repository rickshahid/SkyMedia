function ShowContentProtection(checkbox) {
    var contentProtectionRowId = checkbox.id.replace("encoderContentProtection", "encoderContentProtectionRow");
    if (checkbox.checked) {
        $("#" + contentProtectionRowId).show();
    } else {
        $("#" + contentProtectionRowId).hide();
    }
    EnsureVisibility();
}
function GetContentProtection(taskNumber) {
    var contentProtection = null;
    var idPrefix = "#encoderContentProtection";
    if ($(idPrefix + taskNumber).prop("checked")) {
        contentProtection = {
            Aes: $(idPrefix + "Aes" + taskNumber).prop("checked"),
            DrmPlayReady: $(idPrefix + "DrmPlayReady" + taskNumber).prop("checked"),
            DrmWidevine: $(idPrefix + "DrmWidevine" + taskNumber).prop("checked"),
            DrmFairPlay: $(idPrefix + "DrmFairPlay" + taskNumber).prop("checked"),
            ContentAuthTypeToken: $(idPrefix + "AuthTypeToken" + taskNumber).prop("checked"),
            ContentAuthTypeAddress: $(idPrefix + "AuthTypeAddress" + taskNumber).prop("checked"),
            ContentAuthAddressRange: $(idPrefix + "AuthAddressRange" + taskNumber).val()
        };
    }
    return contentProtection;
}
function SetContentProtection(checkbox) {
    if (checkbox.id.indexOf("Aes") > -1) {
        if (checkbox.checked) {
            var drmPlayReadyId = checkbox.id.replace("Aes", "DrmPlayReady");
            var drmWidevineId = checkbox.id.replace("Aes", "DrmWidevine");
            var drmFairPlayId = checkbox.id.replace("Aes", "DrmFairPlay");
            $("#" + drmPlayReadyId).prop("checked", false);
            $("#" + drmWidevineId).prop("checked", false);
            $("#" + drmFairPlayId).prop("checked", false);
        }
    } else if (checkbox.checked) {
        var aesId = checkbox.id.replace("DrmPlayReady", "Aes");
        aesId = aesId.replace("DrmWidevine", "Aes");
        aesId = aesId.replace("DrmFairPlay", "Aes");
        $("#" + aesId).prop("checked", false);
    }
}
function SetContentAuthAddressRange(checkbox) {
    var authAddressRangeId = checkbox.id.replace("AuthTypeAddress", "AuthAddressRange");
    if (checkbox.checked) {
        $("#" + authAddressRangeId).prop("disabled", false);
        $("#" + authAddressRangeId).focus();
    } else {
        $("#" + authAddressRangeId).prop("disabled", true);
    }
}
