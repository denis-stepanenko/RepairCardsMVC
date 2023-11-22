using Microsoft.EntityFrameworkCore;
using RepairCardsMVC.Exceptions;
using RepairCardsMVC.Models;
using System.Linq.Dynamic.Core;

namespace RepairCardsMVC.Services
{
    public class ExecutorService
    {
        private readonly ApplicationDbContext _db;

        public ExecutorService(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<Executor?> Get(int id)
            => await _db.Executors.FindAsync(id);

        public async Task<IEnumerable<Executor>> GetAll(string query, int count)
            => await _db.Executors.Where(x => x.Name.Contains(query)).ToListAsync();

        public async Task<PaginatedList<Executor>> GetAll(int pageNumber, string filter)
        {
            var items = _db.Executors.Where(x =>
                x.Name.Contains(filter) ||
                x.Department.ToString().Contains(filter)
                );

            return await PaginatedList<Executor>.CreateAsync(items, pageNumber);
        }

        public async Task Add(Executor item)
        {
            _db.Executors.Add(item);
            await _db.SaveChangesAsync();
        }

        public async Task Edit(Executor item)
        {
            _db.Executors.Update(item);
            await _db.SaveChangesAsync();
        }

        public async Task Remove(int id)
        {
            var item = await _db.Executors.FindAsync(id);

            if (item == null)
                throw new NotFoundException();

            _db.Executors.Remove(item);
            await _db.SaveChangesAsync();
        }
    }
}
