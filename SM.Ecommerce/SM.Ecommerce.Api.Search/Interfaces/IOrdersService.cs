using SM.Ecommerce.Api.Search.Models.Orders;
using SM.Ecommerce.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SM.Ecommerce.Api.Search.Interfaces
{
    public interface IOrdersService
    {
        Task<DataTransferResult<IEnumerable<OrderDto>>> FindAsync(int customerId);
    }
}
