import { format } from 'date-fns';
import { UserSettings } from '../types';

export function camelToProperCase(str: string): string {
    if(str.length === 0) {
        return str;
    }
    
    const textSplitWords = str.replace(/([A-Z])/g, ' $1');
    const result = str[0].toUpperCase() + textSplitWords.slice(1);
    return result;
}

export function getPriceString(
    price?: number, currencySymbol?: string, settings?: UserSettings
): string {
    if(price === undefined) return '';

    let resultStr = Math.abs(price).toFixed(2);
    if(settings !== undefined) {
        resultStr = resultStr.replace('.', settings.decimalSeparator);
        resultStr = splitThousands(resultStr, settings.thousandsSeparator);
    }

    return `${price < 0 ? '-' : ''}${currencySymbol ?? ''}${resultStr}`;
}

export function getPerformanceString(
    performance?: number, settings?: UserSettings
): string {
    if(performance === undefined) return '';

    const prefix = performance < 0 ? '-' : '+';
    let resultStr = Math.abs((performance * 100)).toFixed(2);
    if(settings !== undefined) {
        resultStr = resultStr.replace('.', settings.decimalSeparator);
        resultStr = splitThousands(resultStr, settings.thousandsSeparator);
    }

    return `${prefix}${resultStr}%`;
}

export function isValidDateTimeFormat(dtFormat: string): boolean {
    try {
        format(Date.parse("1999-01-01T00:00:00Z"), dtFormat);
        return true;
    }
    catch (ex) {
        return false;
    }
}

export function formatDateTimeString(isoDateTime: string, dtFormat: string): string {
    return format(Date.parse(isoDateTime), dtFormat);
}

function splitThousands(number: string, separator: string): string {
    return number.replace(/\B(?=(\d{3})+(?!\d))/g, separator);
}