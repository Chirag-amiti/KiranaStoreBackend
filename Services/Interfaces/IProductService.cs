using KiranaStore.Models;

namespace KiranaStore.Services.Interfaces
{
    public interface IProductService
    {
        Task<List<Product>> GetAllAsync();
        Task<Product?> GetByIdAsync(int id);
        Task<Product> CreateAsync(Product product);
        Task UpdateAsync(int id, Product updatedProduct);
        Task BulkUpdateAsync(List<Product> updatedProducts);
        Task DeleteAsync(int id);
        Task<Product> BuyAsync(int id, int quantity);
    }
}
