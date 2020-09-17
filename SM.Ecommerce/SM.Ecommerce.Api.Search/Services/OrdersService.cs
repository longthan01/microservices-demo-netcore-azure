using Microsoft.Extensions.Logging;
using SM.Ecommerce.Api.Search.Interfaces;
using SM.Ecommerce.Api.Search.Models.Orders;
using SM.Ecommerce.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace SM.Ecommerce.Api.Search.Services
{
    public class OrdersService : IOrdersService
    {
        private readonly IHttpClientFactory httpClientFactory;
        private readonly ILogger<OrdersService> logger;

        public OrdersService(IHttpClientFactory httpClientFactory, ILogger<OrdersService> logger)
        {
            this.httpClientFactory = httpClientFactory;
            this.logger = logger;
        }
        public async Task<DataTransferResult<IEnumerable<OrderDto>>> FindAsync(int customerId)
        {
            try
            {
                var client = httpClientFactory.CreateClient("OrdersService");
                var response = await client.GetAsync($"api/orders/{customerId}");
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsByteArrayAsync();
                    var result = JsonSerializer.Deserialize<OrderDto>(content, new JsonSerializerOptions()
                    {
                        PropertyNameCaseInsensitive = true
                    });
                    return new DataTransferResult<IEnumerable<OrderDto>>()
                    {
                        IsSuccess = true,
                        Result = new List<OrderDto> { result },
                    };
                }
                return new DataTransferResult<IEnumerable<OrderDto>>()
                {
                    ErrorMessage = response.ReasonPhrase
                };
            }
            catch (Exception ex)
            {
                logger?.LogError(ex, ex.Message);
                return new DataTransferResult<IEnumerable<OrderDto>>() { 
                ErrorMessage = ex.Message
                };
            }
        }
    }
}
