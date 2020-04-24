using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HPlusSportAPI.Infrastructure;
using HPlusSportAPI.Models;
using HPlusSportAPI.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HPlusSportAPI.Controllers
{
    [ApiVersion("1.0")]
    //[Route("v{v:apiVersion}/api/[controller]")]
    [Route("api/products")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        ProductService _pService;

        public ProductsController(ProductService pService)
        {
            _pService = pService;
        }

        [HttpGet]

        public async Task<ActionResult<List<Product>>> GetProducts([FromQuery] ProductQueryParameters queryParameters)
        {
            var products = await _pService.GetAllProductsAsync(queryParameters);

            if (products == null) return NotFound();

            return Ok(products);

        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Product>> GetProduct(int id)
        {
            var product = await _pService.FindProductByIdAsync(id);

            if (product == null) return NotFound();

            return Ok(product);


        }

        [HttpPost]
        public async Task<ActionResult<Product>> PostProduct([FromBody]Product product)
        {
            var _product = await _pService.AddProduct(product);

            return CreatedAtAction("GetProduct", new { id = _product.Id }, _product);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutProduct([FromRoute] int id, [FromBody] Product product)
        {
            if (id != product.Id) return BadRequest("{id} and product.id is not equal");


            try
            {
                var result = await _pService.FindProductByIdAsync(id);
                if (result == null) return NotFound($"cannot found any Product by id:{id}");

                result = await _pService.UpdateProductAsync(id, product);
                return NoContent();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<Product>> DeleteProduct(int id)
        {
            var product = await _pService.FindProductByIdAsync(id);

            if (product == null) return NotFound();

            return await _pService.DeleteProductAsync(id);

        }
    }
}