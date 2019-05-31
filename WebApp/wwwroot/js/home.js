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
    var pageIndex = 0;
    if (_streamTunerPageSize > 0) {
        pageIndex = Math.floor(_streamNumber - 1 / _streamTunerPageSize);
    }
    return pageIndex;
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
function SetStreamFilter(select) {
    var mediaStream = GetMediaStream(null);
    if (select.value != "") {
        mediaStream = $.extend({}, mediaStream);
        mediaStream.url = mediaStream.url.slice(0, -1) + ",filter=" + select.value + ")";
    }
    $("#streamUrl").html("");
    SetPlayerContent(_mediaPlayer, mediaStream);
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
            if (mediaStream.filters != null) {
                if (mediaStream.filters.length == 0) {
                    $("#streamFilters").hide();
                } else {
                    $("#streamFilter").empty();
                    $("#streamFilter").append("<option value=''></option>");
                    for (var i = 0; i < mediaStream.filters.length; i++) {
                        var optionValue = mediaStream.filters[i];
                        $("#streamFilter").append("<option value='" + optionValue + "'>" + optionValue + "</option>");
                    }
                    $("#streamFilters").show();
                }
            }
        }
    }
}
function GetMediaStream(streamNumber) {
    if (streamNumber == null) {
        streamNumber = _streamNumber;
    }
    var streamIndex = streamNumber - 1;
    if (_streamTunerPageSize > 0) {
        streamIndex = streamIndex % _streamTunerPageSize;
    }
    return _mediaStreams.length == 0 ? null : _mediaStreams[streamIndex];
}