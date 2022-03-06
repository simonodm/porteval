import { useState } from 'react';

// Credit: https://usehooks.com/useLocalStorage/
// Modified for TypeScript

type SetStorageCallback = (value: unknown) => void;

export default function useLocalStorage(key: string, initialValue?: string): [string, SetStorageCallback] {
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