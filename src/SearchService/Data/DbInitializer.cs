using System.Text.Json;
using Meilisearch;
using SearchService.Models;
using SearchService.Services;

namespace SearchService.Data;

public class DbInitializer
{
    private const string IndexUid = "items";

    public static async Task ConfigureIndex(WebApplication app)
    {
        var client = app.Services.GetRequiredService<MeilisearchClient>();
        var index = client.Index(IndexUid);
        
        var settingsTask = await index.UpdateSettingsAsync(new Settings
        {
            SearchableAttributes = ["make", "model", "description"],
            FilterableAttributes = ["seller", "winner", "status", "auctionEnd"],
            SortableAttributes = ["auctionEnd", "currentHighBid", "createdAt", "updatedAt", "make", "model"]
        });
        await client.WaitForTaskAsync(settingsTask.TaskUid);
    }

    public static async Task FetchMissingAuctions(WebApplication app)
    {
        var client = app.Services.GetRequiredService<MeilisearchClient>();
        var index = client.Index(IndexUid);
        
        using var scope = app.Services.CreateScope();
        var auctionSvc = scope.ServiceProvider.GetRequiredService<AuctionSvcHttpClient>();

        var items = await auctionSvc.GetItemsForSearch();
        
        if (items.Count == 0)
        {
            Console.WriteLine("Search index is up to date with the AuctionService");
            return;
        }
        
        var addTask = await index.AddDocumentsAsync(items, primaryKey: "id");
        await client.WaitForTaskAsync(addTask.TaskUid);
        
        Console.WriteLine($"Search index populated with {items.Count} items");
    }
}