using System.Security.Claims;
using Duende.IdentityServer.EntityFramework.DbContexts;
using Duende.IdentityServer.EntityFramework.Mappers;
using Duende.IdentityServer.Models;
using IdentityModel;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using StockChat.Identity.Database;

namespace StockChat.Identity;

public class SeedData
{
  public static void EnsureSeedData(WebApplication app)
  {
    using var scope = app.Services.GetRequiredService<IServiceScopeFactory>().CreateScope();
    scope.ServiceProvider.GetService<PersistedGrantDbContext>()?.Database.Migrate();
    scope.ServiceProvider.GetService<ApplicationDbContext>()?.Database.Migrate();
    
    var context = scope.ServiceProvider.GetService<ConfigurationDbContext>();
    context?.Database.Migrate();
    if (context != null) 
      EnsureSeedData(context);
    
    EnsureUsers(scope);
  }

  private static void EnsureSeedData(ConfigurationDbContext context)
  {
    if (!context.Clients.Any())
    {
      foreach (var client in Config.Clients.ToList())
      {
        context.Clients.Add(client.ToEntity());
      }

      context.SaveChanges();
    }

    if (!context.IdentityResources.Any())
    {
      foreach (var resource in Config.IdentityResources.ToList())
      {
        context.IdentityResources.Add(resource.ToEntity());
      }

      context.SaveChanges();
    }

    if (!context.ApiScopes.Any())
    {
      foreach (var resource in Config.ApiScopes.ToList())
      {
        context.ApiScopes.Add(resource.ToEntity());
      }

      context.SaveChanges();
    }

    if (!context.ApiResources.Any())
    {
      foreach (var resource in Config.ApiResources.ToList())
      {
        context.ApiResources.Add(resource.ToEntity());
      }

      context.SaveChanges();
    }

    if (!context.IdentityProviders.Any())
    {
      context.IdentityProviders.Add(new OidcProvider
      {
        Scheme = "demoidsrv",
        DisplayName = "IdentityServer",
        Authority = "https://demo.duendesoftware.com",
        ClientId = "login",
      }.ToEntity());
      context.SaveChanges();
    }
  }

  private static void EnsureUsers(IServiceScope scope)
  {
    var userMgr = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
    var alice = userMgr.FindByNameAsync("alice").Result;
    if (alice == null)
    {
      alice = new IdentityUser
      {
        UserName = "alice",
        Email = "AliceSmith@email.com",
        EmailConfirmed = true,
      };
      var result = userMgr.CreateAsync(alice, "Pass123$").Result;
      if (!result.Succeeded)
      {
        throw new Exception(result.Errors.First().Description);
      }

      result = userMgr.AddClaimsAsync(alice, new Claim[]
      {
        new Claim(JwtClaimTypes.Name, "Alice Smith"),
        new Claim(JwtClaimTypes.GivenName, "Alice"),
        new Claim(JwtClaimTypes.FamilyName, "Smith"),
        new Claim(JwtClaimTypes.WebSite, "http://alice.com"),
      }).Result;
      if (!result.Succeeded)
      {
        throw new Exception(result.Errors.First().Description);
      }

    }

    var bob = userMgr.FindByNameAsync("bob").Result;
    if (bob == null)
    {
      bob = new IdentityUser
      {
        UserName = "bob",
        Email = "BobSmith@email.com",
        EmailConfirmed = true
      };
      var result = userMgr.CreateAsync(bob, "Pass123$").Result;
      if (!result.Succeeded)
      {
        throw new Exception(result.Errors.First().Description);
      }

      result = userMgr.AddClaimsAsync(bob, new Claim[]
      {
        new Claim(JwtClaimTypes.Name, "Bob Smith"),
        new Claim(JwtClaimTypes.GivenName, "Bob"),
        new Claim(JwtClaimTypes.FamilyName, "Smith"),
        new Claim(JwtClaimTypes.WebSite, "http://bob.com"),
        new Claim("location", "somewhere")
      }).Result;
      if (!result.Succeeded)
      {
        throw new Exception(result.Errors.First().Description);
      }
    }
  }
}