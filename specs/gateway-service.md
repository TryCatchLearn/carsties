# GatewayService

## Summary

Single public entry point for the frontend. A YARP reverse proxy that fronts AuctionService,
SearchService, BidService and NotificationService behind one origin, applying JWT auth per-route
(only the write routes require a token) and a CORS policy scoped to the frontend origin for the
SignalR route. Holds no data and no business logic of its own.

## Infrastructure

.Net Web API
YARP reverse proxy
JWT Bearer auth (validated against IdentityService, enforced per-route)
CORS (frontend origin only, for the `/notifications` route)
No database

## Nuget Packages

Microsoft.AspNetCore.Authentication.JwtBearer
Yarp.ReverseProxy

## External (User)

None directly — all "external" operations are proxied through to the downstream services listed below.

## Queries handled

None directly — proxied.

## Events emitted

None.

## Events consumed

None.

## Routes (YARP)

|Route|Path|Methods|Downstream|Auth|
|---|---|---|---|---|
|auctionsRead|`/auctions/{**catch-all}` → `api/auctions/{**catch-all}`|GET|AuctionService|Anon|
|auctionsWrite|`/auctions/{**catch-all}` → `api/auctions/{**catch-all}`|POST, PUT, DELETE|AuctionService|Auth|
|search|`/search/{**catch-all}` → `api/search/{**catch-all}`|GET|SearchService|Anon|
|bidsWrite|`/bids` → `api/bids`|POST|BidService|Auth|
|bidsRead|`/bids/{**catch-all}` → `api/bids/{**catch-all}`|GET|BidService|Anon|
|notifications|`/notifications/{**catch-all}`|any (WebSocket upgrade)|NotificationService|Anon (CORS-scoped to the frontend origin, configured via `ClientApp`)|

Clusters (destinations) are environment-specific — configured per-environment (`appsettings.Development.json` points at `localhost:7001/7002/7003/7004`; Docker Compose points at the container hostnames).

## API Endpoints

None of its own — see Routes table above; this service *is* the API surface the frontend talks to.

## Models

None.

## DTOs

None.

## Event Emitted Types

None.

## Event Consumed Types

None.
