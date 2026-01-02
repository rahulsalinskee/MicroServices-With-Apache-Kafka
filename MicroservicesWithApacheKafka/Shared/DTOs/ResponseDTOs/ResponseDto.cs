namespace Product.API.DTOs.ResponseDTOs
{
    public class ResponseDto
    {
        public object? Result { get; set; }

        public bool IsSuccess { get; set; } = default;

        public string? Message { get; set; }
    }
}
