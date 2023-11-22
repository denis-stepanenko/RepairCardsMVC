using RepairCardsMVC.Models;

namespace RepairCardsMVC.Services
{
    public class ProductOperationService
    {
        private readonly ApplicationDbContext _db;

        public ProductOperationService(ApplicationDbContext db)
        {
            _db = db;
        }
        public IEnumerable<ProductOperation> GetAll(string productCode, string route)
            => _db.GetProductOperations(productCode, route);
    }
}
