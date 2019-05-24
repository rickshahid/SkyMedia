var _mediaStreams, _streamNumber;
function CreateStreamTuner() {
    $("#streamTuner").slider({
        min: GetStreamTunerMinValue(),
        max: GetStreamTunerMaxValue(),
        slide: function (event, ui) {
            if (_mediaStreams.length > 0) {
                var streamNumber = ui.value;
                var mediaStream = GetMediaStream(streamNumber);
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
function GetStreamTunerMinValue() {
    var minValue = 0;
    if (_mediaStreams.length > 0) {
        var pageIndex = GetStreamTunerPageIndex();
        minValue = (pageIndex * _streamTunerPageSize) + 1;
    }
    return minValue;
}
function GetStreamTunerMaxValue() {
    var maxValue = 0;
    if (_mediaStreams.length > 0) {
        var pageIndex = GetStreamTunerPageIndex();
        maxValue = (pageIndex * _streamTunerPageSize) + _mediaStreams.length;
    }
    return maxValue;
}
function GetStreamTunerPageIndex() {
    var streamNumber = _streamNumber - 1;
    return Math.floor(streamNumber / _streamTunerPageSize);
}
function GetStreamName(mediaStream, streamTuner) {
    var streamName = mediaStream.name;
    if (mediaStream.protection != null && mediaStream.protection.length > 0) {
        var lineBreak = streamTuner ? "<br><br>" : "<br>";
        streamName = streamName + lineBreak;
        for (var i = 0; i < mediaStream.protection.length; i++) {
            if (i > 0) {
                streamName = streamName + ", ";
            }
            streamName = streamName + mediaStream.protection[i].type;
        }
    }
    return streamName;
}
function SetStreamTunerPage(streamTunerLeft, streamTunerRight) {
    var streamTunerPageChange = false;
    if (streamTunerLeft && _streamNumber > 1) {
        var minValue = $("#streamTuner").slider("option", "min");
        _streamNumber = _streamNumber - 1;
        if (_streamNumber < minValue) {
            window.location.href = "/?stream=" + _streamNumber;
            streamTunerPageChange = true;
        }
    } else if (streamTunerRight) {
        var maxValue = $("#streamTuner").slider("option", "max");
        if (_streamTunerLastPage) {
            if (_streamNumber < maxValue) {
                _streamNumber = _streamNumber + 1;
            }
        } else {
            _streamNumber = _streamNumber + 1;
            if (_streamNumber > maxValue) {
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
        $("#streamTuner").slider("value", streamNumber);
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
            var mediaStream = GetMediaStream(null);
            var streamName = GetStreamName(mediaStream, false);
            $("#streamName").html(streamName);
            $("#streamUrl").html("");
            SetStreamNumber(_streamNumber);
            SetPlayerContent(_mediaPlayer, mediaStream);
            if (mediaStream.poster != null) {
                $("#playerPoster").show();
            } else {
                $("#playerPoster").hide();
            }
        }
    }
}
function GetMediaStream(streamNumber) {
    if (streamNumber == null) {
        streamNumber = _streamNumber;
    }
    var streamIndex = (streamNumber - 1) % _streamTunerPageSize;
    return _mediaStreams.length == 0 ? null : _mediaStreams[streamIndex];
}
function SetPlayerPoster(checkbox) {
    var posterUrl = null;
    if (checkbox.checked) {
        posterUrl = _mediaStreams[_streamNumber - 1].poster;
    }
    _mediaPlayer.poster(posterUrl);
}