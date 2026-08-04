using Contracts;
using Mapster;
using Meilisearch;
using SearchService.Models;

namespace SearchService.Handlers;

public class TransientSearchException(string message) : Exception(message);

public class AuctionCreatedHandler
{
    private static readonly HashSet<string> FailedOnce = [];
    
    public async Task Handle(AuctionCreated message, MeilisearchClient client)
    {
        if (message.Make == "fail-once")
        {
            if (FailedOnce.Add(message.Id))
            {
                throw new TransientSearchException($"Simulated failure for {message.Id}");
            }
        }

        if (message.Make == "fail-always")
        {
            throw new TransientSearchException($"Simulated always failure for {message.Id}");
        }
        
        var item = message.Adapt<Item>();
        await client.Index("items").AddDocumentsAsync([item]);
    }
}