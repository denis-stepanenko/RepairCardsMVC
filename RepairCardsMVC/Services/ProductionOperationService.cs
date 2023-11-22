using Microsoft.EntityFrameworkCore;
using RepairCardsMVC.Models;

namespace RepairCardsMVC.Services
{
    public class ProductionOperationService
    {
        private readonly ApplicationDbContext _db;

        public ProductionOperationService(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<ProductionOperation?> Get(string code)
            => await _db.ProductionOperations.FirstOrDefaultAsync(x => x.Code == code);

        public async Task<IEnumerable<ProductionOperation>> GetAll(string query, int count)
            => await _db.ProductionOperations
                .Where(x => x.Code.Contains(query) || x.Name.Contains(query))
                .ToListAsync();
    }
}
