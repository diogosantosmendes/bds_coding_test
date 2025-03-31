using Application.Shared.Types;
using Domain.Shared.Abstractions;

namespace Application.Shared.Abstractions
{
    public interface IGenericRepository<TEntity>
    {
        public Task<int> CountAsync(Filter? filter = null, Func<IQueryable<TEntity>, IQueryable<TEntity>>? customQuery = null);
        public Task<bool> AnyAsync(Filter? filter = null, Func<IQueryable<TEntity>, IQueryable<TEntity>>? customQuery = null);
        public Task<List<TEntity>> GetListAsync(Filter? filter = null, Sort? sort = null, Func<IQueryable<TEntity>, IQueryable<TEntity>>? customQuery = null);
        public Task<List<TEntity>> GetListAsync(Page pagination, Filter? filter = null, Sort? sort = null, Func<IQueryable<TEntity>, IQueryable<TEntity>>? customQuery = null);
        public Task<TEntity?> GetByIdAsync(string id);
        public Task<TEntity?> GetFirstAsync(Filter? filter = null, Func<IQueryable<TEntity>, IQueryable<TEntity>>? customQuery = null);
        public Task<TEntity?> GetLastAsync(Filter? filter = null, Func<IQueryable<TEntity>, IQueryable<TEntity>>? customQuery = null);
        public Task CreateAsync(TEntity entity);
        public Task UpdateAsync(TEntity entity);
        public Task DeleteAsync(string Id);
    }
}
