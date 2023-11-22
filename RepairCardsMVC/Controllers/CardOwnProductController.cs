using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RepairCardsMVC.Models;
using RepairCardsMVC.Services;
using RepairCardsMVC.ViewModels;
using System.Data;

namespace RepairCardsMVC.Controllers
{
    public class CardOwnProductController : Controller
    {
        private readonly CardOwnProductService _cardOwnProductService;
        private readonly ProductService _productService;
        private readonly CardService _cardService;

        public CardOwnProductController(
            CardOwnProductService cardOwnProductService,
            ProductService productService,
            CardService cardService)
        {
            _cardOwnProductService = cardOwnProductService;
            _productService = productService;
            _cardService = cardService;
        }

        public async Task<IActionResult> Index(
            int id,
            int cardsPageNumber,
            string cardsFilter)
        {
            var items = await _cardOwnProductService.GetAll(id);
            var roots = GetTreeProducts(items.ToList()).OrderByDescending(x => x.Id);

            var result = new List<TreeNodeViewModel>();

            foreach (var root in roots)
            {
                var nodes = _cardOwnProductService.GetAllProductsByParentRecursively(root);

                result.AddRange(nodes.Select(x => new TreeNodeViewModel
                {
                    Id = x.Id,
                    Code = x.Code,
                    Name = x.Name,
                    Count = x.Count,
                    Route = x.Route ?? "",
                    ParentId = x.ParentId,
                    HasChangedComposition = x.HasChangedComposition,
                    IsOvercoatingRequired = x.IsOvercoatingRequired
                }));
            }

            ViewBag.CardId = id;
            ViewBag.CardsPageNumber = cardsPageNumber;
            ViewBag.CardsFilter = cardsFilter;

            return View(result);
        }

        public IActionResult Create(
            int cardId,
            int cardsPageNumber,
            string cardsFilter)
        {
            ViewBag.CardId = cardId;
            ViewBag.CardsPageNumber = cardsPageNumber;
            ViewBag.CardsFilter = cardsFilter;

            return View();
        }

        [Authorize(Roles = "tb, ooiot, creator, prb")]
        [HttpPost]
        public IActionResult Create([FromBody]CreateOwnProductViewModel model)
        {
            var roots = BuildTree(model.Items);

            List<TreeProduct> productsWithIncompleteComposition = _cardOwnProductService.GetProductsWithIncompleteComposition(roots);

            _cardOwnProductService.RemoveNotSelectedParentsRecursively(roots);

            _cardOwnProductService.AddTreeRecursively(model.CardId, roots, productsWithIncompleteComposition);

            return NoContent();
        }

        public IActionResult CreateByCard(
            int cardId,
            int cardsPageNumber,
            string cardsFilter)
        {
            ViewBag.CardId = cardId;
            ViewBag.CardsPageNumber = cardsPageNumber;
            ViewBag.CardsFilter = cardsFilter;

            return View();
        }

        public async Task<IActionResult> Edit(
            int id,
            int cardId,
            int cardsPageNumber,
            string cardsFilter)
        {
            var item = await _cardOwnProductService.Get(id);

            if (item == null)
                return NotFound();

            ViewBag.Item = item;
            ViewBag.CardId = cardId;
            ViewBag.CardsPageNumber = cardsPageNumber;
            ViewBag.CardsFilter = cardsFilter;

            return View(new EditCardOwnProductViewModel { Count = item.Count });
        }

        [Authorize(Roles = "tb, ooiot, creator, prb")]
        [HttpPost]
        public async Task<IActionResult> Edit(
            EditCardOwnProductViewModel model,
            int id,
            int cardId,
            int cardsPageNumber,
            string cardsFilter = "")
        {
            if (!ModelState.IsValid)
            {
                var product = await _cardOwnProductService.Get(id);

                if (product == null)
                    return NotFound();

                ViewBag.Item = product;
                ViewBag.CardId = cardId;
                ViewBag.CardsPageNumber = cardsPageNumber;
                ViewBag.CardsFilter = cardsFilter;

                return View(model);
            }

            await _cardOwnProductService.Edit(id, model.Count);

            return RedirectToAction("Index", new
            {
                id = cardId,
                cardsPageNumber = cardsPageNumber,
                cardsFilter = cardsFilter
            });
        }

        [Authorize(Roles = "tb, ooiot, creator, prb")]
        public IActionResult Delete(
            int id,
            int cardId,
            int cardsPageNumber,
            int cardsFilter)
        {
            _cardOwnProductService.Remove(id);

            return RedirectToAction("Index", new
            {
                id = cardId,
                cardsPageNumber = cardsPageNumber,
                cardsFilter = cardsFilter
            });
        }

        [Authorize(Roles = "tb, ooiot, creator, prb")]
        public async Task<IActionResult> MarkThatOvercoatingRequired(
            int id,
            int cardId,
            int cardsPageNumber,
            string cardsFilter)
        {
            await _cardOwnProductService.MarkThatOvercoatingRequired(id);

            return RedirectToAction("Index", new
            {
                id = cardId,
                cardsPageNumber = cardsPageNumber,
                cardsFilter = cardsFilter
            });
        }

        [HttpGet]
        public IActionResult GetNodes(string code)
        {
            var relations = _productService.GetProductRelations(code);
            var treeBuilder = new TreeBuilder();
            var root = treeBuilder.Get(relations);
            
            if (root == null)
                return NoContent();

            var nodes = _cardOwnProductService.GetAllProductsByParentRecursively(root);

            int i = 0;

            var result = from node in nodes
                         let parentId = nodes.IndexOf(nodes[i].Parent)
                         select new TreeNodeViewModel
                         {
                             Id = i++,
                             Code = node.Code,
                             Name = node.Name,
                             Count = node.CountAll,
                             Route = node.Route ?? "",
                             ParentId = parentId == -1 ? "" : parentId.ToString()
                         };

            return Ok(result);
        }

        public async Task<IActionResult> GetProductsByCard(int id)
        {
            var items = await _cardOwnProductService.GetAll(id);
            var roots = GetTreeProducts(items.ToList());

            var result = new List<TreeNodeViewModel>();

            foreach (var root in roots)
            {
                var nodes = _cardOwnProductService.GetAllProductsByParentRecursively(root);

                result.AddRange(nodes.Select(x => new TreeNodeViewModel
                {
                    Id = x.Id,
                    Code = x.Code,
                    Name = x.Name,
                    Count = x.Count,
                    Route = x.Route ?? "",
                    ParentId = x.ParentId,
                    HasChangedComposition = x.HasChangedComposition,
                    IsOvercoatingRequired = x.IsOvercoatingRequired
                }));
            }

            return Ok(result);
        }

        [HttpPost]
        public IActionResult GetProductList()
            => Ok(_productService.GetAll(Request));

        [HttpPost]
        public IActionResult GetCards()
            => Ok(_cardService.GetCards(Request));

        List<TreeProduct> GetTreeProducts(List<CardOwnProduct> products)
        {
            List<TreeProduct> GetChildren(int? parentId) =>
                products.Where(x => x.ParentId == parentId)
                        .Select(x => new TreeProduct
                        {
                            Id = x.Id,
                            Code = x.Code,
                            Name = x.Name,
                            Count = x.Count,
                            Route = x.Route,
                            ParentId = parentId.ToString(),
                            HasChangedComposition = x.HasChangedComposition,
                            IsOvercoatingRequired = x.IsOvercoatingRequired
                        }).ToList();

            void Build(List<TreeProduct> items)
            {
                items.ForEach(x => x.Children.AddRange(GetChildren(x.Id)));
                items.ForEach(x => Build(x.Children));
            }

            var roots = GetChildren(null);

            Build(roots);

            return roots;
        }

        List<TreeProduct> BuildTree(List<TreeNodeViewModel> nodes)
        {
            List<TreeProduct> GetChildren(string id) =>
                nodes.Where(x => x.ParentId == id.ToString()).Select(x => new TreeProduct
                {
                    Id = x.Id,
                    ParentId = x.ParentId,
                    Code = x.Code,
                    Name = x.Name,
                    Count = x.Count,
                    Route = x.Route,
                    IsChecked = x.IsChecked
                }).ToList();

            void Recursion(List<TreeProduct> products)
            {
                foreach (var product in products)
                {
                    product.Children = GetChildren(product.Id.ToString());
                    Recursion(product.Children);
                }
            }

            var roots = GetChildren("");

            Recursion(roots);

            return roots;
        }
    }
}
