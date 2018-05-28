var _fileUploader, _fileUploaderStartTime;
function SetMultipleFileAsset() {
    var fileNames = GetFileNames();
    for (var i = 0; i < fileNames.length; i++) {
        var fileName = fileNames[i];
        var fileExtension = fileName.split(".").pop();
        if (fileExtension.toLowerCase() == "ism") {
            $("#multipleFileAsset").prop("checked", true);
        }
    }
}
function GetElapsedTime() {
    var elapsedTime = new Date() - _fileUploaderStartTime;
    var elapsedSeconds = Math.floor(elapsedTime / 1000);
    if (elapsedSeconds == 1) {
        elapsedTime = elapsedSeconds + " Second";
    } else if (elapsedSeconds < 60) {
        elapsedTime = elapsedSeconds + " Seconds";
    } else {
        var elapsedMinutes = elapsedSeconds / 60;
        elapsedTime = elapsedMinutes.toFixed(2) + " Minutes";
    }
    return elapsedTime;
}
function GetFileNames() {
    var fileNames = new Array();
    for (var i = 0; i < _fileUploader.files.length; i++) {
        var fileName = _fileUploader.files[i].name;
        fileName = fileName.split("\\").pop();
        fileNames.push(fileName);
    }
    return fileNames;
}
function CreateUploader() {
    var eventHandlers = {
        PostInit: function (uploader) {
            _fileUploader = uploader;
        },
        FilesAdded: function (uploader, files) {
            SetMultipleFileAsset();
        },
        FilesRemoved: function (uploader, files) {
            SetMultipleFileAsset();
        },
        BeforeUpload: function (uploader, file) {
            uploader.settings.multipart_params = {
                storageAccount: $("#storageAccount").val(),
                contentType: file.type
            };
        },
        StateChanged: function (uploader) {
            var busy = false;
            if (uploader.state == plupload.STARTED) {
                _fileUploaderStartTime = new Date();
                busy = true;
            }
            SetCursor(busy);
        },
        UploadComplete: function (uploader, files) {
            if (uploader.total.failed == 0) {
                var elapsedTime = GetElapsedTime();
                $("#mediaUploadMessage").text("Upload Elapsed Time: " + elapsedTime);
                CreateAssets();
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
    $("#fileUploader").plupload({
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