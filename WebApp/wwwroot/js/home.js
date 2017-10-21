function GetStreamName(mediaStream, streamSlider) {
    var streamName = mediaStream.name;
    if (!streamSlider) {
        streamName = streamName.replace("<br><br>", " ");
    }
    if (mediaStream.contentProtection.length > 0) {
        var lineBreak = streamSlider ? "<br><br>" : "<br>";
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
function ToggleLiveStream(liveButton) {
    var buttonImage = liveButton.children[0];
    if (buttonImage.src.indexOf("LiveStreamOn") > -1) {
        buttonImage.src = buttonImage.src.replace("On", "Off");
        window.location.href = "/?live=off";
    } else {
        buttonImage.src = buttonImage.src.replace("Off", "On");
        window.location.href = "/?live=on";
    }
}
function StartStreamingEndpoint(title) {
    var message = "Your media account does not have a streaming endpoint running.<br><br>Do you want to start your streaming endpoint?"
    var buttons = {
        Yes: function () {
            $(this).dialog("close");
            $.post("/home/endpoint", { },
                function (endpointName) {
                    if (endpointName != "") {
                        message = "A request to start the " + endpointName + " streaming endpoint has been submitted.";
                        DisplayMessage(title, message);
                    }
                }
            );
        },
        No: function () {
            $(this).dialog("close");
        }
    };
    DisplayMessage(title, message, buttons);
}