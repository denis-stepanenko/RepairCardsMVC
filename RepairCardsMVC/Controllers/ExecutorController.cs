using Microsoft.AspNetCore.Mvc;
using RepairCardsMVC.Models;
using RepairCardsMVC.Services;

namespace RepairCardsMVC.Controllers
{
    public class ExecutorController : Controller
    {
        private readonly ExecutorService _executorService;

        public ExecutorController(ExecutorService executorService)
        {
            _executorService = executorService;
        }

        public async Task<IActionResult> Index(int pageNumber = 1, string filter = "")
        {
            ViewBag.PageNumber = pageNumber;
            ViewBag.Filter = filter;

            return View(await _executorService.GetAll(pageNumber, filter));
        }

        public IActionResult Create(int pageNumber = 1, string filter = "")
        {
            ViewBag.PageNumber = pageNumber;
            ViewBag.Filter = filter;

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(
            Executor item,
            string filter = "", 
            int pageNumber = 1)
        {
            if(!ModelState.IsValid)
            {
                ViewBag.PageNumber = pageNumber;
                ViewBag.Filter = filter;

                return View(item);  
            }

            await _executorService.Add(item);

            return RedirectToAction("Index", new { filter = filter, pageNumber = pageNumber });
        }

        public async Task<IActionResult> Edit(int id, int pageNumber = 1, string filter = "")
        {
            var item = await _executorService.Get(id);

            if (item == null)
                return NotFound();

            ViewBag.PageNumber = pageNumber;
            ViewBag.Filter = filter;

            return View(item);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(
            Executor item,
            string filter = "", 
            int pageNumber = 1)
        {
            if(!ModelState.IsValid)
            {
                ViewBag.PageNumber = pageNumber;
                ViewBag.Filter = filter;

                return View(item);
            }

            await _executorService.Edit(item);

            return RedirectToAction("Index", new { pageNumber = pageNumber, filter = filter });
        }

        public async Task<IActionResult> Delete(int id, int pageNumber = 1, string filter = "")
        {
            var item = await _executorService.Get(id);

            if(item == null)
                return NotFound();

            ViewBag.PageNumber = pageNumber;
            ViewBag.Filter = filter;

            return View(item);
        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id, int pageNumber = 1, string filter = "")
        {
            await _executorService.Remove(id);

            return RedirectToAction("Index", new { pageNumber = pageNumber, filter = filter });
        }
    }
}
