var _inputAssets;
function SetWorkflowInputs(uploadView, signiantAccountKey, asperaAccountKey) {
    CreateTipLeft("mediaWorkflowTaskAdd", "Add Job Task");
    CreateTipRight("mediaWorkflowTaskRemove", "Remove Job Task");
    CreateTipLeft("mediaWorkflowStart", "Start Workflow");
    $("#mediaWorkflowTaskRemove").hide();
    if (uploadView) {
        if (signiantAccountKey != "") {
            $("#uploadService[value='signiantFlight']").prop("disabled", false);
        }
        if (asperaAccountKey != "") {
            $("#uploadService[value='asperaFasp']").prop("disabled", false);
        }
        $("#inputProtectionRow").show();
    } else {
        $("#mediaAssetsRow").show();
    }
    $("#jobPriority").slider({
        min: 0,
        max: 9,
        step: 1,
        slide: function (event, ui) {
            $("#jobPriorityValue").text(ui.value);
        }
    });
    SetJobTaskOptions(1);
}
function ValidWorkflowInput(uploadView) {
    var validInput = true;
    if (uploadView) {
        if (_fileUploader.files.length == 0) {
            CreateTipTop("fileUploader_browse", "Select Media");
            SetTipVisible("fileUploader_browse", true);
            validInput = false;
        }
        if ($("#multipleFileAsset").prop("checked") && $("#inputAssetName").val() == "") {
            CreateTipBottom("inputAssetName", "Set Multiple File Asset Name");
            SetTipVisible("inputAssetName", true);
            $("#inputAssetName").focus();
            validInput = false;
        }
    } else {
        _inputAssets = GetInputAssets();
        if (_inputAssets.length == 0) {
            CreateTipTop("mediaAssets", "Check At Least 1 Media Asset");
            SetTipVisible("mediaAssets", true);
            validInput = false;
        }
    }
    var taskNumber = 1;
    do {
        var mediaProcessor = $("#mediaProcessor" + taskNumber).val();
        if (mediaProcessor != null) {
            if (mediaProcessor == "None") {
                CreateTipTop("mediaProcessor" + taskNumber, "Select Media Processor");
                SetTipVisible("mediaProcessor" + taskNumber, true);
                validInput = false;
            }
            var processorConfig = $("#processorConfig" + taskNumber).val();
            if (processorConfig == "Custom") {
                var processorConfigFile = $("#processorConfigFile" + taskNumber).val();
                if (processorConfigFile == "") {
                    CreateTipTop("processorConfigFile" + taskNumber, "Select Custom Configuration File");
                    SetTipVisible("processorConfigFile" + taskNumber, true);
                    validInput = false;
                }
            }
            taskNumber = taskNumber + 1;
        }
    } while (mediaProcessor != null);
    return validInput;
}
function ValidateWorkflow(uploadView) {
    if (ValidWorkflowInput(uploadView)) {
        if (uploadView) {
            _fileUploader.start();
        } else {
            StartWorkflow(null);
        }
    }
}
function GetFileNames(files) {
    var fileNames = new Array();
    if (files != null) {
        for (var i = 0; i < files.length; i++) {
            var fileName = files[i];
            if (files[i].name != null) {
                fileName = files[i].name;
            }
            if (fileName.indexOf("/") > -1) {
                var fileInfo = fileName.split("/");
                fileName = fileInfo[fileInfo.length - 1];
            }
            fileNames.push(fileName);
        }
    }
    return fileNames;
}
function GetInputAssets() {
    var inputAssets = new Array();
    if ($("#mediaAssets").children.length > 0) {
        var mediaAssets = $("#mediaAssets").jstree(true).get_checked();
        for (var i = 0; i < mediaAssets.length; i++) {
            var inputAsset = {
                AssetId: mediaAssets[i]
            };
            inputAssets.push(inputAsset);
        }
    }
    return inputAssets;
}
function ShowContentProtection(checkbox) {
    if (checkbox.checked) {
        $("#contentProtectionRow").show();
    } else {
        $("#contentProtectionRow").hide();
    }
}
function GetContentProtection() {
    var contentProtection = null;
    if ($("#contentProtection").prop("checked")) {
        contentProtection = {
            AES: $("#aes").prop("checked"),
            DRMPlayReady: $("#drmPlayReady").prop("checked"),
            DRMWidevine: $("#drmWidevine").prop("checked"),
            DRMFairPlay: $("#drmFairPlay").prop("checked"),
            ContentAuthTypeToken: $("#contentAuthTypeToken").prop("checked"),
            ContentAuthTypeAddress: $("#contentAuthTypeAddress").prop("checked"),
            ContentAuthAddressRange: $("#contentAuthAddressRange").val()
        };
    }
    return contentProtection;
}
function SetContentProtection(checkbox) {
    switch (checkbox.id) {
        case "aes":
            if (checkbox.checked) {
                $("#drmPlayReady").prop("checked", false);
                $("#drmWidevine").prop("checked", false);
                $("#drmFairPlay").prop("checked", false);
            }
            break;
        case "drmPlayReady":
        case "drmWidevine":
        case "drmFairPlay":
            if (checkbox.checked) {
                $("#aes").prop("checked", false);
            }
            break;
    }
}
function SetContentAuthAddressRange(checkBox) {
    if (checkBox.checked) {
        $("#contentAuthAddressRange").prop("disabled", false);
        $("#contentAuthAddressRange").focus();
    } else {
        $("#contentAuthAddressRange").prop("disabled", true);
    }
}
function GetAssetInfo(result, i) {
    var assetInfo = result[i].assetName + "<br />"
    if (result.length == 1) {
        assetInfo = assetInfo + "<br />";
    }
    if (i > 0) {
        assetInfo = "<br /><br />" + assetInfo;
    }
    return assetInfo + result[i].assetId;
}
function DisplayWorkflow(jobTasks, result) {
    var title, message = "";
    if (jobTasks.length == 0) {
        title = "Azure Media Services Asset";
        if (result.length > 1) {
            title = title + "s";
        }
        for (var i = 0; i < result.length; i++) {
            message = message + GetAssetInfo(result, i);
        }
    } else {
        title = "Azure Media Services Job";
        message = result.name + "<br /><br />" + result.id;
    }
    DisplayMessage(title, message);
}
function StartWorkflow(files) {
    var jobTasks = GetJobTasks();
    $.post("/workflow/start",
        {
            fileNames: GetFileNames(files),
            storageAccount: $("#storageAccount").val(),
            storageEncryption: $("#storageEncryption").prop("checked"),
            inputAssetName: $("#inputAssetName").val(),
            multipleFileAsset: $("#multipleFileAsset").prop("checked"),
            publishInputAsset: $("#publishInputAsset").prop("checked"),
            inputAssets: _inputAssets,
            jobName: $("#jobName").val(),
            jobPriority: $("#jobPriorityValue").text(),
            jobTasks: jobTasks,
            contentProtection: GetContentProtection()
        },
        function (result) {
            DisplayWorkflow(jobTasks, result);
        }
    );
}
