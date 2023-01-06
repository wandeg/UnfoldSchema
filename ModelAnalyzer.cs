internal class ModelAnalyzer
{
    public ModelAnalyzer()
    {
    }

    public IEnumerable<Path> Unfold(IEdmModel model)
    {
        return Unfold(ImmutableHashSet<IEdmType>.Empty, Path.Empty, model.EntityContainer);
    }

    private IEnumerable<Path> Unfold(ImmutableHashSet<IEdmType> visited, Path prefix, IEdmEntityContainer entityContainer)
    {
        foreach (var element in entityContainer.Elements)
        {
            switch (element)
            {
                case IEdmEntitySet entitySet:
                    foreach (var path in Unfold(visited, entitySet))
                    {
                        yield return path;
                    }
                    break;
                case IEdmSingleton singleton:
                    foreach (var path in Unfold(visited, singleton))
                    {
                        yield return path;
                    }
                    break;
                default:
                    break;
            }
        }
    }

    private IEnumerable<Path> Unfold(ImmutableHashSet<IEdmType> visited, IEdmSingleton singleton)
    {
        var prefix = new Path(singleton.Name, singleton.Type);
        yield return prefix;

        if (singleton.Type is IEdmEntityType singletonType)
        {
            foreach (var path in Unfold(visited, singletonType))
            {
                yield return prefix.Append(path);
            }
        }
        else
        {
            throw new NotSupportedException("singleton type not a entity type");
        }
    }

    private IEnumerable<Path> Unfold(ImmutableHashSet<IEdmType> visited, IEdmEntitySet entitySet)
    {
        if (entitySet.Type is IEdmCollectionType collectionType && collectionType.ElementType.Definition is IEdmEntityType elementType)
        {
            var prefix = new Path(entitySet.Name, collectionType);
            yield return prefix;

            foreach (var path in Unfold(visited, collectionType))
            {
                yield return prefix.Append(path);
            }
        }
        else
        {
            throw new NotSupportedException("EntitySet type not a collection of entity types");
        }
    }


    private IEnumerable<Path> Unfold(ImmutableHashSet<IEdmType> visited, IEdmStructuredType structuredType)
    {
        // if we visited the type, return one last path and stop recursion
        if (visited.Contains(structuredType))
        {
            yield return new Path("...", structuredType);
            yield break;
        }
        visited = visited.Add(structuredType);

        foreach (var property in structuredType.NavigationProperties())
        {
            var navPropPath = new Path(property.Name, property.Type.Definition);
            yield return navPropPath;
            switch (property.Type.Definition)
            {
                case IEdmStructuredType propertyStructuredType:
                    foreach (var path in Unfold(visited, propertyStructuredType))
                    {
                        yield return navPropPath.Append(path);
                    }

                    break;

                case IEdmCollectionType collectionType:
                    foreach (var path in Unfold(visited, collectionType))
                    {
                        yield return navPropPath.Append(path);
                    }

                    break;
            }
        }
    }

    private IEnumerable<Path> Unfold(ImmutableHashSet<IEdmType> visited, IEdmCollectionType collectionType)
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
            var key = keys.Single();
            var keyPath = new Path($"{{{key.Name}}}", elementType);
            yield return keyPath;

            foreach (var path in Unfold(visited, elementType))
            {
                yield return keyPath.Append(path);
            }
        }
        else
        {
            throw new NotSupportedException("IEdmCollectionType's element is not a entity type");
        }
    }
}

