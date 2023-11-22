using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RepairCardsMVC.Services;
using RepairCardsMVC.ViewModels;

namespace RepairCardsMVC.Controllers
{
    public class CardOperationController : Controller
    {
        private readonly CardOperationService _cardOperationService;
        private readonly OperationService _operationService;
        private readonly ExecutorService _executorService;
        private readonly TemplateOperationService _templateOperationService;
        private readonly CardService _cardService;
        private readonly TemplateService _templateService;

        public CardOperationController(
            CardOperationService cardOperationService,
            OperationService operationService,
            ExecutorService executorService,
            TemplateOperationService templateOperationService,
            CardService cardService,
            TemplateService templateService)
        {
            _cardOperationService = cardOperationService;
            _operationService = operationService;
            _executorService = executorService;
            _templateOperationService = templateOperationService;
            _cardService = cardService;
            _templateService = templateService;
        }

        public async Task<IActionResult> Index(
            int cardId,
            int type,
            int cardsPageNumber = 1,
            string cardsFilter = "",
            int cardOperationsPageNumber = 1,
            string cardOperationsFilter = "")
        {
            ViewBag.CardId = cardId;
            ViewBag.Type = type;
            ViewBag.CardsPageNumber = cardsPageNumber;
            ViewBag.CardsFilter = cardsFilter;
            ViewBag.CardOperationsPageNumber = cardOperationsPageNumber;
            ViewBag.CardOperationsFilter = cardOperationsFilter;

            return View(type == 0 ? "PlanOperations" : "FactOperations",
                await _cardOperationService.GetAll(cardId, type, cardOperationsPageNumber, cardOperationsFilter));
        }

        public async Task<IActionResult> Create(
            int cardId,
            int type,
            int cardsPageNumber = 1,
            string cardsFilter = "",
            int cardOperationsPageNumber = 1,
            string cardOperationsFilter = "",
            int operationsPageNumber = 1,
            string operationsFilter = "")
        {
            var operations = await _operationService.GetAll(operationsPageNumber, operationsFilter);

            ViewBag.Items = operations;
            ViewBag.CardId = cardId;
            ViewBag.Type = type;
            ViewBag.CardsPageNumber = cardsPageNumber;
            ViewBag.CardsFilter = cardsFilter;
            ViewBag.CardOperationsPageNumber = cardOperationsPageNumber;
            ViewBag.CardOperationsFilter = cardOperationsFilter;
            ViewBag.OperationsPageNumber = operationsPageNumber;
            ViewBag.OperationsFilter = operationsFilter;

            return View();
        }

        [Authorize(Roles = "tb, ooiot, creator, prb")]
        [HttpPost]
        public async Task<IActionResult> Create(
            CreateCardOperationViewModel model,
            int cardId,
            int type,
            int cardsPageNumber = 1,
            string cardsFilter = "",
            int cardOperationsPageNumber = 1,
            string cardOperationsFilter = "",
            int operationsPageNumber = 1,
            string operationsFilter = "")
        {
            if (!ModelState.IsValid)
            {
                var operations = await _operationService.GetAll(operationsPageNumber, operationsFilter);

                var executor = await _executorService.Get(model.ExecutorId ?? 0);

                ViewBag.Items = operations;
                ViewBag.Executor = executor;
                ViewBag.CardId = cardId;
                ViewBag.Type = type;
                ViewBag.CardsPageNumber = cardsPageNumber;
                ViewBag.CardsFilter = cardsFilter;
                ViewBag.CardOperationsPageNumber = cardOperationsPageNumber;
                ViewBag.CardOperationsFilter = cardOperationsFilter;
                ViewBag.OperationsPageNumber = operationsPageNumber;
                ViewBag.OperationsFilter = operationsFilter;

                return View(model);
            }

            var ids = model.SelectedItems.Select(x => int.Parse(x)).ToList();

            await _cardOperationService.AddRange(ids, cardId, model.Count, type, model.ExecutorId ?? 0, model.Date);

            return RedirectToAction("Index", new
            {
                cardId = cardId,
                type = type,
                cardsPageNumber = cardsPageNumber,
                cardsFilter = cardsFilter,
                cardOperationsPageNumber = cardOperationsPageNumber,
                cardOperationsFilter = cardOperationsFilter,
                operationsPageNumber = operationsPageNumber,
                operationsFilter = operationsFilter
            });
        }

        public async Task<IActionResult> Edit(
            int id,
            int cardsPageNumber = 1,
            string cardsFilter = "",
            int cardOperationsPageNumber = 1,
            string cardOperationsFilter = "")
        {
            var item = await _cardOperationService.Get(id);

            if (item == null)
                return NotFound();

            var executor = await _executorService.Get(item.ExecutorId ?? 0);

            ViewBag.Item = item;
            ViewBag.Executor = executor;
            ViewBag.Id = id;
            ViewBag.CardId = item.CardId;
            ViewBag.CardsPageNumber = cardsPageNumber;
            ViewBag.CardsFilter = cardsFilter;
            ViewBag.CardOperationsPageNumber = cardOperationsPageNumber;
            ViewBag.CardOperationsFilter = cardOperationsFilter;

            return View(new EditCardOperationViewModel { Count = item.Count, Date = item.Date, ExecutorId = item.ExecutorId });
        }

        [Authorize(Roles = "tb, ooiot, creator, prb")]
        [HttpPost]
        public async Task<IActionResult> Edit(
            EditCardOperationViewModel model,
            int id,
            int cardId,
            int type,
            int cardsPageNumber = 1,
            string cardsFilter = "",
            int cardOperationsPageNumber = 1,
            string cardOperationsFilter = "")
        {
            if (!ModelState.IsValid)
            {
                var item = await _cardOperationService.Get(id);

                if (item == null)
                    return NotFound();

                var executor = await _executorService.Get(model.ExecutorId ?? 0);

                ViewBag.Item = item;
                ViewBag.Executor = executor;
                ViewBag.Id = id;
                ViewBag.CardId = cardId;
                ViewBag.Type = type;
                ViewBag.CardsPageNumber = cardsPageNumber;
                ViewBag.CardsFilter = cardsFilter;
                ViewBag.CardOperationsPageNumber = cardOperationsPageNumber;
                ViewBag.CardOperationsFilter = cardOperationsFilter;

                return View(model);
            }

            await _cardOperationService.Edit(id, model.Count, model.ExecutorId ?? 0, model.Date);

            return RedirectToAction("Index", new
            {
                cardId = cardId,
                type = type,
                cardsPageNumber = cardsPageNumber,
                cardsFilter = cardsFilter,
                cardOperationsPageNumber = cardOperationsPageNumber,
                cardOperationsFilter = cardOperationsFilter
            });
        }

        public IActionResult CreateByPlanOperations(
            int cardId,
            int type,
            int cardsPageNumber = 1,
            string cardsFilter = "",
            int cardOperationsPageNumber = 1,
            string cardOperationsFilter = "",
            int operationsPageNumber = 1,
            string operationsFilter = "")
        {
            ViewBag.CardId = cardId;
            ViewBag.Type = type;
            ViewBag.CardsPageNumber = cardsPageNumber;
            ViewBag.CardsFilter = cardsFilter;
            ViewBag.CardOperationsPageNumber = cardOperationsPageNumber;
            ViewBag.CardOperationsFilter = cardOperationsFilter;
            ViewBag.OperationsPageNumber = operationsPageNumber;
            ViewBag.OperationsFilter = operationsFilter;

            return View();
        }

        [Authorize(Roles = "tb, ooiot, creator, prb")]
        [HttpPost]
        public async Task<IActionResult> CreateByPlanOperations(
            CreateCardOperationByPlanViewModel model,
            int cardId,
            int type,
            int cardsPageNumber = 1,
            string cardsFilter = "",
            int cardOperationsPageNumber = 1,
            string cardOperationsFilter = "",
            int operationsPageNumber = 1,
            string operationsFilter = "")
        {
            if (!ModelState.IsValid)
            {
                var executor = await _executorService.Get(model.ExecutorId ?? 0);

                ViewBag.Executor = executor;
                ViewBag.CardId = cardId;
                ViewBag.Type = type;
                ViewBag.CardsPageNumber = cardsPageNumber;
                ViewBag.CardsFilter = cardsFilter;
                ViewBag.CardOperationsPageNumber = cardOperationsPageNumber;
                ViewBag.CardOperationsFilter = cardOperationsFilter;
                ViewBag.OperationsPageNumber = operationsPageNumber;
                ViewBag.OperationsFilter = operationsFilter;

                return View(model);
            }

            var ids = model.Ids.Select(x => int.Parse(x)).ToList();

            await _cardOperationService.AddByCard(ids, cardId, model.Date, model.ExecutorId ?? 0, 1);

            return RedirectToAction("Index", new
            {
                cardId = cardId,
                type = type,
                cardsPageNumber = cardsPageNumber,
                cardsFilter = cardsFilter,
                cardOperationsPageNumber = cardOperationsPageNumber,
                cardOperationsFilter = cardOperationsFilter,
                operationsPageNumber = operationsPageNumber,
                operationsFilter = operationsFilter
            });
        }

        public IActionResult CreateFactOperationByCard(
            int cardId,
            int type,
            int cardsPageNumber = 1,
            string cardsFilter = "",
            int cardOperationsPageNumber = 1,
            string cardOperationsFilter = "",
            int operationsPageNumber = 1,
            string operationsFilter = "")
        {
            ViewBag.CardId = cardId;
            ViewBag.Type = type;
            ViewBag.CardsPageNumber = cardsPageNumber;
            ViewBag.CardsFilter = cardsFilter;
            ViewBag.CardOperationsPageNumber = cardOperationsPageNumber;
            ViewBag.CardOperationsFilter = cardOperationsFilter;
            ViewBag.OperationsPageNumber = operationsPageNumber;
            ViewBag.OperationsFilter = operationsFilter;

            return View();
        }

        [Authorize(Roles = "tb, ooiot, creator, prb")]
        [HttpPost]
        public async Task<IActionResult> CreateFactOperationByCard(
            CreateFactCardOperationByCardViewModel model,
            int cardId,
            int type,
            int cardsPageNumber = 1,
            string cardsFilter = "",
            int cardOperationsPageNumber = 1,
            string cardOperationsFilter = "",
            int operationsPageNumber = 1,
            string operationsFilter = "")
        {
            if (!ModelState.IsValid)
            {
                var executor = await _executorService.Get(model.ExecutorId ?? 0);

                ViewBag.Executor = executor;
                ViewBag.CardId = cardId;
                ViewBag.Type = type;
                ViewBag.CardsPageNumber = cardsPageNumber;
                ViewBag.CardsFilter = cardsFilter;
                ViewBag.CardOperationsPageNumber = cardOperationsPageNumber;
                ViewBag.CardOperationsFilter = cardOperationsFilter;
                ViewBag.OperationsPageNumber = operationsPageNumber;
                ViewBag.OperationsFilter = operationsFilter;

                return View(model);
            }

            var ids = model.Ids.Select(x => int.Parse(x)).ToList();

            await _cardOperationService.AddByCard(ids, cardId, model.Date, model.ExecutorId ?? 0, 1);

            return RedirectToAction("Index", new
            {
                cardId = cardId,
                type = type,
                cardsPageNumber = cardsPageNumber,
                cardsFilter = cardsFilter,
                cardOperationsPageNumber = cardOperationsPageNumber,
                cardOperationsFilter = cardOperationsFilter,
                operationsPageNumber = operationsPageNumber,
                operationsFilter = operationsFilter
            });
        }

        public IActionResult CreatePlanOperationByCard(
            int cardId,
            int type,
            int cardsPageNumber = 1,
            string cardsFilter = "",
            int cardOperationsPageNumber = 1,
            string cardOperationsFilter = "",
            int operationsPageNumber = 1,
            string operationsFilter = "")
        {
            ViewBag.CardId = cardId;
            ViewBag.Type = type;
            ViewBag.CardsPageNumber = cardsPageNumber;
            ViewBag.CardsFilter = cardsFilter;
            ViewBag.CardOperationsPageNumber = cardOperationsPageNumber;
            ViewBag.CardOperationsFilter = cardOperationsFilter;
            ViewBag.OperationsPageNumber = operationsPageNumber;
            ViewBag.OperationsFilter = operationsFilter;

            return View();
        }

        [Authorize(Roles = "tb, ooiot, creator, prb")]
        [HttpPost]
        public async Task<IActionResult> CreatePlanOperationByCard(
            CreatePlanCardOperationByCardViewModel model,
            int cardId,
            int type,
            int cardsPageNumber = 1,
            string cardsFilter = "",
            int cardOperationsPageNumber = 1,
            string cardOperationsFilter = "",
            int operationsPageNumber = 1,
            string operationsFilter = "")
        {
            if (!ModelState.IsValid)
            {
                var executor = await _executorService.Get(model.ExecutorId ?? 0);

                ViewBag.Executor = executor;
                ViewBag.CardId = cardId;
                ViewBag.Type = type;
                ViewBag.CardsPageNumber = cardsPageNumber;
                ViewBag.CardsFilter = cardsFilter;
                ViewBag.CardOperationsPageNumber = cardOperationsPageNumber;
                ViewBag.CardOperationsFilter = cardOperationsFilter;
                ViewBag.OperationsPageNumber = operationsPageNumber;
                ViewBag.OperationsFilter = operationsFilter;

                return View(model);
            }

            var ids = model.Ids.Select(x => int.Parse(x)).ToList();

            await _cardOperationService.AddByCard(ids, cardId, model.Date, model.ExecutorId ?? 0, 0);

            return RedirectToAction("Index", new
            {
                cardId = cardId,
                type = type,
                cardsPageNumber = cardsPageNumber,
                cardsFilter = cardsFilter,
                cardOperationsPageNumber = cardOperationsPageNumber,
                cardOperationsFilter = cardOperationsFilter,
                operationsPageNumber = operationsPageNumber,
                operationsFilter = operationsFilter
            });
        }

        public IActionResult CreateFactOperationByTemplate(
            int cardId,
            int type,
            int cardsPageNumber = 1,
            string cardsFilter = "",
            int cardOperationsPageNumber = 1,
            string cardOperationsFilter = "",
            int operationsPageNumber = 1,
            string operationsFilter = "")
        {
            ViewBag.CardId = cardId;
            ViewBag.Type = type;
            ViewBag.CardsPageNumber = cardsPageNumber;
            ViewBag.CardsFilter = cardsFilter;
            ViewBag.CardOperationsPageNumber = cardOperationsPageNumber;
            ViewBag.CardOperationsFilter = cardOperationsFilter;
            ViewBag.OperationsPageNumber = operationsPageNumber;
            ViewBag.OperationsFilter = operationsFilter;

            return View();
        }

        [Authorize(Roles = "tb, ooiot, creator, prb")]
        [HttpPost]
        public async Task<IActionResult> CreateFactOperationByTemplate(
            CreateFactCardOperationByTemplateViewModel model,
            int cardId,
            int type,
            int cardsPageNumber = 1,
            string cardsFilter = "",
            int cardOperationsPageNumber = 1,
            string cardOperationsFilter = "",
            int operationsPageNumber = 1,
            string operationsFilter = "")
        {
            if (!ModelState.IsValid)
            {
                var executor = await _executorService.Get(model.ExecutorId ?? 0);

                ViewBag.Executor = executor;
                ViewBag.CardId = cardId;
                ViewBag.Type = type;
                ViewBag.CardsPageNumber = cardsPageNumber;
                ViewBag.CardsFilter = cardsFilter;
                ViewBag.CardOperationsPageNumber = cardOperationsPageNumber;
                ViewBag.CardOperationsFilter = cardOperationsFilter;
                ViewBag.OperationsPageNumber = operationsPageNumber;
                ViewBag.OperationsFilter = operationsFilter;

                return View(model);
            }

            var ids = model.Ids.Select(x => int.Parse(x)).ToList();

            await _cardOperationService.AddByTemplate(ids, cardId, 1, model.ExecutorId ?? 0, model.Date);

            return RedirectToAction("Index", new
            {
                cardId = cardId,
                type = type,
                cardsPageNumber = cardsPageNumber,
                cardsFilter = cardsFilter,
                cardOperationsPageNumber = cardOperationsPageNumber,
                cardOperationsFilter = cardOperationsFilter,
                operationsPageNumber = operationsPageNumber,
                operationsFilter = operationsFilter
            });
        }

        public IActionResult CreatePlanOperationByTemplate(
            int cardId,
            int type,
            int cardsPageNumber = 1,
            string cardsFilter = "",
            int cardOperationsPageNumber = 1,
            string cardOperationsFilter = "",
            int operationsPageNumber = 1,
            string operationsFilter = "")
        {
            ViewBag.CardId = cardId;
            ViewBag.Type = type;
            ViewBag.CardsPageNumber = cardsPageNumber;
            ViewBag.CardsFilter = cardsFilter;
            ViewBag.CardOperationsPageNumber = cardOperationsPageNumber;
            ViewBag.CardOperationsFilter = cardOperationsFilter;
            ViewBag.OperationsPageNumber = operationsPageNumber;
            ViewBag.OperationsFilter = operationsFilter;

            return View();
        }

        [Authorize(Roles = "tb, ooiot, creator, prb")]
        [HttpPost]
        public async Task<IActionResult> CreatePlanOperationByTemplate(
           CreatePlanCardOperationByTemplateViewModel model,
           int cardId,
           int type,
           int cardsPageNumber = 1,
           string cardsFilter = "",
           int cardOperationsPageNumber = 1,
           string cardOperationsFilter = "",
           int operationsPageNumber = 1,
           string operationsFilter = "")
        {
            if (!ModelState.IsValid)
            {
                var executor = await _executorService.Get(model.ExecutorId ?? 0);

                ViewBag.Executor = executor;
                ViewBag.CardId = cardId;
                ViewBag.Type = type;
                ViewBag.CardsPageNumber = cardsPageNumber;
                ViewBag.CardsFilter = cardsFilter;
                ViewBag.CardOperationsPageNumber = cardOperationsPageNumber;
                ViewBag.CardOperationsFilter = cardOperationsFilter;
                ViewBag.OperationsPageNumber = operationsPageNumber;
                ViewBag.OperationsFilter = operationsFilter;

                return View(model);
            }

            var ids = model.Ids.Select(x => int.Parse(x)).ToList();

            await _cardOperationService.AddByTemplate(ids, cardId, 0, model.ExecutorId ?? 0, model.Date);

            return RedirectToAction("Index", new
            {
                cardId = cardId,
                type = type,
                cardsPageNumber = cardsPageNumber,
                cardsFilter = cardsFilter,
                cardOperationsPageNumber = cardOperationsPageNumber,
                cardOperationsFilter = cardOperationsFilter,
                operationsPageNumber = operationsPageNumber,
                operationsFilter = operationsFilter
            });
        }

        [Authorize(Roles = "tb, ooiot, creator, prb")]
        public async Task<IActionResult> Delete(
            int id,
            int cardId,
            int type,
            int cardsPageNumber = 1,
            string cardsFilter = "",
            int cardOperationsPageNumber = 1,
            string cardOperationsFilter = "")
        {
            await _cardOperationService.Remove(id);

            return RedirectToAction("Index", new
            {
                cardId = cardId,
                type = type,
                cardsPageNumber = cardsPageNumber,
                cardsFilter = cardsFilter,
                cardOperationsPageNumber = cardOperationsPageNumber,
                cardOperationsFilter = cardOperationsFilter
            });
        }

        [Authorize(Roles = "tb, ooiot, creator, prb")]
        public async Task<IActionResult> ChangeDateAndExecutor(
            int? executorId,
            DateTime? date,
            List<string> ids,
            int cardId,
            int type,
            int cardsPageNumber = 1,
            string cardsFilter = "",
            int cardOperationsPageNumber = 1,
            string cardOperationsFilter = "",
            int operationsPageNumber = 1,
            string operationsFilter = "")
        {
            if (executorId != null && date != null)
            {
                var numberIds = ids.Select(x => int.Parse(x)).ToList();

                await _cardOperationService.ChangeDateAndExecutor(numberIds, (int)executorId, (DateTime)date);
            }
            else
                TempData["message"] = "Дата или исполнитель не указаны";

            return RedirectToAction("Index", new
            {
                cardId = cardId,
                type = type,
                cardsPageNumber = cardsPageNumber,
                cardsFilter = cardsFilter,
                cardOperationsPageNumber = cardOperationsPageNumber,
                cardOperationsFilter = cardOperationsFilter,
                operationsPageNumber = operationsPageNumber,
                operationsFilter = operationsFilter
            });
        }

        public async Task<IActionResult> GetOperationsByTemplate(int templateId)
        {
            var items = await _templateOperationService.GetAll(templateId);

            return Ok(items.Select(x => new
            {
                Id = x.Id,
                Code = x.Operation.Code,
                Name = x.Operation.Name,
                Labor = x.Operation.Labor,
                Price = x.Operation.Price,
                GroupName = x.Operation.GroupName,
                Department = x.Operation.Department,
                UnitName = x.Operation.UnitName,
                Description = x.Operation.Description,
                IsInactive = x.Operation.IsInactive,
                ProductionOperationCode = x.Operation.ProductionOperationCode,
                Count = x.Count
            }));
        }

        public async Task<IActionResult> FindExecutors(string query, int count)
            => Ok(await _executorService.GetAll(query, count));

        [HttpPost]
        public IActionResult GetPlanOperations(int cardId)
            => Ok(_cardOperationService.GetAll(Request, cardId, 0));

        [HttpPost]
        public IActionResult GetCards()
            => Ok(_cardService.GetCards(Request));

        [HttpPost]
        public IActionResult GetTemplates()
            => Ok(_templateService.GetAll(Request));

        public async Task<IActionResult> GetFactOperationsByCard(int cardId)
            => Ok(await _cardOperationService.GetFactOperationsByCard(cardId));
    }
}
