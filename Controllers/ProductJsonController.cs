using Microsoft.AspNetCore.Mvc;
using KiranaStore.Models;
using KiranaStore.Services.Interfaces;
using Microsoft.Extensions.Logging;

namespace KiranaStore.Controllers
{
    [ApiController]
    [Route("api/json/[controller]")]
    public class ProductJsonController : ControllerBase
    {
        private readonly IProductJsonService _service;
        private readonly ILogger<ProductJsonController> _logger;

        public ProductJsonController(IProductJsonService service, ILogger<ProductJsonController> logger)
        {
            _service = service;
            _logger = logger;
        }

        [HttpGet]
        public IActionResult GetProducts()
        {
            _logger.LogInformation("[ProductJsonController][GetProducts] Fetching all products...");
            var products = _service.GetAll();
            _logger.LogInformation("[ProductJsonController][GetProducts] Fetched {Count} products.", products?.Count ?? 0);
            return Ok(products);
        }

        [HttpGet("{id}")]
        public IActionResult GetProduct(int id)
        {
            _logger.LogInformation("[ProductJsonController][GetProduct] Fetching JSON product with ID {Id}", id);
            var product = _service.GetById(id);

            if (product == null)
            {
                _logger.LogWarning("[ProductJsonController][GetProduct] Product with ID {Id} not found.", id);
                return NotFound();
            }

            _logger.LogInformation("[ProductJsonController][GetProduct] Found product {Name} (ID {Id})", product.Name, product.Id);
            return Ok(product);
        }

        [HttpPost]
        public IActionResult CreateProduct([FromBody] Product product, [FromHeader] string role)
        {
            _logger.LogInformation("[ProductJsonController][CreateProduct] Attempting to create product {Name}", product?.Name);

            if (role != "admin")
            {
                _logger.LogWarning("[ProductJsonController][CreateProduct] Unauthorized attempt by role {Role}", role);
                return Unauthorized("Only admin can add products.");
            }

            var created = _service.Create(product);
            _logger.LogInformation("[ProductJsonController][CreateProduct] Product {Name} created successfully with ID {Id}", created.Name, created.Id);
            return CreatedAtAction(nameof(GetProduct), new { id = created.Id }, created);
        }

        [HttpPut("{id}")]
        public IActionResult UpdateProduct(int id, [FromBody] Product updatedProduct, [FromHeader] string role)
        {
            _logger.LogInformation("[ProductJsonController][UpdateProduct] Updating product with ID {Id}", id);

            if (role != "admin")
            {
                _logger.LogWarning("[ProductJsonController][UpdateProduct] Unauthorized update attempt by role {Role}", role);
                return Unauthorized("Only admin can update products.");
            }

            try
            {
                _service.Update(id, updatedProduct);
                _logger.LogInformation("[ProductJsonController][UpdateProduct] Product with ID {Id} updated successfully.", id);
                return Ok(new { message = "Product updated successfully." });
            }
            catch (KeyNotFoundException)
            {
                _logger.LogWarning("[ProductJsonController][UpdateProduct] Product with ID {Id} not found for update.", id);
                return NotFound();
            }
        }

        [HttpPut("bulk-update")]
        public IActionResult UpdateMultipleProducts([FromBody] List<Product> updatedProducts, [FromHeader] string role)
        {
            _logger.LogInformation("[ProductJsonController][BulkUpdate] Bulk update request received for {Count} products", updatedProducts?.Count ?? 0);

            if (role != "admin")
            {
                _logger.LogWarning("[ProductJsonController][BulkUpdate] Unauthorized bulk update attempt by role {Role}", role);
                return Unauthorized("Only admin can update products.");
            }

            _service.BulkUpdate(updatedProducts);
            _logger.LogInformation("[ProductJsonController][BulkUpdate] Bulk update completed successfully.");
            return Ok("Products updated successfully.");
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteProduct(int id, [FromHeader] string role)
        {
            _logger.LogInformation("[ProductJsonController][DeleteProduct] Delete request for product ID {Id}", id);

            if (role != "admin")
            {
                _logger.LogWarning("[ProductJsonController][DeleteProduct] Unauthorized delete attempt by role {Role}", role);
                return Unauthorized("Only admin can delete products.");
            }

            try
            {
                _service.Delete(id);
                _logger.LogInformation("[ProductJsonController][DeleteProduct] Product with ID {Id} deleted successfully.", id);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                _logger.LogWarning("[ProductJsonController][DeleteProduct] Product with ID {Id} not found for deletion.", id);
                return NotFound();
            }
        }

        [HttpPost("{id}/buy")]
        public IActionResult BuyProduct(int id, [FromQuery] string role, [FromQuery] int quantity = 1)
        {
            _logger.LogInformation("[ProductJsonController][BuyProduct] Buy request received for Product ID {Id}, Quantity {Qty}, Role {Role}", id, quantity, role);

            if (role != "customer")
            {
                _logger.LogWarning("[ProductJsonController][BuyProduct] Unauthorized buy attempt by role {Role}", role);
                return Unauthorized("Only customer can buy products.");
            }

            try
            {
                var product = _service.Buy(id, quantity);
                _logger.LogInformation("[ProductJsonController][BuyProduct] Purchase successful. Product ID {Id}, Remaining Quantity {Qty}", product.Id, product.Quantity);
                return Ok(new { product.Id, product.Name, product.Quantity });
            }
            catch (KeyNotFoundException)
            {
                _logger.LogWarning("[ProductJsonController][BuyProduct] Product with ID {Id} not found during purchase.", id);
                return NotFound();
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogError(ex, "[ProductJsonController][BuyProduct] Error during product purchase for product ID {Id}", id);
                return BadRequest(ex.Message);
            }
        }
    }
}
