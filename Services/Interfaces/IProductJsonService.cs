using KiranaStore.Models;

namespace KiranaStore.Services.Interfaces
{
    public interface IProductJsonService
    {
        List<Product> GetAll();
        Product? GetById(int id);
        Product Create(Product product);
        void Update(int id, Product updatedProduct);
        void BulkUpdate(List<Product> updatedProducts);
        void Delete(int id);
        Product Buy(int id, int quantity);
    }
}
