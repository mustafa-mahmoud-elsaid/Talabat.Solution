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
        private readonly IGenericRepository<Product> _repository;
        private readonly IMapper _mapper;

        public ProductsController(IGenericRepository<Product> repository , IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }
        [HttpGet()]
        public async Task<ActionResult<IEnumerable<Product>>> GetAll()
        {
            var spec = new ProductWithBrandAndCategorySpecifications();    
            var result = await _repository.GetAllWithSpecAsync(spec);
            return Ok(_mapper.Map<IEnumerable<Product> , IEnumerable<ProductToReturnDTO>>(result));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Product>> Get(int id)
        {
            var spec = new ProductWithBrandAndCategorySpecifications(id);
            var result = await _repository.GetWithSpecAsync(spec);
            if (result == null)
                return NotFound(new ApiResponse(404, "Product not found"));
            return Ok(_mapper.Map<Product , ProductToReturnDTO>(result));
        }
    }
}
