import useLocalStorage from './useLocalStorage';

import { useEffect, useState } from 'react';
import { UserSettings } from '../types';
import { DATE_FORMAT_STORAGE_KEY,
    DEFAULT_DATE_FORMAT,
    TIME_FORMAT_STORAGE_KEY,
    DEFAULT_TIME_FORMAT,
    DECIMAL_SEPARATOR_STORAGE_KEY,
    DEFAULT_DECIMAL_SEPARATOR,
    THOUSANDS_SEPARATOR_STORAGE_KEY,
    DEFAULT_THOUSANDS_SEPARATOR
} from '../constants';

/**
 * A callback to update user settings.
 * 
 * @category Hooks
 * @subcategory Types
 */
type SetUserSettingsCallback = (newSettings: UserSettings) => void;

/**
 * A hook to read and update local user settings.
 * 
 * @category Hooks
 * @returns Current user settings and a callback to update them.
 */
function useUserSettings(): [UserSettings, SetUserSettingsCallback] {
    const [dateFormat, setDateFormat] = useLocalStorage(DATE_FORMAT_STORAGE_KEY, DEFAULT_DATE_FORMAT);
    const [timeFormat, setTimeFormat] = useLocalStorage(TIME_FORMAT_STORAGE_KEY, DEFAULT_TIME_FORMAT);
    const [decimalSeparator, setDecimalSeparator] = useLocalStorage(
        DECIMAL_SEPARATOR_STORAGE_KEY,
        DEFAULT_DECIMAL_SEPARATOR
    );
    const [thousandsSeparator, setThousandsSeparator] = useLocalStorage(
        THOUSANDS_SEPARATOR_STORAGE_KEY,
        DEFAULT_THOUSANDS_SEPARATOR
    );

    const [settings, setSettings] = useState({
        dateFormat,
        timeFormat,
        decimalSeparator,
        thousandsSeparator
    });

    useEffect(() => {
        setSettings({
            dateFormat,
            timeFormat,
            decimalSeparator,
            thousandsSeparator
        });
    }, [dateFormat, timeFormat, decimalSeparator]);

    const changeSettings = (newSettings: UserSettings) => {
        setDateFormat(newSettings.dateFormat);
        setTimeFormat(newSettings.timeFormat);
        setDecimalSeparator(newSettings.decimalSeparator);
        setThousandsSeparator(newSettings.thousandsSeparator);
    };

    return [settings, changeSettings];
}

export default useUserSettings;