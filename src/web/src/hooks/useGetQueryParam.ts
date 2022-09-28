import { useLocation } from 'react-router';

/**
 * A hook to retrieve URL query parameters.
 * 
 * @category Hooks
 * @param paramName Query parameter name
 * @return Query parameter value if it exists, `null` otherwise.
 */
function useGetQueryParam(paramName: string): string | null {
    const location = useLocation();
    const params = new URLSearchParams(location.search);
    return params.get(paramName);
}

export default useGetQueryParam;