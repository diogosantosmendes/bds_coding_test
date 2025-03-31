using Application.Shared.Abstractions;
using Domain.Inventory.Abstractions;

namespace Application.Inventory.Abstractions.Repositories
{
    public interface IVehiclesRepository: IGenericRepository<IVehicle>
    {
    }
}
