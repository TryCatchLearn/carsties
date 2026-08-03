# SearchService

## Summary

Read-only, denormalized search index for auctions, backed by Meilisearch rather than a relational
DB. Builds its index purely from RabbitMQ events (create/update/delete/finish/bid), and on startup
catches up on anything it missed by asking AuctionService for auctions updated since its most
recently indexed document. Exposes anonymous search/filter/sort endpoints — no write API, no auth.

## Infrastructure

- .Net Web API (Minimal APIs)
- Meilisearch (search index — no relational DB)
- Service Bus - RabbitMQ (WolverineFx)
- HttpClient with HTTP resilience handler (startup catch-up sync against AuctionService)

## Nuget Packages

* Mapster
* MeiliSearch
* Microsoft.Extensions.Http.Resilience
* WolverineFx.RabbitMQ
* WolverineFx.RuntimeCompilation

## External (User)

None — SearchService has no authenticated write endpoints; it only builds its index from events.

## Queries handled

- **Search** - Full-text search (`make`/`model`/`description`) with optional `seller`/`winner` filters, a `filterBy` date filter (`live`/`finished`/`endingSoon` — ending within 6 hours), an `orderBy` sort (`make`, `new`, `endingSoon`), and pagination (`pageNumber`, `pageSize`, capped at 50). Returns `{ results, pageCount, totalCount }`
- **GetById** - Gets a single indexed item by ID. Returns 404 if not found

## Events emitted

None.

## Events consumed

- **AuctionService.AuctionCreated** - Adds the auction as a document to the `items` index. Includes a deliberate test hook: `Make == "fail-once"` throws once per auction ID, `Make == "fail-always"` always throws — both as `TransientSearchException`, which Wolverine retries (2x, 500ms/500ms/1s cooldown) before routing to the error queue, demonstrating the retry/dead-letter policy
- **AuctionService.AuctionUpdated** - Updates the document's make/model/description/year/color/mileage
- **AuctionService.AuctionDeleted** - Removes the document from the index
- **AuctionService.AuctionFinished** - Sets `Winner`/`SoldAmount` (if `ItemSold`) and `Status = "Finished"` on the document
- **BidService.BidPlaced** - Updates `CurrentHighBid` on the document if the bid is `Accepted`/`AcceptedBelowReserve` and higher than the current value

## Startup catch-up sync

On boot, `DbInitializer` configures the Meilisearch index settings (searchable/filterable/sortable attributes), then calls `AuctionSvcHttpClient.GetItemsForSearch()`, which looks up the most recently `updatedAt` document already indexed and calls `AuctionService.GET /api/auctions?date=<that timestamp>` to fetch anything newer, and bulk-adds it to the index. This closes the gap for auctions created/updated while SearchService was down (events published to a queue it wasn't yet bound to, or a fresh index).

## API Endpoints

|GET|api/search|Search auctions (`searchTerm`, `seller`, `winner`, `orderBy`, `filterBy`, `pageNumber`, `pageSize`)|Anon|
|---|---|---|---|
|GET|api/search/:id|Get an indexed auction by id|Anon|

## Models

**Item.cs** (Meilisearch document — mirrors AuctionService's `AuctionDto`)

| Property Name   | Property Type | Default Value |
| ---------------- | -------------- | -------------- |
| Id                | string         |                |
| ReservePrice      | int            |                |
| Seller            | string         |                |
| Winner?           | string         |                |
| SoldAmount        | int            |                |
| CurrentHighBid    | int            |                |
| CreatedAt         | DateTime       |                |
| UpdatedAt         | DateTime       |                |
| AuctionEnd        | DateTime       |                |
| Status            | string         |                |
| Make              | string         |                |
| Model             | string         |                |
| Description       | string         |                |
| Year              | int            |                |
| Color             | string         |                |
| Mileage           | int            |                |
| ImageUrl          | string         |                |

Index config: `SearchableAttributes = [make, model, description]`, `FilterableAttributes = [seller, winner, status, auctionEnd]`, `SortableAttributes = [auctionEnd, currentHighBid, createdAt, updatedAt, make, model]`.

## DTOs

None — the Meilisearch `Item` document is returned directly (as search hits, or the raw document for get-by-id).

## Event Consumed Types

**AuctionService.AuctionCreated**

|Property Name|Property Type|Default Value|
|---|---|---|
|Id|string||
|ReservePrice|int||
|Seller|string||
|Winner?|string||
|SoldAmount|int||
|CurrentHighBid|int||
|CreatedAt|DateTime||
|UpdatedAt|DateTime||
|AuctionEnd|DateTime||
|Status|string||
|Make|string||
|Model|string||
|Description|string||
|Year|int||
|Color|string||
|Mileage|int||
|ImageUrl|string||

**AuctionService.AuctionUpdated**

|Property Name|Property Type|Default Value|
|---|---|---|
|Id|string||
|Make|string||
|Model|string||
|Description|string||
|Year|int||
|Color|string||
|Mileage|int||

**AuctionService.AuctionDeleted**

|Property Name|Property Type|Default Value|
|---|---|---|
|Id|string||

**AuctionService.AuctionFinished**

|Property Name|Property Type|Default Value|
|---|---|---|
|ItemSold|bool||
|AuctionId|string||
|Winner?|string||
|Seller|string||
|Amount?|int||

**BidService.BidPlaced**

|Property Name|Property Type|Default Value|
|---|---|---|
|Id|string||
|AuctionId|string||
|Bidder|string||
|BidTime|DateTime||
|Amount|int||
|BidStatus|string||
