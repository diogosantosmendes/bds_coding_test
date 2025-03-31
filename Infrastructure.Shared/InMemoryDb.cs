using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Shared
{
    public interface IInMemoryDb
    {
        List<TEntity> Set<TEntity>();
    }

    public class InMemoryDb : IInMemoryDb
    {
        private IDictionary<Type, object> _sets;

        public InMemoryDb()
        {
            _sets = new Dictionary<Type, object>();
        }

        public InMemoryDb(IDictionary<Type, object> sets)
        {
            _sets = (IDictionary<Type, object>?)sets;
        }

        public List<TEntity> Set<TEntity>()
        {
            if (!_sets.ContainsKey(typeof(TEntity)))
            {
                _sets[typeof(TEntity)] = new List<TEntity>();
            }

            return (List<TEntity>)_sets[typeof(TEntity)];
        }
    }

    public static class InMemoryDbExtensions
    {
        public static IServiceCollection AddInMemoryDb(this IServiceCollection services)
        {
            // Added as a singleton to be able to use the same instance in the whole application through all requests
            // simulating a database in runtime
            services.AddSingleton<IInMemoryDb, InMemoryDb>();
            return services;
        }

        public static IServiceCollection AddInMemoryDb(this IServiceCollection services, IDictionary<Type, object> seed)
        {
            // Added as a singleton to be able to use the same instance in the whole application through all requests
            // simulating a database to run tests
            services.AddSingleton<IInMemoryDb, InMemoryDb>(provider => new InMemoryDb(seed));
            return services;
        }
    }
}
