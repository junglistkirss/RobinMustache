using System;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace Robin.Nodes;

public static class Parser
{
    public static ImmutableArray<INode> Parse(this ref Lexer lexer)
    {
        List<INode> nodes = [];
        while (lexer.TryGetNextToken(out Token? token))
        {
            switch (token.Value.Type)
            {
                case TokenType.Text:
                    nodes.Add(new TextNode(lexer.GetValue(token.Value)));
                    break;
                case TokenType.Variable:
                    nodes.Add(ParseExpression(lexer.GetValue(token.Value), false));
                    break;
                case TokenType.UnescapedVariable:
                    nodes.Add(ParseExpression(lexer.GetValue(token.Value), true));
                    break;
                case TokenType.SectionOpen:
                    nodes.Add(ParseSection(ref lexer, token.Value, false));
                    break;
                case TokenType.InvertedSection:
                    nodes.Add(ParseSection(ref lexer, token.Value, true));
                    break;
                case TokenType.Comment:
                    // Ignore for now
                    break;
                // case TokenType.EOF:
                //     return nodes;
                default:
                    throw new InvalidOperationException($"Unsupported token type {token.Value.Type}");
            }
        }
        return [.. nodes];
    }

    private static INode ParseExpression(string variableExpression, bool isEscaped)
    {
        //ExpressionLexer exprLexer = new(variableExpression.AsSpan());
        
        return new VariableNode(variableExpression, isEscaped);
    }

    private static INode ParseSection(ref Lexer lexer, Token startToken, bool inverted)
    {
        string name = lexer.GetValue(startToken);
        int spaceIndex = name.IndexOf(' ');
        if (spaceIndex > 0)
        {
            string beforeSpace = name[..spaceIndex];
            string afterSpace = name[(spaceIndex + 1)..];
            // this is an helper
            return new HelperNode(beforeSpace, ParseExpression(afterSpace, false));
        }
        else
        {
            List<INode> nodes = [];
            while (lexer.TryGetNextToken(out Token? token))
            {
                if (token.Value.Type == TokenType.SectionClose && lexer.GetValue(token.Value).Equals(name))
                    break;

                switch (token.Value.Type)
                {
                    case TokenType.Text:
                        nodes.Add(new TextNode(lexer.GetValue(token.Value)));
                        break;
                    case TokenType.Variable:
                        nodes.Add(ParseExpression(lexer.GetValue(token.Value), false));
                        break;
                    case TokenType.UnescapedVariable:
                        nodes.Add(ParseExpression(lexer.GetValue(token.Value), true));
                        break;
                    case TokenType.SectionOpen:
                        nodes.Add(ParseSection(ref lexer, token.Value, false));
                        break;
                    case TokenType.InvertedSection:
                        nodes.Add(ParseSection(ref lexer, token.Value, true));
                        break;
                    case TokenType.Comment:
                        break;
                    default:
                        throw new InvalidOperationException($"Unsupported token type {token.Value.Type} in section");
                }
            }
            SectionNode section = new(name, [.. nodes], inverted);

            return section;
        }
    }
}

public interface IExpressionNode : INode;

public readonly struct IdentifierNode(string name) : IExpressionNode
{
    public string Name => name;

    public void Render(Context context, StringBuilder output)
    {
        throw new NotImplementedException();
    }
};
public readonly struct BinaryOperatorNode(string @operator, IExpressionNode left, IExpressionNode right) : IExpressionNode
{
    public string Operator { get; } = @operator;
    public IExpressionNode Left { get; } = left;
    public IExpressionNode Right { get; } = right;

    public void Render(Context context, StringBuilder output)
    {
        throw new NotImplementedException();
    }
}

public readonly struct UnaryOperatorNode(string @operator, IExpressionNode operand) : IExpressionNode
{
    public string Operator { get; } = @operator;
    public IExpressionNode Operand { get; } = operand;

    public void Render(Context context, StringBuilder output)
    {
        throw new NotImplementedException();
    }
}

public readonly struct FunctionCallNode(string functionName, ImmutableArray<IExpressionNode> arguments) : IExpressionNode
{
    public string FunctionName { get; } = functionName;
    public ImmutableArray<IExpressionNode> Arguments { get; } = arguments;

    public void Render(Context context, StringBuilder output)
    {
        throw new NotImplementedException();
    }
}

public static class ExpressionParser
{
    private enum PendingType
    {
        Identifier,
        FunctionCall
    }
    private record struct Pending(string Name, PendingType Type, List<Pending> Arguments);

    private static IExpressionNode Qualify(this Pending pending)
    {
        if(pending.Type == PendingType.Identifier)
        {
            return new IdentifierNode(pending.Name);
        }
        else if(pending.Type == PendingType.FunctionCall)
        {
            return new FunctionCallNode(pending.Name, [.. pending.Arguments.Select(Qualify)]);
        }
        else
        {
            throw new InvalidOperationException("Unsupported pending type");
        }
    }

    public static ImmutableArray<IExpressionNode> Parse(this ref ExpressionLexer lexer)
    {
        List<IExpressionNode> output = [];
        Stack<Pending> funcStack = new();
        //Stack<int> functionArgCountStack = new();
        //ExpressionToken? previousToken = null;
        ExpressionToken? currentToken = null;
        ExpressionToken? nextToken = null;

        // Pre-fetch first token
        if (lexer.TryGetNextToken(out ExpressionToken? token))
        {
            currentToken = token.Value;
        }

        while (currentToken != null)
        {
            // Peek ahead for next token
            if (lexer.TryGetNextToken(out ExpressionToken? peekedToken))
            {
                nextToken = peekedToken.Value;
            }
            else
            {
                nextToken = null;
            }
            ExpressionToken current = currentToken.Value;
            bool isPending = funcStack.TryPeek(out Pending pending);
            switch (current.Type)
            {
                case ExpressionType.Identifier:
                    string currentValue = lexer.GetValue(current);
                    // Check if next token is '(' for function call
                    if (nextToken.HasValue && nextToken.Value.Type == ExpressionType.LeftParenthesis)
                    {
                        if (isPending)
                        {
                            pending.Arguments.Add(new Pending(currentValue, PendingType.FunctionCall, [])); // Push function name
                        }
                        else
                        {
                            funcStack.Push(new Pending(currentValue, PendingType.FunctionCall, []));
                        }
                        nextToken = null; // Consume '('
                    }
                    else
                    {
                        if (isPending)
                            pending.Arguments.Add(new Pending(currentValue, PendingType.Identifier, []));
                        else
                            output.Add(new IdentifierNode(currentValue));

                        //// Track argument count in function calls
                        //if (functionArgCountStack.Count > 0 &&
                        //    (previousToken == null || previousToken.Value.Type == ExpressionType.LeftParenthesis))
                        //{
                        //    int count = functionArgCountStack.Pop();
                        //    functionArgCountStack.Push(count + 1);
                        //}
                    }
                    break;

                //case ExpressionType.Operator:
                //    // Handle unary operators
                //    bool isUnary = previousToken == null ||
                //                   previousToken.Value.Type == ExpressionType.Operator ||
                //                   previousToken.Value.Type == ExpressionType.LeftParenthesis;
                //    string currentValue = lexer.GetValue(current);
                //    if (isUnary && (currentValue == "+" || currentValue == "-"))
                //    {
                //        // Push unary operator with highest precedence
                //        operatorStack.Push(current);//, "unary" + currentValue));
                //    }
                //    else if (currentValue == ",")
                //    {
                //        // Comma separates function arguments
                //        // Pop operators until we hit '('
                //        while (operatorStack.Count > 0)
                //        {
                //            var top = operatorStack.Peek();
                //            string topValue = lexer.GetValue(top);
                //            if (top.Type == ExpressionType.LeftParenthesis)
                //                break;

                //            operatorStack.Pop();
                //            if (top.Type == ExpressionType.Operator)
                //            {
                //                if (OperatorPrecedence.TryGetValue(topValue, out int prec))
                //                {
                //                    output.Add(new OperatorNode(topValue, prec));
                //                }
                //            }
                //        }

                //        // Increment argument count
                //        if (functionArgCountStack.Count > 0)
                //        {
                //            int count = functionArgCountStack.Pop();
                //            functionArgCountStack.Push(count + 1);
                //        }
                //    }
                //    else
                //    {
                //        // Handle binary operators with precedence
                //        if (!OperatorPrecedence.TryGetValue(currentValue, out int precedence))
                //        {
                //            throw new InvalidOperationException($"Unknown operator: {currentValue}");
                //        }

                //        while (operatorStack.Count > 0)
                //        {
                //            var top = operatorStack.Peek();
                //            if (top.Type == ExpressionType.LeftParenthesis)
                //                break;

                //            if (top.Type == ExpressionType.Operator)
                //            {
                //                string topValue = lexer.GetValue(top);
                //                if (!OperatorPrecedence.TryGetValue(topValue, out int topPrecedence))
                //                    break;

                //                // Right-associative for ^ (power), left-associative for others
                //                bool isRightAssociative = currentValue == "^";
                //                if (isRightAssociative ? precedence < topPrecedence : precedence <= topPrecedence)
                //                {
                //                    operatorStack.Pop();
                //                    output.Add(new OperatorNode(topValue, topPrecedence));
                //                }
                //                else
                //                {
                //                    break;
                //                }
                //            }
                //            else
                //            {
                //                break;
                //            }
                //        }

                //        operatorStack.Push(current);
                //    }
                //    break;

                //case ExpressionType.LeftParenthesis:
                //    operatorStack.Push(current);
                //    break;

                case ExpressionType.RightParenthesis:

                    if (isPending)
                    {
                        Pending pop = funcStack.Pop();
                        if(funcStack.TryPeek(out Pending topPending))
                        {
                            topPending.Arguments.Add(pop);
                        }
                        else
                        {
                            output.Add(pop.Qualify());
                        }
                    }
                    break;

                default:
                    throw new InvalidOperationException($"Unsupported token type {current.Type}");
            }
        }

        return [.. output];
    }
}


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

