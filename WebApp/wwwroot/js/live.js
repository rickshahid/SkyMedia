function GetNewEventName() {
    var eventName = $("#name").val();
    if (eventName == "") {
        var inputProtocol = $("#inputProtocol:checked").val();
        var encodingType = $("#encodingType:checked").val();
        eventName = encodingType != "None" ? "Live-Encoding" : "Live-Streaming";
        if (inputProtocol == "FragmentedMP4") {
            inputProtocol = "FMP4";
        }
        eventName = eventName + "-" + inputProtocol;
    }
    return eventName;
}
function CreateLiveEvent() {
    var title = "Confirm Live Event Create";
    var message = "Are you sure you want to create a new live event?";
    var onConfirm = function () {
        SetCursor(true);
        $.post("/live/createEvent",
            {
                eventName: GetNewEventName(),
                eventDescription: $("#description").val(),
                eventTags: _jsonEditor.getText(),
                inputProtocol: $("#inputProtocol:checked").val(),
                encodingType: $("#encodingType:checked").val(),
                encodingPresetName: $("#encodingPresetName").val(),
                streamingPolicyName: $("#streamingPolicies").val(),
                lowLatency: $("#lowLatency").prop("checked"),
                autoStart: false
            },
            function (liveEvent) {
                SetCursor(false);
                var buttons = {
                    OK: function () {
                        window.location = window.location.href;
                        $(this).dialog("close");
                    }
                };
                DisplayMessage("Live Event Created", liveEvent.name, buttons);
            }
        );
        $(this).dialog("close");
    };
    ConfirmMessage(title, message, onConfirm);
}
function CreateLiveOutput() {
    var title = "Confirm Live Event Output Create";
    var message = "Are you sure you want to create a new live event output?";
    var onConfirm = function () {
        SetCursor(true);
        $.post("/live/createOutput",
            {
                eventName: $("#liveEvents").val(),
                eventOutputName: $("#name").val(),
                eventOutputDescription: $("#description").val(),
                outputAssetName: $("#outputAssetName").val(),
                archiveWindowMinutes: $("#archiveWindowMinutes").val()
            },
            function (liveOutput) {
                SetCursor(false);
                var buttons = {
                    OK: function () {
                        window.location = window.location.href;
                        $(this).dialog("close");
                    }
                };
                DisplayMessage("Live Event Output Created", liveOutput.name, buttons);
            }
        );
        $(this).dialog("close");
    };
    ConfirmMessage(title, message, onConfirm);
}
function UpdateLiveEvent() {
    var eventName = $("#name").val();
    if (eventName != "") {
        var title = "Confirm Live Event Update";
        var message = "Are you sure you want to update the '" + eventName + "' event?";
        var onConfirm = function () {
            SetCursor(true);
            $.post("/live/updateEvent",
                {
                    eventName: eventName,
                    eventDescription: $("#description").val(),
                    eventTags: _jsonEditor.getText(),
                    encodingType: $("#encodingType:checked").val(),
                    encodingPresetName: $("#encodingPresetName").val()
                },
                function (liveEvent) {
                    SetCursor(false);
                    var buttons = {
                        OK: function () {
                            window.location = window.location.href;
                            $(this).dialog("close");
                        }
                    };
                    DisplayMessage("Live Event Updated", liveEvent.name, buttons);
                }
            );
            $(this).dialog("close");
        };
        ConfirmMessage(title, message, onConfirm);
    }
}