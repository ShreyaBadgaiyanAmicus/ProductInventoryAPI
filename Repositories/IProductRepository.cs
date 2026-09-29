using ProductInventoryAPI.Models;

namespace ProductInventoryAPI.Repositories
{
    public interface IProductRepository
    {
        Task<List<Product>> GetAll();
        Task<Product> GetById(int id);
        Task Add(Product product);
        Task Update(Product product);
        Task Delete(Product product);
        Task AddRange(List<Product> products);

        Task<List<Product>> Search(string query);
    }
}