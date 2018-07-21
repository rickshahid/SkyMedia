function FindAsset() {
    SetCursor(true);
    $.post("/asset/find",
        {
            assetName: $("#inputAssetName").val()
        },
        function (entityFound) {
            SetCursor(false);
            if (entityFound) {
                $("#inputAssetName").removeClass("entityNotFound").addClass("entityFound");
                SetTipDisable("inputAssetName", true);
            } else {
                $("#inputAssetName").removeClass("entityFound").addClass("entityNotFound");
                SetTipDisable("inputAssetName", false);
                CreateTipRight("inputAssetName", "Media Asset Not Found");
                SetTipVisible("inputAssetName", true);
            }
        }
    );
}
function CreateJob() {
    var transformName = $("#transforms").val();
    var inputAssetName = $("#inputAssetName").val();
    if (transformName == "" || inputAssetName == "") {
        var tipText = "Required Field";
        if (transformName == "") {
            CreateTipRight("transforms", tipText);
            SetTipVisible("transforms", true);
        }
        if (inputAssetName == "") {
            CreateTipRight("inputAssetName", tipText);
            SetTipVisible("inputAssetName", true);
        }
    } else if ($("#inputAssetName").hasClass("entityNotFound")) {
        SetTipVisible("inputAssetName", true);
    } else {
        var title = "Confirm Create Job";
        var message = "Are you sure you want to create a new job?";
        var onConfirm = function () {
            SetCursor(true);
            $.post("/job/create",
                {
                    transformName: transformName,
                    jobName: $("#name").val(),
                    jobDescription: $("#description").val(),
                    jobPriority: $("#jobPriority").val(),
                    inputAssetName: inputAssetName,
                    streamingPolicyName: $("#streamingPolicies").val()
                },
                function (entity) {
                    SetCursor(false);
                    window.location = window.location.href;
                }
            );
            $(this).dialog("close");
        }
        ConfirmMessage(title, message, onConfirm);
    }
}