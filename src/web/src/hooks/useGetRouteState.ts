 import { useLocation } from 'react-router';

export default function useGetRouteState<T>(propName: string): T | undefined {
    const location = useLocation<{ [propName: string]: T }>();
    if(location.state && Object.prototype.hasOwnProperty.call(location.state, propName)) {
        return location.state[propName];
    }

    return undefined;
}