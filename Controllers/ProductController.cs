using Microsoft.AspNetCore.Mvc;
using KiranaStore.Data;
using KiranaStore.Models;
using Microsoft.EntityFrameworkCore;
using KiranaStore.Helpers.Logger;

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
            AppLogger.Info("Fetching all products from the database.");
            var products = await _context.Products.ToListAsync();
            AppLogger.Info($"Total products fetched: {products.Count}");
            return products;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Product>> GetProduct(int id)
        {
            AppLogger.Info($"Fetching product with ID: {id}");
            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                AppLogger.Warning($"Product with ID: {id} not found.");
                return NotFound();
            }

            AppLogger.Info($"Product fetched successfully: {product.Name}");
            return product;
        }

        [HttpPost]
        public async Task<ActionResult<Product>> CreateProduct([FromBody] Product product, [FromHeader] string role)
        {
            AppLogger.Info($"Attempt to create product '{product.Name}' by role: {role}");

            if (role != "admin")
            {
                AppLogger.Warning("Unauthorized product creation attempt.");
                return Unauthorized("Only admin can add products.");
            }

            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            AppLogger.Info($"Product created successfully: {product.Name}, ID: {product.Id}");
            return CreatedAtAction(nameof(GetProduct), new { id = product.Id }, product);
        }

        [HttpPut("bulk-update")]
        public async Task<IActionResult> UpdateMultipleProducts([FromBody] List<Product> updatedProducts, [FromHeader] string role)
        {
            AppLogger.Info($"Bulk update requested by {role} for {updatedProducts?.Count ?? 0} products.");

            if (role != "admin")
            {
                AppLogger.Warning("Unauthorized bulk update attempt.");
                return Unauthorized("Only admin can update the products.");
            }

            if (updatedProducts == null || !updatedProducts.Any())
            {
                AppLogger.Warning("Bulk update called with empty product list.");
                return BadRequest("No products provided for update or the body is not given.");
            }

            foreach (var updatedProduct in updatedProducts)
            {
                var product = await _context.Products.FindAsync(updatedProduct.Id);
                if (product == null)
                {
                    AppLogger.Warning($"Product with ID: {updatedProduct.Id} not found during bulk update.");
                    continue;
                }

                product.Name = updatedProduct.Name;
                product.Price = updatedProduct.Price;
                product.Quantity = updatedProduct.Quantity;
            }

            await _context.SaveChangesAsync();
            AppLogger.Info("Bulk product update completed successfully.");
            return Ok("Products updated successfully.");
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProduct(int id, [FromBody] Product updatedProduct, [FromHeader] string role)
        {
            AppLogger.Info($"Update requested for product ID: {id} by {role}.");

            if (role != "admin")
            {
                AppLogger.Warning("Unauthorized product update attempt.");
                return Unauthorized("Only admin can update products.");
            }

            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                AppLogger.Warning($"Product with ID: {id} not found for update.");
                return NotFound();
            }

            product.Name = updatedProduct.Name;
            product.Price = updatedProduct.Price;
            product.Quantity = updatedProduct.Quantity;

            await _context.SaveChangesAsync();

            AppLogger.Info($"Product updated successfully: {product.Name} (ID: {product.Id})");
            return Ok(new { message = "Product updated successfully." });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id, [FromHeader] string role)
        {
            AppLogger.Info($"Delete requested for product ID: {id} by {role}.");

            if (role != "admin")
            {
                AppLogger.Warning("Unauthorized product delete attempt.");
                return Unauthorized("Only admin can delete products.");
            }

            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                AppLogger.Warning($"Product with ID: {id} not found for deletion.");
                return NotFound();
            }

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();

            AppLogger.Info($"Product deleted successfully: {product.Name} (ID: {product.Id})");
            return NoContent();
        }

        [HttpPost("{id}/buy")]
        public async Task<IActionResult> BuyProduct(int id, [FromQuery] string role, [FromQuery] int quantity = 1)
        {
            AppLogger.Info($"Buy request for product ID: {id}, quantity: {quantity}, role: {role}");

            if (role != "customer")
            {
                AppLogger.Warning("Unauthorized buy attempt.");
                return Unauthorized("Only customer can buy products.");
            }

            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                AppLogger.Warning($"Product with ID: {id} not found for purchase.");
                return NotFound();
            }

            if (product.Quantity < quantity)
            {
                AppLogger.Warning($"Insufficient stock for product ID: {id}. Requested: {quantity}, Available: {product.Quantity}");
                return BadRequest("Not enough stock.");
            }

            product.Quantity -= quantity;
            await _context.SaveChangesAsync();

            AppLogger.Info($"Product purchased successfully: {product.Name}, Remaining Qty: {product.Quantity}");
            return Ok(new { product.Id, product.Name, product.Quantity });
        }
    }
}
