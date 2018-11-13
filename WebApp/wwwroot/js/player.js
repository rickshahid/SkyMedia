var _mediaPlayer, _mediaStreams, _streamNumber;
function GetMediaPlayer(playerId, userId, accountName, autoPlay, galleryView, spriteVttUrl) {
    if (!autoPlay && userId == "") {
        autoPlay = true;
    }
    var playerOptions = {
        fluid: true,
        controls: true,
        autoplay: autoPlay,
        heuristicProfile: "LowLatency",
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
    var sourceUrl = mediaStream.source.src + "(format=mpd-time-cmaf)";
    if (mediaStream.source.protectionInfo != null && mediaStream.source.protectionInfo.length > 0) {
        if (window.location.href.indexOf("token=0") > -1) {
            for (var i = 0; i < mediaStream.source.protectionInfo.length; i++) {
                mediaStream.source.protectionInfo[i].authenticationToken = null;
            }
        }
        mediaPlayer.src(
            [{
                src: sourceUrl,
                protectionInfo: mediaStream.source.protectionInfo
            }],
            mediaStream.textTracks
        );
    } else {
        mediaPlayer.src(
            [{
                src: sourceUrl
            }],
            mediaStream.textTracks
        );
    }
    if (window.location.href.indexOf("poster=0") == -1) {
        if (mediaStream.thumbnails != null && mediaStream.thumbnails.length > 0) {
            mediaPlayer.poster(mediaStream.thumbnails[0]);
        }
    }
}