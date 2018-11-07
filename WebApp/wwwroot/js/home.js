function ClearPlayerControls(controlBar, childIds) {
    for (var i = 0; i < childIds.length; i++) {
        var childId = childIds[i];
        var child = controlBar.children[childId];
        if (child != null) {
            controlBar.removeChild(child);
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
function SetPlayerControls(controlBar, storageCdnUrl) {
    var mediaStream = _mediaStreams[_streamNumber - 1];
    var buttonIds = ["searchButton", "insightButton", "markerButton"];
    ClearPlayerControls(controlBar, buttonIds);
    if (mediaStream.contentInsight.widgetUrl != null) {
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
        SetPlayerControl(controlBar, storageCdnUrl, "MediaInsightShow.png", "insightImage", "insightButton", onClick, "Cognitive<br><br>Insight");
    }
    if (_mediaPlayer.isLive()) {
        onClick = function () {
            alert("Marker");
        };
        SetPlayerControl(controlBar, storageCdnUrl, "MediaLiveMarker.png", "markerImage", "markerButton", onClick, "Insert<br><br>Marker");
    }
}
function SetMediaStream(streamTunerLeft, streamTunerRight) {
    var streamPageRefresh = false;
    var streamNumber = _streamNumber;
    if (_mediaStreams.length > 0) {
        var currentIndex = (streamNumber - 1) % _streamTunerPageSize;
        if (streamTunerLeft && streamNumber > 1) {
            streamNumber = streamNumber - 1;
            if (currentIndex == 0) {
                streamPageRefresh = true;
                window.location.href = "/?stream=" + streamNumber;
            }
        } else if (streamTunerRight) {
            if (_streamLastPage) {
                if (currentIndex < _mediaStreams.length - 1) {
                    streamNumber = streamNumber + 1;
                }
            } else {
                streamNumber = streamNumber + 1;
                if (currentIndex == _mediaStreams.length - 1) {
                    streamPageRefresh = true;
                    window.location.href = "/?stream=" + streamNumber;
                }
            }
        }
    }
    if (!streamPageRefresh) {
        var streamIndex = (streamNumber - 1) % _streamTunerPageSize;
        var mediaStream = _mediaStreams.length == 0 ? null : _mediaStreams[streamIndex];
        var streamTunerWidth = $("#streamTuner").width();
        var streamTunerStep = _mediaStreams.length == 1 ? streamTunerWidth : Math.floor(streamTunerWidth / (_mediaStreams.length - 1));
        var streamTunerValue = streamIndex * streamTunerStep;
        $("#streamTuner").slider({
            min: 0,
            max: streamTunerWidth,
            step: streamTunerStep,
            value: streamTunerValue,
            classes: {
                "ui-slider-handle": "streamTunerHandle"
            },
            slide: function (event, ui) {
                if (_mediaStreams.length > 0) {
                    var streamNumber = _mediaStreams.length == 1 ? 1 : (ui.value / streamTunerStep) + 1;
                    var streamIndex = streamNumber - 1;
                    var mediaStream = _mediaStreams[streamIndex];
                    var streamName = GetStreamName(mediaStream, true);
                    CreateTipTopLeft("streamTuner", streamName, ui.value + 2, -15);
                    SetTipVisible("streamTuner", true);
                    SetStreamNumber(streamNumber, streamIndex);
                }
            },
            stop: function (event, ui) {
                if (_mediaStreams.length > 0) {
                    _streamNumber = _mediaStreams.length == 1 ? 1 : (ui.value / streamTunerStep) + 1;
                    SetMediaStream(false, false);
                }
            }
        });
        if (mediaStream != null) {
            SetPlayerContent(_mediaPlayer, mediaStream);
            SetStreamNumber(streamNumber, streamIndex);
            var streamName = GetStreamName(mediaStream, false);
            $("#streamName").html(streamName);
            $("#streamUrl").html("");
            _streamNumber = streamNumber;
        }
    }
}
function SetStreamNumber(streamNumber, streamIndex) {
    var streamTunerHandle = document.getElementsByClassName("ui-slider-handle")[0];
    if (streamIndex == 0 || streamIndex == _mediaStreams.length - 1) {
        streamTunerHandle.innerText = "";
    } else {
        streamTunerHandle.innerText = streamNumber;
    }
}
function GetStreamName(mediaStream, streamTuner) {
    var streamName = mediaStream.name;
    var lineBreak = streamTuner ? "<br><br>" : "<br>";
    if (mediaStream.source.protectionInfo != null && mediaStream.source.protectionInfo.length > 0) {
        streamName = streamName + lineBreak + "(";
        for (var i = 0; i < mediaStream.source.protectionInfo.length; i++) {
            if (i > 0) {
                streamName = streamName + ", ";
            }
            streamName = streamName + mediaStream.source.protectionInfo[i].type;
        }
        streamName = streamName + ")";
    }
    return streamName;
}