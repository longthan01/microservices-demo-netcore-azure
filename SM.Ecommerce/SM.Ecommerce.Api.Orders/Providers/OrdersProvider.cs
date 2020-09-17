using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SM.Ecommerce.Api.Orders.DataAccess;
using SM.Ecommerce.Api.Orders.Interfaces;
using SM.Ecommerce.Api.Orders.Models;
using SM.Ecommerce.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SM.Ecommerce.Api.Orders.Providers
{
    public class OrdersProvider : IOrdersProvider
    {
        public OrdersDbContext DataContext { get; private set; }
        public IMapper Mapper { get; private set; }
        public ILogger<OrdersProvider> Logger { get; private set; }

        public OrdersProvider(OrdersDbContext context, ILogger<OrdersProvider> logger, IMapper mapper)
        {
            DataContext = context;
            Mapper = mapper;
            Logger = logger;
            SeedData();
        }
        private void SeedData()
        {
            if (!this.DataContext.Orders.Any())
            {
                Random d = new Random();
                var maxOrderItemId = 0;
                if (this.DataContext.OrderItems.Any())
                {
                    maxOrderItemId = this.DataContext.OrderItems.Max(x => x.Id);
                }
                maxOrderItemId++;
                for (int i = 0; i < 123; i++)
                {
                    int orderId = i + 1;
                    // init order items
                    List<OrderItem> items = new List<OrderItem>();
                    for (int j = 0; j < 5; j++)
                    {
                        items.Add(new OrderItem()
                        {
                            Id = maxOrderItemId++,
                            OrderId = orderId,
                            ProductId = d.Next(10, 20),
                            Quantity = d.Next(1, 10),
                            UnitPrice = d.Next(20, 25)
                        });
                    }
                    this.DataContext.Orders.Add(new DataAccess.Order()
                    {
                        Id = orderId,
                        CustomerId = d.Next(1, 20),
                        OrderDate = DateTime.Now,
                        Total = d.Next(100, 500),
                        Items = items
                    });
                }
                this.DataContext.SaveChanges();
            }
        }
        public async Task<DataTransferResult<IEnumerable<OrderDto>>> GetListAsync()
        {
            try
            {
                var orders = await this.DataContext.Orders.ToListAsync();
                if (orders != null && orders.Any())
                {
                    var result = this.Mapper.Map<IEnumerable<DataAccess.Order>, IEnumerable<Models.OrderDto>>(orders);
                    return new DataTransferResult<IEnumerable<OrderDto>>()
                    {
                        ErrorMessage = null,
                        IsSuccess = true,
                        Result = result
                    };
                }
                return new DataTransferResult<IEnumerable<OrderDto>>()
                {
                    ErrorMessage = "Not found"
                };
            }
            catch (Exception ex)
            {
                this.Logger.LogError(ex, ex.Message);
                return new DataTransferResult<IEnumerable<OrderDto>>()
                {
                    ErrorMessage = ex.Message
                };
            }
        }

        public async Task<DataTransferResult<OrderDto>> Get(int id)
        {
            try
            {
                var order = await this.DataContext.Orders.Include(x => x.Items).FirstOrDefaultAsync(x => x.Id == id);
                if (order != null)
                {
                    var result = this.Mapper.Map<DataAccess.Order, Models.OrderDto>(order);
                    return new DataTransferResult<OrderDto>()
                    {
                        IsSuccess = true,
                        Result = result
                    };
                }
                return new DataTransferResult<OrderDto>()
                {
                    ErrorMessage = "Not found"
                };
            }
            catch (Exception ex)
            {
                this.Logger.LogError(ex, ex.Message);
                return new DataTransferResult<OrderDto>()
                {
                    ErrorMessage = ex.Message
                };
            }
        }
    }
}
