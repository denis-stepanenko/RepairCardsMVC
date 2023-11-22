using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RepairCardsMVC.Services;

namespace RepairCardsMVC.Controllers
{
    public class ExportRequestController : Controller
    {
        private readonly ExportRequestService _exportRequestService;
        private readonly CardService _cardService;

        public ExportRequestController(ExportRequestService exportRequestService, CardService cardService)
        {
            _exportRequestService = exportRequestService;
            _cardService = cardService;

        }

        public async Task<IActionResult> Index(int pageNumber = 1, string filter = "")
        {
            ViewBag.PageNumber = pageNumber;
            ViewBag.Filter = filter;

            return View(await _exportRequestService.GetAll(pageNumber, filter));
        }

        [Authorize(Roles = "tb, ooiot, creator, prb, peo, skie")]
        [HttpPost]
        public async Task<IActionResult> Index(int id, int pageNumber = 1, string filter = "")
        {
            await _exportRequestService.Add(id);

            return RedirectToAction("Index", new { pageNumber = pageNumber, filter = filter });
        }

        [Authorize(Roles = "tb, ooiot, creator, prb, peo, skie")]
        public async Task<IActionResult> Delete(int id, int pageNumber = 1, string filter = "")
        {
            await _exportRequestService.Remove(id);

            return RedirectToAction("Index", new { filter = filter, pageNumber = pageNumber });
        }

        [Authorize(Roles = "skb")]
        public async Task<IActionResult> CloseApplication(int id, int pageNumber = 1, string filter = "")
        {
            await _exportRequestService.CloseApplication(id);

            return RedirectToAction("Index", new { pageNumber = pageNumber, filter = filter });
        }

        [Authorize(Roles = "skb")]
        public async Task<IActionResult> CancelApplicationClosing(int id, int pageNumber = 1, string filter = "")
        {
            await _exportRequestService.CancelApplicationClosing(id);

            return RedirectToAction("Index", new { pageNumber = pageNumber, filter = filter });
        }

        public async Task<IActionResult> SetDeficitCreationDate(int id, int pageNumber = 1, string filter = "")
        {
            await _exportRequestService.SetDeficitCreationDate(id);

            return RedirectToAction("Index", new { pageNumber = pageNumber, filter = filter });
        }

        public async Task<IActionResult> SearchCards(string query, int count)
            => Ok(await _cardService.FindCards(query, count));
    }
}
