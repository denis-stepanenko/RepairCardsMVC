using RepairCardsMVC.Models;

namespace RepairCardsMVC.Services
{
    public class MaterialService
    {
        private readonly ApplicationDbContext _db;

        public MaterialService(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<PaginatedList<Material>> GetAll(int pageNumber, string filter)
        {
            var items = _db.Materials.Where(x => x.Name.Contains(filter));

            return await PaginatedList<Material>.CreateAsync(items, pageNumber);
        }

        public async Task<IEnumerable<ProductMaterial>> GetMaterialsByProduct(string code)
            => await _db.GetMaterialsByProduct(code);
    }
}
