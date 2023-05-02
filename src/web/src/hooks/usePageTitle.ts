import { useEffect, useState } from 'react';

/**
 * A hook to update page title.
 * 
 * @category Hooks
 * @param title Page title to set.
 */
function usePageTitle(title: string | undefined | null): void {
    const [prevTitle, setPrevTitle] = useState(document.title);

    useEffect(() => {
        setPrevTitle(document.title);
        document.title = title ? `PortEval - ${title}` : 'PortEval';

        return () => {
            document.title = prevTitle;
        }
    }, [title]);
}

export default usePageTitle;