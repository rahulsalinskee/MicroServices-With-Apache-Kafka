namespace Shared.DTOs.ProductDtos
{
    public class CreateProductDto
    {
        public string? Name { get; set; }

        public decimal Price { get; set; } = decimal.Zero;
    }
}
