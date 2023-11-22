using Microsoft.EntityFrameworkCore;
using RepairCardsMVC.Exceptions;
using RepairCardsMVC.Models;

namespace RepairCardsMVC.Services
{
    public class RepairTypeService
    {
        private readonly ApplicationDbContext _db;

        public RepairTypeService(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<IEnumerable<RepairType>> GetAll()
            => await _db.RepairTypes.ToListAsync();

        public async Task<PaginatedList<RepairType>> GetAll(int pageNumber, string filter)
        {
            var items = _db.RepairTypes.Where(x => x.Name.Contains(filter));

            return await PaginatedList<RepairType>.CreateAsync(items, pageNumber);
        }

        public async Task<RepairType?> Get(int id)
            => await _db.RepairTypes.FindAsync(id);

        public async Task Edit(RepairType item)
        {
            _db.RepairTypes.Update(item);
            await _db.SaveChangesAsync();
        }

        public async Task Add(RepairType item)
        {
            _db.RepairTypes.Add(item);
            await _db.SaveChangesAsync();
        }

        public async Task Remove(int id)
        {
            var item = await _db.RepairTypes.FindAsync(id);

            if (item == null)
                throw new NotFoundException();

            _db.RepairTypes.Remove(item);
            _db.SaveChanges();
        }
    }
}
