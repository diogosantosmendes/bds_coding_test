using Application.Auctions.Abstractions.Repositories;
using Infrastructure.Auctions.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Auctions
{
    public static class ConfigureInfrastructure
    {
        public static IServiceCollection AddAuctionsInfrastructure(this IServiceCollection services)
        {
            services.AddScoped<IAuctionsRepository, AuctionsRepository>();
            services.AddScoped<IBidsRepository, BidsRepository>();
            return services;
        }
    }
}
