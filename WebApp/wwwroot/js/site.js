var _authToken, _mediaStreams, _streamNumber, _encoderConfig, _assetIds;
function SetLayout(b2b) {
    CreateTipBottom("siteHome", "Azure Sky Media<br><br>Site Home");
    CreateTipBottom("siteCode", "Azure Sky Media<br><br>Open Source");
    CreateTipBottom("accountInventory", "Azure Media Services<br><br>Account Inventory");
    CreateTipBottom("mediaBot", "Azure Sky Media<br><br>Bot (Viddy)");
    if (b2b) {
        CreateTipBottom("userDirectory", "Azure<br><br>Active Directory");
    } else {
        CreateTipBottom("userDirectory", "Azure B2C<br><br>Active Directory");
    }
    CreateTipBottom("userProfileEdit", "Azure Sky Media<br><br>User Profile Edit");
    CreateTipBottom("userSignIn", "Azure Sky Media<br><br>User Sign In");
    CreateTipBottom("userSignOut", "Azure Sky Media<br><br>User Sign Out");
    CreateTipRight("amsPlatform", "Azure Media Services");
    CreateTipRight("amsPlayer", "Azure Media Player");
    CreateTipRight("mediaStreaming", "Azure Media Services<br><br>Streaming");
    CreateTipRight("mediaEncoding", "Azure Media Services<br><br>Encoding");
    CreateTipRight("mediaProtection", "Azure Media Services<br><br>Content Protection");
    CreateTipRight("mediaClipper", "Azure Media Clipper");
    CreateTipRight("mediaIndexer", "Azure Video Indexer");
    CreateTipLeft("cognitiveServices", "Azure Cognitive Services");
    CreateTipLeft("appServiceWeb", "Azure App Service<br><br>Web Apps");
    CreateTipLeft("appServiceFunctions", "Azure App Service<br><br>Function Apps");
    CreateTipLeft("contentDeliveryNetwork", "Azure Content Delivery Network");
    CreateTipLeft("botService", "Azure Bot Service");
    CreateTipLeft("batchRendering", "Azure Batch Rendering");
    CreateTipLeft("cosmosDB", "Azure Cosmos DB");
    CreateTipTop("mediaProcessorPresetEditor", "Media Processor<br><br>Preset Editor");
    CreateTipTop("mediaFileWorkflow", "Media File Workflow");
    CreateTipTop("mediaAssetWorkflow", "Media Asset Workflow");
    CreateTipTop("mediaAssetFiles", "Media Asset Files");
    CreateTipTop("mediaStreamLeft", "Stream Tuner Left");
    CreateTipTop("mediaStreamRight", "Stream Tuner Right");
    CreateTipTop("mediaStreamLive", "Media Stream Live");
    CreateTipTop("mediaInsightSearch", "Media Insight Search");
    CreateTipBottom("mediaCertification", "Azure Media Services<br><br>Security Certification");
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
function GetMediaPlayer() {
    var options = {
        fluid: true,
        playbackSpeed: {
            enabled: true
        },
        plugins: {
            videobreakdown: {}
        }
    };
    return amp("videoPlayer", options);
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
            src: mediaStream.source.src,
            protectionInfo: mediaStream.streamProtection
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