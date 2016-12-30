var _mediaAsset, _editedAssets, _inputAssets;
function LoadTreeAssets(authToken, cdnUrl, workflowView) {
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
                if (treeNode.a_attr.isStreamable) {
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
function DisplayVideoEditor(treeNode, authToken) {
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
        var editedAsset = GetEditedAsset(_mediaAsset.id);
        if (editedAsset != null) {
            SetAssetText(true);
        }
        $("#videoEditor").empty();
    };
    DisplayDialog(dialogId, title, null, buttons, null, null, onClose);
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
        var editedAsset;
        var markIn = Math.floor(clipData.markIn);
        var markOut = Math.floor(clipData.markOut);
        for (var i = 0; i < _editedAssets.length; i++) {
            if (_editedAssets[i].AssetId == _mediaAsset.id) {
                _editedAssets[i].MarkId = markIn;
                _editedAssets[i].MarkOut = markOut;
                editedAsset = _editedAssets[i];
            }
        }
        if (editedAsset == null) {
            editedAsset = {
                AssetId: _mediaAsset.id,
                MarkIn: markIn,
                MarkOut: markOut
            };
            _editedAssets.push(editedAsset);
        }
        $("#editorDialog").dialog("close");
    }
}
function ClearVideoEdit(treeNode) {
    SetAssetText(false);
    for (var i = 0; i < _editedAssets.length; i++) {
        if (_editedAssets[i].AssetId == _mediaAsset.id) {
            _editedAssets.splice(i, 1);
        }
    }
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
function SetAssetText(editActive) {
    var editedText = "(<i>Edited</i>) ";
    var assetsTree = $.jstree.reference("#mediaAssets");
    var assetNode = assetsTree.get_node(_mediaAsset.id);
    var nodeText = assetsTree.get_text(assetNode);
    if (editActive) {
        if (nodeText.indexOf(editedText) == -1) {
            nodeText = editedText + nodeText;
        }
    } else {
        nodeText = nodeText.replace(editedText, "");
    }
    assetsTree.rename_node(assetNode, nodeText);
}
