using SM.Ecommerce.Api.Customers.Models;
using SM.Ecommerce.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SM.Ecommerce.Api.Customers.Interfaces
{
    public interface ICustomersProvider
    {
        Task<DataTransferResult<IEnumerable<CustomerDto>>>GetListAsync();
        Task<DataTransferResult<CustomerDto>> Get(int id);
    }
}
