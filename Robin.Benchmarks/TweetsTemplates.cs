namespace Robin.Benchmarks;

public static class TweetsTemplates
{
    public const string List = @"<ul class=""tweets"">
  {{#.}}
  <li class=""tweet"" id=""tweet-{{id}}"">
    <header class=""tweet-header"">
      <strong class=""tweet-user"">@{{user}}</strong>
      <span class=""tweet-date"">
        {{#formatDate}}{{created_at}}{{/formatDate}}
        {{^formatDate}}{{created_at}}{{/formatDate}}
      </span>
    </header>

    <div class=""tweet-content"">
      {{content}}
    </div>

    <div class=""tweet-hashtags"">
    {{#hashtags}}
        <a class=""hashtag"" href=""/search?q={{.}}"">{{.}}</a>{{^last}} {{/last}}
    {{/hashtags}}
    </div>

    <div class=""tweet-meta"">
      <span class=""note"">Note: 
        {{#formatNote}}{{note}}{{/formatNote}}
        {{^formatNote}}{{note}}{{/formatNote}}
      </span>
      <span class=""likes"">Likes {{likes}}</span>
      <span class=""retweets"">Retwwets: {{retweets}}</span>
      <span class=""bookmarks"">Favorites {{bookmarks}}</span>
      <span class=""lang"">({{language}})</span>
    </div>

    {{#replies}}
    <div class=""tweet-replies"">
      <small>{{replies.length}} réponse(s)</small>
      <ul>
        {{#replies}}
          <li class=""reply-id""><a href=""#tweet-{{.}}"">{{.}}</a></li>
        {{/replies}}
      </ul>
    </div>
    {{/replies}}
  </li>
  {{/.}}
</ul>";
}