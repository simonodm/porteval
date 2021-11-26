import { useLocation } from 'react-router';

export default function useGetQueryParam(paramName: string): string | null {
    const location = useLocation();
    const params = new URLSearchParams(location.search);
    return params.get(paramName);
}