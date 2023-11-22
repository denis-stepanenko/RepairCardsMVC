using Microsoft.EntityFrameworkCore;
using RepairCardsMVC.Models;

namespace RepairCardsMVC.Services
{
    public class CardConfirmationObjectService
    {
        private readonly ApplicationDbContext _db;

        public CardConfirmationObjectService(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<IEnumerable<CardConfirmationObject>> GetAll()
            => await _db.CardConfirmationObjects.ToListAsync();
    }
}
