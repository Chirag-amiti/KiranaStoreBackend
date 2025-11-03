using KiranaStore.Data;
using KiranaStore.Models;
using KiranaStore.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace KiranaStore.Services.Implementations
{
    public class ProductService : IProductService
    {
        private readonly KiranaContext _context;

        public ProductService(KiranaContext context)
        {
            _context = context;
        }

        public async Task<List<Product>> GetAllAsync()
        {
            return await _context.Products.ToListAsync();
        }

        public async Task<Product?> GetByIdAsync(int id)
        {
            return await _context.Products.FindAsync(id);
        }

        public async Task<Product> CreateAsync(Product product)
        {
            _context.Products.Add(product);
            await _context.SaveChangesAsync();
            return product;
        }

        public async Task UpdateAsync(int id, Product updatedProduct)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null) throw new KeyNotFoundException($"Product with id {id} not found.");

            product.Name = updatedProduct.Name;
            product.Price = updatedProduct.Price;
            product.Quantity = updatedProduct.Quantity;

            await _context.SaveChangesAsync();
        }

        public async Task BulkUpdateAsync(List<Product> updatedProducts)
        {
            if (updatedProducts == null || !updatedProducts.Any()) return;

            foreach (var updatedProduct in updatedProducts)
            {
                var product = await _context.Products.FindAsync(updatedProduct.Id);
                if (product == null) continue;

                product.Name = updatedProduct.Name;
                product.Price = updatedProduct.Price;
                product.Quantity = updatedProduct.Quantity;
            }

            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null) throw new KeyNotFoundException($"Product with id {id} not found.");

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
        }

        public async Task<Product> BuyAsync(int id, int quantity)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null) throw new KeyNotFoundException($"Product with id {id} not found.");

            if (product.Quantity < quantity) throw new InvalidOperationException("Not enough stock.");

            product.Quantity -= quantity;
            await _context.SaveChangesAsync();
            return product;
        }
    }
}
