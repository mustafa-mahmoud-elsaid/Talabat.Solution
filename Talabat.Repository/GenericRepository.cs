using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core.Entities;
using Talabat.Core.Repositories.Contract;
using Talabat.Repository.Data;

namespace Talabat.Repository
{
    public class GenericRepository<T> : IGenericRepository<T> where T : BaseEntity
    {
        private readonly StoreDbContext _dbContext;

        public GenericRepository(StoreDbContext dbContext)
            => _dbContext = dbContext;

        public async Task<T?> GetAsync(int id)
            => await _dbContext.Set<T>().FindAsync(id);

        public async Task<IEnumerable<T>> GetAllAsync()
            => await _dbContext.Set<T>().ToListAsync();
    }
}
