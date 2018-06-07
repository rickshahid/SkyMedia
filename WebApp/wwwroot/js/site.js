var _authToken, _mediaStreams, _streamNumber, _streamIndex, _encoderConfig, _assetIds, _storageCdnUrl;
function SetCursor(isBusy) {
    if (isBusy) {
        $("body").css("cursor", "wait");
    } else {
        $("body").css("cursor", "auto");
    }
}
function SetLayout() {
    CreateTipBottom("siteHome", "Azure Sky Media<br><br>Site Home");
    CreateTipBottom("siteCode", "Azure Sky Media<br><br>Open Source");
    CreateTipBottom("mediaBlog", "Azure Media Services<br><br>News Blog");
    CreateTipBottom("botService", "Azure Bot Service");
    CreateTipBottom("userDirectory", "Azure B2C<br><br>Active Directory");
    CreateTipBottom("userProfileEdit", "Azure Sky Media<br><br>User Profile Edit");
    CreateTipBottom("userSignIn", "Azure Sky Media<br><br>User Sign In");
    CreateTipBottom("userSignOut", "Azure Sky Media<br><br>User Sign Out");
    CreateTipRight("amsPlatform", "Azure Media Services");
    CreateTipRight("amsPlayer", "Azure Media Player");
    CreateTipRight("mediaStreaming", "Azure Media Services<br><br>Streaming");
    CreateTipRight("mediaEncoding", "Azure Media Services<br><br>Encoding");
    CreateTipRight("mediaProtection", "Azure Media Services<br><br>Content Protection");
    CreateTipRight("mediaInsight", "Azure Video Indexer<br><br>Content Insight");
    CreateTipLeft("mediaStorage", "Azure Blob Storage");
    CreateTipLeft("cosmosDB", "Azure Cosmos DB");
    CreateTipLeft("contentDeliveryNetwork", "Azure Content Delivery Network");
    CreateTipLeft("appServiceWeb", "Azure Web Apps");
    CreateTipLeft("appServiceFunctions", "Azure Function Apps");
    CreateTipLeft("eventGrid", "Azure Event Grid");
    CreateTipTop("mediaAccount", "Azure Media Services<br><br>Account Name");
    CreateTipTop("mediaUpload", "Media Upload");
    CreateTipTop("mediaTransform", "Media Transform");
    CreateTipTop("mediaJob", "Media Job");
    CreateTipTop("streamSlider", "Media Stream Tuner", 0, -10);
    CreateTipTop("mediaSearch", "Media Search");
    CreateTipTop("mediaLive", "Media Live");
    CreateTipTop("mediaEditor", "Media Edit");
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
function GetMediaPlayer(userId, accountName) {
    var options = {
        fluid: true,
        playbackSpeed: {
            enabled: true
        },
        plugins: {
            appInsights: {
                userId: userId,
                accountId: accountName
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