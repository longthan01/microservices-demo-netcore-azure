using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SM.Ecommerce.Api.Customers.DataAccess;
using SM.Ecommerce.Api.Customers.Interfaces;
using SM.Ecommerce.Api.Customers.Models;
using SM.Ecommerce.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SM.Ecommerce.Api.Customers.Providers
{
    public class CustomersProvider : ICustomersProvider
    {
        public CustomersDbContext DataContext { get; private set; }
        public IMapper Mapper { get; private set; }
        public ILogger<CustomersProvider> Logger { get; private set; }

        public CustomersProvider(CustomersDbContext context, ILogger<CustomersProvider> logger, IMapper mapper)
        {
            DataContext = context;
            Mapper = mapper;
            Logger = logger;
            SeedData();
        }
        private void SeedData()
        {
            if (!this.DataContext.Customers.Any())
            {
                Random d = new Random();
                for (int i = 0; i < 123; i++)
                {
                    this.DataContext.Customers.Add(new Customer()
                    {
                        Id = i + 1,
                        Name = "Customer " + (i + 1),
                        Address = "Address " + (i + 1),
                    });
                }
                this.DataContext.SaveChanges();
            }
        }
        public async Task<DataTransferResult<IEnumerable<CustomerDto>>> GetListAsync()
        {
            try
            {
                var orders = await this.DataContext.Customers.ToListAsync();
                if (orders != null && orders.Any())
                {
                    var result = this.Mapper.Map<IEnumerable<DataAccess.Customer>, IEnumerable<Models.CustomerDto>>(orders);
                    return new DataTransferResult<IEnumerable<CustomerDto>>()
                    {
                        ErrorMessage = null,
                        IsSuccess = true,
                        Result = result
                    };
                }
                return new DataTransferResult<IEnumerable<CustomerDto>>()
                {
                    ErrorMessage = "Not found"
                };
            }
            catch (Exception ex)
            {
                this.Logger.LogError(ex, ex.Message);
                return new DataTransferResult<IEnumerable<CustomerDto>>()
                {
                    ErrorMessage = ex.Message
                };
            }
        }

        public async Task<DataTransferResult<CustomerDto>> Get(int id)
        {
            try
            {
                var order = await this.DataContext.Customers.FirstOrDefaultAsync(x => x.Id == id);
                if (order != null)
                {
                    var result = this.Mapper.Map<DataAccess.Customer, Models.CustomerDto>(order);
                    return new DataTransferResult<CustomerDto>()
                    {
                        IsSuccess = true,
                        Result = result
                    };
                }
                return new DataTransferResult<CustomerDto>()
                {
                    ErrorMessage = "Not found"
                };
            }
            catch (Exception ex)
            {
                this.Logger.LogError(ex, ex.Message);
                return new DataTransferResult<CustomerDto>()
                {
                    ErrorMessage = ex.Message
                };
            }
        }
    }
}
