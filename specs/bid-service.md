# BidService

## Summary

Handles placing bids and tracking each auction's bid history. Keeps its own lightweight read-model
copy of an auction (id, end time, seller, reserve price, finished flag) so it can validate bids
without calling AuctionService synchronously on every request — that copy is built from
`AuctionCreated` events, with a gRPC call to AuctionService as a fallback if a bid arrives for an
auction it hasn't seen yet. Also owns ending auctions: it schedules a check for each auction's end
time and publishes `AuctionFinished` when that fires.

## Infrastructure

.Net Web API (Minimal APIs)
Postgres DB
Dapper (raw SQL) + dbup-postgresql (schema migrations)
Service Bus - RabbitMQ (WolverineFx)
gRPC client (fallback auction lookups against AuctionService)

## Nuget Packages

Dapper
dbup-postgresql
Google.Protobuf
Grpc.Net.Client
Grpc.Tools
Mapster
Microsoft.AspNetCore.Authentication.JwtBearer
Npgsql
WolverineFx.Postgresql
WolverineFx.RabbitMQ
WolverineFx.RuntimeCompilation

## External (User)

**PlaceBid** - Places a bid on an auction. If the auction isn't in the local read model, falls back to a gRPC call to AuctionService and caches the result. Rejects bids from the auction's own seller. Bid is stamped `Finished`/`TooLow`/`AcceptedBelowReserve`/`Accepted` based on auction end time and current highest bid. Emits **BidPlaced**

## Queries handled

**GetBidsForAuction** - Gets all bids for an auction, newest first. Returns list of **BidDto**

## Events emitted

**BidPlaced** - When a bid is placed, in response to PlaceBid
**AuctionFinished** - When a scheduled `CheckAuctionEnded` message fires for an auction that hasn't already been marked finished

## Events consumed

**AuctionService.AuctionCreated** - Creates a local `Auction` read-model row (id, auctionEnd, seller, reservePrice) and schedules an internal `CheckAuctionEnded` message to fire at `AuctionEnd`

## Internal scheduled messages

**CheckAuctionEnded(AuctionId)** - Not a RabbitMQ event; a Wolverine durable scheduled message created per-auction when `AuctionCreated` is consumed, timed to fire at the auction's end. On fire: marks the local auction `Finished`, looks up the winning bid (highest `Accepted` bid), and publishes **AuctionFinished**. No-ops if the auction is already finished or unknown.

## gRPC (client)

Calls **AuctionService.GrpcAuction/GetAuction(auctionId)** when a bid arrives for an auction not present in the local read model (e.g. BidService started after the auction was created, or missed the event). On success, the result is persisted into the local `Auction` table so future bids don't need the gRPC round trip. Returns `null` on `NOT_FOUND`, which surfaces to the caller as 404.

## API Endpoints

|POST|api/bids|Place a bid (`auctionId`, `amount` as query params)|Auth|
|---|---|---|---|
|GET|api/bids/:auctionId|Get bids for an auction, newest first|Anon|

Auth is JWT Bearer, validated against IdentityService (`Authority` = IdentityServiceUrl), with `NameClaimType` mapped to the `username` claim.

## Models

**Bid.cs**

| Property Name | Property Type | Default Value |
| -------------- | -------------- | -------------- |
| Id             | string (Guid string) | Guid.NewGuid().ToString() |
| AuctionId      | string          |                |
| Bidder         | string          |                |
| BidTime        | DateTime        | DateTime.UtcNow |
| Amount         | int             |                |
| BidStatus      | BidStatus       |                |

**Auction.cs** (local read model, not the full AuctionService entity)

| Property Name | Property Type | Default Value |
| -------------- | -------------- | -------------- |
| Id             | string          |                |
| AuctionEnd     | DateTime        |                |
| Seller         | string          |                |
| ReservePrice   | int             |                |
| Finished       | bool            | false          |

**BidStatus.cs (enum)**

|Accepted|— the bid was accepted, and is the current highest bid|
|---|---|
|AcceptedBelowReserve|— the bid was accepted, but is below the reserve|
|TooLow|— the bid was not at least higher than the current highest bid|
|Finished|— the auction has already finished|

## DTOs

**BidDto.cs**

|Property Name|Property Type|Default Value|
|---|---|---|
|Id|string||
|AuctionId|string||
|Bidder|string||
|BidTime|DateTime||
|Amount|int||
|BidStatus|string||

## Event Emitted Types

**BidPlaced**

|Property Name|Property Type|Default Value|
|---|---|---|
|Id|string||
|AuctionId|string||
|Bidder|string||
|BidTime|DateTime||
|Amount|int||
|BidStatus|string||

**AuctionFinished**

|Property Name|Property Type|Default Value|
|---|---|---|
|ItemSold|bool||
|AuctionId|string||
|Winner?|string||
|Seller|string||
|Amount?|int||

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

Only `Id`, `AuctionEnd`, `Seller` and `ReservePrice` are used — the rest of the payload is ignored.
