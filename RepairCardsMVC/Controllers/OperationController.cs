using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RepairCardsMVC.Models;
using RepairCardsMVC.Services;

namespace RepairCardsMVC.Controllers
{
    public class OperationController : Controller
    {
        private readonly OperationService _operationService;
        private readonly ProductionOperationService _productionOperationService;

        public OperationController(OperationService operationService, ProductionOperationService productionOperationService)
        {
            _operationService = operationService;
            _productionOperationService = productionOperationService;
        }

        public async Task<IActionResult> Index(int pageNumber = 1, string filter = "")
        {
            ViewBag.PageNumber = pageNumber;
            ViewBag.Filter = filter;

            return View(await _operationService.GetAll(pageNumber, filter));
        }

        public async Task<IActionResult> Create(int pageNumber = 1, string filter = "")
        {
            var newCode = await _operationService.GenerateNewCode();

            ViewBag.PageNumber = pageNumber;
            ViewBag.Filter = filter;

            return View(new Operation { Code = newCode });
        }

        [Authorize(Roles = "ooiot")]
        [HttpPost]
        public async Task<IActionResult> Create(
            Operation item,
            int pageNumber = 1, 
            string filter = "")
        {
            if(!ModelState.IsValid)
            {
                ViewBag.PageNumber = pageNumber;
                ViewBag.Filter = filter;

                return View(item);
            }

            await _operationService.Add(item);

            return RedirectToAction("Index", new { pageNumber = pageNumber, filter = filter });
        }

        public async Task<IActionResult> Edit(int id, int pageNumber = 1, string filter = "")
        {
            var item = await _operationService.Get(id);

            if (item == null)
                return NotFound();

            var productionOperation = await _productionOperationService.Get(item.ProductionOperationCode);

            if (productionOperation == null)
                return NotFound();

            ViewBag.ProductionOperation = productionOperation;
            ViewBag.PageNumber = pageNumber;
            ViewBag.Filter = filter;

            return View(item);
        }

        [Authorize(Roles = "ooiot")]
        [HttpPost]
        public async Task<IActionResult> Edit(
            Operation item,
            int pageNumber = 1, 
            string filter = "")
        {
            if(!ModelState.IsValid)
            {
                var productionOperation = await _productionOperationService.Get(item.ProductionOperationCode);

                if (productionOperation == null)
                    return NotFound();

                ViewBag.ProductionOperation = productionOperation;
                ViewBag.PageNumber = pageNumber;
                ViewBag.Filter = filter;

                return View(item);
            }

            await _operationService.Edit(item);

            return RedirectToAction("Index", new { pageNumber = pageNumber, filter = filter });
        }

        public async Task<IActionResult> Delete(int id, int pageNumber = 1, string filter = "")
        {
            var item = await _operationService.Get(id);

            if(item == null)
                return NotFound();

            ViewBag.PageNumber = pageNumber;
            ViewBag.Filter = filter;

            return View(item);
        }

        [Authorize(Roles = "ooiot")]
        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id, int pageNumber = 1, string filter = "")
        {
            await _operationService.Remove(id);

            return RedirectToAction("Index", new { pageNumber = pageNumber, filter = filter });
        }

        public async Task<IActionResult> FindProductionOperations(string query, int count)
            => Ok(await _productionOperationService.GetAll(query, count));

        public async Task<IActionResult> GenerateNewCode() =>
            Ok(await _operationService.GenerateNewCode());

        [AcceptVerbs("GET", "POST")]
        public async Task<IActionResult> VerifyCode(string code, int id)
            => Json(!await _operationService.IsThereOperationWithSuchCode(code, id));  
    }
}
