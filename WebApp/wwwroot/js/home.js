function SetStreamSlider(slideDirection) {
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
                streamName = streamName + ", ";
            }
            streamName = streamName + mediaStream.source.protectionInfo[i].type;
        }
    }
    return streamName;
}
function ToggleLiveStream(button) {
    var buttonImage = button.children[0];
    if (buttonImage.src.indexOf("LiveStreamOn") > -1) {
        buttonImage.src = buttonImage.src.replace("On", "Off");
        window.location.href = "/?live=off";
    } else {
        buttonImage.src = buttonImage.src.replace("Off", "On");
        window.location.href = "/?live=on";
    }
}
function StartStreamingEndpoint(title) {
    var message = "Your media account streaming endpoint is not running.<br><br>Do you want to start your streaming endpoint now?"
    var buttons = {
        Yes: function () {
            $(this).dialog("close");
            SetCursor(true);
            $.post("/home/endpoint", { },
                function (endpointName) {
                    SetCursor(false);
                    if (endpointName != "") {
                        message = "A request to start your " + endpointName + " streaming endpoint has been submitted.";
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