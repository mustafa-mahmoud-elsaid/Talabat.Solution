
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Talabat.Core.Services.Contract;

namespace Talabat.APIs.Controllers
{
    public class ProductsController : BaseController
    {
        #region Constructor And Fields
        private readonly IProductService _productService;
        private readonly IMapper _mapper;

        public ProductsController(IProductService productService,
            IMapper mapper)
        {
            _productService = productService;
            _mapper = mapper;
        }

        #endregion

        #region Get Products
        [HttpGet()]
        //[Authorize]
        public async Task<ActionResult<IReadOnlyList<ProductPagination<ProductToReturnDTO>>>> GetAll([FromQuery] ProductSpecParams productParams)
        {
            var result = await _productService.GetProductsAsync(productParams);

            var count = await _productService.GetProductCountAsync(productParams);

            var data = _mapper.Map<IReadOnlyList<Product>, IReadOnlyList<ProductToReturnDTO>>(result);

            return Ok(new ProductPagination<ProductToReturnDTO>(productParams.PageSize, productParams.PageIndex, count, data));
        }
        #endregion

        #region Get Product By Id
        [ProducesResponseType(typeof(ProductToReturnDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
        [HttpGet("{id}")]
        public async Task<ActionResult<ProductToReturnDTO>> Get(int id)
        {

            var result = await _productService.GetProductAsync(id);

            if (result == null)
                return NotFound(new ApiResponse(404, "Product not found"));

            return Ok(_mapper.Map<Product, ProductToReturnDTO>(result));
        }
        #endregion

        #region Get Categories And Brand
        [HttpGet("Brands")]
        public async Task<ActionResult<IReadOnlyList<ProductBrand>>> GetBrands()
            => Ok(await _productService.GetBrandsAsync());


        [HttpGet("types")]
        public async Task<ActionResult<IReadOnlyList<ProductBrand>>> GetCategories()
            => Ok(await _productService.GetCategoriesAsync()); 
        #endregion
    }
}
