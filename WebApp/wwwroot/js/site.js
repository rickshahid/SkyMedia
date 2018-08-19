var _authToken, _mediaPlayer, _mediaStreams, _streamNumber, _encoderConfig, _assetIds, _storageCdnUrl;
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
    CreateTipBottom("userProfileEdit", "Azure Sky Media<br><br>Account Profile Edit");
    CreateTipBottom("userSignIn", "Azure Sky Media<br><br>User Sign In");
    CreateTipBottom("userSignOut", "Azure Sky Media<br><br>User Sign Out");
    CreateTipRight("amsPlatform", "Azure Media Services");
    CreateTipRight("amsPlayer", "Azure Media Player");
    CreateTipRight("mediaStreaming", "Azure Media Services<br><br>Streaming");
    CreateTipRight("mediaEncoding", "Azure Media Services<br><br>Encoding");
    CreateTipRight("mediaProtection", "Azure Media Services<br><br>Content Protection");
    CreateTipRight("mediaIndexer", "Azure Video Indexer");
    CreateTipLeft("mediaStorage", "Azure Blob Storage");
    CreateTipLeft("cosmosDB", "Azure Cosmos DB");
    CreateTipLeft("contentDeliveryNetwork", "Azure Content Delivery Network");
    CreateTipLeft("appServiceWeb", "Azure Web Apps");
    CreateTipLeft("appServiceFunctions", "Azure Function Apps");
    CreateTipLeft("eventGrid", "Azure Event Grid");
    CreateTipTop("mediaAccount", "Azure Media Services<br><br>Account Name");
    CreateTipTop("mediaUpload", "Upload");
    CreateTipTop("mediaTransform", "Transform");
    CreateTipTop("mediaJob", "Job");
    CreateTipTop("streamTuner", "Stream Tuner", 0, -10);
    CreateTipTop("mediaSearch", "Search");
    CreateTipTop("mediaCompose", "Compose");
    CreateTipTop("mediaGallery", "Gallery");
    CreateTipBottom("mediaServicesCompliance", "Azure Media Services<br><br>Security Compliance");
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
    title = decodeURIComponent(title);
    if (buttons == null) {
        buttons = {
            OK: function () {
                $(this).dialog("close");
            }
        };
    }
    if (html != null) {
        var txtArea = document.createElement("textarea");
        txtArea.innerHTML = html;
        html = txtArea.value;
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
function DisplayMessage(title, message, buttons, onClose) {
    var dialogId = "messageDialog";
    DisplayDialog(dialogId, title, message, buttons, null, null, null, onClose);
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
function GetMediaPlayer(playerId, userId, accountName) {
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
            videobreakdown: {
            }
        }
    };
    return amp(playerId, options);
}
function SetPlayerSpinner(visible) {
    if (visible) {
        $(".vjs-loading-spinner").show();
    } else {
        $(".vjs-loading-spinner").hide();
    }
}
function SetPlayerContent(mediaPlayer, mediaStream) {
    $("#mediaStreamLeft").prop("disabled", true);
    $("#mediaStreamRight").prop("disabled", true);
    $("#streamTuner").slider("option", "disabled", true);
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
}