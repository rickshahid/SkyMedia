var _authToken, _inputAssets, _encoderConfig, _mediaStreams, _streamNumber, _spokenLanguages, _saveWorkflow;
function SetLayout() {
    CreateTipBottom("siteHome", "Azure Sky Media<br /><br />Site Home");
    CreateTipBottom("siteCode", "Azure Sky Media<br /><br />Open Source");
    CreateTipBottom("accountInventory", "Azure Media Services<br /><br />Account Inventory");
    CreateTipBottom("userDirectory", "Azure B2C<br /><br />Active Directory");
    CreateTipBottom("userSignIn", "Azure Sky Media<br /><br />User Sign In");
    CreateTipBottom("userSignOut", "Azure Sky Media<br /><br />User Sign Out");
    CreateTipBottom("userProfileEdit", "Azure Sky Media<br /><br />User Profile Edit");
    CreateTipRight("amsPlatform", "Azure Media Services");
    CreateTipRight("amsPlayer", "Azure Media Player");
    CreateTipRight("mediaStreaming", "Azure Media Services<br /><br />Streaming");
    CreateTipRight("mediaEncoding", "Azure Media Services<br /><br />Encoding");
    CreateTipRight("mediaProtection", "Azure Media Services<br /><br />Content Protection");
    CreateTipRight("mediaAnalytics", "Azure Media Services<br /><br />Media Analytics");
    CreateTipRight("appServiceBot", "Azure Bot Service");
    CreateTipLeft("appService", "Azure App Service");
    CreateTipLeft("appServiceWeb", "Azure App Service<br /><br />Web Apps");
    CreateTipLeft("appServiceMobile", "Azure App Service<br /><br />Mobile Apps");
    CreateTipLeft("appServiceFunctions", "Azure App Service<br /><br />Function Apps");
    CreateTipLeft("appServiceApi", "Azure App Service<br /><br />API Apps");
    CreateTipLeft("appServiceLogic", "Azure Logic Apps");
    CreateTipLeft("appServiceInsights", "Azure Application Insights");
    CreateTipTop("mediaFileUpload", "Azure Media Services<br /><br />File Uploader");
    CreateTipTop("mediaAssetWorkflow", "Azure Media Services<br /><br />Asset Workflow");
    CreateTipTop("mediaStreamLeft", "Azure Media Services<br /><br />Stream Tuner Left");
    CreateTipTop("mediaStreamRight", "Azure Media Services<br /><br />Stream Tuner Right");
    CreateTipTop("mediaLive", "Azure Media Services<br /><br />Live Stream");
    CreateTipTop("mediaLibrary", "Azure Media Services<br /><br />Asset Library");
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
function DisplayDialog(dialogId, title, html, buttons, height, width, onOpen, onClose) {
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
        open: onOpen,
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
    DisplayDialog(dialogId, title, message, buttons, null, width, null, onClose);
}
function DisplayWorkflow(result) {
    var title, message = "", onClose = null;
    if (result.id.indexOf("jid") > -1 || result.id.indexOf("jtid") > -1) {
        title = "Azure Media Services Job";
        if (result.id.indexOf("jtid") > -1) {
            title = title + " Template";
            onClose = function () {
                window.location = window.location.href;
            }
        }
        message = result.name + "<br /><br />" + result.id;
    } else {
        title = "Azure Media Services Asset";
        if (result.length > 1) {
            title = title + "s";
        }
        for (var i = 0; i < result.length; i++) {
            message = message + GetAssetInfo(result, i);
        }
    }
    DisplayMessage(title, message, null, null, onClose);
}
function GetAssetInfo(result, i) {
    var assetInfo = result[i].assetName + "<br />";
    if (result.length == 1) {
        assetInfo = assetInfo + "<br />";
    }
    if (i > 0) {
        assetInfo = "<br /><br />" + assetInfo;
    }
    return assetInfo + result[i].assetId;
}
function GetSourceType(sourceUrl) {
    return sourceUrl.toLowerCase().indexOf(".mp4") > -1 ? "video/mp4" : "application/vnd.ms-sstr+xml";
}
function GetLanguageLabel(languageCode) {
    return _spokenLanguages[languageCode];
}
function GetProtectionInfo(protectionTypes) {
    var protectionInfo = null;
    if (protectionTypes.length > 0) {
        protectionInfo = new Array();
        var authToken = window.location.href.indexOf("notoken") > -1 ? "" : "Bearer=" + _authToken;
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
    var plugins = {};
    if (clipMode) {
        plugins.AMVE = {
            containerId: "mediaClipper",
            clipdataCallback: CreateVideoClip
        };
    }
    var playerOptions = {
        plugins: plugins,
        playbackSpeed: {
            enabled: true
        }
    };
    return amp(videoTagId, playerOptions);
}
function SetPlayerSpinner(visible) {
    if (visible) {
        $(".vjs-loading-spinner").show();
    } else {
        $(".vjs-loading-spinner").hide();
    }
}
function SetPlayerContent(mediaPlayer, mediaStream, languageCode, autoPlay) {
    mediaPlayer.autoplay(autoPlay);
    mediaPlayer.src(
        [{
            src: mediaStream.sourceUrl,
            type: GetSourceType(mediaStream.sourceUrl),
            protectionInfo: GetProtectionInfo(mediaStream.protectionTypes)
        }],
        mediaStream.textTracks
    );
    if (languageCode != "") {
        for (var i = 0; i < mediaPlayer.textTracks_.length; i++) {
            if (mediaPlayer.textTracks_.tracks_[i].language == languageCode) {
                mediaPlayer.textTracks_.tracks_[i].mode = "showing";
            }
        }
    }
}
function ToggleMediaAnalytics() {
    ClearVideoOverlay();
    var analyticsPanelImage = document.getElementById("analyticsPanelImage");
    if ($("#analyticsPanel").is(":visible")) {
        analyticsPanelImage.src = analyticsPanelImage.src.replace("Out", "In");
        $("#analyticsPanel").hide();
    } else {
        analyticsPanelImage.src = analyticsPanelImage.src.replace("In", "Out");
        var mediaPlayer = GetMediaPlayer(false);
        var playerHeight = mediaPlayer.el().clientHeight;
        $("#mediaTranscript").height(playerHeight);
        $("#mediaMetadata").height(playerHeight);
        $("#analyticsPanel").show();
    }
}
function ToggleMediaLive(button) {
    var buttonImage = button.children[0];
    if (buttonImage.src.indexOf("MediaLiveOn") > -1) {
        buttonImage.src = buttonImage.src.replace("On", "Off");
    } else {
        buttonImage.src = buttonImage.src.replace("Off", "On");
    }
}
