


public static class EdmExtensions
{
    public static string Format(this IEdmType type) => type switch
    {
        IEdmCollectionType collectionType => $"[{Format(collectionType.ElementType.Definition)}]",
        IEdmEntityType entityType => entityType.Name,
        IEdmComplexType complexType => complexType.Name,
        _ => type?.ToString() ?? "<null>",
    };
}