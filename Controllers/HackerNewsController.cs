using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace HackerNewsApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class HackerNewsController : ControllerBase
    {

        private readonly ILogger<HackerNewsController> _logger;

        public HackerNewsController(ILogger<HackerNewsController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        [Route("loadNewStories")]
        public IEnumerable<HackerNewsStory> LoadNewStories()
        {
            var rng = new Random();
            return Enumerable.Range(1, 5).Select(index => new HackerNewsStory
            {
                By = "guy",
                Score = 5,
                Time = 6,
                Title = "title",
                URL="url"
            })
            .ToArray();
        }

        [HttpGet]
        [Route("searchNewStories")]

        public IEnumerable<HackerNewsStory> SearchNewStories()
        {
            var rng = new Random();
            return Enumerable.Range(1, 5).Select(index => new HackerNewsStory
            {
                By = "guy",
                Score = 5,
                Time = 6,
                Title = "title",
                URL = "url"
            })
            .ToArray();
        }
    }
}
