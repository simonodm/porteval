import { DateTime } from 'luxon';

export function camelToProperCase(str: string): string {
    if(str.length === 0) {
        return str;
    }
    
    const textSplitWords = str.replace(/([A-Z])/g, ' $1');
    const result = str[0].toUpperCase() + textSplitWords.slice(1);
    return result;
}

export function getPriceString(price?: number, decimalSeparator?: string, currencySymbol?: string): string {
    if(price === undefined) return '';

    let resultStr = price.toFixed(2);
    if(decimalSeparator !== undefined) {
        resultStr = resultStr.replace('.', decimalSeparator);
    }

    if(!currencySymbol) return resultStr;

    return currencySymbol + resultStr;
}

export function getPerformanceString(performance?: number, decimalSeparator?: string): string {
    if(performance === undefined) return '';

    const prefix = performance < 0 ? '-' : '+';
    let resultStr = Math.abs((performance * 100)).toFixed(2);
    if(decimalSeparator !== undefined) {
        resultStr = resultStr.replace('.', decimalSeparator);
    }

    return `${prefix}${resultStr}%`;
}

export function formatDateTimeString(isoDateTime: string, format: string): string {
    return DateTime.fromISO(isoDateTime).toFormat(format)
}