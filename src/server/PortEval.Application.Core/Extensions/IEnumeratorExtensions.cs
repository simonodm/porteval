using System;
using System.Collections.Generic;

namespace PortEval.Application.Core.Extensions;

/// <summary>
///     Implements <see cref="IEnumerator{T}" /> extension methods.
/// </summary>
internal static class IEnumeratorExtensions
{
    /// <summary>
    ///     Finds the last element in an enumerator which fulfills the specified criteria, while the next element does not.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="enumerator">Enumerator to iterate through.</param>
    /// <param name="predicate">Predicate to check if an element fulfills the criteria.</param>
    /// <param name="callback">Callback to execute on each passed element.</param>
    /// <returns>The next element which fulfills the predicate if such exists, <c>default</c> otherwise.</returns>
    public static T FindNextElementInEnumerator<T>(this IEnumerator<T> enumerator, Predicate<T> predicate,
        Action<T> callback = null)
    {
        try
        {
            T previousElement = default;

            while (enumerator.Current != null && predicate(enumerator.Current))
            {
                previousElement = enumerator.Current;
                callback?.Invoke(previousElement);
                if (!enumerator.MoveNext())
                {
                    return previousElement;
                }
            }

            return previousElement;
        }
        catch (InvalidOperationException)
        {
            return default;
        }
    }
}