using RepairCardsMVC.Exceptions;
using RepairCardsMVC.Models;

namespace RepairCardsMVC.Services
{
    public class RequestToCreateStagesIn1SService
    {
        private readonly ApplicationDbContext _db;

        public RequestToCreateStagesIn1SService(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<IEnumerable<RequestToCreateStagesIn1S>> GetAll(int pageNumber, string filter)
        {
            var items = _db.RequestsToCreateStagesIn1S
                .Where(x => x.CardNumber.Contains(filter))
                .OrderByDescending(x => x.Id);

            return await PaginatedList<RequestToCreateStagesIn1S>.CreateAsync(items, pageNumber);
        }

        public async Task Add(int id)
        {
            var card = await _db.Cards.FindAsync(id);

            if (card == null)
                throw new NotFoundException();

            var item = new RequestToCreateStagesIn1S
            {
                CardNumber = card.Number,
                Department = card.Department,
                Date = DateTime.Now
            };

            _db.RequestsToCreateStagesIn1S.Add(item);
            await _db.SaveChangesAsync();
        }

        public async Task Remove(int id)
        {
            var item = await _db.RequestsToCreateStagesIn1S.FindAsync(id);

            if(item == null)
                throw new NotFoundException();

            _db.RequestsToCreateStagesIn1S.Remove(item);

            await _db.SaveChangesAsync();
        }

        public async Task CloseApplication(int id)
        {
            var item = await _db.RequestsToCreateStagesIn1S.FindAsync(id);

            if (item == null)
                throw new NotFoundException();

            item.CloseDate = DateTime.Now;

            _db.RequestsToCreateStagesIn1S.Update(item);

            await _db.SaveChangesAsync();
        }

        public async Task CancelApplicationClosing(int id)
        {
            var item = await _db.RequestsToCreateStagesIn1S.FindAsync(id);

            if (item == null)
                throw new NotFoundException();

            item.CloseDate = null;

            _db.RequestsToCreateStagesIn1S.Update(item);

            await _db.SaveChangesAsync();
        }
    }
}
