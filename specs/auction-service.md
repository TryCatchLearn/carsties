# AuctionService

## Summary

Owns auction and item data — the system of record for auctions. Exposes a REST API for
creating/reading/updating/deleting auctions, publishes domain events over RabbitMQ whenever an
auction changes, and exposes a gRPC endpoint that BidService calls to fetch an auction it doesn't
yet have locally. Also consumes bid/auction-lifecycle events from BidService to keep `CurrentHighBid`,
`Winner`, `SoldAmount` and `Status` up to date.

## Infrastructure

.Net Web API
Postgres DB
Entity Framework ORM
Service Bus - RabbitMQ (WolverineFx)
gRPC server (auction lookups for BidService)

## Nuget Packages

Grpc.AspNetCore
Mapster
Microsoft.AspNetCore.Authentication.JwtBearer
Microsoft.EntityFrameworkCore.Design
Npgsql.EntityFrameworkCore.PostgreSQL
WolverineFx.EntityFrameworkCore
WolverineFx.Postgresql
WolverineFx.RabbitMQ
WolverineFx.RuntimeCompilation

## External (User)

- **CreateAuction** - Creates an Auction/Item. Emits **AuctionCreated**
- **UpdateAuction** - Updates an auction's item details. Rejected if the auction already has a bid (`CurrentHighBid > 0`) or the caller isn't the seller. Emits **AuctionUpdated**
- **DeleteAuction** - Deletes an auction. Rejected if the auction already has a bid or the caller isn't the seller. Emits **AuctionDeleted**

## Queries handled

- **GetAuctions** - Gets all auctions, optionally filtered to `UpdatedAt > date` (used by SearchService to catch up on startup). Returns list of **AuctionDto**, ordered by Make/Model
- **GetAuctionById** - Gets an auction by ID. Returns **AuctionDto**

## Events emitted

- **AuctionCreated** - When an auction is created, in response to CreateAuction
- **AuctionUpdated** - When an auction is updated, in response to UpdateAuction
- **AuctionDeleted** - When an auction is deleted, in response to DeleteAuction

All three are published via the Wolverine EF Core outbox (`IDbContextOutbox<AuctionDbContext>`) in the same transaction as the DB write, to fanout exchanges (`auction-created`, `auction-updated`, `auction-deleted`).

## Events consumed

- **BidService.BidPlaced** - Updates `CurrentHighBid` when the incoming bid is `Accepted`/`AcceptedBelowReserve` and higher than the current value
- **BidService.AuctionFinished** - Sets `Winner`/`SoldAmount` (if `ItemSold`) and moves `Status` to `Finished` or `ReserveNotMet` depending on whether `SoldAmount > ReservePrice`

Also listens on `wolverine-dead-letter-queue`: a `DeadLetterHandler` logs any `AuctionCreated` message that lands there (test/demo hook, not part of normal flow).

## gRPC

**GetAuction(auctionId) → GrpcAuctionResponse** - Returns `{ id, auctionEnd, seller, reservePrice, finished }` for an auction. Called by BidService when it receives a bid for an auction it doesn't have in its own read model yet (e.g. BidService restarted before consuming `AuctionCreated`). Returns `NOT_FOUND` status if the auction doesn't exist.

## API Endpoints

|POST|api/auctions|Create auction|Auth|
|---|---|---|---|
|PUT|api/auctions/:id|Update auction (owner + no bids only)|Auth|
|DELETE|api/auctions/:id|Delete auction (owner + no bids only)|Auth|
|GET|api/auctions|Get auctions (optional `?date=` filter)|Anon|
|GET|api/auctions/:id|Get auction by id|Anon|
|POST|api/auctions/test|Echoes the authenticated username (auth smoke-test endpoint)|Auth|

Auth is JWT Bearer, validated against IdentityService (`Authority` = IdentityServiceUrl), with `NameClaimType` mapped to the `username` claim.

## Models

**Auction.cs**

| Property Name   | Property Type                | Default Value      |
| --------------- | ----------------------------- | ------------------- |
| Id              | string (Guid string)          | Guid.NewGuid().ToString() |
| ReservePrice    | int                            | 0                    |
| Seller          | string (username from claim)   |                      |
| Winner?         | string (username of winner)    |                      |
| SoldAmount?     | int                             |                      |
| CurrentHighBid? | int                             |                      |
| CreatedAt       | DateTime                       | DateTime.UtcNow      |
| UpdatedAt       | DateTime                       | DateTime.UtcNow      |
| AuctionEnd      | DateTime                       |                      |
| Status          | Status                          | Status.Live (default enum value) |
| Item            | Item                            |                      |

**Item.cs**

| Property Name | Property Type                    | Default Value |
| ------------- | ---------------------------------- | ------------- |
| Id            | string (Guid string)                | Guid.NewGuid().ToString() |
| Make          | string                              |               |
| Model         | string                              |               |
| Year          | int                                  |               |
| Color         | string                              |               |
| Mileage       | int                                  |               |
| Description   | string                              |               |
| ImageUrl      | string                              |               |
| Auction       | Auction? (related to **Auction**)   |               |

**Status.cs (enum)**

|Live|
|---|
|Finished|
|ReserveNotMet|

## DTOs

**AuctionDto.cs**

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

**CreateAuctionDto.cs**

|Property Name|Property Type|Default Value|
|---|---|---|
|Make|string||
|Model|string||
|Year|int||
|Color|string||
|Mileage|int||
|ImageUrl|string||
|Description|string||
|ReservePrice|int||
|AuctionEnd|DateTime||

**UpdateAuctionDto.cs**

|Property Name|Property Type|Default Value|
|---|---|---|
|Make?|string||
|Model?|string||
|Description?|string||
|Year?|int||
|Color?|string||
|Mileage?|int||

## Event Emitted Types

**AuctionCreated**

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

**AuctionUpdated**

|Property Name|Property Type|Default Value|
|---|---|---|
|Id|string||
|Make|string||
|Model|string||
|Description|string||
|Year|int||
|Color|string||
|Mileage|int||

**AuctionDeleted**

|Property Name|Property Type|Default Value|
|---|---|---|
|Id|string||

## Event Consumed Types

**BidService.BidPlaced**

|Property Name|Property Type|Default Value|
|---|---|---|
|Id|string||
|AuctionId|string||
|Bidder|string||
|BidTime|DateTime||
|Amount|int||
|BidStatus|string||

**BidService.AuctionFinished**

|Property Name|Property Type|Default Value|
|---|---|---|
|ItemSold|bool||
|AuctionId|string||
|Winner?|string||
|Seller|string||
|Amount?|int||
