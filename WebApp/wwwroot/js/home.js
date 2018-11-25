var _mediaStreams, _streamNumber;
function CreateStreamTuner() {
    var streamTunerStep = GetStreamTunerStep();
    var streamTunerWidth = $("#streamTuner").width();
    $("#streamTuner").slider({
        min: 0,
        max: streamTunerWidth,
        step: streamTunerStep,
        slide: function (event, ui) {
            if (_mediaStreams.length > 0) {
                var streamNumber = GetStreamNumber(ui.value, streamTunerStep);
                var mediaStream = _mediaStreams[streamNumber - 1];
                var streamName = GetStreamName(mediaStream, true);
                var adjustTipX = ui.value - (streamTunerWidth / 2) + 1;
                var adjustTipY = -15;
                SetStreamNumber(streamNumber);
                CreateTipTop("streamTuner", streamName, adjustTipX, adjustTipY);
                SetTipVisible("streamTuner", true);
            }
        },
        stop: function (event, ui) {
            if (_mediaStreams.length > 0) {
                _streamNumber = GetStreamNumber(ui.value, streamTunerStep);
                SetMediaStream(false, false);
            }
        }
    });
}
function GetStreamIndex(streamNumber) {
    return (streamNumber - 1) % _streamTunerPageSize;
}
function GetStreamNumber(streamTunerValue, streamTunerStep) {
    return _mediaStreams.length == 1 ? 1 : (streamTunerValue / streamTunerStep) + 1;
}
function GetStreamName(mediaStream, streamTuner) {
    var streamName = mediaStream.name;
    if (mediaStream.source.protectionInfo != null && mediaStream.source.protectionInfo.length > 0) {
        var lineBreak = streamTuner ? "<br><br>" : "<br>";
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
function GetStreamTunerStep() {
    var streamTunerStep = $("#streamTuner").width();
    if (_mediaStreams.length > 1) {
        streamTunerStep = Math.floor(streamTunerStep / (_mediaStreams.length - 1));
    }
    return streamTunerStep;
}
function SetStreamTunerValue() {
    var streamIndex = GetStreamIndex(_streamNumber);
    var streamTunerStep = GetStreamTunerStep();
    var streamTunerValue = streamIndex * streamTunerStep;
    $("#streamTuner").slider({
        value: streamTunerValue
    });
}
function SetStreamTunerPage(streamTunerLeft, streamTunerRight) {
    var streamTunerPageChange = false;
    var streamIndex = GetStreamIndex(_streamNumber);
    if (streamTunerLeft && _streamNumber > 1) {
        _streamNumber = _streamNumber - 1;
        if (streamIndex == 0) {
            window.location.href = "/?stream=" + _streamNumber;
            streamTunerPageChange = true;
        }
    } else if (streamTunerRight) {
        if (_streamTunerLastPage) {
            if (streamIndex < _mediaStreams.length - 1) {
                _streamNumber = _streamNumber + 1;
            }
        } else {
            _streamNumber = _streamNumber + 1;
            if (streamIndex == _mediaStreams.length - 1) {
                window.location.href = "/?stream=" + _streamNumber;
                streamTunerPageChange = true;
            }
        }
    }
    return streamTunerPageChange;
}
function SetStreamNumber(streamNumber) {
    var streamIndex = GetStreamIndex(streamNumber);
    var streamTunerHandle = $(".ui-slider-handle")[0];
    if (streamIndex == 0 || streamIndex == _mediaStreams.length - 1) {
        streamTunerHandle.innerText = "";
    } else {
        streamTunerHandle.innerText = streamNumber;
    }
}
function SetMediaStream(streamTunerLeft, streamTunerRight) {
    if (_mediaStreams.length > 0) {
        var streamTunerPageChange = false;
        if (streamTunerLeft || streamTunerRight) {
            streamTunerPageChange = SetStreamTunerPage(streamTunerLeft, streamTunerRight);
        }
        if (!streamTunerPageChange) {
            var mediaStream = GetMediaStream();
            var streamName = GetStreamName(mediaStream, false);
            $("#streamName").html(streamName);
            $("#streamUrl").html("");
            SetStreamTunerValue();
            SetStreamNumber(_streamNumber);
            SetPlayerContent(_mediaPlayer, mediaStream);
        }
    }
}
function GetMediaStream() {
    var streamIndex = GetStreamIndex(_streamNumber);
    return _mediaStreams.length == 0 ? null : _mediaStreams[streamIndex];
}