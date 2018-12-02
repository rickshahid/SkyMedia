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
    var title = "Confirm Create Live Event";
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
    if (ValidLiveOutput()) {
        var title = "Confirm Create Live Event Output";
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
}
function ValidLiveOutput() {
    var validFields = true;
    var tipText = "Required Field";
    var eventOutputName = $("#name").val();
    if (eventOutputName == "") {
        CreateTipRight("name", tipText);
        SetTipVisible("name", true);
        validFields = false;
    }
    var eventName = $("#liveEvents").val();
    if (eventName == "") {
        CreateTipRight("liveEvents", tipText);
        SetTipVisible("liveEvents", true);
        validFields = false;
    }
    var outputAssetName = $("#outputAssetName").val();
    if (outputAssetName == "") {
        CreateTipRight("outputAssetName", tipText);
        SetTipVisible("outputAssetName", true);
        validFields = false;
    }
    return validFields;
}
function UpdateLiveEvent() {
    var eventName = $("#name").val();
    if (eventName != "") {
        var title = "Confirm Update Live Event";
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
function SetArchiveWindow(defaultMinutes) {
    $("#archiveWindowMinutes").spinner({
        min: 0,
        max: 1500,
        change: function (event, ui) {
            var minutes = $(this).spinner("value");
            SetArchiveWindowHour(minutes);
        },
        spin: function (event, ui) {
            SetArchiveWindowHour(ui.value);
        }
    });
    $("#archiveWindowMinutes").spinner("value", defaultMinutes);
}
function SetArchiveWindowHour(minutes) {
    var unitLabel = " Hour";
    var archiveWindowHours = (minutes / 60).toFixed(2);
    if (archiveWindowHours != 1) {
        unitLabel = unitLabel + "s";
    }
    archiveWindowHours = archiveWindowHours + unitLabel;
    $("#archiveWindowHours").text(archiveWindowHours);
}