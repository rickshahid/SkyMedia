function SetMediaStream(tunerLeft, tunerRight) {
    var streamPageRefresh = false;
    var streamNumber = _streamNumber;
    if (_mediaStreams.length > 0) {
        var currentIndex = (streamNumber - 1) % _tunerPageSize;
        if (tunerLeft && streamNumber > 1) {
            streamNumber = streamNumber - 1;
            if (currentIndex == 0) {
                streamPageRefresh = true;
                window.location.href = "/?stream=" + streamNumber;
            }
        } else if (tunerRight) {
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
        var streamIndex = (streamNumber - 1) % _tunerPageSize;
        var mediaStream = _mediaStreams[streamIndex];
        var tunerWidth = $("#streamTuner").width();
        var tunerStep = _mediaStreams.length == 1 ? tunerWidth : Math.floor(tunerWidth / (_mediaStreams.length - 1));
        var tunerValue = streamIndex * tunerStep;
        $("#streamTuner").slider({
            min: 0,
            max: tunerWidth,
            step: tunerStep,
            value: tunerValue,
            classes: {
                "ui-slider-handle": "streamTunerHandle"
            },
            slide: function (event, ui) {
                if (_mediaStreams.length > 0) {
                    var streamNumber = _mediaStreams.length == 1 ? 1 : (ui.value / tunerStep) + 1;
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
                    _streamNumber = _mediaStreams.length == 1 ? 1 : (ui.value / tunerStep) + 1;
                    SetMediaStream(false, false);
                }
            }
        });
        SetPlayerContent(_mediaPlayer, mediaStream);
        SetStreamNumber(streamNumber, streamIndex);
        var streamName = GetStreamName(mediaStream, false);
        $("#streamName").html(streamName);
        $("#streamUrl").html("");
        _streamNumber = streamNumber;
    }
}
function SetStreamNumber(streamNumber, streamIndex) {
    var tunerHandle = document.getElementsByClassName("ui-slider-handle")[0];
    if (streamIndex == 0 || streamIndex == _mediaStreams.length - 1) {
        tunerHandle.innerText = "";
    } else {
        tunerHandle.innerText = streamNumber;
    }
}
function GetStreamName(mediaStream, streamTuner) {
    var streamName = mediaStream.name;
    var lineBreak = streamTuner ? "<br><br>" : "<br>";
    streamName = streamName.replace(" - ", lineBreak);
    if (mediaStream.source.protectionInfo.length > 0) {
        streamName = streamName + lineBreak;
        for (var i = 0; i < mediaStream.source.protectionInfo.length; i++) {
            if (i > 0) {
                streamName = streamName + ", ";
            }
            streamName = streamName + mediaStream.source.protectionInfo[i].type;
        }
    }
    return streamName;
}