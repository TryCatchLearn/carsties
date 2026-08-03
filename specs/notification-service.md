# NotificationService

## Summary

Thin fan-out bridge from RabbitMQ to the browser. Has no database and no REST API — it consumes the
same three domain events every other service consumes, and rebroadcasts each one verbatim to all
connected clients over a SignalR hub so the frontend can show live toasts (new auction, bid placed,
auction finished) without polling.

## Infrastructure

.Net Web API
SignalR Hub (WebSocket push to browser clients)
Service Bus - RabbitMQ (WolverineFx)
No database

## Nuget Packages

Mapster
WolverineFx.RabbitMQ
WolverineFx.RuntimeCompilation

## External (User)

None — no REST API. Clients connect to the SignalR hub directly.

## Queries handled

None.

## Events emitted

None (rebroadcasts happen over SignalR, not RabbitMQ).

## Events consumed

**AuctionService.AuctionCreated** - Rebroadcast to all SignalR clients as `AuctionCreated`
**BidService.AuctionFinished** - Rebroadcast to all SignalR clients as `AuctionFinished`
**BidService.BidPlaced** - Rebroadcast to all SignalR clients as `BidPlaced`

Each handler simply forwards the deserialized event payload unchanged via `hubContext.Clients.All.SendAsync(...)`.

## SignalR Hub

**`/notifications`** - `NotificationHub` (empty `Hub` subclass — server never receives client-invoked methods, it's push-only). Routed through GatewayService with a CORS policy allowing the frontend origin (needed because SignalR negotiation isn't a simple CORS request).

## API Endpoints

None — only the SignalR hub endpoint above.

## Models

None.

## DTOs

None — events are forwarded as-is; the client (frontend) defines its own matching shape.

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

**BidService.AuctionFinished**

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
