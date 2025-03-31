using Application.Auctions.Abstractions.Repositories;
using Domain.Auctions.Entities;
using Infrastructure.Shared;
using Infrastructure.Shared.Repositories;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Auctions.Repositories
{
    public class BidsRepository : GenericRepository<Bid>, IBidsRepository
    {
        public BidsRepository(IInMemoryDb db, ILogger<BidsRepository> logger) : base(db, logger)
        {
        }
    }
}
