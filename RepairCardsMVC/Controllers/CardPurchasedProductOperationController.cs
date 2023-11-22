using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using RepairCardsMVC.Services;
using RepairCardsMVC.ViewModels;
using System.Data;

namespace RepairCardsMVC.Controllers
{
    public class CardPurchasedProductOperationController : Controller
    {
        private readonly CardPurchasedProductOperationService _cardPurchasedProductOperationService;
        private readonly CardPurchasedProductService _cardPurchasedProductService;
        private readonly ProductService _productService;
        private readonly ProductOperationService _productOperationService;
        private readonly ExecutorService _executorService;

        public CardPurchasedProductOperationController(
            CardPurchasedProductOperationService cardPurchasedsProductOperationService,
            CardPurchasedProductService cardPurchasedProductService,
            ProductService productService,
            ProductOperationService productOperationService,
            ExecutorService executorService)
        {
            _cardPurchasedProductOperationService = cardPurchasedsProductOperationService;
            _cardPurchasedProductService = cardPurchasedProductService;
            _productService = productService;
            _productOperationService = productOperationService;
            _executorService = executorService;

        }

        async Task FillCreateBags(int productId)
        {
            var product = await _cardPurchasedProductService.Get(productId);

            if (product == null)
                return;

            var route = _productService.GetPurchasedProductRoute(product.Code);

            if (route == null)
                return;

            var operations = _productOperationService.GetAll(product.Code, route);

            ViewBag.Items = operations;
            ViewBag.Route = route;
        }

        public async Task<IActionResult> Create(
            int productId,
            int cardId,
            int cardsPageNumber,
            string cardsFilter)
        {
            await FillCreateBags(productId);

            ViewBag.ProductId = productId;
            ViewBag.CardId = cardId;
            ViewBag.CardsPageNumber = cardsPageNumber;
            ViewBag.CardsFilter = cardsFilter;

            return View();
        }

        [Authorize(Roles = "tb, ooiot, creator, prb")]
        [HttpPost]
        public async Task<IActionResult> Create(
            CreateCardPurchasedProductOperationViewModel model,
            int cardId,
            int productId,
            DateTime date,
            int cardsPageNumber,
            string cardsFilter = "")
        {
            if (!ModelState.IsValid)
            {
                await FillCreateBags(productId);

                ViewBag.ProductId = productId;
                ViewBag.CardId = cardId;
                ViewBag.CardsPageNumber = cardsPageNumber;
                ViewBag.CardsFilter = cardsFilter;

                var executor = await _executorService.Get(model.ExecutorId ?? 0);

                ViewBag.Executor = executor;

                return View(model);
            }

            var ids = model.Ids.Select(x => int.Parse(x)).ToList();

            await _cardPurchasedProductOperationService.AddRange(ids, productId, model.ExecutorId ?? 0, model.Date);

            return RedirectToAction(actionName: "Edit", controllerName: "CardPurchasedProduct",
                new
                {
                    id = productId,
                    cardId = cardId,
                    cardsPageNumber = cardsPageNumber,
                    cardsFilter = cardsFilter
                });
        }

        public async Task<IActionResult> Edit(
            int id,
            int cardId,
            int cardsPageNumber,
            string cardsFilter)
        {
            var item = await _cardPurchasedProductOperationService.Get(id);

            if(item == null)
                return NotFound();

            var executor = await _executorService.Get(item.ExecutorId ?? 0);

            ViewBag.Executor = executor;
            ViewBag.Item = item;
            ViewBag.CardId = cardId;
            ViewBag.CardsPageNumber = cardsPageNumber;
            ViewBag.CardsFilter = cardsFilter;

            return View(new EditCardPurchasedProductOperationViewModel { Date = item.Date, ExecutorId = item.ExecutorId });
        }

        [Authorize(Roles = "tb, ooiot, creator, prb")]
        [HttpPost]
        public async Task<IActionResult> Edit(
            EditCardPurchasedProductOperationViewModel model,
            int id,
            int cardId,
            int cardPurchasedProductId,
            int cardsPageNumber,
            string cardsFilter = "")
        {
            if(!ModelState.IsValid)
            {
                var operation = await _cardPurchasedProductOperationService.Get(id);

                if (operation == null)
                    return NotFound();

                var executor = await _executorService.Get(operation.ExecutorId ?? 0);

                ViewBag.Executor = executor;
                ViewBag.Item = operation;
                ViewBag.CardId = cardId;
                ViewBag.CardsPageNumber = cardsPageNumber;
                ViewBag.CardsFilter = cardsFilter;

                return View(model);
            }

            await _cardPurchasedProductOperationService.Edit(id, model.ExecutorId ?? 0, (DateTime)model.Date);

            return RedirectToAction(actionName: "Edit", controllerName: "CardPurchasedProduct",
                new
                {
                    id = cardPurchasedProductId,
                    cardId = cardId,
                    cardsPageNumber = cardsPageNumber,
                    cardsFilter = cardsFilter
                });
        }

        [Authorize(Roles = "tb, ooiot, creator, prb")]
        public async Task<IActionResult> Delete(
            int id,
            int cardId,
            int cardPurchasedProductId,
            int cardsPageNumber,
            string cardsFilter)
        {
            await _cardPurchasedProductOperationService.Remove(id);

            return RedirectToAction(actionName: "Edit", controllerName: "CardPurchasedProduct",
                new
                {
                    id = cardPurchasedProductId,
                    cardId = cardId,
                    cardsPageNumber = cardsPageNumber,
                    cardsFilter = cardsFilter
                });
        }

        [Authorize(Roles = "tb, ooiot, creator, prb")]
        public async Task<IActionResult> Confirm(
            int id,
            int cardId,
            int cardPurchasedProductId,
            int cardsPageNumber,
            string cardsFilter)
        {
            await _cardPurchasedProductOperationService.Confirm(id, "", "");

            return RedirectToAction(actionName: "Edit", controllerName: "CardPurchasedProduct",
                new
                {
                    id = cardPurchasedProductId,
                    cardId = cardId,
                    cardsPageNumber = cardsPageNumber,
                    cardsFilter = cardsFilter
                });
        }

        [Authorize(Roles = "tb, ooiot, creator, prb")]
        public async Task<IActionResult> Unconfirm(
            int id,
            int cardId,
            int cardPurchasedProductId,
            int cardsPageNumber,
            string cardsFilter)
        {
            await _cardPurchasedProductOperationService.Unconfirm(id, "");

            return RedirectToAction(actionName: "Edit", controllerName: "CardPurchasedProduct",
                new
                {
                    id = cardPurchasedProductId,
                    cardId = cardId,
                    cardsPageNumber = cardsPageNumber,
                    cardsFilter = cardsFilter
                });
        }

        public async Task<IActionResult> FindExecutors(string query, int count)
            => Ok(await _executorService.GetAll(query, count));
    }
}
