namespace Robin.Expressions;

public interface IExpressionNode
{
    TOut Accept<TOut, TArgs>(IExpressionNodeVisitor<TOut, TArgs> visitor, TArgs args);
};

public interface IExpressionNodeVisitor<TOut, TArgs>
{
    TOut VisitIdenitifer(IdentifierNode node, TArgs args);
    TOut VisitFunctionCall(FunctionCallNode node, TArgs args);
    TOut VisitNumber(NumberNode node, TArgs args);
    TOut VisitLiteral(LiteralNode node, TArgs args);
    TOut VisitVariable(VariableNode node, TArgs args);
    TOut VisitBinaryOperation(BinaryOperationNode node, TArgs args);
    TOut VisitUnaryOperation(UnaryOperationNode node, TArgs args);
}