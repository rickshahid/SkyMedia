function ShowVideoOverlay(checkbox) {
    var videoOverlayRowId = checkbox.id.replace("encoderVideoOverlay", "encoderVideoOverlayRow");
    if (checkbox.checked) {
        $("#" + videoOverlayRowId).show();
    } else {
        $("#" + videoOverlayRowId).hide();
    }
    ScrollToBottom();
}