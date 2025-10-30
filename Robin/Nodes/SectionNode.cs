using System.Collections.Immutable;
using System.Text;

namespace Robin.Nodes;

public readonly struct SectionNode(string name, ImmutableArray<INode> children, bool inverted = false) : INode
{
    public string Name { get; } = name;
    public ImmutableArray<INode> Children { get; } = children;
    public bool Inverted { get; } = inverted;
}

