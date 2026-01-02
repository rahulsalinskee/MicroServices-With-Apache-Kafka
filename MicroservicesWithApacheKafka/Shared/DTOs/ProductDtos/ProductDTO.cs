using System.ComponentModel.DataAnnotations;

namespace Shared.DTOs.ProductDtos
{
    public class ProductDTO
    {
        public int Id { get; set; }

        [Required]
        public string? Name { get; set; }

        public decimal Price { get; set; } = decimal.Zero;
    }
}
