using System.Security.Claims;
using Duende.IdentityModel;
using Duende.IdentityServer.Models;
using Duende.IdentityServer.Services;
using IdentityService.Models;
using Microsoft.AspNetCore.Identity;

namespace IdentityService.Services;

public class CustomProfileService(UserManager<ApplicationUser> userManager) : IProfileService
{
    public async Task GetProfileDataAsync(ProfileDataRequestContext context, CancellationToken ct)
    {
        var user = await userManager.GetUserAsync(context.Subject)
            ?? throw new InvalidOperationException("User not found");
        
        var existingClaims = await userManager.GetClaimsAsync(user);

        var claims = new List<Claim>
        {
            new("username", user.UserName ?? string.Empty),
            new(JwtClaimTypes.Email, user.Email ?? string.Empty),
        };
        
        context.IssuedClaims.AddRange(claims);
        
        var nameClaim = existingClaims.FirstOrDefault(c => c.Type == JwtClaimTypes.Name);
        if (nameClaim != null)
        {
            context.IssuedClaims.Add(nameClaim);
        }
    }

    public Task IsActiveAsync(IsActiveContext context, CancellationToken ct)
    {
        return Task.CompletedTask;
    }
}