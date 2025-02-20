using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core.Entities;
using Talabat.Core.Entities.Order;
using Talabat.Core.Repositories.Contract;
using Talabat.Repository.Data;

namespace Talabat.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly StoreDbContext _dbContext;
        private Hashtable repos;

        public UnitOfWork(StoreDbContext dbContext)
        {
            _dbContext = dbContext;
            repos = new Hashtable();
        }
        public IGenericRepository<TEntity> Repository<TEntity>() where TEntity : BaseEntity
        {
            var key = typeof(TEntity).Name;
            if(!repos.ContainsKey(key))
            {
                var value = new GenericRepository<TEntity>(_dbContext);
                repos.Add(key, value);
            }
            return (repos[key] as GenericRepository<TEntity>)!;
        }
        public async Task<int> CompleteAsync() 
            => await _dbContext.SaveChangesAsync();
        public async ValueTask DisposeAsync() 
            => await _dbContext.DisposeAsync();

    }
}
