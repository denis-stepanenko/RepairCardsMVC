using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RepairCardsMVC.Models;
using RepairCardsMVC.Services;

namespace RepairCardsMVC.Controllers
{
    public class RepairTypeController : Controller
    {
        private readonly RepairTypeService _repairTypeService;

        public RepairTypeController(RepairTypeService repairTypeService)
        {
            _repairTypeService = repairTypeService;
        }

        public async Task<IActionResult> Index(int pageNumber = 1, string filter = "")
        {
            ViewBag.PageNumber = pageNumber;
            ViewBag.Filter = filter;

            return View(await _repairTypeService.GetAll(pageNumber, filter));
        }

        public IActionResult Create(int pageNumber = 1, string filter = "")
        {
            ViewBag.PageNumber = pageNumber;
            ViewBag.Filter = filter;

            return View();
        }

        [Authorize(Roles = "admin")]
        [HttpPost]
        public async Task<IActionResult> Create(
            RepairType item,
            int pageNumber = 1, 
            string filter = "")
        {
            if(!ModelState.IsValid)
            {
                ViewBag.PageNumber = pageNumber;
                ViewBag.Filter = filter;

                return View(item);
            }

            await _repairTypeService.Add(item);

            return RedirectToAction("Index", new { pageNumber = pageNumber, filter = filter });
        }

        public async Task<IActionResult> Edit(int id, int pageNumber = 1, string filter = "")
        {
            var item = await _repairTypeService.Get(id);

            if (item == null)
                return NotFound();

            ViewBag.PageNumber = pageNumber;
            ViewBag.Filter = filter;

            return View(item);
        }

        [Authorize(Roles = "admin")]
        [HttpPost]
        public async Task<IActionResult> Edit(
            RepairType item,
            int pageNumber = 1, 
            string filter = "")
        {
            if(!ModelState.IsValid)
            {
                ViewBag.PageNumber = pageNumber;
                ViewBag.Filter = filter;

                return View(item);
            }

            await _repairTypeService.Edit(item);

            return RedirectToAction("Index", new { filter = filter, pageNumber = pageNumber });
        }

        public async Task<IActionResult> Delete(int id, int pageNumber = 1, string filter = "")
        {
            var item = await _repairTypeService.Get(id);

            if(item == null)
                return NotFound();

            ViewBag.PageNumber = pageNumber;
            ViewBag.Filter = filter;

            return View(item);
        }

        [Authorize(Roles = "admin")]
        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id, int pageNumber = 1, string filter = "")
        {
            await _repairTypeService.Remove(id);

            return RedirectToAction("Index", new { pageNumber = pageNumber, filter = filter });
        }
    }
}
