using Microsoft.AspNetCore.Mvc;
using KiranaStore.Models;
using KiranaStore.Helpers;

namespace KiranaStore.Controllers
{
    [ApiController]
    [Route("api/json/[controller]")]
    public class ProductJsonController : ControllerBase
    {
        [HttpGet]
        public IActionResult GetProducts()
        {
            var products = JsonHelper.ReadJson<Product>();
            return Ok(products);
        }

        [HttpGet("{id}")]
        public IActionResult GetProduct(int id)
        {
            var products = JsonHelper.ReadJson<Product>();
            var product = products.FirstOrDefault(p => p.Id == id);
            if (product == null) return NotFound();
            return Ok(product);
        }

        [HttpPost]
        public IActionResult CreateProduct([FromBody] Product product, [FromHeader] string role)
        {
            if (role != "admin") return Unauthorized("Only admin can add products.");

            var products = JsonHelper.ReadJson<Product>();

            // Here I amke a auto-generate ID if it is not provided
            if (product.Id == 0)
                product.Id = products.Any() ? products.Max(p => p.Id) + 1 : 1;

            products.Add(product);
            JsonHelper.WriteJson(products);
            return CreatedAtAction(nameof(GetProduct), new { id = product.Id }, product); // return 201
        }

        [HttpPut("{id}")]
        public IActionResult UpdateProduct(int id, [FromBody] Product updatedProduct, [FromHeader] string role)
        {
            if (role != "admin") return Unauthorized("Only admin can update products.");

            var products = JsonHelper.ReadJson<Product>();
            var product = products.FirstOrDefault(p => p.Id == id);
            if (product == null) return NotFound();

            product.Name = updatedProduct.Name;
            product.Price = updatedProduct.Price;
            product.Quantity = updatedProduct.Quantity;

            JsonHelper.WriteJson(products);
            return Ok(new { message = "Product updated successfully." });
        }

        [HttpPut("bulk-update")]
        public IActionResult UpdateMultipleProducts([FromBody] List<Product> updatedProducts, [FromHeader] string role)
        {
            if (role != "admin") return Unauthorized("Only admin can update products.");

            var products = JsonHelper.ReadJson<Product>();

            foreach (var updatedProduct in updatedProducts)
            {
                var product = products.FirstOrDefault(p => p.Id == updatedProduct.Id);
                if (product == null) continue;

                product.Name = updatedProduct.Name;
                product.Price = updatedProduct.Price;
                product.Quantity = updatedProduct.Quantity;
            }

            JsonHelper.WriteJson(products);
            return Ok("Products updated successfully.");
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteProduct(int id, [FromHeader] string role)
        {
            if (role != "admin") return Unauthorized("Only admin can delete products.");

            var products = JsonHelper.ReadJson<Product>();
            var product = products.FirstOrDefault(p => p.Id == id);
            if (product == null) return NotFound();

            products.Remove(product);
            JsonHelper.WriteJson(products);
            return NoContent();
        }

        [HttpPost("{id}/buy")]
        public IActionResult BuyProduct(int id, [FromQuery] string role, [FromQuery] int quantity = 1)
        {
            if (role != "customer") return Unauthorized("Only customer can buy products.");

            var products = JsonHelper.ReadJson<Product>();
            var product = products.FirstOrDefault(p => p.Id == id);
            if (product == null) return NotFound();
            if (product.Quantity < quantity) return BadRequest("Not enough stock.");

            product.Quantity -= quantity;
            JsonHelper.WriteJson(products);

            return Ok(new { product.Id, product.Name, product.Quantity });
        }
    }
}
