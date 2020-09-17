using SM.Ecommerce.Api.Products.DataAccess;
using SM.Ecommerce.Api.Products.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SM.Ecommerce.Api.Products.Profiles
{
    public class ProductProfile : AutoMapper.Profile
    {
        public ProductProfile()
        {
            CreateMap<DataAccess.Product, Models.Product>();
        }
    }
}
