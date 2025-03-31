using Domain.Inventory.Enums;
using Domain.Shared.Abstractions;

namespace Domain.Inventory.Abstractions
{
    public interface IVehicle : IBaseEntity
    {
        // Type will be defined by the implementation Class
        public VehicleType Type { get; }

        // Although the Manufacturer field is redundant, since we may know it through the Model,
        // with this field we greatly optimize the Manufacturer/Model searches
        public string ManufacturerId { get; set; }
        public string ModelId { get; set; }

        public int Year { get; set; }

        public float StartingBid { get; set; }

    }
}
