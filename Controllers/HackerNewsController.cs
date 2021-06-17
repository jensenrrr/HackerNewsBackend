using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace HackerNewsApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HackerNewsController : ControllerBase
    {

        private readonly IHackerNewsService _newsService;

        public HackerNewsController(IHackerNewsService newsService)
        {
            _newsService = newsService;
        }

        [HttpGet]
        [Route("loadNewStories/{loadedPages}/{storyCount}")]
        public IEnumerable<HackerNewsStory> LoadNewStories(int loadedPages, int storyCount)
        {
            return _newsService.GetNewStories(loadedPages, storyCount).Result;
        }

        [HttpGet]
        [Route("searchNewStories/{searchTerm}")]
        public IEnumerable<HackerNewsStory> SearchNewStories(string searchTerm)
        {
            return _newsService.SearchStories(searchTerm).Result;
        }
    }
}
