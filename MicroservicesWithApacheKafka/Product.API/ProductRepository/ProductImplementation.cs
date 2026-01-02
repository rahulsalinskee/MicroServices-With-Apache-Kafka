using ApplicationDataContext.DataBaseContext;
using Microsoft.EntityFrameworkCore;
using Product.API.DTOs.ResponseDTOs;
using Shared.DTOs.ProductDtos;
using Shared.Mapper;

namespace Product.API.ProductRepository
{
    public class ProductImplementation : IProductService
    {
        private readonly ProductDbContext _productDbContext;

        public ProductImplementation(ProductDbContext productDbContext)
        {
            this._productDbContext = productDbContext;
        }

        public async Task<ResponseDto> CreateProductAsync(CreateProductDto newProductDto)
        {
            if (newProductDto is null)
            {
                return new ResponseDto()
                {
                    Result = null,
                    IsSuccess = false,
                    Message = "Product is null"
                };
            }

            ProductDTO productDto = new()
            {
                //Id = newProductDto.Id,
                Name = newProductDto.Name,
                Price = newProductDto.Price
            };

            var newProduct = productDto.ConvertProductDtoToProductExtension();

            /* Check if the new product is already existing in the database */
            var existingProduct = await this._productDbContext.Products.FirstOrDefaultAsync(product => product.Name == newProduct.Name);

            if (existingProduct is not null)
            {
                return new ResponseDto()
                {
                    Result = null,
                    IsSuccess = false,
                    Message = "Product already exists"
                };
            }

            await this._productDbContext.Products.AddAsync(newProduct);
            await this._productDbContext.SaveChangesAsync();

            var addedNewProductDto = newProduct.ConvertProductToProductDtoExtension();

            return new ResponseDto()
            {
                Result = addedNewProductDto,
                IsSuccess = true,
                Message = "New product is added!"
            };
        }

        public async Task<ResponseDto> DeleteProductAsync(int productId)
        {
            if (productId <= 0)
            {
                return new ResponseDto()
                {
                    Result = null,
                    IsSuccess = false,
                    Message = "Invalid Product Id"
                };
            }

            var product = await this._productDbContext.Products.FirstOrDefaultAsync(product => product.Id == productId);

            if (product is null)
            {
                return new ResponseDto()
                {
                    Result = null,
                    IsSuccess = false,
                    Message = "Product not found"
                };
            }

            this._productDbContext.Remove(product);
            await this._productDbContext.SaveChangesAsync();

            return new ResponseDto()
            {
                Result = null,
                IsSuccess = true,
                Message = "Product is deleted!"
            };
        }

        public async Task<ResponseDto> GetProductByIdAsync(int productId)
        {
            if (productId <= 0)
            {
                return new ResponseDto()
                {
                    Result = null,
                    IsSuccess = false,
                    Message = "Invalid Product Id"
                };
            }

            var product = await this._productDbContext.Products.FirstOrDefaultAsync(product => product.Id == productId);

            if (product is null)
            {
                return new ResponseDto()
                {
                    Result = null,
                    IsSuccess = false,
                    Message = "Product not found"
                };
            }

            var productDto = product.ConvertProductToProductDtoExtension();

            return new ResponseDto()
            {
                Result = productDto,
                IsSuccess = true,
                Message = "Product is found!",
            };
        }

        public async Task<ResponseDto> GetProductsAsync()
        {
            var products = await this._productDbContext.Products.ToListAsync();

            if (products is null)
            {
                return new ResponseDto()
                {
                    Result = null,
                    IsSuccess = false,
                    Message = "Products not found"
                };
            }

            IList<ProductDTO> productsDto = [];

            foreach (var product in products)
            {
                var productDto = product.ConvertProductToProductDtoExtension();

                productsDto.Add(item: productDto);
            }

            return new ResponseDto()
            {
                Result = productsDto,
                IsSuccess = true,
                Message = "Success"
            };
        }

        public async Task<ResponseDto> UpdateProductAsync(UpdateProductDto updatedProductDto)
        {
            if (updatedProductDto is null)
            {
                return new ResponseDto()
                {
                    Result = null,
                    IsSuccess = false,
                    Message = "Product is null"
                };
            }

            var existingProduct = await this._productDbContext.Products.FirstOrDefaultAsync(product => product.Name == updatedProductDto.Name && product.Price == updatedProductDto.Price);

            if (existingProduct is not null)
            {
                return new ResponseDto()
                {
                    Result = null,
                    IsSuccess = false,
                    Message = "Product is already existing"
                };
            }

            //var updatedProduct = updatedProductDto.ConvertProductDtoToProductExtension();

            existingProduct?.Name = updatedProductDto.Name;
            existingProduct?.Price = updatedProductDto.Price;

            await this._productDbContext.SaveChangesAsync();

            var updatedProductAddedToDatabaseDto = existingProduct?.ConvertProductToProductDtoExtension();

            return new ResponseDto()
            {
                Result = updatedProductAddedToDatabaseDto,
                IsSuccess = true,
                Message = "Product is updated!"
            };
        }
    }
}
