using System;
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

        public IGenericRepository<Product> ProductsRepo { get; set; }
        public IGenericRepository<ProductBrand> ProductBrandsRepo { get; set; }
        public IGenericRepository<ProductCategory> ProductCategoriesRepo { get; set; }
        public IGenericRepository<OrderItem> OrderItemsRepo { get; set; }
        public IGenericRepository<DeliveryMethod> DeliveryMethodsRepo { get; set; }
        public IGenericRepository<Order> OrdersRepo { get; set; }
        public UnitOfWork(StoreDbContext dbContext)
        {
            _dbContext = dbContext;
            ProductsRepo = new GenericRepository<Product>(_dbContext);
            ProductBrandsRepo = new GenericRepository<ProductBrand>(_dbContext);
            ProductCategoriesRepo = new GenericRepository<ProductCategory>(_dbContext);
            OrderItemsRepo = new GenericRepository<OrderItem>(_dbContext);
            DeliveryMethodsRepo = new GenericRepository<DeliveryMethod>(_dbContext);
            OrdersRepo = new GenericRepository<Order>(_dbContext);
        }
        public async Task<int> CompleteAsync() 
            => await _dbContext.SaveChangesAsync();
        public async ValueTask DisposeAsync() 
            => await _dbContext.DisposeAsync();

    }
}
