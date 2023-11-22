using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RepairCardsMVC.Services;
using RepairCardsMVC.ViewModels;
using System.Data;

namespace RepairCardsMVC.Controllers
{
    public class CardOwnProductOperationController : Controller
    {
        private readonly CardOwnProductOperationService _cardOwnProductOperationService;
        private readonly ProductOperationService _productOperationService;
        private readonly CardOwnProductService _cardOwnProductService;
        private readonly ExecutorService _executorService;

        public CardOwnProductOperationController(
            CardOwnProductOperationService cardOwnProductOperationService,
            ProductOperationService productOperationService,
            CardOwnProductService cardOwnProductService,
            ExecutorService executorService)
        {
            _cardOwnProductOperationService = cardOwnProductOperationService;
            _productOperationService = productOperationService;
            _cardOwnProductService = cardOwnProductService;
            _executorService = executorService;
        }

        public async Task<IActionResult> Create(
            int productId,
            int cardId,
            int cardsPageNumber,
            string cardsFilter)
        {
            var product = await _cardOwnProductService.Get(productId);

            if (product == null)
                return NotFound();

            var operations = _productOperationService.GetAll(product.Code, product.Route ?? "");

            ViewBag.Operations = operations;
            ViewBag.ProductId = productId;
            ViewBag.CardId = cardId;
            ViewBag.CardsPageNumber = cardsPageNumber;
            ViewBag.CardsFilter = cardsFilter;

            return View();
        }

        [Authorize(Roles = "tb, ooiot, creator, prb")]
        [HttpPost]
        public async Task<IActionResult> Create(
            CreateCardOwnProductOperationViewModel model,
            int cardId,
            int executorId,
            DateTime date,
            int productId, 
            int cardsPageNumber, 
            string cardsFilter = "")
        {
            if (!ModelState.IsValid)
            {
                var product = await _cardOwnProductService.Get(productId);

                if (product == null)
                    return NotFound();

                var operations = _productOperationService.GetAll(product.Code, product.Route ?? "");

                var executor = await _executorService.Get(model.ExecutorId ?? 0);

                ViewBag.Executor = executor;
                ViewBag.Operations = operations;
                ViewBag.ProductId = productId;
                ViewBag.CardId = cardId;
                ViewBag.CardsPageNumber = cardsPageNumber;
                ViewBag.CardsFilter = cardsFilter;

                return View(model);
            }

            var ids = model.Ids.Select(x => int.Parse(x)).ToList();

            await _cardOwnProductOperationService.AddByProduct(ids, productId, executorId, date);

            return RedirectToAction(actionName: "Edit", controllerName: "CardOwnProduct",
                new
                {
                    id = productId,
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
            var item = await _cardOwnProductOperationService.Get(id);

            if (item == null)
                return NotFound();
            
            var executor = await _executorService.Get(item.ExecutorId ?? 0);

            ViewBag.Executor = executor;
            ViewBag.Item = item;
            ViewBag.CardId = cardId;
            ViewBag.CardsPageNumber = cardsPageNumber;
            ViewBag.CardsFilter = cardsFilter;

            return View(new EditCardOwnProductOperationViewModel { Date = item.Date, ExecutorId = item.ExecutorId });
        }

        [Authorize(Roles = "tb, ooiot, creator, prb")]
        [HttpPost]
        public async Task<IActionResult> Edit(
            EditCardOwnProductOperationViewModel model,
            int id,
            int cardId,
            int cardOwnProductId,
            int cardsPageNumber,
            string cardsFilter = "")
        {
            if (!ModelState.IsValid)
            {
                var operation = await _cardOwnProductOperationService.Get(id);

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

            await _cardOwnProductOperationService.Edit(id, (DateTime)model.Date, (int)model.ExecutorId);

            return RedirectToAction(actionName: "Edit", controllerName: "CardOwnProduct",
                new
                {
                    id = cardOwnProductId,
                    cardId = cardId,
                    cardsPageNumber = cardsPageNumber,
                    cardsFilter = cardsFilter
                });
        }

        [Authorize(Roles = "tb, ooiot, creator, prb")]
        public async Task<IActionResult> Delete(
            int id,
            int cardId,
            int cardOwnProductId,
            int cardsPageNumber,
            string cardsFilter)
        {
            await _cardOwnProductOperationService.Remove(id);

            return RedirectToAction(actionName: "Edit", controllerName: "CardOwnProduct",
                new
                {
                    id = cardOwnProductId,
                    cardId = cardId,
                    cardsPageNumber = cardsPageNumber,
                    cardsFilter = cardsFilter
                });
        }

        [Authorize(Roles = "tb, ooiot, creator, prb")]
        public async Task<IActionResult> Confirm(
            int id,
            int cardId,
            int cardOwnProductId,
            int cardsPageNumber,
            string cardsFilter)
        {
            await _cardOwnProductOperationService.Confirm(id, "", "");

            return RedirectToAction(actionName: "Edit", controllerName: "CardOwnProduct",
                new
                {
                    id = cardOwnProductId,
                    cardId = cardId,
                    cardsPageNumber = cardsPageNumber,
                    cardsFilter = cardsFilter
                });
        }

        [Authorize(Roles = "tb, ooiot, creator, prb")]
        public async Task<IActionResult> Unconfirm(
            int id,
            int cardId,
            int cardOwnProductId,
            int cardsPageNumber,
            string cardsFilter)
        {
            await _cardOwnProductOperationService.Unconfirm(id, "");

            return RedirectToAction(actionName: "Edit", controllerName: "CardOwnProduct",
                new
                {
                    id = cardOwnProductId,
                    cardId = cardId,
                    cardsPageNumber = cardsPageNumber,
                    cardsFilter = cardsFilter
                });
        }

        public async Task<IActionResult> FindExecutors(string query, int count)
            => Ok(await _executorService.GetAll(query, count));
    }
}
