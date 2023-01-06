internal class ModelAnalyzer
{
    public ModelAnalyzer()
    {
    }

    public Node Create(IEdmModel model)
    {
        return Unfold(ImmutableHashSet<IEdmType>.Empty, model.EntityContainer);
    }

    private Node Unfold(ImmutableHashSet<IEdmType> visited, IEdmEntityContainer entityContainer)
    {
        var node = new Node("", EdmUntypedStructuredType.Instance);
        foreach (var element in entityContainer.Elements)
        {
            switch (element)
            {
                case IEdmEntitySet entitySet:
                    var child = Unfold(visited, entitySet);
                    node.Add(child);
                    break;
                case IEdmSingleton singleton:
                    child = Unfold(visited, singleton);
                    node.Add(child);
                    break;
                default:
                    break;
            }
        }
        return node;
    }

    private Node Unfold(ImmutableHashSet<IEdmType> visited, IEdmSingleton singleton)
    {
        var node = new Node(singleton.Name, singleton.Type);

        if (singleton.Type is IEdmEntityType singletonType)
        {
            foreach (var n in Unfold(visited, singletonType))
            {
                node.Add(n);
            }
        }
        else
        {
            throw new NotSupportedException("singleton type not a entity type");
        }

        return node;
    }

    private Node Unfold(ImmutableHashSet<IEdmType> visited, IEdmEntitySet entitySet)
    {
        var node = new Node(entitySet.Name, entitySet.Type);

        if (entitySet.Type is IEdmCollectionType collectionType && collectionType.ElementType.Definition is IEdmEntityType elementType)
        {
            // var prefix = new Path(entitySet.Name, collectionType);
            // yield return prefix;

            foreach (var n in Unfold(visited, collectionType))
            {
                node.Add(n);
            }
        }
        else
        {
            throw new NotSupportedException("EntitySet type not a collection of entity types");
        }

        return node;
    }


    private IEnumerable<Node> Unfold(ImmutableHashSet<IEdmType> visited, IEdmStructuredType structuredType)
    {
        // if we visited the type, return one last path and stop recursion
        if (visited.Contains(structuredType))
        {
            // yield return new Path("...", structuredType);
            yield break;
        }
        visited = visited.Add(structuredType);

        foreach (var property in structuredType.NavigationProperties())
        {
            var node = new Node(property.Name, property.Type.Definition);
            switch (property.Type.Definition)
            {
                case IEdmStructuredType propertyStructuredType:
                    node.AddRange(Unfold(visited, propertyStructuredType));
                    break;

                case IEdmCollectionType collectionType:
                    node.AddRange(Unfold(visited, collectionType));
                    break;
            }
            yield return node;
        }
    }

    private IEnumerable<Node> Unfold(ImmutableHashSet<IEdmType> visited, IEdmCollectionType collectionType)
    {
        // if we visited the type, return one last path and stop recursion
        // // there is a bug that two structurally equal collection types can have different hash codes.
        // // https://github.com/OData/odata.net/issues/2589
        // if (visited.Contains(collectionType) || visited.Contains(collectionType.ElementType.Definition))
        // {
        //     yield return new Path("...!", collectionType);
        //     yield break;
        // }
        // // System.Console.WriteLine("inspect: {0} {1}", collectionType.FullTypeName(), string.Join(", ", visited.Select(v => v.FullTypeName())));
        // // System.Console.WriteLine("inspect: {0} {1}", collectionType.GetHashCode(), string.Join(", ", visited.Select(v => v.GetHashCode())));

        if (collectionType.ElementType.Definition is IEdmEntityType elementType)
        {
            var keys = elementType.Key();
            var key = keys.Single(); // TODO: custom exception for multipart key

            var node = new Node($"{{{key.Name}}}", elementType);
            node.AddRange(Unfold(visited, elementType));
            yield return node;
        }
        else
        {
            throw new NotSupportedException("IEdmCollectionType's element is not a entity type");
        }
    }
}

