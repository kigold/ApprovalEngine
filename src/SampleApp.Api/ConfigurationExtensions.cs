using ApprovalEngine;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using OpenIddict.Validation.AspNetCore;
using SampleApp.Core.Data;
using SampleApp.Core.Data.Entities;
using SampleApp.Core.Data.Entities.ApprovalEngine;
using SampleApp.Core.Data.Repositories;
using SampleApp.Core.Services;
using static AspNet.Security.OpenIdConnect.Primitives.OpenIdConnectConstants;

namespace SampleApp.Api
{
    internal static class ConfigurationExtensions
    {
        public static void AddServices(this IServiceCollection services)
        {
            services.AddScoped<IStudentService, StudentService>();
            services.AddTransient<IRepository<Student>, Repository<Student>>();
            services.AddTransient<IRepository<ApprovalRequest>, Repository<ApprovalRequest>>();
            services.AddTransient<IRepository<ApprovalHistory>, Repository<ApprovalHistory>>();
            services.AddTransient<IRepository<ApprovalStage>, Repository<ApprovalStage>>();
            services.AddScoped<IStudentService, StudentService>();
            services.AddScoped<IApprovalService, ApprovalService>();
        }

        public static void DatabaseSetup(this WebApplication app)
        {
            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;

                var context = services.GetRequiredService<SampleDbContext>();
                context.Database.Migrate();
            }
        }

        public static void AddEFDbContext(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("Default");
            services.AddDbContext<SampleDbContext>(options =>
            {
                options.UseSqlServer(connectionString, b => b.MigrationsAssembly(typeof(SampleDbContext).FullName));
            });
        }

        public static void AddAppSwagger(this IServiceCollection services)
        {
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Sample API",
                    Description = "Sample API",
                    Version = "v1"
                });

                c.AddSecurityDefinition("BearerAuth", new OpenApiSecurityScheme()
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT",
                    Description = "JWT Authorization header using the Bearer scheme."
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "BearerAuth"
                        }
                    },
                    Array.Empty<string>()
                }
            });

                //var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                //c.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
            });
        }

        public static void UseAppSwagger(this WebApplication app)
        {
            app.UseSwagger();
            app.UseSwaggerUI(c => { c.SwaggerEndpoint("/swagger/v1/swagger.json", "SampleApp V1"); });
        }

        public static void MapAppControllers(this WebApplication app)
        {
            app.MapControllers().RequireAuthorization(new AuthorizationOptions { AuthenticationSchemes = OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme });
            app.MapGet("/", () => Results.Redirect("/swagger")).ExcludeFromDescription();
            app.MapDefaultControllerRoute();
        }

        public static void ConfigureAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                options.Authority = options.Authority = "http://localhost:5287/";
                options.RequireHttpsMetadata = false;

                options.TokenValidationParameters = new TokenValidationParameters
                {
                    NameClaimType = Claims.Name,
                    RoleClaimType = Claims.Role,
                    //IssuerSigningKey = signingKey,
                    //ValidateAudience = authSettings.ValidateAudience,
                    //ValidateIssuer = authSettings.ValidateIssuer,
                };

                options.Events = new JwtBearerEvents
                {
                    OnAuthenticationFailed = context =>
                    {
                        if (context.Exception.GetType() == typeof(SecurityTokenExpiredException))
                        {
                            context.Response.Headers.Add("Token-Expired", "true");
                        }
                        return Task.CompletedTask;
                    }
                };
            });

            services.AddIdentity<User, Role>(options =>
            {
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequiredLength = 6;

                options.Lockout.AllowedForNewUsers = true;
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(10);
                options.Lockout.MaxFailedAccessAttempts = 3;
                options.User.RequireUniqueEmail = false;
            })
            .AddUserStore<UserStore<User, Role, SampleDbContext, long, IdentityUserClaim<long>, UserRole, IdentityUserLogin<long>, IdentityUserToken<long>, RoleClaim>>()
            .AddSignInManager()
            .AddUserManager<UserManager<User>>()
            .AddEntityFrameworkStores<SampleDbContext>()
            .AddDefaultTokenProviders();

            services.Configure<DataProtectionTokenProviderOptions>(options =>
            {
                options.TokenLifespan = TimeSpan.FromHours(24);
            });

            services.Configure<IdentityOptions>(options =>
            {
                options.ClaimsIdentity.UserNameClaimType = Claims.Name;
                options.ClaimsIdentity.UserIdClaimType = Claims.Subject;
                options.ClaimsIdentity.RoleClaimType = Claims.Role;
                options.SignIn.RequireConfirmedAccount = false;
                // configure more options if necessary...
            });

            services.AddControllersWithViews(); 

            services.AddDbContext<SampleDbContext>(options =>
            {
                // Configure Entity Framework Core to use Microsoft SQL Server.
                options.UseSqlServer(configuration.GetConnectionString("Default"));

                // Register the entity sets needed by OpenIddict.
                // Note: use the generic overload if you need to replace the default OpenIddict entities.
                options.UseOpenIddict<long>();
            });

            services.AddOpenIddict()
                // Register the OpenIddict core components.
                .AddCore(options =>
                {
                // Configure OpenIddict to use the Entity Framework Core stores and models.
                // Note: call ReplaceDefaultEntities() to replace the default entities.
                    options.UseEntityFrameworkCore()
                           .UseDbContext<SampleDbContext>()
                           .ReplaceDefaultEntities<long>();
                })

                // Register the OpenIddict server components.
                .AddServer(options =>
                {
                    options.RegisterScopes(Scopes.Email,
                        Scopes.Profile,
                        Scopes.Address,
                        Scopes.Phone,
                        Scopes.OfflineAccess,
                        Scopes.OpenId
                    );

                    // Enable the token endpoint.
                    options.SetTokenEndpointUris("/connect/token")
                           .SetAuthorizationEndpointUris("/connect/authorize")
                           .SetUserinfoEndpointUris("/connect/userinfo");

                    // Enable the client credentials flow.
                    //options.AllowClientCredentialsFlow();
                    //options.AllowAuthorizationCodeFlow().RequireProofKeyForCodeExchange();

                    //options.AllowImplicitFlow();
                    options.AllowPasswordFlow();
                    options.AcceptAnonymousClients();

                    // Register the signing and encryption credentials.
                    options.AddDevelopmentEncryptionCertificate()
                           .AddDevelopmentSigningCertificate()
                           .DisableAccessTokenEncryption();
                           //.DisableRollingRefreshTokens()
                           //.DisableSlidingRefreshTokenExpiration();

                    // Register scopes (permissions)
                    options.RegisterScopes(Scopes.Email, Scopes.Profile, Scopes.OfflineAccess, Scopes.OpenId, Scopes.Phone, "api");

                    // Register the ASP.NET Core host and configure the ASP.NET Core options.
                    options.UseAspNetCore()
                           .EnableTokenEndpointPassthrough()
                           .EnableAuthorizationEndpointPassthrough()
                           .DisableTransportSecurityRequirement();
                })

                    // Register the OpenIddict validation components.
                .AddValidation(options =>
                {
                    // Import the configuration from the local OpenIddict server instance.
                    options.UseLocalServer();

                    // Register the ASP.NET Core host.
                    options.UseAspNetCore();
                });

                    // Register the worker responsible of seeding the database with the sample clients.
                    // Note: in a real world application, this step should be part of a setup script.
                    services.AddHostedService<Worker>();
        }
    }

    public class AuthorizationOptions : Microsoft.AspNetCore.Authorization.IAuthorizeData
    {
        public string? Policy { get; set; }
        public string? Roles {  get; set; }
        public string? AuthenticationSchemes { get; set; }
    }
}
