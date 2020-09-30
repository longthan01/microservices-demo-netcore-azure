using GraphQL.Types;
using SM.Ecommerce.Api.Products.GraphQL.Queries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SM.Ecommerce.Api.Products.GraphQL.Schemas
{
    public class ProductSchema : Schema
    {
        public ProductSchema(IServiceProvider provider)
        {
            Query = (ProductGraphQuery) provider.GetService(typeof(ProductGraphQuery));
        }
    }
}
