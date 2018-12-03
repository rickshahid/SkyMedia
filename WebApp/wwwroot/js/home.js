var _mediaStreams, _streamNumber;
function CreateStreamTuner() {
    $("#streamTuner").slider({
        min: _mediaStreams.length == 0 ? 0 : 1,
        max: _mediaStreams.length < _streamTunerPageSize ? _mediaStreams.length : _streamTunerPageSize,
        slide: function (event, ui) {
            if (_mediaStreams.length > 0) {
                var streamNumber = GetStreamNumber(ui.value);
                var streamIndex = GetStreamIndex(streamNumber);
                var mediaStream = _mediaStreams[streamIndex];
                var streamName = GetStreamName(mediaStream, true);
                var adjustTipX = GetLeftOffset("streamTuner", ui.value);
                var adjustTipY = -15;
                SetStreamNumber(streamNumber);
                CreateTipTop("streamTuner", streamName, adjustTipX, adjustTipY);
                SetTipVisible("streamTuner", true);
            }
        },
        stop: function (event, ui) {
            if (_mediaStreams.length > 0) {
                _streamNumber = GetStreamNumber(ui.value);
                SetMediaStream(false, false);
            }
        }
    });
}
function GetStreamNumber(streamTunerValue) {
    var streamPage = Math.floor(_streamNumber / _streamTunerPageSize);
    return (streamPage * _streamTunerPageSize) + streamTunerValue;
}
function GetStreamIndex(streamNumber) {
    return (streamNumber - 1) % _streamTunerPageSize;
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
    SetSliderValue("streamTuner", "streamNumber", streamNumber);
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
            $("#streamTuner").slider({
                value: _streamNumber
            });
            SetStreamNumber(_streamNumber);
            SetPlayerContent(_mediaPlayer, mediaStream);
        }
    }
}
function GetMediaStream() {
    var streamIndex = GetStreamIndex(_streamNumber);
    return _mediaStreams.length == 0 ? null : _mediaStreams[streamIndex];
}