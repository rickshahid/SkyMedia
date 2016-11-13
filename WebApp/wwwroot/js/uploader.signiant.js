var _signiantUploader, _signiantUploaderStartTime;
function GetElapsedTime() {
    var elapsedTime = new Date() - _signiantUploaderStartTime;
    return MapElapsedTime(elapsedTime);
}
function InitializeUploader(serviceGateway, accountKey) {
    detectPlugin({
        success: InitializeSuccess(serviceGateway, accountKey),
        error: function () {
            var title = "Signiant Transfer Error";
            var message = "Transfer service initialization error.";
            DisplayMessage(title, message);
        }
    });
}
function InitializeSuccess(serviceGateway, accountKey) {
    _signiantUploader = new Signiant.Mst.Upload();
    _signiantUploader.setServer(serviceGateway);
    _signiantUploader.setApiKey(accountKey);
    _signiantUploader.setProbeLB(true);
    _signiantUploader.subscribeForBasicEvents(UploadEvent);
    _signiantUploader.subscribeForTransferErrors(UploadError);
    var transferProgress = new Signiant.Mst.TransferProgressWidget("transferProgress", _signiantUploader);
    transferProgress.showPercent(true);
    transferProgress.render();
}
function StartUpload() {
    $.post("/upload/storage",
        {
            transferService: "SigniantFlight",
            storageAccount: $("#storageAccount").val(),
            containerName: _storageContainer
        },
        function (result) {
            _signiantUploader.setStorageConfig(result);
            _signiantUploaderStartTime = new Date();
            _signiantUploader.startUpload();
        }
    );
}
function StopUpload() {
    _signiantUploader.cancel();
}
function UploadEvent(uploader, eventCode, eventMessage, eventData) {
    if (eventCode != "TRANSFER_PRE_FILE_EVENT") {
        $("#transferMessage").text(_statusLabel + eventMessage);
    }
    if (eventCode == "TRANSFER_COMPLETED") {
        var elapsedTime = GetElapsedTime();
        var uploaderFiles = GetUploaderFiles(true);
        $("#transferMessage").text(_statusLabel + eventMessage + " (" + elapsedTime + ")");
        UploadWorkflow(uploaderFiles);
    }
}
function UploadError(uploader, eventCode, eventMessage, eventData) {
    var title = "Error Message";
    DisplayMessage(title, eventMessage);
}
