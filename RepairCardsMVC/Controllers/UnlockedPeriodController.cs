using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RepairCardsMVC.Models;
using RepairCardsMVC.Services;

namespace RepairCardsMVC.Controllers
{
    public class UnlockedPeriodController : Controller
    {
        private readonly UnlockedPeriodService _unlockedPeriodService;
        private readonly CardService _cardService;

        public UnlockedPeriodController(UnlockedPeriodService unlockedPeriodService, CardService cardService)
        {
            _unlockedPeriodService = unlockedPeriodService;
            _cardService = cardService;

        }

        public async Task<IActionResult> Index(int pageNumber = 1, string filter = "")
        {
            ViewBag.PageNumber = pageNumber;
            ViewBag.Filter = filter;

            return View(await _unlockedPeriodService.GetAll(pageNumber, filter));
        }

        public async Task<IActionResult> Edit(int id, int pageNumber = 1, string filter = "")
        {
            var item = await _unlockedPeriodService.Get(id);

            if (item == null)
                return NotFound();

            var card = await _cardService.Get((int)item.CardId);

            if(card == null) 
                return NotFound();

            ViewBag.Card = card;
            ViewBag.PageNumber = pageNumber;
            ViewBag.Filter = filter;

            return View(item);
        }

        [Authorize(Roles = "ooiot")]
        [HttpPost]
        public async Task<IActionResult> Edit(
            UnlockedPeriod item,
            int pageNumber = 1, 
            string filter = "")
        {
            if (!ModelState.IsValid)
            {
                var card = await _cardService.Get((int)item.CardId);

                ViewBag.Card = card;
                ViewBag.PageNumber = pageNumber;
                ViewBag.Filter = filter;

                return View(item);
            }

            await _unlockedPeriodService.Edit(item);

            return RedirectToAction("Index", new { filter = filter, pageNumber = pageNumber });
        }

        public IActionResult Create(int pageNumber = 1, string filter = "")
        {
            ViewBag.PageNumber = pageNumber;
            ViewBag.Filter = filter;

            return View();
        }

        [Authorize(Roles = "ooiot")]
        [HttpPost]
        public async Task<IActionResult> Create(
            UnlockedPeriod item,
            int pageNumber = 1,
            string filter = "")
        {
            if (!ModelState.IsValid)
            {
                ViewBag.PageNumber = pageNumber;
                ViewBag.Filter = filter;

                return View(item);
            }

            await _unlockedPeriodService.Add(item);

            return RedirectToAction("Index", new { filter = filter, pageNumber = pageNumber });
        }

        public async Task<IActionResult> Delete(int id, int pageNumber = 1, string filter = "")
        {
            var item = await _unlockedPeriodService.Get(id);

            if (item == null)
                return NotFound();

            ViewBag.PageNumber = pageNumber;
            ViewBag.Filter = filter;

            return View(item);
        }

        [Authorize(Roles = "ooiot")]
        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id, int pageNumber = 1, string filter = "")
        {
            await _unlockedPeriodService.Remove(id);

            return RedirectToAction("Index", new { filter = filter, pageNumber = pageNumber });
        }
    }
}
