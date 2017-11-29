function CreateClip(clipData) {
    alert("CreateClip");
}
function ToggleClipper() {
    var videoClipper = document.getElementById("videoClipper");
    if (videoClipper != null) {
        $("#videoClipper").remove();
    } else {
        videoClipper = document.createElement("div");
        videoClipper.id = "videoClipper"
        videoClipper.className = "azure-subclipper dark-skin";
        var mediaPlayer = document.getElementById("mediaPlayer");
        mediaPlayer.appendChild(videoClipper);
        var clipper = new subclipper({
            restVersion: "2.0",
            selector: "#videoClipper",
            submitSubclipCallback: CreateClip,
            height: 600
        });
    }
}
//function DisplayVideoClipper(languageCode) {
//    var dialogId = "clipperDialog";
//    var title = "Azure Video Clipper";
//    var buttons = {};
//    var mediaPlayer = GetMediaPlayer(true);
//    var mediaStream = _mediaStreams[_streamNumber - 1];
//    SetPlayerContent(mediaPlayer, mediaStream, languageCode, true);
//    DisplayDialog(dialogId, title, null, buttons, null, null, null, null);
//}
//function CreateVideoClip(clipData) {
//    if (clipData != null) {
//        $.post("/asset/clip",
//            {
//                clipMode: clipData._amveUX.mode,
//                clipName: clipData.title,
//                sourceUrl: clipData.src,
//                markIn: Math.floor(clipData.markIn),
//                markOut: Math.floor(clipData.markOut)
//            },
//            function (result) {
//                if (result.id.indexOf("jid") > -1) {
//                    DisplayWorkflow(result);
//                } else {
//                    window.location = window.location.href;
//                }
//            }
//        );
//        $("#clipperDialog").dialog("close");
//    }
//}