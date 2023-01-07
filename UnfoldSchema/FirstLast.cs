



[Flags]
enum FirstLast { None = 0, First = 1, Last = 2 }



static class EnumerableExtensions
{
    public static IEnumerable<(T, FirstLast)> WithFirstLast<T>(this IEnumerable<T> items)
    {
        var enumerator = items.GetEnumerator();
        if (!enumerator.MoveNext()) { yield break; };
        var state = FirstLast.First;
        var current = enumerator.Current;
        while (enumerator.MoveNext())
        {
            yield return (current, state);
            state &= ~FirstLast.First; // not the first anymore since MoveNext succeeded twice
            current = enumerator.Current;
        }
        state |= FirstLast.Last; // add the Last flag
        yield return (current, state);
    }
}
