using Application.Shared.Types;
using Domain.Auctions.Entities;

namespace Application.Auctions.Abstractions.services
{
    public interface IAuctionsService
    {
        Task<DataResult<Auction>> StartAsync(string vehicleId);
        Task<Result> BidAsync(string userId, string auctionId, float value);
        Task<Result> CloseAsync(string auctionId);
    }
}
