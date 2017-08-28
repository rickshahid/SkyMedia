function GetStreamName(mediaStream, streamSlider) {
    var streamName = mediaStream.name;
    if (!streamSlider) {
        streamName = streamName.replace("<br /><br />", " ");
    }
    if (mediaStream.contentProtection.length > 0) {
        var lineBreak = streamSlider ? "<br /><br />" : "<br />";
        streamName = streamName + lineBreak + "(" + mediaStream.contentProtection.join(", ") + ")";
    }
    return streamName;
}
function SlideStreamNumber(slideDirection) {
    var streamNumber = _streamNumber;
    if (slideDirection == "left" && streamNumber > 1) {
        streamNumber = streamNumber - 1;
    } else if (slideDirection == "right" && streamNumber < _mediaStreams.length) {
        streamNumber = streamNumber + 1;
    }
    window.location.href = "/?stream=" + streamNumber;
}
function SetStreamNumber(streamNumber) {
    var sliderHandle = document.getElementsByClassName("ui-slider-handle")[0];
    if (streamNumber == 1 || streamNumber == _mediaStreams.length) {
        sliderHandle.innerText = "";
    } else {
        sliderHandle.innerText = streamNumber;
    }
}
function SetStreamName(mediaStream) {
    var streamName = GetStreamName(mediaStream, false);
    $("#streamName").html(streamName);
}
