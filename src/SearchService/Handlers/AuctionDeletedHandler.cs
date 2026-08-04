using Contracts;
using Meilisearch;

namespace SearchService.Handlers;

public class AuctionDeletedHandler
{
    public async Task Handle(AuctionDeleted message, MeilisearchClient client)
    {
        await client.Index("items").DeleteOneDocumentAsync(message.Id);
    }
}