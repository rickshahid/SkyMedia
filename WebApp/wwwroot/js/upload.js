var _fileUploader, _uploadStartTime;
function SetStorageTip() {
    var tipText = $("#storageAccount option:selected").text();
    CreateTipTop("storageAccount", tipText);
}
function SetUploadOption(checkbox) {
    var reservedUnits = "";
    var encoderReservedUnits = "1 S3 reserved unit";
    var analyzerReservedUnits = "10 S3 reserved units";
    switch (checkbox.id) {
        case "standardEncoderPreset":
            if (checkbox.checked) {
                if ($("#videoAnalyzerPreset").prop("checked") ||
                    $("#audioAnalyzerPreset").prop("checked")) {
                    reservedUnits = analyzerReservedUnits;
                } else {
                    reservedUnits = encoderReservedUnits;
                }
            }
            break;
        case "videoAnalyzerPreset":
        case "audioAnalyzerPreset":
            if (checkbox.id == "videoAnalyzerPreset") {
                $("#audioAnalyzerPreset").prop("checked", false);
                $("#audioAnalyzerPreset").prop("disabled", checkbox.checked);
            }
            if (checkbox.checked) {
                reservedUnits = analyzerReservedUnits;
            }
            break;
    }
    if (reservedUnits != "") {
        var message = "For enhanced media processing performance,<br>make sure you have at least " + reservedUnits + "<br>allocated in your media account before proceeding."
        DisplayMessage("Media Job Reserved Units", message);
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
            description: $("#assetDescription").val(),
            alternateId: $("#assetAlternateId").val(),
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
                var entityCreated = FormatDateTime(entity["properties.created"]);
                if (message != "") {
                    message = message + "<br><br>";
                }
                message = message + "Media " + entityType + " Created<br>" + entity.name;
                message = message + "<br>(" + entityCreated.replace("<br>", " ") + ")";
                if (entity["properties.alternateId"] != null) {
                    message = message + "<br><br>Video Indexer Id<br>" + entity["properties.alternateId"];
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