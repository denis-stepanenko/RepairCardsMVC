using Microsoft.EntityFrameworkCore;
using RepairCardsMVC.Exceptions;
using RepairCardsMVC.Models;
using RepairCardsMVC.ViewModels;
using System.Text.RegularExpressions;

namespace RepairCardsMVC.Services
{
    public class CardService
    {
        private readonly ApplicationDbContext _db;

        public CardService(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<Card?> Get(int id)
            => await _db.Cards
                .Include(x => x.Parent)
                .Include(x => x.Parent2)
                .Include(x => x.Operations)
                .FirstOrDefaultAsync(x => x.Id == id);

        public async Task<PaginatedList<Card>> GetAll(int pageNumber = 1, string filter = "")
        {
            var items = _db.Cards.Where(x =>
                (x.Number.Contains(filter) ||
                x.ProductCode.Contains(filter) ||
                (x.ProductName ?? "").Contains(filter) ||
                (x.Direction ?? "").Contains(filter) ||
                (x.Cipher ?? "").Contains(filter) ||
                (x.ReasonForRepair ?? "").Contains(filter)))// && x.Id == 7826)
                .Include(x => x.RepairType)
                .Include(x => x.CardStatus)
                .OrderByDescending(x => x.Id);

            return await PaginatedList<Card>.CreateAsync(items, pageNumber, 10);
        }

        public async Task Create(Card item)
        {
            if (await _db.Cards.AnyAsync(x => x.Number == item.Number && x.Id != item.Id))
                throw new BusinessLogicException("Карта с таким номером уже существует");

            if (!string.IsNullOrWhiteSpace(item.ActNumber))
                if (await _db.Cards.AnyAsync(x => x.ActNumber == item.ActNumber && x.Id != item.Id))
                    throw new BusinessLogicException("Карта с таким номером акта уже существует");

            var product = _db.Products.FirstOrDefault(x => x.Code == item.ProductCode);

            if (product == null)
                throw new NotFoundException();

            item.ProductName = product.Name;

            var order = _db.Orders.FirstOrDefault(x => x.Number == item.Order);
            if (order != null)
            {
                item.Direction = order.Direction;
                item.Cipher = order.Cipher;
                item.ClientOrder = order.ClientOrder;
            }

            _db.Cards.Add(item);
            _db.SaveChanges();

            _db.CardDetails.Add(new CardDetails
            {
                Id = item.Id
            });

            await _db.SaveChangesAsync();
        }

        public async Task Edit(Card item)
        {
            if (await _db.Cards.AnyAsync(x => x.Number == item.Number && x.Id != item.Id))
                throw new BusinessLogicException("Карта с таким номером уже существует");

            if (!string.IsNullOrWhiteSpace(item.ActNumber))
                if (await _db.Cards.AnyAsync(x => x.ActNumber == item.ActNumber && x.Id != item.Id))
                    throw new BusinessLogicException("Карта с таким номером акта уже существует");

            var product = _db.Products.FirstOrDefault(x => x.Code == item.ProductCode);

            if (product == null)
                throw new NotFoundException();

            var order = _db.Orders.FirstOrDefault(x => x.Number == item.Order);
            if (order != null)
            {
                var oldCard = _db.Cards.AsNoTracking().FirstOrDefault(x => x.Id == item.Id);

                if (oldCard == null)
                    throw new NotFoundException();

                if (item.Order != oldCard.Order)
                {
                    item.Direction = order.Direction;
                    item.Cipher = order.Cipher;
                    item.ClientOrder = order.ClientOrder;
                }
            }

            item.ProductName = product.Name;

            _db.Cards.Update(item);
            await _db.SaveChangesAsync();
        }

        public async Task Delete(int id)
        {
            var item = await _db.Cards.FindAsync(id);

            if (item == null)
                throw new NotFoundException();

            _db.Cards.Remove(item);
            await _db.SaveChangesAsync();
        }

        public async Task Duplicate(int id, int department)
        {
            int maxNumber = await _db.Cards
                .Where(x => x.Number.Contains("/" + department) && x.Number.EndsWith(DateTime.Now.Year.ToString().Substring(2)))
                .MaxAsync(x => (int?)Convert.ToInt32(x.Number.Substring(0, x.Number.IndexOf("/")))) ?? 0;

            string number = $"{maxNumber + 1}/{department}.{DateTime.Now.ToString("yy")}";

            using var transaction = _db.Database.BeginTransaction();

            var card = await _db.Cards.FindAsync(id);
            var details = await _db.CardDetails.FindAsync(id);

            if (card == null || details == null)
                throw new NotFoundException();

            try
            {
                // Card
                _db.Entry(card).State = EntityState.Detached;

                card.Number = number;
                card.Id = 0;
                card.ParentId = null;
                card.ParentId2 = null;
                card.IsDepartment4Confirmed = false;
                card.IsDepartment5Confirmed = false;
                card.IsDepartment6Confirmed = false;
                card.IsDepartment13Confirmed = false;
                card.IsDepartment17Confirmed = false;
                card.IsDepartment80Confirmed = false;
                card.IsDepartment82Confirmed = false;

                _db.Cards.Add(card);
                await _db.SaveChangesAsync();

                // Card Details
                _db.Entry(details).State = EntityState.Detached;

                details.Id = card.Id;
                _db.CardDetails.Add(details);

                await _db.SaveChangesAsync();

                // Operations
                var operations = await _db.CardOperations
                    .Where(x => x.CardId == id)
                    .AsNoTracking()
                    .ToListAsync();

                operations.ForEach(x =>
                {
                    x.Id = 0;
                    x.CardId = card.Id;
                });

                _db.CardOperations.AddRange(operations);

                // Materials
                var materials = await _db.CardMaterials
                    .Where(x => x.CardId == id)
                    .AsNoTracking()
                    .ToListAsync();

                materials.ForEach(x =>
                {
                    x.Id = 0;
                    x.CardId = card.Id;
                });

                _db.CardMaterials.AddRange(materials);

                // Documents
                var documents = await _db.CardDocuments
                    .Where(x => x.CardId == id)
                    .AsNoTracking()
                    .ToListAsync();

                documents.ForEach(x =>
                {
                    x.Id = 0;
                    x.CardId = card.Id;
                });

                _db.CardDocuments.AddRange(documents);

                // Purchased Products
                var purchasedProducts = await _db.CardPurchasedProducts
                    .Where(x => x.CardId == id)
                    .AsNoTracking()
                    .ToListAsync();

                foreach (var purchasedProduct in purchasedProducts)
                {
                    var purchasedProductOperations = await _db.CardPurchasedProductOperations
                        .Where(x => x.CardPurchasedProductId == purchasedProduct.Id)
                        .AsNoTracking()
                        .ToListAsync();

                    purchasedProduct.Id = 0;
                    purchasedProduct.CardId = card.Id;

                    _db.CardPurchasedProducts.Add(purchasedProduct);
                    await _db.SaveChangesAsync();

                    purchasedProductOperations.ForEach(x =>
                    {
                        x.Id = 0;
                        x.CardPurchasedProductId = purchasedProduct.Id;
                    });

                    _db.CardPurchasedProductOperations.AddRange(purchasedProductOperations);
                    await _db.SaveChangesAsync();
                }

                // Own Products
                var roots = await _db.CardOwnProducts
                    .Where(x => x.CardId == id && x.ParentId == null)
                    .AsNoTracking()
                    .ToListAsync();

                void AddRecursively(CardOwnProduct parent)
                {
                    var children = _db.CardOwnProducts
                        .Where(x => x.CardId == id && x.ParentId == parent.Id)
                        .AsNoTracking()
                        .ToList();

                    var productionOperations = _db.CardOwnProductOperations
                        .Where(x => x.CardOwnProductId == parent.Id)
                        .AsNoTracking()
                        .ToList();

                    var repairOperations = _db.CardOwnProductRepairOperations
                        .Where(x => x.CardOwnProductId == parent.Id)
                        .AsNoTracking()
                        .ToList();

                    parent.Id = 0;
                    parent.CardId = card.Id;

                    _db.CardOwnProducts.Add(parent);

                    _db.SaveChanges();

                    children.ForEach(x => x.ParentId = parent.Id);

                    productionOperations.ForEach(x =>
                    {
                        x.Id = 0;
                        x.CardOwnProductId = parent.Id;
                    });

                    _db.CardOwnProductOperations.AddRange(productionOperations);
                    _db.SaveChanges();

                    repairOperations.ForEach(x =>
                    {
                        x.Id = 0;
                        x.CardOwnProductId = parent.Id;
                    });

                    _db.CardOwnProductRepairOperations.AddRange(repairOperations);
                    _db.SaveChanges();

                    children.ForEach(x => AddRecursively(x));
                }

                roots.ForEach(x => AddRecursively(x));

                transaction.Commit();
            }
            catch (Exception)
            {
                transaction.Rollback();
                throw;
            }
        }

        public async Task ExportToNormaVremia(int id, int department)
        {
            var card = await _db.Cards.FindAsync(id);

            if (card == null)
                throw new NotFoundException();

            if (string.IsNullOrWhiteSpace(card.ProductCode))
                throw new BusinessLogicException("Продукт не указан в карте");

            if (!Regex.IsMatch(card.ProductCode, @"^.+\.\d+/\d{2}\.\d{2}$"))
                throw new BusinessLogicException("Децимальный номер, указанный в карте не является ремонтным");

            if (department == 0)
                throw new BusinessLogicException("Подразделение не указано");

            //_db.ExportToNormaVremia(id, card.ProductCode, department);
        }

        public async Task ExportToPDM(int id)
        {
            var card = await _db.Cards.FindAsync(id);

            if (card == null)
                throw new NotFoundException();

            if (!card.ProductCode.Contains("/"))
                throw new BusinessLogicException("Децимальный номер, который указан в карте не является ремонтным");

            if (!Regex.IsMatch(card.ProductCode, @"^.+\.\d+/\d{2}\.\d{2}$"))
                throw new BusinessLogicException("Децимальный номер, который указан в карте не является ремонтным");

            if (_db.CardOwnProducts.Where(x => x.CardId == id).Any(x => x.Code == card.ProductCode))
                throw new BusinessLogicException("Децимальный номер, который указан в карте также находится в списке ДСЕ на восполнение");

            if (card.Order == null)
                throw new BusinessLogicException("Заказ не указан в карте");

            if (_db.IsPlannedOrder(card.Order))
                throw new BusinessLogicException("В этом году уже есть дефицит на этот заказ. Свяжитесь с ПДО по поводу удаления дефицита");

            if (_db.IsExistingOrderInPDM(card.Order))
                throw new BusinessLogicException("Уже есть ВПР на этот заказ");

            //_db.ExportToPDM(card);
        }

        public async Task<string> GenerateNewNumber(int userDepartment)
        {
            int maxNumber = await _db.Cards
                .Where(x => x.Number.Contains("/" + userDepartment) && x.Number.EndsWith(DateTime.Now.Year.ToString().Substring(2)))
                .MaxAsync(x => (int?)Convert.ToInt32(x.Number.Substring(0, x.Number.IndexOf("/")))) ?? 0;

            string newNumber = $"{maxNumber + 1}/{userDepartment}.{DateTime.Now.ToString("yy")}";

            return newNumber;
        }

        public async Task<IEnumerable<Card>> FindCards(string query, int count)
        {
            //var items = await _db.Cards.Where(x => x.Number.Contains(query)).ToListAsync();

            var items =
                await _db.Cards.Where(x =>
                x.Number.Contains(query) ||
                x.ProductCode.Contains(query) ||
                (x.ProductName ?? "").Contains(query)).Take(count)
                .ToListAsync();

            return items;
        }

        public async Task<bool> IsThereCardWithNumber(string number, int id)
            => await _db.Cards.AnyAsync(x => x.Number == number && x.Id != id);

        public async Task<bool> IsThereCardWithActNumber(string number, int id)
            => await _db.Cards.AnyAsync(x => x.ActNumber == number && x.Id != id);

        public async Task<IEnumerable<Card>> GetExtractedChildCards(int cardId)
            => await _db.Cards.Where(x => x.ParentId == cardId).ToListAsync();

        public async Task<IEnumerable<Card>> GetInstalledCards(int cardId)
            => await _db.Cards.Where(x => x.ParentId2 == cardId).ToListAsync();

        public IEnumerable<Card> GetExtractedChildCardsRecursively(Card parent)
        {
            var result = new List<Card>();

            void Build(List<Card> cards)
            {
                foreach (var card in cards)
                {
                    result.Add(card);
                    var children = _db.Cards.Where(x => x.ParentId == card.Id).ToList();
                    Build(children);
                }
            }

            Build(new List<Card> { parent });

            result.RemoveAll(x => x.Id == parent.Id);

            result.ForEach(x => x.ParentId = x.ParentId == parent.Id ? null : x.ParentId);

            return result;
        }

        public IEnumerable<Card> GetInstalledChildCardsRecursively(Card parent)
        {
            var result = new List<Card>();

            void Build(List<Card> cards)
            {
                foreach (var card in cards)
                {
                    result.Add(card);
                    var children = _db.Cards.Where(x => x.ParentId2 == card.Id).ToList();
                    Build(children);
                }
            }

            Build(new List<Card> { parent });

            result.RemoveAll(x => x.Id == parent.Id);

            result.ForEach(x => x.ParentId2 = x.ParentId2 == parent.Id ? null : x.ParentId2);

            return result;
        }

        public async Task AddExtractedCard(int id, int parentId)
        {
            var item = await _db.Cards.FindAsync(id);

            if (item == null)
                throw new NotFoundException();

            var parent = await _db.Cards.FindAsync(parentId);

            if(parent == null) 
                throw new NotFoundException();

            if (parent.IsDepartment4Confirmed && 
                parent.IsDepartment5Confirmed && 
                parent.IsDepartment6Confirmed && 
                parent.IsDepartment13Confirmed &&
                parent.IsDepartment17Confirmed && 
                parent.IsDepartment80Confirmed && 
                parent.IsDepartment82Confirmed)
            {
                throw new BusinessLogicException("Карта утверждена ООИОТ, поэтому вы не можете добавить входящую карту");
            }

            if (item.ParentId == parentId)
            {
                throw new BusinessLogicException($"Карта {item.Number} уже входит в {parent?.Number} в качестве демонтированной");
            }

            if(id == parentId)
            {
                throw new BusinessLogicException("Карта не может входить сама в себя");
            }

            if(_db.CardConfirmations.Any(x => x.CardId == parentId && x.CardConfirmationObjectId == 1))
            {
                throw new BusinessLogicException("Родительская карта утверждена");
            }

            item.ParentId = parentId;
            _db.Cards.Update(item);
            await _db.SaveChangesAsync();
        }

        public async Task AddInstalledCard(int id, int parentId)
        {
            var item = await _db.Cards.FindAsync(id);

            if (item == null)
                throw new NotFoundException();

            var parent = await _db.Cards.FindAsync(parentId);

            if (parent == null)
                throw new NotFoundException();

            if (parent.IsDepartment4Confirmed &&
                parent.IsDepartment5Confirmed &&
                parent.IsDepartment6Confirmed &&
                parent.IsDepartment13Confirmed &&
                parent.IsDepartment17Confirmed &&
                parent.IsDepartment80Confirmed &&
                parent.IsDepartment82Confirmed)
            {
                throw new BusinessLogicException("Карта утверждена ООИОТ, поэтому вы не можете добавить входящую карту");
            }

            if (item.ParentId2 == parentId)
            {
                throw new BusinessLogicException($"Карта {item.Number} уже входит в {parent?.Number} в качестве установленной");
            }

            if (id == parentId)
            {
                throw new BusinessLogicException("Карта не может входить сама в себя");
            }

            if (_db.CardConfirmations.Any(x => x.CardId == parentId && x.CardConfirmationObjectId == 1))
            {
                throw new BusinessLogicException("Родительская карта утверждена");
            }

            item.ParentId2 = parentId;
            _db.Cards.Update(item);
            _db.SaveChanges();
        }

        public async Task DeleteExtractedCard(int id)
        {
            var item = await _db.Cards.FindAsync(id);

            if (item == null)
                throw new NotFoundException();

            var parent = await _db.Cards.FindAsync(item.ParentId);

            if(parent == null)
                throw new NotFoundException();

            if (parent.IsDepartment4Confirmed &&
                parent.IsDepartment5Confirmed &&
                parent.IsDepartment6Confirmed &&
                parent.IsDepartment13Confirmed &&
                parent.IsDepartment17Confirmed &&
                parent.IsDepartment80Confirmed &&
                parent.IsDepartment82Confirmed)
            {
                throw new BusinessLogicException("Карта утверждена ООИОТ, поэтому вы не можете добавить входящую карту");
            }

            if (_db.CardConfirmations.Any(x => x.CardId == item.ParentId && x.CardConfirmationObjectId == 1))
            {
                throw new BusinessLogicException("Родительская карта утверждена");
            }

            item.ParentId = null;

            _db.Cards.Update(item);

            await _db.SaveChangesAsync();
        }

        public async Task DeleteInstalledCard(int id)
        {
            var item = await _db.Cards.FindAsync(id);

            if (item == null)
                throw new NotFoundException();

            var parent = await _db.Cards.FindAsync(item.ParentId2);

            if(parent == null)
                throw new NotFoundException();

            if (parent.IsDepartment4Confirmed &&
                parent.IsDepartment5Confirmed &&
                parent.IsDepartment6Confirmed &&
                parent.IsDepartment13Confirmed &&
                parent.IsDepartment17Confirmed &&
                parent.IsDepartment80Confirmed &&
                parent.IsDepartment82Confirmed)
            {
                throw new BusinessLogicException("Карта утверждена ООИОТ, поэтому вы не можете добавить входящую карту");
            }

            if (_db.CardConfirmations.Any(x => x.CardId == item.ParentId2 && x.CardConfirmationObjectId == 1))
            {
                throw new BusinessLogicException("Родительская карта утверждена");
            }

            item.ParentId2 = null;

            _db.Cards.Update(item);

            await _db.SaveChangesAsync();
        }

        public DataHelperViewModel<Card> GetCards(HttpRequest request)
            => request.GetDataForJQueryDataTable(_db.Cards.AsQueryable(), "Number", "ProductCode", "ProductName");
    }
}
