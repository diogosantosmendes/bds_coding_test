using Domain.Shared.Abstractions;

namespace Domain.Inventory.Entities
{
    class Manufacturer : IBaseEntity
    {
        public required string Id { get; set; }
        public required string Name { get; set; }

        // I'm adding the models directly on the Manufacturer class for better optimization on queries
        // I'm assuming everytime we want to present a vehicle we will need both data (Manufacturer & Model)
        // Also for frontend dynamic dropdowns, its better to update options if we get the data already grouped
        public List<Model> Models { get; set; } = new List<Model>();
    }
}
