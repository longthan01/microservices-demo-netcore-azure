using SM.Ecommerce.Api.Search.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SM.Ecommerce.Api.Search.Services
{
    public class SearchService : ISearchService
    {
        private readonly IOrdersService ordersService;

        public SearchService(IOrdersService ordersService)
        {
            this.ordersService = ordersService;
        }
        public async Task<Shared.DataTransferResult<dynamic>> SearchAsync(int customerId)
        {
            var res = await ordersService.FindAsync(customerId);
            if (res.IsSuccess)
            {
                var result = new
                {
                    Orders = res.Result
                };
                return new Shared.DataTransferResult<dynamic>()
                {
                    IsSuccess = true,
                    Result = result
                };
            }
            return new Shared.DataTransferResult<dynamic>()
            {
                ErrorMessage = "Not found"
            }; 
        }
    }
}
