function isTimecodeFragment(timeSeconds, timeScale, fragment) {
    var timeStart = fragment.start / timeScale;
    var timeEnd = (fragment.start + fragment.duration) / timeScale;
    return timeSeconds >= timeStart && timeSeconds <= timeEnd;
}
