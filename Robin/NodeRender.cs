using Robin.Abstractions;
using Robin.Contracts.Nodes;
using System.Collections;
using System.Collections.Immutable;
using System.Net;

namespace Robin;



public class NodeRender : INodeVisitor<NoValue, RenderContext>
{
    public readonly static NodeRender Instance = new();

    public NoValue VisitText(TextNode node, RenderContext context)
    {
        context.Builder.Append(node.Text);
        return NoValue.Instance;
    }
    public NoValue VisitComment(CommentNode node, RenderContext context)
    {
        return NoValue.Instance;
    }

    public NoValue VisitPartial(PartialDefineNode node, RenderContext context)
    {
        return NoValue.Instance;
    }

    public NoValue VisitVariable(VariableNode node, RenderContext context)
    {
        if (context.Evaluator.TryResolve(node.Expression, context.Data, out object? value))
        {
            if (node.IsUnescaped)
                context.Builder.Append(value?.ToString());
            else
                context.Builder.Append(WebUtility.HtmlEncode(value?.ToString()));
            return NoValue.Instance;
        }
        return NoValue.Instance;
    }

    public NoValue VisitSection(SectionNode node, RenderContext context)
    {
        object? subData = context.Evaluator.TryResolve(node.Expression, context.Data, out object? value) ? value : null;
        bool thruly = context.Evaluator.IsTrue(subData);

        if ((!node.Inverted && thruly) || (node.Inverted && !thruly))
        {
            return RenderTree(context, subData, node.Children);
        }
        return NoValue.Instance;
    }

    public NoValue VisitPartialCall(PartialCallNode node, RenderContext context)
    {
        object? subData = context.Evaluator.TryResolve(node.Expression, context.Data, out object? value) ? value : null;
        bool thruly = context.Evaluator.IsTrue(subData);
        if (context.Partials.TryGetValue(node.PartialName, out ImmutableArray<INode> partialTemplate) && thruly)
        {
            return RenderTree(context, subData, partialTemplate);

        }
        return NoValue.Instance;
    }

    private NoValue RenderTree(RenderContext context, object? subData, ImmutableArray<INode> partialTemplate)
    {
        if (context.Evaluator.IsCollection(subData, out IEnumerable? collection))
        {
            foreach (object? item in collection)
            {
                RenderContext itemCtx = context with
                {
                    Data = context.Data?.Child(item) ?? new DataContext(item, null),
                };
                ImmutableArray<INode>.Enumerator enumerator = partialTemplate.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    _ = enumerator.Current.Accept(this, itemCtx);
                }
            }
        }
        else
        {
            RenderContext innerCtx = context with
            {
                Data = context.Data?.Child(subData) ?? new DataContext(subData, null),
            };
            ImmutableArray<INode>.Enumerator enumerator = partialTemplate.GetEnumerator();
            while (enumerator.MoveNext())
            {
                _ = enumerator.Current.Accept(this, innerCtx);
            }
        }

        return NoValue.Instance;
    }

    public NoValue VisitLineBreak(LineBreakNode node, RenderContext context)
    {
        for (int i = 0; i < node.Count; i++)
            context.Builder.AppendLine();
        return NoValue.Instance;
    }
}