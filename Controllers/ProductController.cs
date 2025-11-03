using Microsoft.AspNetCore.Mvc;
using KiranaStore.Data;
using KiranaStore.Models;
using Microsoft.EntityFrameworkCore;
using KiranaStore.Services.Interfaces;

namespace KiranaStore.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductController : ControllerBase
    {
        // private readonly KiranaContext _context;
        private readonly IProductService _service;

        public ProductController(KiranaContext context, IProductService service)
        {
            // _context = context;
            _service = service;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Product>>> GetProducts()
        {
            // return await _context.Products.ToListAsync(); // SELECT * FROM Products; and return in list

            var products = await _service.GetAllAsync();
            return Ok(products);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Product>> GetProduct(int id)
        {
            // var product = await _context.Products.FindAsync(id);
            // if (product == null) return NotFound(); // -> return HTTP 404
            // return product; // -> return HTTP 200

            var product = await _service.GetByIdAsync(id);
            if (product == null) return NotFound();
            return Ok(product);
        }

        [HttpPost]
        public async Task<ActionResult<Product>> CreateProduct([FromBody] Product product, [FromHeader] string role)
        {
            // if (role != "admin") return Unauthorized("Only admin can add products."); // -> HTTP 401
            //
            // _context.Products.Add(product);
            // await _context.SaveChangesAsync();
            // return CreatedAtAction(nameof(GetProduct), new { id = product.Id }, product);

            if (role != "admin") return Unauthorized("Only admin can add products.");

            var created = await _service.CreateAsync(product);
            return CreatedAtAction(nameof(GetProduct), new { id = created.Id }, created);
        }

        [HttpPut("bulk-update")]
        public async Task<IActionResult> UpdateMultipleProducts([FromBody] List<Product> updatedProducts, [FromHeader] string role)
        {
            /*
            if (role != "admin")
                return Unauthorized("Only admin can update the products.");

            if (updatedProducts == null || !updatedProducts.Any())
                return BadRequest("No products provided for update or the body is not given.");

            foreach (var updatedProduct in updatedProducts)
            {
                var product = await _context.Products.FindAsync(updatedProduct.Id);
                if (product == null)
                    continue;

                product.Name = updatedProduct.Name;
                product.Price = updatedProduct.Price;
                product.Quantity = updatedProduct.Quantity;
            }

            await _context.SaveChangesAsync();

            return Ok("Products updated successfully.");
            */

            if (role != "admin")
                return Unauthorized("Only admin can update the products.");

            if (updatedProducts == null || !updatedProducts.Any())
                return BadRequest("No products provided for update or the body is not given.");

            await _service.BulkUpdateAsync(updatedProducts);

            return Ok("Products updated successfully.");
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProduct(int id, [FromBody] Product updatedProduct, [FromHeader] string role)
        {
            /*
            if (role != "admin") return Unauthorized("Only admin can update products.");

            var product = await _context.Products.FindAsync(id);
            if (product == null) return NotFound();

            product.Name = updatedProduct.Name;
            product.Price = updatedProduct.Price;
            product.Quantity = updatedProduct.Quantity;

            await _context.SaveChangesAsync();

            return Ok(new { message = "Product updated successfully." });
            */

            if (role != "admin") return Unauthorized("Only admin can update products.");

            try
            {
                await _service.UpdateAsync(id, updatedProduct);
                return Ok(new { message = "Product updated successfully." });
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id, [FromHeader] string role)
        {
            /*
            if (role != "admin") return Unauthorized("Only admin can delete products.");

            var product = await _context.Products.FindAsync(id);
            if (product == null) return NotFound();

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
            return NoContent();
            */

            if (role != "admin") return Unauthorized("Only admin can delete products.");

            try
            {
                await _service.DeleteAsync(id);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }

        [HttpPost("{id}/buy")]
        public async Task<IActionResult> BuyProduct(int id, [FromQuery] string role, [FromQuery] int quantity = 1)
        {
            /*
            if (role != "customer") return Unauthorized("Only customer can buy products.");

            var product = await _context.Products.FindAsync(id);
            if (product == null) return NotFound();

            if (product.Quantity < quantity) return BadRequest("Not enough stock.");

            product.Quantity -= quantity;
            await _context.SaveChangesAsync();
            return Ok(new { product.Id, product.Name, product.Quantity });
            */

            if (role != "customer") return Unauthorized("Only customer can buy products.");

            try
            {
                var product = await _service.BuyAsync(id, quantity);
                return Ok(new { product.Id, product.Name, product.Quantity });
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
