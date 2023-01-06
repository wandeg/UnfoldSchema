

public record class Node(string Name, IEdmType Type)
{

    private readonly List<Node> Nodes = new List<Node>();

    public IEnumerable<(IEnumerable<string> Segments, IEdmType ResponseType)> Unfold()
    {
        return this.Unfold(ImmutableList<string>.Empty);
    }

    public void Add(Node node)
    {
        Nodes.Add(node);
    }

    public void AddRange(IEnumerable<Node> node)
    {
        Nodes.AddRange(node);
    }

    public IEnumerable<(IEnumerable<string>, IEdmType)> Unfold(ImmutableList<string> path)
    {
        path = path.Add(Name);
        yield return (path, Type);
        foreach (var node in Nodes)
        {
            foreach (var child in node.Unfold(path))
            {
                yield return child;
            }
        }
    }
}

// public record class Path(ImmutableList<(string Name, IEdmType Type)> Segments)
// {
//     public static Path Empty = new Path(ImmutableList<(string Name, IEdmType Type)>.Empty);

//     public override string ToString()
//     {
//         string Kind(IEdmType type) =>
//          (type is IEdmCollectionType coll) ? $"Collection<{Kind(coll.ElementType.Definition)}>" : type.TypeKind.ToString();

//         // return $"/{string.Join("/", Segments.Select(s => s.Name))}: {ResponseType.FullTypeName()}{{{Kind(ResponseType)}}}";
//         return $"/{string.Join("/", Segments.Select(s => s.Name))}: {ResponseType.FullTypeName()}";
//     }

//     private IEdmType ResponseType
//     {
//         get => Segments.IsEmpty ? EdmUntypedStructuredType.Instance : Segments.Last().Type;
//     }

//     public Path Append(Path tail)
//     {
//         return new Path(Segments.AddRange(tail.Segments));
//     }


//     public Path(string name, IEdmType type) : this(ImmutableList.Create((name, type)))
//     { }
// }
