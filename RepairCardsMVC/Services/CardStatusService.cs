using Microsoft.EntityFrameworkCore;
using RepairCardsMVC.Exceptions;
using RepairCardsMVC.Models;

namespace RepairCardsMVC.Services
{
    public class CardStatusService
    {
        private readonly ApplicationDbContext _db;

        public CardStatusService(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<CardStatus?> Get(int id)
            => await _db.CardStatuses.FindAsync(id);

        public async Task<IEnumerable<CardStatus>> GetAll()
            => await _db.CardStatuses.ToListAsync();

        public async Task<PaginatedList<CardStatus>> GetAll(int pageNumber, string filter)
        {
            var items = _db.CardStatuses.Where(x => x.Name.Contains(filter));

            return await PaginatedList<CardStatus>.CreateAsync(items, pageNumber);
        }

        public async Task Add(CardStatus item)
        {
            _db.CardStatuses.Add(item);
            await _db.SaveChangesAsync();
        }

        public async Task Edit(CardStatus item)
        {
            _db.CardStatuses.Update(item);
            await _db.SaveChangesAsync();
        }

        public async Task Remove(int id)
        {
            var item = await _db.CardStatuses.FindAsync(id);

            if (item != null)
                throw new NotFoundException();

            _db.CardStatuses.Remove(item);
            await _db.SaveChangesAsync();
        }
    }
}
