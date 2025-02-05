using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Talabat.APIs.DTOs;
using Talabat.APIs.Errors;
using Talabat.Core.Entities;
using Talabat.Core.ProductsSpecifications;
using Talabat.Core.Repositories.Contract;
using Talabat.Core.Specifications;

namespace Talabat.APIs.Controllers
{
    public class ProductsController : BaseController
    {
        private readonly IGenericRepository<Product> _productRepository;
        private readonly IGenericRepository<ProductBrand> _brandRepo;
        private readonly IGenericRepository<ProductCategory> _categoryRepo;
        private readonly IMapper _mapper;

        public ProductsController(IGenericRepository<Product> productRepo,
            IGenericRepository<ProductBrand> brandRepo,
            IGenericRepository<ProductCategory> categoryRepo,
            IMapper mapper)
        {
            _productRepository = productRepo;
            _brandRepo = brandRepo;
            _categoryRepo = categoryRepo;
            _mapper = mapper;
        }
        [HttpGet()]
        public async Task<ActionResult<IReadOnlyList<Product>>> GetAll(string? sort = "nameAsc")
        {
            var spec = new ProductWithBrandAndCategorySpecifications(sort);
            var result = await _productRepository.GetAllWithSpecAsync(spec);
            return Ok(_mapper.Map<IReadOnlyList<Product>, IReadOnlyList<ProductToReturnDTO>>(result));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Product>> Get(int id)
        {
            var spec = new ProductWithBrandAndCategorySpecifications(id);
            var result = await _productRepository.GetWithSpecAsync(spec);
            if (result == null)
                return NotFound(new ApiResponse(404, "Product not found"));
            return Ok(_mapper.Map<Product, ProductToReturnDTO>(result));
        }
        [HttpGet("Brands")]
        public async Task<ActionResult<IReadOnlyList<ProductBrand>>> GetBrands()
        {
            return Ok(await _brandRepo.GetAllAsync());
        }
        [HttpGet("Categories")]
        public async Task<ActionResult<IReadOnlyList<ProductBrand>>> GetCategories()
        {
            return Ok(await _categoryRepo.GetAllAsync());
        }
    }
}
