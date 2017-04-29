function DisplayVideoClipper() {
    var dialogId = "clipperDialog";
    var title = "Azure Media Video Clipper";
    var buttons = {};
    var onOpen = function () {
        $("#" + dialogId).load("/home/clipper");
    };
    var onClose = function () {
        $("#mediaClipper").empty();
    };
    DisplayDialog(dialogId, title, null, buttons, null, null, onOpen, onClose);
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
