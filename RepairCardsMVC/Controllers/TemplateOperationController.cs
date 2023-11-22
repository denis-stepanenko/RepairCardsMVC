using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RepairCardsMVC.Services;
using RepairCardsMVC.ViewModels;
using System.Data;

namespace RepairCardsMVC.Controllers
{
    public class TemplateOperationController : Controller
    {
        private readonly TemplateOperationService _templateOperationService;
        private readonly OperationService _operationService;
        private readonly CardService _cardService;
        private readonly CardOperationService _cardOperationService;

        public TemplateOperationController(
            TemplateOperationService templateOperationService, 
            OperationService operationService, 
            CardService cardService, 
            CardOperationService cardOperationService)
        {
            _templateOperationService = templateOperationService;
            _operationService = operationService;
            _cardService = cardService;
            _cardOperationService = cardOperationService;
        }

        public async Task<IActionResult> Index(
            int id,
            int templatesPageNumber = 1,
            string templatesFilter = "",
            int templateOperationsPageNumber = 1,
            string templateOperationsFilter = "")
        {
            ViewBag.Id = id;
            ViewBag.TemplatesPageNumber = templatesPageNumber;
            ViewBag.TemplatesFilter = templatesFilter;
            ViewBag.TemplateOperationsPageNumber = templateOperationsPageNumber;
            ViewBag.TemplateOperationsFilter = templateOperationsFilter;

            return View(await _templateOperationService.GetAll(id, templateOperationsPageNumber, templateOperationsFilter));
        }

        public async Task<IActionResult> Create(
            int templateId,
            int templatesPageNumber = 1,
            string templatesFilter = "",
            int templateOperationsPageNumber = 1,
            string templateOperationsFilter = "",
            int operationsPageNumber = 1,
            string operationsFilter = "")
        {
            ViewBag.Items = await _operationService.GetAll(operationsPageNumber, operationsFilter);
            ViewBag.TemplateId = templateId;
            ViewBag.TemplatesPageNumber = templatesPageNumber;
            ViewBag.TemplatesFilter = templatesFilter;
            ViewBag.TemplateOperationsPageNumber = templateOperationsPageNumber;
            ViewBag.TemplateOperationsFilter = templateOperationsFilter;
            ViewBag.OperationsPageNumber = operationsPageNumber;
            ViewBag.OperationsFilter = operationsFilter;
            return View();
        }

        [Authorize(Roles = "tb, ooiot, creator, otk, prb")]
        [HttpPost]
        public async Task<IActionResult> Create(
            CreateTemplateOperationViewModel model,
            int templateId,
            int templatesPageNumber = 1,
            string templatesFilter = "",
            int templateOperationsPageNumber = 1,
            string templateOperationsFilter = "",
            int operationsPageNumber = 1,
            string operationsFilter = "")
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Items = await _operationService.GetAll(operationsPageNumber, operationsFilter);
                ViewBag.TemplateId = templateId;
                ViewBag.TemplatesPageNumber = templatesPageNumber;
                ViewBag.TemplatesFilter = templatesFilter;
                ViewBag.TemplateOperationsPageNumber = templateOperationsPageNumber;
                ViewBag.TemplateOperationsFilter = templateOperationsFilter;
                ViewBag.OperationsPageNumber = operationsPageNumber;
                ViewBag.OperationsFilter = operationsFilter;

                return View(model);
            }

            var ids = model.Ids.Select(x => int.Parse(x)).ToList();

            await _templateOperationService.AddRange(ids, templateId, model.Count);

            return RedirectToAction("Index", new
            {
                id = templateId,
                templatesPageNumber = templatesPageNumber,
                templatesFilter = templatesFilter,
                templateOperationsPageNumber = templateOperationsPageNumber,
                templateOperationsFilter = templateOperationsFilter,
                operationsPageNumber = operationsPageNumber,
                operationsFilter = operationsFilter
            });
        }

        public async Task<IActionResult> Edit(
            int id,
            int templatesPageNumber = 1,
            string templatesFilter = "",
            int templateOperationsPageNumber = 1,
            string templateOperationsFilter = "")
        {
            var item = await _templateOperationService.Get(id);

            if (item == null)
                return NotFound();

            ViewBag.Item = item;
            ViewBag.TemplateId = item.TemplateId;
            ViewBag.TemplatesPageNumber = templatesPageNumber;
            ViewBag.TemplatesFilter = templatesFilter;
            ViewBag.TemplateOperationsPageNumber = templateOperationsPageNumber;
            ViewBag.TemplateOperationsFilter = templateOperationsFilter;

            return View(new EditTemplateOperationViewModel { Id = item.Id, Count = item.Count });
        }

        [Authorize(Roles = "tb, ooiot, creator, otk, prb")]
        [HttpPost]
        public async Task<IActionResult> Edit(
            EditTemplateOperationViewModel model,
            int templateId,
            int templatesPageNumber = 1,
            string templatesFilter = "",
            int templateOperationsPageNumber = 1,
            string templateOperationsFilter = "")
        {
            if (!ModelState.IsValid)
            {
                var operation = await _templateOperationService.Get(model.Id);

                if (operation == null)
                    return NotFound();

                ViewBag.Item = operation;
                ViewBag.TemplateId = operation.TemplateId;
                ViewBag.TemplatesPageNumber = templatesPageNumber;
                ViewBag.TemplatesFilter = templatesFilter;
                ViewBag.TemplateOperationsPageNumber = templateOperationsPageNumber;
                ViewBag.TemplateOperationsFilter = templateOperationsFilter;

                return View(model);
            }

            await _templateOperationService.Edit(model.Id, model.Count);

            return RedirectToAction("Index", new
            {
                id = templateId,
                templatesPageNumber = templatesPageNumber,
                templatesFilter = templatesFilter,
                templateOperationsPageNumber = templateOperationsPageNumber,
                templateOperationsFilter = templateOperationsFilter
            });
        }

        [Authorize(Roles = "tb, ooiot, creator, otk, prb")]
        public async Task<IActionResult> Delete(
            int id,
            int templateId,
            int templatesPageNumber = 1,
            string templatesFilter = "",
            int templateOperationsPageNumber = 1,
            string templateOperationsFilter = "")
        {
            await _templateOperationService.Remove(id);

            return RedirectToAction("Index", new
            {
                id = templateId,
                templatesPageNumber = templatesPageNumber,
                templatesFilter = templatesFilter,
                templateOperationsPageNumber = templateOperationsPageNumber,
                templateOperationsFilter = templateOperationsFilter
            });
        }

        public IActionResult CreateByCard(
            int templateId,
            int templatesPageNumber = 1,
            string templatesFilter = "",
            int templateOperationsPageNumber = 1,
            string templateOperationsFilter = "")
        {
            ViewBag.TemplateId = templateId;
            ViewBag.TemplatesPageNumber = templatesPageNumber;
            ViewBag.TemplatesFilter = templatesFilter;
            ViewBag.TemplateOperationsPageNumber = templateOperationsPageNumber;
            ViewBag.TemplateOperationsFilter = templateOperationsFilter;

            return View();
        }

        [Authorize(Roles = "tb, ooiot, creator, otk, prb")]
        [HttpPost]
        public async Task<IActionResult> CreateByCard(
            CreateTemplateOperationByCardViewModel model,
            int templateId,
            int templatesPageNumber = 1,
            string templatesFilter = "",
            int templateOperationsPageNumber = 1,
            string templateOperationsFilter = "")
        {
            if(!ModelState.IsValid)
            {
                ViewBag.TemplateId = templateId;
                ViewBag.TemplatesPageNumber = templatesPageNumber;
                ViewBag.TemplatesFilter = templatesFilter;
                ViewBag.TemplateOperationsPageNumber = templateOperationsPageNumber;
                ViewBag.TemplateOperationsFilter = templateOperationsFilter;

                return View(model);
            }

            var ids = model.Ids.Select(x => int.Parse(x)).ToList();

            await _templateOperationService.AddByCard(ids, templateId);

            return RedirectToAction("Index", new
            {
                id = templateId,
                templatesPageNumber = templatesPageNumber,
                templatesFilter = templatesFilter,
                templateOperationsPageNumber = templateOperationsPageNumber,
                templateOperationsFilter = templateOperationsFilter
            });
        }

        [Authorize(Roles = "tb, ooiot, creator, otk, prb")]
        public async Task<IActionResult> MoveUp(
            int id,
            int templateId,
            int templatesPageNumber = 1,
            string templatesFilter = "",
            int templateOperationsPageNumber = 1,
            string templateOperationsFilter = "")
        {
            await _templateOperationService.MoveUp(id);

            return RedirectToAction("Index", new
            {
                id = templateId,
                templatesPageNumber = templatesPageNumber,
                templatesFilter = templatesFilter,
                templateOperationsPageNumber = templateOperationsPageNumber,
                templateOperationsFilter = templateOperationsFilter
            });
        }

        [Authorize(Roles = "tb, ooiot, creator, otk, prb")]
        public async Task<IActionResult> MoveDown(
            int id,
            int templateId,
            int templatesPageNumber = 1,
            string templatesFilter = "",
            int templateOperationsPageNumber = 1,
            string templateOperationsFilter = "")
        {
            await _templateOperationService.MoveDown(id);

            return RedirectToAction("Index", new
            {
                id = templateId,
                templatesPageNumber = templatesPageNumber,
                templatesFilter = templatesFilter,
                templateOperationsPageNumber = templateOperationsPageNumber,
                templateOperationsFilter = templateOperationsFilter
            });
        }

        [HttpPost]
        public IActionResult GetCards()
            => Ok(_cardService.GetCards(Request));

        public async Task<IActionResult> GetOperationsByCard(int cardId)
            => Ok(await _cardOperationService.GetFactOperationsByCard(cardId));   
    }
}
