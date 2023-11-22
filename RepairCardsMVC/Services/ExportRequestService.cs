using RepairCardsMVC.Exceptions;
using RepairCardsMVC.Models;

namespace RepairCardsMVC.Services
{
    public class ExportRequestService
    {
        private readonly ApplicationDbContext _db;

        public ExportRequestService(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<PaginatedList<ExportRequest>> GetAll(int pageNumber, string filter)
        {
            var items = _db.ExportRequests
                .Where(x => x.CardNumber.Contains(filter))
                .OrderByDescending(x => x.Id);

            return await PaginatedList<ExportRequest>.CreateAsync(items, pageNumber);
        }

        public async Task Add(int cardId)
        {
            var card = await _db.Cards.FindAsync(cardId);

            if (card == null)
                throw new NotFoundException();

            var item = new ExportRequest
            {
                CardNumber = card.Number,
                Department = card.Department,
                Date = DateTime.Now
            };

            _db.ExportRequests.Add(item);
            _db.SaveChanges();
        }

        public async Task Remove(int id)
        {
            var item = await _db.ExportRequests.FindAsync(id);

            if (item == null)
                throw new NotFoundException();

            _db.ExportRequests.Remove(item);

            await _db.SaveChangesAsync();
        }

        public async Task CloseApplication(int id)
        {
            var item = await _db.ExportRequests.FindAsync(id);

            if (item == null)
                throw new NotFoundException();

            item.CloseDate = DateTime.Now;

            _db.ExportRequests.Update(item);

            await _db.SaveChangesAsync();
        }

        public async Task CancelApplicationClosing(int id)
        {
            var item = await _db.ExportRequests.FindAsync(id);

            if (item == null)
                throw new NotFoundException();

            item.CloseDate = null;

            _db.ExportRequests.Update(item);

            await _db.SaveChangesAsync();
        }

        public async Task SetDeficitCreationDate(int id)
        {
            var item = await _db.ExportRequests.FindAsync(id);

            if (item == null)
                throw new NotFoundException();

            if (item.DeficitCreationDate == null)
                item.DeficitCreationDate = DateTime.Now;
            else
                item.DeficitCreationDate = null;

            _db.ExportRequests.Update(item);

            await _db.SaveChangesAsync();
        }
    }
}
