using var reader = XmlReader.Create("example89.csdl.xml");
if (!CsdlReader.TryParse(reader, out var model, out var errors))
{
    Console.WriteLine(string.Join(Environment.NewLine, errors));
    return;
}

var analyzer = new ModelAnalyzer();

Console.WriteLine("analyzing... ");
Console.WriteLine();

var tree = analyzer.Create(model);
tree.WriteTo(Console.Out);


// foreach (var path in tree.Paths())
// {
//     Console.WriteLine("{0} -> \x1b[34m{1}\x1b[m", path.Segments.SeparatedBy("/"), path.ResponseType.Format());
// }
