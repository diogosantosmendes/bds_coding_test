using Application.Auctions.Abstractions.Repositories;
using Application.Auctions.Abstractions.services;
using Application.Auctions.Types;
using Application.Inventory.Abstractions.Repositories;
using Application.Shared.Types;
using Domain.Auctions.Entities;
using Domain.Auctions.Enums;
using Microsoft.Extensions.Logging;

namespace Application.Auctions.Services
{
    public class AuctionsService : IAuctionsService
    {
        private readonly IAuctionsRepository auctionsRepository;
        private readonly IVehiclesRepository vehiclesRepository;
        private readonly IBidsRepository bidsRepository;
        private readonly ILogger<AuctionsService> logger;

        public AuctionsService(ILogger<AuctionsService> logger, IAuctionsRepository auctionsRepository, IBidsRepository bidsRepository, IVehiclesRepository vehiclesRepository)
        {
            this.logger = logger;
            this.auctionsRepository = auctionsRepository;
            this.bidsRepository = bidsRepository;
            this.vehiclesRepository = vehiclesRepository;
        }

        public async Task<DataResult<Auction>> StartAsync(string vehicleId)
        {
            var activeAutcion = await this.auctionsRepository.GetFirstAsync(customQuery: q => q.Where(auction => auction.VehicleId.Equals(vehicleId) && auction.State == AuctionState.STARTED));

            if (activeAutcion != null)
            {
                this.logger.LogError("{ErrorCode}: Auction[{AuctionId}], Vehicle[{VehicleId}]", AuctionsErrorCodes.VehicleAlreadyInAuction, activeAutcion.Id, vehicleId);
                return DataResult<Auction>.Fail(AuctionsErrorCodes.VehicleAlreadyInAuction);
            }

            var vehicle = await this.vehiclesRepository.GetByIdAsync(vehicleId);

            if (vehicle == null)
            {
                this.logger.LogError("{ErrorCode}: Vehicle {VehicleId} not found", AuctionsErrorCodes.VehicleNotFound, vehicleId);
                return DataResult<Auction>.Fail(AuctionsErrorCodes.VehicleNotFound);
            }

            var newAuction = new Auction()
            {
                Id = Guid.NewGuid().ToString(),
                VehicleId = vehicleId,
                State = AuctionState.STARTED,
                HighestBidValue = vehicle.StartingBid - 1
            };

            await this.auctionsRepository.CreateAsync(newAuction);

            return Result.Success(newAuction);
        }

        public async Task<Result> BidAsync(string userId, string auctionId, float value)
        {
            var auction = await this.auctionsRepository.GetByIdAsync(auctionId);

            if (auction == null)
            {
                this.logger.LogError("{ErrorCode}: Auction {AuctionId} not found", AuctionsErrorCodes.AuctionNotFound, auctionId);
                return Result.Fail(AuctionsErrorCodes.AuctionNotFound);
            }

            if (auction.State == AuctionState.CLOSED)
            {
                this.logger.LogError("{ErrorCode}: Auction {AuctionId} closed.", AuctionsErrorCodes.AuctionAlreadyClosed, auctionId);
                return Result.Fail(AuctionsErrorCodes.AuctionAlreadyClosed);
            }

            if (value <= auction.HighestBidValue)
            {
                this.logger.LogError("{ErrorCode}: Bid {Value} must be higher than {HighestBidValue}", AuctionsErrorCodes.BidValueMustBeHigher, value, auction.HighestBidValue);
                return Result.Fail(AuctionsErrorCodes.BidValueMustBeHigher);
            }

            auction.HighestBidValue = value;
            await this.auctionsRepository.UpdateAsync(auction);

            await this.bidsRepository.CreateAsync(new Bid()
            {
                Id = Guid.NewGuid().ToString(),
                AuctionId = auctionId,
                UserId = userId,
                Value = value
            });

            return Result.Success();
        }

        public async Task<Result> CloseAsync(string auctionId)
        {
            var auction = await this.auctionsRepository.GetByIdAsync(auctionId);

            if (auction == null)
            {
                this.logger.LogError("{ErrorCode}: Auction {AuctionId} not found", AuctionsErrorCodes.AuctionNotFound, auctionId);
                return Result.Fail(AuctionsErrorCodes.AuctionNotFound);
            }

            if (auction.State == AuctionState.CLOSED)
            {
                this.logger.LogError("{ErrorCode}: Auction {AuctionId} already closed", AuctionsErrorCodes.AuctionAlreadyClosed, auctionId);
                return Result.Fail(AuctionsErrorCodes.AuctionAlreadyClosed);
            }

            auction.State = AuctionState.CLOSED;
            await this.auctionsRepository.UpdateAsync(auction);

            return Result.Success();
        }
    }
}
