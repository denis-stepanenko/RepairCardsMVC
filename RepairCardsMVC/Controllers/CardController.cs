using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using RepairCardsMVC.Exceptions;
using RepairCardsMVC.Models;
using RepairCardsMVC.Services;
using System.Data;

namespace RepairCardsMVC.Controllers
{
    public class CardController : Controller
    {
        private readonly CardService _cardService;
        private readonly RepairTypeService _repairTypeService;
        private readonly CardStatusService _cardStatusService;
        private readonly UserManager<User> _userManager;
        private readonly ProductService _productService;
        private readonly OrderService _orderService;

        public CardController( 
            CardService cardService,
            RepairTypeService repairTypeService,
            CardStatusService cardStatusService,
            UserManager<User> userManager,
            ProductService productService,
            OrderService orderService)
        {
            _cardService = cardService;
            _repairTypeService = repairTypeService;
            _cardStatusService = cardStatusService;
            _userManager = userManager;
            _productService = productService;
            _orderService = orderService;

        }

        public async Task<IActionResult> Index(int pageNumber = 1, string filter = "")
        {
            ViewBag.PageNumber = pageNumber;
            ViewBag.Filter = filter;

            return View(await _cardService.GetAll(pageNumber, filter));
        }

        private async Task CreateBags(int pageNumber, string filter)
        {
            var types = (await _repairTypeService.GetAll())
                .Select(x => new SelectListItem { Text = x.Name, Value = x.Id.ToString() });

            var statuses = (await _cardStatusService.GetAll())
                .Select(x => new SelectListItem { Text = x.Name, Value = x.Id.ToString() });

            ViewBag.Types = types;
            ViewBag.Statuses = statuses;
            ViewBag.PageNumber = pageNumber;
            ViewBag.Filter = filter;
        }

        [Authorize(Roles = "tb, ooiot, creator, prb")]
        public async Task<IActionResult> Create(int pageNumber = 1, string filter = "")
        {
            await CreateBags(pageNumber, filter);

            var user = await _userManager.GetUserAsync(User);

            if (user == null)
                return NotFound();

            string newNumber = await _cardService.GenerateNewNumber(user.Department);

            return View(new Card { Number = newNumber });
        }

        [Authorize(Roles = "tb, ooiot, creator, prb")]
        [HttpPost]
        public async Task<IActionResult> Create(Card item, int pageNumber = 1, string filter = "")
        {
            if (!ModelState.IsValid)
            {
                await CreateBags(pageNumber, filter);

                return View(item);
            }

            try
            {
                await _cardService.Create(item);
            }
            catch (BusinessLogicException ex)
            {
                TempData["message"] = ex.Message;
                
                await CreateBags(pageNumber, filter);

                return View(item);
            }

            return RedirectToAction("Index", new { filter = item, pageNumber = item });
        }

        private async Task EditBags(int pageNumber, string filter)
        {
            var types = (await _repairTypeService.GetAll())
                .Select(x => new SelectListItem { Text = x.Name, Value = x.Id.ToString() });

            var statuses = (await _cardStatusService.GetAll())
                .Select(x => new SelectListItem { Text = x.Name, Value = x.Id.ToString() });

            ViewBag.Types = types;
            ViewBag.Statuses = statuses;
            ViewBag.PageNumber = pageNumber;
            ViewBag.Filter = filter;
        }

        public async Task<IActionResult> Edit(int id, int pageNumber = 1, string filter = "")
        {
            var item = await _cardService.Get(id);

            if(item == null)
                return NotFound();

            await EditBags(pageNumber, filter);

            return View(item);
        }

        [Authorize(Roles = "tb, ooiot, creator, otk, prb")]
        [HttpPost]
        public async Task<IActionResult> Edit(Card item, int pageNumber = 1, string filter = "")
        {
            if (!ModelState.IsValid)
            {
                await EditBags(pageNumber, filter);

                return View(item);
            }

            try
            {
                await _cardService.Edit(item);
            }
            catch (BusinessLogicException ex)
            {
                TempData["message"] = ex.Message;

                await EditBags(pageNumber, filter);

                return View(item);
            }

            return RedirectToAction("Index", new { filter = filter, pageNumber = pageNumber });
        }

        public async Task<IActionResult> Delete(int id, int pageNumber = 1, string filter = "")
        {
            var item = await _cardService.Get(id);

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
            await _cardService.Delete(id);

            return RedirectToAction("Index", new { filter = filter, pageNumber = pageNumber });
        }

        [Authorize(Roles = "tb, ooiot, creator, prb")]
        public async Task<IActionResult> Duplicate(int id, int pageNumber = 1, string filter = "")
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
                return NotFound();

            await _cardService.Duplicate(id, user.Department);

            return RedirectToAction("Index", new { filter = filter, pageNumber = pageNumber });
        }

        [Authorize(Roles = "ooiot")]
        [HttpPost]
        public async Task<IActionResult> ExportToNormaVremia(int id, int pageNumber, string filter, int department)
        {
            try
            {
                await _cardService.ExportToNormaVremia(id, department);
            }
            catch (BusinessLogicException ex)
            {
                TempData["message"] = ex.Message;
            }

            return RedirectToAction("Edit", new { id = id, filter = filter, pageNumber = pageNumber });
        }

        [Authorize(Roles = "skb")]
        public async Task<IActionResult> ExportToPDM(int cardId, int cardsPageNumber, int cardsFilter)
        {
            try
            {
                await _cardService.ExportToPDM(cardId);
            }
            catch (BusinessLogicException ex)
            {
                TempData["message"] = ex.Message;
            }

            return RedirectToAction("Edit", new
            {
                id = cardId,
                cardsPageNumber = cardsPageNumber,
                cardsFilter = cardsFilter
            });
        }

        public async Task<IActionResult> SearchProducts(string query, int count)
        {
            var items = await _productService.GetAll(query, count);

            return Ok(items.Select(x => new { Code = x.Code, Name = x.Name }));
        }

        public async Task<IActionResult> SearchOrders(string productCode)
        {
            var orders = await _orderService.GetAll(productCode);

            var items = orders.Where(x => x.Direction != null)
                              .Select(x => new { order = x.Number, cipher = x.Cipher ?? "", direction = x.Direction });

            return Ok(items);
        }

        public async Task<IActionResult> FindCards(string query, int count)
            => Ok(await _cardService.FindCards(query, count));

        public async Task<IActionResult> GenerateNewNumber()
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
                return NotFound();

            string newNumber = await _cardService.GenerateNewNumber(user.Department);

            return Ok(newNumber);
        }

        [AcceptVerbs("GET", "POST")]
        public async Task<IActionResult> VerifyCardNumber(string number, int id)
            => Json(!await _cardService.IsThereCardWithNumber(number, id));

        [AcceptVerbs("GET", "POST")]
        public async Task<IActionResult> VerifyActNumber(string number, int id)
            => Json(!await _cardService.IsThereCardWithActNumber(number, id));
    }
}
