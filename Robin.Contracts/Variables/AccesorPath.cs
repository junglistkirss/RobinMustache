using Robin.Contracts.Context;
using System.Collections.Immutable;
using System.Text;

namespace Robin.Contracts.Variables;

public readonly struct AccesorPath(ImmutableArray<IAccessor> segments)
{
    public static implicit operator string(AccesorPath value) => value.ToString();
    public ImmutableArray<IAccessor> Segments { get; } = segments;
    public override string ToString()
    {
        return Segments.Aggregate(
            new StringBuilder(),
            (sb, segment) =>
            {
                return segment.Accept(InlineAccessorPrinter.Instance, sb);
            },
            sb => sb.ToString()
        );
    }

    public EvaluationResult Accept(IAccessorVisitor<EvaluationResult, DataContext> visitor, DataContext args, bool usePreviousFallback = true)
    {
        EvaluationResult result = new(true, null);
        int i = 0;
        DataContext ctx = args;
        while (result.Found && i < Segments.Length)
        {
            IAccessor item = Segments[i];
            EvaluationResult res = item.Accept(visitor, ctx);
            if (res.Found)
            {
                result = res;
                ctx = ctx.Child(res.Value);
            }
            else
            {
                result = result with { Found = false };
            }
            i++;
        }
        if(usePreviousFallback && !result.Found && args.Previsous is not null)
        {
            return Accept(visitor, args.Previsous, usePreviousFallback);
        }
        return result;
    }

}
