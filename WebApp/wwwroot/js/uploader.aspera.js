var _asperaUploader, _asperaUploaderStartTime, _asperaTransferApi, _asperaTransferId;
function GetElapsedTime() {
    var elapsedTime = new Date() - _asperaUploaderStartTime;
    return MapElapsedTime(elapsedTime);
}
function InitializeUploader(transferApi) {
    $("#transferProgress").progressbar();
    _asperaTransferApi = transferApi;
    _asperaUploader = new AW4.Connect({
        sdkLocation: _asperaTransferApi
    });
    _asperaUploader.addEventListener(AW4.Connect.EVENT.STATUS, ConnectHandler);
    _asperaUploader.addEventListener(AW4.Connect.EVENT.TRANSFER, TransferHandler);
    _asperaUploader.initSession();
}
function ConnectHandler(event, data) {
    if (event == AW4.Connect.EVENT.STATUS) {
        var connectInstaller = new AW4.ConnectInstaller({
            sdkLocation: _asperaTransferApi
        });
        switch (data) {
            case AW4.Connect.STATUS.INITIALIZING:
                connectInstaller.showLaunching();
                break;
            case AW4.Connect.STATUS.FAILED:
                connectInstaller.showDownload();
                break;
            case AW4.Connect.STATUS.OUTDATED:
                connectInstaller.showUpdate();
                break;
            case AW4.Connect.STATUS.RUNNING:
                connectInstaller.connected();
                break;
        }
    }
}
function TransferHandler(event, data) {
    switch (event) {
        case AW4.Connect.TRANSFER_STATUS.COMPLETED:
            var elapsedTime = GetElapsedTime();
            $("#transferMessage").text("Status: Transfer Completed (" + elapsedTime + ")");
            break;
    }
}
function StartUpload() {
    $.post("/upload/storage",
        {
            transferService: "AsperaFasp",
            filePaths: GetUploaderFiles(),
            storageAccount: $("#storageAccount").val(),
            containerName: _storageContainer
        },
        function (transferSpecs) {
            _asperaUploaderStartTime = new Date();
            _asperaUploader.startTransfers(transferSpecs, {
                success: TransferStart,
                error: TransferError
            });
        }
    );
}
function StopUpload() {
    _asperaUploader.stopTransfer(_asperaTransferId, {
        error: TransferError
    });
}
function TransferStart(e) {
    _asperaTransferId = e.transfer_specs[0].uuid;
    _asperaUploader.showTransferMonitor(_asperaTransferId, {
        error: MonitorError
    });
}
function TransferError(e) {
    var title = "Aspera Transfer Error";
    var message = e.error.user_message;
    DisplayMessage(title, message);
}
function MonitorError(e) {
    var title = "Aspera Monitor Error";
    var message = e.error.user_message;
    DisplayMessage(title, message);
}
