using GraphQL.Types;
using SM.Ecommerce.Api.Products.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SM.Ecommerce.Api.Products.GraphQL.Types
{
    public class ProductGraphType : ObjectGraphType<Product>
    {
        public ProductGraphType()
        {
            Name = "Product";
            Field(x => x.Name).Description("Name of product");
            Field(x => x.Price).Description("Product's price");
            Field(x => x.Inventory).Description("Inventory amount");
        }
    }
}
