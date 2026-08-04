using AuctionsSystem.AccountService.Application.Behaviors;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace AuctionsSystem.AccountService.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            var assembly = typeof(DependencyInjection).Assembly;


            services.AddValidatorsFromAssembly(assembly);

            services.AddMediatR(cfg =>
            {
                cfg.RegisterServicesFromAssembly(assembly);

                cfg.AddOpenBehavior(typeof(LoggingBehavior<,>));

                cfg.AddOpenBehavior(typeof(ValidationBehavior<,>));
                
            });


            return services;
        }
    }
}
