using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using RepairCardsMVC.Models;
using RepairCardsMVC.Services;

namespace RepairCardsMVC.Controllers
{
    public class RequestController : Controller
    {
        private readonly RequestService _requestService;
        private readonly RepairTypeService _repairTypeService;

        public RequestController(RequestService requestService, RepairTypeService repairTypeService)
        {
            _requestService = requestService;
            _repairTypeService = repairTypeService;
        }

        public async Task<IActionResult> Index(int pageNumber = 1, string filter = "")
        {
            ViewBag.PageNumber = pageNumber;
            ViewBag.Filter = filter;

            return View(await _requestService.GetAll(pageNumber, filter));
        }

        public async Task<IActionResult> Create(int pageNumber = 1, string filter = "")
        {
            var types = (await _repairTypeService.GetAll())
                .Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.Name });

            ViewBag.Types = types;
            ViewBag.PageNumber = pageNumber;
            ViewBag.Filters = filter;

            return View();
        }

        [Authorize(Roles = "tb, ooiot, creator, prb, peo, skie")]
        [HttpPost]
        public async Task<IActionResult> Create(
            Request item,
            int pageNumber = 1, 
            string filter = "")
        {
            if (!ModelState.IsValid)
            {
                var types = (await _repairTypeService.GetAll())
                        .Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.Name });

                ViewBag.Types = types;
                ViewBag.PageNumber = pageNumber;
                ViewBag.Filters = filter;

                return View(item);
            }

            await _requestService.Add(item);

            return RedirectToAction("Index", new { pageNumber = pageNumber, filter = filter });
        }

        public async Task<IActionResult> Edit(int id, int pageNumber = 1, string filter = "")
        {
            var item = await _requestService.Get(id);

            if(item == null)
                return NotFound();

            var types = (await _repairTypeService.GetAll())
                .Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.Name });

            ViewBag.Types = types;
            ViewBag.PageNumber = pageNumber;
            ViewBag.Filter = filter;

            return View(item);
        }

        [Authorize(Roles = "tb, ooiot, creator, prb, peo, skie")]
        [HttpPost]
        public async Task<IActionResult> Edit(
            Request item,
            int pageNumber = 1, 
            string filter = "")
        {
            if(!ModelState.IsValid)
            {
                var types = (await _repairTypeService.GetAll())
                        .Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.Name });

                ViewBag.Types = types;
                ViewBag.PageNumber = pageNumber;
                ViewBag.Filter = filter;

                return View(item);
            }

            await _requestService.Edit(item);

            return RedirectToAction("Index", new { pageNumber = pageNumber, filter = filter });
        }

        public async Task<IActionResult> Delete(int id, int pageNumber = 1, string filter = "")
        {
            var item = await _requestService.Get(id);

            if (item == null)
                return NotFound();

            ViewBag.PageNumber = pageNumber;
            ViewBag.Filter = filter;

            return View(item);
        }

        [Authorize(Roles = "tb, ooiot, creator, prb, peo, skie")]
        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id, int pageNumber = 1, string filter = "")
        {
            await _requestService.Remove(id);

            return RedirectToAction("Index", new { filter = filter, pageNumber = pageNumber });
        }

        [Authorize(Roles = "tb, ooiot, creator, prb, peo, skie")]
        public async Task<IActionResult> Confirm(int id, int pageNumber = 1, string filter = "")
        {
            await _requestService.Confirm(id);

            return RedirectToAction("Index", new { pageNumber = pageNumber, filter = filter });
        }
    }
}
