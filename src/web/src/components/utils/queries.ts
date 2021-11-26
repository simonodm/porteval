type Query = {
    isFetching?: boolean;
    isLoading?: boolean;
    error?: undefined | unknown;
}

export function checkIsLoaded(...args: Array<Query>): boolean {
    return !args.map(arg => arg.isFetching || arg.isLoading).reduce((prev, curr) => prev || curr);
}

export function checkIsError(...args: Array<Query>): boolean {
    return args.map(arg => !!arg.error).reduce((prev, curr) => prev || curr);
}