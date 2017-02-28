function SetWorkflowInputs(uploadView, signiantAccountKey, asperaAccountKey) {
    CreateTipLeft("mediaWorkflowTaskAdd", "Add Job Task");
    CreateTipRight("mediaWorkflowTaskRemove", "Remove Job Task");
    CreateTipLeft("mediaWorkflowStart", "Start Workflow");
    $("#mediaWorkflowTaskRemove").hide();
    if (uploadView) {
        var currentUrl = window.location.href;
        if (currentUrl.indexOf("signiant.") > -1) {
            $("#uploadSigniantFlight").show();
        } else if (currentUrl.indexOf("aspera.") > -1) {
            $("#uploadAsperaFasp").show();
        } else {
            $("#uploadSigniantFlight").show();
            $("#uploadAsperaFasp").show();
        }
        if (signiantAccountKey != "") {
            $("#uploadService[value='signiantFlight']").prop("disabled", false);
        }
        if (asperaAccountKey != "") {
            $("#uploadService[value='asperaFasp']").prop("disabled", false);
        }
    } else {
        $("#mediaAssetsRow").show();
    }
    $("#jobPriority").slider({
        min: 0,
        max: 9,
        step: 1,
        classes: {
            "ui-slider-handle": "sliderHandle"
        },
        slide: function (event, ui) {
            $("#jobPriorityLabel").text(ui.value);
        }
    });
    SetJobTaskWidgets(1, false);
}
function ValidWorkflowInput(uploadView) {
    var validInput = true;
    if (uploadView) {
        if (_fileUploader.files.length == 0) {
            CreateTipTop("fileUploader_browse", "Select Source Media");
            SetTipVisible("fileUploader_browse", true);
            validInput = false;
        }
        if ($("#multipleFileAsset").prop("checked") && $("#inputAssetName").val() == "") {
            CreateTipTop("inputAssetName", "Set Multiple File Asset Name");
            SetTipVisible("inputAssetName", true);
            $("#inputAssetName").focus();
            validInput = false;
        }
    } else {
        _inputAssets = GetInputAssets();
        if (_inputAssets.length == 0) {
            CreateTipTopLeft("mediaAssets", "Check Media Asset(s)", 48, 5);
            SetTipVisible("mediaAssets", true);
            validInput = false;
        }
    }
    return ValidWorkflowTasks(validInput);
}
function ValidWorkflow(uploadView) {
    if (ValidWorkflowInput(uploadView)) {
        if (uploadView) {
            _fileUploader.start();
        } else {
            StartWorkflow();
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
            if (_clippedAssets != null) {
                for (var x = 0; x < _clippedAssets.length; x++) {
                    if (inputAsset.AssetId == _clippedAssets[x].AssetId) {
                        inputAsset.MarkIn = _clippedAssets[x].MarkIn;
                        inputAsset.MarkOut = _clippedAssets[x].MarkOut;
                    }
                }
            }
            inputAssets.push(inputAsset);
        }
    }
    return inputAssets;
}
function GetAssetInfo(result, i) {
    var assetInfo = result[i].assetName + "<br />";
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
function GetJob() {
    var jobTasks = GetJobTasks();
    var job = {
        Name: $("#jobName").val(),
        Scale: $("#jobScale").val(),
        Priority: $("#jobPriorityLabel").text(),
        Notification: $("input[name='jobNotification']:checked").val(),
        Tasks: jobTasks
    };
    return job;
}
function UploadWorkflow(files) {
    var job = GetJob();
    $.post("/workflow/upload",
        {
            fileNames: GetFileNames(files),
            storageAccount: $("#storageAccount").val(),
            storageEncryption: $("#storageEncryption").prop("checked"),
            inputAssetName: $("#inputAssetName").val(),
            multipleFileAsset: $("#multipleFileAsset").prop("checked"),
            publishInputAsset: $("#publishInputAsset").prop("checked"),
            inputAssets: _inputAssets,
            mediaJob: job
        },
        function (result) {
            DisplayWorkflow(job.Tasks, result);
        }
    );
}
function StartWorkflow() {
    var job = GetJob();
    $.post("/workflow/start",
        {
            inputAssets: _inputAssets,
            mediaJob: job
        },
        function (result) {
            DisplayWorkflow(job.Tasks, result);
        }
    );
}
