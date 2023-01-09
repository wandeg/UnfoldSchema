using Microsoft.OData.Edm;

class Program
{

    const string DEFAULT_INPUT = "directory.csdl.xml";
    // const string DEFAULT_INPUT = "example89.csdl.xml";

    private static void Main(string[] args)
    {
        // System.Console.Error.WriteLine(string.Join(" ", args));
        var file = args.Length > 0 ? args[0] : DEFAULT_INPUT;

        using var reader = XmlReader.Create(file);
        if (!CsdlReader.TryParse(reader, out var model, out var errors))
        {
            Console.WriteLine(string.Join(Environment.NewLine, errors));
            return;
        }

        Console.WriteLine("analyzing... ");
        Console.WriteLine();
        var analyzer = new ModelAnalyzer(model);
        var tree = analyzer.CreateTree();
        using var writer = new TreeWriter(Console.Out, true);

        writer.Display(tree);

        // foreach (var path in tree.Paths())
        // {
        //     Console.WriteLine("{0}", path.Segments.SeparatedBy("/"));

        //     // Console.WriteLine("{0}\n\t\x1b[36m{1}\x1b[m",
        //     //     path.Segments.SeparatedBy("/"),
        //     //     Signature(path));

        //     // Console.WriteLine("{0} \x1b[36m{1}\x1b[m",
        //     // path.Segments.SeparatedBy("/"),
        //     // path.ResponseType.Format());

        // }
    }



    static string Signature((IEnumerable<string> Segments, IEdmType ResponseType) path)
    {
        var parameters = string.Join(", ", path.Segments.Where(s => s.StartsWith('{')).Select(w => w.Trim('{', '}')));
        var name = string.Join("Of", path.Segments.Where(s => !s.StartsWith('{')).Select(s => s.Capitalize()).Reverse());

        return $"{name}({parameters}) -> {path.ResponseType.Format()}";
    }
}
