using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RepairCardsMVC.Models;
using RepairCardsMVC.Services;

namespace RepairCardsMVC.Controllers
{
    public class CardDetailsController : Controller
    {
        private readonly CardDetailsService _cardDetailsService;

        public CardDetailsController(CardDetailsService cardDetailsService)
        {
            _cardDetailsService = cardDetailsService;
        }

        public async Task<IActionResult> Edit(
            int cardId,
            int cardsPageNumber = 1,
            string cardsFilter = "")
        {
            var item = await _cardDetailsService.Get(cardId);

            if (item == null)
                return NotFound();

            ViewBag.CardId = cardId;
            ViewBag.CardsPageNumber = cardsPageNumber;
            ViewBag.CardsFilter = cardsFilter;

            return View(item);
        }

        [Authorize(Roles = "tb, ooiot, creator, prb")]
        [HttpPost]
        public async Task<IActionResult> Edit(
            CardDetails item,
            int cardId,
            int cardsPageNumber = 1,
            string cardsFilter = "")
        {
            if (!ModelState.IsValid)
                return View(item);

            await _cardDetailsService.Edit(item);

            return RedirectToAction(
                actionName: "Edit", 
                controllerName: "Card",
                new 
            { 
                id = item.Id,
                pageNumber = cardsPageNumber,
                filter = cardsFilter
            });
        }
    }
}
