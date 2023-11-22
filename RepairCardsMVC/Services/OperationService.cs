using Microsoft.EntityFrameworkCore;
using RepairCardsMVC.Exceptions;
using RepairCardsMVC.Models;
using RepairCardsMVC.ViewModels;

namespace RepairCardsMVC.Services
{
    public class OperationService
    {
        private readonly ApplicationDbContext _db;

        public OperationService(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<PaginatedList<Operation>> GetAll(int pageNumber, string filter)
        {
            var items = _db.Operations
                .Include(x => x.ProductionOperation)
                .Where(x =>
                x.Name.Contains(filter) ||
                x.Department.ToString().Contains(filter) ||
                (x.ProductionOperationCode ?? "").Contains(filter) ||
                (x.ProductionOperation.Name ?? "").Contains(filter));

            return await PaginatedList<Operation>.CreateAsync(items, pageNumber);
        }

        public DataHelperViewModel<Operation> GetAll(HttpRequest request)
            => request.GetDataForJQueryDataTable(_db.Operations.AsQueryable(), "Code", "Name");

        public async Task<Operation?> Get(int id)
            => await _db.Operations.FindAsync(id);

        public async Task Add(Operation item)
        {
            _db.Operations.Add(item);
            await _db.SaveChangesAsync();
        }

        public async Task Edit(Operation item)
        {
            _db.Operations.Update(item);
            await _db.SaveChangesAsync();
        }

        public async Task Remove(int id)
        {
            var item = await _db.Operations.FindAsync(id);

            if (item == null)
                throw new NotFoundException();

            _db.Operations.Remove(item);
            await _db.SaveChangesAsync();
        }

        public async Task<string> GenerateNewCode()
            => ((await _db.Operations.MaxAsync(x => Convert.ToInt32(x.Code))) + 1).ToString("D6");

        public async Task<bool> IsThereOperationWithSuchCode(string code, int id)
            => await _db.Operations.AnyAsync(x => x.Code == code && x.Id != id);
    }
}
