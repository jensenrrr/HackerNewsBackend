using HackerNewsApi;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Newtonsoft.Json;

public class HackerNewsService : IHackerNewsService
{
	private readonly IHttpClientFactory _clientFactory;
    public IEnumerable<HackerNewsStory> StoriesCache { get; private set; } = new List<HackerNewsStory>() { };
	public HackerNewsService(IHttpClientFactory clientFactory)
	{
		_clientFactory = clientFactory;
    }
    private async Task<HackerNewsStory> GetStory(int storyID)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, $"https://hacker-news.firebaseio.com/v0/item/{storyID.ToString()}.json");
        var client = _clientFactory.CreateClient();
        var response = await client.SendAsync(request);
        if (response.IsSuccessStatusCode)
        {
            var contents = await response.Content.ReadAsStringAsync();
            var story = JsonConvert.DeserializeObject<HackerNewsStory>(contents);
            return story;
        }
        else
        {
            return null;
        }
    }
	public async Task<IEnumerable<HackerNewsStory>> GetNewStories(int loadedPages)
    {
        if (StoriesCache == null || !StoriesCache.Any() || StoriesCache.Count() == 0)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, "https://hacker-news.firebaseio.com/v0/newstories.json");
            var client = _clientFactory.CreateClient();
            var response = await client.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                using var responseStream = await response.Content.ReadAsStreamAsync();
                var storyIDs = await System.Text.Json.JsonSerializer.DeserializeAsync<IEnumerable<int>>(responseStream);
                storyIDs = storyIDs.Take(64);
                var taskArray = new List<Task<HackerNewsStory>>() { };
                foreach (var id in storyIDs)
                {
                    taskArray.Add(GetStory(id));
                }
                await Task<HackerNewsStory>.WhenAll(taskArray);
                var returnedStories = new List<HackerNewsStory>() { };
                foreach (var task in taskArray)
                {
                    if (task.Result != null)
                    {
                        returnedStories.Add(task.Result);
                    }
                }
                StoriesCache = returnedStories.OrderBy(story => story.Time);
                return StoriesCache.ToList().GetRange(0, 20);
            }
            else
            {
                return StoriesCache;
            }
        }
        else
        {
            var count = 20;
            if (loadedPages + count >= StoriesCache.Count())
            {
                count = StoriesCache.Count() - loadedPages;
            }
            if(loadedPages>= StoriesCache.Count())
            {
                return null;
            }
            return StoriesCache.ToList().GetRange(loadedPages, count); 
        }
        
    }
    public IEnumerable<HackerNewsStory> SearchStories(string searchTerm)
    {        
        return StoriesCache.ToList().Where(story => 
                story.By.IndexOf(searchTerm, StringComparison.OrdinalIgnoreCase) >= 0 || 
                story.Title.IndexOf(searchTerm, StringComparison.OrdinalIgnoreCase) >= 0);
    }

}