var _fileUploader, _uploadStartTime;
function SetStorageTip() {
    var tipText = $("#storageAccount option:selected").text();
    CreateTipTop("storageAccount", tipText);
}
function SetUploadOption(checkbox) {
    if (checkbox.id == "videoAnalyzerPreset") {
        $("#audioAnalyzerPreset").prop("checked", false);
        $("#audioAnalyzerPreset").prop("disabled", checkbox.checked);
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
function GetFileNames() {
    var fileNames = new Array();
    for (var i = 0; i < _fileUploader.files.length; i++) {
        var fileName = _fileUploader.files[i].name;
        fileNames.push(fileName);
    }
    return fileNames;
}
function CreateAsset() {
    SetCursor(true);
    $.post("/asset/create",
        {
            storageAccount: $("#storageAccount").val(),
            assetName: $("#assetName").val(),
            assetDescription: $("#assetDescription").val(),
            assetAlternateId: $("#assetAlternateId").val(),
            fileNames: GetFileNames(),
            standardEncoderPreset: $("#standardEncoderPreset").prop("checked"),
            videoAnalyzerPreset: $("#videoAnalyzerPreset").prop("checked"),
            audioAnalyzerPreset: $("#audioAnalyzerPreset").prop("checked")
        },
        function (entities) {
            SetCursor(false);
            var message = "";
            for (var i = 0; i < entities.length; i++) {
                var entity = entities[i];
                var entityType = entity["properties.state"] != null ? "Job" : "Asset";
                var indexId = entityType == "Job" ? entity["properties.correlationData"]["indexId"] : entity["properties.alternateId"];
                if (message != "") {
                    message = message + "<br><br>";
                }
                message = message + "Media " + entityType + " Created - " + entity.name;
                if (indexId != null) {
                    var insightType = $("#audioAnalyzerPreset").prop("checked") ? "Audio" : "Video";
                    message = message + "<br><br>" + insightType + " Indexer Insight - " + IndexId;
                }
            }
            $("#mediaUploadEntities").html(message);
        }
    );
}
function CreateUploader() {
    var eventHandlers = {
        PostInit: function (uploader) {
            _fileUploader = uploader;
        },
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
                CreateAsset();
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
    };
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
        init: eventHandlers
    });
}