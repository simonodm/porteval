import { useLocation, Location } from 'react-router';

/**
 * A hook to retrieve route state.
 * 
 * @category Hooks
 * @template T State type.
 * @param propName Route state property name.
 * @returns State at the specified key if it exists, `undefined` otherwise.
 */
function useGetRouteState<T>(propName: string): T | undefined {
    const location = useLocation() as Location & { [propName: string]: T };
    if(location.state && Object.prototype.hasOwnProperty.call(location.state, propName)) {
        return location.state[propName];
    }

    return undefined;
}

export default useGetRouteState;