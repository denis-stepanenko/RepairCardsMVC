using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RepairCardsMVC.Services;
using RepairCardsMVC.ViewModels;
using System.Data;

namespace RepairCardsMVC.Controllers
{
    public class CardOwnProductRepairOperationController : Controller
    {
        private readonly CardOwnProductRepairOperationService _cardOwnProductReapirOperationService;
        private readonly ExecutorService _executorService;
        private readonly OperationService _operationService;

        public CardOwnProductRepairOperationController(
            CardOwnProductRepairOperationService cardOwnProductRepairOperationService,
            ExecutorService executorService,
            OperationService operationService)
        {
            _cardOwnProductReapirOperationService = cardOwnProductRepairOperationService;
            _executorService = executorService;
            _operationService = operationService;
        }

        public IActionResult Create(
            int productId,
            int cardId,
            int cardsPageNumber,
            string cardsFilter)
        {
            ViewBag.ProductId = productId;
            ViewBag.CardId = cardId;
            ViewBag.CardsPageNumber = cardsPageNumber;
            ViewBag.CardsFilter = cardsFilter;

            return View();
        }

        [Authorize(Roles = "tb, ooiot, creator, prb")]
        [HttpPost]
        public async Task<IActionResult> Create(
            CreateCardOwnProductRepairOperationViewModel model,
            int productId,
            int cardId,
            int cardsPageNumber,
            string cardsFilter = "")
        {
            if (!ModelState.IsValid)
            {
                var executor = await _executorService.Get(model.ExecutorId ?? 0);

                ViewBag.Executor = executor;
                ViewBag.ProductId = productId;
                ViewBag.CardId = cardId;
                ViewBag.CardsPageNumber = cardsPageNumber;
                ViewBag.CardsFilter = cardsFilter;

                return View(model);
            }

            var ids = model.Ids.Select(x => int.Parse(x)).ToList();

            await _cardOwnProductReapirOperationService.AddRange(ids, productId, model.Count, model.Date, model.ExecutorId ?? 0);

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
            var item = await _cardOwnProductReapirOperationService.Get(id);

            if (item == null)
                return NotFound();

            var executor = await _executorService.Get(item.ExecutorId);

            ViewBag.Executor = executor;
            ViewBag.Item = item;
            ViewBag.CardId = cardId;
            ViewBag.CardsPageNumber = cardsPageNumber;
            ViewBag.CardsFilter = cardsFilter;

            return View(new EditCardOwnProductRepairOperationViewModel { Count = item.Count, Date = item.Date, ExecutorId = item.ExecutorId });
        }

        [Authorize(Roles = "tb, ooiot, creator, prb")]
        [HttpPost]
        public async Task<IActionResult> Edit(
            EditCardOwnProductRepairOperationViewModel model,
            int id,
            int cardId,
            int cardOwnProductId,
            int cardsPageNumber,
            string cardsFilter = "")
        {
            if (!ModelState.IsValid)
            {
                var operation = await _cardOwnProductReapirOperationService.Get(id);

                if (operation == null)
                    return NotFound();

                var executor = await _executorService.Get(operation.ExecutorId);

                ViewBag.Executor = executor;
                ViewBag.Item = operation;
                ViewBag.CardId = cardId;
                ViewBag.CardsPageNumber = cardsPageNumber;
                ViewBag.CardsFilter = cardsFilter;

                return View(model);
            }

            await _cardOwnProductReapirOperationService.Edit(id, model.Count, model.ExecutorId ?? 0, model.Date);

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
            await _cardOwnProductReapirOperationService.Remove(id);

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
            await _cardOwnProductReapirOperationService.Confirm(id, "", "");

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
            await _cardOwnProductReapirOperationService.Unconfirm(id);

            return RedirectToAction(actionName: "Edit", controllerName: "CardOwnProduct",
                new
                {
                    id = cardOwnProductId,
                    cardId = cardId,
                    cardsPageNumber = cardsPageNumber,
                    cardsFilter = cardsFilter
                });
        }

        public IActionResult GetOperations()
            => Ok(_operationService.GetAll(Request));

        public async Task<IActionResult> FindExecutors(string query, int count)
            => Ok(await _executorService.GetAll(query, count));
    }
}
