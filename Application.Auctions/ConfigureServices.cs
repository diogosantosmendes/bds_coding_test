using Application.Auctions.Abstractions.services;
using Application.Auctions.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Application.Auctions
{
    public static class ConfigureServices
    {
        public static IServiceCollection AddAuctionsServices(this IServiceCollection services)
        {
            services.AddScoped<IAuctionsService, AuctionsService>();
            return services;
        }
    }
}
