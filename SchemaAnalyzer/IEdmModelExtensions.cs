
static class IEdmModelExtensions
{

    public static bool TryFindDeclaredType(this IEdmModel model, string fqn, [MaybeNullWhen(false)] out IEdmType type)
    {
        type = model.FindDeclaredType(fqn);
        return type != null;
    }

    public static bool TryFindDeclaredEntityType(this IEdmModel model, string fqn, [MaybeNullWhen(false)] out IEdmEntityType entityType)
    {
        if (model.TryFindDeclaredType(fqn, out var type) && type is IEdmEntityType entityType1)
        {
            entityType = entityType1;
            return true;
        }
        entityType = default;
        return false;
    }


    public static bool TryFindAllDerivedTypes(this IEdmModel model, IEdmEntityType baseType, out IEnumerable<IEdmEntityType> subTypes)
    {
        subTypes = model.FindAllDerivedTypes(baseType).Cast<IEdmEntityType>();
        return true;
    }



    public static bool TryFindAllDerivedTypes(this IEdmModel model, IEdmComplexType baseType, out IEnumerable<IEdmComplexType> subTypes)
    {
        subTypes = model.FindAllDerivedTypes(baseType).Cast<IEdmComplexType>();
        return true;
    }

    public static bool TryFindDeclaredEntitySubTypes(this IEdmModel model, string fqn, [MaybeNullWhen(false)] out IEnumerable<IEdmEntityType> subtypes)
    {
        if (model.TryFindDeclaredEntityType(fqn, out var baseType))
        {
            subtypes = model.FindAllDerivedTypes(baseType).Cast<IEdmEntityType>();
            return true;
        }

        subtypes = default;
        return false;
    }
}