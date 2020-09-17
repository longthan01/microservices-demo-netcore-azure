using AutoMapper;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using SM.Ecommerce.Api.Products.DataAccess;
using SM.Ecommerce.Api.Products.Profiles;
using SM.Ecommerce.Api.Products.Providers;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SM.Ecommerce.Tests
{
    public class ProductServicesTests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public async Task GetAllProducts_ReturnAllProducts()
        {
            var dbContextOptions = new DbContextOptionsBuilder<ProductsDbContext>()
                .UseInMemoryDatabase(nameof(GetAllProducts_ReturnAllProducts)).Options;
            var dbContext = new ProductsDbContext(dbContextOptions);
            CreateProducts(dbContext);
            var profile = new ProductProfile();
            var config = new MapperConfiguration(cfg => cfg.AddProfile(profile));
            var mapper = new Mapper(config);
            var provider = new ProductsProvider(dbContext, null, mapper);
            var products = await provider.GetProductsAsync();
            Assert.AreEqual(true, products.IsSuccess);
            Assert.IsNotNull(products.Products);
            Assert.IsTrue(products.Products.Count() == 10);
            Assert.IsTrue(products.Products.First().Name  == "Product 0");
        }

        private void CreateProducts(ProductsDbContext dbContext)
        {
            for (int i = 0; i < 10; i++)
            {
                dbContext.Products.Add(new Product()
                {
                    Id = i + 1,
                    Inventory = i * 10,
                    Name = "Product " + i,
                    Price = (i * 15)

                });
            }
            dbContext.SaveChanges();
        }
    }
}