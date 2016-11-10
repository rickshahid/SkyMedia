function ToggleAnalytics(span) {
    if (span.innerText.trim() == "+") {
        span.innerText = "-";
        $("#analyticsView").show();
    } else {
        span.innerText = "+";
        $("#analyticsView").hide();
    }
}
function SetAnalytics(timeSeconds) {
    $.get("/analytics/metadata",
        {
            mediaProcessor: null,
            documentId: null,
            timeSeconds: timeSeconds
        },
        function (result) {
            // TBD
        }
    );
}
function SetAnalyticsView(mediaProcessor) {
    alert(mediaProcessor.value);
}
