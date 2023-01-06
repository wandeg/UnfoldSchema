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

        if (!(singleton.Type is IEdmEntityType singletonType))
        {
            throw new NotSupportedException("singleton type not a entity type");
        }

        foreach (var n in Unfold(visited, singletonType))
        {
            node.Add(n);
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

        // Get all properties, not just navigation properties 
        // This will generate navigation paths that navigate through a structural property
        // (e.g.  /Suppliers/{ID}/Address/Country: in example 89)
        // If the property type is neither complex nor entity, nothing will be (yield) returned in the switch statement
        foreach (var property in structuredType.Properties())
        {
            var node = new Node(property.Name, property.Type.Definition);
            switch (property.Type.Definition)
            {
                case IEdmStructuredType propertyStructuredType:
                    node.AddRange(Unfold(visited, propertyStructuredType));
                    yield return node;
                    break;

                case IEdmCollectionType collectionType:
                    node.AddRange(Unfold(visited, collectionType));
                    yield return node;
                    break;
            }
        }
    }

    private IEnumerable<Node> Unfold(ImmutableHashSet<IEdmType> visited, IEdmCollectionType collectionType)
    {

        if (!(collectionType.ElementType.Definition is IEdmEntityType elementType))
        {
            throw new NotSupportedException("IEdmCollectionType's element is not a entity type");
        }

        var keys = elementType.Key();
        var key = keys.Single(); // TODO: custom exception for multipart key

        var node = new Node($"{{{key.Name}}}", elementType);
        node.AddRange(Unfold(visited, elementType));
        yield return node;
    }
}

