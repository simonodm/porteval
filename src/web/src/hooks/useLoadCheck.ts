import { useEffect, useState } from 'react';

type Query = {
    isFetching?: boolean;
    isLoading?: boolean;
    error?: undefined | unknown;
}

export default function useLoadCheck(...args: Array<Query>): [boolean, boolean] {
    const [ isLoaded, setIsLoaded ] = useState(false);
    const [ isError, setIsError ] = useState(false);

    useEffect(() => {
        setIsLoaded(!args.map(arg => arg.isFetching || arg.isLoading).reduce((prev, curr) => prev || curr));
        setIsError(args.map(arg => arg.error).reduce((prev, curr) => prev || curr) ? true : false);
    }, [args, ...args.map(arg => arg.isFetching || arg.isLoading), ...args.map(arg => arg.error)]);

    return [ isLoaded, isError ];
}