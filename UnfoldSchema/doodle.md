
Linq doodle exercise: 

given 

```
[Flags]
enum FirstLast { None = 0, First = 1, Last = 2 }
```

write an extension 
```
public static IEnumerable<(T, FirstLast)> WithFirstLast<T>(this IEnumerable<T> items)
```

so that 

```
Console.WriteLine("{0}", string.Join("; ", Enumerable.Range(0, 1).WithFirstLast()));
Console.WriteLine("{0}", string.Join("; ", Enumerable.Range(0, 2).WithFirstLast()));
Console.WriteLine("{0}", string.Join("; ", Enumerable.Range(0, 3).WithFirstLast()));
```

returns 

```
(0, First, Last)
(0, First); (1, Last)
(0, First); (1, None); (2, Last)
```
