using RobinMustache.Abstractions;
using RobinMustache.Abstractions.Context;
using RobinMustache.Abstractions.Facades;
using RobinMustache.Abstractions.Iterators;
using RobinMustache.Abstractions.Nodes;
using RobinMustache.Extensions;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.Text;

namespace RobinMustache.Internals;

internal sealed class StringNodeRender(IEnumerable<IPartialLoader> loaders) : INodeVisitor<RenderContext<StringBuilder>>
{

    public void VisitText(TextNode node, RenderContext<StringBuilder> context)
    {
        context.Builder.Append(node.Text);
    }
    public void VisitWhitespace(WhitespaceNode node, RenderContext<StringBuilder> context)
    {
        context.Builder.Append(node.Text);
    }
    public void VisitComment(CommentNode node, RenderContext<StringBuilder> context)
    {
    }

    public void VisitPartialDefine(PartialDefineNode node, RenderContext<StringBuilder> context)
    {
    }

    public void VisitVariable(VariableNode node, RenderContext<StringBuilder> context)
    {
        object? value = context.Evaluator.Resolve(node.Expression, DataContext.Current, out IDataFacade facade);
        if (value is not null && facade.IsTrue(value))
        {
            string? str;
            if (value is string s)
                str = s;
            else
                str = value.ToString();
            if (str is not null)
            {
                if (node.IsUnescaped)
                    context.Builder.Append(str);
                else
                    context.Builder.Append(str.Escape());
            }

        }
    }

    public void VisitSection(SectionNode node, RenderContext<StringBuilder> context)
    {
        object? value = context.Evaluator.Resolve(node.Expression, DataContext.Current, out IDataFacade facade);
        bool thruly = facade.IsTrue(value);
        bool shouldRenderTree = (!node.Inverted && thruly) || (node.Inverted && !thruly);
        if (shouldRenderTree && facade.IsCollection(value, out IIterator? iterator) && iterator is not null)
        {
            iterator.Iterate(value, context, node.Children.AsSpan(), node.TrailingBreak, this);
        }
        else if (shouldRenderTree)
        {
            using (DataContext.Push(value))
            {
                foreach (INode child in node.Children.AsSpan())
                {
                    child.Accept(this, context);
                }
            }
            if (node.TrailingBreak is not null && ((node.Inverted && shouldRenderTree) || (!node.Inverted && shouldRenderTree)))
                VisitLineBreak(node.TrailingBreak, context);
        }
    }

    public void VisitPartialCall(PartialCallNode node, RenderContext<StringBuilder> context)
    {
        object? value = context.Evaluator.Resolve(node.Expression, DataContext.Current, out IDataFacade facade);

        if (facade.IsTrue(value))
        {
            foreach (IPartialLoader loader in loaders)
            {
                if (loader.Load(node.PartialName, context, out ImmutableArray<INode> partialTemplate))
                {
                    ReadOnlySpan<INode> span = partialTemplate.AsSpan();

                    ReadOnlyDictionary<string, ImmutableArray<INode>> tempPartials = new(span.ExtractsPartials(context.Partials));
                    using (new PartialsScope<StringBuilder>(context, tempPartials))
                    {
                        using (DataContext.Push(value))
                        {
                            foreach (INode child in span)
                            {
                                child.Accept(this, context);
                            }
                        }
                    }
                    return;
                }
            }
        }
    }

    public void VisitLineBreak(LineBreakNode node, RenderContext<StringBuilder> context)
    {
        context.Builder.Append(node.Content);
    }
}


