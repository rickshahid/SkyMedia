var _uploadStartTime;
function SetStorageTip() {
    var tipText = $("#storageAccount option:selected").text();
    CreateTipTop("storageAccount", tipText);
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
            adaptiveStreaming: $("#adaptiveStreaming").prop("checked"),
            thumbnailImages: $("#thumbnailImages").prop("checked"),
            videoAnalyzer: $("#videoAnalyzer").prop("checked"),
            audioAnalyzer: $("#audioAnalyzer").prop("checked"),
            videoIndexer: $("#videoIndexer").prop("checked"),
            audioIndexer: $("#audioIndexer").prop("checked")
        },
        function (entities) {
            SetCursor(false);
            var message = "";
            for (var i = 0; i < entities.length; i++) {
                var entity = entities[i];
                var entityType = entity["properties.state"] == null ? "Asset" : "Job";
                var entityItemRef, insightId;
                if (entityType == "Asset") {
                    entityItemRef = "/asset/item?assetName=" + entity.name;
                    insightId = entity["properties.alternateId"];
                } else {
                    var transformName = GetParentName(entity);
                    entityItemRef = "/job/item?transformName=" + transformName + "&jobName=" + entity.name;
                    insightId = entity["properties.correlationData"]["insightId"];
                }
                if (message != "") {
                    message = message + "<br><br>";
                }
                message = message + "<a class='siteLink' target='_blank' href='" + entityItemRef + "'>";
                message = message + "Media " + entityType + " Created - " + entity.name + "</a>";
                if (insightId != null) {
                    var insightRef = "/account/indexerInsights?insightId=" + insightId;
                    var insightType = $("#audioIndexer").prop("checked") ? "Audio" : "Video";
                    message = message + "<br><br><a class='siteLink' target='_blank' href='" + insightRef + "'>";
                    message = message + insightType + " Indexer Insight - " + insightId + "</a>";
                }
            }
            $("#mediaUploadEntities").html(message);
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
            prevent_duplicates: true,
            max_file_size: "4GB"
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