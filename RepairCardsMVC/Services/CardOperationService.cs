using Microsoft.EntityFrameworkCore;
using RepairCardsMVC.Exceptions;
using RepairCardsMVC.Models;
using RepairCardsMVC.ViewModels;

namespace RepairCardsMVC.Services
{
    public class CardOperationService
    {
        private readonly ApplicationDbContext _db;

        public CardOperationService(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<IEnumerable<CardOperation>> GetAll(int cardId, int type, int pageNumber, string filter)
        {
            var items =
                _db.CardOperations
                .Include(x => x.Executor)
                .Where(x => x.CardId == cardId && x.Type == type &&
                x.Name.Contains(filter))
                .OrderByDescending(x => x.Id);

            return await PaginatedList<CardOperation>.CreateAsync(items, pageNumber);
        }

        public DataHelperViewModel<CardOperation> GetAll(HttpRequest request, int cardId, int type)
            => request.GetDataForJQueryDataTable(_db.CardOperations.Where(x => x.CardId == cardId && x.Type == type).AsQueryable(), "Code", "Name");

        public async Task<CardOperation?> Get(int id)
            => await _db.CardOperations.FindAsync(id);

        public async Task Edit(int id, int count, int executorId, DateTime date)
        {
            var oldItem = await _db.CardOperations.FindAsync(id);

            if (oldItem == null)
                throw new NotFoundException();

            oldItem.Count = count;
            oldItem.LaborAll = oldItem.Labor * count;
            oldItem.Date = date;
            oldItem.ExecutorId = executorId;

            _db.CardOperations.Update(oldItem);
            await _db.SaveChangesAsync();
        }

        public async Task AddRange(List<int> ids, int cardId, int count, int type, int executorId, DateTime date)
        {
            var result = new List<CardOperation>();

            foreach (var id in ids)
            {
                var operation = await _db.Operations.FindAsync(id);

                if (operation == null)
                    continue;

                result.Add(new CardOperation
                {
                    CardId = cardId,
                    Code = operation.Code,
                    Name = operation.Name,
                    Count = count,
                    Labor = operation.Labor,
                    LaborAll = operation.Labor * count,
                    Type = type,
                    Date = date,
                    UnitName = operation.UnitName,
                    GroupName = operation.GroupName,
                    Department = operation.Department,
                    ExecutorId = executorId
                });
            }

            _db.CardOperations.AddRange(result);
            await _db.SaveChangesAsync();
        }

        public async Task AddByCard(List<int> ids, int cardId, DateTime date, int executorId, int type)
        {
            var items = new List<CardOperation>();

            foreach(var id in ids)
            {
                var operation = await _db.CardOperations.FindAsync(id);

                if (operation == null)
                    continue;

                items.Add(new CardOperation
                {
                    CardId = cardId,
                    Code = operation.Code,
                    Name = operation.Name,
                    Count = operation.Count,
                    Labor = operation.Labor,
                    LaborAll = operation.LaborAll,
                    Type = type,
                    Date = date,
                    UnitName = operation.UnitName,
                    GroupName = operation.GroupName,
                    Department = operation.Department,
                    ExecutorId = executorId
                });
            }

            _db.CardOperations.AddRange(items);
            await _db.SaveChangesAsync();
        }

        public async Task AddByTemplate(List<int> ids, int cardId, int type, int executorId, DateTime date)
        {
            var result = new List<CardOperation>();

            foreach (var id in ids)
            {
                var operation = await _db.TemplateOperations
                    .Include(o => o.Operation)
                    .FirstOrDefaultAsync(o => o.Id == id);

                if (operation?.Operation == null)
                    continue;

                result.Add(new CardOperation
                {
                    CardId = cardId,
                    Code = operation.Operation.Code,
                    Name = operation.Operation.Name,
                    Count = operation.Count,
                    Labor = operation.Operation.Labor,
                    LaborAll = operation.Operation.Labor * operation.Count,
                    Type = type,
                    Date = date,
                    UnitName = operation.Operation.UnitName,
                    GroupName = operation.Operation.GroupName,
                    Department = operation.Operation.Department,
                    ExecutorId = executorId
                });
            }

            _db.CardOperations.AddRange(result);
            await _db.SaveChangesAsync();
        }

        public async Task Remove(int id)
        {
            var item = await _db.CardOperations.FindAsync(id);

            if (item == null)
                throw new NotFoundException();

            _db.CardOperations.Remove(item);
            await _db.SaveChangesAsync();
        }

        public async Task ChangeDateAndExecutor(List<int> ids, int executorId, DateTime date)
        {
                foreach(var id in ids)
                {
                    var item = await _db.CardOperations.FindAsync(id);

                    if (item != null)
                    {
                        item.Date = (DateTime)date;
                        item.ExecutorId = executorId;

                        _db.CardOperations.Update(item);
                    }
                };

                await _db.SaveChangesAsync();
        }

        public async Task<IEnumerable<CardOperation>> GetFactOperationsByCard(int cardId)
            => await _db.CardOperations.Where(x => x.CardId == cardId && x.Type == 1).ToListAsync();
    }
}
