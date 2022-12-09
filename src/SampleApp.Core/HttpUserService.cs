using AspNet.Security.OpenIdConnect.Primitives;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace SampleApp.Core
{
    public interface IHttpUserService
    {
        UserPrincipal GetCurrentUser();
        UserPrincipal GetCurrentUserIdOrDefault();
    }

    public class HttpUserService : IHttpUserService
    {
        private readonly IHttpContextAccessor _httpContext;

        public HttpUserService(IHttpContextAccessor httpContext)
        {
            _httpContext = httpContext;
        }

        public UserPrincipal GetCurrentUser()
        {
            if (_httpContext.HttpContext != null && _httpContext.HttpContext.User != null)
            {
                return new UserPrincipal(_httpContext.HttpContext.User);
            }

            throw new Exception("Current user cannot be determined");
        }

        public UserPrincipal GetCurrentUserIdOrDefault()
        {
            if (_httpContext.HttpContext != null && _httpContext.HttpContext.User != null)
            {
                return new UserPrincipal(_httpContext.HttpContext.User);
            }

            return null;
        }
    }

    public class UserPrincipal : ClaimsPrincipal
    {
        public UserPrincipal(ClaimsPrincipal principal) : base(principal)
        {
        }

        private string GetClaimValue(string key)
        {
            var identity = Identity as ClaimsIdentity;
            if (identity == null)
                return null;

            var claim = identity.Claims.FirstOrDefault(c => c.Type == key);
            return claim?.Value;
        }

        public string Email
        {
            get
            {
                if (FindFirst(OpenIdConnectConstants.Claims.Email) == null)
                    return string.Empty;

                return GetClaimValue(OpenIdConnectConstants.Claims.Email);
            }
        }

        public int UserId
        {
            get
            {
                if (FindFirst(OpenIdConnectConstants.Claims.Subject) == null)
                    return default;

                return Convert.ToInt32(GetClaimValue(OpenIdConnectConstants.Claims.Subject));
            }
        }

        public string FirstName
        {
            get
            {
                var usernameClaim = FindFirst(OpenIdConnectConstants.Claims.GivenName);

                if (usernameClaim == null)
                    return string.Empty;

                return usernameClaim.Value;
            }
        }

        public string LastName
        {
            get
            {
                var usernameClaim = FindFirst(OpenIdConnectConstants.Claims.FamilyName);

                if (usernameClaim == null)
                    return string.Empty;

                return usernameClaim.Value;
            }
        }
    }
}
