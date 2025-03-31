using Application.Shared.Abstractions;
using Application.Shared.Types;
using Domain.Shared.Abstractions;
using Infrastructure.Shared.Extensions;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Shared.Repositories
{

    // returns handled this way to mock the async db call
    public abstract class GenericRepository<TEntity> : IGenericRepository<TEntity> where TEntity : IBaseEntity
    {
        // this List represent a data source collection eg: DbSet from Entity Framework,
        // the write actions wold need to be handled in a different way
        // and the read actions would be the same without the .AsQueryable() and preferably with Async calls
        protected List<TEntity> _collection;
        protected readonly ILogger _logger;

        protected GenericRepository(IInMemoryDb db, ILogger logger)
        {
            _collection = db.Set<TEntity>();
            _logger = logger;
        }

        public virtual Task<int> CountAsync(
            Filter? filter = null,
            Func<IQueryable<TEntity>, IQueryable<TEntity>>? customQuery = null
        )
        {
            IQueryable<TEntity> query = _collection.AsQueryable().ApplyFilter(filter);

            if (customQuery != null)
            {
                query = customQuery(query);
            }

            return Task.FromResult(query.Count());
        }

        public virtual Task<bool> AnyAsync(
            Filter? filter = null,
            Func<IQueryable<TEntity>, IQueryable<TEntity>>? customQuery = null
        )
        {
            IQueryable<TEntity> query = _collection.AsQueryable().ApplyFilter(filter);

            if (customQuery != null)
            {
                query = customQuery(query);
            }
            return Task.FromResult(query.Count() > 0);
        }

        public virtual Task<List<TEntity>> GetListAsync(
            Filter? filter = null,
            Sort? sort = null,
            Func<IQueryable<TEntity>, IQueryable<TEntity>>? customQuery = null
        )
        {
            IQueryable<TEntity> query = _collection
                .AsQueryable()
                .ApplyFilter(filter)
                .ApplySorting(sort);

            if (customQuery != null)
            {
                query = customQuery(query);
            }

            return Task.FromResult(query.ToList());
        }

        public virtual Task<List<TEntity>> GetListAsync(
            Page pagination,
            Filter? filter = null,
            Sort? sort = null,
            Func<IQueryable<TEntity>, IQueryable<TEntity>>? customQuery = null
        )
        {
            IQueryable<TEntity> query = _collection
                .AsQueryable()
                .ApplyFilter(filter)
                .ApplySorting(sort);

            if (customQuery != null)
            {
                query = customQuery(query);
            }

            return Task.FromResult(query
                .Skip((pagination.Index - 1) * pagination.Size)
                .Take(pagination.Size + 1)
                .ToList());
        }

        public virtual Task<TEntity?> GetByIdAsync(string id)
        {
            return Task.FromResult(_collection.FirstOrDefault(x => x.Id.Equals(id)));
        }

        public virtual Task<TEntity?> GetFirstAsync(
            Filter? filter = null,
            Func<IQueryable<TEntity>, IQueryable<TEntity>>? customQuery = null
        )
        {
            IQueryable<TEntity> query = _collection
                .AsQueryable()
                .ApplyFilter(filter);

            if (customQuery != null)
            {
                query = customQuery(query);
            }

            return Task.FromResult(query.ToList().FirstOrDefault());
        }

        public virtual Task<TEntity?> GetLastAsync(
            Filter? filter = null,
            Func<IQueryable<TEntity>, IQueryable<TEntity>>? customQuery = null
        )
        {
            IQueryable<TEntity> query = _collection
                .AsQueryable()
                .ApplyFilter(filter);

            if (customQuery != null)
            {
                query = customQuery(query);
            }

            return Task.FromResult(query.ToList().LastOrDefault());
        }

        public virtual Task CreateAsync(TEntity entity)
        {
            _collection.Add(entity);
            return Task.CompletedTask;
        }

        public virtual Task UpdateAsync(TEntity entity)
        {
            var entry = _collection.FirstOrDefault(x => x.Id.Equals(entity.Id));
            if (entry != null)
            {
                _collection.Remove(entry);
                _collection.Add(entity);
            }
            return Task.CompletedTask;
        }

        public virtual Task DeleteAsync(string id)
        {
            var entry = _collection.FirstOrDefault(x => x.Id.Equals(id));
            if (entry != null)
            {
                _collection.Remove(entry);
            }
            return Task.CompletedTask;
        }
    }
}
