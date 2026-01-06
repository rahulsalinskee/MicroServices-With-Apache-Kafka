using Shared.DTOs.OrderDtos;
using Shared.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Shared.Mapper
{
    public static class OrderMap
    {
        public static OrderDto? ConvertOrderToOrderDto(this Order order)
        {
            if (order is null)
            {
                return null;
            }
            return new OrderDto()
            {
                Id = order.Id,
                ProductId = order.ProductId,
                Quantity = order.Quantity
            };
        }

        public static Order? ConvertOrderDtoToOrder(this OrderDto orderDto)
        {
            if (orderDto is null)
            {
                return null;
            }
            return new Order()
            {
                Id = orderDto.Id,
                ProductId = orderDto.ProductId,
                Quantity = orderDto.Quantity
            };
        }
    }
}
