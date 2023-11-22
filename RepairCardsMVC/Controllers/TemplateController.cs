using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RepairCardsMVC.Models;
using RepairCardsMVC.Services;

namespace RepairCardsMVC.Controllers
{
    public class TemplateController : Controller
    {
        private readonly TemplateService _templateService;

        public TemplateController(TemplateService templateService)
        {
            _templateService = templateService;
        }

        public async Task<IActionResult> Index(int pageNumber = 1, string filter = "")
        {
            ViewBag.PageNumber = pageNumber;
            ViewBag.Filter = filter;

            return View(await _templateService.GetAll(pageNumber, filter));
        }

        public IActionResult Create(int pageNumber = 1, string filter = "")
        {
            ViewBag.PageNumber = pageNumber;
            ViewBag.Filter = filter;
            return View();
        }

        [Authorize(Roles = "tb, ooiot, creator, otk, prb")]
        [HttpPost]
        public async Task<IActionResult> Create(
            Template item,
            int pageNumber = 1, 
            string filter = "")
        {
            if (!ModelState.IsValid)
            {
                ViewBag.PageNumber = pageNumber;
                ViewBag.Filter = filter;

                return View(item);
            }

            await _templateService.Add(item);

            return RedirectToAction("Index", new { pageNumber = pageNumber, filter = filter });
        }

        public async Task<IActionResult> Edit(int id, int pageNumber = 1, string filter = "")
        {
            var item = await _templateService.Get(id);

            if (item == null)
                return NotFound();

            ViewBag.PageNumber = pageNumber;
            ViewBag.Filter = filter;

            return View(item);
        }

        [Authorize(Roles = "tb, ooiot, creator, otk, prb")]
        [HttpPost]
        public async Task<IActionResult> Edit(
            Template item, 
            int pageNumber = 1, 
            string filter = "")
        {
            if (!ModelState.IsValid)
            {
                ViewBag.PageNumber = pageNumber;
                ViewBag.Filter = filter;

                return View(item);
            }

            await _templateService.Edit(item);

            return RedirectToAction("Index", new { pageNumber = pageNumber, filter = filter });
        }

        public async Task<IActionResult> Delete(int id, int pageNumber = 1, string filter = "")
        {
            var item = await _templateService.Get(id);

            if (item == null)
                return NotFound();

            ViewBag.PageNumber = pageNumber;
            ViewBag.Filter = filter;

            return View(item);
        }

        [Authorize(Roles = "tb, ooiot, creator, otk, prb")]
        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id, int pageNumber = 1, string filter = "")
        {
            await _templateService.Remove(id);

            return RedirectToAction("Index", new { pageNumber = pageNumber, filter = filter });
        }
    }
}
