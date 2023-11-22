using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using RepairCardsMVC.Models;
using RepairCardsMVC.Services;
using RepairCardsMVC.ViewModels;

namespace RepairCardsMVC.Controllers
{
    public class CardMaterialController : Controller
    {
        private readonly CardMaterialService _cardMaterialService;
        private readonly MaterialService _materialService;
        private readonly UserManager<User> _userManager;
        private readonly ProductService _productService;
        private readonly CardService _cardService;

        public CardMaterialController(
            CardMaterialService cardMaterialService, 
            MaterialService materialService,
            UserManager<User> userManager,
            ProductService productService,
            CardService cardService)
        {
            _cardMaterialService = cardMaterialService;
            _materialService = materialService;
            _userManager = userManager;
            _productService = productService;
            _cardService = cardService;
        }

        public async Task<IActionResult> Index(
            int cardId,
            int cardsPageNumber = 1,
            string cardsFilter = "",
            int cardMaterialsPageNumber = 1,
            string cardMaterialsFilter = "")
        {
            ViewBag.CardId = cardId;
            ViewBag.CardsPageNumber = cardsPageNumber;
            ViewBag.CardsFilter = cardsFilter;
            ViewBag.CardMaterialsPageNumber = cardMaterialsPageNumber;
            ViewBag.CardMaterialsFilter = cardMaterialsFilter;

            return View(await _cardMaterialService.GetAll(cardId, cardMaterialsPageNumber, cardMaterialsFilter));
        }

        public async Task<IActionResult> Create(
            int cardId,
            int cardsPageNumber = 1,
            string cardsFilter = "",
            int cardMaterialsPageNumber = 1,
            string cardMaterialsFilter = "",
            int materialsPageNumber = 1,
            string materialsFilter = "")
        {
            var materials = await _materialService.GetAll(materialsPageNumber, materialsFilter);

            ViewBag.Materials = materials;
            ViewBag.CardId = cardId;
            ViewBag.CardsPageNumber = cardsPageNumber;
            ViewBag.CardsFilter = cardsFilter;
            ViewBag.CardMaterialsPageNumber = cardMaterialsPageNumber;
            ViewBag.CardMaterialsFilter = cardMaterialsFilter;
            ViewBag.MaterialsPageNumber = materialsPageNumber;
            ViewBag.MaterialsFilter = materialsFilter;

            return View();
        }

        [Authorize(Roles = "tb, ooiot, creator, prb")]
        [HttpPost]
        public async Task<IActionResult> Create(
            CreateCardMaterialViewModel model,
            int cardId,
            int cardsPageNumber = 1,
            string cardsFilter = "",
            int cardMaterialsPageNumber = 1,
            string cardMaterialsFilter = "",
            int materialsPageNumber = 1,
            string materialsFilter = "")
        {
            if (!ModelState.IsValid)
            {
                var materials = await _materialService.GetAll(materialsPageNumber, materialsFilter);

                ViewBag.Materials = materials;
                ViewBag.CardId = cardId;
                ViewBag.CardsPageNumber = cardsPageNumber;
                ViewBag.CardsFilter = cardsFilter;
                ViewBag.CardMaterialsPageNumber = cardMaterialsPageNumber;
                ViewBag.CardMaterialsFilter = cardMaterialsFilter;
                ViewBag.MaterialsPageNumber = materialsPageNumber;
                ViewBag.MaterialsFilter = materialsFilter;

                return View(model);
            }

            var ids = model.SelectedItems.Select(x => int.Parse(x)).ToList();

            await _cardMaterialService.AddRange(ids, cardId, model.Count, model.Department);

            return RedirectToAction("Index", new
            {
                cardId = cardId,
                cardsPageNumber = cardsPageNumber,
                cardsFilter = cardsFilter,
                cardMaterialsPageNumber = cardMaterialsPageNumber,
                cardMaterialsFilter = cardMaterialsFilter,
                materialsPageNumber = materialsPageNumber,
                materialsFilter = materialsFilter
            });
        }

        public IActionResult CreateByCard(
            int cardId,
            int cardsPageNumber = 1,
            string cardsFilter = "",
            int cardMaterialsPageNumber = 1,
            string cardMaterialsFilter = "",
            int materialsPageNumber = 1,
            string materialsFilter = "")
        {
            ViewBag.CardId = cardId;
            ViewBag.CardsPageNumber = cardsPageNumber;
            ViewBag.CardsFilter = cardsFilter;
            ViewBag.CardMaterialsPageNumber = cardMaterialsPageNumber;
            ViewBag.CardMaterialsFilter = cardMaterialsFilter;
            ViewBag.MaterialsPageNumber = materialsPageNumber;
            ViewBag.MaterialsFilter = materialsFilter;

            return View();
        }

        [Authorize(Roles = "tb, ooiot, creator, prb")]
        [HttpPost]
        public async Task<IActionResult> CreateByCard([FromBody] CreateCardMaterialByCardViewModel model)
        {
            await _cardMaterialService.AddRange(model.Items, model.CardId);

            return Ok();
        }

        public IActionResult CreateByProduct(
            int cardId,
            int cardsPageNumber = 1,
            string cardsFilter = "",
            int cardMaterialsPageNumber = 1,
            string cardMaterialsFilter = "",
            int materialsPageNumber = 1,
            string materialsFilter = "")
        {
            ViewBag.CardId = cardId;
            ViewBag.CardsPageNumber = cardsPageNumber;
            ViewBag.CardsFilter = cardsFilter;
            ViewBag.CardMaterialsPageNumber = cardMaterialsPageNumber;
            ViewBag.CardMaterialsFilter = cardMaterialsFilter;
            ViewBag.MaterialsPageNumber = materialsPageNumber;
            ViewBag.MaterialsFilter = materialsFilter;

            return View();
        }

        [Authorize(Roles = "tb, ooiot, creator, prb")]
        [HttpPost]
        public async Task<IActionResult> CreateByProduct([FromBody] CreateCardMaterialByProductViewModel model)
        {
            if (model.Department == 0)
                return BadRequest();

            await _cardMaterialService.AddRange(model.Items, model.CardId, model.Department);

            return Ok();
        }

        [Authorize(Roles = "tb, ooiot, creator, prb")]
        public async Task<IActionResult> CreateByAllProductsInCard(
            int cardId,
            int cardsPageNumber = 1,
            string cardsFilter = "",
            int cardMaterialsPageNumber = 1,
            string cardMaterialsFilter = "")
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
                return NotFound();

            await _cardMaterialService.AddByAllOwnProductsInCard(cardId, user.Department);

            return RedirectToAction("Index", new
            {
                cardId = cardId,
                cardsPageNumber = cardsPageNumber,
                cardsFilter = cardsFilter,
                cardMaterialsPageNumber = cardMaterialsPageNumber,
                cardMaterialsFilter = cardMaterialsFilter
            });
        }

        public async Task<IActionResult> Edit(
            int id,
            int cardId,
            int cardsPageNumber = 1,
            string cardsFilter = "",
            int cardMaterialsPageNumber = 1,
            string cardMaterialsFilter = "")
        {
            var item = await _cardMaterialService.Get(id);

            if (item == null)
                return NotFound();

            ViewBag.CardId = cardId;
            ViewBag.CardsPageNumber = cardsPageNumber;
            ViewBag.CardsFilter = cardsFilter;
            ViewBag.CardMaterialsPageNumber = cardMaterialsPageNumber;
            ViewBag.CardMaterialsFilter = cardMaterialsFilter;
            ViewBag.Item = item;

            return View(new EditCardMaterialViewModel { Count = item.Count });
        }

        [Authorize(Roles = "tb, ooiot, creator, prb")]
        [HttpPost]
        public async Task<IActionResult> Edit(
            EditCardMaterialViewModel model,
            int cardId,
            int cardsPageNumber = 1,
            string cardsFilter = "",
            int cardMaterialsPageNumber = 1,
            string cardMaterialsFilter = "")
        {
            if (!ModelState.IsValid)
            {
                var item = await _cardMaterialService.Get(model.Id);

                ViewBag.CardsPageNumber = cardsPageNumber;
                ViewBag.CardsFilter = cardsFilter;
                ViewBag.CardMaterialsPageNumber = cardMaterialsPageNumber;
                ViewBag.CardMaterialsFilter = cardMaterialsFilter;
                ViewBag.Item = item;

                return View(model);
            }

            await _cardMaterialService.Edit(model.Id, model.Count);

            return RedirectToAction("Index", new
            {
                cardId = cardId,
                cardsPageNumber = cardsPageNumber,
                cardsFilter = cardsFilter,
                cardMaterialsPageNumber = cardMaterialsPageNumber,
                cardMaterialsFilter = cardMaterialsFilter
            });
        }

        [Authorize(Roles = "tb, ooiot, creator, prb")]
        public async Task<IActionResult> Delete(
            int id,
            int cardId,
            int cardsPageNumber = 1,
            string cardsFilter = "",
            int cardMaterialsPageNumber = 1,
            string cardMaterialsFilter = "")
        {
            await _cardMaterialService.Remove(id);

            return RedirectToAction("Index", new
            {
                cardId = cardId,
                cardsPageNumber = cardsPageNumber,
                cardsFilter = cardsFilter,
                cardMaterialsPageNumber = cardMaterialsPageNumber,
                cardMaterialsFilter = cardMaterialsFilter
            });
        }

        public async Task<IActionResult> GetMaterialsByProduct(string code)
            => Ok(await _materialService.GetMaterialsByProduct(code));

        public async Task<IActionResult> GetMaterialsByCard(int cardId)
            => Ok(await _cardMaterialService.GetMaterialsByCard(cardId));

        [HttpPost]
        public IActionResult GetProducts()
            => Ok(_productService.GetAll(Request));

        [HttpPost]
        public IActionResult GetCards()
            => Ok(_cardService.GetCards(Request));
    }
}
