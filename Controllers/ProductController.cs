using Microsoft.AspNetCore.Mvc;
using KiranaStore.Data;
using KiranaStore.Models;
using KiranaStore.Services.Interfaces;
using Microsoft.Extensions.Logging;

namespace KiranaStore.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _service;
        private readonly ILogger<ProductController> _logger;

        public ProductController(KiranaContext context, IProductService service, ILogger<ProductController> logger)
        {
            _service = service;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Product>>> GetProducts()
        {
            _logger.LogInformation("[ProductController][GetProducts] Fetching all products...");
            var products = await _service.GetAllAsync();
            _logger.LogInformation("[ProductController][GetProducts] Fetched {Count} products.", products?.Count() ?? 0);
            return Ok(products);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Product>> GetProduct(int id)
        {
            _logger.LogInformation("[ProductController][GetProduct] Fetching product with ID {Id}", id);
            var product = await _service.GetByIdAsync(id);

            if (product == null)
            {
                _logger.LogWarning("[ProductController][GetProduct] Product with ID {Id} not found.", id);
                return NotFound();
            }

            _logger.LogInformation("[ProductController][GetProduct] Found product {Name} (ID {Id})", product.Name, product.Id);
            return Ok(product);
        }

        [HttpPost]
        public async Task<ActionResult<Product>> CreateProduct([FromBody] Product product, [FromHeader] string role)
        {
            _logger.LogInformation("[ProductController][CreateProduct] Attempting to create product {Name}", product?.Name);

            if (role != "admin")
            {
                _logger.LogWarning("[ProductController][CreateProduct] Unauthorized attempt to create product by role {Role}", role);
                return Unauthorized("Only admin can add products.");
            }

            var created = await _service.CreateAsync(product);
            _logger.LogInformation("[ProductController][CreateProduct] Product {Name} created successfully with ID {Id}", created.Name, created.Id);

            return CreatedAtAction(nameof(GetProduct), new { id = created.Id }, created);
        }

        [HttpPut("bulk-update")]
        public async Task<IActionResult> UpdateMultipleProducts([FromBody] List<Product> updatedProducts, [FromHeader] string role)
        {
            _logger.LogInformation("[ProductController][BulkUpdate] Bulk update request received for {Count} products", updatedProducts?.Count ?? 0);

            if (role != "admin")
            {
                _logger.LogWarning("[ProductController][BulkUpdate] Unauthorized bulk update attempt by role {Role}", role);
                return Unauthorized("Only admin can update the products.");
            }

            if (updatedProducts == null || !updatedProducts.Any())
            {
                _logger.LogWarning("[ProductController][BulkUpdate] No products provided for bulk update.");
                return BadRequest("No products provided for update or the body is not given.");
            }

            await _service.BulkUpdateAsync(updatedProducts);
            _logger.LogInformation("[ProductController][BulkUpdate] Bulk update completed successfully.");

            return Ok("Products updated successfully.");
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProduct(int id, [FromBody] Product updatedProduct, [FromHeader] string role)
        {
            _logger.LogInformation("[ProductController][UpdateProduct] Updating product with ID {Id}", id);

            if (role != "admin")
            {
                _logger.LogWarning("[ProductController][UpdateProduct] Unauthorized update attempt by role {Role}", role);
                return Unauthorized("Only admin can update products.");
            }

            try
            {
                await _service.UpdateAsync(id, updatedProduct);
                _logger.LogInformation("[ProductController][UpdateProduct] Product with ID {Id} updated successfully.", id);
                return Ok(new { message = "Product updated successfully." });
            }
            catch (KeyNotFoundException)
            {
                _logger.LogWarning("[ProductController][UpdateProduct] Product with ID {Id} not found for update.", id);
                return NotFound();
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id, [FromHeader] string role)
        {
            _logger.LogInformation("[ProductController][DeleteProduct] Delete request for product ID {Id}", id);

            if (role != "admin")
            {
                _logger.LogWarning("[ProductController][DeleteProduct] Unauthorized delete attempt by role {Role}", role);
                return Unauthorized("Only admin can delete products.");
            }

            try
            {
                await _service.DeleteAsync(id);
                _logger.LogInformation("[ProductController][DeleteProduct] Product with ID {Id} deleted successfully.", id);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                _logger.LogWarning("[ProductController][DeleteProduct] Product with ID {Id} not found for deletion.", id);
                return NotFound();
            }
        }

        [HttpPost("{id}/buy")]
        public async Task<IActionResult> BuyProduct(int id, [FromQuery] string role, [FromQuery] int quantity = 1)
        {
            _logger.LogInformation("[ProductController][BuyProduct] Buy request received for product ID {Id}, Quantity {Qty}, Role {Role}", id, quantity, role);

            if (role != "customer")
            {
                _logger.LogWarning("[ProductController][BuyProduct] Unauthorized buy attempt by role {Role}", role);
                return Unauthorized("Only customer can buy products.");
            }

            try
            {
                var product = await _service.BuyAsync(id, quantity);
                _logger.LogInformation("[ProductController][BuyProduct] Purchase successful. Product ID {Id}, Remaining Quantity {Qty}", product.Id, product.Quantity);
                return Ok(new { product.Id, product.Name, product.Quantity });
            }
            catch (KeyNotFoundException)
            {
                _logger.LogWarning("[ProductController][BuyProduct] Product ID {Id} not found during purchase.", id);
                return NotFound();
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogError(ex, "[ProductController][BuyProduct] Error during product purchase for product ID {Id}", id);
                return BadRequest(ex.Message);
            }
        }
    }
}
