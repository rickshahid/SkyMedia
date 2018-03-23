function SetStreamTuner(slideForward) {
    var streamNumber = _streamNumber;
    if (_mediaStreams.length > 0) {
        if (slideForward) {
            streamNumber = streamNumber + 1;
        } else if (streamNumber > 1) {
            streamNumber = streamNumber - 1;
        }
    }
    if (streamNumber != _streamNumber) {
        window.location.href = "/?stream=" + streamNumber;
    }
}
function SetStreamNumber(streamNumber, streamIndex) {
    var sliderHandle = document.getElementsByClassName("ui-slider-handle")[0];
    if (streamIndex == 0 || streamIndex == _mediaStreams.length - 1) {
        sliderHandle.innerText = "";
    } else {
        sliderHandle.innerText = streamNumber;
    }
}
function GetStreamName(mediaStream, streamSlider) {
    var streamName = mediaStream.name;
    if (!streamSlider) {
        streamName = streamName.replace("<br><br>", " ");
    }
    if (mediaStream.source.protectionInfo.length > 0) {
        if (streamSlider) {
            streamName = streamName + "<br><br>";
        }
        for (var i = 0; i < mediaStream.source.protectionInfo.length; i++) {
            if (!streamSlider || i > 0) {
                streamName = streamName + " + ";
            }
            streamName = streamName + mediaStream.source.protectionInfo[i].type;
        }
    }
    return streamName;
}