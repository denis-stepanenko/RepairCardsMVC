using Microsoft.EntityFrameworkCore;
using RepairCardsMVC.Exceptions;
using RepairCardsMVC.Models;

namespace RepairCardsMVC.Services
{
    public class CardOwnProductRepairOperationService
    {
        private readonly ApplicationDbContext _db;

        public CardOwnProductRepairOperationService(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task AddRange(List<int> ids, int productId, int count, DateTime date, int executorId)
        {
            var result = new List<CardOwnProductRepairOperation>();

            foreach (var id in ids)
            {
                var operation = await _db.Operations.FindAsync(id);

                if(operation == null)
                    continue;

                result.Add(new CardOwnProductRepairOperation
                {
                    CardOwnProductId = productId,
                    Code = operation.Code,
                    Name = operation.Name,
                    Labor = operation.Labor,
                    Department = operation.Department,
                    Count = count,
                    LaborAll = operation.Labor * 0,
                    Date = date,
                    UnitName = operation.UnitName ?? "",
                    GroupName = operation.GroupName ?? "",
                    ExecutorId = executorId
                });
            }

            _db.CardOwnProductRepairOperations.AddRange(result);
            _db.SaveChanges();
        }

        public async Task<CardOwnProductRepairOperation?> Get(int id)
            => await _db.CardOwnProductRepairOperations
                .Include(x => x.Executor)
                .FirstOrDefaultAsync(x => x.Id == id);

        public async Task Edit(int id, int count, int executorId, DateTime date)
        {
            var item = _db.CardOwnProductRepairOperations.Find(id);

            if (item == null)
                throw new NotFoundException();

            item.Date = date;
            item.ExecutorId = executorId;
            item.Count = count;
            item.LaborAll = item.Labor * count;

            _db.CardOwnProductRepairOperations.Update(item);
            await _db.SaveChangesAsync();
        }

        public async Task Remove(int id)
        {
            var item = await _db.CardOwnProductRepairOperations.FindAsync(id);

            if (item == null)
                throw new NotFoundException();

            _db.CardOwnProductRepairOperations.Remove(item);
            await _db.SaveChangesAsync();
        }

        public async Task Confirm(int id, string userId, string userName)
        {
            var item = await _db.CardOwnProductRepairOperations.FindAsync(id);

            if (item == null)
                throw new NotFoundException();

            if (item.ConfirmUserId == null)
            {

            }
        }

        public async Task Unconfirm(int id)
        {
            var item = await _db.CardOwnProductRepairOperations.FindAsync(id);

            if (item == null)
                throw new NotFoundException();

            if (item.ConfirmUserId != null)
            {

            }
        }
    }
}
