using Microsoft.AspNetCore.Mvc;
using KiranaStore.Data;
using KiranaStore.Models;
using Microsoft.EntityFrameworkCore;

namespace KiranaStore.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductController : ControllerBase
    {
        private readonly KiranaContext _context;

        public ProductController(KiranaContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Product>>> GetProducts()
        {
            return await _context.Products.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Product>> GetProduct(int id)
        {
            var product = await _context.Products.FindAsync(id); // SELECT * FROM Products; and return in list
            if (product == null) return NotFound(); // -> return HTTP 404
            return product; // -> return HTTP 200
        }

        [HttpPost]
        public async Task<ActionResult<Product>> CreateProduct([FromBody] Product product, [FromHeader] string role)
        {
            if (role != "admin") return Unauthorized("Only admin can add products."); // -> HTTP 401

            _context.Products.Add(product);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetProduct), new { id = product.Id }, product);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProduct(int id, [FromBody] Product updatedProduct, [FromQuery] string role)
        {
            if (role != "admin") return Unauthorized("Only admin can update products.");

            var product = await _context.Products.FindAsync(id);
            if (product == null) return NotFound();

            product.Name = updatedProduct.Name;
            product.Price = updatedProduct.Price;
            product.Quantity = updatedProduct.Quantity;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id, [FromQuery] string role)
        {
            if (role != "admin") return Unauthorized("Only admin can delete products.");

            var product = await _context.Products.FindAsync(id);
            if (product == null) return NotFound();

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpPost("{id}/buy")]
        public async Task<IActionResult> BuyProduct(int id, [FromQuery] string role, [FromQuery] int quantity = 1)
        {
            if (role != "customer") return Unauthorized("Only customer can buy products.");

            var product = await _context.Products.FindAsync(id);
            if (product == null) return NotFound();

            if (product.Quantity < quantity) return BadRequest("Not enough stock.");

            product.Quantity -= quantity;
            await _context.SaveChangesAsync();
            return Ok(new { product.Id, product.Name, product.Quantity });
        }
    }
}
