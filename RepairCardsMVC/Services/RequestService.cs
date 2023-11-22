using Microsoft.EntityFrameworkCore;
using RepairCardsMVC.Exceptions;
using RepairCardsMVC.Models;

namespace RepairCardsMVC.Services
{
    public class RequestService
    {
        private readonly ApplicationDbContext _db;

        public RequestService(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<PaginatedList<Request>> GetAll(int pageNumber, string filter)
        {
            var items = _db.Requests.Include(x => x.RepairType)
                .Where(x => x.CardNumber.Contains(filter))
                .OrderByDescending(x => x.Id);

            return await PaginatedList<Request>.CreateAsync(items, pageNumber);
        }

        public async Task<Request?> Get(int id)
            => await _db.Requests.FindAsync(id);

        public async Task Add(Request item)
        {
            var card = _db.Cards.FirstOrDefault(x => x.Number == item.CardNumber);

            if (card == null)
                throw new NotFoundException();

            item.ProductCode = card.ProductCode;
            item.ProductName = card.ProductName;
            item.Date = DateTime.Now;

            _db.Requests.Add(item);
            await _db.SaveChangesAsync();
        }

        public async Task Edit(Request item)
        {
            var request = await _db.Requests.FindAsync(item.Id);

            if (request == null)
                throw new NotFoundException();

            var card = await _db.Cards.FirstOrDefaultAsync(x => x.Number == item.CardNumber);

            if (card == null)
                throw new NotFoundException();

            request.ProductCode = card.ProductCode;
            request.ProductName = card.ProductName;

            request.CardNumber = item.CardNumber;
            request.Department = item.Department;
            request.ProductFactoryNumber = item.ProductFactoryNumber;
            request.RepairTypeId = item.RepairTypeId;
            request.ShortOrder = item.ShortOrder;
            request.ContractNumber = item.ContractNumber;
            request.RepairCode = item.RepairCode;
            request.RepairName = item.RepairName;
            request.RepairOrder = item.RepairOrder;
            request.RepairDirection = item.RepairDirection;
            request.RepairCipher = item.RepairCipher;
            request.RepairClientOrder = item.RepairClientOrder;
            request.ConstructorConfirmDate = item.ConstructorConfirmDate;

            _db.Entry(request).State = EntityState.Modified;
            await _db.SaveChangesAsync();
        }

        public async Task Remove(int id)
        {
            var item = await _db.Requests.FindAsync(id);

            if (item == null)
                throw new NotFoundException();

            _db.Requests.Remove(item);
            await _db.SaveChangesAsync();
        }

        public async Task Confirm(int id)
        {
            var item = _db.Requests.Find(id);

            if (item == null)
                throw new NotFoundException();

            if (item.ConstructorConfirmDate == null)
            {
                item.ConstructorConfirmDate = DateTime.Now;
            }
            else
            {
                item.ConstructorConfirmDate = null;
            }

            _db.Requests.Update(item);
            await _db.SaveChangesAsync();
        }
    }
}
