using System;
using System.Collections.Generic;
using System.Text;

namespace Shared.DTOs.OrderDtos
{
    public class OrderDto
    {
        public int Id { get; set; }

        public int ProductId { get; set; }

        public int Quantity { get; set; } = 0;
    }
}
