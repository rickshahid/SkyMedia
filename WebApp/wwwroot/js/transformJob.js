function CreateJob() {
    var title = "Confirm Job Create";
    var message = "Are you sure you want to create a new job?";
    var onConfirm = function () {
        SetCursor(true);
        $.post("/job/create",
            {
                name: $("#name").val(),
                description: $("#description").val()
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