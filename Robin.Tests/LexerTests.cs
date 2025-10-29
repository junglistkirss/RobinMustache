namespace Robin.tests;

public class LexerTests
{
    [Fact]
    public void TestName()
    {
        string template = @"Hello {{ name }}!
{{#items}}
  - {{title}}: {{& description}}
{{^items}}Missing item{{/items}}
{{! This is a comment }}
{{> footer}}";
        ReadOnlySpan<char> source = template.AsSpan();
        Token[] tokens = Tokenizer.Tokenize(source);
        Assert.NotEmpty(tokens);

        Token[] txt = [.. tokens.Where(x => x.Type == TokenType.Text)];
        Assert.Equal(8, txt.Length);
        Assert.Equal("Hello ", txt[0].GetValue(source).Span);
        Assert.Equal("!\n", txt[1].GetValue(source).Span);
        Assert.Equal("\n  - ", txt[2].GetValue(source).Span);
        Assert.Equal(": ", txt[3].GetValue(source).Span);
        Assert.Equal("\n", txt[4].GetValue(source).Span);
        Assert.Equal("Missing item", txt[5].GetValue(source).Span);
        Assert.Equal("\n", txt[6].GetValue(source).Span);
        Assert.Equal("\n", txt[7].GetValue(source).Span);

        Token[] vars = [.. tokens.Where(x => x.Type == TokenType.Variable)];
        Assert.Equal(2, vars.Length);
        Assert.Equal("name", vars[0].GetValue(source).Span);
        Assert.Equal("title", vars[1].GetValue(source).Span);

        Token[] uvars = [.. tokens.Where(x => x.Type == TokenType.UnescapedVariable)];
        Token s = Assert.Single(uvars);
        Assert.Equal("description", s.GetValue(source).Span);

        Token secOpen = Assert.Single(tokens, x => x.Type == TokenType.SectionOpen);
        Assert.Equal("items", secOpen.GetValue(source).Span);

        Token invSecOpen = Assert.Single(tokens, x => x.Type == TokenType.InvertedSection);
        Assert.Equal("items", invSecOpen.GetValue(source).Span);

        Token secClose = Assert.Single(tokens, x => x.Type == TokenType.SectionClose);
        Assert.Equal("items", secClose.GetValue(source).Span);

        Token com = Assert.Single(tokens, x => x.Type == TokenType.Comment);
        Assert.Equal("This is a comment", com.GetValue(source).Span);

        Token part = Assert.Single(tokens, x => x.Type == TokenType.Partial);
        Assert.Equal("footer", part.GetValue(source).Span);
    }
}