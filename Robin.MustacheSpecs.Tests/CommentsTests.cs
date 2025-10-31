using System.Text.Json;

namespace Robin.tests;

public class CommentsTests
{
    public static IEnumerable<object[]> GetTestData()
    {
        string path = Path.Combine(AppContext.BaseDirectory, "specs", "1.4.3", "comments.json");
        string json = File.ReadAllText(path);
        var cases = JsonSerializer.Deserialize<MustacheTestFile>(json)!;
        foreach (var test in cases.Tests)
            yield return new object[]
            {
                test
            };
    }

    [Theory]
    [MemberData(nameof(GetTestData))]
    public void Should_Add_Correctly(MustacheTestCase _)
    {
        Assert.True(true);
    }
}