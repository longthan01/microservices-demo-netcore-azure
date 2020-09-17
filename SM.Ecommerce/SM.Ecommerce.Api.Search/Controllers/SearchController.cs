using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SM.Ecommerce.Api.Search.Interfaces;
using SM.Ecommerce.Api.Search.Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace SM.Ecommerce.Api.Search.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SearchController : ControllerBase
    {
        private readonly ISearchService searchService;

        public SearchController(ISearchService searchService)
        {
            this.searchService = searchService;
        }
        
        // GET api/<SearchController>/5
       
        public async Task<IActionResult> Get([FromQuery] SearchTerm searchTerm) 
        {
            var result = await this.searchService.SearchAsync(searchTerm.CustomerId);
            if(result.IsSuccess)
            {
                return Ok(result.Result);
            }
            return NotFound();
        }
         
        // POST api/<SearchController>
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/<SearchController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<SearchController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
