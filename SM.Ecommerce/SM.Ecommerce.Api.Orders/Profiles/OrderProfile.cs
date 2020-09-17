using SM.Ecommerce.Api.Orders.DataAccess;
using SM.Ecommerce.Api.Orders.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SM.Ecommerce.Api.Orders.Profiles
{
    public class OrderProfile : AutoMapper.Profile
    {
        public OrderProfile()
        {
            CreateMap<Order, Models.OrderDto>();
            CreateMap<OrderItem, OrderItemDto>();
        }
    }
}
