using Application.Auctions.Abstractions.services;
using Application.Inventory.Abstractions.Services;
using Domain.Auctions.Entities;
using Domain.Auctions.Enums;
using Domain.Inventory.Abstractions;
using Domain.Inventory.Entities;
using Infrastructure.Shared;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Moq;

public class CustomWebApplicationFactory<TStartup> : WebApplicationFactory<TStartup> where TStartup : class
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            var vehicles = new List<IVehicle>()
                    {
                        new Suv { Id = "1", ManufacturerId = "1", ModelId = "1", Year = 2021, StartingBid = 10000, Seats = 5 },
                        new Sudan { Id = "2", ManufacturerId = "2", ModelId = "2", Year = 2021, StartingBid = 10000, Doors = 4 },
                        new Hatchback { Id = "3", ManufacturerId = "3", ModelId = "3", Year = 2021, StartingBid = 10000, Doors = 4 },
                        new Truck { Id = "4", ManufacturerId = "4", ModelId = "4", Year = 2021, StartingBid = 10000, LoadCapacity = 1000 },
                    };

            var auctions = new List<Auction>()
                    {
                        new Auction { Id = "1", VehicleId = "1", HighestBidValue = 999, State = AuctionState.STARTED },
                    };

            var sets = new Dictionary<Type, object>();
            sets.Add(typeof(IVehicle), vehicles);
            sets.Add(typeof(Auction), auctions);

            // Seed data
            services.AddInMemoryDb(sets);
        });

    }
}
