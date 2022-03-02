type PrimitiveValue = number | string | symbol;
type Dictionary = {
    [key: string]: boolean;
}

export default function removeDuplicates<T>(arr: Array<T>, predicate: (el: T) => PrimitiveValue): Array<T> {
    const seenValues: Dictionary = {};

    return arr.filter(element => {
        if(!Object.hasOwnProperty.call(seenValues, predicate(element))) {
            seenValues[predicate(element).toString()] = true;
            return true;
        }
        return false;
    })
}