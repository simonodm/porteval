// While the result number of seconds may not be applicable to arithmetic with dates (due to different month lengths, leap years, etc.), it
// is deterministic, which makes it applicable for duration comparisons.
function getDurationSeconds(duration: globalThis.Duration): number {
    let result = 0;
    if(duration.years !== undefined) {
        result += duration.years * 365 * 24 * 60 * 60;
    }
    if(duration.months !== undefined) {
        result += duration.months * 30 * 24 * 60 * 60;
    }
    if(duration.weeks !== undefined) {
        result += duration.weeks * 7 * 24 * 60 * 60;
    }
    if(duration.days !== undefined) {
        result += duration.days * 24 * 60 * 60;
    }
    if(duration.hours !== undefined) {
        result += duration.hours * 60 * 60;
    }
    if(duration.minutes !== undefined) {
        result += duration.minutes * 60;
    }
    if(duration.seconds !== undefined) {
        result += duration.seconds;
    }

    return result;
}

export function durationGreaterThan(first: globalThis.Duration, second: globalThis.Duration) {
    return getDurationSeconds(first) > getDurationSeconds(second);
}

export function durationGreaterThanOrEqualTo(first: globalThis.Duration, second: globalThis.Duration) {
    return getDurationSeconds(first) >= getDurationSeconds(second);
}

export function durationLessThan(first: globalThis.Duration, second: globalThis.Duration) {
    return getDurationSeconds(first) < getDurationSeconds(second);
}

export function durationLessThanOrEqualTo(first: globalThis.Duration, second: globalThis.Duration) {
    return getDurationSeconds(first) <= getDurationSeconds(second);
}