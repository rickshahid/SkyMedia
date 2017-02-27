var _mediaAsset, _editedAssets, _inputAssets;
function EndsWith(source, suffix) {
    source = source.toLowerCase();
    suffix = suffix.toLowerCase();
    return (source.lastIndexOf(suffix) == source.length - suffix.length) > 0;
}
function SignOut(cookieName) {
    $.removeCookie(cookieName);
    window.location.href = "/account/signout";
}
function ReplaceAll(text, find, replace) {
    var regExp = new RegExp(find, "g");
    return text.replace(regExp, replace);
}
function CreateTip(targetId, tipText, tipPosition, hideEvent) {
   $("#" + targetId).qtip({
        content: { text: tipText },
        position: tipPosition,
        show: { delay: 1000 },
        hide: { event: hideEvent }
    });
}
function CreateTipTop(targetId, tipText, adjustX, adjustY, hideEvent) {
    var tipPosition = { my: "bottom center", at: "top center", adjust: { x: adjustX, y: adjustY } };
    CreateTip(targetId, tipText, tipPosition, hideEvent);
}
function CreateTipTopLeft(targetId, tipText, adjustX, adjustY, hideEvent) {
    var tipPosition = { my: "bottom center", at: "top left", adjust: { x: adjustX, y: adjustY } };
    CreateTip(targetId, tipText, tipPosition, hideEvent);
}
function CreateTipBottom(targetId, tipText, adjustX, adjustY, hideEvent) {
    var tipPosition = { my: "top center", at: "bottom center", adjust: { x: adjustX, y: adjustY } };
    CreateTip(targetId, tipText, tipPosition, hideEvent);
}
function CreateTipBottomLeft(targetId, tipText, adjustX, adjustY, hideEvent) {
    var tipPosition = { my: "top center", at: "bottom left", adjust: { x: adjustX, y: adjustY } };
    CreateTip(targetId, tipText, tipPosition, hideEvent);
}
function CreateTipLeft(targetId, tipText, adjustX, adjustY, hideEvent) {
    var tipPosition = { my: "right center", at: "left center", adjust: { x: adjustX, y: adjustY } };
    CreateTip(targetId, tipText, tipPosition, hideEvent);
}
function CreateTipRight(targetId, tipText, adjustX, adjustY, hideEvent) {
    var tipPosition = { my: "left center", at: "right center", adjust: { x: adjustX, y: adjustY } };
    CreateTip(targetId, tipText, tipPosition, hideEvent);
}
function SetTipVisible(targetId, tipVisible) {
    $("#" + targetId).qtip("toggle", tipVisible);
}
function SetLayout() {
    CreateTipRight("siteHome", "Azure Sky Media<br /><br />Site Home");
    CreateTipRight("siteCode", "Azure Sky Media<br /><br />Open Source");
    CreateTipRight("accountInventory", "Azure Media Services<br /><br />Account Inventory");
    CreateTipRight("accountDashboard", "Azure Media Services<br /><br />Account Dashboard");
    CreateTipLeft("userDirectory", "Azure Active Directory<br /><br />B2C");
    CreateTipLeft("userProfileEdit", "Azure Sky Media<br /><br />User Profile Edit");
    CreateTipLeft("userSignIn", "Azure Sky Media<br /><br />User Identity");
    CreateTipLeft("userSignOut", "Azure Sky Media<br /><br />User Sign Out");
    CreateTipRight("amsPlatform", "Azure Media Services");
    CreateTipRight("amsPlayer", "Azure Media Player");
    CreateTipRight("mediaStreaming", "Azure Media Services<br /><br />Streaming");
    CreateTipRight("mediaEncoding", "Azure Media Services<br /><br />Encoding");
    CreateTipRight("mediaProtection", "Azure Media Services<br /><br />Content Protection");
    CreateTipRight("mediaAnalytics", "Azure Media Services<br /><br />Media Analytics");
    CreateTipLeft("appServiceWeb", "Azure App Service<br /><br />Web Apps");
    CreateTipLeft("appServiceMobile", "Azure App Service<br /><br />Mobile Apps");
    CreateTipLeft("appServiceApi", "Azure App Service<br /><br />API Apps");
    CreateTipLeft("appServiceApiManagement", "Azure App Service<br /><br />API Management");
    CreateTipLeft("appServiceLogic", "Azure App Service<br /><br />Logic Apps");
    CreateTipLeft("appServiceFunctions", "Azure App Service<br /><br />Functions");
    CreateTipTop("mediaFileUpload", "Azure Media Services<br /><br />File Uploader");
    CreateTipTop("mediaAssetWorkflow", "Azure Media Services<br /><br />Asset Workflow");
    CreateTipTop("mediaAssetLibrary", "Azure Media Services<br /><br />Asset Library");
    CreateTipTop("mediaAssetAnalytics", "Azure Media Services<br /><br />Media Analytics");
    $(document).ajaxError(function (event, xhr, settings, error) {
        DisplayMessage("Error Message", error);
    });
}
function DisplayDialog(dialogId, title, html, buttons, height, width, onClose) {
    if (buttons == null) {
        buttons = {
            OK: function () {
                $(this).dialog("close");
            }
        };
    }
    if (html != null) {
        html = unescape(html);
        $("#" + dialogId).html(html);
    }
    if (height == null) {
        height = "auto";
    }
    if (width == null) {
        width = "auto";
    }
    $("#" + dialogId).dialog({
        buttons: buttons,
        height: height,
        width: width,
        title: title,
        close: onClose,
        resizable: false,
        modal: true
    });
    if (jQuery.isEmptyObject(buttons)) {
        $(".ui-dialog-titlebar-close").show();
    }
    $(".ui-button:last").focus();
}
function DisplayMessage(title, message, buttons, width, onClose) {
    var dialogId = "messageDialog";
    DisplayDialog(dialogId, title, message, buttons, null, width, onClose);
}
function GetMediaPlayer(editMode) {
    var mediaPlayer = amp("mediaPlayer");
    if (editMode) {
        mediaPlayer.AMVE({
            containerId: "videoEditor",
            clipdataCallback: SetVideoEdit
        });
    }
    $(".amp-logo").click(function () {
        window.open("http://amslabs.azurewebsites.net/");
    });
    return mediaPlayer;
}
function SetPlayerSpinner(visible) {
    if (visible) {
        $(".vjs-loading-spinner").show();
    } else {
        $(".vjs-loading-spinner").hide();
    }
}
function GetSourceType(sourceUrl) {
    return sourceUrl.toLowerCase().indexOf(".mp4") > -1 ? "video/mp4" : "application/vnd.ms-sstr+xml";
}
function GetLanguageLabel(languageCode) {
    var languageLabel;
    switch (languageCode.substr(0, 2).toLowerCase()) {
        case "en":
            languageLabel = "English";
            break;
        case "es":
            languageLabel = "Spanish";
            break;
        case "ar":
            languageLabel = "Arabic";
            break;
        case "zh":
            languageLabel = "Chinese";
            break;
        case "fr":
            languageLabel = "French";
            break;
        case "de":
            languageLabel = "German";
            break;
        case "it":
            languageLabel = "Italian";
            break;
        case "ja":
            languageLabel = "Japanese";
            break;
        case "pt":
            languageLabel = "Portuguese";
            break;
    }
    return languageLabel;
}
function GetProtectionInfo(protectionTypes, authToken) {
    var protectionInfo = null;
    if (protectionTypes.length > 0) {
        protectionInfo = new Array();
        authToken = window.location.href.indexOf("notoken") > -1 ? "" : "Bearer=" + authToken;
        for (var i = 0; i < protectionTypes.length; i++) {
            protectionInfo.push({
                type: protectionTypes[i],
                authenticationToken: authToken
            });
        }
    }
    return protectionInfo;
}
function ToggleMediaAnalytics(button) {
    ClearVideoOverlay();
    var buttonImage = button.children[0];
    if (buttonImage.src.indexOf("MediaAnalyticsOpen") > -1) {
        buttonImage.src = buttonImage.src.replace("Open", "Close");
        var mediaPlayer = GetMediaPlayer(false);
        var playerHeight = mediaPlayer.height();
        $("#mediaMetadata").height(playerHeight);
        $("#analyticsPanel").show();
    } else {
        buttonImage.src = buttonImage.src.replace("Close", "Open");
        $("#analyticsPanel").hide();
    }
}
