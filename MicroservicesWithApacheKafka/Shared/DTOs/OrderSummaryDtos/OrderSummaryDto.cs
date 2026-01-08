using System;
using System.Collections.Generic;
using System.Text;

namespace Shared.DTOs.OrderSummaryDtos
{
    public class OrderSummaryDto
    {
        public int OrderId { get; set; }

        public int ProductId { get; set; }

        public string? ProductName { get; set; }

        public decimal ProductPrice { get; set; }

        public int OrderedQuantity { get; set; }

        public decimal TotalAmount 
        { 
            get
            {
                return ProductPrice * OrderedQuantity;
            }
            set; 
        }
    }
}
