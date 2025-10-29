using System.Text;

namespace Robin.Nodes;

// Reuse previous TokenType and Token

// AST nodes
public interface INode
{
    public void Render(Context context, StringBuilder output);
}

