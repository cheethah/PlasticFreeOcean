    using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AspNet.Security.OpenIdConnect.Extensions;
using AspNet.Security.OpenIdConnect.Primitives;
using AspNet.Security.OpenIdConnect.Server;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using PlasticFreeOcean.Models;

namespace PlasticFreeOcean.OuthProvider
{
    public sealed class AuthorizationProvider : OpenIdConnectServerProvider
    {
       
        public override Task ValidateTokenRequest(ValidateTokenRequestContext context)
        {
            if (!context.Request.IsPasswordGrantType() && !context.Request.IsRefreshTokenGrantType())
            {
                context.Reject(
                    error: OpenIdConnectConstants.Errors.UnsupportedGrantType,
                    description: "Only the resource owner password credentials and refresh token " +
                                 "grants are accepted by this authorization server");
                return Task.FromResult(0);
            }
            context.Skip();
            return Task.FromResult(0);
        }


        public override async Task HandleTokenRequest(HandleTokenRequestContext context)
        {
            var manager = context.HttpContext.RequestServices.GetRequiredService<UserManager<User>>();
            //var rolemanager = context.HttpContext.RequestServices.GetRequiredService<RoleManager<Role>>();
            if (context.Request.IsPasswordGrantType())
            {
                var user = await manager.FindByNameAsync(context.Request.Username);
                var userRole = manager.GetRolesAsync(user).Result.SingleOrDefault();
                // Retrieve the application details corresponding to the requested client_id.
                //var Role = rolemanager.Roles.Single(x => x.Name == userRole).Name;


                if (user == null)
                {
                    context.Reject(
                        error: OpenIdConnectConstants.Errors.InvalidGrant,
                        description: "Invalid credentials.");
                    return;
                }
                if (manager.SupportsUserTwoFactor && await manager.GetTwoFactorEnabledAsync(user))
                {
                    context.Reject(
                        error: OpenIdConnectConstants.Errors.InvalidGrant,
                        description: "Two-factor authentication is required for this account.");
                    return;
                }
                // Ensure the user is not already locked out.
                if (manager.SupportsUserLockout && await manager.IsLockedOutAsync(user))
                {
                    context.Reject(
                        error: OpenIdConnectConstants.Errors.InvalidGrant,
                        description: "Invalid credentials.");
                    return;
                }
                // Ensure the password is valid.
                if (!await manager.CheckPasswordAsync(user, context.Request.Password))
                {
                    if (manager.SupportsUserLockout)
                    {
                        await manager.AccessFailedAsync(user);
                    }
                    context.Reject(
                        error: OpenIdConnectConstants.Errors.InvalidGrant,
                        description: "Invalid credentials.");
                    return;
                }
                if (manager.SupportsUserLockout)
                {
                    await manager.ResetAccessFailedCountAsync(user);
                }
                var identity = new ClaimsIdentity(
                    OpenIdConnectServerDefaults.AuthenticationScheme,
                    OpenIdConnectConstants.Claims.Name,
                    OpenIdConnectConstants.Claims.Role);

                identity.AddClaim(OpenIdConnectConstants.Claims.Subject, user.Id.ToString(),
                    OpenIdConnectConstants.Destinations.AccessToken,
                    OpenIdConnectConstants.Destinations.IdentityToken);

                identity.AddClaim("Id", user.Id.ToString(),
                    OpenIdConnectConstants.Destinations.AccessToken,
                    OpenIdConnectConstants.Destinations.IdentityToken);

                identity.AddClaim(OpenIdConnectConstants.Claims.Name, user.UserName,
                    OpenIdConnectConstants.Destinations.AccessToken,
                    OpenIdConnectConstants.Destinations.IdentityToken);

                identity.AddClaim(OpenIdConnectConstants.Claims.Role, "Cuk",
                    OpenIdConnectConstants.Destinations.AccessToken,
                    OpenIdConnectConstants.Destinations.IdentityToken);

                // Create a new authentication ticket holding the user identity.
                var ticket = new AuthenticationTicket(
                    new ClaimsPrincipal(identity),
                    new AuthenticationProperties(),
                    OpenIdConnectServerDefaults.AuthenticationScheme);
                ticket.SetScopes(
                    /* email: */ OpenIdConnectConstants.Scopes.Email,
                    /* profile: */ OpenIdConnectConstants.Scopes.Profile, 
                    OpenIdConnectConstants.Scopes.OfflineAccess
                );
                ticket.SetResources("resource_server");
                context.Validate(ticket);
            }
        }
    }
}
