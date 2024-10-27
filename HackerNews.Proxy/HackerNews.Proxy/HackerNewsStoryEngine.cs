using HackerNews.Proxy.Models;
using Microsoft.Extensions.Caching.Memory;
using System.Text.Json;

namespace HackerNews.Proxy
{
    public class HackerNewsStoryEngine : IStoryEngine
    {
        private const int ExpirationInSeconds = 1000;
        private const string Topic = "HackerNewsBestStories";
        private const string BestStoriesUrl = "https://hacker-news.firebaseio.com/v0/beststories.json";
        private const string StoryDetailsUrl = "https://hacker-news.firebaseio.com/v0/item";

        private readonly TimeSpan Expiration = TimeSpan.FromSeconds(ExpirationInSeconds);

        private readonly HttpClient _httpClient;
        private readonly IMemoryCache _cache;
        private readonly ILogger<HackerNewsStoryEngine> _logger;

        public HackerNewsStoryEngine(
            ILogger<HackerNewsStoryEngine> logger,
            IHttpClientFactory httpClientFactory,
            IMemoryCache cache)
        {
            _logger = logger;
            _httpClient = httpClientFactory.CreateClient();
            _cache = cache;
        }

        public async Task<List<int>> GetIdsAsync()
        {
            _logger.LogInformation(nameof(GetIdsAsync));

            if (!_cache.TryGetValue(Topic, out List<int> bestStoriesIds))
            {
                var response = await _httpClient.GetFromJsonAsync<List<int>>(BestStoriesUrl);

                var cacheEntryOptions = new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = Expiration
                };

                _cache.Set(Topic, response, cacheEntryOptions);

                return response ?? new List<int>();
            }

            _logger.LogInformation($"{nameof(GetIdsAsync)} resulted in {bestStoriesIds.Count} records");

            return bestStoriesIds;
        }

        public async Task<List<StoryDetailResponse>> GetDetailsAsync(IEnumerable<int> storyIds)
        {
            _logger.LogInformation(nameof(GetDetailsAsync));

            if (storyIds == null || !storyIds.Any()) return new List<StoryDetailResponse>();

            var tasks = storyIds.Select(async id =>
            {
                return await _cache.GetOrCreateAsync(id.ToString(), async record =>
                {
                    record.AbsoluteExpirationRelativeToNow = Expiration;

                    var hackerNewsStory = await GetStoryDetailsFromApiAsync(id);
                    var storyResponse = ConvertToResponse(hackerNewsStory);

                    return storyResponse;
                });
            }).ToList();

            _logger.LogInformation($"{nameof(GetDetailsAsync)} awaits {tasks.Count} tasks");

            var results = await Task.WhenAll(tasks);

            return results.ToList();
        }

        private async Task<HackerNewsStory> GetStoryDetailsFromApiAsync(int id)
        {
            _logger.LogInformation(nameof(GetStoryDetailsFromApiAsync));

            var storyResponse = await _httpClient.GetAsync($"{StoryDetailsUrl}/{id}.json");
            storyResponse.EnsureSuccessStatusCode();

            var storyJson = await storyResponse.Content.ReadAsStringAsync();

            _logger.LogInformation($"{nameof(GetStoryDetailsFromApiAsync)} returns response, length = {storyJson.Length}");

            var story = JsonSerializer.Deserialize<HackerNewsStory>(storyJson, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            return story ?? throw new NullReferenceException("Deserialization failed");
        }

        private static StoryDetailResponse ConvertToResponse(HackerNewsStory storyHackerNews)
        {
            return new StoryDetailResponse
            {
                Title = storyHackerNews.Title,
                Uri = storyHackerNews.Url,
                PostedBy = storyHackerNews.By,
                Time = storyHackerNews.Time,
                Score = storyHackerNews.Score,
                CommentCount = storyHackerNews?.Kids?.Length
            };
        }
    }
}
