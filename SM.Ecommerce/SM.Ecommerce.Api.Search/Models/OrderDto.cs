using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SM.Ecommerce.Api.Search.Models.Orders
{
    public class OrderItemDto
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
    }
    public class OrderDto
    {
        public int Id { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal Total { get; set; }
        public IEnumerable<OrderItemDto> Items { get; set; }
    }
}
