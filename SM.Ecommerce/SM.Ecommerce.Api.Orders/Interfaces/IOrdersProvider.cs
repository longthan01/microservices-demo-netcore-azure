using SM.Ecommerce.Api.Orders.DataAccess;
using SM.Ecommerce.Api.Orders.Models;
using SM.Ecommerce.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SM.Ecommerce.Api.Orders.Interfaces
{
    public interface IOrdersProvider
    {
        Task<DataTransferResult<IEnumerable<OrderDto>>>GetListAsync();
        Task<DataTransferResult<OrderDto>> Get(int id);
    }
}
