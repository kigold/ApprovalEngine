using Microsoft.AspNetCore.Identity;

namespace SampleApp.Core.Data.Entities
{
    public class User : IdentityUser<long>
    {
        public string Firstname { get; set; }
        public string Lastname { get; set; }
    }

    public class Role : IdentityRole<long>
    {
    }

    public class RoleClaim : IdentityRoleClaim<long> { }
    public class UserClaim : IdentityUserClaim<long> { }
}
