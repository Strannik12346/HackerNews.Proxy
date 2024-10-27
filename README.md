# HackerNews.Proxy

## Functional requirements

Using ASP.NET Core, implement a RESTful API to retrieve the details of the first n "best stories" from the Hacker News API, where n is specified by the caller to the API.

The Hacker News API is documented here: https://github.com/HackerNews/API .
The IDs for the "best stories" can be retrieved from this URI: https://hacker-news.firebaseio.com/v0/beststories.json .
The details for an individual story ID can be retrieved from this URI: https://hacker-news.firebaseio.com/v0/item/21233041.json (in this case for the story with ID
21233041)

The API should return an array of the first n "best stories" as returned by the Hacker News API, sorted by their score in a descending order, in the form:

```
[  
    {  
        "title": "A uBlock Origin update was rejected from the Chrome Web Store",  
        "uri": "https://github.com/uBlockOrigin/uBlock-issues/issues/745",  
        "postedBy": "ismaildonmez",  
        "time": "2019-10-12T13:43:01+00:00",  
        "score": 1716,  
        "commentCount": 572  
    },  
    { ... },  
    { ... },  
    { ... },  
    ...  
]  
```

## Non-Functional requirements

In addition to the above, your API should be able to efficiently service large numbers of requests without risking overloading of the Hacker News API.

README.md file should describe how to run the application, any assumptions and any enhancements or potential future changes to this solution

## How to run the application
```
1. Go to HackerNews.Proxy\HackerNews.Proxy (find .csproj file)
2. Launch cmd or powershell in the folder with .csproj file
3. Run cmd "dotnet run --configuration Debug"
4. Open https://localhost:7012/swagger in browser
```

## Assumptions and future TODOs

1. HackerNews BestStories API seems to return 200 items without pagination.  
   The API designed in this repository implies this constraint.
2. First request to HackerNews API could be optimized to perform n+1 calls instead of all.  
   Current implementation gets and saves all 200 results to in-memory cache.
3. Several constant values from HackerNewsStoryEngine could be moved to appsettings.json.
4. Other launch settings and configuration profiles could be provided.

## Thanks for reading!
