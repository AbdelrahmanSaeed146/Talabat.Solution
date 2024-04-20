using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Talabat.APIs.Dtos;
using Talabat.APIs.Errors;
using Talabat.Core.Entities;
using Talabat.Core.Repositories;
using Talabat.Core.Specifications.Product_Specs;

namespace Talabat.APIs.Controllers
{
 
    public class ProductsController : BaseApiController
    {
        private readonly IGenericRepository<Product> _productRepo;
        private readonly IMapper _mapper;
        private readonly IGenericRepository<ProductBrand> _productBrandRepo;
        private readonly IGenericRepository<ProductCategory> _productCategoryRepo;

        public ProductsController(
            IGenericRepository<Product> productRepo 
            , IMapper mapper,
            IGenericRepository<ProductBrand> productBrandRepo,
             IGenericRepository<ProductCategory> productCategoryRepo
            
            )
        {
            _productRepo = productRepo;
            _mapper = mapper;
            _productBrandRepo = productBrandRepo;
            _productCategoryRepo = productCategoryRepo;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductToReturnDto>>> GetProducts()
        {

            var Spec = new ProductWithBrandAndCategorySpecifications();
            var products = await _productRepo.GetAllWithSpecAsync(Spec);

            return Ok(_mapper.Map<IEnumerable<Product> , IEnumerable<ProductToReturnDto>>(products));
        }

        [ProducesResponseType(typeof(ProductToReturnDto) , StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse) , StatusCodes.Status404NotFound)]
        [HttpGet("{id}")]

        public async Task<ActionResult<ProductToReturnDto>> GetProduct(int id)
        {

            var spec = new ProductWithBrandAndCategorySpecifications(id);
            var product = await _productRepo.GetByIdWithSpecAsync(spec);

            if (product is null)
                return NotFound(new ApiResponse(404));
            return Ok(_mapper.Map<Product , ProductToReturnDto>(product));
        }

        [HttpGet("brands")]
        public async Task<ActionResult<IEnumerable<ProductBrand>>> GetBrands()
        {
            var brands = await _productBrandRepo.GetAllAsync();
            return Ok(brands);
        }


        [HttpGet("categories")]
        public async Task<ActionResult<IEnumerable<ProductCategory>>> GetCategory()
        {
            var category = await _productCategoryRepo.GetAllAsync();
            return Ok(category);
        }
    }
}
