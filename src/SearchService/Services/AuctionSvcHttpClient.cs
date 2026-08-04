using Meilisearch;
using SearchService.Models;

namespace SearchService.Services;

public class AuctionSvcHttpClient(HttpClient httpClient, IConfiguration config, 
    MeilisearchClient searchClient)
{
    public async Task<List<Item>> GetItemsForSearch()
    {
        var search = await searchClient.Index("items")
            .SearchAsync<Item>(string.Empty, new SearchQuery
            {
                Sort = ["updatedAt:desc"],
                Limit = 1,
            });

        var lastUpdated = search.Hits.FirstOrDefault()?.UpdatedAt;
        
        var dateParam = lastUpdated?.ToString("O") ?? string.Empty;

        var items = await httpClient.GetFromJsonAsync<List<Item>>(
            $"{config["AuctionSvcUrl"]}/api/auctions?date={Uri.EscapeDataString(dateParam)}"
        );
        
        return items ?? [];
    }
}