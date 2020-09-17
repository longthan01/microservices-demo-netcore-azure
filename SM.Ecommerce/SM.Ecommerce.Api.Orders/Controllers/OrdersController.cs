using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SM.Ecommerce.Api.Orders.Interfaces;

namespace SM.Ecommerce.Api.Orders.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly IOrdersProvider ordersProvider;

        public OrdersController(IOrdersProvider ordersProvider)
        {
            this.ordersProvider = ordersProvider;
        }
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var result = await this.ordersProvider.GetListAsync();
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
            var result = await this.ordersProvider.Get(id);
            if (result.IsSuccess)
            {
                return Ok(result.Result);
            }
            return NotFound();
        }
    }
}
