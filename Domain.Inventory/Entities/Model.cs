using Domain.Shared.Abstractions;

namespace Domain.Inventory.Entities
{
    public class Model: IBaseEntity
    {
        public required string Id { get; set; }
        public required string Name { get; set; }
    }
}
