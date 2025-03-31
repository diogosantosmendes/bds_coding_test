namespace Application.Auctions.Types
{
    public static class AuctionsErrorCodes
    {
        public const string VehicleAlreadyInAuction = "AUC_001";
        public const string AuctionNotFound = "AUC_002";
        public const string BidValueMustBeHigher = "AUC_003";
        public const string AuctionAlreadyClosed = "AUC_004";
        public const string VehicleNotFound = "AUC_005";
    }
}
