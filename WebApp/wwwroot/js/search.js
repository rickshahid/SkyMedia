function DisplaySearchParameters() {
    var title = "Media Account Search";
    var onConfirm = function () {
        SetCursor(true);
        $.get("/search/query",
            {
                searchQuery: $("#searchQuery").val()
            },
            function (searchResults) {
                SetCursor(false);
                DisplaySearchResults(searchResults);
            }
        );
        $(this).dialog("close");
    };
    ConfirmDialog("searchDialog", title, onConfirm);
    $("#searchQuery").focus();
}
function DisplaySearchResults(searchResults) {
    if (_jsonEditor != null) {
        _jsonEditor.destroy();
    }
    CreateJsonEditor("searchResults", "Media Account Search Results", searchResults);
}