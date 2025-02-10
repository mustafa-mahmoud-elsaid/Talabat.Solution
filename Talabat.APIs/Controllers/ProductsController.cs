
namespace Talabat.APIs.Controllers
{
    public class ProductsController : BaseController
    {
        #region Constructor And Fields
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

        #endregion

        #region Get Products
        [HttpGet()]
        public async Task<ActionResult<IReadOnlyList<ProductPagination<ProductToReturnDTO>>>> GetAll([FromQuery] ProductSpecParams productParams)
        {
            var spec = new ProductWithBrandAndCategorySpecifications(productParams);

            var result = await _productRepository.GetAllWithSpecAsync(spec);

            var productCountSpec = new ProductWithCountSpecification(productParams);

            var count = await _productRepository.GetCountAsync(productCountSpec);

            var data = _mapper.Map<IReadOnlyList<Product>, IReadOnlyList<ProductToReturnDTO>>(result);

            return Ok(new ProductPagination<ProductToReturnDTO>(productParams.PageSize, productParams.PageIndex, count, data));
        }
        #endregion

        #region Get Product By Id
        [HttpGet("{id}")]
        public async Task<ActionResult<ProductToReturnDTO>> Get(int id)
        {
            var spec = new ProductWithBrandAndCategorySpecifications(id);

            var result = await _productRepository.GetWithSpecAsync(spec);

            if (result == null)
                return NotFound(new ApiResponse(404, "Product not found"));

            return Ok(_mapper.Map<Product, ProductToReturnDTO>(result));
        }
        #endregion

        #region Get Categories And Brand
        [HttpGet("Brands")]
        public async Task<ActionResult<IReadOnlyList<ProductBrand>>> GetBrands()
            => Ok(await _brandRepo.GetAllAsync());


        [HttpGet("Categories")]
        public async Task<ActionResult<IReadOnlyList<ProductBrand>>> GetCategories()
            => Ok(await _categoryRepo.GetAllAsync()); 
        #endregion
    }
}
