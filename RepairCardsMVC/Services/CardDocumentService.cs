using RepairCardsMVC.Exceptions;
using RepairCardsMVC.Models;

namespace RepairCardsMVC.Services
{
    public class CardDocumentService
    {
        private readonly ApplicationDbContext _db;

        public CardDocumentService(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<PaginatedList<CardDocument>> GetAll(int cardId, int pageNumber, string filter)
        {
            var items = _db.CardDocuments
                .Where(x => x.CardId == cardId && x.Name.Contains(filter))
                .OrderByDescending(x => x.Id);

            return await PaginatedList<CardDocument>.CreateAsync(items, pageNumber);
        }

        public async Task<CardDocument?> Get(int id)
            => await _db.CardDocuments.FindAsync(id);

        public async Task Add(CardDocument item)
        {
            _db.CardDocuments.Add(item);
            await _db.SaveChangesAsync();
        }

        public async Task Edit(CardDocument item)
        {
            _db.CardDocuments.Update(item);
            await _db.SaveChangesAsync();
        }

        public async Task Remove(int id)
        {
            var item = await _db.CardDocuments.FindAsync(id);

            if (item == null)
                throw new NotFoundException();

            _db.CardDocuments.Remove(item);
            await _db.SaveChangesAsync();
        }
    }
}
