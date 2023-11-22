using Microsoft.EntityFrameworkCore;
using RepairCardsMVC.Exceptions;
using RepairCardsMVC.Models;

namespace RepairCardsMVC.Services
{
    public class CardMaterialService
    {
        private readonly ApplicationDbContext _db;

        public CardMaterialService(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<PaginatedList<CardMaterial>> GetAll(int cardId, int pageNumber, string filter)
        {
            var items = _db.CardMaterials
                .Where(x => x.CardId == cardId && x.Name.Contains(filter))
                .OrderByDescending(x => x.Id);

            return await PaginatedList<CardMaterial>.CreateAsync(items, pageNumber);
        }

        public async Task<CardMaterial?> Get(int id)
            => await _db.CardMaterials.FindAsync(id);

        public async Task Edit(int id, decimal count)
        {
            var oldItem = await _db.CardMaterials.FindAsync(id);

            if (oldItem == null)
                throw new NotFoundException();

            oldItem.Count = count;

            _db.CardMaterials.Update(oldItem);
            await _db.SaveChangesAsync();
        }

        public async Task AddRange(List<int> ids, int cardId, int count, int department)
        {
            var result = new List<CardMaterial>();

            foreach (var id in ids)
            {
                var material = await _db.Materials.FindAsync(id);

                if (material == null)
                    continue;

                result.Add(new CardMaterial
                {
                    CardId = cardId,
                    Code = material.Code,
                    Name = material.Name,
                    Size = material.Size,
                    Type = material.Type,
                    Count = count,
                    Price = material.Price,
                    Department = department
                });
            }

            _db.CardMaterials.AddRange(result);
            await _db.SaveChangesAsync();
        }

        public async Task AddRange(List<CardMaterial> items, int cardId)
        {
            var result = new List<CardMaterial>();

            foreach (var item in items)
            {
                var material = await _db.Materials.FirstOrDefaultAsync(x => x.Code == item.Code);

                if(material == null) 
                    continue;

                result.Add(new CardMaterial
                {
                    CardId = cardId,
                    Code = material.Code,
                    Name = material.Name,
                    Size = material.Size,
                    Type = material.Type,
                    Count = item.Count,
                    Department = item.Department,
                    UnitId = item.UnitId
                });
            }
            
            _db.CardMaterials.AddRange(result);
            await _db.SaveChangesAsync();
        }

        public async Task AddRange(List<ProductMaterial> items, int cardId, int department)
        {
            if (department == 0)
                throw new BusinessLogicException("Укажите подразделение");

            var result = new List<CardMaterial>();

            foreach (var item in items)
            {
                var material = await _db.Materials.FindAsync(item.Id);

                if(material == null)
                    continue;

                result.Add(new CardMaterial
                {
                    CardId = cardId,
                    Code = material.Code,
                    Name = material.Name,
                    Size = material.Size,
                    Type = material.Type,
                    Count = item.Count,
                    Department = department,
                    UnitId = item.UnitId
                });
            }

            _db.CardMaterials.AddRange(result);
            await _db.SaveChangesAsync();
        }

        public async Task AddByAllOwnProductsInCard(int cardId, int department)
        {
            var products = await _db.CardOwnProducts
                .Where(x => x.CardId == cardId)
                .ToListAsync();

            var cardMaterials = new List<CardMaterial>();

            foreach (var product in products)
            {
                var materials = await _db.GetMaterialsByProduct(product.Code);

                foreach (var material in materials)
                {
                    cardMaterials.Add(new CardMaterial
                    {
                        CardId = cardId,
                        Code = material.Code,
                        Name = material.Name,
                        Size = material.Size,
                        Type = material.Type,
                        Count = material.Count * product.Count,
                        Price = material.Price,
                        Department = department,
                        UnitId = material.UnitId
                    });
                }
            }

            var groupedCardMaterials =
                cardMaterials.GroupBy(x => new { x.CardId, x.Code, x.Name, x.Size, x.Type, x.Price, x.Department, x.UnitId })
                .Select(x => new CardMaterial
                {
                    CardId = x.Key.CardId,
                    Code = x.Key.Code,
                    Name = x.Key.Name,
                    Size = x.Key.Size,
                    Type = x.Key.Type,
                    Price = x.Key.Price,
                    Department = x.Key.Department,
                    UnitId = x.Key.UnitId,
                    Count = x.Sum(m => m.Count)
                });

            _db.CardMaterials.AddRange(groupedCardMaterials);

            await _db.SaveChangesAsync();
        }

        public async Task Remove(int id)
        {
            var item = await _db.CardMaterials.FindAsync(id);

            if (item == null)
                throw new NotFoundException();

            _db.CardMaterials.Remove(item);
            await _db.SaveChangesAsync();
        }

        public async Task<IEnumerable<CardMaterial>> GetMaterialsByCard(int cardId)
            => await _db.CardMaterials.Where(x => x.CardId == cardId).ToListAsync();
    }
}
