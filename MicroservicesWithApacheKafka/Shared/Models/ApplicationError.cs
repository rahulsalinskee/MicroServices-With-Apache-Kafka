using System;
using System.Collections.Generic;
using System.Text;

namespace Shared.Models
{
    public class ApplicationError
    {
        public Guid Id { get; set; }

        public DateTime When { get; set; }

        public string Message { get; set; } = string.Empty;
    }
}
