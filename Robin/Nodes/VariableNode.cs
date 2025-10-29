using System.Text;

namespace Robin.Nodes;

public readonly struct VariableNode(string name, bool unescaped) : INode
{
    public string Name { get; } = name;
    public bool IsUnescaped { get; } = unescaped;

    public void Render(Context context, StringBuilder output)
    {
        if (context.TryResolve(Name.ToString(), out var value))
        {
            var str = value?.ToString() ?? string.Empty;
            output.Append(IsUnescaped ? str : System.Net.WebUtility.HtmlEncode(str));
        }
    }
}

