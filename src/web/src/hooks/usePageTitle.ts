import { useEffect } from 'react';

/**
 * A hook to update page title.
 * 
 * @category Hooks
 * @param title Page title to set.
 */
function usePageTitle(title: string): void {
    useEffect(() => {
        document.title = title ? `PortEval - ${title}` : 'PortEval';
    }, [title]);
}

export default usePageTitle;