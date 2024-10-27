namespace HackerNews.Proxy.Models
{
    public record HackerNewsStory
    {
        public string? By { get; init; }

        public int Descendants { get; init; }

        public int Id { get; init; }

        public int[]? Kids { get; init; }

        public int Score { get; init; }

        public int Time { get; init; }

        public string? Title { get; init; }

        public string? Url { get; init; }
    }
}
