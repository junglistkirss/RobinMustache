using System.Text;

namespace Robin.Nodes;

public readonly struct HelperNode(ReadOnlyMemory<char> name) : INode
{
    public ReadOnlyMemory<char> Name { get; } = name;
    public List<INode> Arguments { get; } = [];

    public void Render(Context context, StringBuilder output)
    {
        if (context.Helpers.TryGetValue(Name.ToString(), out Action<Context, INode[], StringBuilder>? helper))
        {
            helper(context, [.. Arguments], output);
        }
    }
}

