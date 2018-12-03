var _mediaStreams, _streamNumber;
function CreateStreamTuner() {
    $("#streamTuner").slider({
        min: GetStreamTunerMin(),
        max: GetStreamTunerMax(),
        slide: function (event, ui) {
            if (_mediaStreams.length > 0) {
                var streamNumber = ui.value;
                var streamIndex = GetStreamIndex(streamNumber);
                var mediaStream = _mediaStreams[streamIndex];
                var streamName = GetStreamName(mediaStream, true);
                var adjustTipX = GetLeftOffset("streamTuner", streamNumber);
                var adjustTipY = -15;
                SetStreamNumber(streamNumber);
                CreateTipTop("streamTuner", streamName, adjustTipX, adjustTipY);
                SetTipVisible("streamTuner", true);
            }
        },
        stop: function (event, ui) {
            if (_mediaStreams.length > 0) {
                _streamNumber = ui.value;
                SetMediaStream(false, false);
            }
        }
    });
}
function GetStreamTunerMin() {
    var minValue = 0;
    if (_mediaStreams.length > 0) {
        var pageCount = Math.floor(_streamNumber / _streamTunerPageSize);
        minValue = (pageCount * _streamTunerPageSize) + 1;
    }
    return minValue;
}
function GetStreamTunerMax() {
    var maxValue = 0;
    if (_mediaStreams.length > 0) {
        var pageCount = Math.floor(_streamNumber / _streamTunerPageSize);
        maxValue = (pageCount * _streamTunerPageSize) + _mediaStreams.length;
    }
    return maxValue;
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
    if (streamNumber == null) {
        streamNumber = $("#streamTuner").slider("value");
    } else {
        $("#streamTuner").slider({
            value: streamNumber
        });
    }
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
            SetStreamNumber(_streamNumber);
            SetPlayerContent(_mediaPlayer, mediaStream);
        }
    }
}
function GetMediaStream() {
    var streamIndex = GetStreamIndex(_streamNumber);
    return _mediaStreams.length == 0 ? null : _mediaStreams[streamIndex];
}