var _mediaPlayer, _liveEventName, _insightWidgetUrl;
function GetMediaPlayer(playerId, userId, accountName, autoPlay, galleryView, lowLatency, spriteVttUrl) {
    var playerOptions = {
        fluid: true,
        controls: true,
        autoplay: autoPlay,
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
    if (lowLatency) {
        playerOptions.heuristicProfile = "LowLatency";
    }
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
        _insightWidgetUrl = null;
        if (homePage) {
            var mediaStream = GetMediaStream(null);
            if (mediaStream != null && mediaStream.contentInsight != null) {
                _insightWidgetUrl = mediaStream.contentInsight.widgetUrl;
            }
        }
        CreatePlayerControls(storageCdnUrl, liveEncoding);
    });
    mediaPlayer.addEventListener(amp.eventName.play, function () {
        var streamUrl = mediaPlayer.currentSrc();
        streamUrl = streamUrl.replace("http:", "");
        streamUrl = streamUrl.replace("https:", "");
        streamUrl = streamUrl.split("/");
        for (var i = 0; i < 4; i++) {
            var urlIndex = streamUrl.length - 1 - i;
            streamUrl[urlIndex] = streamUrl[urlIndex] + "<br>";
        }
        streamUrl = streamUrl.join("/");
        $("#streamUrl").html(streamUrl);
    });
}
function SetPlayerContent(mediaPlayer, mediaStream) {
    var sourceUrl = mediaStream.source.src;
    var textTracks = mediaStream.textTracks;
    var protectionInfo = mediaStream.source.protectionInfo;
    SetPlayerSource(mediaPlayer, sourceUrl, textTracks, protectionInfo);
    if (window.location.href.indexOf("poster=0") == -1) {
        if (mediaStream.thumbnails != null && mediaStream.thumbnails.length > 0) {
            mediaPlayer.poster(mediaStream.thumbnails[0]);
        }
    }
}
function SetPlayerSource(mediaPlayer, sourceUrl, textTracks, protectionInfo) {
    if (sourceUrl.indexOf("/manifest") > -1) {
        sourceUrl = sourceUrl + "(format=mpd-time-cmaf)";
    }
    if (protectionInfo != null && protectionInfo.length > 0) {
        if (window.location.href.indexOf("token=0") > -1) {
            for (var i = 0; i < protectionInfo.length; i++) {
                protectionInfo[i].authenticationToken = null;
            }
        }
        mediaPlayer.src(
            [{
                src: sourceUrl,
                protectionInfo: protectionInfo
            }],
            textTracks
        );
    } else {
        mediaPlayer.src(
            [{
                src: sourceUrl
            }],
            textTracks
        );
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
function CreatePlayerControls(storageCdnUrl, liveEncoding) {
    var controlBar = $(".amp-controlbaricons-right")[0];
    var buttonIds = ["signalButton", "insightButton"];
    ClearPlayerControls(controlBar, buttonIds);
    if (liveEncoding) {
        CreatePlayerControl(controlBar, storageCdnUrl, InsertAdSignal, "signalButton", "signalImage", "MediaLiveSignal.png", "Insert<br><br>Ad Signal");
    }
    if (_insightWidgetUrl != null) {
        CreatePlayerControl(controlBar, storageCdnUrl, ToggleMediaInsight, "insightButton", "insightImage", "MediaInsightShow.png", "Media<br><br>Insight");
    }
}
function CreatePlayerControl(controlBar, storageCdnUrl, onClick, buttonId, imageId, imageFile, tipText) {
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
function ToggleMediaInsight() {
    var insightImageSource = $("#insightImage").prop("src");
    if ($("#indexerInsight").is(":visible")) {
        $("#indexerInsight").hide();
        $(".layoutPanel.side").show();
        $("#insightImage").prop("src", insightImageSource.replace("Hide", "Show"));
    } else {
        var playerHeight = $("#videoPlayer video").height();
        $("#indexerInsight").height(playerHeight);
        $("#indexerInsight").prop("src", _insightWidgetUrl);
        $("#indexerInsight").show();
        $(".layoutPanel.side").hide();
        $("#insightImage").prop("src", insightImageSource.replace("Show", "Hide"));
    }
}
function InsertAdSignal() {
    var title = "Confirm Insert Advertising Signal";
    var message = "Are you sure you want to insert an advertising signal?";
    message = message + "<br><br>Signal Id&nbsp;&nbsp;<input id='signalId' type='text' />";
    message = message + "<br><br>Signal Duration&nbsp;&nbsp;<input id='signalDuration' class='signalDuration' value='30' />&nbsp;Seconds";
    var buttons = {
        OK: function () {
            SetCursor(true);
            $.post("/live/insertSignal",
                {
                    eventName: _liveEventName,
                    signalId: $("#signalId").val(),
                    signalDurationSeconds: $("#signalDuration").val()
                },
                function () {
                    SetCursor(false);
                }
            );
            $(this).dialog("close");
        },
        Cancel: function () {
            $(this).dialog("close");
        }
    };
    DisplayMessage(title, message, buttons);
    $("#signalDuration").spinner({
        min: 1,
        max: 99
    });
    $("#signalId").focus();
}