var _asperaUploader, _asperaUploaderStartTime, _transferApi, _transferId;
function GetElapsedTime() {
    var elapsedTime = new Date() - _asperaUploaderStartTime;
    return MapElapsedTime(elapsedTime);
}
function GetCurrentTransfer(data) {
    var transfer = null;
    for (var i = 0; i < data.transfers.length; i++) {
        if (data.transfers[i].uuid == _transferId) {
            transfer = data.transfers[i];
        }
    }
    return transfer;
}
function InitializeUploader(transferApi) {
    _transferApi = transferApi;
    $("#transferProgress").progressbar({
        change: function (event, data) {
            var progressValue = $("#transferProgress").progressbar("value");
            $("#transferProgressLabel").text(progressValue + "%");
        }
    });
    _asperaUploader = new AW4.Connect({
        sdkLocation: transferApi
    });
    _asperaUploader.addEventListener(AW4.Connect.EVENT.STATUS, ConnectHandler);
    _asperaUploader.addEventListener(AW4.Connect.EVENT.TRANSFER, TransferHandler);
    _asperaUploader.initSession();
}
function ConnectHandler(event, data) {
    if (event == AW4.Connect.EVENT.STATUS) {
        var connectInstaller = new AW4.ConnectInstaller({
            sdkLocation: _transferApi
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
    if (event == AW4.Connect.EVENT.TRANSFER && data.transfers.length > 0) {
        var transfer = GetCurrentTransfer(data);
        if (transfer != null) {
            switch (transfer.status) {
                case AW4.Connect.TRANSFER_STATUS.FAILED:
                    $("#transferMessage").text(_statusLabel + transfer.error_desc);
                    break;
                case AW4.Connect.TRANSFER_STATUS.RUNNING:
                    var transferProgress = Math.floor(transfer.percentage * 100);
                    $("#transferProgress").progressbar("value", transferProgress);
                    $("#transferMessage").text(_statusLabel + "Transfer Running (" + transfer.calculated_rate_kbps + " Kbps)");
                    break;
                case AW4.Connect.TRANSFER_STATUS.COMPLETED:
                    var elapsedTime = GetElapsedTime();
                    var uploaderFiles = GetUploaderFiles(true);
                    $("#transferProgress").progressbar("value", 100);
                    $("#transferMessage").text(_statusLabel + "Transfer Completed (" + elapsedTime + ")");
                    StartWorkflow(uploaderFiles);
                    break;
            }
        }
    }
}
function StartUpload() {
    $.post("/upload/storage",
        {
            transferService: "AsperaFasp",
            filePaths: GetUploaderFiles(false),
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
    _asperaUploader.stopTransfer(_transferId, {
        error: TransferError
    });
}
function TransferStart(e) {
    _transferId = e.transfer_specs[0].uuid;
}
function TransferError(e) {
    var title = "Aspera Transfer Error";
    var message = e.error.user_message;
    DisplayMessage(title, message);
}
