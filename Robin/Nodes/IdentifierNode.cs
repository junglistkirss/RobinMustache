using System.Collections.Immutable;
using System.Text;

namespace Robin.Nodes;

public interface IPathSegment;
public readonly struct ChainPath(ImmutableArray<IPathSegment> segments)
{
    public static implicit operator ChainPath (string value) => Parse(value);
    public static implicit operator string (ChainPath value) => value.ToString();
    public ImmutableArray<IPathSegment> Segments { get; } = segments;
    public override string ToString()
    {
        return Segments.Aggregate(
            new StringBuilder(),
            (sb, segment) =>
            {
                switch (segment)
                {
                    case MemberAccesor member:
                        if (sb.Length > 0)
                            sb.Append('.');
                        sb.Append(member.MemberName);
                        break;
                    case IndexAccessor index:
                        sb.Append('[').Append(index.Index).Append(']');
                        break;
                }
                return sb;
            },
            sb => sb.ToString()
        );
    }

    public static ChainPath Parse(string path)
    {
        if (string.IsNullOrEmpty(path))
            return new ChainPath(ImmutableArray<IPathSegment>.Empty);
        
        var segments = new List<IPathSegment>();
        int i = 0;
        
        while (i < path.Length)
        {
            // Skip dots
            if (path[i] == '.')
            {
                i++;
                continue;
            }
            
            // Parse index accessor
            if (path[i] == '[')
            {
                i++; // skip '['
                int start = i;
                while (i < path.Length && path[i] != ']')
                    i++;
                
                if (i >= path.Length)
                    throw new FormatException("Unclosed index accessor");
                
                string indexStr = path.Substring(start, i - start);
                if (!int.TryParse(indexStr, out int index))
                    throw new FormatException($"Invalid index: {indexStr}");
                
                segments.Add(new IndexAccessor(index));
                i++; // skip ']'
            }
            // Parse member accessor
            else
            {
                int start = i;
                while (i < path.Length && path[i] != '.' && path[i] != '[')
                    i++;
                
                string memberName = path.Substring(start, i - start);
                if (string.IsNullOrEmpty(memberName))
                    throw new FormatException("Empty member name");
                
                segments.Add(new MemberAccesor(memberName));
            }
        }
        
        return new ChainPath(segments.ToImmutableArray());
    }
}

public readonly struct MemberAccesor(string memberName) : IPathSegment
{
    public string MemberName { get; } = memberName;
}
public readonly struct IndexAccessor(int index) : IPathSegment
{
    public int Index { get; } = index;
}
public readonly struct IdentifierNode(ChainPath path) : IExpressionNode
{
    public ChainPath Path { get; } = path;
};


//// Example usage and testing
//public class Program
//{
//    public static void Main()
//    {
//        TestExpression("3 + 4 * 2");
//        TestExpression("(3 + 4) * 2");
//        TestExpression("foo(1, 2, 3)");
//        TestExpression("max(a, b) + min(c, d)");
//        TestExpression("-5 + 3");
//        TestExpression("a > 5 && b < 10");
//        TestExpression("x == 5 || y == 10");
//        TestExpression("2 ^ 3 ^ 2");
//        TestExpression("price * 1.2");
//        TestExpression("obj.property[0]");
//    }

//    static void TestExpression(string expr)
//    {
//        Console.WriteLine($"\nParsing: {expr}");
//        try
//        {
//            var parser = new ExpressionParser(expr.AsSpan());
//            var result = parser.Parse();
//            Console.WriteLine($"Result: {result}");
//        }
//        catch (Exception ex)
//        {
//            Console.WriteLine($"Error: {ex.Message}");
//        }
//    }
//}

