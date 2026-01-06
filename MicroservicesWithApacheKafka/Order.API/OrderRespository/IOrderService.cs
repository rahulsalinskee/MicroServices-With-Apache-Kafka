using Product.API.DTOs.ResponseDTOs;
using Shared.DTOs.OrderDtos;

namespace Order.API.OrderRespository
{
    public interface IOrderService
    {
        public Task StartConsumingServiceAsync();

        public Task<ResponseDto> AddOrderAsync(AddOrderDto addOrderDto);

        public Task<ResponseDto> GetProductsAsync();

        public Task<ResponseDto> GetOrderSummaryAsync();
    }
}
