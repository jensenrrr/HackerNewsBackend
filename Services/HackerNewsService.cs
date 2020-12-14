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
    private IEnumerable<HackerNewsStory> _storiesCache = new List<HackerNewsStory>() { };
	public HackerNewsService(IHttpClientFactory clientFactory)
	{
		_clientFactory = clientFactory;
    }

    public async Task<IEnumerable<HackerNewsStory>> SearchStories(string searchTerm)
    {
        if (_storiesCache == null || !_storiesCache.Any() || _storiesCache.Count() == 0)
        {
            await FetchNewsStories();
        }
        return _storiesCache.ToList().Where(story => 
            story.By.IndexOf(searchTerm, StringComparison.OrdinalIgnoreCase) >= 0 || 
            story.Title.IndexOf(searchTerm, StringComparison.OrdinalIgnoreCase) >= 0);
    }

    public async Task<IEnumerable<HackerNewsStory>> GetNewStories(int loadedPages)
    {
        if (_storiesCache == null || !_storiesCache.Any() || _storiesCache.Count() == 0)
        {
            await FetchNewsStories();
            return _storiesCache.ToList().GetRange(0, 20);
        }
        else
        {
            var count = 20;
            if (loadedPages + count >= _storiesCache.Count())
            {
                count = _storiesCache.Count() - loadedPages;
            }
            if (loadedPages >= _storiesCache.Count())
            {
                return null;
            }
            return _storiesCache.ToList().GetRange(loadedPages, count);
        }

    }
    private async Task FetchNewsStories()
    {
        var request = new HttpRequestMessage(HttpMethod.Get, "https://hacker-news.firebaseio.com/v0/newstories.json");
        var client = _clientFactory.CreateClient();
        var response = await client.SendAsync(request);
        if (response.IsSuccessStatusCode)
        {
            using var responseStream = await response.Content.ReadAsStreamAsync();
            var storyIDs = await System.Text.Json.JsonSerializer.DeserializeAsync<IEnumerable<int>>(responseStream);
            var taskArray = new List<Task<HackerNewsStory>>() { };
            foreach (var id in storyIDs)
            {
                taskArray.Add(GetStory(id));
            }
            await Task.WhenAll(taskArray);
            var returnedStories = new List<HackerNewsStory>() { };
            foreach (var task in taskArray)
            {
                if (task.Result != null)
                {
                    returnedStories.Add(task.Result);
                }
            }
            _storiesCache = returnedStories.OrderBy(story => story.Time);
        }
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



}