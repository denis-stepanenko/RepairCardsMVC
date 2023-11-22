using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RepairCardsMVC.Services;

namespace RepairCardsMVC.Controllers
{
    public class RequestToCreateStagesIn1SController : Controller
    {
        private readonly RequestToCreateStagesIn1SService _requestToCreateStagesIn1SService;
        private readonly CardService _cardService;

        public RequestToCreateStagesIn1SController(RequestToCreateStagesIn1SService requestToCreateStagesIn1SService, CardService cardService)
        {
            _requestToCreateStagesIn1SService = requestToCreateStagesIn1SService;
            _cardService = cardService;
        }

        public async Task<IActionResult> Index(int pageNumber = 1, string filter = "")
        {
            ViewBag.PageNumber = pageNumber;
            ViewBag.Filter = filter;

            return View(await _requestToCreateStagesIn1SService.GetAll(pageNumber, filter));
        }

        [Authorize(Roles = "tb, ooiot, creator, otk, prb")]
        [HttpPost]
        public async Task<IActionResult> Index(int id, int pageNumber = 1, string filter = "")
        {
            await _requestToCreateStagesIn1SService.Add(id);

            return RedirectToAction("Index", new { pageNumber = pageNumber, filter = filter });
        }

        [Authorize(Roles = "tb, ooiot, creator, otk, prb")]
        public async Task<IActionResult> Delete(int id, int pageNumber = 1, string filter = "")
        {
            await _requestToCreateStagesIn1SService.Remove(id);

            return RedirectToAction("Index", new { pageNumber = pageNumber, filter = filter });
        }

        [Authorize(Roles = "tb, ooiot, creator, otk, prb")]
        public async Task<IActionResult> CloseApplication(int id, int pageNumber = 1, string filter = "")
        {
            await _requestToCreateStagesIn1SService.CloseApplication(id);

            return RedirectToAction("Index", new { pageNumber = pageNumber, filter = filter });
        }

        [Authorize(Roles = "tb, ooiot, creator, otk, prb")]
        public async Task<IActionResult> CancelApplicationClosing(int id, int pageNumber = 1, string filter = "")
        {
            await _requestToCreateStagesIn1SService.CancelApplicationClosing(id);

            return RedirectToAction("Index", new { pageNumber = pageNumber, filter = filter});
        }

        public async Task<IActionResult> SearchCards(string query, int count)
            => Ok(await _cardService.FindCards(query, count));
    }
}
