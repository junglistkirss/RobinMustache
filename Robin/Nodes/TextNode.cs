using System.Text;

namespace Robin.Nodes;

public readonly struct TextNode(string text) : INode
{
    public string Text { get; } = text;

    public void Render(Context context, StringBuilder output) => output.Append(Text);
}

