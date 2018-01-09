function GetIndexerInsight(mediaStream, videoIndexer) {
    var insight = null;
    for (var i = 0; i < mediaStream.contentInsight.length; i++) {
        var processorName = mediaStream.contentInsight[i].processorName;
        if (processorName == videoIndexer) {
            insight = mediaStream.contentInsight[i];
        }
    }
    return insight;
}
function ToggleIndexerInsight(indexerInsight) {
    var insightImage = document.getElementById("insightImage");
    if ($("#indexerInsight").is(":visible")) {
        insightImage.src = insightImage.src.replace("Close", "Open");
        $("#indexerInsight").hide();
    } else {
        var indexerFrame = document.getElementById("indexerFrame");
        indexerFrame.src = indexerInsight.sourceUrl;
        insightImage.src = insightImage.src.replace("Open", "Close");
        $("#indexerInsight").show();
    }
}
function ToggleAnalyticsInsight(mediaStream) {
    var analyticsImage = document.getElementById("analyticsImage");
    if ($("#analyticsInsight").is(":visible")) {
        analyticsImage.src = analyticsImage.src.replace("Close", "Open");
        $("#analyticsInsight").hide();
    } else {
        analyticsImage.src = analyticsImage.src.replace("Open", "Close");
        $("#analyticsInsight").show();
    }
}
function SetAnalyticsProcessors(mediaStream) {
    var processors = document.getElementById("analyticsProcessors");
    processors.options.length = 0;
    processors.options[processors.options.length] = new Option("", "");
    for (var i = 0; i < mediaStream.contentInsight.length; i++) {
        var insight = mediaStream.contentInsight[i];
        var optionName = insight.processorName;
        var optionValue = insight.documentId;
        processors.options[processors.options.length] = new Option(optionName, optionValue);
    }
}
function SetAnalyticsMetadata() {
    $("#analyticsMetadata").empty();
    var mediaProcessor = $("#analyticsProcessors option:selected").text();
    if (mediaProcessor != "") {
        var mediaPlayer = GetMediaPlayer();
        mediaPlayer.pause();
        $.get("/insight/metadata",
            {
                mediaProcessor: mediaProcessor.replace(/\s+/g, ""),
                documentId: $("#analyticsProcessors").val(),
                timeSeconds: mediaPlayer.currentTime()
            },
            function (metadata) {
                $("#analyticsMetadata").jsonBrowse(metadata);
            }
        );
    }
}