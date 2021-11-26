export function logProfilerResult(id, phase, actualDuration, baseDuration, startTime, commitTime, interactions) {
    console.log(phase + ": " + id);
    console.log("Start time: " + startTime.toFixed(2) + "ms");
    console.log("Commit time: " + commitTime.toFixed(2) + "ms");
    console.log("Base duration: " + baseDuration.toFixed(2) + "ms");
    console.log("Actual duration: " + actualDuration.toFixed(2) + "ms");
}