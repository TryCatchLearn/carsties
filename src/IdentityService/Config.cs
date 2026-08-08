using Duende.IdentityServer.Models;

namespace IdentityService;

public static class Config
{
    public static IEnumerable<IdentityResource> IdentityResources =>
    [
        new IdentityResources.OpenId(),
        new IdentityResources.Profile()
    ];

    public static IEnumerable<ApiScope> ApiScopes =>
    [
        new ApiScope("auctionApp", "Auction app full access")
    ];

    public static IEnumerable<Client> Clients(IConfiguration config) =>
    [
        new()
        {
            ClientId = "bruno",
            ClientName = "Bruno Client",
            AllowedScopes = { "openid", "profile", "auctionApp" },
            AllowedGrantTypes = GrantTypes.ResourceOwnerPassword,
            ClientSecrets = {new Secret("NotASecret".Sha256())},
        },
        new()
        {
            ClientId = "nextApp",
            ClientName = "NextApp Client",
            ClientSecrets = {new Secret("NotASecret".Sha256())},
            AllowedGrantTypes = GrantTypes.Code,
            RedirectUris = {config["ClientAppUrl"] + "/api/auth/oauth2/callback/duende" },
            AllowedScopes = { "openid", "profile", "auctionApp" },
            AccessTokenLifetime = 3600 * 24 * 30
        }
    ];
}