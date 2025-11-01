using System.Text.Json.Nodes;
using Robin.Contracts.Expressions;

namespace Robin.Evaluator.System.Text.Json;

internal sealed class JsonObjectExpressionNodeVisitor : IExpressionNodeVisitor<object?, JsonNode>
{
    internal static readonly JsonObjectExpressionNodeVisitor Instance = new();
    public object? VisitBinaryOperation(BinaryOperationExpressionNode node, JsonNode args)
    {
        var left = node.Left.Accept(this, args);
        var right= node.Right.Accept(this, args);
        switch (node.Operator)
        {
            case BinaryOperator.Add:
                break;
            case BinaryOperator.Subtract:
                break;
            case BinaryOperator.Multiply:
                break;
            case BinaryOperator.Divide:
                break;
            case BinaryOperator.Power:
                break;
            case BinaryOperator.Modulus:
                break;
            case BinaryOperator.And:
                break;
            case BinaryOperator.Or:
                break;
            case BinaryOperator.Equal:
                break;
            case BinaryOperator.NotEqual:
                break;
            case BinaryOperator.GreaterThan:
                break;
            case BinaryOperator.LessThan:
                break;
            case BinaryOperator.GreaterThanOrEqual:
                break;
            case BinaryOperator.LessThanOrEqual:
                break;
            default:
                break;
        }
        throw new NotImplementedException();
    }

    public object? VisitFunctionCall(FunctionCallNode node, JsonNode args)
    {
        throw new NotImplementedException();
    }

    public object? VisitIdenitifer(IdentifierExpressionNode node, JsonNode args)
    {
        return node.Path.Evaluate(args);
    }

    public object? VisitLiteral(LiteralExpressionNode node, JsonNode _)
    {
        return node.Constant;
    }

    public object? VisitNumber(NumberExpressionNode node, JsonNode _)
    {
        return node.Constant;
    }

    public object? VisitUnaryOperation(UnaryOperationExpressionNode node, JsonNode args)
    {
        throw new NotImplementedException();
    }
}

