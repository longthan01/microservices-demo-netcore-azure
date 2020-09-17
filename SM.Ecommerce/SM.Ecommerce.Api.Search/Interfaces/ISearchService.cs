﻿using SM.Ecommerce.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SM.Ecommerce.Api.Search.Interfaces
{
    public interface ISearchService
    {
        Task<DataTransferResult<dynamic>> SearchAsync(int customerId);
    }
}
