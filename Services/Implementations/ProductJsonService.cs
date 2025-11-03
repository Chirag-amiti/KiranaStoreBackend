using KiranaStore.Helpers;
using KiranaStore.Models;
using KiranaStore.Services.Interfaces;

namespace KiranaStore.Services.Implementations
{
    public class ProductJsonService : IProductJsonService
    {
        public List<Product> GetAll()
        {
            return JsonHelper.ReadJson<Product>();
        }

        public Product? GetById(int id)
        {
            var products = JsonHelper.ReadJson<Product>();
            return products.FirstOrDefault(p => p.Id == id);
        }

        public Product Create(Product product)
        {
            var products = JsonHelper.ReadJson<Product>();
            if (product.Id == 0)
                product.Id = products.Any() ? products.Max(p => p.Id) + 1 : 1;

            products.Add(product);
            JsonHelper.WriteJson(products);
            return product;
        }

        public void Update(int id, Product updatedProduct)
        {
            var products = JsonHelper.ReadJson<Product>();
            var product = products.FirstOrDefault(p => p.Id == id);
            if (product == null) throw new KeyNotFoundException($"Product with id {id} not found.");

            product.Name = updatedProduct.Name;
            product.Price = updatedProduct.Price;
            product.Quantity = updatedProduct.Quantity;
            JsonHelper.WriteJson(products);
        }

        public void BulkUpdate(List<Product> updatedProducts)
        {
            if (updatedProducts == null || !updatedProducts.Any()) return;

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
        }

        public void Delete(int id)
        {
            var products = JsonHelper.ReadJson<Product>();
            var product = products.FirstOrDefault(p => p.Id == id);
            if (product == null) throw new KeyNotFoundException($"Product with id {id} not found.");

            products.Remove(product);
            JsonHelper.WriteJson(products);
        }

        public Product Buy(int id, int quantity)
        {
            var products = JsonHelper.ReadJson<Product>();
            var product = products.FirstOrDefault(p => p.Id == id);
            if (product == null) throw new KeyNotFoundException($"Product with id {id} not found.");

            if (product.Quantity < quantity) throw new InvalidOperationException("Not enough stock.");

            product.Quantity -= quantity;
            JsonHelper.WriteJson(products);
            return product;
        }
    }
}
