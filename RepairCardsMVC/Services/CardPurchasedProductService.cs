using Microsoft.EntityFrameworkCore;
using RepairCardsMVC.Exceptions;
using RepairCardsMVC.Models;

namespace RepairCardsMVC.Services
{
    public class CardPurchasedProductService
    {
        private readonly ApplicationDbContext _db;

        public CardPurchasedProductService(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<IEnumerable<CardPurchasedProduct>> GetAll(int id)
            => await _db.CardPurchasedProducts.Where(x => x.CardId == id).ToListAsync();

        public async Task<CardPurchasedProduct?> Get(int id)
            => await _db.CardPurchasedProducts
                .Include(x => x.Operations)
                    .ThenInclude(x => x.Executor)
                .FirstOrDefaultAsync(x => x.Id == id);

        public async Task Edit(int id, int count)
        {
            var item = await _db.CardPurchasedProducts.FindAsync(id);

            if (item == null)
                throw new NotFoundException();

            item.Count = count;

            _db.CardPurchasedProducts.Update(item);
            await _db.SaveChangesAsync();
        }

        public async Task Remove(int id)
        {
            var item = await _db.CardPurchasedProducts.FindAsync(id);

            if (item == null)
                throw new NotFoundException();

            _db.CardPurchasedProducts.Remove(item);
            await _db.SaveChangesAsync();
        }

        public async Task AddRange(List<int> ids, int cardId)
        {
            var result = new List<CardPurchasedProduct>();

            foreach (var id in ids)
            {
                var product = await _db.CardPurchasedProducts.FindAsync(id);

                if (product == null)
                    continue;

                result.Add(new CardPurchasedProduct
                {
                    CardId = cardId,
                    Code = product.Code,
                    Name = product.Name,
                    Count = product.Count
                });
            }

            _db.CardPurchasedProducts.AddRange(result);
            await _db.SaveChangesAsync();
        }

        public async Task AddRange(List<Relation> items, int cardId)
        {
            var result = new List<CardPurchasedProduct>();

            foreach (var item in items)
            {
                var product = await _db.PurchasedProducts.FirstOrDefaultAsync(x => x.Code == item.Code);

                if(product == null)
                    continue;

                result.Add(new CardPurchasedProduct
                {
                    CardId = cardId,
                    Code = item.Code,
                    Name = item.Name,
                    Count = (int)item.Count
                });
            }

            _db.CardPurchasedProducts.AddRange(result);
            await _db.SaveChangesAsync();
        }
    }
}
