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

    public static IEnumerable<Client> Clients =>
    [
        new()
        {
            ClientId = "bruno",
            ClientName = "Bruno Client",
            AllowedScopes = { "openid", "profile", "auctionApp" },
            AllowedGrantTypes = GrantTypes.ResourceOwnerPassword,
            ClientSecrets = {new Secret("NotASecret".Sha256())},
        }
    ];
}