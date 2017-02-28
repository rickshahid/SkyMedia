var _inputAssets, _mediaStreams, _streamNumber, _authToken;
function SetLayout() {
    CreateTipBottom("siteHome", "Azure Sky Media<br /><br />Site Home");
    CreateTipBottom("siteCode", "Azure Sky Media<br /><br />Open Source");
    CreateTipBottom("accountInventory", "Azure Media Services<br /><br />Account Inventory");
    CreateTipBottom("mediaLibrary", "Azure Media Services<br /><br />Asset Library");
    CreateTipBottom("userDirectory", "Azure Active Directory<br /><br />B2C Service");
    CreateTipBottom("userSignIn", "Azure Sky Media<br /><br />User Identity");
    CreateTipBottom("userSignOut", "Azure Sky Media<br /><br />User Sign Out");
    CreateTipBottom("userProfileEdit", "Azure Sky Media<br /><br />User Profile Edit");
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
    CreateTipTop("mediaAssetClipper", "Azure Media Services<br /><br />Video Clipper");
    CreateTipTop("mediaAssetAnalytics", "Azure Media Services<br /><br />Media Analytics");
    $(".amp-logo").click(function () {
        window.open("http://amslabs.azurewebsites.net/");
    });
    $(document).ajaxError(function (event, xhr, settings, error) {
        DisplayMessage("Error Message", error);
    });
}
function SignOut(cookieName) {
    $.removeCookie(cookieName);
    window.location.href = "/account/signout";
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
function GetMediaPlayer(clipMode) {
    var videoId = clipMode ? "videoClipper" : "videoPlayer";
    var mediaPlayer = amp(videoId);
    if (clipMode) {
        mediaPlayer.AMVE({
            containerId: "mediaClipper",
            clipdataCallback: CreateVideoClip
        });
    }
    return mediaPlayer;
}
function SetPlayerSpinner(visible) {
    if (visible) {
        $(".vjs-loading-spinner").show();
    } else {
        $(".vjs-loading-spinner").hide();
    }
}
function SetPlayerContent(mediaStream, languageCode, clipMode, autoPlay, authToken) {
    var mediaPlayer = GetMediaPlayer(clipMode);
    mediaPlayer.autoplay(autoPlay);
    mediaPlayer.src(
        [{
            src: mediaStream.sourceUrl,
            type: GetSourceType(mediaStream.sourceUrl),
            protectionInfo: GetProtectionInfo(mediaStream.protectionTypes, authToken)
        }],
        mediaStream.textTracks
    );
    if (languageCode != "") {
        var languageCode = languageCode.toLowerCase();
        for (var i = 0; i < mediaPlayer.textTracks_.length; i++) {
            if (mediaPlayer.textTracks_.tracks_[i].language.toLowerCase() == languageCode) {
                mediaPlayer.textTracks_.tracks_[i].mode = "showing";
            }
        }
    }
}
function DisplayVideoClipper(languageCode) {
    var mediaStream = _mediaStreams[_streamNumber - 1];
    var dialogId = "clipperDialog";
    var title = "Azure Media Video Clipper";
    var buttons = {};
    var onClose = function () {
        $("#mediaClipper").empty();
        location.reload(true);
    };
    SetPlayerContent(mediaStream, languageCode, true, true, _authToken);
    DisplayDialog(dialogId, title, null, buttons, null, null, onClose);
}
function CreateVideoClip(clipData) {
    if (clipData != null) {
        //if (clipData._amveUX.mode == 2) { // Rendered
            // TBD
        //} else {
            $.post("/asset/filter",
                {
                    sourceUrl: clipData.src,
                    filterName: clipData.title,
                    markIn: Math.floor(clipData.markIn),
                    markOut: Math.floor(clipData.markOut)
                },
                function (result) {
                }
            );
        //}
        $("#clipperDialog").dialog("close");
    }
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
