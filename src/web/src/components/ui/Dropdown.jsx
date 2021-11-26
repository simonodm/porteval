export default function Dropdown({ values, callback }) {
    return (
        <select onChange={event => callback(event.target.value)}>
            {values.map(value => <option value={value}>{value}</option>)}
        </select>
    )
}