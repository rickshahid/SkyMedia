var _spacingPatterns, _spacingInserts, _storageCdnUrl;
function InitializeApp(appName) {
    CreateTipBottom("siteHome", appName + "<br><br>Site Home");
    CreateTipBottom("siteCode", appName + "<br><br>Open Source");
    CreateTipBottom("mediaBlog", "Azure Media Services<br><br>News Blog");
    CreateTipBottom("botService", "Azure Bot Service");
    CreateTipBottom("userDirectory", "Azure B2C<br><br>Active Directory");
    CreateTipBottom("userProfileEdit", appName + "<br><br>Account Profile Edit");
    CreateTipBottom("userSignIn", appName + "<br><br>User Sign In");
    CreateTipBottom("userSignOut", appName + "<br><br>User Sign Out");
    CreateTipBottom("mediaAccount", "Azure Media Services<br><br>Account Inventory");
    CreateTipRight("mediaServices", "Azure Media Services");
    CreateTipRight("mediaEncoding", "Azure Media Services<br><br>Encoding");
    CreateTipRight("mediaProtection", "Azure Media Services<br><br>Content Protection");
    CreateTipRight("mediaStreaming", "Azure Media Services<br><br>Streaming");
    CreateTipRight("mediaServicesPlayer", "Azure Media Player");
    CreateTipRight("mediaIndexer", "Azure Video Indexer");
    CreateTipLeft("cognitiveSearch", "Azure Cognitive Search");
    CreateTipLeft("contentDeliveryNetwork", "Azure Content Delivery Network");
    CreateTipLeft("functionApp", "Azure Functions");
    CreateTipLeft("logicApp", "Azure Logic App");
    CreateTipLeft("cosmosDB", "Azure Cosmos DB");
    CreateTipLeft("mediaStorage", "Azure Blob Storage");
    CreateTipTop("mediaUpload", "Upload");
    CreateTipTop("mediaTransform", "Transform");
    CreateTipTop("mediaJob", "Job");
    CreateTipTop("streamTuner", "Stream Tuner", 0, -10);
    CreateTipTop("mediaSearch", "Search");
    CreateTipTop("mediaEdit", "Edit");
    CreateTipTop("userEngagement", "Engagement");
    CreateTipTop("mediaServicesCompliance", "Azure Media Services<br><br>Security Compliance");
    $(document).ajaxError(function (event, xhr, settings, error) {
        SetCursor(false);
        var title = "Error Message";
        var message = error;
        if (xhr.responseJSON != null) {
            var ex = JSON.parse(xhr.responseJSON);
            title = title + " (" + ex.error.code + ")";
            message = ex.error.message;
        }
        DisplayMessage(title, message);
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
function GetParentResourceName(childResource) {
    var childId = childResource.id.split("/");
    return childId.length > 12 ? childId[childId.length - 3] : childId[childId.length - 1];
}
function DisplayDialog(dialogId, title, html, buttons, height, width, onClose) {
    title = decodeURIComponent(title);
    if (html != null) {
        html = decodeURIComponent(html);
        $("#" + dialogId).html(html);
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
    if (buttons == null) {
        $(".ui-dialog-titlebar-close").show();
    } else {
        $(".ui-button:last").focus();
    }
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