function SetAnalyticsProcessors(mediaStream) {
    var processors = document.getElementById("analyticsProcessors");
    processors.options.length = 0;
    processors.options[processors.options.length]= new Option("", "");
    for (var i = 0; i < mediaStream.analyticsMetadata.length; i++) {
        var analyticsMetadata = mediaStream.analyticsMetadata[i];
        var optionName = analyticsMetadata.processorName;
        var optionValue = analyticsMetadata.sourceUrl != null ? analyticsMetadata.sourceUrl : analyticsMetadata.documentId;
        processors.options[processors.options.length]= new Option(optionName, optionValue);
    }
    $("#mediaMetadata").empty();
}
function GetFragmentEvent(timeSeconds, timescale, fragment) {
    var ticksPerMillisecond = timescale / 1000;
    var millisecondsPerInterval = fragment.interval / ticksPerMillisecond;
    var millisecondsOffset = (timeSeconds - (fragment.start / timescale)) * 1000;
    var eventIndex = Math.floor(millisecondsOffset / millisecondsPerInterval);
    return fragment["events"][eventIndex];
}
function GetFaceEmotion(faceScores) {
    var faceScore = 0, faceEmotion;
    if (faceScores != null) {
        var keys = Object.keys(faceScores);
        for (var i = 0; i < keys.length; i++) {
            var key = keys[i];
            if (faceScores[key] > faceScore) {
                faceScore = faceScores[key];
                faceEmotion = key[0].toUpperCase() + key.slice(1);
            }
        }
    }
    return faceEmotion;
}
function GetFacesMetadata(playerHeight, playerWidth, timeSeconds, mediaMetadata) {
    var facesMetadata = new Array();
    var fragment = mediaMetadata.fragment
    if (fragment.events != null) {
        var frameHeight = mediaMetadata.height;
        var frameWidth = mediaMetadata.width;
        var scaleHeight = playerHeight / frameHeight;
        var scaleWidth = playerWidth / frameWidth;
        var fragmentEvent = GetFragmentEvent(timeSeconds, mediaMetadata.timescale, fragment);
        for (var i = 0; i < fragmentEvent.length; i++) {
            var faceMetadata = {};
            faceMetadata.id = fragmentEvent[i].id;
            faceMetadata.label = "Face " + faceMetadata.id;
            faceMetadata.x = fragmentEvent[i].x * frameWidth;
            faceMetadata.x = faceMetadata.x * scaleWidth;
            faceMetadata.y = fragmentEvent[i].y * frameHeight;
            faceMetadata.y = faceMetadata.y * scaleHeight;
            faceMetadata.height = fragmentEvent[i].height * frameHeight;
            faceMetadata.height = faceMetadata.height * scaleHeight;
            faceMetadata.width = fragmentEvent[i].width * frameWidth;
            faceMetadata.width = faceMetadata.width * scaleWidth;
            faceMetadata.emotion = GetFaceEmotion(fragmentEvent[i]["scores"]);
            facesMetadata.push(faceMetadata);
        }
    }
    return facesMetadata;
}
function SetMediaMetadata() {
    var mediaPlayer = GetMediaPlayer(false);
    var playerHeight = mediaPlayer.height();
    var playerWidth = mediaPlayer.width();
    var timeSeconds = mediaPlayer.currentTime();
    var selectedText = $("#analyticsProcessors option:selected").text();
    var selectedValue = $("#analyticsProcessors").val();
    var jsonOptions = {
        collapsed: false,
        withQuotes: false
    };
    $("#mediaTranscript").hide();
    $("#mediaMetadata").hide();
    switch (selectedText) {
        case "Indexer":
            $("#mediaTranscript").show();
            $.get(selectedValue, function (result) {
                $("#mediaTranscript").text(result);
            });
            break;
        case "Face Detection":
            $("#mediaMetadata").show();
            $.post("/analytics/metadata",
                {
                    documentId: selectedValue,
                    timeSeconds: timeSeconds
                },
                function (result) {
                    $("#mediaMetadata").jsonBrowse(result, jsonOptions);
                    var facesMetadata = GetFacesMetadata(playerHeight, playerWidth, timeSeconds, result);
                    for (var i = 0; i < facesMetadata.length; i++) {
                        var faceMetadata = facesMetadata[i];
                        SetVideoOverlay(faceMetadata);
                    }
                }
            );
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
function ResetMediaMetadata() {
    ClearVideoOverlay();
    //SetMediaMetadata();
}
