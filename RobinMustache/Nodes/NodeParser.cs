using RobinMustache.Abstractions.Expressions;
using RobinMustache.Abstractions.Nodes;
using RobinMustache.Abstractions.Variables;
using RobinMustache.Expressions;
using RobinMustache.Nodes;
using System.Collections.Immutable;

namespace RobinMustache.Nodes;

public static class NodeParser
{
    private readonly static IdentifierExpressionNode That = new(new VariablePath([ThisSegment.Instance]));

    public static bool TryParse(this ref NodeLexer lexer, out ImmutableArray<INode>? nodes)
    {
        try
        {
            nodes = Parse(ref lexer);
            return true;
        }
        catch (Exception)
        {
            nodes = null;
            return false;
        }
    }

    public static ImmutableArray<INode> Parse(this ref NodeLexer lexer)
    {
        List<INode> nodes = [];
        while (lexer.TryGetNextToken(out Token? token) && token is not null)
        {
            nodes.Add(lexer.ParseNode(token.Value));
        }
        return [.. nodes];
    }

    private static LineBreakNode AggregateLineBreaks(ref NodeLexer lexer)
    {
        int c = 1;
        while (lexer.TryPeekNextToken(out Token? nextToken, out int position) && nextToken is not null && nextToken.Value.Type == TokenType.LineBreak)
        {
            c++;
            lexer.AdvanceTo(position);
        }
        return new LineBreakNode(c);
    }

    private static VariableNode ParseVariable(string variableExpression, bool isEscaped)
    {
        ExpressionLexer exprLexer = new(variableExpression.AsSpan());
        IExpressionNode node = exprLexer.Parse() ?? throw new Exception("Variable expression is invalid");

        return new VariableNode(node, isEscaped);
    }

    private static PartialCallNode ParsePartialCall(string variableExpression)
    {
        int firstSpace = variableExpression.IndexOf(' ');
        if (firstSpace == -1)
        {
            return new PartialCallNode(variableExpression, That);
        }
        else
        {
            string name = variableExpression.Substring(0, firstSpace);
            ExpressionLexer exprLexer = new(variableExpression.Substring(firstSpace + 1).AsSpan());
            IExpressionNode node = exprLexer.Parse() ?? throw new Exception("Variable expression is invalid");
            return new PartialCallNode(name, node);
        }
    }

    private static PartialDefineNode ParsePartialDefine(ref NodeLexer lexer, Token startToken)
    {
        string name = lexer.GetValue(startToken);
        List<INode> nodes = [];
        while (lexer.TryGetNextToken(out Token? token) && token is not null)
        {
            if (token.Value.Type == TokenType.SectionClose && lexer.GetValue(token.Value).Equals(name))
                break;

            nodes.Add(lexer.ParseNode(token.Value));
        }
        PartialDefineNode partial = new(name, [.. nodes]);

        return partial;

    }

    private static SectionNode ParseSection(ref NodeLexer lexer, Token startToken, bool inverted)
    {
        string name = lexer.GetValue(startToken);
        ExpressionLexer exprLexer = new(name.AsSpan());
        IExpressionNode node = exprLexer.Parse() ?? throw new Exception("Variable expression is invalid");
        List<INode> nodes = [];
        while (lexer.TryGetNextToken(out Token? token) && token is not null)
        {
            if (token.Value.Type == TokenType.SectionClose && lexer.GetValue(token.Value).Equals(name))
                break;

            nodes.Add(lexer.ParseNode(token.Value));
        }
        SectionNode section = new(node, [.. nodes], inverted);

        return section;
    }

    private static INode ParseNode(this ref NodeLexer lexer, Token token)
    {
        switch (token.Type)
        {
            case TokenType.Text:
                return new TextNode(lexer.GetValue(token));
            case TokenType.Variable:
                return ParseVariable(lexer.GetValue(token), false);
            case TokenType.UnescapedVariable:
                return ParseVariable(lexer.GetValue(token), true);
            case TokenType.SectionOpen:
                return ParseSection(ref lexer, token, false);
            case TokenType.InvertedSection:
                return ParseSection(ref lexer, token, true);
            case TokenType.Comment:
                return new CommentNode(lexer.GetValue(token));
            case TokenType.PartialCall:
                return ParsePartialCall(lexer.GetValue(token));
            case TokenType.PartialDefine:
                return ParsePartialDefine(ref lexer, token);
            case TokenType.LineBreak:
                return AggregateLineBreaks(ref lexer);
            default:
                throw new InvalidTokenException($"Unsupported token {token} in section");
        }
    }
}