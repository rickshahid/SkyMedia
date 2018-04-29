function GetIndexerInsight(contentInsight, videoIndexer) {
    var insightSource = null;
    for (var i = 0; i < contentInsight.sources.length; i++) {
        var mediaProcessor = contentInsight.sources[i].mediaProcessor;
        if (mediaProcessor == videoIndexer) {
            insightSource = contentInsight.sources[i];
        }
    }
    return insightSource;
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
function SetAnalyticsMetadata(mediaPlayer) {
    $("#analyticsMetadata").empty();
    var mediaProcessor = $("#analyticsProcessors option:selected").text();
    if (mediaProcessor != "") {
        mediaPlayer.pause();
        SetCursor(true);
        $.get("/insight/metadata",
            {
                mediaProcessor: mediaProcessor.replace(/\s+/g, ""),
                documentId: $("#analyticsProcessors").val(),
                timeSeconds: mediaPlayer.currentTime()
            },
            function (metadata) {
                SetCursor(false);
                $("#analyticsMetadata").jsonBrowse(metadata);
            }
        );
    }
}