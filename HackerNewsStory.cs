using System;

namespace HackerNewsApi
{
    public class HackerNewsStory
    {
        public string By { get; set; }
        public int Descendants { get; set; }
        public int Score { get; set; }
        public int Time { get; set; }
        public string Title { get; set; }
        public string URL { get; set; }
    }
}
