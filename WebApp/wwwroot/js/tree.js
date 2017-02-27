function LoadTreeNodes(authToken, cdnUrl, workflowView) {
    var plugins = workflowView ? ["contextmenu", "checkbox"] : ["contextmenu"];
    if (_editedAssets == null) {
        _editedAssets = new Array();
    }
    $("#mediaAssets").jstree({
        "core": {
            "check_callback": function (operation, node, node_parent, node_position, more) {
                return operation == "rename_node";
            },
            "themes": {
                "variant": "large"
            },
            "data": {
                "url": function (node) {
                    return (node.id == "#") ? "/asset/roots" : "/asset/children?assetId=" + node.id;
                },
                "data": function (node) {
                    return { "id": node.id };
                }
            }
        },
        "plugins": plugins,
        "checkbox": {
            "keep_selected_style": false,
            "tie_selection": false,
            "three_state": false,
            "whole_node": false
        },
        "contextmenu": {
            "items": function (treeNode) {
                var menuItems = {};
                if (treeNode.a_attr.class == "mediaAsset") {
                    menuItems = {
                        "Set": {
                            "label": "Set Video Edit",
                            "icon": cdnUrl + "/MediaAssetEditSet.png",
                            "action": function (treeNode) {
                                DisplayVideoEditor(treeNode, authToken);
                            }
                        },
                        "Clear": {
                            "label": "Clear Video Edit",
                            "icon": cdnUrl + "/MediaAssetEditClear.png",
                            "action": function (treeNode) {
                                ClearVideoEdit(treeNode);
                            }
                        }
                    };
                }
                return menuItems;
            }
        }
    });
}
function SetSubclipTimes(assetId) {
    var startTimeHours = 0, startTimeMinutes = 0, startTimeSeconds = 0;
    var endTimeHours = 0, endTimeMinutes = 0, endTimeSeconds = 0;
    var editedAsset = GetEditedAsset(assetId);
    if (editedAsset != null) {
        startTimeHours = Math.floor(editedAsset.MarkIn / 3600);
        startTimeMinutes = Math.floor((editedAsset.MarkIn - (startTimeHours * 3600)) / 60);
        startTimeSeconds = Math.floor(editedAsset.MarkIn - (startTimeHours * 3600) - (startTimeMinutes * 60));
        endTimeHours = Math.floor(editedAsset.MarkOut / 3600);
        endTimeMinutes = Math.floor((editedAsset.MarkOut - (endTimeHours * 3600)) / 60);
        endTimeSeconds = Math.floor(editedAsset.MarkOut - (endTimeHours * 3600) - (endTimeMinutes * 60));
    }
    $("#subclipStartTimeHours").spinner("value", startTimeHours);
    $("#subclipStartTimeMinutes").spinner("value", startTimeMinutes);
    $("#subclipStartTimeSeconds").spinner("value", startTimeSeconds);
    $("#subclipEndTimeHours").spinner("value", endTimeHours);
    $("#subclipEndTimeMinutes").spinner("value", endTimeMinutes);
    $("#subclipEndTimeSeconds").spinner("value", endTimeSeconds);
}
function GetEditedAsset(assetId) {
    var editedAsset;
    for (var i = 0; i < _editedAssets.length; i++) {
        if (_editedAssets[i].AssetId == assetId) {
            editedAsset = _editedAssets[i];
        }
    }
    return editedAsset;
}
function SetEditedAsset(markInSeconds, markOutSeconds) {
    var editedAsset = GetEditedAsset(_mediaAsset.id);
    if (editedAsset == null) {
        editedAsset = {
            AssetId: _mediaAsset.id,
            MarkIn: markInSeconds,
            MarkOut: markOutSeconds
        };
        _editedAssets.push(editedAsset);
    } else {
        editedAsset.MarkIn = markInSeconds;
        editedAsset.MarkOut = markOutSeconds;
    }
}
function SetAssetText(clearEdit) {
    var assetsTree = $.jstree.reference("#mediaAssets");
    var assetNode = assetsTree.get_node(_mediaAsset.id);
    var nodeText = assetsTree.get_text(assetNode);
    var editedText = "<span class='treeNodeEdited'>(Edited)</span> ";
    if (clearEdit) {
        nodeText = nodeText.replace(editedText, "");
    } else if (nodeText.indexOf(editedText) == -1) {
        nodeText = editedText + nodeText;
    }
    assetsTree.rename_node(assetNode, nodeText);
}
function DisplayVideoEditor(treeNode, authToken) {
    var title = "Azure Media Video Editor";
    var nodeRef = $.jstree.reference(treeNode.reference);
    _mediaAsset = nodeRef.get_node(treeNode.reference);
    if (_mediaAsset.a_attr.isStreamable) {
        var mediaPlayer = GetMediaPlayer(true);
        var sourceUrl = _mediaAsset.original.url;
        var protectionTypes = _mediaAsset.data.protectionTypes;
        mediaPlayer.src([{
            src: sourceUrl,
            type: GetSourceType(sourceUrl),
            protectionInfo: GetProtectionInfo(protectionTypes, authToken)
        }]);
        var dialogId = "editorDialog";
        var buttons = {};
        var onClose = function () {
            var editedAsset = GetEditedAsset(_mediaAsset.id);
            if (editedAsset != null) {
                SetAssetText(false);
            }
            $("#videoEditor").empty();
        };
        DisplayDialog(dialogId, title, null, buttons, null, null, onClose);
    } else {
        var dialogId = "subclipDialog";
        var buttons = {
            OK: function () {
                var startTimeHours = $("#subclipStartTimeHours").spinner("value");
                var startTimeMinutes = $("#subclipStartTimeMinutes").spinner("value");
                var startTimeSeconds = $("#subclipStartTimeSeconds").spinner("value");
                var endTimeHours = $("#subclipEndTimeHours").spinner("value");
                var endTimeMinutes = $("#subclipEndTimeMinutes").spinner("value");
                var endTimeSeconds = $("#subclipEndTimeSeconds").spinner("value");
                var markInSeconds = (startTimeHours * 3600) + (startTimeMinutes * 60) + startTimeSeconds;
                var markOutSeconds = (endTimeHours * 3600) + (endTimeMinutes * 60) + endTimeSeconds;
                SetEditedAsset(markInSeconds, markOutSeconds);
                SetAssetText(false);
                $(this).dialog("close");
            },
            Cancel: function () {
                $(this).dialog("close");
            }
        };
        DisplayDialog(dialogId, title, null, buttons);
        $("#subclipStartTimeHours").spinner({
            min: 0,
            max: 23
        });
        $("#subclipStartTimeMinutes").spinner({
            min: 0,
            max: 59
        });
        $("#subclipStartTimeSeconds").spinner({
            min: 0,
            max: 59
        });
        $("#subclipEndTimeHours").spinner({
            min: 0,
            max: 23
        });
        $("#subclipEndTimeMinutes").spinner({
            min: 0,
            max: 59
        });
        $("#subclipEndTimeSeconds").spinner({
            min: 0,
            max: 59
        });
        SetSubclipTimes(_mediaAsset.id);
    }
}
function SetVideoEdit(clipData) {
    if (clipData == null) {
        $(".amve-rendered-btn")[0].click();
        var editedAsset = GetEditedAsset(_mediaAsset.id);
        if (editedAsset != null) {
            var mediaPlayer = GetMediaPlayer(false);
            mediaPlayer.currentTime(editedAsset.MarkIn);
            $(".amve-setmarkin-btn")[0].click();
            mediaPlayer.currentTime(editedAsset.MarkOut);
            $(".amve-setmarkout-btn")[0].click();
        }
    } else {
        var markInSeconds = Math.floor(clipData.markIn);
        var markOutSeconds = Math.floor(clipData.markOut);
        SetEditedAsset(markInSeconds, markOutSeconds);
        $("#editorDialog").dialog("close");
    }
}
function ClearVideoEdit(treeNode) {
    if (_mediaAsset != null) {
        SetAssetText(true);
        for (var i = 0; i < _editedAssets.length; i++) {
            if (_editedAssets[i].AssetId == _mediaAsset.id) {
                _editedAssets.splice(i, 1);
            }
        }
    }
}
