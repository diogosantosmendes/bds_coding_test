using Application.Inventory.Abstractions.Repositories;
using Application.Inventory.Abstractions.Services;
using Application.Inventory.DTOs;
using Application.Inventory.Types;
using Application.Shared.Extensions;
using Application.Shared.Types;
using AutoMapper;
using Domain.Inventory.Abstractions;
using Domain.Inventory.Entities;
using Domain.Inventory.Enums;
using Microsoft.Extensions.Logging;

namespace Application.Inventory.Services
{
    public class VehiclesService : IVehiclesService
    {
        private readonly IVehiclesRepository vehicleRepository;
        private readonly ILogger<VehiclesService> logger;
        private readonly IMapper mapper;

        public VehiclesService(ILogger<VehiclesService> logger, IMapper mapper, IVehiclesRepository vehicleRepository)
        {
            this.logger = logger;
            this.mapper = mapper;
            this.vehicleRepository = vehicleRepository;
        }

        public async Task<Result> AddAsync(AddVehicleDto vehicleDto)
        {
            var existing = await this.vehicleRepository.AnyAsync(customQuery: q => q.Where(vehicle => vehicle.Id.Equals(vehicleDto.Id)));

            if (existing)
            {
                this.logger.LogError("{ErrorCode}: Vehicle {VehicleId} already exists.", InventoryErrorCodes.VehicleAlreadyExists, vehicleDto.Id);
                return Result.Fail(InventoryErrorCodes.VehicleAlreadyExists);
            }

            try
            {
                IVehicle vehicle;
                switch (vehicleDto.Type)
                {
                    case VehicleType.HATCHBACK:
                        vehicle = mapper.Map<Hatchback>(vehicleDto);
                        break;
                    case VehicleType.SUDAN:
                        vehicle = mapper.Map<Sudan>(vehicleDto);
                        break;
                    case VehicleType.SUV:
                        vehicle = mapper.Map<Suv>(vehicleDto);
                        break;
                    case VehicleType.TRUCK:
                        vehicle = mapper.Map<Truck>(vehicleDto);
                        break;
                    default:
                        this.logger.LogError("{ErrorCode}: Invalid Vehicle type {VehicleType}.", InventoryErrorCodes.InvalidVehicleType, vehicleDto.Type);
                        return Result.Fail(InventoryErrorCodes.InvalidVehicleType);
                }

                await this.vehicleRepository.CreateAsync(vehicle);

                return Result.Success();
            }
            catch (ArgumentNullException e)
            {
                this.logger.LogError("{ErrorCode}: {Message}.", InventoryErrorCodes.MissingVehicleParameters, e.Message);
                return Result.Fail(InventoryErrorCodes.MissingVehicleParameters);
            }
        }

        public async Task<PageResult<IVehicle>> Search(Page page, Filter? filters, Sort? sort)
        {
            try
            {
                filters.CheckAllowedFields(new[] { "ManufacturerId", "ModelId", "Year", "Type" });

                var data = await this.vehicleRepository.GetListAsync(page, filters, sort);

                return Result.Success(data, page);
            }
            catch (ArgumentException e)
            {
                this.logger.LogError("{ErrorCode}: {Message}.", InventoryErrorCodes.InvalidFilterField, e.Message);
                return PageResult<IVehicle>.Fail(InventoryErrorCodes.InvalidFilterField);
            }
        }
    }
}
