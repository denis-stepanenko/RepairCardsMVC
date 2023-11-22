using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using RepairCardsMVC.Exceptions;
using RepairCardsMVC.Models;

namespace RepairCardsMVC.Services
{
    public class CardPurchasedProductOperationService
    {
        private readonly ApplicationDbContext _db;

        public CardPurchasedProductOperationService(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<CardPurchasedProductOperation?> Get(int id)
            => await _db.CardPurchasedProductOperations
                .FirstOrDefaultAsync(x => x.Id == id);

        public async Task AddRange(List<int> ids, int productId, int executorId, DateTime date)
        {
            var product = await _db.CardPurchasedProducts.FindAsync(productId);

            if (product == null)
                throw new NotFoundException();

            var route = _db.GetPurchasedProductRoute(product.Code);

            if(route == null)
                throw new NotFoundException();

            var operations = _db.GetProductOperations(product.Code, route)
                .Where(x => ids.Contains(x.Id));

            var items = operations.Select(x => new CardPurchasedProductOperation
            {
                CardPurchasedProductId = productId,
                Code = x.Code,
                Name = x.Name,
                Labor = x.Labor,
                Department = x.Department,
                ExecutorId = executorId,
                Date = date
            }).ToList();

            _db.CardPurchasedProductOperations.AddRange(items);
            await _db.SaveChangesAsync();
        }

        public async Task Edit(int id, int executorId, DateTime date)
        {
            var item = await _db.CardPurchasedProductOperations.FindAsync(id);

            if (item == null)
                throw new NotFoundException();

            item.ExecutorId = executorId;
            item.Date = date;

            _db.CardPurchasedProductOperations.Update(item);
            _db.SaveChanges();
        }

        public async Task Remove(int id)
        {
            var item = await _db.CardPurchasedProductOperations.FindAsync(id);

            if (item == null)
                throw new NotFoundException();

            _db.CardPurchasedProductOperations.Remove(item);
            await _db.SaveChangesAsync();
        }

        public async Task Confirm(int id, string userId, string userName)
        {
            var item = await _db.CardPurchasedProductOperations.FindAsync(id);

            if (item == null)
                throw new NotFoundException();

            if (item.ConfirmUserId == null)
            {

            }
        }

        public async Task Unconfirm(int id, string userId)
        {
            var item = await _db.CardPurchasedProductOperations.FindAsync(id);

            if (item == null)
                throw new NotFoundException();

            if (item.ConfirmUserId != null)
            {

            }
        }
    }
}
