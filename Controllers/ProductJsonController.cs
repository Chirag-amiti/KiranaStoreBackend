using Microsoft.AspNetCore.Mvc;
using KiranaStore.Models;
using KiranaStore.Helpers;
using KiranaStore.Helpers.Logger;

namespace KiranaStore.Controllers
{
    [ApiController]
    [Route("api/json/[controller]")]
    public class ProductJsonController : ControllerBase
    {
        [HttpGet]
        public IActionResult GetProducts()
        {
            AppLogger.Info("Fetching all products from JSON file.");
            var products = JsonHelper.ReadJson<Product>();
            AppLogger.Info($"Total products fetched from JSON: {products.Count}");
            return Ok(products);
        }

        [HttpGet("{id}")]
        public IActionResult GetProduct(int id)
        {
            AppLogger.Info($"Fetching JSON product with ID: {id}");
            var products = JsonHelper.ReadJson<Product>();
            var product = products.FirstOrDefault(p => p.Id == id);
            if (product == null)
            {
                AppLogger.Warning($"JSON product with ID: {id} not found.");
                return NotFound();
            }

            AppLogger.Info($"JSON product fetched successfully: {product.Name}");
            return Ok(product);
        }

        [HttpPost]
        public IActionResult CreateProduct([FromBody] Product product, [FromHeader] string role)
        {
            AppLogger.Info($"Attempt to create JSON product '{product.Name}' by role: {role}");

            if (role != "admin")
            {
                AppLogger.Warning("Unauthorized JSON product creation attempt.");
                return Unauthorized("Only admin can add products.");
            }

            var products = JsonHelper.ReadJson<Product>();

            if (product.Id == 0)
                product.Id = products.Any() ? products.Max(p => p.Id) + 1 : 1;

            products.Add(product);
            JsonHelper.WriteJson(products);

            AppLogger.Info($"JSON product created successfully: {product.Name} (ID: {product.Id})");
            return CreatedAtAction(nameof(GetProduct), new { id = product.Id }, product);
        }

        [HttpPut("{id}")]
        public IActionResult UpdateProduct(int id, [FromBody] Product updatedProduct, [FromHeader] string role)
        {
            AppLogger.Info($"Update requested for JSON product ID: {id} by {role}");

            if (role != "admin")
            {
                AppLogger.Warning("Unauthorized JSON product update attempt.");
                return Unauthorized("Only admin can update products.");
            }

            var products = JsonHelper.ReadJson<Product>();
            var product = products.FirstOrDefault(p => p.Id == id);
            if (product == null)
            {
                AppLogger.Warning($"JSON product with ID: {id} not found for update.");
                return NotFound();
            }

            product.Name = updatedProduct.Name;
            product.Price = updatedProduct.Price;
            product.Quantity = updatedProduct.Quantity;

            JsonHelper.WriteJson(products);
            AppLogger.Info($"JSON product updated successfully: {product.Name} (ID: {product.Id})");
            return Ok(new { message = "Product updated successfully." });
        }

        [HttpPut("bulk-update")]
        public IActionResult UpdateMultipleProducts([FromBody] List<Product> updatedProducts, [FromHeader] string role)
        {
            AppLogger.Info($"Bulk update requested for JSON products by {role}");

            if (role != "admin")
            {
                AppLogger.Warning("Unauthorized bulk update on JSON file.");
                return Unauthorized("Only admin can update products.");
            }

            var products = JsonHelper.ReadJson<Product>();

            foreach (var updatedProduct in updatedProducts)
            {
                var product = products.FirstOrDefault(p => p.Id == updatedProduct.Id);
                if (product == null)
                {
                    AppLogger.Warning($"JSON product with ID: {updatedProduct.Id} not found during bulk update.");
                    continue;
                }

                product.Name = updatedProduct.Name;
                product.Price = updatedProduct.Price;
                product.Quantity = updatedProduct.Quantity;
            }

            JsonHelper.WriteJson(products);
            AppLogger.Info("Bulk JSON product update completed successfully.");
            return Ok("Products updated successfully.");
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteProduct(int id, [FromHeader] string role)
        {
            AppLogger.Info($"Delete requested for JSON product ID: {id} by {role}");

            if (role != "admin")
            {
                AppLogger.Warning("Unauthorized JSON product delete attempt.");
                return Unauthorized("Only admin can delete products.");
            }

            var products = JsonHelper.ReadJson<Product>();
            var product = products.FirstOrDefault(p => p.Id == id);
            if (product == null)
            {
                AppLogger.Warning($"JSON product with ID: {id} not found for deletion.");
                return NotFound();
            }

            products.Remove(product);
            JsonHelper.WriteJson(products);

            AppLogger.Info($"JSON product deleted successfully: {product.Name} (ID: {product.Id})");
            return NoContent();
        }

        [HttpPost("{id}/buy")]
        public IActionResult BuyProduct(int id, [FromQuery] string role, [FromQuery] int quantity = 1)
        {
            AppLogger.Info($"Buy request for JSON product ID: {id}, quantity: {quantity}, role: {role}");

            if (role != "customer")
            {
                AppLogger.Warning("Unauthorized JSON buy attempt.");
                return Unauthorized("Only customer can buy products.");
            }

            var products = JsonHelper.ReadJson<Product>();
            var product = products.FirstOrDefault(p => p.Id == id);
            if (product == null)
            {
                AppLogger.Warning($"JSON product with ID: {id} not found for purchase.");
                return NotFound();
            }
            if (product.Quantity < quantity)
            {
                AppLogger.Warning($"Insufficient stock in JSON for product ID: {id}. Requested: {quantity}, Available: {product.Quantity}");
                return BadRequest("Not enough stock.");
            }

            product.Quantity -= quantity;
            JsonHelper.WriteJson(products);

            AppLogger.Info($"JSON product purchased successfully: {product.Name}, Remaining Qty: {product.Quantity}");
            return Ok(new { product.Id, product.Name, product.Quantity });
        }
    }
}
