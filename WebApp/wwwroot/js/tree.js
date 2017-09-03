function LoadTreeNodes(browserView) {
    $("#mediaAssets").jstree({
        "core": {
            "themes": {
                "variant": "large"
            },
            "data": {
                "url": function (node) {
                    return (node.id == "#") ? "/asset/parents" : "/asset/children?assetId=" + node.id;
                },
                "data": function (node) {
                    return { "id": node.id };
                }
            }
        },
        "plugins": browserView ? [] : ["checkbox"],
        "checkbox": {
            "keep_selected_style": false,
            "tie_selection": false,
            "three_state": false,
            "whole_node": false
        }
    });
}