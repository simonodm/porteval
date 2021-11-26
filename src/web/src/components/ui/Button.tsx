import React from 'react';
import './Button.css';

type Props = {
    value: string,
    onClick: () => void,
    active: boolean
}

export default function Button({ value = 'Button', onClick, active=true }: Props): JSX.Element {
    return <input type="button" className="button" value={value} onClick={onClick} disabled={!active} />
}