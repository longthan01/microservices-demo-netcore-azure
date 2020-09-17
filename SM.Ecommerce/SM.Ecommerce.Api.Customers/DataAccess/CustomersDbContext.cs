using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SM.Ecommerce.Api.Customers.DataAccess
{
    public class Customer
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
    }
    public class CustomersDbContext : DbContext
    {
        public DbSet<Customer> Customers { get; set; }
        public CustomersDbContext(DbContextOptions options) : base(options)
        {

        }
    }
}
