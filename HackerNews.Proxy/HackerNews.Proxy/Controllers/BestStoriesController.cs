using HackerNews.Proxy.Models;
using Microsoft.AspNetCore.Mvc;

namespace HackerNews.Proxy.Controllers
{
    [ApiController]
    public class BestStoriesController : ControllerBase
    {
        private readonly IStoryEngine _storyEngine;

        public BestStoriesController(IStoryEngine engine)
        {
            _storyEngine = engine;
        }

        [HttpGet("api/[action]/{n}")]
        public async Task<ActionResult<IEnumerable<StoryDetailResponse>>> NBestStories(int n)
        {
            var ids = await _storyEngine.GetIdsAsync();
            var stories = await GetBestStoriesDetailsAsync(ids, n);
            return Ok(stories);
        }

        private async Task<StoryDetailResponse[]> GetBestStoriesDetailsAsync(
            IEnumerable<int> storyIds, int n)
        {
            if (storyIds == null) return Array.Empty<StoryDetailResponse>();
            
            var storyIdList = storyIds.ToList();

            if (storyIdList.Count == 0) return Array.Empty<StoryDetailResponse>();

            var details = await _storyEngine.GetDetailsAsync(storyIds);

            return details.OrderByDescending(story => story.Score).Take(n).ToArray();
        }
    }
}
