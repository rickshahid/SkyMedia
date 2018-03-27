function SetWorkflowInputs(uploadView) {
    if (!uploadView) {
        $("#mediaAssetsRow").show();
    }
    //$("#jobName").editableSelect();
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
    CreateTipRight("mediaWorkflowSave", "Save Workflow");
    CreateTipLeft("mediaWorkflowStart", "Start Workflow");
    CreateTipLeft("mediaWorkflowTaskAdd", "Add Job Task");
    CreateTipRight("mediaWorkflowTaskRemove", "Remove Job Task");
    $("#mediaWorkflowTaskRemove").hide();
    SetJobTaskInputs(1);
}
function ValidWorkflowInput(uploadView, saveWorkflow) {
    var validInput = true;
    if (uploadView) {
        if (_fileUploader.files.length == 0) {
            CreateTipTop("fileUploader_browse", "Select Source Media");
            SetTipVisible("fileUploader_browse", true);
            validInput = false;
        }
    } else {
        _assetIds = GetAssetIds();
        if (_assetIds.length == 0) {
            CreateTipTopLeft("mediaAssets", "Select<br><br>Media Asset", 48, 0);
            SetTipVisible("mediaAssets", true);
            validInput = false;
        }
    }
    if (saveWorkflow && $("#jobName").val() == "") {
        CreateTipTop("jobName", "Job Template Name");
        SetTipVisible("jobName", true);
        $("#jobName").focus();
        validInput = false;
    }
    return validInput;
}
function ValidWorkflowTasks(validInput) {
    var taskNumber = 1;
    do {
        var mediaProcessor = $("#mediaProcessor" + taskNumber).val();
        if (mediaProcessor != null) {
            if (mediaProcessor == "") {
                CreateTipTop("mediaProcessor" + taskNumber, "Select Media Processor");
                SetTipVisible("mediaProcessor" + taskNumber, true);
                validInput = false;
            }
            var encoderConfig = $("#encoderConfig" + taskNumber).val();
            if (encoderConfig == "Custom") {
                var encoderConfigFile = $("#encoderConfigFile" + taskNumber).val();
                if (encoderConfigFile == "") {
                    CreateTipTop("encoderConfigFile" + taskNumber, "Select Custom Configuration File");
                    SetTipVisible("encoderConfigFile" + taskNumber, true);
                    validInput = false;
                }
            }
            taskNumber = taskNumber + 1;
        }
    } while (mediaProcessor != null);
    return validInput;
}
function ValidWorkflow(uploadView, saveWorkflow) {
    var validWorkflow = ValidWorkflowInput(uploadView, saveWorkflow);
    validWorkflow = ValidWorkflowTasks(validWorkflow);
    if (validWorkflow) {
        if (uploadView) {
            _fileUploader.start();
        } else {
            StartWorkflow(saveWorkflow);
        }
    }
}
function DisplayWorkflow(result) {
    var title, message = "", onClose = null;
    if (result.id != null && (result.id.indexOf("jid") > -1 || result.id.indexOf("jtid") > -1)) {
        title = "Azure Media Services Job";
        if (result.id.indexOf("jtid") > -1) {
            title = title + " Template";
            onClose = function () {
                window.location = window.location.href;
            }
        }
        message = result.name + "<br><br>" + result.id;
    } else {
        title = "Azure Media Services Asset";
        if (result.length > 1) {
            title = title + "s";
        }
        for (var i = 0; i < result.length; i++) {
            message = message + GetAssetInfo(result, i);
        }
    }
    DisplayMessage(title, message, null, null, onClose);
}
function GetAssetIds() {
    var assetIds = new Array();
    if ($("#mediaAssets").children.length > 0) {
        assetIds = $("#mediaAssets").jstree(true).get_checked();
    }
    return assetIds;
}
function GetAssetInfo(result, i) {
    var assetInfo = result[i].assetName + "<br>";
    if (result.length == 1) {
        assetInfo = assetInfo + "<br>";
    }
    if (i > 0) {
        assetInfo = "<br><br>" + assetInfo;
    }
    return assetInfo + result[i].assetId;
}
function CreateWorkflow() {
    var job = GetJob();
    var fileNames = GetFileNames();
    SetCursor(true);
    $.post("/workflow/create",
        {
            storageAccount: $("#storageAccount").val(),
            storageEncryption: $("#storageEncryption").prop("checked"),
            inputAssetName: $("#inputAssetName").val(),
            multipleFileAsset: $("#multipleFileAsset").prop("checked"),
            fileNames: fileNames,
            mediaJob: job
        },
        function (result) {
            SetCursor(false);
            DisplayWorkflow(result);
        }
    );
}
function StartWorkflow(saveWorkflow) {
    var job = GetJob();
    job.SaveWorkflow = saveWorkflow;
    SetCursor(true);
    $.post("/workflow/start",
        {
            assetIds: _assetIds,
            mediaJob: job
        },
        function (result) {
            SetCursor(false);
            DisplayWorkflow(result);
        }
    );
}