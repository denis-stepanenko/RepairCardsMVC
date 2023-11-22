using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RepairCardsMVC.Models;
using RepairCardsMVC.Services;

namespace RepairCardsMVC.Controllers
{
    public class CardStatusController : Controller
    {
        private readonly CardStatusService _cardStatusService;

        public CardStatusController(CardStatusService cardStatusService)
        {
            _cardStatusService = cardStatusService;
        }

        public async Task<IActionResult> Index(int pageNumber = 1, string filter = "")
        {
            ViewBag.PageNumber = pageNumber;
            ViewBag.Filter = filter;

            return View(await _cardStatusService.GetAll(pageNumber, filter));
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
            CardStatus item,
            int pageNumber = 1, 
            string filter = "")
        {
            if(!ModelState.IsValid)
            {
                ViewBag.PageNumber = pageNumber;
                ViewBag.Filter = filter;

                return View(item);
            }

            await _cardStatusService.Add(item);

            return RedirectToAction("Index", new { pageNumber = pageNumber, filter = filter });
        }

        public async Task<IActionResult> Edit(int id, int pageNumber = 1, string filter = "")
        {
            ViewBag.PageNumber = pageNumber;
            ViewBag.Filter = filter;

            var item = await _cardStatusService.Get(id);

            return View(item);
        }

        [Authorize(Roles = "admin")]
        [HttpPost]
        public async Task<IActionResult> Edit(
            CardStatus item,
            int pageNumber = 1, 
            string filter = "")
        {
            if(!ModelState.IsValid)
            {
                ViewBag.PageNumber = pageNumber;
                ViewBag.Filter = filter;

                return View(item);
            }

            await _cardStatusService.Edit(item);

            return RedirectToAction("Index", new { pageNumber = pageNumber, filter = filter });
        }

        public async Task<IActionResult> Delete(int id, int pageNumber = 1, string filter = "")
        {
            var item = await _cardStatusService.Get(id);

            if (item == null)
                return NotFound();

            ViewBag.PageNumber = pageNumber;
            ViewBag.Filter = filter;

            return View(item);
        }

        [Authorize(Roles = "admin")]
        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id, int pageNumber = 1, string filter = "")
        {
            await _cardStatusService.Remove(id);

            return RedirectToAction("Index", new { pageNumber = pageNumber, filter = filter });
        }
    }
}
