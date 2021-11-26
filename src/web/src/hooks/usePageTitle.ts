import { useEffect } from 'react';

export default function usePageTitle(title: string): void {
    useEffect(() => {
        document.title = title ? `PortEval - ${title}` : 'PortEval';
    }, [title]);
}