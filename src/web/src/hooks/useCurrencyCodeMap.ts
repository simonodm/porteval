import { useMemo } from "react";
import { Currency } from "../types";

export default function useCurrencyCodeMap(currencies?: Array<Currency>): Record<string, Currency> {
    return useMemo(() => {
        const map: Record<string, Currency> = {};

        currencies?.forEach(currency => map[currency.code] = currency);

        return map;
    }, currencies);
}