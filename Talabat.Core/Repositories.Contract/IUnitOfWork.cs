using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core.Entities;
using Talabat.Core.Entities.Order;

namespace Talabat.Core.Repositories.Contract
{
    public interface IUnitOfWork : IAsyncDisposable
    {
        public IGenericRepository<Product> ProductsRepo { get; set; }
        public IGenericRepository<ProductBrand> ProductBrandsRepo { get; set; }
        public IGenericRepository<ProductCategory> ProductCategoriesRepo { get; set; }
        public IGenericRepository<OrderItem> OrderItemsRepo { get; set; }
        public IGenericRepository<DeliveryMethod> DeliveryMethodsRepo { get; set; }
        public IGenericRepository<Order> OrdersRepo { get; set; }
        Task<int> CompleteAsync();
    }
}
