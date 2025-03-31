using Application.Auctions.Abstractions.Repositories;
using Application.Auctions.Services;
using Application.Auctions.Types;
using Application.Inventory.Abstractions.Repositories;
using Domain.Auctions.Entities;
using Domain.Auctions.Enums;
using Domain.Inventory.Abstractions;
using Domain.Inventory.Entities;
using Microsoft.Extensions.Logging;
using Moq;

namespace Tests.Unit.Application.Auctions
{
    [TestFixture]
    public class TestAuctionsService
    {
        private Mock<IAuctionsRepository> _mockAuctionsRepository;
        private Mock<IBidsRepository> _mockBidsRepository;
        private Mock<IVehiclesRepository> _mockVehiclesRepository;
        private Mock<ILogger<AuctionsService>> _mockLogger;
        private AuctionsService _auctionsService;

        [SetUp]
        public void Setup()
        {
            _mockAuctionsRepository = new Mock<IAuctionsRepository>();
            _mockBidsRepository = new Mock<IBidsRepository>();
            _mockVehiclesRepository = new Mock<IVehiclesRepository>();
            _mockLogger = new Mock<ILogger<AuctionsService>>();
            _auctionsService = new AuctionsService(_mockLogger.Object, _mockAuctionsRepository.Object, _mockBidsRepository.Object, _mockVehiclesRepository.Object);
        }

        [Test]
        public async Task StartAsync_FailWhen_VehicleAlreadyInAuction()
        {
            // Arrange
            var vehicleId = "1";
            var existingAuction = new Auction { Id = "1", VehicleId = vehicleId, State = AuctionState.STARTED };
            _mockAuctionsRepository.Setup(repo => repo.GetFirstAsync(null, It.IsAny<Func<IQueryable<Auction>, IQueryable<Auction>>>()))
                                   .ReturnsAsync(existingAuction);

            // Act
            var result = await _auctionsService.StartAsync(vehicleId);

            // Assert
            Assert.IsFalse(result.Ok);
            Assert.AreEqual(AuctionsErrorCodes.VehicleAlreadyInAuction, result.ErrorCode);
        }

        [Test]
        public async Task StartAsync_FailWhen_VehicleDoesntExist()
        {
            // Arrange
            var vehicleId = "1";
            var existingAuction = new Auction { Id = "1", VehicleId = vehicleId, State = AuctionState.STARTED };
            _mockAuctionsRepository.Setup(repo => repo.GetFirstAsync(null, It.IsAny<Func<IQueryable<Auction>, IQueryable<Auction>>>()))
                                   .ReturnsAsync((Auction)null);
            _mockVehiclesRepository.Setup(repo => repo.GetByIdAsync(vehicleId))
                                   .ReturnsAsync((IVehicle)null);

            // Act
            var result = await _auctionsService.StartAsync(vehicleId);

            // Assert
            Assert.IsFalse(result.Ok);
            Assert.AreEqual(AuctionsErrorCodes.VehicleNotFound, result.ErrorCode);
        }

        [Test]
        public async Task StartAsync_SuccessWhen_VehicleExistsAndIsNotInAuction()
        {
            // Arrange
            var vehicleId = "2";
            _mockAuctionsRepository.Setup(repo => repo.GetFirstAsync(null, It.IsAny<Func<IQueryable<Auction>, IQueryable<Auction>>>()))
                                   .ReturnsAsync((Auction)null);
            _mockVehiclesRepository.Setup(repo => repo.GetByIdAsync(vehicleId))
                                   .ReturnsAsync(new Suv { Id = vehicleId, ManufacturerId = "1", ModelId = "1", Year = 2024, StartingBid = 10000, Seats = 5 });

            // Act
            var result = await _auctionsService.StartAsync(vehicleId);

            // Assert
            Assert.IsTrue(result.Ok);
            Assert.IsNotNull(result.Data);
            Assert.AreEqual(vehicleId, result.Data.VehicleId);
            Assert.AreEqual(AuctionState.STARTED, result.Data.State);
        }

        [Test]
        public async Task BidAsync_FailWhen_AuctionNotFound()
        {
            // Arrange
            var userId = "user1";
            var auctionId = "1";
            _mockAuctionsRepository.Setup(repo => repo.GetByIdAsync(auctionId))
                                   .ReturnsAsync((Auction)null);

            // Act
            var result = await _auctionsService.BidAsync(userId, auctionId, 1000);

            // Assert
            Assert.IsFalse(result.Ok);
            Assert.AreEqual(AuctionsErrorCodes.AuctionNotFound, result.ErrorCode);
        }

        [Test]
        public async Task BidAsync_FailWhen_AuctionIsClosed()
        {
            // Arrange
            var userId = "user1";
            var auctionId = "1";
            var auction = new Auction
            {
                Id = "1",
                VehicleId = "1",
                State = AuctionState.CLOSED,
                HighestBidValue = 1500
            };
            _mockAuctionsRepository.Setup(repo => repo.GetByIdAsync(auctionId))
                                   .ReturnsAsync(auction);

            // Act
            var result = await _auctionsService.BidAsync(userId, auctionId, 1000);

            // Assert
            Assert.IsFalse(result.Ok);
            Assert.AreEqual(AuctionsErrorCodes.AuctionAlreadyClosed, result.ErrorCode);
        }

        [Test]
        public async Task BidAsync_FailWhen_BidValueIsLower()
        {
            // Arrange
            var userId = "user1";
            var auctionId = "1";
            var auction = new Auction
            {
                Id = "1",
                VehicleId = "1",
                State = AuctionState.STARTED,
                HighestBidValue = 1500
            };
            _mockAuctionsRepository.Setup(repo => repo.GetByIdAsync(auctionId))
                                   .ReturnsAsync(auction);

            // Act
            var result = await _auctionsService.BidAsync(userId, auctionId, 1000);

            // Assert
            Assert.IsFalse(result.Ok);
            Assert.AreEqual(AuctionsErrorCodes.BidValueMustBeHigher, result.ErrorCode);
        }

        [Test]
        public async Task BidAsync_SuccessWhen_BidIsValid()
        {
            // Arrange
            var userId = "user1";
            var auctionId = "1";
            var auction = new Auction
            {
                Id = "1",
                VehicleId = "1",
                State = AuctionState.STARTED,
                HighestBidValue = 1500
            };
            _mockAuctionsRepository.Setup(repo => repo.GetByIdAsync(auctionId))
                                   .ReturnsAsync(auction);

            // Act
            var result = await _auctionsService.BidAsync(userId, auctionId, 2000);

            // Assert
            Assert.IsTrue(result.Ok);
        }

        [Test]
        public async Task CloseAsync_FailWhen_AuctionNotFound()
        {
            // Arrange
            var auctionId = "1";
            _mockAuctionsRepository.Setup(repo => repo.GetByIdAsync(auctionId))
                                   .ReturnsAsync((Auction)null);

            // Act
            var result = await _auctionsService.CloseAsync(auctionId);

            // Assert
            Assert.IsFalse(result.Ok);
            Assert.AreEqual(AuctionsErrorCodes.AuctionNotFound, result.ErrorCode);
        }

        [Test]
        public async Task CloseAsync_FailWhen_AuctionIsAlreadyClosed()
        {
            // Arrange
            var auctionId = "1";
            var auction = new Auction
            {
                Id = auctionId,
                VehicleId = "1",
                State = AuctionState.CLOSED,
                HighestBidValue = 1500
            };
            _mockAuctionsRepository.Setup(repo => repo.GetByIdAsync(auctionId))
                                   .ReturnsAsync(auction);

            // Act
            var result = await _auctionsService.CloseAsync(auctionId);

            // Assert
            Assert.IsFalse(result.Ok);
            Assert.AreEqual(AuctionsErrorCodes.AuctionAlreadyClosed, result.ErrorCode);
        }

        [Test]
        public async Task CloseAsync_SuccessWhen_AuctionIsOpen()
        {
            // Arrange
            var auctionId = "1";
            var auction = new Auction
            {
                Id = auctionId,
                VehicleId = "1",
                State = AuctionState.STARTED,
                HighestBidValue = 1500
            };
            _mockAuctionsRepository.Setup(repo => repo.GetByIdAsync(auctionId))
                                   .ReturnsAsync(auction);

            // Act
            var result = await _auctionsService.CloseAsync(auctionId);

            // Assert
            Assert.IsTrue(result.Ok);
        }
    }
}
