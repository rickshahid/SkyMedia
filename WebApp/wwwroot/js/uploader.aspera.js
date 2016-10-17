var _asperaUploader, _asperaUploaderStartTime;
function InitializeUploader(transferApi) {
    _asperaUploader = new AW4.Connect({ sdkLocation: transferApi });
    var connectInstaller = new AW4.ConnectInstaller({ sdkLocation: transferApi });
    var connectHandler = function (event, data) {
        if (event == AW4.Connect.EVENT.STATUS) {
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
    };
    _asperaUploader.addEventListener(AW4.Connect.EVENT.STATUS, connectHandler);
    _asperaUploader.initSession();
}
function AsperaTransferStart(e) {
    var transferId = e.transfer_specs[0].uuid;
    _asperaUploader.showTransferMonitor(transferId, {
        error: MonitorError
    });
}
function AsperaTransferError(e) {
    var title = "Aspera Transfer Error";
    var message = e.error.user_message;
    DisplayMessage(title, message);
}
function AsperaMonitorError(e) {
    var title = "Aspera Monitor Error";
    var message = e.error.user_message;
    DisplayMessage(title, message);
}
function StartUpload(appUrl) {
    $.post("/upload/storage",
        {
            transferService: "AsperaFasp",
            storageAccount: $("#storageAccount").val(),
            containerName: _storageContainer
        },
        function (result) {
            _asperaUploader.showTransferMonitor();
            _asperaUploaderStartTime = new Date();
            //for (var i = 0; i < _fileUploader.files.length; i++) {
            //    transferSpec.paths.push({ "source": uploader.files[i].name });
            //}
            //AsperaTransferStart(transferSpec, appUrl);
        }
    );
}
function AsperaTransferStart(transferSpec, appUrl) {
    var connectSettings = {
        app_id: appUrl,
        back_link: appUrl,
        request_id: uuid.v4()
    };
    _asperaUploader.startTransfer(transferSpec, connectSettings, {
        success: TransferStart,
        error: TransferError
    });
}
