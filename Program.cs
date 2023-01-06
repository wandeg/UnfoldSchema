

using var reader = XmlReader.Create("example89.csdl.xml");
if (!CsdlReader.TryParse(reader, out var model, out var errors))
{
    Console.WriteLine(string.Join(Environment.NewLine, errors));
    return;
}

var analyzer = new ModelAnalyzer();

Console.WriteLine("analyzing... ");
var tree = analyzer.Create(model);


foreach (var path in tree.Paths())
{
    Console.WriteLine("{0} -> \x1b[34m{1}\x1b[m", path.Segments.SeparatedBy("/"), Format(path.ResponseType));
}

string Format(IEdmType type) => type switch
{
    IEdmCollectionType collectionType => $"[{Format(collectionType.ElementType.Definition)}]",
    IEdmEntityType entityType => entityType.Name,
    IEdmComplexType complexType => complexType.Name,
    _ => type?.ToString() ?? "<null>",
};
