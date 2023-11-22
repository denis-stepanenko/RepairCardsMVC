using RepairCardsMVC.Models;

namespace RepairCardsMVC.Services
{
    public class OrderService
    {
        private readonly ApplicationDbContext _db;

        public OrderService(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<IEnumerable<Order>> GetAll(string productCode)
        {
            var orders = await _db.GetOrdersByProductAsync(productCode);

            var items = orders.Where(x => x.Direction != null)
                .Select(x => new Order
                {
                    Number = x.Number,
                    Cipher = x.Cipher ?? "",
                    Direction = x.Direction
                });

            return items;
        }
    }
}
