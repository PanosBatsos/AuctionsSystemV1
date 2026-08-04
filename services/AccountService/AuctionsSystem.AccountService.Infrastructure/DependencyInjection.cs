using AuctionsSystem.AccountService.Application.Abstractions.Persistence;
using AuctionsSystem.AccountService.Application.Abstractions.Security;
using AuctionsSystem.AccountService.Infrastructure.Configuration;
using AuctionsSystem.AccountService.Infrastructure.Persistence;
using AuctionsSystem.AccountService.Infrastructure.Persistence.Repositories;
using AuctionsSystem.AccountService.Infrastructure.Security;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Security.Cryptography;

namespace AuctionsSystem.AccountService.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {

            services.Configure<JwtSettings>(configuration.GetSection("JwtSettings"));

            services.AddDbContext<AccountDbContext>(options =>
                options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

            services.AddScoped<IAccountRepository, AccountRepository>();
            services.AddSingleton<IPasswordHasher, PasswordHasher>();

            services.AddScoped<ITokenProvider, TokenProvider>();


            var jwtSettings = configuration.GetSection("JwtSettings").Get<JwtSettings>();

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                var rsa = RSA.Create();
                rsa.ImportFromPem(jwtSettings.PublicKey.Replace("\\n", "\n"));

                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtSettings.Issuer,
                    ValidAudience = jwtSettings.Audience,
                    IssuerSigningKey = new RsaSecurityKey(rsa)
                };
            });

            services.AddAuthorization();
            return services;
        }
    }
}
