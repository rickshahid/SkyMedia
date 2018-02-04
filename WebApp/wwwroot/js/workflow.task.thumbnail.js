function ShowThumbnailGeneration(checkbox) {
    var thumbnailGenerationRowId = checkbox.id.replace("encoderThumbnailGeneration", "encoderThumbnailGenerationRow");
    if (checkbox.checked) {
        $("#" + thumbnailGenerationRowId).show();
    } else {
        $("#" + thumbnailGenerationRowId).hide();
    }
    ScrollToBottom();
}