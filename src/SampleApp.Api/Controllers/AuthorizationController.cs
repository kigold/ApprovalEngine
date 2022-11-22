using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using OpenIddict.Abstractions;
using OpenIddict.Server.AspNetCore;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using static OpenIddict.Abstractions.OpenIddictConstants;
using Microsoft.AspNetCore.Authentication;
using SampleApp.Core.Data.Entities;
using Microsoft.AspNetCore.Authorization;

namespace SampleApp.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class AuthorizationController : ControllerBase
    {
        private readonly IOpenIddictApplicationManager _applicationManager;
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly RoleManager<Role> _roleManager;

        public AuthorizationController(UserManager<User> userManager, SignInManager<User> signInManager, RoleManager<Role> roleManager, IOpenIddictApplicationManager applicationManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _applicationManager = applicationManager;
            _roleManager= roleManager;
        }

        [HttpPost("~/connect/token"), Produces("application/json")]
        public async Task<IActionResult> Exchange()
        {
            var request = HttpContext.GetOpenIddictServerRequest();
            ClaimsPrincipal claimsPrincipal;

            if (request.IsPasswordGrantType())
                return await TokensForPasswordGrantType(request);

            if (request.IsClientCredentialsGrantType())
            {
                // Note: the client credentials are automatically validated by OpenIddict:
                // if client_id or client_secret are invalid, this action won't be invoked.

                var application = await _applicationManager.FindByClientIdAsync(request.ClientId) ??
                    throw new InvalidOperationException("The application cannot be found.");

                // Create a new ClaimsIdentity containing the claims that
                // will be used to create an id_token, a token or a code.
                var identity = new ClaimsIdentity(TokenValidationParameters.DefaultAuthenticationType, Claims.Name, Claims.Role);

                // Use the client_id as the subject identifier.
                identity.AddClaim(Claims.Subject,
                    await _applicationManager.GetClientIdAsync(application),
                    Destinations.AccessToken, Destinations.IdentityToken);

                identity.AddClaim(Claims.Name,
                    await _applicationManager.GetDisplayNameAsync(application),
                    Destinations.AccessToken, Destinations.IdentityToken);

                claimsPrincipal = new ClaimsPrincipal(identity);
            }
            else if (request.IsAuthorizationCodeGrantType())
            {
                // Retrieve the claims principal stored in the authorization code
                claimsPrincipal = (await HttpContext.AuthenticateAsync(OpenIddictServerAspNetCoreDefaults.AuthenticationScheme)).Principal;
            }
            else
            {
                throw new NotImplementedException("The specified grant is not implemented.");
            }

            //return SignIn(new ClaimsPrincipal(identity), OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
            return SignIn(claimsPrincipal, OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
        }

        private async Task<IActionResult> TokensForPasswordGrantType(OpenIddictRequest request)
        {
            var user = await _userManager.FindByNameAsync(request.Username);
            if (user == null)
                return Unauthorized();

            var signInResult = await _signInManager.CheckPasswordSignInAsync(user, request.Password, false);
            if (signInResult.Succeeded)
            {
                var identity = new ClaimsIdentity(
                    TokenValidationParameters.DefaultAuthenticationType,
                    OpenIddictConstants.Claims.Name,
                    OpenIddictConstants.Claims.Role);

                identity.AddClaim(OpenIddictConstants.Claims.Subject, user.Id.ToString(), OpenIddictConstants.Destinations.AccessToken);
                identity.AddClaim(OpenIddictConstants.Claims.Username, user.UserName, OpenIddictConstants.Destinations.AccessToken);
                // Add more claims if necessary

                foreach (var userRole in await _userManager.GetRolesAsync(user))
                {
                    identity.AddClaim(OpenIddictConstants.Claims.Role, userRole, OpenIddictConstants.Destinations.AccessToken);
                }

                var claimsPrincipal = new ClaimsPrincipal(identity);
                claimsPrincipal.SetScopes(new string[]
                {
                    OpenIddictConstants.Scopes.Roles,
                    OpenIddictConstants.Scopes.OfflineAccess,
                    OpenIddictConstants.Scopes.Email,
                    OpenIddictConstants.Scopes.Profile,
                    "api"
                });

                return SignIn(claimsPrincipal, OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
            }
            else
                return Unauthorized();
        }

    }
}

//TODO Correctly implement AuthorizationCode flow Grant_type
