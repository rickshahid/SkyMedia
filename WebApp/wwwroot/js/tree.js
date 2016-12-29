function LoadTreeAssets(authToken, cdnUrl, workflowView) {
    var plugins = workflowView ? ["contextmenu", "checkbox"] : [];
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
                            "label": "Set Video Marks",
                            "icon": cdnUrl + "/MediaAssetEditSet.png",
                            "action": function (treeNode) {
                                DisplayVideoEditor(treeNode, authToken);
                            }
                        },
                        "Clear": {
                            "label": "Clear Video Marks",
                            "icon": cdnUrl + "/MediaAssetEditClear.png",
                            "action": function (treeNode) {
                                ClearVideoMarks(treeNode);
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
        if (AssetEdited(_mediaAsset.id)) {
            SetAssetText(true);
        }
        $("#videoEditor").empty();
    };
    DisplayDialog(dialogId, title, null, buttons, null, null, onClose);
}
function SetVideoMarks(clipData) {
    if (clipData == null) {
        $(".amve-rendered-btn")[0].click();
        _editedAssets = new Array();
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
            _mediaAsset.Edited = true;
        }
        $("#editorDialog").dialog("close");
    }
}
function ClearVideoMarks(treeNode) {
    SetAssetText(false);
    for (var i = 0; i < _editedAssets.length; i++) {
        if (_editedAssets[i].AssetId == _mediaAsset.id) {
            _editedAssets.splice(i, 1);
        }
    }
}
function AssetEdited(assetId) {
    var assetEdited = false;
    if (_editedAssets != null) {
        for (var i = 0; i < _editedAssets.length; i++) {
            if (_editedAssets[i].AssetId == assetId) {
                assetEdited = true;
            }
        }
    }
    return assetEdited;
}
function SetAssetText(editActive) {
    var editedText = "(<i>Edited</i>) ";
    var assetsTree = $.jstree.reference("#mediaAssets");
    var assetNode = assetsTree.get_node(_mediaAsset.id);
    var nodeText = assetsTree.get_text(assetNode);
    if (editActive) {
        nodeText = editedText + nodeText;
    } else {
        nodeText = nodeText.replace(editedText, "");
    }
    assetsTree.rename_node(assetNode, nodeText);
}
