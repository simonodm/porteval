import { format } from 'date-fns';
import { UserSettings } from '../types';

/**
 * Converts camelCase to ProperCase.
 * 
 * @category Utilities
 * @subcategory String
 * @param str String to convert. 
 * @returns Provided string in ProperCase.
 */
export function camelToProperCase(str: string): string {
    if(str.length === 0) {
        return str;
    }
    
    const textSplitWords = str.replace(/([A-Z])/g, ' $1');
    const result = str[0].toUpperCase() + textSplitWords.slice(1);
    return result;
}

function getBrowserLocales(): Array<string> | undefined {
    const browserLocales = navigator.languages === undefined ? [navigator.language] : navigator.languages;
    if(!browserLocales) {
        return undefined;
    }

    return browserLocales.map(locale => locale.trim());
}

function getCurrencySymbol (currencyCode: string) {
    const browserLocales = getBrowserLocales();
    
    return (0).toLocaleString(
      browserLocales?.length ? browserLocales[0] : 'en-US',
      {
        style: 'currency',
        currency: currencyCode,
        minimumFractionDigits: 0,
        maximumFractionDigits: 0
      }
    ).replace(/\d/g, '').trim()
  }

/**
 * Converts the provided number to a price string.
 * 
 * @category Utilities
 * @subcategory String
 * @param price Price as a decimal number.
 * @param currencyCode Code of the price currency.
 * @param settings User settings.
 * @returns A string representing the number as a price.
 */
export function getPriceString(
    price?: number, currencyCode?: string, settings?: UserSettings
): string {
    if(price === undefined) return '';

    let resultStr = Math.abs(price).toFixed(2);
    if(settings !== undefined) {
        resultStr = resultStr.replace('.', settings.decimalSeparator);
        resultStr = splitThousands(resultStr, settings.thousandsSeparator);
    }

    const currencySymbol = currencyCode ? getCurrencySymbol(currencyCode) : '';

    return `${price < 0 ? '-' : ''}${currencySymbol}${resultStr}`;
}

/**
 * Converts the provided number to a relative performance string.
 * 
 * @category Utilities
 * @subcategory String
 * @param performance Performance as a decimal number.
 * @param settings User settings.
 * @returns A string representing the number as a percentage change.
 */
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

/**
 * Determines whether the provided date/time format is valid for usage in PortEval.
 * 
 * @category Utilities
 * @subcategory String
 * @param dtFormat Format to test.
 * @returns `true` if the provided format is valid, `false` otherwise.
 */
export function isValidDateTimeFormat(dtFormat: string): boolean {
    try {
        format(Date.parse('1999-01-01T00:00:00Z'), dtFormat);
        return true;
    } catch (ex) {
        return false;
    }
}

/**
 * Converts the provided ISO date to the specified format.
 * 
 * @category Utilities
 * @subcategory String
 * @param isoDateTime ISO date/time.
 * @param dtFormat Date/time format to convert to.
 * @returns Converted date/time.
 */
export function formatDateTimeString(isoDateTime: string, dtFormat: string): string {
    return format(Date.parse(isoDateTime), dtFormat);
}

function splitThousands(number: string, separator: string): string {
    return number.replace(/\B(?=(\d{3})+(?!\d))/g, separator);
}