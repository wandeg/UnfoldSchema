using Microsoft.OData.Edm;

class Program
{
    private static void Main(string[] args)
    {
        // using var reader = XmlReader.Create("example89.csdl.xml");
        using var reader = XmlReader.Create("directory.csdl.xml");
        if (!CsdlReader.TryParse(reader, out var model, out var errors))
        {
            Console.WriteLine(string.Join(Environment.NewLine, errors));
            return;
        }

        var analyzer = new ModelAnalyzer();

        Console.WriteLine("analyzing... ");
        Console.WriteLine();

        var tree = analyzer.Create(model);
        Console.Out.Display(tree);

        foreach (var path in tree.Paths())
        {
            Console.WriteLine("{0}\n\t\x1b[34m{1}\x1b[m",
                path.Segments.SeparatedBy("/"),
                Signature(path));
        }

    }

    static string Signature((IEnumerable<string> Segments, IEdmType ResponseType) path)
    {
        var parameters = string.Join(", ", path.Segments.Where(s => s.StartsWith('{')).Select(w => w.Trim('{', '}')));
        var name = string.Join("Of", path.Segments.Where(s => !s.StartsWith('{')).Select(s => s.Capitalize()).Reverse());

        return $"{name}({parameters}) -> {path.ResponseType.Format()}";
    }

}