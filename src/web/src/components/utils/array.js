export function convertToObjectUsingKey(array, key, properties = {}) {
    let result = {};
    array.forEach(element => {
        result[element[key]] = properties ? properties : element;
    });

    return result;
}