# IdentityService

## Summary

OIDC/OAuth2 identity provider for the whole system, built on Duende IdentityServer over ASP.NET
Core Identity. Issues the JWTs that every other backend service validates, and is the only service
with its own login/consent/logout UI (Razor Pages). Seeds two demo users (`alice`, `bob`) on first
run. No REST API and no RabbitMQ involvement — it only speaks the OIDC protocol plus Razor Pages UI.

## Infrastructure

.Net Web API + Razor Pages
Duende IdentityServer (OIDC/OAuth2 provider)
ASP.NET Core Identity (user store)
Postgres DB
Entity Framework ORM
Serilog (console + diagnostic file logging)
No RabbitMQ

## Nuget Packages

Duende.IdentityServer.AspNetIdentity
Microsoft.AspNetCore.Diagnostics.EntityFrameworkCore
Microsoft.AspNetCore.Identity.EntityFrameworkCore
Microsoft.AspNetCore.Identity.UI
Microsoft.EntityFrameworkCore.Tools
Npgsql.EntityFrameworkCore.PostgreSQL
Serilog.AspNetCore

## External (User)

Not a REST API — "external" surface is the OIDC protocol plus Identity's Razor Pages UI (login, logout, consent, register, external login, device flow, CIBA, grants, server-side sessions, diagnostics).

## Queries handled

None (not a data API — see OIDC endpoints below).

## Events emitted

None (no RabbitMQ integration).

## Events consumed

None.

## OIDC configuration

**Identity resources**: `openid`, `profile`, `email`
**API scopes**: `auctionApp` — "Auction app full access"

**Clients**:

|ClientId|Grant Type|Notes|
|---|---|---|
|bruno|Resource Owner Password|For direct API testing via the Bruno client; scopes: openid, profile, email, auctionApp|
|nextApp|Authorization Code + PKCE|The Next.js frontend; redirect URI = `{ClientAppUrl}/api/auth/oauth2/callback/duende`; scopes: openid, profile, email, auctionApp; access token lifetime 30 days|

**Custom claims** (`CustomProfileService`): every issued token gets a `username` claim (the ASP.NET Identity username), `email`, `email_verified`, and — if set — a `name` claim. Downstream services (AuctionService, BidService, GatewayService) all set `TokenValidationParameters.NameClaimType = "username"`, so `User.Identity.Name` resolves to this claim, not the default `sub`/`name`.

**Seed data**: on first run (only if no users exist), creates `alice` (AliceSmith@example.com) and `bob` (BobSmith@example.com), both password `Pass123$`, with a `name` claim of "Alice Smith" / "Bob Smith".

## API Endpoints

No conventional REST endpoints. Exposes IdentityServer's standard OIDC protocol endpoints (`/connect/authorize`, `/connect/token`, `/connect/userinfo`, `/connect/endsession`, discovery document, etc.) plus authenticated Razor Pages for account/consent/device/CIBA flows.

## Models

**ApplicationUser.cs**

| Property Name | Property Type | Default Value |
| -------------- | -------------- | -------------- |
| (all standard `IdentityUser` fields — Id, UserName, Email, etc.) | | |

No additional fields have been added yet.

## DTOs

None (page-bound `InputModel`/`ViewModel` types per Razor Page, not a public API contract).

## Event Emitted Types

None.

## Event Consumed Types

None.
