using Robin.Generators.Accessor;

namespace Robin.tests;

[GenerateAccessor]
public record Tweet(
    Guid Id,
    string User,
    string Content,
    string[] Hashtags,
    double Note,
    DateTime CreatedAt,
    int Likes,
    int Retweets,
    int Bookmarks,
    string Language,
    Guid[] Replies
);