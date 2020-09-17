using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SM.Ecommerce.Api.Customers.Interfaces;

namespace SM.Ecommerce.Api.Orders.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomersController : ControllerBase
    {
        private readonly ICustomersProvider customersProvider;

        public CustomersController(ICustomersProvider customersProvider)
        {
            this.customersProvider = customersProvider;
        }
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var result = await this.customersProvider.GetListAsync();
            if (result.IsSuccess)
            {
                return Ok(result.Result);
            }
            return NotFound();
        }

        // GET api/<ProductsController>/5
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var result = await this.customersProvider.Get(id);
            if (result.IsSuccess)
            {
                return Ok(result.Result);
            }
            return NotFound();
        }
    }
}
