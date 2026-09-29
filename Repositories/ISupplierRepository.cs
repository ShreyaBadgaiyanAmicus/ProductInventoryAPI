using ProductInventoryAPI.Models;

namespace ProductInventoryAPI.Repositories
{
    public interface ISupplierRepository
    {
        Task<bool> AllExist(List<int> supplierIds);

        Task<List<Supplier>> GetByIds(List<int> supplierIds);
    }
}