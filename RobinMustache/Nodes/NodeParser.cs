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
        while (lexer.TryGetNextToken(out Token token))
        {
            INode node = lexer.ParseNode(token);
            nodes.Add(node);

        }
        return [.. nodes];
    }

    private static LineBreakNode ParseLineBreak(string lineBreakType)
    {
        return lineBreakType switch
        {
            "\r" => LineBreakNode.InstanceReturn,
            "\n" => LineBreakNode.InstanceLine,
            _ => LineBreakNode.Instance,
        };
    }
    private static VariableNode ParseVariable(string variableExpression, bool isEscaped)
    {
        ExpressionLexer exprLexer = new(variableExpression.AsSpan());
        IExpressionNode node = exprLexer.Parse() ?? throw new Exception("Variable expression is invalid");

        return new VariableNode(node, isEscaped);
    }

    private static PartialCallNode ParsePartialCall(this ref NodeLexer lexer, Token token)
    {
        string variableExpression = lexer.GetValue(token);
        int firstSpace = variableExpression.IndexOf(' ');
        IExpressionNode expressionNode;
        string name;
        if (firstSpace == -1)
        {
            name = variableExpression;
            expressionNode = That;
        }
        else
        {
            name = variableExpression.Substring(0, firstSpace);
            var expression = variableExpression.Substring(firstSpace + 1).AsSpan();
            expressionNode = expression.ParseExpression() ?? throw new Exception("Variable expression is invalid");
        }
        return new PartialCallNode(name, expressionNode, token.IsAtlineStart && token.IsAtLineEnd);
    }

    private static PartialDefineNode ParsePartialDefine(ref NodeLexer lexer, Token startToken)
    {
        bool isStandalone = false;
        string name = lexer.GetValue(startToken);
        List<INode> nodes = [];
        bool isAtLineStart = startToken.IsAtlineStart;
        if (isAtLineStart && startToken.IsAtLineEnd)
            lexer.SkipNextLineBreak(startToken.Type);
        while (lexer.TryGetNextToken(out Token token))
        {
            if (token.Type == TokenType.SectionClose && lexer.GetValue(token).Equals(name))
            {
                isStandalone = token.IsAtLineEnd && isAtLineStart;
                if (token.IsAtlineStart && token.IsAtLineEnd)
                    RemoveTrailingLineBreak(nodes);
                break;
            }
            INode node = lexer.ParseNode(token);
            nodes.Add(node);
        }
        PartialDefineNode partial = new(name, [.. nodes], isStandalone);

        return partial;

    }

    private static SectionNode ParseSection(ref NodeLexer lexer, Token startToken, bool inverted)
    {
        bool isStandalone = false;
        string name = lexer.GetValue(startToken);
        IExpressionNode expNode = name.AsSpan().ParseExpression() ?? throw new Exception("Variable expression is invalid");
        List<INode> nodes = [];
        bool isAtLineStart = startToken.IsAtlineStart;
        if (isAtLineStart && startToken.IsAtLineEnd)
            lexer.SkipNextLineBreak(startToken.Type);
        while (lexer.TryGetNextToken(out Token token))
        {
            if (token.Type is TokenType.SectionClose && lexer.GetValue(token).Equals(name))
            {
                isStandalone = token.IsAtLineEnd && isAtLineStart;
                if (token.IsAtlineStart && token.IsAtLineEnd)
                    RemoveTrailingLineBreak(nodes);
                break;
            }
            INode node = lexer.ParseNode(token);
            nodes.Add(node);
        }
        SectionNode section = new(expNode, [.. nodes], isStandalone, inverted);

        return section;
    }

    private static CommentNode ParseComment(this ref NodeLexer lexer, Token token)
    {
        return new(lexer.GetValue(token), token.IsAtlineStart && token.IsAtLineEnd);
    }

    private static INode ParseNode(this ref NodeLexer lexer, Token token)
    {
        return token.Type switch
        {
            TokenType.Text => new TextNode(lexer.GetValue(token)),
            TokenType.Variable => ParseVariable(lexer.GetValue(token), false),
            TokenType.UnescapedVariable => ParseVariable(lexer.GetValue(token), true),
            TokenType.SectionOpen => ParseSection(ref lexer, token, false),
            TokenType.InvertedSection => ParseSection(ref lexer, token, true),
            TokenType.Comment => ParseComment(ref lexer, token),
            TokenType.PartialCall => lexer.ParsePartialCall(token),
            TokenType.PartialDefine => ParsePartialDefine(ref lexer, token),
            TokenType.LineBreak => ParseLineBreak(lexer.GetValue(token)),
            TokenType.Whitepsaces => new WhitespaceNode(lexer.GetValue(token)),
            _ => throw new InvalidTokenException($"Unsupported token {token} in section"),
        };
    }
    private static void RemoveTrailingLineBreak(this List<INode> nodes)
    {
        for (int i = nodes.Count - 1; i >= 0; i--)
        {
            if (nodes[i] is WhitespaceNode)
                nodes.RemoveAt(i);
            else if (nodes[i] is LineBreakNode)
            {
                nodes.RemoveAt(i);
                break; // stop après avoir retiré un seul saut de ligne
            }
            else break;
        }
    }

    private static void SkipNextLineBreak(this ref NodeLexer lexer, TokenType currentType)
    {
        if (currentType is TokenType.SectionOpen or TokenType.SectionClose or TokenType.InvertedSection or TokenType.PartialDefine or TokenType.PartialCall)
            while (lexer.TryPeekNextToken(out Token nextToken, out int next) && nextToken.Type is TokenType.LineBreak or TokenType.Whitepsaces)
            {
                lexer.AdvanceTo(next);
            }
    }
}