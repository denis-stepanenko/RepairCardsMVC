using RepairCardsMVC.Models;

namespace RepairCardsMVC.Services
{
    public class CardDetailsService
    {
        private readonly ApplicationDbContext _db;

        public CardDetailsService(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<CardDetails?> Get(int id)
            => await _db.CardDetails.FindAsync(id); 

        public async Task Edit(CardDetails item)
        {
            _db.CardDetails.Update(item);
            await _db.SaveChangesAsync();
        }
    }
}
