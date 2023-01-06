// See https://aka.ms/new-console-template for more information

public record class Path(ImmutableList<(string Name, IEdmType Type)> Segments)
{
    public static Path Empty = new Path(ImmutableList<(string Name, IEdmType Type)>.Empty);

    public override string ToString()
    {
        string Kind(IEdmType type) =>
         (type is IEdmCollectionType coll) ? $"Collection<{Kind(coll.ElementType.Definition)}>" : type.TypeKind.ToString();

        // return $"/{string.Join("/", Segments.Select(s => s.Name))}: {ResponseType.FullTypeName()}{{{Kind(ResponseType)}}}";
        return $"/{string.Join("/", Segments.Select(s => s.Name))}: {ResponseType.FullTypeName()}";
    }

    private IEdmType ResponseType
    {
        get => Segments.IsEmpty ? EdmUntypedStructuredType.Instance : Segments.Last().Type;
    }

    public Path Append(Path tail)
    {
        return new Path(Segments.AddRange(tail.Segments));
    }

    public Path(string name, IEdmType type) : this(ImmutableList.Create((name, type)))
    { }
}
