var _authToken, _inputAssets, _encoderConfig, _saveWorkflow, _mediaStreams, _streamNumber, _spokenLanguages;
function SetLayout() {
    CreateTipBottom("siteHome", "Azure Sky Media<br /><br />Site Home");
    CreateTipBottom("siteCode", "Azure Sky Media<br /><br />Open Source");
    CreateTipBottom("accountInventory", "Azure Media Services<br /><br />Account Inventory");
    CreateTipBottom("mediaLibrary", "Azure Media Services<br /><br />Asset Library");
    CreateTipBottom("userDirectory", "Azure B2C<br /><br />Active Directory");
    CreateTipBottom("userSignIn", "Azure Sky Media<br /><br />User Identity");
    CreateTipBottom("userSignOut", "Azure Sky Media<br /><br />User Sign Out");
    CreateTipBottom("userProfileEdit", "Azure Sky Media<br /><br />User Profile Edit");
    CreateTipRight("amsPlatform", "Azure Media Services");
    CreateTipRight("amsPlayer", "Azure Media Player");
    CreateTipRight("mediaStreaming", "Azure Media Services<br /><br />Streaming");
    CreateTipRight("mediaEncoding", "Azure Media Services<br /><br />Encoding");
    CreateTipRight("mediaProtection", "Azure Media Services<br /><br />Content Protection");
    CreateTipRight("mediaAnalytics", "Azure Media Services<br /><br />Media Analytics");
    CreateTipRight("contentDeliveryNetwork", "Azure CDN<br /><br />(Content Delivery Network)");
    CreateTipLeft("appServiceWeb", "Azure App Service<br /><br />Web Apps");
    CreateTipLeft("appServiceMobile", "Azure App Service<br /><br />Mobile Apps");
    CreateTipLeft("appServiceFunctions", "Azure App Service<br /><br />Function Apps");
    CreateTipLeft("appServiceApi", "Azure App Service<br /><br />API Apps");
    CreateTipLeft("appServiceApiManagement", "Azure API Management");
    CreateTipLeft("appServiceLogic", "Azure Logic Apps");
    CreateTipLeft("appServiceBot", "Azure Bot Service");
    CreateTipTop("mediaFileUpload", "Azure Media Services<br /><br />File Uploader");
    CreateTipTop("mediaAssetWorkflow", "Azure Media Services<br /><br />Asset Workflow");
    CreateTipTop("mediaStreamLeft", "Azure Media Services<br /><br />Stream Tuner Left");
    CreateTipTop("mediaStreamRight", "Azure Media Services<br /><br />Stream Tuner Right");
    CreateTipTop("mediaAssetClipper", "Azure Media Services<br /><br />Video Clipper");
    CreateTipTop("mediaAssetAnalytics", "Azure Media Services<br /><br />Media Analytics");
    $(".amp-logo").click(function () {
        window.open("http://amslabs.azurewebsites.net/");
    });
    $(document).ajaxError(function (event, xhr, settings, error) {
        DisplayMessage("Error Message", error);
    });
    $.ajaxSetup({
        cache: false
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
    return _spokenLanguages[languageCode];
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
    var videoTagId = clipMode ? "videoClipper" : "videoPlayer";
    var playerOptions = {
        plugins: {
            "playbackrate": {}
        }
    };
    var mediaPlayer = amp(videoTagId, playerOptions);
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
    };
    SetPlayerContent(mediaStream, languageCode, true, true, _authToken);
    DisplayDialog(dialogId, title, null, buttons, null, null, onClose);
}
function CreateVideoClip(clipData) {
    if (clipData != null) {
        $.post("/asset/clip",
            {
                clipMode: clipData._amveUX.mode,
                clipName: clipData.title,
                sourceUrl: clipData.src,
                markIn: Math.floor(clipData.markIn),
                markOut: Math.floor(clipData.markOut)
            },
            function (result) {
                window.location = window.location.href;
            }
        );
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
