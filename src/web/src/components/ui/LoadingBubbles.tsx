import React from 'react';
import ReactLoading from 'react-loading';
import { MAIN_COLOR } from '../../constants';

/**
 * Renders a 32x32 loading bubbles animation.
 * 
 * @category UI
 * @component
 */
export default function LoadingBubbles(): JSX.Element {
    return (
        <ReactLoading color={MAIN_COLOR} height="32px" type="bubbles"
            width="32px"
        />
    );
}