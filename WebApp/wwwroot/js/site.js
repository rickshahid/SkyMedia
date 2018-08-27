var _mediaPlayer, _mediaStreams, _streamNumber, _storageCdnUrl;
function SetLayout(appTitle) {
    CreateTipBottom("siteHome", appTitle + "<br><br>Site Home");
    CreateTipBottom("siteCode", appTitle + "<br><br>Open Source");
    CreateTipBottom("mediaBlog", "Azure Media Services<br><br>News Blog");
    CreateTipBottom("botService", "Azure Bot Service");
    CreateTipBottom("userDirectory", "Azure B2C<br><br>Active Directory");
    CreateTipBottom("userProfileEdit", appTitle + "<br><br>Account Profile Edit");
    CreateTipBottom("userSignIn", appTitle + "<br><br>User Sign In");
    CreateTipBottom("userSignOut", appTitle + "<br><br>User Sign Out");
    CreateTipBottom("mediaAccount", "Azure Media Services<br><br>Account Inventory");
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
    CreateTipLeft("appServiceFunctions", "Azure Functions");
    CreateTipLeft("eventGrid", "Azure Event Grid");
    CreateTipTop("mediaUpload", "Upload");
    CreateTipTop("mediaTransform", "Transform");
    CreateTipTop("mediaJob", "Job");
    CreateTipTop("streamTuner", "Stream Tuner", 0, -10);
    CreateTipTop("mediaSearch", "Search");
    CreateTipTop("mediaCompose", "Compose");
    CreateTipTop("mediaGallery", "Gallery");
    CreateTipTop("mediaServicesCompliance", "Azure Media Services<br><br>Security Compliance");
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
function SetCursor(isBusy) {
    if (isBusy) {
        $("body").css("cursor", "wait");
    } else {
        $("body").css("cursor", "auto");
    }
}
function SignOut(cookieName) {
    $.removeCookie(cookieName);
    window.location.href = "/account/signOut";
}
function GetMediaPlayer(playerId, userId, accountName, galleryView, spriteVttUrl) {
    var playerOptions = {
        fluid: true,
        controls: true,
        width: galleryView ? "400" : "100%",
        height: galleryView ? "400" : "auto",
        playbackSpeed: {
            enabled: true
        },
        plugins: {
            appInsights: {
                userId: userId,
                accountId: accountName
            },
            spriteTip: {
                vttUrl: spriteVttUrl
            },
            videobreakdown: {
            }
        }
    };
    if (window.location.href.indexOf("debug") > -1) {
        playerOptions.plugins.diagnosticOverlay = {
            title: "Diagnostics",
            bgColor: "black",
            opacity: 0.5,
            x: "left",
            y: "top"
        };
    }
    return amp(playerId, playerOptions);
}
function SetPlayerContent(mediaPlayer, mediaStream) {
    $("#mediaStreamLeft").prop("disabled", true);
    $("#mediaStreamRight").prop("disabled", true);
    $("#streamTuner").slider("option", "disabled", true);
    if (mediaStream.source.protectionInfo.length > 0) {
        if (window.location.href.indexOf("notoken") > -1) {
            for (var i = 0; i < mediaStream.source.protectionInfo.length; i++) {
                mediaStream.source.protectionInfo[i].authenticationToken = null;
            }
        }
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
function DisplayDialog(dialogId, title, html, buttons, height, width) {
    title = decodeURIComponent(title);
    if (html != null) {
        html = decodeURIComponent(html);
        $("#" + dialogId).html(html);
    }
    if (buttons == null) {
        buttons = {
            OK: function () {
                $(this).dialog("close");
            }
        };
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
        width: width
    });
    if (jQuery.isEmptyObject(buttons)) {
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
        Yes: onConfirm,
        No: function () {
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