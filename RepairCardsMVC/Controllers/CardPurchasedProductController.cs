using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RepairCardsMVC.Services;
using RepairCardsMVC.ViewModels;
using System.Data;

namespace RepairCardsMVC.Controllers
{
    public class CardPurchasedProductController : Controller
    {
        private readonly CardPurchasedProductService _cardPurchasedProductService;
        private readonly CardService _cardService;
        private ProductService _productService;

        public CardPurchasedProductController( 
            CardPurchasedProductService cardPurchasedProductService,
            CardService cardService,
            ProductService productService)
        {
            _cardPurchasedProductService = cardPurchasedProductService;
            _cardService = cardService;
            _productService = productService;
        }

        public async Task<IActionResult> Index(
            int id,
            int cardsPageNumber,
            string cardsFilter)
        {
            var items = await _cardPurchasedProductService.GetAll(id);

            ViewBag.CardId = id;
            ViewBag.CardsPageNumber = cardsPageNumber;
            ViewBag.CardsFilter = cardsFilter;

            return View(items);
        }

        public async Task<IActionResult> Edit(
            int id,
            int cardId,
            int cardsPageNumber,
            string cardsFilter)
        {
            var item = await _cardPurchasedProductService.Get(id);

            if (item == null)
                return NotFound();

            ViewBag.Item = item;
            ViewBag.CardId = cardId;
            ViewBag.CardsPageNumber = cardsPageNumber;
            ViewBag.CardsFilter = cardsFilter;

            return View(new EditCardPurchasedProductViewModel { Count = item.Count });
        }

        [Authorize(Roles = "tb, ooiot, creator, prb")]
        [HttpPost]
        public async Task<IActionResult> Edit(
            EditCardPurchasedProductViewModel model,
            int id,
            int cardId,
            int cardsPageNumber,
            string cardsFilter = "")
        {
            if(!ModelState.IsValid)
            {
                var product = await _cardPurchasedProductService.Get(id);

                if (product == null)
                    return NotFound();

                ViewBag.Item = product;
                ViewBag.CardId = cardId;
                ViewBag.CardsPageNumber = cardsPageNumber;
                ViewBag.CardsFilter = cardsFilter;

                return View(model);
            }

            await _cardPurchasedProductService.Edit(id, model.Count);

            return RedirectToAction("Index", new
            {
                id = cardId,
                cardsPageNumber = cardsPageNumber,
                cardsFilter = cardsFilter
            });
        }

        [Authorize(Roles = "tb, ooiot, creator, prb")]
        public async Task<IActionResult> Delete(
            int id,
            int cardId,
            int cardsPageNumber,
            string cardsFilter)
        {
            await _cardPurchasedProductService.Remove(id);

            return RedirectToAction("Index", new
            {
                id = cardId,
                cardsPageNumber = cardsPageNumber,
                cardsFilter = cardsFilter
            });
        }

        public IActionResult CreateByCard(
            int cardId,
            int cardsPageNumber,
            string cardsFilter)
        {
            ViewBag.CardId = cardId;
            ViewBag.CardsPageNumber = cardsPageNumber;
            ViewBag.CardsFilter = cardsFilter;

            return View();
        }

        [Authorize(Roles = "tb, ooiot, creator, prb")]
        [HttpPost]
        public async Task<IActionResult> CreateByCard(
            CreateCardPurchasedProductByCardViewModel model,
            int cardId,
            int cardsPageNumber,
            string cardsFilter = "")
        {
            if(!ModelState.IsValid)
            {
                ViewBag.CardId = cardId;
                ViewBag.CardsPageNumber = cardsPageNumber;
                ViewBag.CardsFilter = cardsFilter;

                return View(model);
            }

            var ids = model.Ids.Select(x => int.Parse(x)).ToList();

            await _cardPurchasedProductService.AddRange(ids, cardId);

            return RedirectToAction("Index", new 
            { 
                id = cardId,
                cardsPageNumber = cardsPageNumber,
                cardsFilter = cardsFilter
            });
        }

        public IActionResult CreateByProduct(
            int cardId,
            int cardsPageNumber,
            string cardsFilter)
        {
            ViewBag.CardId = cardId;
            ViewBag.CardType = cardId;
            ViewBag.CardsPageNumber = cardsPageNumber;
            ViewBag.CardsFilter = cardsFilter;

            return View();
        }

        [Authorize(Roles = "tb, ooiot, creator, prb")]
        [HttpPost]
        public async Task<IActionResult> CreateByProduct([FromBody]CreateCardPurchasedProductByProductViewModel model)
        {
            await _cardPurchasedProductService.AddRange(model.Items, model.CardId);

            return Ok();
        }

        [HttpPost]
        public IActionResult GetCards()
            => Ok(_cardService.GetCards(Request));

        [HttpPost]
        public IActionResult GetProducts()
            => Ok(_productService.GetAll(Request));

        public IActionResult GetPurchasedProductsByProducts(string code)
            => Ok(_productService.GetPurchasedProductsByProduct(code));

        public async Task<IActionResult> GetProductsByCard(int cardId)
            => Ok(await _cardPurchasedProductService.GetAll(cardId));  
    }
}
