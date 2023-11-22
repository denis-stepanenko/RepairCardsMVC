using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RepairCardsMVC.Exceptions;
using RepairCardsMVC.Services;
using RepairCardsMVC.ViewModels;

namespace RepairCardsMVC.Controllers
{
    public class CardChildCardController : Controller
    {
        private readonly CardService _cardService;

        public CardChildCardController(CardService cardService)
        {
            _cardService = cardService;
        }

        public async Task<IActionResult> Index(
            int cardId,
            int cardsPageNumber,
            string cardsFilter)
        {
            var extractedCards = await _cardService.GetExtractedChildCards(cardId);
            var installedCards = await _cardService.GetInstalledCards(cardId);

            ViewBag.ExtractedCards = extractedCards;
            ViewBag.InstalledCards = installedCards;
            ViewBag.CardId = cardId;
            ViewBag.CardsPageNumber = cardsPageNumber;
            ViewBag.CardsFilter = cardsFilter;

            return View();
        }

        [Authorize(Roles = "tb, ooiot, creator, prb")]
        public async Task<IActionResult> AddExtractedCard(
            int id,
            int cardId,
            int cardsPageNumber,
            string cardsFilter)
        {
            try
            {
                await _cardService.AddExtractedCard(id, cardId);
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

        [Authorize(Roles = "tb, ooiot, creator, prb")]
        public async Task<IActionResult> AddInstalledCard(
            int id,
            int cardId,
            int cardsPageNumber,
            string cardsFilter)
        {
            try
            {
                await _cardService.AddInstalledCard(id, cardId);
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

        [Authorize(Roles = "tb, ooiot, creator, prb")]
        public async Task<IActionResult> DeleteExtractedCard(
            int id,
            int cardId,
            int cardsPageNumber,
            string cardsFilter)
        {
            try
            {
                await _cardService.DeleteExtractedCard(id);
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

        [Authorize(Roles = "tb, ooiot, creator, prb")]
        public async Task<IActionResult> DeleteInstalledCard(
            int id,
            int cardId,
            int cardsPageNumber,
            string cardsFilter)
        {
            try
            {
                await _cardService.DeleteInstalledCard(id);
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

        public async Task<IActionResult> GetExtractedCards(int parentId)
        {
            var card = await _cardService.Get(parentId);

            if (card == null)
                return NotFound();

            var extractedCards = _cardService.GetExtractedChildCardsRecursively(card);

            var extractedCardModels = extractedCards
                .Select(x => new ChildCardViewModel
                {
                    Id = x.Id,
                    ParentId = x.ParentId == null ? "" : x.ParentId.ToString(),
                    Number = x.Number,
                    ProductCode = x.ProductCode,
                    ProductName = x.ProductName ?? "",
                    Direction = x.Direction ?? "",
                    Cipher = x.Cipher ?? "",
                    HasNotBeenRepaired = x.HasNotBeenRepaired
                }).ToList();

            return Ok(extractedCardModels);
        }

        public async Task<IActionResult> GetInstalledCards(int parentId)
        {
            var card = await _cardService.Get(parentId);

            if (card == null)
                return NotFound();

            var items = _cardService.GetInstalledChildCardsRecursively(card);

            var models = items
                .Select(x => new ChildCardViewModel
                {
                    Id = x.Id,
                    ParentId = x.ParentId2 == null ? "" : x.ParentId2.ToString(),
                    Number = x.Number,
                    ProductCode = x.ProductCode,
                    ProductName = x.ProductName ?? "",
                    Direction = x.Direction ?? "",
                    Cipher = x.Cipher ?? "",
                    HasNotBeenRepaired = x.HasNotBeenRepaired
                }).ToList();

            return Ok(models);
        }

        [HttpPost]
        public IActionResult GetCards()
            => Ok(_cardService.GetCards(Request));
    }
}
