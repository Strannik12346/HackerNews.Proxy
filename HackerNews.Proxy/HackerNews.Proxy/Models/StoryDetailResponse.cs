using System.Text.Json.Serialization;

namespace HackerNews.Proxy.Models
{
    public class StoryDetailResponse
    {
        public string? Title { get; init; }

        public string? Uri { get; init; }

        [JsonPropertyName("postedBy")]
        public string? PostedBy { get; init; }

        public int Time { get; init; }

        public int Score { get; init; }

        [JsonPropertyName("commentCount")]
        public int? CommentCount { get; init; }

        [JsonIgnore]
        public int Id { get; init; }
    }
}
