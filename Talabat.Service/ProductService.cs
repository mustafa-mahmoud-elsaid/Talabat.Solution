using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core.Entities;
using Talabat.Core.ProductsSpecifications;
using Talabat.Core.Repositories.Contract;
using Talabat.Core.Services.Contract;

namespace Talabat.Service
{
    public class ProductService : IProductService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ProductService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<IReadOnlyList<Product>> GetProductsAsync(ProductSpecParams productParams)
        {
            var spec = new ProductWithBrandAndCategorySpecifications(productParams);

            var result = await _unitOfWork.Repository<Product>().GetAllWithSpecAsync(spec);
            return result;
        }
        public async Task<Product?> GetProductAsync(int productId)
        {
            var spec = new ProductWithBrandAndCategorySpecifications(productId);
            var result = await _unitOfWork.Repository<Product>().GetWithSpecAsync(spec);
            return result;
        }
        public async Task<IReadOnlyList<ProductBrand>> GetBrandsAsync()
            => await _unitOfWork.Repository<ProductBrand>().GetAllAsync();

        public async Task<IReadOnlyList<ProductCategory>> GetCategoriesAsync()
            => await _unitOfWork.Repository<ProductCategory>().GetAllAsync();

        public async Task<int> GetProductCountAsync(ProductSpecParams productParams)
        {
            var productCountSpec = new ProductWithCountSpecification(productParams);
            return await _unitOfWork.Repository<Product>().GetCountAsync(productCountSpec);
        }
    }
}
