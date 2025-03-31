using Application.Inventory.Abstractions.Repositories;
using Application.Inventory.DTOs;
using Application.Inventory.mappingProfiles;
using Application.Inventory.Services;
using Application.Inventory.Types;
using Application.Shared.Types;
using AutoMapper;
using Domain.Inventory.Abstractions;
using Domain.Inventory.Entities;
using Domain.Inventory.Enums;
using Microsoft.Extensions.Logging;
using Moq;

namespace Tests.Unit.Application.Inventory
{
    [TestFixture]
    public class TestVehiclesService
    {
        private Mock<IVehiclesRepository> _mockRepository;
        private Mock<ILogger<VehiclesService>> _mockLogger;
        private IMapper _mapper;
        private VehiclesService _vehiclesService;

        [SetUp]
        public void Setup()
        {
            _mockRepository = new Mock<IVehiclesRepository>();
            _mockLogger = new Mock<ILogger<VehiclesService>>();
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<VehicleProfile>();
            });
            _mapper = config.CreateMapper();
            _vehiclesService = new VehiclesService(_mockLogger.Object, _mapper, _mockRepository.Object);
        }

        [Test]
        public async Task AddAsync_FailWhen_VehicleAlreadyExists()
        {
            // Arrange
            var vehicleDto = new AddVehicleDto { 
                Id = "1", 
                Type = VehicleType.SUV,
                ManufacturerId = "1",
                ModelId = "1",
                Year = 2021,
                StartingBid = 10000
            };
            _mockRepository.Setup(repo => repo.AnyAsync(null, It.IsAny<Func<IQueryable<IVehicle>, IQueryable<IVehicle>>>()))
                           .ReturnsAsync(true);

            // Act
            var result = await _vehiclesService.AddAsync(vehicleDto);

            // Assert
            Assert.IsFalse(result.Ok);
            Assert.AreEqual(InventoryErrorCodes.VehicleAlreadyExists, result.ErrorCode);
        }

        [Test]
        public async Task AddAsync_FailWhen_VehicleTypeIsInvalid()
        {
            // Arrange
            var vehicleDto = new AddVehicleDto
            {
                Id = "2",
                Type = (VehicleType)8,
                ManufacturerId = "1",
                ModelId = "1",
                Year = 2021,
                StartingBid = 10000,
            };
            _mockRepository.Setup(repo => repo.AnyAsync(null, It.IsAny<Func<IQueryable<IVehicle>, IQueryable<IVehicle>>>()))
                           .ReturnsAsync(false);

            // Act
            var result = await _vehiclesService.AddAsync(vehicleDto);

            // Assert
            Assert.IsFalse(result.Ok);
            Assert.AreEqual(InventoryErrorCodes.InvalidVehicleType, result.ErrorCode);
        }

        public async Task AddAsync_FailWhen_MissingVehicleParametersByType()
        {
            // Arrange
            var vehicleDto = new AddVehicleDto
            {
                Id = "3",
                Type = VehicleType.SUV,
                ManufacturerId = "1",
                ModelId = "1",
                Year = 2021,
                StartingBid = 10000
            };
            _mockRepository.Setup(repo => repo.AnyAsync(null, It.IsAny<Func<IQueryable<IVehicle>, IQueryable<IVehicle>>>()))
                           .ReturnsAsync(false);

            // Act
            var result = await _vehiclesService.AddAsync(vehicleDto);

            // Assert
            Assert.IsFalse(result.Ok);
            Assert.AreEqual(InventoryErrorCodes.MissingVehicleParameters, result.ErrorCode);
        }

        [Test]
        public async Task AddAsync_SuccessWhen_VehicleIsNew()
        {
            // Arrange
            var vehicleDto = new AddVehicleDto
            {
                Id = "3",
                Type = VehicleType.SUV,
                ManufacturerId = "1",
                ModelId = "1",
                Year = 2021,
                StartingBid = 10000,
                Seats = 5
            };
            _mockRepository.Setup(repo => repo.AnyAsync(null, It.IsAny<Func<IQueryable<IVehicle>, IQueryable<IVehicle>>>()))
                           .ReturnsAsync(false);

            // Act
            var result = await _vehiclesService.AddAsync(vehicleDto);

            // Assert
            Assert.IsTrue(result.Ok);
        }

        [Test]
        public async Task Search_SuccessWhen_FiltersAreValid()
        {
            // Arrange
            var page = new Page { Index = 1, Size = 10 };
            var filters = new Filter { Field = "ManufacturerId", Value = "1", Comparator = Filter.FilterComparisionOperator.EQUAL };
            var sort = new Sort { By = "Year", Desc = true };
            var vehicles = new List<IVehicle> { new Suv { Id = "1", ManufacturerId = "1", ModelId = "1", Year = 2021, StartingBid = 10000, Seats= 5 } };
            _mockRepository.Setup(repo => repo.GetListAsync(page, filters, sort, null)).ReturnsAsync(vehicles);

            // Act
            var result = await _vehiclesService.Search(page, filters, sort);

            // Assert
            Assert.IsTrue(result.Ok);
            Assert.AreEqual(vehicles, result.Data);
        }

        [Test]
        public async Task Search_FailWhen_FiltersAreInvalid()
        {
            // Arrange
            var page = new Page { Index = 1, Size = 10 };
            var filters = new Filter { Field = "InvalidField", Value = "1", Comparator = Filter.FilterComparisionOperator.EQUAL };
            var sort = new Sort { By = "Year", Desc = true };

            // Act
            var result = await _vehiclesService.Search(page, filters, sort);

            // Assert
            Assert.IsFalse(result.Ok);
            Assert.AreEqual(InventoryErrorCodes.InvalidFilterField, result.ErrorCode);
        }
    }
}
