var _mediaPlayer;
function GetMediaPlayer(playerId, userId, accountName, autoPlay, galleryView, spriteVttUrl) {
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
function SetPlayerEvents(mediaPlayer, storageCdnUrl, liveEncoding, homePage) {
    mediaPlayer.ready(function () {
        if (homePage) {
            this.videobreakdown({
                syncTranscript: true,
                syncLanguage: true
            });
        }
    });
    mediaPlayer.addEventListener(amp.eventName.loadeddata, function () {
        var mediaStream = GetMediaStream();
        SetPlayerControls(mediaStream, storageCdnUrl, liveEncoding);
    });
    mediaPlayer.addEventListener(amp.eventName.play, function () {
        var streamUrl = mediaPlayer.currentSrc();
        streamUrl = streamUrl.replace("http:", "");
        streamUrl = streamUrl.split("/");
        for (var i = streamUrl.length - 1; i > 1; i--) {
            streamUrl[i] = streamUrl[i] + "<br>";
        }
        streamUrl = streamUrl.join("/");
        $("#streamUrl").html(streamUrl);
    });
}
function SetPlayerContent(mediaPlayer, mediaStream) {
    var sourceUrl = mediaStream.source.src;
    if (sourceUrl.indexOf("/manifest") > -1) {
        sourceUrl = sourceUrl + "(format=mpd-time-cmaf)";
    }
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
function SetPlayerControl(controlBar, storageCdnUrl, imageFile, imageId, buttonId, onClick, tipText) {
    var image = document.createElement("img");
    image.id = imageId;
    image.src = storageCdnUrl + "/" + imageFile;
    var button = document.createElement("button");
    button.id = buttonId;
    button.className = "vjs-control vjs-button mediaPlayerButton";
    button.appendChild(image);
    controlBar.appendChild(button);
    $("#" + buttonId).click(onClick);
    CreateTipTop(buttonId, tipText);
}
function SetPlayerControls(mediaStream, storageCdnUrl, liveEncoding) {
    var controlBar = $(".amp-controlbaricons-right")[0];
    var buttonIds = ["insightButton", "signalButton"];
    ClearPlayerControls(controlBar, buttonIds);
    if (mediaStream != null && mediaStream.contentInsight != null && mediaStream.contentInsight.widgetUrl != null) {
        onClick = function () {
            var imageSource = $("#insightImage").prop("src");
            if ($("#indexerInsight").is(":visible")) {
                $("#indexerInsight").hide();
                $(".layoutPanel.side").show();
                $("#insightImage").prop("src", imageSource.replace("Hide", "Show"));
            } else {
                var playerHeight = $("#videoPlayer video").height();
                $("#indexerInsight").height(playerHeight);
                $("#indexerInsight").prop("src", mediaStream.contentInsight.widgetUrl);
                $("#indexerInsight").show();
                $(".layoutPanel.side").hide();
                $("#insightImage").prop("src", imageSource.replace("Show", "Hide"));
            }
        };
        SetPlayerControl(controlBar, storageCdnUrl, "MediaInsightShow.png", "insightImage", "insightButton", onClick, "Media<br><br>Insight");
    }
    if (liveEncoding) {
        onClick = function () {
            alert("Insert Ad Signal");
        };
        SetPlayerControl(controlBar, storageCdnUrl, "MediaLiveSignal.png", "signalImage", "signalButton", onClick, "Insert<br><br>Ad Signal");
    }
}
function ClearPlayerControls(controlBar, childIds) {
    for (var i = 0; i < childIds.length; i++) {
        var childId = childIds[i];
        var child = controlBar.children[childId];
        if (child != null) {
            controlBar.removeChild(child);
        }
    }
}