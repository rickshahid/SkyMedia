function SetAudioAnalyzer(videoAnalyzer) {
    $("#audioAnalyzerPreset").prop("checked", false);
    $("#audioAnalyzerPreset").prop("disabled", videoAnalyzer.checked);
}