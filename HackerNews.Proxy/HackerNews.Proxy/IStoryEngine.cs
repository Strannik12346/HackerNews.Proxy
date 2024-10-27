using HackerNews.Proxy.Models;

namespace HackerNews.Proxy
{
    public interface IStoryEngine
    {
        Task<List<int>> GetIdsAsync();
    
        Task<List<StoryDetailResponse>> GetDetailsAsync(IEnumerable<int> storyIds);
    }
}
