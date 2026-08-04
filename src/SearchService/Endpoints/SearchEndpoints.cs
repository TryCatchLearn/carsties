using Meilisearch;
using SearchService.Models;

namespace SearchService.Endpoints;

public static class SearchEndpoints
{
    public static async Task<IResult> GetAuctionById(MeilisearchClient client, string id)
    {
        try
        {
            var result = await client.Index("items").GetDocumentAsync<Item>(id);
            
            return Results.Ok(result);
        }
        catch (MeilisearchApiError e) when (e.Code == "document_not_found")
        {
            return Results.NotFound();
        }
    }
    
    public static async Task<IResult> GetSearchResults(
        MeilisearchClient client, 
        string? searchTerm,
        string? seller,
        string? winner,
        string? orderBy,
        string? filterBy,
        int pageNumber = 1,
        int pageSize = 10
        )
    {
        var filters = new List<string>();
        if (!string.IsNullOrEmpty(seller))
            filters.Add($"seller={seller}");
        if (!string.IsNullOrEmpty(winner))
            filters.Add($"winner={winner}");
        if (!string.IsNullOrEmpty(filterBy))
        {
            var now =  DateTime.UtcNow;
            var dateFilter = filterBy switch
            {
                "live" => $"auctionEnd > \"{now:o}\"",
                "finished" => $"auctionEnd < \"{now:o}\"",
                "endingSoon" => $"auctionEnd > \"{now:o}\" AND auctionEnd < \"{now.AddHours(6):o}\"",
                _ => null
            };
            if (dateFilter != null)
                filters.Add(dateFilter);
        }

        List<string>? sort = orderBy switch
        {
            "make" => ["make:asc", "model:asc"],
            "new" => ["createdAt:desc"],
            "endingSoon" => ["auctionEnd:asc"],
            _ => null
        };
        
        var query = new SearchQuery
        {
            Page = pageNumber < 1 ? 1 : pageNumber,
            HitsPerPage = pageSize > 50 ? 50 : pageSize,
            Filter = filters.Count > 0 ? string.Join(" AND ", filters) : null,
            Sort = sort,
        };
    
        var result = (PaginatedSearchResult<Item>)await client.Index("items")
            .SearchAsync<Item>(searchTerm ?? string.Empty, query);
    
        return Results.Ok(new
        {
            results = result.Hits,
            pageCount = result.TotalPages,
            totalCount = result.TotalHits
        });
    }
}