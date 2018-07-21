var _fileUploader, _uploadStartTime;
function SetIndexerOptions(videoIndexerKey) {
    if (videoIndexerKey == "") {
        var tipText = "To enable this media processing option,<br><br>add a Video Indexer Key to your User Account Profile";
        if ($("#videoIndexerInsight").prop("disabled") && $("#audioIndexerInsight").prop("disabled")) {
            CreateTipTop("videoIndexerInsightOption", tipText, 25);
            CreateTipTop("audioIndexerInsightOption", tipText, 25);
        }
    } else {
        $("#videoIndexerInsight").prop("disabled", false);
        $("#audioIndexerInsight").prop("disabled", false);
    }
}
function SetStorageTip() {
    var tipText = $("#storageAccount option:selected").text();
    CreateTipTop("storageAccount", tipText);
}
function SetUploadOption(checkbox) {
    switch (checkbox.id) {
        case "videoAnalyzerPreset":
            $("#audioAnalyzerPreset").prop("checked", false);
            $("#audioAnalyzerPreset").prop("disabled", checkbox.checked);
            break;
        case "videoIndexerInsight":
            $("#audioIndexerInsight").prop("checked", false);
            $("#audioIndexerInsight").prop("disabled", checkbox.checked);
            break;
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
            audioAnalyzerPreset: $("#audioAnalyzerPreset").prop("checked"),
            videoIndexerInsight: $("#videoIndexerInsight").prop("checked"),
            audioIndexerInsight: $("#audioIndexerInsight").prop("checked")
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
                message = message + "Media " + entityType + " Created<br>(" + entity.name + ")";
                if (indexId != null) {
                    var insightType = $("#audioIndexerInsight").prop("checked") ? "Audio" : "Video";
                    message = message + "<br><br>" + insightType + " Indexer Insight";
                    message = message + "<br>(" + indexId + ")";
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