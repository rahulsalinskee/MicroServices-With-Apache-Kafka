using Shared.DTOs.ProductDtos;

namespace Shared.Mapper
{
    public static class ProductMap
    {
        public static ProductDTO ConvertProductToProductDtoExtension(this Models.Product product)
        {
            return new ProductDTO()
            {
                Id = product.Id,
                Name = product.Name,
                Price = product.Price
            };
        }

        public static Models.Product ConvertProductDtoToProductExtension(this ProductDTO productDto)
        {
            return new Models.Product()
            {
                Id = productDto.Id,
                Name = productDto.Name,
                Price = productDto.Price
            };
        }
    }
}
