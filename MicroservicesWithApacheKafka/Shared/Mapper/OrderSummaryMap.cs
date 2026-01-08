using Shared.DTOs.OrderSummaryDtos;
using Shared.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Shared.Mapper
{
    public static class OrderSummaryMap
    {
        public static OrderSummaryDto ConvertOrderSummaryToOrderSummaryDtoExtension(this OrderSummary orderSummary)
        {
            return new OrderSummaryDto()
            {
                OrderId = orderSummary.OrderId,
                ProductId = orderSummary.ProductId,
                OrderedQuantity = orderSummary.OrderedQuantity,
                ProductName = orderSummary.ProductName,
                ProductPrice = orderSummary.ProductPrice,
                TotalAmount = orderSummary.TotalAmount,
            };
        }

        public static OrderSummary ConvertOrderSummaryDtoToOrderExtension(this OrderSummaryDto orderSummaryDto)
        {
            return new OrderSummary()
            {
                OrderId = orderSummaryDto.OrderId,
                ProductId = orderSummaryDto.ProductId,
                OrderedQuantity = orderSummaryDto.OrderedQuantity,
                ProductName = orderSummaryDto.ProductName,
                ProductPrice = orderSummaryDto.ProductPrice,
                TotalAmount = orderSummaryDto.TotalAmount,
            };
        }
    }
}
