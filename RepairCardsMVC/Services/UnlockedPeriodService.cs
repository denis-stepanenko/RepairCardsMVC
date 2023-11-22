using Microsoft.EntityFrameworkCore;
using RepairCardsMVC.Exceptions;
using RepairCardsMVC.Models;

namespace RepairCardsMVC.Services
{
    public class UnlockedPeriodService
    {
        private readonly ApplicationDbContext _db;

        public UnlockedPeriodService(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<PaginatedList<UnlockedPeriod>> GetAll(int pageNumber, string filter)
        {
            var items = _db.UnlockedPeriods
                .Include(x => x.Card)
                .Where(x => x.Card != null && x.Card.Number.Contains(filter));

            return await PaginatedList<UnlockedPeriod>.CreateAsync(items, pageNumber, 10);
        }

        public async Task<UnlockedPeriod?> Get(int id)
            => await _db.UnlockedPeriods
                .Include(x => x.Card)
                .FirstOrDefaultAsync(x => x.Id == id);

        public async Task Edit(UnlockedPeriod item)
        {
            _db.UnlockedPeriods.Update(item);
            await _db.SaveChangesAsync();
        }

        public async Task Add(UnlockedPeriod item)
        {
            _db.UnlockedPeriods.Add(item);
            await _db.SaveChangesAsync();
        }

        public async Task Remove(int id)
        {
            var item = await _db.UnlockedPeriods.FindAsync(id);

            if (item == null)
                throw new NotFoundException();

            _db.UnlockedPeriods.Remove(item);

            await _db.SaveChangesAsync();
        }
    }
}
