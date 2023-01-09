

public class TreeWriter : IDisposable
{
    public TreeWriter(TextWriter writer, bool v) { this.writer = writer; }

    public void Display(Node node)
    {
        foreach (var (child, isLast) in node.Nodes.WithLast())
        {
            WriteNode(child, indent: "", isLast: isLast);
        }
        writer.WriteLine();
    }

    // adapted from https://andrewlock.net/creating-an-ascii-art-tree-in-csharp/
    private void WriteNode(Node node, string indent, bool isLast)
    {
        // Print the provided pipes/spaces indent
        Console.Write(indent);

        // Depending if this node is a last child, print the corner or cross, and 
        // calculate the indent that will be passed to its children
        if (isLast)
        {
            writer.Write(CONFIG.LastChild);
            indent += CONFIG.Space;
        }
        else
        {
            writer.Write(CONFIG.Child);
            indent += CONFIG.Vertical;
        }
        writer.WriteLine("{0} \x1b[34m{1}\x1b[m", node.Name, node.Type.Format());

        // Loop through the children recursively, passing in the
        // indent, and the isLast parameter
        foreach (var (child, isLastChild) in node.Nodes.WithLast())
        {
            WriteNode(child, indent, isLastChild);
        }
    }

    public void Dispose()
    {
        ((IDisposable)writer).Dispose();
    }

    record struct Config(
        string Child, string LastChild, string Vertical, string Space)
    { }

    // Constants for indentation
    // https://unicode-table.com/en/blocks/box-drawing/
    static Config[] CONFIGS = new[]{
        new Config(" +-"," +-"," | ", "   "),
        new Config(" ├─"," └─"," │ ", "   "),
        new Config(" ╠═"," ╚═"," ║ ", "   "),
        new Config(" ╟─"," ╙─"," ║ ", "   "),
    };

    static Config CONFIG = CONFIGS[0];
    private readonly TextWriter writer;
}