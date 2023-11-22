using Microsoft.EntityFrameworkCore;
using RepairCardsMVC.Models;
using RepairCardsMVC.ViewModels;

namespace RepairCardsMVC.Services
{
    public class ProductService
    {
        private readonly ApplicationDbContext _db;

        public ProductService(ApplicationDbContext db)
        {
            _db = db;
        }

        public DataHelperViewModel<Product> GetAll(HttpRequest request)
            => request.GetDataForJQueryDataTable(_db.Products.AsQueryable(), "Code", "Name");

        public async Task<IEnumerable<Product>> GetAll(string query, int count)
        {
            var items = await _db.Products.Where(x => x.Code.Contains(query) || x.Name.Contains(query))
            .Take(count)
                .Select(x => new Product { Code = x.Code, Name = x.Name })
                .ToListAsync();

            return items;
        }

        public IEnumerable<Relation> GetProductRelations(string code)
            => _db.GetProductRelations(code);

        public IEnumerable<Relation> GetPurchasedProductsByProduct(string code)
            => _db.GetProductRelations(code, true);

        public string? GetPurchasedProductRoute(string code)
            => _db.GetPurchasedProductRoute(code);
    }
}
