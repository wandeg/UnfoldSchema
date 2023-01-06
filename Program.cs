
using var reader = XmlReader.Create("example89.csdl.xml");
if (CsdlReader.TryParse(reader, out var model, out var errors))
{

    var analyzer = new ModelAnalyzer();

    Console.WriteLine("analyzing... ");
    foreach (var path in analyzer.Unfold(model))
    {
        Console.WriteLine(string.Join("/", path));
    }
}
else
{
    Console.WriteLine(string.Join(Environment.NewLine, errors));
}

