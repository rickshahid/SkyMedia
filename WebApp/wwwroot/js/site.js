var _spacingPatterns, _spacingInserts, _storageCdnUrl;
function InitializeApp(appName, userId) {
    CreateTipBottom("siteHome", appName + "<br><br>Site Home");
    CreateTipBottom("siteCode", appName + "<br><br>Open Source");
    CreateTipBottom("mediaBlog", "Azure Media<br><br>News Blog");
    CreateTipBottom("botService", "Azure Bot Service");
    CreateTipBottom("userDirectory", "Azure B2C<br><br>Active Directory");
    CreateTipBottom("userProfileEdit", appName + "<br><br>Account Profile Edit" + "<br><br>(" + userId + ")");
    CreateTipBottom("userSignIn", appName + "<br><br>User Sign In");
    CreateTipBottom("userSignOut", appName + "<br><br>User Sign Out");
    CreateTipBottom("mediaAccount", "Media Account");
    CreateTipBottom("mediaSearch", "Media Account<br><br>Search");
    CreateTipRight("mediaServices", "Azure Media Services");
    CreateTipRight("mediaServicesPlayer", "Azure Media Services<br><br>Player");
    CreateTipRight("mediaProtection", "Azure Media Services<br><br>Content Protection");
    CreateTipRight("mediaStreaming", "Azure Media Services<br><br>Streaming");
    CreateTipRight("mediaEncoding", "Azure Media Services<br><br>Encoding");
    CreateTipRight("mediaIndexing", "Azure Media Services<br><br>Video Indexer");
    CreateTipLeft("contentDeliveryNetwork", "Azure Content Delivery Network");
    CreateTipLeft("cognitiveServices", "Azure Cognitive Services");
    CreateTipLeft("search", "Azure Search");
    CreateTipLeft("functionApp", "Azure Functions");
    CreateTipLeft("cosmosDB", "Azure Cosmos DB");
    CreateTipLeft("mediaStorage", "Azure Blob Storage");
    CreateTipTop("mediaUpload", "Upload");
    CreateTipTop("mediaTransform", "Transform");
    CreateTipTop("mediaJob", "Job");
    CreateTipTop("streamTuner", "Stream Tuner", 0, -10);
    CreateTipTop("mediaLive", "Live");
    CreateTipTop("mediaInsight", "Insight");
    CreateTipTop("mediaEdit", "Edit");
    CreateTipTop("mediaServicesCompliance", "Azure Media Services<br><br>Security Compliance");
    $(document).ajaxError(function (event, xhr, settings, error) {
        SetCursor(false);
        var title = "Error Message";
        var message = error;
        if (xhr.responseJSON != null) {
            var ex = xhr.responseJSON;
            if (typeof (ex) != "object") {
                ex = JSON.parse(ex);
            }
            if (ex.ErrorType != null) {
                title = title + " (" + ex.ErrorType + ")";
                message = ex.Message;
            } else {
                title = title + " (" + ex.error.code + ")";
                message = ex.error.message;
            }
        }
        if (message != null && message != "") {
            DisplayMessage(title, message);
        }
    });
    $.ajaxSetup({
        cache: false
    });
}
function SetCursor(busy) {
    if (busy) {
        $("body").css("cursor", "wait");
    } else {
        $("body").css("cursor", "auto");
    }
}
function SignOut(cookieName) {
    $.removeCookie(cookieName);
    window.location.href = "/account/signOut";
}
function GetParentName(child) {
    var childId = child.id.split("/");
    return childId.length > 12 ? childId[childId.length - 3] : childId[childId.length - 1];
}
function DisplayDialog(dialogId, title, html, buttons, height, width, onClose) {
    title = decodeURIComponent(title);
    if (html != null) {
        html = decodeURIComponent(html);
        $("#" + dialogId).html(html);
    }
    if (buttons == null) {
        buttons = {
            OK: function () {
                $(this).dialog("close");
            }
        };
    }
    if (height == null) {
        height = "auto";
    }
    if (width == null) {
        width = "auto";
    }
    $("#" + dialogId).dialog({
        resizable: false,
        title: title,
        buttons: buttons,
        height: height,
        width: width,
        close: onClose
    });
    $(".ui-button:last").focus();
}
function DisplayMessage(title, message, buttons) {
    var dialogId = "messageDialog";
    DisplayDialog(dialogId, title, message, buttons);
}
function ConfirmMessage(title, message, onConfirm) {
    var buttons = {
        OK: onConfirm,
        Cancel: function () {
            $(this).dialog("close");
        }
    };
    DisplayMessage(title, message, buttons);
}
function ConfirmDialog(dialogId, title, onConfirm) {
    var buttons = {
        OK: onConfirm,
        Cancel: function () {
            $(this).dialog("close");
        }
    };
    DisplayDialog(dialogId, title, null, buttons);
}
function CreateTip(targetId, tipText, tipPosition) {
    $("#" + targetId).qtip({
        content: { text: tipText },
        position: tipPosition,
        show: { delay: 1000 }
    });
}
function CreateTipTop(targetId, tipText, adjustX, adjustY) {
    var tipPosition = { my: "bottom center", at: "top center", adjust: { x: adjustX, y: adjustY } };
    CreateTip(targetId, tipText, tipPosition);
}
function CreateTipTopLeft(targetId, tipText, adjustX, adjustY) {
    var tipPosition = { my: "bottom center", at: "top left", adjust: { x: adjustX, y: adjustY } };
    CreateTip(targetId, tipText, tipPosition);
}
function CreateTipBottom(targetId, tipText, adjustX, adjustY) {
    var tipPosition = { my: "top center", at: "bottom center", adjust: { x: adjustX, y: adjustY } };
    CreateTip(targetId, tipText, tipPosition);
}
function CreateTipLeft(targetId, tipText, adjustX, adjustY) {
    var tipPosition = { my: "right center", at: "left center", adjust: { x: adjustX, y: adjustY } };
    CreateTip(targetId, tipText, tipPosition);
}
function CreateTipRight(targetId, tipText, adjustX, adjustY) {
    var tipPosition = { my: "left center", at: "right center", adjust: { x: adjustX, y: adjustY } };
    CreateTip(targetId, tipText, tipPosition);
}
function SetTipVisible(targetId, tipVisible) {
    $("#" + targetId).qtip("toggle", tipVisible);
}
function SetSliderValue(sliderId, labelId, sliderValue) {
    var minValue = $("#" + sliderId).slider("option", "min");
    var maxValue = $("#" + sliderId).slider("option", "max");
    if (sliderValue == minValue || sliderValue == maxValue) {
        $("#" + labelId).addClass("sliderRangeBoundary");
    } else {
        var leftOffset = GetLeftOffset(sliderId, sliderValue);
        $("#" + labelId).text(sliderValue);
        $("#" + labelId).css({ left: leftOffset + "px" });
        $("#" + labelId).removeClass("sliderRangeBoundary");
    }
}
function GetLeftOffset(sliderId, sliderValue) {
    var sliderWidth = $("#" + sliderId).width();
    var sliderMinValue = $("#" + sliderId).slider("option", "min");
    var sliderMaxValue = $("#" + sliderId).slider("option", "max");
    var sliderValueWidth = Math.floor(sliderWidth / (sliderMaxValue - sliderMinValue));
    return (sliderValueWidth * (sliderValue - sliderMinValue)) - (sliderWidth / 2) + 1;
}