import { DateTime } from 'luxon';

export function camelToProperCase(str: string): string {
    if(str.length === 0) {
        return str;
    }
    
    const textSplitWords = str.replace(/([A-Z])/g, ' $1');
    const result = str[0].toUpperCase() + textSplitWords.slice(1);
    return result;
}

export function getPriceString(price?: number, currencySymbol?: string): string {
    if(price === undefined) return '';
    if(!currencySymbol) return price.toFixed(2);

    return currencySymbol + price.toFixed(2);
}

export function getPerformanceString(performance?: number): string {
    if(performance === undefined) return '';

    const prefix = performance < 0 ? '-' : '+';

    return `${prefix}${Math.abs((performance * 100)).toFixed(2)}%`;
}

export function getDateTimeLocaleString(isoDateTime: string): string {
    return DateTime.fromISO(isoDateTime).toLocaleString(DateTime.DATETIME_SHORT);
}