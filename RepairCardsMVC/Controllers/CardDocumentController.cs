using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RepairCardsMVC.Models;
using RepairCardsMVC.Services;

namespace RepairCardsMVC.Controllers
{
    public class CardDocumentController : Controller
    {
        private readonly CardDocumentService _cardDocumentService;

        public CardDocumentController(CardDocumentService cardDocumentService)
        {
            _cardDocumentService = cardDocumentService;
        }

        public async Task<IActionResult> Index(
            int cardId,
            int cardsPageNumber = 1,
            string cardsFilter = "",
            int cardDocumentsPageNumber = 1,
            string cardDocumentsFilter = "")
        {

            ViewBag.CardId = cardId;
            ViewBag.CardsPageNumber = cardsPageNumber;
            ViewBag.CardsFilter = cardsFilter;
            ViewBag.CardDocumentsPageNumber = cardDocumentsPageNumber;
            ViewBag.CardDocumentsFilter = cardDocumentsFilter;

            return View(await _cardDocumentService.GetAll(cardId, cardDocumentsPageNumber, cardDocumentsFilter));
        }

        public IActionResult Create(
            int cardId,
            int cardsPageNumber = 1,
            string cardsFilter = "",
            int cardDocumentsPageNumber = 1,
            string cardDocumentsFilter = "")
        {
            ViewBag.CardsPageNumber = cardsPageNumber;
            ViewBag.CardsFilter = cardsFilter;
            ViewBag.CardDocumentsPageNumber = cardDocumentsPageNumber;
            ViewBag.CardDocumentsFilter = cardDocumentsFilter;

            return View(new CardDocument { CardId = cardId });
        }

        [Authorize(Roles = "tb, ooiot, creator, prb")]
        [HttpPost]
        public async Task<IActionResult> Create(
            CardDocument item,
            int cardId,
            int cardsPageNumber = 1,
            string cardsFilter = "",
            int cardDocumentsPageNumber = 1,
            string cardDocumentsFilter = "")
        {
            if (!ModelState.IsValid)
            {
                ViewBag.CardsPageNumber = cardsPageNumber;
                ViewBag.CardsFilter = cardsFilter;
                ViewBag.CardDocumentsPageNumber = cardDocumentsPageNumber;
                ViewBag.CardDocumentsFilter = cardDocumentsFilter;

                return View(item);
            }

            await _cardDocumentService.Add(item);

            return RedirectToAction("Index", new
            {
                cardId = item.CardId,
                cardsPageNumber = cardsPageNumber,
                cardsFilter = cardsFilter,
                cardDocumentsPageNumber = cardDocumentsPageNumber,
                cardDocumentsFilter = cardDocumentsFilter
            });
        }

        public async Task<IActionResult> Edit(
            int id,
            int cardsPageNumber = 1,
            string cardsFilter = "",
            int cardDocumentsPageNumber = 1,
            string cardDocumentsFilter = "")
        {
            var item = await _cardDocumentService.Get(id);

            if (item == null)
                return NotFound();

            ViewBag.CardsPageNumber = cardsPageNumber;
            ViewBag.CardsFilter = cardsFilter;
            ViewBag.CardDocumentsPageNumber = cardDocumentsPageNumber;
            ViewBag.CardDocumentsFilter = cardDocumentsFilter;

            return View(item);
        }

        [Authorize(Roles = "tb, ooiot, creator, prb")]
        [HttpPost]
        public async Task<IActionResult> Edit(
            CardDocument item,
            int cardsPageNumber = 1,
            string cardsFilter = "",
            int cardDocumentsPageNumber = 1,
            string cardDocumentsFilter = "")
        {
            if (!ModelState.IsValid)
            {
                ViewBag.CardsPageNumber = cardsPageNumber;
                ViewBag.CardsFilter = cardsFilter;
                ViewBag.CardDocumentsPageNumber = cardDocumentsPageNumber;
                ViewBag.CardDocumentsFilter = cardDocumentsFilter;

                return View(item);
            }

            await _cardDocumentService.Edit(item);

            return RedirectToAction("Index", new
            {
                cardId = item.CardId,
                cardsPageNumber = cardsPageNumber,
                cardsFilter = cardsFilter,
                cardDocumentsPageNumber = cardDocumentsPageNumber,
                cardDocumentsFilter = cardDocumentsFilter
            });
        }

        [Authorize(Roles = "tb, ooiot, creator, prb")]
        public async Task<IActionResult> Delete(
            int id,
            int cardId,
            int cardsPageNumber = 1,
            string cardsFilter = "",
            int cardDocumentsPageNumber = 1,
            string cardDocumentsFilter = "")
        {
            await _cardDocumentService.Remove(id);

            return RedirectToAction("Index", new
            {
                cardId = cardId,
                cardsPageNumber = cardsPageNumber,
                cardsFilter = cardsFilter,
                cardDocumentsPageNumber = cardDocumentsPageNumber,
                cardDocumentsFilter = cardDocumentsFilter
            });
        }
    }
}
