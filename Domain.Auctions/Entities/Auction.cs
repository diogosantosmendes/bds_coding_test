using Domain.Auctions.Enums;
using Domain.Shared.Abstractions;

namespace Domain.Auctions.Entities
{
    public class Auction : IBaseEntity
    {
        public required string Id { get; set; }

        public AuctionState State { get; set; } = AuctionState.STARTED;

        public required string VehicleId { get; set; }


        /* 
         * Redundant value that avoid searching in a (probably) huge collection of bids for every auction access
         * 
         * I'm assuming that each auction may have a lot of bids and thats why I'm not adding the related bids 
         * directly on this class, avoiding huge documents
         * 
         * Ideally this value should be in a cache system to optimize the access, as probably during an auction this 
         * value should change a lot and the ACID transactions on persisten datastores may be unable to handle the 
         * high number of accesses and writes 
         */
        public float HighestBidValue { get; set; }
    }
}
