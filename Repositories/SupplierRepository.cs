using Microsoft.EntityFrameworkCore;
using ProductInventoryAPI.Data;
using ProductInventoryAPI.Models;

namespace ProductInventoryAPI.Repositories
{
    public class SupplierRepository : ISupplierRepository
    {
        private readonly AppDbContext _context;

        public SupplierRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<bool> AllExist(List<int> supplierIds)
        {
            var count = await _context.Suppliers
                .CountAsync(s => supplierIds.Contains(s.Id));

            return count == supplierIds.Count;
        }

        public async Task<List<Supplier>> GetByIds(List<int> supplierIds)
        {
            return await _context.Suppliers
                .Where(s => supplierIds.Contains(s.Id))
                .ToListAsync();
        }
    }
}