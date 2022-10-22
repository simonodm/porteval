import { useState } from 'react';

// Credit: https://usehooks.com/useLocalStorage/
// Modified for TypeScript

/**
 * A callback to set the value of key in local storage.
 * @category Hooks
 * @subcategory Types
 */
type SetStorageCallback = (value: unknown) => void;

/**
 * A hook to manage local storage state at the specified key.
 * 
 * @category Hooks
 * @param key Local storage state key.
 * @param initialValue Default value to use if no value is available at the specified key.
 * @returns The value in the local storage and a callback to modify it.
 */
function useLocalStorage(key: string, initialValue?: string): [string, SetStorageCallback] {
    // State to store our value
    // Pass initial state function to useState so logic is only executed once
    const [storedValue, setStoredValue] = useState(() => {
        if (window === undefined) {
            return initialValue;
        }

        try {
            const item = window.localStorage.getItem(key);
            return item ? JSON.parse(item) : initialValue;
        } catch (error) {
            console.log(error);
            return initialValue;
        }
    });

    // Return a wrapped version of useState's setter function that
    // persists the new value to localStorage.
    const setValue: SetStorageCallback = (value) => {
        try {
            // Allow value to be a function so we have same API as useState
            const valueToStore = value instanceof Function ? value(storedValue) : value;

            setStoredValue(valueToStore);
            if (window !== undefined) {
                window.localStorage.setItem(key, JSON.stringify(valueToStore));
            }
        } catch (error) {
            console.log(error);
        }
    };

    return [storedValue, setValue];
}

export default useLocalStorage;