function getDurationSeconds(duration: globalThis.Duration): number {
    // While the result number of seconds may not be applicable to arithmetic
    // with dates (due to different month lengths, leap years, etc.),
    // it is deterministic, which makes it applicable for duration comparisons.
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

/**
 * Compares two durations and returns `true` if the first duration is longer than the second duration.
 * 
 * @category Utilities
 * @subcategory Date
 * @param first 
 * @param second 
 * @returns `true` if the first duration is longer than the second duration, `false` otherwise.
 */
function durationGreaterThan(first: globalThis.Duration, second: globalThis.Duration): boolean {
    return getDurationSeconds(first) > getDurationSeconds(second);
}

/**
 * Compares two durations and returns `true` if the first duration is longer than or equal to the second duration.
 * 
 * @category Utilities
 * @subcategory Date
 * @param first 
 * @param second 
 * @returns `true` if the first duration is longer than or equal to the second duration, `false` otherwise.
 */
function durationGreaterThanOrEqualTo(first: globalThis.Duration, second: globalThis.Duration): boolean {
    return getDurationSeconds(first) >= getDurationSeconds(second);
}

/**
 * Compares two durations and returns `true` if the first duration is shorter than the second duration.
 * 
 * @category Utilities
 * @subcategory Date
 * @param first 
 * @param second 
 * @returns `true` if the first duration is shorter than the second duration, `false` otherwise.
 */
function durationLessThan(first: globalThis.Duration, second: globalThis.Duration): boolean {
    return getDurationSeconds(first) < getDurationSeconds(second);
}

/**
 * Compares two durations and returns `true` if the first duration is shorter than or equal to the second duration.
 * 
 * @category Utilities
 * @subcategory Date
 * @param first 
 * @param second 
 * @returns `true` if the first duration is shorter than or equal to the second duration, `false` otherwise.
 */
function durationLessThanOrEqualTo(first: globalThis.Duration, second: globalThis.Duration): boolean {
    return getDurationSeconds(first) <= getDurationSeconds(second);
}

export {
    durationGreaterThan,
    durationGreaterThanOrEqualTo,
    durationLessThan,
    durationLessThanOrEqualTo
}