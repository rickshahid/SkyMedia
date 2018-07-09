function ValidateAccount(accountMessage, accountUrl) {
    if (accountMessage != "") {
        var onClose = null;
        if (accountUrl != "") {
            var onClose = function () {
                window.location.href = accountUrl;
            }
        }
        DisplayMessage("Account Message", accountMessage, null, onClose);
    }
}