function SetWorkflowInputs(uploadView) {
    if (!uploadView) {
        $("#mediaAssetsRow").show();
    }
    $("#jobName").editableSelect();
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
    SetJobTaskWidgets(1);
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
        _jobInputs = GetJobInputs(uploadView);
        if (_jobInputs.length == 0) {
            CreateTipTopLeft("mediaAssets", "Select Media Assets", 48, 0);
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
function ValidWorkflowTaskClear(taskNumber) {
    SetTipVisible("mediaProcessor" + taskNumber, false);
    SetTipVisible("encoderConfigFile" + taskNumber, false);
}
function ValidWorkflow(uploadView, saveWorkflow) {
    var validWorkflow = ValidWorkflowInput(uploadView, saveWorkflow);
    var jobTemplateId = GetJobTemplateId();
    if (jobTemplateId == "") {
        validWorkflow = ValidWorkflowTasks(validWorkflow);
    }
    if (validWorkflow) {
        _saveWorkflow = saveWorkflow;
        if (uploadView) {
            _fileUploader.start();
        } else {
            StartWorkflow();
        }
    }
}
function GetJobInputs(uploadView) {
    var jobInputs = new Array();
    if ($("#mediaAssets").children.length > 0) {
        var mediaAssets = $("#mediaAssets").jstree(true).get_checked();
        for (var i = 0; i < mediaAssets.length; i++) {
            var jobInput = {
                AssetId: mediaAssets[i],
                WorkflowView: !uploadView
            };
            jobInputs.push(jobInput);
        }
    }
    return jobInputs;
}
function IngestAssets() {
    var job = GetJob();
    var files = GetUploaderFiles(true);
    $.post("/workflow/ingest",
        {
            fileNames: files,
            storageAccount: $("#storageAccount").val(),
            storageEncryption: $("#storageEncryption").prop("checked"),
            inputAssetName: $("#inputAssetName").val(),
            multipleFileAsset: $("#multipleFileAsset").prop("checked"),
            mediaJob: job
        },
        function (result) {
            DisplayWorkflow(result);
        }
    );
}
function StartWorkflow() {
    var job = GetJob();
    $.post("/workflow/start",
        {
            jobInputs: _jobInputs,
            mediaJob: job
        },
        function (result) {
            DisplayWorkflow(result);
        }
    );
}
