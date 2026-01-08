using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Order.API.OrderRespository;
using Order.API.ServerSideValidation;
using Shared.DTOs.OrderDtos;

namespace Order.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;
        private readonly ILogger<OrderController> logger;

        public OrderController(IOrderService orderService, ILogger<OrderController> logger)
        {
            this._orderService = orderService;
            this.logger = logger;
        }

        [HttpGet("start-consuming-service")]
        public async Task<IActionResult> StartService()
        {
            await this._orderService.StartConsumingServiceAsync();

            return NoContent();
        }

        [HttpGet]
        public async Task<IActionResult> GetOrdersSummary()
        {
            var response = await this._orderService.GetOrderSummaryAsync();

            if (response.IsSuccess)
            {
                return Ok(response);
            }
            return NotFound();
        }

        [HttpGet("products")]
        public async Task<IActionResult> Getproducts()
        {
            var response = await this._orderService.GetProductsAsync();

            if (response.IsSuccess)
            {
                return Ok(response);
            }
            return NotFound();
        }

        [HttpPost("add-order")]
        [ModelValidation]
        public async Task<IActionResult> AddOrder(AddOrderDto addOrderDto)
        {
            var response = await this._orderService.AddOrderAsync(addOrderDto: addOrderDto);

            if (response.IsSuccess)
            {
                return Ok(response);
            }
            return NotFound();
        }
    }
}
