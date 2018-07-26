﻿function SetStreamTuner(slideForward) {
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