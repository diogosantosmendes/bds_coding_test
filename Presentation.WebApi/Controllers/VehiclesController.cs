using Application.Shared.Types;
using Application.Inventory.Abstractions.Services;
using Domain.Inventory.Abstractions;
using Microsoft.AspNetCore.Mvc;
using Application.Inventory.DTOs;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class VehiclesController : ControllerBase
    {

        private readonly ILogger<VehiclesController> _logger;
        private readonly IVehiclesService _vehiclesService;

        public VehiclesController(ILogger<VehiclesController> logger, IVehiclesService vehiclesService)
        {
            _logger = logger;
            _vehiclesService = vehiclesService;
        }

        [HttpGet(Name = "Search")]
        public Task<PageResult<IVehicle>> Search([FromQuery] Page page, [FromQuery] Filter? filter, [FromQuery] Sort? sort)
        {
            return _vehiclesService.Search(page, filter, sort);
        }

        [HttpPost]
        public Task<Result> Add([FromBody] AddVehicleDto vehicleDto)
        {
            return _vehiclesService.AddAsync(vehicleDto);
        }
    }
}
