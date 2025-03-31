using Domain.Inventory.Abstractions;
using Domain.Inventory.Enums;

namespace Domain.Inventory.Entities
{
    public class Sudan : IVehicle
    {
        public required string Id { get; set; }

        public VehicleType Type { get; } = VehicleType.SUDAN;

        public required string ManufacturerId { get; set; }
        public required string ModelId { get; set; }
        public int Year { get; set; }
        public float StartingBid { get; set; }

        public int Doors {  get; set; }
    }
}
