 using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GraphQL;
using GraphQL.NewtonsoftJson;
using GraphQL.SystemTextJson;
using GraphQL.Types;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using SM.Ecommerce.Api.Products.Interfaces;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace SM.Ecommerce.Api.Products.Controllers
{
    public class GraphQLRequest
    {
        public string OperationName { get; set; }
        public string NamedQuery { get; set; }
        public string Query { get; set; }
        public JObject Variables { get; set; }
    }
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IProductsProvider productsProvider;
        private readonly ISchema schema;
        private readonly IDocumentExecuter documentExecuter;

        public ProductsController(IProductsProvider productsProvider,
                                  ISchema schema,
                                  IDocumentExecuter documentExecuter)
        {
            this.productsProvider = productsProvider;
            this.schema = schema;
            this.documentExecuter = documentExecuter;
        }
        // GET: api/<ProductsController>
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var result = await this.productsProvider.GetProductsAsync();
            if(result.IsSuccess)
            {
                return Ok(result.Products);
            }
            return NotFound();
        }

        // GET api/<ProductsController>/5
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var result = await this.productsProvider.GetProduct(id);
            if(result.IsSuccess)
            {
                return Ok(result.Product);
            }
            return NotFound();
        }

        // POST api/<ProductsController>
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] GraphQLRequest request)
        {
            if(string.IsNullOrEmpty(request.Query))
            {
                return BadRequest();
            }
            var res = await documentExecuter.ExecuteAsync(x =>
            {
                x.Schema = schema;
                x.Query = request.Query;
                x.Inputs = request.Variables?.ToInputs();
            });
            if(res.Errors?.Count > 0)
            {
                return BadRequest();
            }
            return Ok(res.Data);
        }

        // PUT api/<ProductsController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<ProductsController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
