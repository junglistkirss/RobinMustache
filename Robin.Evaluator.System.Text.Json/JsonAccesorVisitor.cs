using Robin.Abstractions.Accessors;
using Robin.Contracts.Variables;
using System.Text.Json.Nodes;

namespace Robin.Evaluator.System.Text.Json;

internal sealed class JsonAccesorVisitor : BaseAccessorVisitor
{
    public readonly static JsonAccesorVisitor Instance = new();
    public override bool VisitIndex(IndexSegment segment, Type args, out ChainableGetter getter)
    {
        if (Nullable.GetUnderlyingType(args) == typeof(JsonArray))
        {
            int index = segment.Index;
            getter = new ChainableGetter((object? input, out object? value) =>
            {
                return input.TryGetIndexValue(index, out value);
            });
            return true;
        }
        getter = ChainableGetters.ReturnNull;
        return false;
    }

    public override bool VisitMember(MemberSegment segment, Type args, out ChainableGetter getter)
    {
        if (Nullable.GetUnderlyingType(args) == typeof(JsonObject))
        {
            string memberName = segment.MemberName;
            getter = new ChainableGetter((object? input, out object? value) =>
            {
                return input.TryGetMemberValue(memberName, out value);
            });
            return true;
        }
        getter = ChainableGetters.ReturnNull;
        return false;
    }

}

