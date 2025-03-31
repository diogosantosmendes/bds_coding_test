using Application.Inventory.Abstractions.Repositories;
using Domain.Inventory.Abstractions;
using Infrastructure.Shared;
using Infrastructure.Shared.Repositories;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Inventory.Repositories
{
    public class VehiclesRepository : GenericRepository<IVehicle> , IVehiclesRepository
    {

        public VehiclesRepository(IInMemoryDb db, ILogger<VehiclesRepository> logger) : base(db, logger)
        {
        }
    }
}
