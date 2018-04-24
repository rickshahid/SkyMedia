var _authToken, _mediaStreams, _streamNumber, _streamIndex, _encoderConfig, _assetIds;
function SetCursor(busy) {
    if (busy) {
        $("body").css("cursor", "wait");
    } else {
        $("body").css("cursor", "auto");
    }
}
function SetLayout(b2b) {
    CreateTipBottom("siteHome", "Azure Sky Media<br><br>Site Home");
    CreateTipBottom("siteCode", "Azure Sky Media<br><br>Open Source");
    CreateTipBottom("mediaStreamLive", "Azure Media Services<br><br>Live Streaming");
    CreateTipBottom("accountInventory", "Azure Media Services<br><br>Account Inventory");
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
    CreateTipTop("mediaProcessorPresets", "Media Processor<br><br>Presets");
    CreateTipTop("mediaFileWorkflow", "Media File<br><br>Workflow");
    CreateTipTop("mediaAssetWorkflow", "Media Asset<br><br>Workflow");
    CreateTipTop("streamSlider", "Media Stream<br><br>Tuner", 0, -10);
    CreateTipTop("mediaAssetFiles", "Media Asset<br><br>Files");
    CreateTipTop("mediaAssetClipper", "Media Asset<br><br>Clipper");
    CreateTipTop("mediaInsightSearch", "Media Insight<br><br>Search");
    CreateTipBottom("mediaCertification", "Azure Media Services<br><br>Security Certification");
    $(document).ajaxError(function (event, xhr, settings, error) {
        SetCursor(false);
        if (error != "") {
            DisplayMessage("Error Message", error);
        }
    });
    $.ajaxSetup({
        cache: false
    });
}
function SignOut(cookieName) {
    $.removeCookie(cookieName);
    window.location.href = "/account/signOut";
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
function ConfirmMessage(title, message, onConfirm) {
    var buttons = {
        Yes: onConfirm,
        No: function () {
            $(this).dialog("close");
        }
    };
    DisplayMessage(title, message, buttons);
}
function GetMediaPlayer(userId, accountId) {
    var options = {
        fluid: true,
        playbackSpeed: {
            enabled: true
        },
        plugins: {
            appInsights: {
                userId: userId,
                accountId: accountId
            },
            spriteTip: {
            },
            videobreakdown: {
            }
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
    if (mediaStream.source.protectionInfo.length > 0 && window.location.href.indexOf("auth=off") == -1) {
        mediaPlayer.src(
            [{
                src: mediaStream.source.src,
                protectionInfo: mediaStream.source.protectionInfo
            }],
            mediaStream.textTracks
        );
    } else {
        mediaPlayer.src(
            [{
                src: mediaStream.source.src
            }],
            mediaStream.textTracks
        );
    }
    if (languageCode != "") {
        for (var i = 0; i < mediaPlayer.textTracks_.length; i++) {
            if (mediaPlayer.textTracks_.tracks_[i].language == languageCode) {
                mediaPlayer.textTracks_.tracks_[i].mode = "showing";
            }
        }
    }
}