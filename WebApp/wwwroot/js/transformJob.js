function FindAsset() {
    SetCursor(true);
    $.post("/asset/find",
        {
            assetName: $("#inputAssetName").val()
        },
        function (asset) {
            SetCursor(false);
            if (asset != null) {
                $("#inputAssetName").removeClass("mediaEntityNotFound").addClass("mediaEntityFound");
                SetTipVisible("inputAssetName", false);
            } else {
                $("#inputAssetName").removeClass("mediaEntityFound").addClass("mediaEntityNotFound");
                CreateTipRight("inputAssetName", "Media Asset<br><br>Not Found");
                SetTipVisible("inputAssetName", true);
            }
        }
    );
}
function CreateJob() {
    var transformName = $("#transforms").val();
    var inputAssetName = $("#inputAssetName").val();
    var inputFileUrl = $("#inputFileUrl").val();
    if (transformName == "" || (inputAssetName == "" && inputFileUrl == "")) {
        var tipText = "Required Field";
        if (transformName == "") {
            CreateTipRight("transforms", tipText);
            SetTipVisible("transforms", true);
        } else {
            SetTipVisible("transforms", false);
        }
        if (inputAssetName == "" && inputFileUrl == "") {
            CreateTipRight("inputAssetName", tipText);
            CreateTipRight("inputFileUrl", tipText);
            SetTipVisible("inputAssetName", true);
            SetTipVisible("inputFileUrl", true);
        } else {
            SetTipVisible("inputAssetName", false);
            SetTipVisible("inputFileUrl", false);
        }
        if (inputFileUrl == "" && $("#inputAssetName").hasClass("entityNotFound")) {
            SetTipVisible("inputAssetName", true);
        }
    } else {
        var title = "Confirm Create Media Job";
        var message = "Are you sure you want to create a new media job?";
        var onConfirm = function () {
            SetCursor(true);
            $.post("/job/create",
                {
                    transformName: transformName,
                    jobName: $("#name").val(),
                    jobDescription: $("#description").val(),
                    jobPriority: $("#jobPriority:checked").val(),
                    jobData: _jsonEditor.getText(),
                    inputFileUrl: inputFileUrl,
                    inputAssetName: inputAssetName,
                    outputAssetMode: $("#outputAssetMode").val(),
                    streamingPolicyName: $("#streamingPolicies").val()
                },
                function (job) {
                    SetCursor(false);
                    var buttons = {
                        OK: function() {
                            window.location = "/job?transformName=" + transformName + "&jobName=" + job.name;
                            $(this).dialog("close");
                        }
                    };
                    DisplayMessage("Media Job Created", job.name, buttons);
                }
            );
        };
        ConfirmMessage(title, message, onConfirm);
    }
}
function UpdateJob() {
    var transformName = $("#transforms").val();
    var jobName = $("#name").val();
    if (transformName != "" && jobName != "") {
        var title = "Confirm Update Job";
        var message = "Are you sure you want to update the current job?";
        var onConfirm = function () {
            SetCursor(true);
            $.post("/job/update",
                {
                    transformName: transformName,
                    jobName: jobName,
                    jobDescription: $("#description").val(),
                    jobPriority: $("#jobPriority:checked").val()
                },
                function (job) {
                    SetCursor(false);
                    window.location = window.location.href;
                }
            );
            $(this).dialog("close");
        };
        ConfirmMessage(title, message, onConfirm);
    }
}
function CancelJob(jobName, transformName) {
    var title = "Confirm Job Cancel Request";
    var message = "Are you sure you want to cancel the '" + FormatValue(jobName) + "' job?";
    var onConfirm = function () {
        SetCursor(true);
        $.post("/job/cancel",
            {
                transformName: decodeURIComponent(transformName),
                jobName: decodeURIComponent(jobName)
            },
            function () {
                SetCursor(false);
                window.location = window.location.href;
            }
        );
        $(this).dialog("close");
    };
    ConfirmMessage(title, message, onConfirm);
}