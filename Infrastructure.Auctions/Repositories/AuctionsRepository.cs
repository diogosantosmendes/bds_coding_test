using Application.Auctions.Abstractions.Repositories;
using Domain.Auctions.Entities;
using Infrastructure.Shared;
using Infrastructure.Shared.Repositories;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Auctions.Repositories
{
    public class AuctionsRepository : GenericRepository<Auction>, IAuctionsRepository
    {
        public AuctionsRepository(IInMemoryDb db, ILogger<AuctionsRepository> logger) : base(db, logger)
        {
        }
    }
}
