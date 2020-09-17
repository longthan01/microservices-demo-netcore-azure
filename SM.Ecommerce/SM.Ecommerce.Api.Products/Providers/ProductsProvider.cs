using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SM.Ecommerce.Api.Products.DataAccess;
using SM.Ecommerce.Api.Products.Interfaces;
using SM.Ecommerce.Api.Products.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SM.Ecommerce.Api.Products.Providers
{
    public class ProductsProvider : IProductsProvider
    {
        public ProductsDbContext DataContext { get; private set; }
        public IMapper Mapper { get; private set; }
        public ILogger<ProductsProvider> Logger { get; private set; }

        public ProductsProvider(ProductsDbContext context, ILogger<ProductsProvider> logger, IMapper mapper)
        {
            this.DataContext = context;
            Mapper = mapper;
            this.Logger = logger;
            SeedData();
        }
        private void SeedData()
        {
            if (!this.DataContext.Products.Any())
            {
                Random d = new Random();
                for (int i = 0; i < 123; i++)
                {
                    this.DataContext.Products.Add(new DataAccess.Product()
                    {
                        Id = i + 1,
                        Name = "Product " + (i + 1),
                        Inventory = d.Next(1, 20),
                        Price = d.Next(1, 10) * 10
                    });
                }
                this.DataContext.SaveChanges();
            }
        }
        public async Task<(bool IsSuccess, IEnumerable<Models.Product> Products, string ErrorMessage)> GetProductsAsync()
        {
            try
            {
                var products = await this.DataContext.Products.ToListAsync();
                if (products != null && products.Any())
                {
                    var result = this.Mapper.Map<IEnumerable<DataAccess.Product>, IEnumerable<Models.Product>>(products);
                    return (true, result, null);
                }
                return (false, null, "Not found");
            }  
            catch (Exception ex)
            {
                this.Logger.LogError(ex, ex.Message);
                return (false, null, ex.Message);
            }
        }

        public async Task<(bool IsSuccess, Models.Product Product, string ErrorMessage)> GetProduct(int id)
        {
            try
            {
                var product = await this.DataContext.Products.FirstOrDefaultAsync(x => x.Id == id);
                if (product != null)
                {
                    var result = this.Mapper.Map<DataAccess.Product, Models.Product>(product);
                    return (true, result, null);
                }
                return (false, null, "Not found");
            }
            catch (Exception ex)
            {
                this.Logger.LogError(ex, ex.Message);
                return (false, null, ex.Message);
            }
        }
    }
}
