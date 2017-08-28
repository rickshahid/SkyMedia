function GetFragmentEvent(fragment, timeScale, timeSeconds) {
    var ticksPerMillisecond = timeScale / 1000;
    var millisecondsPerInterval = fragment.interval / ticksPerMillisecond;
    var millisecondsOffset = (timeSeconds - (fragment.start / timeScale)) * 1000;
    var eventIndex = Math.floor(millisecondsOffset / millisecondsPerInterval);
    return fragment["events"][eventIndex];
}
function GetFaceEmotion(emotionScores) {
    var faceEmotion, emotionScore = 0;
    var emotionKeys = Object.keys(emotionScores);
    for (var i = 0; i < emotionKeys.length; i++) {
        var key = emotionKeys[i];
        if (emotionScores[key] > emotionScore) {
            emotionScore = emotionScores[key];
            faceEmotion = key[0].toUpperCase() + key.slice(1);
        }
    }
    return faceEmotion;
}
function GetFacesMetadata(mediaMetadata, playerHeight, playerWidth, timeSeconds) {
    var facesMetadata = new Array();
    var fragment = mediaMetadata.fragment
    if (fragment.events != null) {
        var frameHeight = mediaMetadata.height;
        var frameWidth = mediaMetadata.width;
        var scaleHeight = playerHeight / frameHeight;
        var scaleWidth = playerWidth / frameWidth;
        var fragmentEvent = GetFragmentEvent(fragment, mediaMetadata.timescale, timeSeconds);
        for (var i = 0; i < fragmentEvent.length; i++) {
            var faceMetadata = {};
            faceMetadata.id = fragmentEvent[i].id;
            faceMetadata.x = fragmentEvent[i].x * frameWidth;
            faceMetadata.x = faceMetadata.x * scaleWidth;
            faceMetadata.y = fragmentEvent[i].y * frameHeight;
            faceMetadata.y = faceMetadata.y * scaleHeight;
            faceMetadata.height = fragmentEvent[i].height * frameHeight;
            faceMetadata.height = faceMetadata.height * scaleHeight;
            faceMetadata.width = fragmentEvent[i].width * frameWidth;
            faceMetadata.width = faceMetadata.width * scaleWidth;
            faceMetadata.label = "Face " + faceMetadata.id;
            faceMetadata.emotion = GetFaceEmotion(fragmentEvent[i]["scores"]);
            facesMetadata.push(faceMetadata);
        }
    }
    return facesMetadata;
}
function SetAnalyticsProcessors(mediaStream) {
    var processors = document.getElementById("analyticsProcessors");
    processors.options.length = 0;
    processors.options[processors.options.length] = new Option("", "");
    for (var i = 0; i < mediaStream.analyticsMetadata.length; i++) {
        var metadata = mediaStream.analyticsMetadata[i];
        var optionName = metadata.processor;
        var optionValue = metadata.sourceUrl != null ? metadata.sourceUrl : metadata.documentId;
        processors.options[processors.options.length] = new Option(optionName, optionValue);
    }
}
function SetAnalyticsMetadata() {
    $("#mediaMetadata").empty();
    var selectedText = $("#analyticsProcessors option:selected").text();
    if (selectedText != "") {
        var selectedValue = $("#analyticsProcessors").val();
        var mediaPlayer = GetMediaPlayer(false);
        var playerHeight = mediaPlayer.el().clientHeight;
        var playerWidth = mediaPlayer.el().clientWidth;
        var timeSeconds = mediaPlayer.currentTime();
        var jsonOptions = {
            collapsed: false,
            withQuotes: false
        };
        switch (selectedText) {
            case "Video Indexer":
                $.get("/asset/metadata",
                    {
                        documentId: selectedValue
                    },
                    function (result) {
                        $("#mediaMetadata").jsonBrowse(result, jsonOptions);
                    }
                );
                break;
            case "Face Detection":
                $.get("/asset/metadata",
                    {
                        documentId: selectedValue,
                        timeSeconds: timeSeconds
                    },
                    function (result) {
                        $("#mediaMetadata").jsonBrowse(result, jsonOptions);
                        _facesMetadata = GetFacesMetadata(result, playerHeight, playerWidth, timeSeconds);
                        for (var i = 0; i < _facesMetadata.length; i++) {
                            var faceMetadata = _facesMetadata[i];
                            SetVideoOverlay(faceMetadata);
                        }
                    }
                );
        }
    }
}
function SetVideoOverlay(faceMetadata) {
    $("<div id='videoOverlay" + faceMetadata.id + "' class='mediaOverlay'></div>").insertBefore("#videoPlayer");
    var playerPosition = $("#videoPlayer").position();
    $("#videoOverlay" + faceMetadata.id).css({
        top: (playerPosition.top + faceMetadata.y) + "px",
        left: (playerPosition.left + faceMetadata.x) + "px",
        height: faceMetadata.height + "px",
        width: faceMetadata.width + "px"
    });
    $("#videoOverlay" + faceMetadata.id).show();
    var faceLabel = faceMetadata.label;
    if (faceMetadata.emotion != null) {
        faceLabel = faceLabel + "<br /><br />(" + faceMetadata.emotion + ")";
    }
    CreateTipTop("videoOverlay" + faceMetadata.id, faceLabel, 0, 0, false);
    SetTipVisible("videoOverlay" + faceMetadata.id, true);
}
function ClearVideoOverlay() {
    var videoOverlays = $("div[id^='videoOverlay']");
    for (var i = 0; i < videoOverlays.length; i++) {
        var videoOverlay = videoOverlays[i];
        SetTipVisible(videoOverlay.id, false);
        $("#" + videoOverlay.id).remove();
    }
}
function ResetVideoOverlay() {
    ClearVideoOverlay();
    if (_facesMetadata != null) {
        for (var i = 0; i < _facesMetadata.length; i++) {
            var faceMetadata = _facesMetadata[i];
            SetVideoOverlay(faceMetadata);
        }
    }
}
