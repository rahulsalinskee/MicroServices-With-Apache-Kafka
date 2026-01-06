using System;
using System.Collections.Generic;
using System.Text;

namespace Shared.DTOs.OrderDtos
{
    public class AddOrderDto
    {
        public int ProductId { get; set; }

        public int Quantity { get; set; }
    }
}
