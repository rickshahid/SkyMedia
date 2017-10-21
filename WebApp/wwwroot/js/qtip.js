function CreateTip(targetId, tipText, tipPosition, hideEvent) {
   $("#" + targetId).qtip({
        content: { text: tipText },
        position: tipPosition,
        show: { delay: 1000 },
        hide: { event: hideEvent }
    });
}
function CreateTipTop(targetId, tipText, adjustX, adjustY, hideEvent) {
    var tipPosition = { my: "bottom center", at: "top center", adjust: { x: adjustX, y: adjustY } };
    CreateTip(targetId, tipText, tipPosition, hideEvent);
}
function CreateTipTopLeft(targetId, tipText, adjustX, adjustY, hideEvent) {
    var tipPosition = { my: "bottom center", at: "top left", adjust: { x: adjustX, y: adjustY } };
    CreateTip(targetId, tipText, tipPosition, hideEvent);
}
function CreateTipBottom(targetId, tipText, adjustX, adjustY, hideEvent) {
    var tipPosition = { my: "top center", at: "bottom center", adjust: { x: adjustX, y: adjustY } };
    CreateTip(targetId, tipText, tipPosition, hideEvent);
}
function CreateTipBottomLeft(targetId, tipText, adjustX, adjustY, hideEvent) {
    var tipPosition = { my: "top center", at: "bottom left", adjust: { x: adjustX, y: adjustY } };
    CreateTip(targetId, tipText, tipPosition, hideEvent);
}
function CreateTipLeft(targetId, tipText, adjustX, adjustY, hideEvent) {
    var tipPosition = { my: "right center", at: "left center", adjust: { x: adjustX, y: adjustY } };
    CreateTip(targetId, tipText, tipPosition, hideEvent);
}
function CreateTipRight(targetId, tipText, adjustX, adjustY, hideEvent) {
    var tipPosition = { my: "left center", at: "right center", adjust: { x: adjustX, y: adjustY } };
    CreateTip(targetId, tipText, tipPosition, hideEvent);
}
function SetTipVisible(targetId, tipVisible) {
    $("#" + targetId).qtip("toggle", tipVisible);
}
