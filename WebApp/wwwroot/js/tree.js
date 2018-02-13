function SetNodeTips(node) {
    var targetId = node.data.clientId + "_anchor";
    var protectionTip = node.data.contentProtectionTip;
    if (protectionTip != "") {
        CreateTipRight(targetId, protectionTip);
    }
}
function LoadTreeNodes(filesView) {
    var treeNodes = $("#mediaAssets").jstree({
        "core": {
            "themes": {
                "variant": "large"
            },
            "data": {
                "url": function (node) {
                    return node.id == "#" ? "/asset/parents" : "/asset/children?assetId=" + node.data.entityId + "&getFiles=true"
                },
                "data": function (node) {
                    return { "id": node.id };
                }
            }
        },
        "plugins": filesView ? [] : ["checkbox"],
        "checkbox": {
            "keep_selected_style": false,
            "tie_selection": false,
            "three_state": false,
            "whole_node": false
        }
    });
    treeNodes.on("load_node.jstree", function (e, data) {
        if (data.node.data != null) {
            SetNodeTips(data.node);
        }
        for (var i = 0; i < data.node.children.length; i++) {
            var nodeId = data.node.children[i];
            var node = data.instance.get_node(nodeId);
            SetNodeTips(node);
        }
    });
}