using Microsoft.AspNetCore.Identity;
using OpenIddict.Abstractions;
using SampleApp.Core.Data;
using SampleApp.Core.Data.Entities;
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
            };
            using var scope = _serviceProvider.CreateScope();
            foreach (var user in users)
            {
                var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
                if (userManager.FindByNameAsync(user.UserName).GetAwaiter().GetResult() is null)
                {
                    var hash = userManager.PasswordHasher.HashPassword(user, "P@ssw0rd");
                    user.PasswordHash = hash;
                    userManager.CreateAsync(user).GetAwaiter().GetResult();
                }
            }
        }


        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }
}