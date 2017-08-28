var _authToken, _jobInputs, _encoderConfig, _mediaStreams, _streamNumber, _saveWorkflow, _facesMetadata;
function SetLayout() {
    CreateTipBottom("siteHome", "Azure Sky Media<br /><br />Site Home");
    CreateTipBottom("siteCode", "Azure Sky Media<br /><br />Open Source");
    CreateTipBottom("siteMail", "Azure Sky Media<br /><br />Site Mail");
    CreateTipBottom("userDirectory", "Azure B2C<br /><br />Active Directory");
    CreateTipBottom("userSignIn", "Azure Sky Media<br /><br />User Sign In");
    CreateTipBottom("userSignOut", "Azure Sky Media<br /><br />User Sign Out");
    CreateTipBottom("userProfileEdit", "Azure Sky Media<br /><br />User Profile Edit");
    CreateTipRight("amsPlatform", "Azure Media Services");
    CreateTipRight("amsPlayer", "Azure Media Player");
    CreateTipRight("mediaStreaming", "Azure Media Services<br /><br />Streaming");
    CreateTipRight("mediaEncoding", "Azure Media Services<br /><br />Encoding");
    CreateTipRight("mediaProtection", "Azure Media Services<br /><br />Content Protection");
    CreateTipRight("mediaCertification", "Azure Media Services<br /><br />Security Certification");
    CreateTipRight("mediaIndexer", "Azure Cognitive Services<br /><br />Video Indexer");
    CreateTipLeft("cognitiveServices", "Azure Cognitive Services");
    CreateTipLeft("appServiceWeb", "Azure App Service<br /><br />Web Apps");
    CreateTipLeft("appServiceFunctions", "Azure App Service<br /><br />Function Apps");
    CreateTipLeft("appServiceLogic", "Azure App Service<br /><br />Logic Apps");
    CreateTipLeft("cosmosDB", "Azure Cosmos DB");
    CreateTipLeft("botService", "Azure Bot Service");
    CreateTipLeft("batchRendering", "Azure Batch Rendering");
    CreateTipTop("mediaFileUpload", "Media File Upload");
    CreateTipTop("mediaAssetWorkflow", "Media Asset Workflow");
    CreateTipTop("mediaAssetLibrary", "Media Asset Library");
    CreateTipTop("mediaStreamLeft", "Stream Tuner Left");
    CreateTipTop("mediaStreamRight", "Stream Tuner Right");
    CreateTipTop("mediaSearchLibrary", "Media Library Search");
    CreateTipTop("mediaStreamLive", "Media Live Stream");
    CreateTipTop("mediaBotViddy", "Viddy Media Bot");
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
        resizable: false,
        buttons: buttons,
        height: height,
        width: width,
        title: title,
        open: onOpen,
        close: onClose
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
    if (result.id != null && (result.id.indexOf("jid") > -1 || result.id.indexOf("jtid") > -1)) {
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
function GetProtectionInfo(contentProtection) {
    var protectionInfo = null;
    if (contentProtection.length > 0) {
        protectionInfo = new Array();
        var authToken = window.location.href.indexOf("notoken") > -1 ? "" : "Bearer=" + _authToken;
        for (var i = 0; i < contentProtection.length; i++) {
            protectionInfo.push({
                type: contentProtection[i],
                authenticationToken: authToken
            });
        }
    }
    return protectionInfo;
}
function GetMediaPlayer(clipMode) {
    var playerMode = clipMode ? "1" : "0";
    var plugins = {
        videobreakdown: {}
    };
    if (clipMode) {
        plugins.AMVE = {
            containerId: "videoClipper" + playerMode,
            clipdataCallback: CreateVideoClip
        };
    }
    var playerOptions = {
        plugins: plugins,
        playbackSpeed: {
            enabled: true
        }
    };
    var playerId = "videoPlayer" + playerMode;
    return amp(playerId, playerOptions);
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
            protectionInfo: GetProtectionInfo(mediaStream.contentProtection)
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
function ToggleIndexerInsights(mediaStream) {
    var insightsImage = document.getElementById("insightsImage");
    if ($("#indexerInsights").is(":visible")) {
        insightsImage.src = insightsImage.src.replace("Close", "Open");
        $("#indexerInsights").hide();
    } else {
        insightsImage.src = insightsImage.src.replace("Open", "Close");
        var indexerFrame = document.getElementById("indexerFrame");
        indexerFrame.src = mediaStream.insightsUrl;
        $("#indexerInsights").show();
    }
}
function ToggleMetadataPanel() {
    ClearVideoOverlay();
    var metadataImage = document.getElementById("metadataImage");
    if ($("#metadataPanel").is(":visible")) {
        metadataImage.src = metadataImage.src.replace("Close", "Open");
        $("#metadataPanel").hide();
    } else {
        metadataImage.src = metadataImage.src.replace("Open", "Close");
        var mediaPlayer = GetMediaPlayer(false);
        var playerHeight = mediaPlayer.el().clientHeight;
        $("#mediaMetadata").height(playerHeight);
        $("#metadataPanel").show();
    }
}
function SetLiveMode() {
    window.location.href = "/?mode=live";
}