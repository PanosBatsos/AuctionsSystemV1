using AuctionsSystem.AccountService.Application.Abstractions.Persistence;
using AuctionsSystem.AccountService.Application.Abstractions.Security;
using AuctionsSystem.AccountService.Infrastructure.Persistence;
using AuctionsSystem.AccountService.Infrastructure.Persistence.Repositories;
using AuctionsSystem.AccountService.Infrastructure.Security;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace AuctionsSystem.AccountService.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<AccountDbContext>(options =>
                options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

            services.AddScoped<IAccountRepository, AccountRepository>();
            services.AddSingleton<IPasswordHasher, PasswordHasher>();

            return services;
        }
    }
}
