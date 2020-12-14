using HackerNewsApi;
using System.Collections.Generic;
using System.Threading.Tasks;

public interface IHackerNewsService
{
    Task<IEnumerable<HackerNewsStory>> GetNewStories(int loadedPages);
    Task<IEnumerable<HackerNewsStory>> SearchStories(string searchTerm);

}
