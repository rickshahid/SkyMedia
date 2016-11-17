var _mediaAsset;
function DisplayEditor(treeNode, authToken) {
    var nodeRef = $.jstree.reference(treeNode.reference);
    _mediaAsset = nodeRef.get_node(treeNode.reference);
    var mediaPlayer = GetMediaPlayer(true);
    var sourceUrl = _mediaAsset.original.url;
    var protectionTypes = _mediaAsset.data.protectionTypes;
    mediaPlayer.src([{
        src: sourceUrl,
        type: GetSourceType(sourceUrl),
        protectionInfo: GetProtectionInfo(protectionTypes, authToken)
    }]);
    var dialogId = "editorDialog";
    var title = "Azure Media Video Editor";
    var buttons = {};
    var onClose = function () {
        $("#videoEditor").empty();
    };
    DisplayDialog(dialogId, title, null, buttons, null, null, onClose);
}
function CreateSubclip(clipData) {
    if (clipData == null) {
        $(".amve-rendered-btn")[0].click();
    } else {
        var inputAsset = {
            AssetId: _mediaAsset.id,
            MarkIn: clipData.markIn,
            MarkOut: clipData.markOut
        };
        var jobTask = {
            MediaProcessor: "EncoderStandard",
            ProcessorDocumentId: _mediaAsset.data.processorDocumentId,
            OutputAssetName: clipData.title
        };
        var jobTasks = [jobTask];
        $.post("/workflow/start",
            {
                inputAssets: [inputAsset],
                jobTasks: jobTasks,
            },
            function (result) {
                DisplayWorkflow(jobTasks, result);
            }
        );
        $("#editorDialog").dialog("close");
    }
}
