using GraphQL.Types;
using SM.Ecommerce.Api.Products.DataAccess;
using SM.Ecommerce.Api.Products.GraphQL.Types;
using SM.Ecommerce.Api.Products.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SM.Ecommerce.Api.Products.GraphQL.Queries
{
    public class ProductGraphQuery : ObjectGraphType
    {
        public ProductGraphQuery(IProductsProvider provider)
        {
            Name = "ProductGraphQuery";
            Field<ListGraphType<ProductGraphType>>("products", "Returns a list of Product", resolve: ctx =>
            {
                var res = provider.GetProductsAsync();
                res.Wait();
                return res.Result.Products;
            });
        }
    }
}
