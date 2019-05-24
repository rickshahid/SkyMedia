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
            if (mediaStream != null && mediaStream.insight != null) {
                _insightWidgetUrl = mediaStream.insight.widgetUrl;
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
    if (mediaStream.protection != null && mediaStream.protection.length > 0) {
        if (window.location.href.indexOf("token=0") > -1) {
            for (var i = 0; i < mediaStream.protection.length; i++) {
                mediaStream.protection[i].authenticationToken = null;
            }
        }
        mediaPlayer.src(
            [{
                src: mediaStream.url,
                protectionInfo: mediaStream.protection
            }],
            mediaStream.tracks
        );
    } else {
        mediaPlayer.src(
            [{
                src: mediaStream.url
            }],
            mediaStream.tracks
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
    var buttonIds = ["insightButton", "signalButton"];
    ClearPlayerControls(controlBar, buttonIds);
    if (_insightWidgetUrl != null) {
        CreatePlayerControl(controlBar, storageCdnUrl, ToggleMediaInsight, "insightButton", "insightImage", "MediaInsightShow.png", "Media Insight<br><br>Toggle");
    }
    if (liveEncoding) {
        CreatePlayerControl(controlBar, storageCdnUrl, InsertAdSignal, "signalButton", "signalImage", "MediaLiveSignal.png", "Insert<br><br>Ad Signal");
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
    var title = "Insert Advertising Signal";
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