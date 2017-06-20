function DisplayVideoClipper(languageCode) {
    var dialogId = "clipperDialog";
    var title = "Azure Video Clipper";
    var buttons = {};
    var mediaPlayer = GetMediaPlayer(true);
    var mediaStream = _mediaStreams[_streamNumber - 1];
    SetPlayerContent(mediaPlayer, mediaStream, languageCode, true);
    DisplayDialog(dialogId, title, null, buttons, null, null, null, null, true);
}
function CreateVideoClip(clipData) {
    if (clipData != null) {
        $.post("/asset/clip",
            {
                clipMode: clipData._amveUX.mode,
                clipName: clipData.title,
                sourceUrl: clipData.src,
                markIn: Math.floor(clipData.markIn),
                markOut: Math.floor(clipData.markOut)
            },
            function (result) {
                if (result.id.indexOf("jid") > -1) {
                    DisplayWorkflow(result);
                } else {
                    window.location = window.location.href;
                }
            }
        );
        $("#clipperDialog").dialog("close");
    }
}
