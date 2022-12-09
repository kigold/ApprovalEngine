using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace SampleApp.Core.Data.Entities
{
    public class User : IdentityUser<long>
    {
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        [NotMapped]
        public string FullName => $"{Firstname} {Lastname}";
    }

    public class Role : IdentityRole<long>
    {
    }

    public class RoleClaim : IdentityRoleClaim<long> { }
    public class UserClaim : IdentityUserClaim<long> { }
    public class UserRole : IdentityUserRole<long> { }
}
