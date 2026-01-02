using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Product.API.ProductRepository;
using Product.API.ServerSideValidation;
using Shared.DTOs.ProductDtos;

namespace Product.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;
        private readonly ILogger<ProductController> _logger;

        public ProductController(IProductService productService, ILogger<ProductController> logger)
        {
            this._productService = productService;
            this._logger = logger;
        }

        [HttpGet]
        [Route("GetProducts")]
        public async Task<IActionResult> GetProducts()
        {
            var response = await this._productService.GetProductsAsync();

            if (response.IsSuccess)
            {
                return Ok(response);
            }

            return BadRequest(response);
        }

        [HttpGet("{id}", Name = "GetProduct")]
        public async Task<IActionResult> GetProductById([FromRoute] int id)
        {
            var response = await this._productService.GetProductByIdAsync(productId: id);

            if (response.IsSuccess)
            {
                return Ok(response);
            }

            return BadRequest(response);
        }

        [HttpPost]
        [ModelValidation]
        public async Task<IActionResult> CreateProduct([FromBody] CreateProductDto createNewProductDto)
        {
            var response = await this._productService.CreateProductAsync(newProductDto: createNewProductDto);

            if (response.IsSuccess)
            {
                return Ok(response);
            }

            return BadRequest(response);
        }

        [HttpPut]
        [ModelValidation]
        public async Task<IActionResult> UpdateProduct([FromBody] UpdateProductDto updateProductDto)
        {
            var response = await this._productService.UpdateProductAsync(updatedProductDto: updateProductDto);

            if (response.IsSuccess)
            {
                return Ok(response);
            }

            return BadRequest(response);
        }
    }
}
