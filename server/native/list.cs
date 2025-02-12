namespace Nanina;
public static class IEnumerableExtention {

    public static T RandomElement<T>(this IEnumerable<T> enumerable){
        return enumerable.ElementAt(new Random().Next(enumerable.Count()));
    }
}