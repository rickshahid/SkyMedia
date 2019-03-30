function SearchAccount() {
    var title = "Media Account Search Query";
    var onConfirm = function () {
        var searchQuery = $("#searchQuery").val();
        if (searchQuery != "") {
            SetCursor(true);
            $.get("/insight/query",
                {
                    searchQuery: searchQuery
                },
                function (searchResults) {
                    SetCursor(false);
                    var title = "Media Account Search Results (" + searchQuery + ")";
                    DisplayJson(title, searchResults);
                }
            );
        }
        $(this).dialog("close");
    };
    ConfirmDialog("searchDialog", title, onConfirm);
    $("#searchQuery").focus();
}