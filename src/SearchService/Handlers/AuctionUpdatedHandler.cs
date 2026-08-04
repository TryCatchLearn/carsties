using Contracts;
using Meilisearch;

namespace SearchService.Handlers;

public class AuctionUpdatedHandler
{
    public async Task Handle(AuctionUpdated message, MeilisearchClient client)
    {
        await client.Index("items").UpdateDocumentsAsync([message]);
    }
}