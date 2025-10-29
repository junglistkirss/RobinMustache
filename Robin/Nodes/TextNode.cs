using System.Text;

namespace Robin.Nodes;

public readonly struct TextNode(ReadOnlyMemory<char> text) : INode
{
    public ReadOnlyMemory<char> Text { get; } = text;

    public void Render(Context context, StringBuilder output) => output.Append(Text);
}

