using Application.Shared.Abstractions;
using Domain.Auctions.Entities;

namespace Application.Auctions.Abstractions.Repositories
{
    public interface IBidsRepository: IGenericRepository<Bid>
    {
    }
}
