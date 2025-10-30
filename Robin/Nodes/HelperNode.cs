using System.Collections.Immutable;
using System.Text;

namespace Robin.Nodes;

public readonly struct HelperNode(string name, INode argument) : INode
{
    public string Name { get; } = name;
    public INode Argument { get; } = argument;
}

