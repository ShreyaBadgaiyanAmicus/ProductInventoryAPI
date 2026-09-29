using ProductInventoryAPI.Data;
using ProductInventoryAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace ProductInventoryAPI.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly AppDbContext _context;

        public ProductRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Product>> GetAll()
        {
            return await _context.Products
                .Include(p => p.Suppliers)
                .ToListAsync();
        }

        public async Task<Product> GetById(int id)
        {
            return await _context.Products
                .Include(p => p.Suppliers)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task Add(Product product)
        {
            _context.Products.Add(product);
            await _context.SaveChangesAsync();
        }

        public async Task AddRange(List<Product> products)
        {
            _context.Products.AddRange(products);
            await _context.SaveChangesAsync();
        }
        public async Task Update(Product product)
        {
            _context.Products.Update(product);
            await _context.SaveChangesAsync();
        }

        public async Task Delete(Product product)
        {
            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
        }

        public async Task<List<Product>> Search(string query)
        {
            return await _context.Products
                .Include(p => p.Suppliers)
                .Where(p =>
                    p.Name.Contains(query) ||
                    p.Description.Contains(query) ||
                    _context.Categories.Any(c =>
                        c.Id == p.CategoryId &&
                        c.Name.Contains(query)))
                .ToListAsync();
        }
    }
}