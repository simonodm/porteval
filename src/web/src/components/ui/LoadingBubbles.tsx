import { MAIN_COLOR } from '../../constants';
import React from 'react';
import ReactLoading from 'react-loading';

export default function LoadingBubbles(): JSX.Element {
    return <ReactLoading color={MAIN_COLOR} height="32px" type="bubbles"
        width="32px"
    />
}