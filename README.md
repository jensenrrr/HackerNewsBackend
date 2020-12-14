

# Nextech HackerNews App

## Goal

Angular and .NET Core application that displays new stories from the Hacker News API and allow users to search those stories.

## Initial Design

### Frontend

The frontend architecture looks fairly simple. Design is a little awkward, I'm essentially thinking of how I would do it in a React app and may be missing out on some Angular best practices.

Services:

- NewsService: In charge of managing our data, which consists of our array of hacker-news-stories:
  - loadNewStories(storiesToLoad): Calls a GET request to our backend at "api/loadNewStories?{storiesToLoad}".
    - This is mostly backend design, but the requirements seemed to indicate we should load stories incrementally as the user reaches the bottom of the page. storiesToLoad is a number showing how many stories we've loaded already, so if storiesToLoad == 20 then the backend knows to send us stories 21-40.
  - searchNewsStories(searchTerm): Calls a GET request to our backend at "api/search?{searchTerm}".

Components:

- hacker-news-card: Display information for a Hacker News Story given the appropriate data.
- search-bar: Takes in user input.
- app-component: Parent Component. Displays the search-bar at the top and a list of hacker-news-cards below it.
  - onInit -> Calls loadNewStories(0).
  - User reaches end of stories -> calls loadNewStories(storiesToLoad)

### Backend Development

Our backend is a simple web API that gets new Hackers News stories and caches them. Caching the stories is a requirement of the coding assessment, but it comes with benefits and drawbacks.

- Benefits: We don't have to call the hacker news api to get our news stories, which means we respond to the client faster and don't run the risk of getting rate limited by over active clients. It's also easier to incrementally serve stories to the client, since the data is static in between our calls to the hacker news api.
- Drawbacks: The user isn't getting the most up to date new stories and if our server updates the cache while the user already has stories loaded, we have the issue of possibly loading the same stories again.

Tasks?: Call "/v0/newstories" every x minutes to get the latest 500 news stories. Store them inside our .NET application as an "stories" object.

API:

- api/loadNewStories?{storiesToLoad}: returns a subset of the stories object from index {storiesToLoad} to index {storiesToLoad + x} (x is however many stories we want to return.
- api/searchNewStories?{searchTerm}: returns all stories that have a title or by attribute that contain the string {searchTerm}.

## Backend Development

Creating the .NET backend took much more time than expected. I'm confident I could've written the same backend in Node/Express in less than two hours (optimistically one hour).

A big reason it took so long to make this backend was researching into doing specific tasks, such as caching or running a task on startup, failing to figure it out quickly enough that it would be worth doing it for this use case and then falling back on the way I knew I could do it without too much pain.

I think the backend design is fine for what the application is. There might be an argument to be made to for splitting HackerNewsService into two services, one that handles/caches the data and one that requests data from the (real) Hacker News API, but at this scale there's no point.

The biggest issues with the backend are:

 1. The .NET application doesn't get the newest HackerNews stories on startup.
	* This means that the first call to our controller is VERY slow, as it has to do 500 calls to the real HackerNews api.
	* Additionally the current setup never refreshes on HackerNews' stories.
2. There are no unit tests. I started up an xUnit project to start adding unit tests and stopped for two reasons.
	* The way I set the git repository up meant that I would have to push the testing project separately or mess with the position of all the files or find out something else I was missing.
	* When I was almost done testing the controller (but running into some type bugs) I realized I was going to have to add tests for HackerNewsService (the more important part to add tests to in the first place) as well and simply didn't have the time for it or the frontend tests.
