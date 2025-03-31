using Application.Inventory.Abstractions.Repositories;
using Infrastructure.Inventory.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Inventory
{
    public static class ConfigureInfrastructure
    {
        public static IServiceCollection AddInventoryInfrastructure(this IServiceCollection services)
        {
            services.AddScoped<IVehiclesRepository, VehiclesRepository>();
            return services;
        }
    }
}
