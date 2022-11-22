using ApprovalEngine.Enums;
using Microsoft.AspNetCore.Identity;
using OpenIddict.Abstractions;
using SampleApp.Core.Data;
using SampleApp.Core.Data.Entities;
using System.Security.Claims;
using System.Xml.Linq;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace SampleApp.Api
{
    public class Worker : IHostedService
    {
        private readonly IServiceProvider _serviceProvider;

        public Worker(IServiceProvider serviceProvider)
            => _serviceProvider = serviceProvider;

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            using var scope = _serviceProvider.CreateScope();

            var context = scope.ServiceProvider.GetRequiredService<SampleDbContext>();
            await context.Database.EnsureCreatedAsync();

            await SeedClients();
            await SeedUsers();
            await SeedRoles();
        }

        public async Task SeedClients()
        {
            var clients = new List<OpenIddictApplicationDescriptor>
            {
                new OpenIddictApplicationDescriptor
                {
                    ClientId = "console2",
                    ClientSecret = "secret",
                    DisplayName = "My client application",
                    Permissions =
                    {
                        Permissions.Endpoints.Token,
                        Permissions.GrantTypes.ClientCredentials,
                        OpenIddictConstants.Permissions.Prefixes.Scope + "api"
                    }
                },
                new OpenIddictApplicationDescriptor
                {
                    ClientId = "console3",
                    ClientSecret = "secret",
                    DisplayName = "My client application",
                    RedirectUris = { new Uri("https://oauth.pstmn.io/v1/callback") },
                    Permissions =
                    {
                        Permissions.Endpoints.Authorization,
                        Permissions.Endpoints.Token,

                        Permissions.GrantTypes.AuthorizationCode,
                        Permissions.GrantTypes.ClientCredentials,

                        Permissions.Prefixes.Scope + "api",

                        Permissions.ResponseTypes.Code
                    }
                },
                new OpenIddictApplicationDescriptor
                {
                    ClientId = "console4",
                    ClientSecret = "secret",
                    DisplayName = "My client application",
                    RedirectUris = { new Uri("https://oauth.pstmn.io/v1/callback") },
                    Permissions =
                    {
                        Permissions.Endpoints.Authorization,
                        Permissions.Endpoints.Token,

                        Permissions.GrantTypes.AuthorizationCode,
                        Permissions.GrantTypes.ClientCredentials,
                        Permissions.GrantTypes.Password,

                        Permissions.Prefixes.Scope + "api",

                        Permissions.ResponseTypes.Code
                    }
                }
            };
            using var scope = _serviceProvider.CreateScope();
            var manager = scope.ServiceProvider.GetRequiredService<IOpenIddictApplicationManager>();
            foreach (var client in clients)
            {
                if (await manager.FindByClientIdAsync(client.ClientId) is null)
                {
                    await manager.CreateAsync(client);
                }
            }
        }

        public async Task SeedUsers()
        {
            var users = new List<User>
            {
                new User { Firstname = "Admin", Lastname = "Boss", Email = "admin@sample.com", UserName = "admin@sample.com" },
                new User { Firstname = "Tester", Lastname = "Bae", Email = "tester@sample.com", UserName = "tester@sample.com" },
                new User { Firstname = "IT", Lastname = "Guy", Email = "it@sample.com", UserName = "it@sample.com" },
                new User { Firstname = "HOD", Lastname = "Oga", Email = "hod@sample.com", UserName = "hod@sample.com" },
            };
            using var scope = _serviceProvider.CreateScope();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
            foreach (var user in users)
            {
                if (await userManager.FindByNameAsync(user.UserName) is null)
                {
                    var hash = userManager.PasswordHasher.HashPassword(user, "P@ssw0rd");
                    user.PasswordHash = hash;
                    userManager.CreateAsync(user).GetAwaiter().GetResult();
                }
            }
        }

        public async Task SeedRoles()
        {
            var approver = new Role { Name = "Approver" };
            var creator = new Role { Name = "Creator" };
            var admin = new Role { Name = "Admin" };
            var hod = new Role { Name = "HOD" };
            var it = new Role { Name = "IT" };
            using var scope = _serviceProvider.CreateScope();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<Role>>();

            void AddRole(ref Role role)
            {
                var roleData = roleManager.FindByNameAsync(role.Name).GetAwaiter().GetResult();
                if (roleData is not null)
                {
                    role = roleData;
                    return;
                }
                roleManager.CreateAsync(role).GetAwaiter().GetResult();
            }

            AddRole(ref approver);
            AddRole(ref creator);
            AddRole(ref admin);
            AddRole(ref hod);
            AddRole(ref it);

            await roleManager.AddClaimAsync(approver, new Claim(nameof(Permission), Permission.Approver.ToString()));
            await roleManager.AddClaimAsync(admin, new Claim(nameof(Permission), Permission.Admin.ToString()));
            await roleManager.AddClaimAsync(hod, new Claim(nameof(Permission), Permission.HOD.ToString()));
            await roleManager.AddClaimAsync(it, new Claim(nameof(Permission), Permission.IT.ToString()));

            var user = await userManager.FindByNameAsync("admin@sample.com");
            await userManager.AddToRolesAsync(user, new[] { approver.Name, admin.Name } );
            user = await userManager.FindByNameAsync("hod@sample.com");
            await userManager.AddToRolesAsync(user, new[] { approver.Name, hod.Name });
            user = await userManager.FindByNameAsync("it@sample.com");
            await userManager.AddToRolesAsync(user, new[] { approver.Name, it.Name });
            user = await userManager.FindByNameAsync("tester@sample.com");
            await userManager.AddToRolesAsync(user, new[] { creator.Name });
        }

        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }
}