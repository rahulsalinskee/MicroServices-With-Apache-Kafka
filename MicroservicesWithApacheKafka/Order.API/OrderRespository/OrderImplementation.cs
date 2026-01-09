using ApplicationDataContext.DataBaseContext;
using Confluent.Kafka;
using Microsoft.EntityFrameworkCore;
using Product.API.DTOs.ResponseDTOs;
using Shared.DTOs.OrderDtos;
using Shared.DTOs.OrderSummaryDtos;
using Shared.DTOs.ProductDtos;
using Shared.Mapper;
using Shared.Models;
using System.Text.Json;

namespace Order.API.OrderRespository
{
    public class OrderImplementation : IOrderService
    {
        private const string ADD_PRODUCT_TOPIC = "Add-Product-Topic";
        private const string GET_PRODUCT_TOPIC = "Get-Product-Topic";
        private const string GET_PRODUCTS_TOPIC = "Get-Products-Topic";
        private const string DELETE_PRODUCT_TOPIC = "Delete-Product-Topic";
        private const string UPDATE_PRODUCT_TOPIC = "Update-Product-Topic";

        private readonly IConsumer<Null, string> _consumer;
        private readonly OrderDbContext _orderDbContext;
        private readonly ProductDbContext _productDbContext;
        private readonly OrderSummaryDbContext _orderSummaryDbContext;

        public OrderImplementation(IConsumer<Null, string> consumer, OrderDbContext orderDbContext, ProductDbContext productDbContext, OrderSummaryDbContext orderSummaryDbContext)
        {
            this._consumer = consumer;
            this._orderDbContext = orderDbContext;
            this._productDbContext = productDbContext;
            this._orderSummaryDbContext = orderSummaryDbContext;
        }

        public async Task StartConsumingServiceAsync()
        {
            await Task.Delay(10);

            /* Subscribe to topics */
            this._consumer.Subscribe(topics: new List<string> { ADD_PRODUCT_TOPIC, DELETE_PRODUCT_TOPIC });

            while (true)
            {
                var consumerResponse = this._consumer.Consume();

                if (!string.IsNullOrEmpty(consumerResponse?.Message?.Value))
                {
                    switch (consumerResponse.Topic)
                    {
                        case ADD_PRODUCT_TOPIC:
                            {
                                var productDto = JsonSerializer.Deserialize<ProductDTO>(json: consumerResponse.Message.Value);

                                if (productDto is not null)
                                {
                                    var product = productDto.ConvertProductDtoToProductExtension();

                                    OrderSummaryDto orderSummaryDto = new()
                                    {
                                        OrderId = product.Id,
                                        ProductId = product.Id,
                                        ProductName = product.Name,
                                        ProductPrice = product.Price,
                                    };

                                    var orderSummary = orderSummaryDto.ConvertOrderSummaryDtoToOrderExtension();

                                    await this._orderSummaryDbContext.OrderSummaries.AddAsync(orderSummary);
                                    await this._orderSummaryDbContext.SaveChangesAsync();
                                }
                                break;
                            }
                        case UPDATE_PRODUCT_TOPIC:
                            {
                                var productDto = JsonSerializer.Deserialize<ProductDTO>(json: consumerResponse.Message.Value);

                                if (productDto is not null)
                                {
                                    var product = productDto.ConvertProductDtoToProductExtension();

                                    OrderSummaryDto orderSummaryDto = new()
                                    {
                                        OrderId = product.Id,
                                        ProductId = product.Id,
                                        ProductName = product.Name,
                                        ProductPrice = product.Price,
                                    };

                                    var orderSummary = orderSummaryDto.ConvertOrderSummaryDtoToOrderExtension();

                                    this._orderSummaryDbContext.OrderSummaries.Update(orderSummary);
                                    await this._orderSummaryDbContext.SaveChangesAsync();
                                }
                                break;
                            }
                        case DELETE_PRODUCT_TOPIC:
                            {
                                var productDto = JsonSerializer.Deserialize<ProductDTO>(json: consumerResponse.Message.Value);

                                if (productDto is not null)
                                {
                                    var product = productDto.ConvertProductDtoToProductExtension();

                                    OrderSummaryDto orderSummaryDto = new()
                                    {
                                        OrderId = product.Id,
                                        ProductId = product.Id,
                                        ProductName = product.Name,
                                        ProductPrice = product.Price,
                                    };

                                    var orderSummary = orderSummaryDto.ConvertOrderSummaryDtoToOrderExtension();

                                    this._orderSummaryDbContext.OrderSummaries.Remove(orderSummary);
                                    await this._orderSummaryDbContext.SaveChangesAsync();
                                }
                                break;
                            }
                    }
                    DisplayProduct();
                }
            }
        }

        public async Task<ResponseDto> AddOrderAsync(AddOrderDto addOrderDto)
        {
            if (addOrderDto is null)
            {
                return new ResponseDto()
                {
                    Result = null,
                    IsSuccess = false,
                    Message = "Order is null"
                };
            }

            if (addOrderDto.ProductId <= 0 || addOrderDto.Quantity <= 0)
            {
                return new ResponseDto()
                {
                    Result = null,
                    IsSuccess = false,
                    Message = "Product ID or Quantity cannot be less than 1"
                };
            }

            OrderDto orderDto = new()
            {
                ProductId = addOrderDto.ProductId,
                Quantity = addOrderDto.Quantity
            };

            var order = orderDto.ConvertOrderDtoToOrderExtension();

            var existingOrder = await this._orderDbContext.Orders.FirstOrDefaultAsync(order => order.Id == order.Id);

            if (existingOrder is not null)
            {
                return new ResponseDto()
                {
                    Result = null,
                    IsSuccess = false,
                    Message = "Order is repeating!"
                };
            }

            await this._orderDbContext.Orders.AddAsync(order);
            await this._orderDbContext.SaveChangesAsync();

            var addedOrderInDatabaseDto = order.ConvertOrderToOrderDtoExtension();

            return new ResponseDto()
            {
                Result = addedOrderInDatabaseDto,
                IsSuccess = true,
                Message = "Order is added successfully!"
            };
        }

        public async Task<ResponseDto> GetOrderSummaryAsync()
        {
            IList<OrderSummaryDto> ordersSummaryDto = [];

            IList<OrderDto> ordersDto = [];

            var orders = await this._orderDbContext.Orders.ToListAsync();

            if (orders is null)
            {
                ApplicationError applicationError = new()
                {
                    Id = new Guid(),
                    When = DateTime.Now,
                    Message = "Order is null!"
                };

                return new ResponseDto()
                {
                    Result = null,
                    IsSuccess = false,
                    Message = applicationError.Message
                };
            }

            if (!orders.Any())
            {
                ApplicationError applicationError = new()
                {
                    Id = new Guid(),
                    When = DateTime.Now,
                    Message = "Currently, There are no orders!"
                };

                return new ResponseDto()
                {
                    Result = null,
                    IsSuccess = false,
                    Message = applicationError.Message
                };
            }

            foreach (var order in orders)
            {
                var orderDto = order.ConvertOrderToOrderDtoExtension();
                
                ordersDto.Add(orderDto);

                var productDetailInOrderPlaced = await this._productDbContext.Products.FirstOrDefaultAsync(product => product.Id == orderDto.ProductId);

                var orderSummaryDto = new OrderSummaryDto()
                {
                    OrderId = orderDto.Id,
                    ProductId = orderDto.ProductId,
                    ProductName = productDetailInOrderPlaced?.Name ?? "Product not found",
                    ProductPrice = productDetailInOrderPlaced.Price,
                    OrderedQuantity = orderDto.Quantity
                };

                var orderSummary = orderSummaryDto.ConvertOrderSummaryDtoToOrderExtension();

                await this._orderSummaryDbContext.OrderSummaries.AddAsync(orderSummary);
                await this._orderSummaryDbContext.SaveChangesAsync();

                var addedOrderSummaryDto = orderSummary.ConvertOrderSummaryToOrderSummaryDtoExtension();

                ordersSummaryDto.Add(addedOrderSummaryDto);
            }

            return new ResponseDto()
            {
                Result = ordersSummaryDto,
                IsSuccess = true,
                Message = "Order summary fetched!"
            };
        }

        public async Task<ResponseDto> GetProductsAsync()
        {
            IList<ProductDTO> _productsDto = [];
            var products = await this._productDbContext.Products.ToListAsync();

            if (products is null)
            {
                return null;
            }

            foreach (var product in products)
            {
                var productDto = product.ConvertProductToProductDtoExtension();
                _productsDto.Add(productDto);
            }
            return new ResponseDto()
            {
                Result = _productsDto,
                IsSuccess = true,
                Message = "Products are fetched!"
            };
        }

        private void DisplayProduct()
        {
            Console.Clear();

            foreach (var product in this._productDbContext.Products)
            {
                Console.WriteLine($"Product Id: {product.Id}");
                Console.WriteLine($"Product Name: {product.Name}");
                Console.WriteLine($"Product Price: {product.Price} \n");
            }
        }
    }
}
