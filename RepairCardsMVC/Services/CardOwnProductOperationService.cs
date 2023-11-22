using Microsoft.EntityFrameworkCore;
using RepairCardsMVC.Exceptions;
using RepairCardsMVC.Models;

namespace RepairCardsMVC.Services
{
    public class CardOwnProductOperationService
    {
        private readonly ApplicationDbContext _db;

        public CardOwnProductOperationService(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<CardOwnProductOperation?> Get(int id)
            => await _db.CardOwnProductOperations
                .Include(x => x.Executor)
                .FirstOrDefaultAsync(x => x.Id == id);

        public async Task AddByProduct(List<int> ids, int productId, int executorId, DateTime date)
        {
            var item = await _db.CardOwnProducts.FindAsync(productId);

            if (item == null)
                throw new NotFoundException();

            var selectedOperations = _db.GetProductOperations(item.Code, item.Route ?? "")
                .Where(x => ids.Contains(x.Id));

            var items = selectedOperations.Select(x => new CardOwnProductOperation
            {
                CardOwnProductId = productId,
                Code = x.Code,
                Name = x.Name,
                Labor = x.Labor,
                Department = x.Department,
                ExecutorId = executorId,
                Date = date
            }).ToList();

            _db.CardOwnProductOperations.AddRange(items);
            _db.SaveChanges();
        }

        public async Task Edit(int id, DateTime date, int executorId)
        {
            var item = _db.CardOwnProductOperations.Find(id);

            if (item == null)
                throw new NotFoundException();

            item.Date = date;
            item.ExecutorId = executorId;

            _db.CardOwnProductOperations.Update(item);
            await _db.SaveChangesAsync();
        }

        public async Task Remove(int id)
        {
            var item = await _db.CardOwnProductOperations.FindAsync(id);

            if (item == null)
                throw new NotFoundException();

            _db.CardOwnProductOperations.Remove(item);
            await _db.SaveChangesAsync();
        }

        public async Task Confirm(int id, string userId, string userName)
        {
            var item = await _db.CardOwnProductOperations.FindAsync(id);

            if (item == null)
                throw new NotFoundException();

            if (item.ConfirmUserId == null)
            {

            }
        }

        public async Task Unconfirm(int id, string userId)
        {
            var item = await _db.CardOwnProductOperations.FindAsync(id);

            if (item == null)
                throw new NotFoundException();

            if (item.ConfirmUserId != null)
            {

            }
        }
    }
}
