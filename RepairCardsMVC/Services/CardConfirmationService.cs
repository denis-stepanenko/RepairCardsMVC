using Microsoft.EntityFrameworkCore;
using RepairCardsMVC.Exceptions;
using RepairCardsMVC.Models;

namespace RepairCardsMVC.Services
{
    public class CardConfirmationService
    {
        private readonly ApplicationDbContext _db;

        public CardConfirmationService(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<IEnumerable<CardConfirmation>> GetAll(int cardId)
            => await _db.CardConfirmations
                .Include(x => x.CardConfirmationObject)
                .Include(x => x.UserRole)
                .Where(x => x.CardId == cardId)
                .OrderByDescending(x => x.Id)
                .ToListAsync();

        public async Task Add(int cardId, int confirmationObjectId, string userName, List<string> userRoles)
        {
            if (GetOldRole(userRoles.First()) == 0)
                throw new BusinessLogicException("Добавление утверждения непредусмотрено для этой роли");

            if (confirmationObjectId != 0)
            {
                _db.CardConfirmations.Add(new CardConfirmation
                {
                    CardId = cardId,
                    UserName = userName,
                    UserRoleId = GetOldRole(userRoles.First()),
                    CardConfirmationObjectId = confirmationObjectId,
                    Date = DateTime.Now
                });

                await _db.SaveChangesAsync();
            }
        }

        public async Task Remove(int id, List<string> userRoles)
        {
            var item = await _db.CardConfirmations.FindAsync(id);

            if (item == null)
                throw new NotFoundException();

            if (item.UserRoleId != GetOldRole(userRoles.First()))
                if(!userRoles.Contains("admin"))
                    throw new BusinessLogicException("У вас нет прав удалить это утверждение");

            _db.CardConfirmations.Remove(item);
            await _db.SaveChangesAsync();
        }

        private int GetOldRole(string role)
            => role switch
            {
                "tb" => 3,
                "ooiot" => 4,
                "creator" => 5,
                "otk" => 6,
                "prb" => 7,
                "skb" => 8,
                "vp" => 9,
                "peo" => 10,
                "skie" => 11,
                _ => 0
            };
    }
}
