using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using RepairCardsMVC.Exceptions;
using RepairCardsMVC.Models;
using RepairCardsMVC.Services;

namespace RepairCardsMVC.Controllers
{
    public class CardConfirmationController : Controller
    {
        private readonly CardConfirmationService _cardConfirmationService;
        private readonly CardConfirmationObjectService _cardConfirmationObjectService;
        private readonly UserManager<User> _userManager;

        public CardConfirmationController(
             CardConfirmationService cardConfirmationService,
             CardConfirmationObjectService cardConfirmationObjectService,
             UserManager<User> userManager)
        {
            _cardConfirmationService = cardConfirmationService;
            _cardConfirmationObjectService = cardConfirmationObjectService;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index(
            int cardId,
            int cardsPageNumber = 1,
            string cardsFilter = "")
        {
            var items = await _cardConfirmationService.GetAll(cardId);

            var confirmationObjects = await _cardConfirmationObjectService.GetAll();

            ViewBag.ConfirmationObjects = confirmationObjects.Select(x =>
                new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.Name
                });

            ViewBag.CardId = cardId;
            ViewBag.CardsPageNumber = cardsPageNumber;
            ViewBag.CardsFilter = cardsFilter;

            return View(items.ToList());
        }

        [Authorize(Roles = "tb, ooiot, otk, prb, vp")]
        public async Task<IActionResult> Create(
            int confirmationObjectId,
            int cardId,
            int cardsPageNumber = 1,
            string cardsFilter = "")
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
                return NotFound();

            var roles = await _userManager.GetRolesAsync(user);

            await _cardConfirmationService.Add(cardId, confirmationObjectId, user.Name, roles.ToList());

            return RedirectToAction("Index", new
            {
                cardId = cardId,
                cardsPageNumber = cardsPageNumber,
                cardsFilter = cardsFilter
            });
        }

        [Authorize(Roles = "tb, ooiot, otk, prb, vp")]
        public async Task<IActionResult> Delete(
            int id,
            int cardId,
            int cardsPageNumber = 1,
            string cardsFilter = "")
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
                return NotFound();

            var roles = await _userManager.GetRolesAsync(user);

            try
            {
                await _cardConfirmationService.Remove(id, roles.ToList());
            }
            catch (BusinessLogicException ex)
            {
                TempData["message"] = ex.Message;               
            }

            return RedirectToAction("Index", new
            {
                cardId = cardId,
                cardsPageNumber = cardsPageNumber,
                cardsFilter = cardsFilter
            });
        }
    }
}
