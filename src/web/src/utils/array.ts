type PrimitiveValue = number | string | symbol;
type Dictionary = {
    [key: string]: boolean;
}

/**
 * Removes all duplicates from an array based on the provided predicate.
 * 
 * @category Utilities
 * @subcategory Array
 * @param arr Array to remove duplicates from.
 * @param predicate Predicate to select property which might contain duplicates.
 * @returns A new array with duplicates removed.
 */
function removeDuplicates<T>(arr: Array<T>, predicate: (el: T) => PrimitiveValue): Array<T> {
    const seenValues: Dictionary = {};

    return arr.filter(element => {
        if(!Object.hasOwnProperty.call(seenValues, predicate(element))) {
            seenValues[predicate(element).toString()] = true;
            return true;
        }
        return false;
    })
}

export {
    removeDuplicates
}