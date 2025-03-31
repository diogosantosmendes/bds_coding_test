using Application.Inventory.Abstractions.Services;
using Application.Inventory.Services;
using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Application.Inventory
{
    public static class ConfigureServices
    {
        public static IServiceCollection AddInventoryServices(this IServiceCollection services)
        {
            // loading mapping profiles here to avoid dependecies propagation
            var type = typeof(Profile);
            services.AddAutoMapper(Assembly.GetExecutingAssembly().GetTypes().Where(x => x.IsSubclassOf(type)).ToArray());

            services.AddScoped<IVehiclesService, VehiclesService>();
            return services;
        }
    }
}
