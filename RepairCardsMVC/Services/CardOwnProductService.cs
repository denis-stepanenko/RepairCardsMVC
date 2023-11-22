using Microsoft.EntityFrameworkCore;
using RepairCardsMVC.Exceptions;
using RepairCardsMVC.Models;

namespace RepairCardsMVC.Services
{
    public class CardOwnProductService
    {
        private readonly ApplicationDbContext _db;

        public CardOwnProductService(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<IEnumerable<CardOwnProduct>> GetAll(int cardId)
            => await _db.CardOwnProducts.Where(x => x.CardId == cardId).ToListAsync();

        public async Task<CardOwnProduct?> Get(int id)
            => await _db.CardOwnProducts
                .Include(x => x.Operations.OrderByDescending(x => x.Id))
                    .ThenInclude(x => x.Executor)
                .Include(x => x.RepairOperations.OrderByDescending(x => x.Id))
                    .ThenInclude(x => x.Executor)
                .FirstOrDefaultAsync(x => x.Id == id);

        public async Task Edit(int id, int count)
        {
            var item = await _db.CardOwnProducts.FindAsync(id);

            if (item == null)
                throw new NotFoundException();

            item.Count = count;

            _db.CardOwnProducts.Update(item);
            await _db.SaveChangesAsync();
        }

        public void Remove(int id)
            => _db.DeleteCardOwnProductRecursively(id);

        public async Task MarkThatOvercoatingRequired(int id)
        {
            var item = await _db.CardOwnProducts.FindAsync(id);

            if (item == null)
                throw new NotFoundException();

            item.IsOvercoatingRequired = !item.IsOvercoatingRequired;

            _db.CardOwnProducts.Update(item);

            await _db.SaveChangesAsync();
        }

        public List<TreeProduct> GetTrees(List<CardOwnProduct> products)
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

        public List<TreeProduct> GetAllProductsByParentRecursively(TreeProduct product)
        {
            var result = new List<TreeProduct>();

            void Recursion(TreeProduct product)
            {
                result.Add(product);
                var children = product.Children;

                foreach (var child in children)
                {
                    Recursion(child);
                }
            }

            Recursion(product);

            return result;
        }

        public List<TreeProduct> GetProductsWithIncompleteComposition(List<TreeProduct> products)
        {
            var result = new List<TreeProduct>();

            void FindParents(TreeProduct item)
            {
                if (item == null) return;

                result.Add(item);

                FindParents(item.Parent);
            }

            void FindUnchekedProducts(List<TreeProduct> items)
            {
                foreach (var item in items)
                {
                    if (!item.IsChecked)
                        FindParents(item);

                    FindUnchekedProducts(item.Children);
                }
            }

            FindUnchekedProducts(products);

            return result;
        }

        public void RemoveNotSelectedParentsRecursively(List<TreeProduct> products)
        {
            for (int i = 0; i < products.Count; i++)
            {
                if (!products[i].IsChecked)
                {
                    products.InsertRange(i + 1, products[i].Children);
                    products.RemoveAt(i);
                    i--;
                }
                else
                    RemoveNotSelectedParentsRecursively(products[i].Children);
            }
        }

        public void AddTreeRecursively(int cardId, List<TreeProduct> products, List<TreeProduct> productsWithIncompleteComposition)
        {
            void Recursion(List<TreeProduct> products, int? parentId)
            {
                foreach (var product in products)
                {
                    var newCardOwnProduct = new CardOwnProduct
                    {
                        CardId = cardId,
                        ParentId = parentId,
                        Code = product.Code,
                        Name = product.Name,
                        Count = (int)product.Count,
                        Route = product.Route,
                        HasChangedComposition = productsWithIncompleteComposition.Contains(product)
                    };

                    _db.CardOwnProducts.Add(newCardOwnProduct);
                    _db.SaveChanges();

                    int id = newCardOwnProduct.Id;

                    var operations = _db.GetProductOperations(product.Code, product.Route);

                    var newOpeartions = operations.Select(x => new CardOwnProductOperation
                    {
                        CardOwnProductId = id,
                        Code = x.Code,
                        Name = x.Name,
                        Labor = x.Labor,
                        Type = 0,
                        Department = x.Department
                    }).ToList();

                    newOpeartions.ForEach(x => _db.CardOwnProductOperations.Add(x));
                    _db.SaveChanges();

                    Recursion(product.Children, id);
                }
            }
            Recursion(products, null);
        }
    }

}

