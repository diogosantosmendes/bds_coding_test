using Domain.Inventory.Enums;

namespace Application.Inventory.DTOs
{
    public class AddVehicleDto
    {
        public required string Id { get; set; }

        public VehicleType Type { get; set; }

        public required string ManufacturerId { get; set; }
        public required string ModelId { get; set; }

        public int Year { get; set; }

        public float StartingBid { get; set; }

        public int? Doors { get; set; }
        public int? Seats { get; set; }
        public int? LoadCapacity { get; set; }

    }
}
