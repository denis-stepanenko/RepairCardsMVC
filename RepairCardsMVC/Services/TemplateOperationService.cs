using Microsoft.EntityFrameworkCore;
using RepairCardsMVC.Exceptions;
using RepairCardsMVC.Models;

namespace RepairCardsMVC.Services
{
    public class TemplateOperationService
    {
        private readonly ApplicationDbContext _db;

        public TemplateOperationService(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<PaginatedList<TemplateOperation>> GetAll(int id, int pageNumber, string filter)
        {
            var items = _db.TemplateOperations
                .Include(x => x.Operation)
                .Where(x => x.TemplateId == id && x.Operation.Name.Contains(filter))
                .OrderBy(x => x.Number);

            return await PaginatedList<TemplateOperation>.CreateAsync(items, pageNumber);
        }

        public async Task<IEnumerable<TemplateOperation>> GetAll(int templateId)
            => await _db.TemplateOperations
                .Include(x => x.Operation)
                .Where(x => x.TemplateId == templateId)
                .ToListAsync();

        public async Task<TemplateOperation?> Get(int id)
            => await _db.TemplateOperations
                .Include(x => x.Operation)
                .FirstOrDefaultAsync(x => x.Id == id);

        public async Task AddRange(List<int> ids, int templateId, int count)
        {
            var items = new List<TemplateOperation>();

            foreach (var id in ids)
            {
                var operation = await _db.Operations.FindAsync(id);

                if (operation == null)
                    continue;

                items.Add(new TemplateOperation
                {
                    TemplateId = templateId,
                    OperationId = operation.Id,
                    Count = count
                });
            }

            foreach (var item in items)
            {
                var maxNumber = await _db.TemplateOperations
                                .Where(o => o.TemplateId == templateId)
                                .MaxAsync(o => (int?)o.Number) ?? 0;

                item.Number = maxNumber + 1;

                _db.TemplateOperations.Add(item);
                await _db.SaveChangesAsync();
            }
        }

        public async Task AddByCard(List<int> ids, int templateId)
        {
            var items = new List<TemplateOperation>();

            foreach (var id in ids)
            {
                var cardOperation = await _db.CardOperations.FindAsync(id);

                if(cardOperation == null) 
                    continue;

                var operation = await _db.Operations.FirstOrDefaultAsync(o => o.Code == cardOperation.Code);

                if(operation == null)
                    continue;

                items.Add(new TemplateOperation
                {
                    TemplateId = templateId,
                    OperationId = operation.Id,
                    Count = cardOperation.Count
                });
            }

            foreach (var item in items)
            {
                var maxNumber = await _db.TemplateOperations
                                .Where(o => o.TemplateId == templateId)
                                .MaxAsync(o => (int?)o.Number) ?? 0;

                item.Number = maxNumber + 1;

                _db.TemplateOperations.Add(item);

                await _db.SaveChangesAsync();
            }
        }

        public async Task Edit(int id, int count)
        {
            var item = await _db.TemplateOperations.FindAsync(id);

            if (item == null)
                throw new NotFoundException();

            item.Count = count;

            _db.TemplateOperations.Update(item);
            await _db.SaveChangesAsync();
        }

        public async Task Remove(int id)
        {
            var item = await _db.TemplateOperations.FindAsync(id);

            if (item == null)
                throw new NotFoundException();

            _db.TemplateOperations.Remove(item);
            await _db.SaveChangesAsync();
        }

        public async Task MoveUp(int id)
        {
            var item = await _db.TemplateOperations.FindAsync(id);

            if(item == null)
                throw new NotFoundException();

            var previousNumber = await _db.TemplateOperations
                .Where(x => x.Number < item.Number && x.TemplateId == item.TemplateId)
                .MaxAsync(x => (int?)x.Number);

            if (previousNumber != null)
            {
                var previousItem = await _db.TemplateOperations
                    .FirstOrDefaultAsync(x => x.Number == previousNumber && x.TemplateId == item.TemplateId);

                if (previousItem == null)
                    throw new NotFoundException();

                previousItem.Number = item.Number;
                _db.TemplateOperations.Update(previousItem);

                item.Number = (int)previousNumber;
                _db.TemplateOperations.Update(item);
            }

            await _db.SaveChangesAsync();
        }

        public async Task MoveDown(int id)
        {
            var item = await _db.TemplateOperations.FindAsync(id);

            if(item == null)
                throw new NotFoundException();

            var nextNumber = await _db.TemplateOperations
                .Where(x => x.Number > item.Number && x.TemplateId == item.TemplateId)
                .MinAsync(x => (int?)x.Number);

            if (nextNumber != null)
            {
                var nextItem = await _db.TemplateOperations
                    .FirstOrDefaultAsync(x => x.Number == nextNumber && x.TemplateId == item.TemplateId);

                if(nextItem == null) 
                    throw new NotFoundException();

                nextItem.Number = item.Number;
                _db.TemplateOperations.Update(nextItem);

                item.Number = (int)nextNumber;
                _db.TemplateOperations.Update(item);

                await _db.SaveChangesAsync();
            }
        }
    }
}
