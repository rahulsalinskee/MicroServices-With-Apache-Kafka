using Product.API.DTOs.ResponseDTOs;
using Shared.DTOs.ProductDtos;

namespace Product.API.ProductRepository
{
    public interface IProductService
    {
        public Task<ResponseDto> GetProductsAsync();

        public Task<ResponseDto> GetProductByIdAsync(int productId);

        public Task<ResponseDto> CreateProductAsync(CreateProductDto newProductDto);

        public Task<ResponseDto> UpdateProductAsync(UpdateProductDto updatedProductDto);

        public Task<ResponseDto> DeleteProductAsync(int productId);
    }
}
