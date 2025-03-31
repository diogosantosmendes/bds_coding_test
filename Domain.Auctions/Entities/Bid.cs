using Domain.Shared.Abstractions;

namespace Domain.Auctions.Entities
{
    public class Bid : IBaseEntity
    {
        public required string Id { get; set; }

        // I will not add the User domain as it is out od the this coding test scope
        public required string UserId { get; set; }

        public required string AuctionId { get; set; }

        public required float Value { get; set; }
    }
}
