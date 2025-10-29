using System.Collections.Immutable;

namespace Robin.Nodes;

public static class Parser
{
    public static ImmutableArray<INode> Parse(ref Lexer lexer)
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
                    nodes.Add(new VariableNode(lexer.GetValue(token.Value), false));
                    break;
                case TokenType.UnescapedVariable:
                    nodes.Add(new VariableNode(lexer.GetValue(token.Value), true));
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
        return [..nodes];
    }

    private static SectionNode ParseSection(ref Lexer lexer, Token startToken, bool inverted)
    {
        List<INode> nodes = [];

        ReadOnlyMemory<char> name = lexer.GetValue(startToken);
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
                    nodes.Add(new VariableNode(lexer.GetValue(token.Value), false));
                    break;
                case TokenType.UnescapedVariable:
                    nodes.Add(new VariableNode(lexer.GetValue(token.Value), true));
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

