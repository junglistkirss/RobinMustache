using Robin.Abstractions.Accessors;
using Robin.Abstractions.Context;
using Robin.Abstractions.Facades;
using Robin.Contracts.Nodes;
using Robin.Extensions;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.Text;

namespace Robin.Internals;

internal sealed class StringNodeRender : INodeVisitor<RenderContext<StringBuilder>>
{
    public readonly static StringNodeRender Instance = new();

    public void VisitText(TextNode node, RenderContext<StringBuilder> context)
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
        if (facade.IsTrue(value))
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

        if (!node.Inverted && thruly || node.Inverted && !thruly)
        {
            RenderTree(context, value, facade, node.Children);
        }

    }

    public void VisitPartialCall(PartialCallNode node, RenderContext<StringBuilder> context)
    {
        object? value = context.Evaluator.Resolve(node.Expression, DataContext.Current, out IDataFacade facade);

        if (facade.IsTrue(value) && context.Partials is not null && context.Partials.TryGetValue(node.PartialName, out ImmutableArray<INode> partialTemplate))
        {
            ReadOnlyDictionary<string, ImmutableArray<INode>> tempPartials = partialTemplate.ExtractsPartials(context.Partials).AsReadOnly();

            using (new PartialsScope<StringBuilder>(context, tempPartials))
            {
                // Ici context.Partials == tempPartials
                // tu peux faire ton rendu spécifique
                RenderTree(context, value, facade, partialTemplate);
            }

        }
    }

    private void RenderTree(RenderContext<StringBuilder> context, object? value, IDataFacade facade, ImmutableArray<INode> partialTemplate)
    {
        void action(object? o)
        {
            using (DataContext.Push(o))
            {

                foreach (var node in partialTemplate)
                {
                    node.Accept(this, context);
                }
            }
        }

        if (facade.IsCollection(value, out IIterator? collection))
            collection.Iterate(action);
        else
            action(value);

    }

    public void VisitLineBreak(LineBreakNode node, RenderContext<StringBuilder> context)
    {
        for (int i = 0; i < node.Count; i++)
            context.Builder.AppendLine();
    }
}