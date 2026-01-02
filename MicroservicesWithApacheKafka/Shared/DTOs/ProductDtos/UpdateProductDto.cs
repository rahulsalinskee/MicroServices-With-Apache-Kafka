namespace Shared.DTOs.ProductDtos
{
    public class UpdateProductDto
    {
        public string Name { get; set; }
        public decimal Price { get; set; } = decimal.Zero;
    }
}
