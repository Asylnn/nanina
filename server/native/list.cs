using System.Diagnostics.CodeAnalysis;

namespace Nanina;
public static class IEnumerableExtention
{

    public static T RandomElement<T>(this IEnumerable<T> enumerable)
    {
        return enumerable.ElementAt(new Random().Next(enumerable.Count()));
    }

    public static bool TryGet<K, T> (this IDictionary<K, T> dictionnary, K? key, [NotNullWhen(returnValue: true)] out T? value)
    {
        if(key is null)
        {
            value = default;
            return false;
        }
        else
        {
            var success = dictionnary.TryGetValue(key, out var check);
            if(check is null)
            {
                value = default;
                return false;
            }
            else 
            {
                value = check;
                return true;
            }
        }
    }
}