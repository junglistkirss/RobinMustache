using Robin.Abstractions;
using Robin.Contracts.Nodes;
using System.Collections.Immutable;
using System.Text;

namespace Robin;

public static class Renderer
{
    public static string Render(this INodeVisitor<RenderResult, RenderContext> visitor, IEvaluator evaluator, ImmutableArray<INode> template, object? data)
    {
        RenderContext ctx = new()
        {
            Data = new DataContext(data, null),
            Evaluator = evaluator,
            Builder = new StringBuilder()
        };
        ImmutableArray<INode>.Enumerator enumerator = template.GetEnumerator();
        while (enumerator.MoveNext())
        {
            RenderResult result = enumerator.Current.Accept(visitor, ctx);
            if (!result.IsComplete && result.Exception is not null)
            {
                throw result.Exception;
            }
        }
        return ctx.Builder.ToString();
    }
}
