using Application.Shared.Types;
using Application.Inventory.DTOs;
using Domain.Inventory.Abstractions;

namespace Application.Inventory.Abstractions.Services
{
    public interface IVehiclesService
    {
        Task<Result> AddAsync(AddVehicleDto vehicleDto);
        Task<PageResult<IVehicle>> Search(Page page, Filter? filters, Sort? sort);

    }
}
