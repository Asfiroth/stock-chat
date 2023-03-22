using Duende.IdentityServer.Models;

namespace StockChat.Identity;

public static class Config
{
    public static IEnumerable<IdentityResource> IdentityResources =>
        new IdentityResource[]
        {
            new IdentityResources.OpenId(),
            new IdentityResources.Profile(),
        };

    public static IEnumerable<ApiScope> ApiScopes =>
        new ApiScope[]
        {
            new ApiScope("chatapi.read"),
            new ApiScope("chatapi.write"),
        };

    public static IEnumerable<ApiResource> ApiResources => new[]
    {
        new ApiResource("chatapi", "Chat API")
        {
            Scopes = new List<string> {"chatapi.read", "chatapi.write"},
            ApiSecrets = new List<Secret> {new Secret("SuperSecretScope".Sha256())},
            UserClaims = new List<string> {"role"}
        }
    };
    
    public static IEnumerable<Client> Clients =>
        new Client[]
        {
            new Client
            {
                ClientId = "m2m.client",
                ClientName = "Client Credentials Client",

                AllowedGrantTypes = GrantTypes.ClientCredentials,

                ClientSecrets = { new Secret("511536EF-F270-4058-80CA-1C89C192F69A".Sha256()) },
                AllowedScopes = {"chatapi.read", "chatapi.write"}
            },
            
            new Client
            {
                ClientId = "webapp",
                ClientSecrets = { new Secret("49C1A7E1-0C79-4A89-A3D6-A37998FB86B0".Sha256()) },
                AllowedGrantTypes = GrantTypes.Code,
                RequireConsent = false,
                RequirePkce = true,
                AllowOfflineAccess = true,
                RedirectUris = new List<string> { "https://web:6011/signin-oidc" },
                PostLogoutRedirectUris = new List<string> { "https://web:6011/signout-callback-oidc" },
                AllowedScopes =
                {
                    "openid", 
                    "profile", 
                    "chatapi.read", 
                    "chatapi.write"
                },
            },

        };
}