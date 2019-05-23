var _uploadStartTime;
function SetStorageTip() {
    var tipText = $("#storageAccount option:selected").text();
    CreateTipTop("storageAccount", tipText);
}
function SetEncodingPreset(radioButton, setState) {
    if (setState) {
        radioButton.tag = radioButton.checked;
    } else {
        radioButton.checked = !radioButton.tag;
    }
}
function GetUploadTime() {
    var uploadTime = new Date() - _uploadStartTime;
    var elapsedSeconds = Math.floor(uploadTime / 1000);
    if (elapsedSeconds == 1) {
        uploadTime = elapsedSeconds + " Second";
    } else if (elapsedSeconds < 60) {
        uploadTime = elapsedSeconds + " Seconds";
    } else {
        var elapsedMinutes = elapsedSeconds / 60;
        uploadTime = elapsedMinutes.toFixed(2) + " Minutes";
    }
    return uploadTime;
}
function GetFileNames(files) {
    var fileNames = new Array();
    for (var i = 0; i < files.length; i++) {
        var fileName = files[i].name;
        fileNames.push(fileName);
    }
    return fileNames;
}
function CreateWorkflow(files) {
    SetCursor(true);
    $.post("/asset/workflow",
        {
            storageAccount: $("#storageAccount").val(),
            assetName: $("#assetName").val(),
            assetDescription: $("#assetDescription").val(),
            assetAlternateId: $("#assetAlternateId").val(),
            fileNames: GetFileNames(files),
            adaptiveStreaming: $("input:radio[name='encodingPreset'][value='adaptiveStreaming']").prop("checked"),
            contentAwareEncoding: $("input:radio[name='encodingPreset'][value='contentAwareEncoding']").prop("checked"),
            thumbnailImages: $("#thumbnailImages").prop("checked"),
            thumbnailSprite: $("#thumbnailSprite").prop("checked"),
            videoAnalyzer: $("#videoAnalyzer").prop("checked"),
            audioAnalyzer: $("#audioAnalyzer").prop("checked"),
            videoIndexer: $("#videoIndexer").prop("checked"),
            audioIndexer: $("#audioIndexer").prop("checked")
        },
        function (newEntities) {
            SetCursor(false);
            var message = "";
            for (var i = 0; i < newEntities.length; i++) {
                var newEntity = newEntities[i], newEntityRef;
                if (newEntity.type == "Asset") {
                    newEntityRef = "/asset?assetName=" + newEntity.name;
                } else {
                    var transformName = GetParentName(newEntity);
                    newEntityRef = "/job?transformName=" + transformName + "&jobName=" + newEntity.name;
                }
                if (message != "") {
                    message = message + "<br><br>";
                }
                message = message + "<a class='siteLink' target='_blank' href='" + newEntityRef + "'>";
                message = message + "Media " + FormatValue(newEntity.type) + " Name - " + newEntity.name + "</a>";
                if (newEntity.insightId != null) {
                    newEntityRef = "/insight?insightId=" + newEntity.insightId;
                    message = message + "<br><br><a class='siteLink' target='_blank' href='" + newEntityRef + "'>";
                    message = message + " Video Indexer Insight Id - " + newEntity.insightId + "</a>";
                }
            }
            $("#mediaEntitiesCreated").html(message);
        }
    );
}
function CreateUploader() {
    $("#mediaFileUploader").plupload({
        url: "/upload/block",
        runtimes: "html5",
        chunk_size: "10MB",
        max_retries: 3,
        multipart: true,
        dragdrop: false,
        sortable: true,
        rename: true,
        filters: {
            max_file_size: "10GB",
            prevent_duplicates: true
        },
        init: {
            BeforeUpload: function (uploader, file) {
                uploader.settings.multipart_params = {
                    storageAccount: $("#storageAccount").val(),
                    contentType: file.type
                };
            },
            StateChanged: function (uploader) {
                var isBusy = false;
                if (uploader.state == plupload.STARTED) {
                    _uploadStartTime = new Date();
                    isBusy = true;
                }
                SetCursor(isBusy);
            },
            UploadComplete: function (uploader, files) {
                if (uploader.total.failed == 0) {
                    var uploadTime = GetUploadTime();
                    $("#mediaUploadMessage").text("Upload Elapsed Time: " + uploadTime);
                    CreateWorkflow(files);
                }
            },
            Error: function (uploader, error) {
                var title = "Error Message";
                var message = error.message;
                if (error.response != null && error.response != "") {
                    message = error.response;
                }
                DisplayMessage(title, message);
            }
        }
    });
}