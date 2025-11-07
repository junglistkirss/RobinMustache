using Robin.Abstractions;
using Robin.Abstractions.Context;
using Robin.Abstractions.Extensions;
using Robin.Abstractions.Helpers;
using Robin.Contracts.Nodes;
using Robin.Internals;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.Text;

namespace Robin;

public static class Renderer
{
    public static T Render<T>(this T defaultBuilder, INodeVisitor<NoValue, RenderContext<T>> visitor, IEvaluator evaluator, ImmutableArray<INode> template, object? data, Action<Helper>? helperConfig = null)
        where T : class
    {
        ReadOnlyDictionary<string, ImmutableArray<INode>> partials = template.ExtractsPartials().AsReadOnly(); // calculer une seule fois
        using (DataContext.Push(data))
        {
            helperConfig?.Invoke(DataContext.Current.Helper);
            RenderContext<T> ctx = RenderContextPool<T>.Get(evaluator, defaultBuilder, partials);
            try
            {

                foreach (var item in template)
                {

                    // helperConfig?.Invoke(DataContext.Current.Helper);
                    // RenderContext<T> ctx = new()
                    // {
                    //     Partials = template.ExtractsPartials(),
                    //     Evaluator = evaluator,
                    //     Builder = defaultBuilder
                    // };
                    item.Accept(visitor, ctx);
                }
            }
            finally
            {
                RenderContextPool<T>.Return(ctx); // remet dans le pool
            }
            return defaultBuilder;

        }
    }

    public static string RenderString(this IEvaluator evaluator, ImmutableArray<INode> template, object? data, Action<Helper>? helperConfig = null)
    {
        StringBuilder sb = new();
        Render(sb, StringNodeRender.Instance, evaluator, template, data, helperConfig);
        return sb.ToString();
    }
}
