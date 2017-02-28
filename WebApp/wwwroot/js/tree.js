function LoadTreeNodes(workflowView) {
    $("#mediaAssets").jstree({
        "core": {
            "check_callback": function (operation, node, node_parent, node_position, more) {
                var operationAllowed = false;
                if (operation == "rename_node") {
                    operationAllowed = true;
                }
                return operationAllowed;
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
        "plugins": workflowView ? ["checkbox"] : [],
        "checkbox": {
            "keep_selected_style": false,
            "tie_selection": false,
            "three_state": false,
            "whole_node": false
        }
    });
}
