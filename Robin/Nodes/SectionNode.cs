using System.Collections.Immutable;
using System.Text;

namespace Robin.Nodes;

public readonly struct SectionNode(ReadOnlyMemory<char> name, ImmutableArray<INode> children, bool inverted = false) : INode
{
    public ReadOnlyMemory<char> Name { get; } = name;
    public ImmutableArray<INode> Children { get; } = children;
    public bool Inverted { get; } = inverted;

    public void Render(Context context, StringBuilder output)
    {
        if (context.TryResolve(Name.ToString(), out var value))
        {
            bool isTruthy = value != null && (!Inverted);
            if (Inverted) isTruthy = value == null || value.Equals(false);
            if (isTruthy)
                foreach (INode child in Children) child.Render(context, output);
        }
        else if (Inverted)
        {
            foreach (INode child in Children) child.Render(context, output);
        }
    }
}

