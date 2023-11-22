using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Reporting.NETCore;
using RepairCardsMVC.Models;
using RepairCardsMVC.ViewModels;

namespace RepairCardsMVC.Controllers
{
    public class CardReportController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly IWebHostEnvironment _environment;
        private readonly IConfiguration _configuration;

        public CardReportController(
            ApplicationDbContext db,
            IWebHostEnvironment environment,
            IConfiguration configuration)
        {
            _db = db;
            _environment = environment;
            _configuration = configuration;
        }

        public IActionResult Index(
            int cardId,
            int cardsPageNumber = 1,
            string cardsFilter = "")
        {
            var coefficients = _db.SalariedEmployeeLaborCoefficients.ToList();

            var model = new CardReportViewModel
            {
                Coefficients = coefficients,
                CardId = cardId,
                CardsPageNumber = cardsPageNumber,
                CardsFilter = cardsFilter
            };

            return View(model);
        }

        public IActionResult GetMilitaryRepairReport(int id, string format)
        {
            using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnectionString"));
            connection.Open();

            var card = connection.Query<ReportCardViewModel>(
@"select c.*, 
(select Number from CRCards p where p.Id = c.ParentId) ParentNumber,
(select ProductCode from CRCards p where p.Id = c.ParentId) ParentProductCode,
(select SpecificationType from ref_TypeOfContract toc where toc.Name = c.Direction) SpecificationType,
t.Name RepairType 
from CRCards c
left join CRRepairTypes t on c.RepairTypeId = t.Id
where c.Id = @Id", new { Id = id });

            var cardDetails = connection.Query<CardDetails>(
@"select * from CRCardDetails where Id = @Id",
 new { Id = id });

            var planOperations = connection.Query<PlanCardOperationViewModel>(
@"select o.*, e.Name Executor from CRCardOperations o
left join CRExecutors e on e.Id = o.ExecutorId
where o.CardId = @CardId and o.[Type] = @Type",
new { CardId = id, Type = 0 });

            var factOperations = connection.Query<FactCardOperationViewModel>(
@"select o.*, e.Name Executor from CRCardOperations o
left join CRExecutors e on e.Id = o.ExecutorId
where o.CardId = @CardId and o.[Type] = @Type",
new { CardId = id, Type = 1 });

            var productsAndMaterials = connection.Query<ProductsAndMaterialsViewModel>(
@"select Code, Name, Count, 'шт' UnitName, 'ДСЕ' Type from CRCardOwnProducts where CardId = @CardId
union all
select Code, Name, Count, 'шт' UnitName, 'ПКИ' Type from CRCardPurchasedProducts where CardId = @CardId
union
select m.Code, m.Name, m.Count, u.Name UnitName, 'Материал' Type from CRCardMaterials m left join tUnit u on m.UnitId = u.UnitId where m.CardId = @CardId",
new { CardId = id });

            var productOperations = connection.Query<ReportProductOperationViewModel>(
@"select o.*, 1 OperationCount, p.Id ProductId, p.Code ProductCode, p.Count ProductCount, e.Name Executor 
from CRCards c 
join CRCardOwnProducts p on p.CardId = c.Id
join CRCardOwnProductOperations o on o.CardOwnProductId = p.Id
left join CRExecutors e on e.Id = o.ExecutorId
where c.Id = @CardId

union all

select o.*, 1 OperationCount, p.Id ProductId, p.Code ProductCode, p.Count ProductCount, e.Name Executor 
from CRCards c 
join CRCardPurchasedProducts p on p.CardId = c.Id
join CRCardPurchasedProductOperations o on o.CardPurchasedProductId = p.Id
left join CRExecutors e on e.Id = o.ExecutorId
where c.Id = @CardId

union all

select o.Id, o.CardOwnProductId, o.Code, o.Name, o.Labor, null Type, o.Date, o.Department, o.ExecutorId, 
o.RowVersion, o.ConfirmUserId, o.ConfirmUserName, o.Count OperationCount, p.Id ProductId, p.Code ProductCode, p.Count ProductCount, e.Name Executor 
from CRCards c 
join CRCardOwnProducts p on p.CardId = c.Id
join CRCardOwnProductRepairOperations o on o.CardOwnProductId = p.Id
left join CRExecutors e on e.Id = o.ExecutorId
where c.Id = @CardId",
new { CardId = id });

            var extractedCards = connection.Query<Card, RepairType, CardStatus, Card>(
@"select c.*, 
(select top 1 Number from CRCards p where p.Id = c.ParentId) ParentNumber,
t.*,
s.*
from CRCards c
left join CRRepairTypes t on c.RepairTypeId = t.Id
left join CRCardStatuses s on c.CardStatusId = s.Id
where c.ParentId = @Id
order by c.Id desc",
(c, t, s) => { c.RepairType = t; c.CardStatus = s; return c; },
new { Id = id });

            var installedCards = connection.Query<Card, RepairType, CardStatus, Card>(
@"select c.*, 
t.*,
s.*
from CRCards c
left join CRRepairTypes t on c.RepairTypeId = t.Id
left join CRCardStatuses s on c.CardStatusId = s.Id
where c.ParentId2 = @Id
order by c.Id desc",
(c, t, s) => { c.RepairType = t; c.CardStatus = s; return c; },
new { Id = id });

            var documents = connection.Query<CardDocument>(
@"select Name from CRCardDocuments
where CardId = @CardId
", new { CardId = id });

			var otkCardConfirmationUsername = connection.QueryFirstOrDefault<string?>(
@"select UserName from CRCardConfirmations 
where CardId = @CardId and CardConfirmationObjectId = 1 and UserRoleId = 6",
new { CardId = id });

            var innerDefectsConfirmationUsername = connection.QueryFirstOrDefault<string?>(
@"select UserName from CRCardConfirmations 
where CardId = @CardId and CardConfirmationObjectId = 2 and UserRoleId = 9",
new { CardId = id });

            var malfunctionsConfirmationUsername = connection.QueryFirstOrDefault<string?>(
@"select UserName from CRCardConfirmations 
where CardId = @CardId and CardConfirmationObjectId = 3 and UserRoleId = 9",
new { CardId = id });

            var externalDefectsConfirmationUsername = connection.QueryFirstOrDefault<string?>(
@"select UserName from CRCardConfirmations 
where CardId = @CardId and CardConfirmationObjectId = 4 and UserRoleId = 9",
new { CardId = id });

            var otkInnerDefectsUsername = connection.QueryFirstOrDefault<string?>(
@"select UserName from CRCardConfirmations 
where CardId = @CardId and CardConfirmationObjectId = 2 and UserRoleId = 6",
new { CardId = id });

            var otkMalfunctionsUsername = connection.QueryFirstOrDefault<string?>(
@"select UserName from CRCardConfirmations 
where CardId = @CardId and CardConfirmationObjectId = 3 and UserRoleId = 6",
new { CardId = id });

            var otkExternalDefectsUsername = connection.QueryFirstOrDefault<string?>(
@"select UserName from CRCardConfirmations 
where CardId = @CardId and CardConfirmationObjectId = 4 and UserRoleId = 6",
new { CardId = id });

            var parameters = new ReportParameterCollection
            {
                new ReportParameter("otkCardConfirmationUsername", otkCardConfirmationUsername),
                new ReportParameter("innerDefectsConfirmationUsername", innerDefectsConfirmationUsername),
                new ReportParameter("malfunctionsConfirmationUsername", malfunctionsConfirmationUsername),
                new ReportParameter("externalDefectsConfirmationUsername", externalDefectsConfirmationUsername),
                new ReportParameter("otkInnerDefectsUsername", otkInnerDefectsUsername),
                new ReportParameter("otkMalfunctionsUsername", otkMalfunctionsUsername),
                new ReportParameter("otkExternalDefectsUsername", otkExternalDefectsUsername),
            };

            string path = Path.Combine(_environment.ContentRootPath, "Reports\\MilitaryRepairReport.rdlc");

            using TextReader stream = System.IO.File.OpenText(path);
            using var report = new LocalReport();
            report.LoadReportDefinition(stream);

            report.DataSources.Add(new ReportDataSource("Card", card));
            report.DataSources.Add(new ReportDataSource("CardDetails", cardDetails));
            report.DataSources.Add(new ReportDataSource("PlanOperations", planOperations));
            report.DataSources.Add(new ReportDataSource("FactOperations", factOperations));
            report.DataSources.Add(new ReportDataSource("ProductsAndMaterials", productsAndMaterials));
            report.DataSources.Add(new ReportDataSource("ProductOperations", productOperations));
            report.DataSources.Add(new ReportDataSource("Documents", documents));
            report.DataSources.Add(new ReportDataSource("DismantledProductCards", extractedCards));
            report.DataSources.Add(new ReportDataSource("InstalledProductCards", installedCards));
            report.SetParameters(parameters);

            var file = GetFile(report, format);

            if (file == null)
                return BadRequest();

            return file;
        }

        public IActionResult GetCivilianRepairReport(int id, string format)
        {
            using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnectionString"));
            connection.Open();

            var card = connection.Query<ReportCardViewModel>(
@"select c.*, 
(select Number from CRCards p where p.Id = c.ParentId) ParentNumber,
(select ProductCode from CRCards p where p.Id = c.ParentId) ParentProductCode,
(select SpecificationType from ref_TypeOfContract toc where toc.Name = c.Direction) SpecificationType,
t.Name RepairType 
from CRCards c
left join CRRepairTypes t on c.RepairTypeId = t.Id
where c.Id = @Id", new { Id = id });

            var cardDetails = connection.Query<CardDetails>(
@"select * from CRCardDetails where Id = @Id",
 new { Id = id });

            var planOperations = connection.Query<PlanCardOperationViewModel>(
@"select o.*, e.Name Executor from CRCardOperations o
left join CRExecutors e on e.Id = o.ExecutorId
where o.CardId = @CardId and o.[Type] = @Type",
new { CardId = id, Type = 0 });

            var factOperations = connection.Query<FactCardOperationViewModel>(
@"select o.*, e.Name Executor from CRCardOperations o
left join CRExecutors e on e.Id = o.ExecutorId
where o.CardId = @CardId and o.[Type] = @Type",
new { CardId = id, Type = 1 });

            var productsAndMaterials = connection.Query<ProductsAndMaterialsViewModel>(
@"select Code, Name, Count, 'шт' UnitName, 'ДСЕ' Type from CRCardOwnProducts where CardId = @CardId
union all
select Code, Name, Count, 'шт' UnitName, 'ПКИ' Type from CRCardPurchasedProducts where CardId = @CardId
union
select m.Code, m.Name, m.Count, u.Name UnitName, 'Материал' Type from CRCardMaterials m left join tUnit u on m.UnitId = u.UnitId where m.CardId = @CardId",
new { CardId = id });

            var productOperations = connection.Query<ReportProductOperationViewModel>(
@"select o.*, 1 OperationCount, p.Id ProductId, p.Code ProductCode, p.Count ProductCount, e.Name Executor 
from CRCards c 
join CRCardOwnProducts p on p.CardId = c.Id
join CRCardOwnProductOperations o on o.CardOwnProductId = p.Id
left join CRExecutors e on e.Id = o.ExecutorId
where c.Id = @CardId

union all

select o.*, 1 OperationCount, p.Id ProductId, p.Code ProductCode, p.Count ProductCount, e.Name Executor 
from CRCards c 
join CRCardPurchasedProducts p on p.CardId = c.Id
join CRCardPurchasedProductOperations o on o.CardPurchasedProductId = p.Id
left join CRExecutors e on e.Id = o.ExecutorId
where c.Id = @CardId

union all

select o.Id, o.CardOwnProductId, o.Code, o.Name, o.Labor, null Type, o.Date, o.Department, o.ExecutorId, 
o.RowVersion, o.ConfirmUserId, o.ConfirmUserName, o.Count OperationCount, p.Id ProductId, p.Code ProductCode, p.Count ProductCount, e.Name Executor 
from CRCards c 
join CRCardOwnProducts p on p.CardId = c.Id
join CRCardOwnProductRepairOperations o on o.CardOwnProductId = p.Id
left join CRExecutors e on e.Id = o.ExecutorId
where c.Id = @CardId",
new { CardId = id });

            var extractedCards = connection.Query<Card, RepairType, CardStatus, Card>(
@"select c.*, 
(select top 1 Number from CRCards p where p.Id = c.ParentId) ParentNumber,
t.*,
s.*
from CRCards c
left join CRRepairTypes t on c.RepairTypeId = t.Id
left join CRCardStatuses s on c.CardStatusId = s.Id
where c.ParentId = @Id
order by c.Id desc",
(c, t, s) => { c.RepairType = t; c.CardStatus = s; return c; },
new { Id = id });

            var installedCards = connection.Query<Card, RepairType, CardStatus, Card>(
@"select c.*, 
t.*,
s.*
from CRCards c
left join CRRepairTypes t on c.RepairTypeId = t.Id
left join CRCardStatuses s on c.CardStatusId = s.Id
where c.ParentId2 = @Id
order by c.Id desc",
(c, t, s) => { c.RepairType = t; c.CardStatus = s; return c; },
new { Id = id });

            var documents = connection.Query<CardDocument>(
@"select Name from CRCardDocuments
where CardId = @CardId
", new { CardId = id });

            var otkCardConfirmationUsername = connection.QueryFirstOrDefault<string?>(
@"select UserName from CRCardConfirmations 
where CardId = @CardId and CardConfirmationObjectId = 1 and UserRoleId = 6",
new { CardId = id });

            var innerDefectsConfirmationUsername = connection.QueryFirstOrDefault<string?>(
@"select UserName from CRCardConfirmations 
where CardId = @CardId and CardConfirmationObjectId = 2 and UserRoleId = 9",
new { CardId = id });

            var malfunctionsConfirmationUsername = connection.QueryFirstOrDefault<string?>(
@"select UserName from CRCardConfirmations 
where CardId = @CardId and CardConfirmationObjectId = 3 and UserRoleId = 9",
new { CardId = id });

            var externalDefectsConfirmationUsername = connection.QueryFirstOrDefault<string?>(
@"select UserName from CRCardConfirmations 
where CardId = @CardId and CardConfirmationObjectId = 4 and UserRoleId = 9",
new { CardId = id });

            var otkInnerDefectsUsername = connection.QueryFirstOrDefault<string?>(
@"select UserName from CRCardConfirmations 
where CardId = @CardId and CardConfirmationObjectId = 2 and UserRoleId = 6",
new { CardId = id });

            var otkMalfunctionsUsername = connection.QueryFirstOrDefault<string?>(
@"select UserName from CRCardConfirmations 
where CardId = @CardId and CardConfirmationObjectId = 3 and UserRoleId = 6",
new { CardId = id });

            var otkExternalDefectsUsername = connection.QueryFirstOrDefault<string?>(
@"select UserName from CRCardConfirmations 
where CardId = @CardId and CardConfirmationObjectId = 4 and UserRoleId = 6",
new { CardId = id });

            var parameters = new ReportParameterCollection
            {
                new ReportParameter("otkCardConfirmationUsername", otkCardConfirmationUsername),
                new ReportParameter("innerDefectsConfirmationUsername", innerDefectsConfirmationUsername),
                new ReportParameter("malfunctionsConfirmationUsername", malfunctionsConfirmationUsername),
                new ReportParameter("externalDefectsConfirmationUsername", externalDefectsConfirmationUsername),
                new ReportParameter("otkInnerDefectsUsername", otkInnerDefectsUsername),
                new ReportParameter("otkMalfunctionsUsername", otkMalfunctionsUsername),
                new ReportParameter("otkExternalDefectsUsername", otkExternalDefectsUsername),
            };

            string path = Path.Combine(_environment.ContentRootPath, "Reports\\CivilianRepairReport.rdlc");

            using TextReader stream = System.IO.File.OpenText(path);
            using var report = new LocalReport();
            report.LoadReportDefinition(stream);

            report.DataSources.Add(new ReportDataSource("Card", card));
            report.DataSources.Add(new ReportDataSource("CardDetails", cardDetails));
            report.DataSources.Add(new ReportDataSource("PlanOperations", planOperations));
            report.DataSources.Add(new ReportDataSource("FactOperations", factOperations));
            report.DataSources.Add(new ReportDataSource("ProductsAndMaterials", productsAndMaterials));
            report.DataSources.Add(new ReportDataSource("ProductOperations", productOperations));
            report.DataSources.Add(new ReportDataSource("Documents", documents));
            report.DataSources.Add(new ReportDataSource("DismantledProductCards", extractedCards));
            report.DataSources.Add(new ReportDataSource("InstalledProductCards", installedCards));
            report.SetParameters(parameters);

            var file = GetFile(report, format);

            if (file == null)
                return BadRequest();

            return file;
        }

        public IActionResult GetMilitaryRepairWithoutOrderReport(int id, string format)
        {
            using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnectionString"));
            connection.Open();

            var card = connection.Query<ReportCardViewModel>(
@"select c.*, 
(select Number from CRCards p where p.Id = c.ParentId) ParentNumber,
(select ProductCode from CRCards p where p.Id = c.ParentId) ParentProductCode,
(select SpecificationType from ref_TypeOfContract toc where toc.Name = c.Direction) SpecificationType,
t.Name RepairType 
from CRCards c
left join CRRepairTypes t on c.RepairTypeId = t.Id
where c.Id = @Id", new { Id = id });

            var cardDetails = connection.Query<CardDetails>(
@"select * from CRCardDetails where Id = @Id",
 new { Id = id });

            var planOperations = connection.Query<PlanCardOperationViewModel>(
@"select o.*, e.Name Executor from CRCardOperations o
left join CRExecutors e on e.Id = o.ExecutorId
where o.CardId = @CardId and o.[Type] = @Type",
new { CardId = id, Type = 0 });

            var factOperations = connection.Query<FactCardOperationViewModel>(
@"select o.*, e.Name Executor from CRCardOperations o
left join CRExecutors e on e.Id = o.ExecutorId
where o.CardId = @CardId and o.[Type] = @Type",
new { CardId = id, Type = 1 });

            var productsAndMaterials = connection.Query<ProductsAndMaterialsViewModel>(
@"select Code, Name, Count, 'шт' UnitName, 'ДСЕ' Type from CRCardOwnProducts where CardId = @CardId
union all
select Code, Name, Count, 'шт' UnitName, 'ПКИ' Type from CRCardPurchasedProducts where CardId = @CardId
union
select m.Code, m.Name, m.Count, u.Name UnitName, 'Материал' Type from CRCardMaterials m left join tUnit u on m.UnitId = u.UnitId where m.CardId = @CardId",
new { CardId = id });

            var productOperations = connection.Query<ReportProductOperationViewModel>(
@"select o.*, 1 OperationCount, p.Id ProductId, p.Code ProductCode, p.Count ProductCount, e.Name Executor 
from CRCards c 
join CRCardOwnProducts p on p.CardId = c.Id
join CRCardOwnProductOperations o on o.CardOwnProductId = p.Id
left join CRExecutors e on e.Id = o.ExecutorId
where c.Id = @CardId

union all

select o.*, 1 OperationCount, p.Id ProductId, p.Code ProductCode, p.Count ProductCount, e.Name Executor 
from CRCards c 
join CRCardPurchasedProducts p on p.CardId = c.Id
join CRCardPurchasedProductOperations o on o.CardPurchasedProductId = p.Id
left join CRExecutors e on e.Id = o.ExecutorId
where c.Id = @CardId

union all

select o.Id, o.CardOwnProductId, o.Code, o.Name, o.Labor, null Type, o.Date, o.Department, o.ExecutorId, 
o.RowVersion, o.ConfirmUserId, o.ConfirmUserName, o.Count OperationCount, p.Id ProductId, p.Code ProductCode, p.Count ProductCount, e.Name Executor 
from CRCards c 
join CRCardOwnProducts p on p.CardId = c.Id
join CRCardOwnProductRepairOperations o on o.CardOwnProductId = p.Id
left join CRExecutors e on e.Id = o.ExecutorId
where c.Id = @CardId",
new { CardId = id });

            var extractedCards = connection.Query<Card, RepairType, CardStatus, Card>(
@"select c.*, 
(select top 1 Number from CRCards p where p.Id = c.ParentId) ParentNumber,
t.*,
s.*
from CRCards c
left join CRRepairTypes t on c.RepairTypeId = t.Id
left join CRCardStatuses s on c.CardStatusId = s.Id
where c.ParentId = @Id
order by c.Id desc",
(c, t, s) => { c.RepairType = t; c.CardStatus = s; return c; },
new { Id = id });

            var installedCards = connection.Query<Card, RepairType, CardStatus, Card>(
@"select c.*, 
t.*,
s.*
from CRCards c
left join CRRepairTypes t on c.RepairTypeId = t.Id
left join CRCardStatuses s on c.CardStatusId = s.Id
where c.ParentId2 = @Id
order by c.Id desc",
(c, t, s) => { c.RepairType = t; c.CardStatus = s; return c; },
new { Id = id });

            var documents = connection.Query<CardDocument>(
@"select Name from CRCardDocuments
where CardId = @CardId
", new { CardId = id });

            var otkCardConfirmationUsername = connection.QueryFirstOrDefault<string?>(
@"select UserName from CRCardConfirmations 
where CardId = @CardId and CardConfirmationObjectId = 1 and UserRoleId = 6",
new { CardId = id });

            var innerDefectsConfirmationUsername = connection.QueryFirstOrDefault<string?>(
@"select UserName from CRCardConfirmations 
where CardId = @CardId and CardConfirmationObjectId = 2 and UserRoleId = 9",
new { CardId = id });

            var malfunctionsConfirmationUsername = connection.QueryFirstOrDefault<string?>(
@"select UserName from CRCardConfirmations 
where CardId = @CardId and CardConfirmationObjectId = 3 and UserRoleId = 9",
new { CardId = id });

            var externalDefectsConfirmationUsername = connection.QueryFirstOrDefault<string?>(
@"select UserName from CRCardConfirmations 
where CardId = @CardId and CardConfirmationObjectId = 4 and UserRoleId = 9",
new { CardId = id });

            var otkInnerDefectsUsername = connection.QueryFirstOrDefault<string?>(
@"select UserName from CRCardConfirmations 
where CardId = @CardId and CardConfirmationObjectId = 2 and UserRoleId = 6",
new { CardId = id });

            var otkMalfunctionsUsername = connection.QueryFirstOrDefault<string?>(
@"select UserName from CRCardConfirmations 
where CardId = @CardId and CardConfirmationObjectId = 3 and UserRoleId = 6",
new { CardId = id });

            var otkExternalDefectsUsername = connection.QueryFirstOrDefault<string?>(
@"select UserName from CRCardConfirmations 
where CardId = @CardId and CardConfirmationObjectId = 4 and UserRoleId = 6",
new { CardId = id });

            var parameters = new ReportParameterCollection
            {
                new ReportParameter("otkCardConfirmationUsername", otkCardConfirmationUsername),
                new ReportParameter("innerDefectsConfirmationUsername", innerDefectsConfirmationUsername),
                new ReportParameter("malfunctionsConfirmationUsername", malfunctionsConfirmationUsername),
                new ReportParameter("externalDefectsConfirmationUsername", externalDefectsConfirmationUsername),
                new ReportParameter("otkInnerDefectsUsername", otkInnerDefectsUsername),
                new ReportParameter("otkMalfunctionsUsername", otkMalfunctionsUsername),
                new ReportParameter("otkExternalDefectsUsername", otkExternalDefectsUsername),
            };

            string path = Path.Combine(_environment.ContentRootPath, "Reports\\MilitaryRepairWithoutOrderReport.rdlc");

            using TextReader stream = System.IO.File.OpenText(path);
            using var report = new LocalReport();
            report.LoadReportDefinition(stream);

            report.DataSources.Add(new ReportDataSource("Card", card));
            report.DataSources.Add(new ReportDataSource("CardDetails", cardDetails));
            report.DataSources.Add(new ReportDataSource("PlanOperations", planOperations));
            report.DataSources.Add(new ReportDataSource("FactOperations", factOperations));
            report.DataSources.Add(new ReportDataSource("ProductsAndMaterials", productsAndMaterials));
            report.DataSources.Add(new ReportDataSource("ProductOperations", productOperations));
            report.DataSources.Add(new ReportDataSource("Documents", documents));
            report.DataSources.Add(new ReportDataSource("DismantledProductCards", extractedCards));
            report.DataSources.Add(new ReportDataSource("InstalledProductCards", installedCards));
            report.SetParameters(parameters);

            var file = GetFile(report, format);

            if (file == null)
                return BadRequest();

            return file;
        }

        public IActionResult GetCivilianRepairWithoutOrderReport(int id, string format)
        {
            using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnectionString"));
            connection.Open();

            var card = connection.Query<ReportCardViewModel>(
@"select c.*, 
(select Number from CRCards p where p.Id = c.ParentId) ParentNumber,
(select ProductCode from CRCards p where p.Id = c.ParentId) ParentProductCode,
(select SpecificationType from ref_TypeOfContract toc where toc.Name = c.Direction) SpecificationType,
t.Name RepairType 
from CRCards c
left join CRRepairTypes t on c.RepairTypeId = t.Id
where c.Id = @Id", new { Id = id });

            var cardDetails = connection.Query<CardDetails>(
@"select * from CRCardDetails where Id = @Id",
 new { Id = id });

            var planOperations = connection.Query<PlanCardOperationViewModel>(
@"select o.*, e.Name Executor from CRCardOperations o
left join CRExecutors e on e.Id = o.ExecutorId
where o.CardId = @CardId and o.[Type] = @Type",
new { CardId = id, Type = 0 });

            var factOperations = connection.Query<FactCardOperationViewModel>(
@"select o.*, e.Name Executor from CRCardOperations o
left join CRExecutors e on e.Id = o.ExecutorId
where o.CardId = @CardId and o.[Type] = @Type",
new { CardId = id, Type = 1 });

            var productsAndMaterials = connection.Query<ProductsAndMaterialsViewModel>(
@"select Code, Name, Count, 'шт' UnitName, 'ДСЕ' Type from CRCardOwnProducts where CardId = @CardId
union all
select Code, Name, Count, 'шт' UnitName, 'ПКИ' Type from CRCardPurchasedProducts where CardId = @CardId
union
select m.Code, m.Name, m.Count, u.Name UnitName, 'Материал' Type from CRCardMaterials m left join tUnit u on m.UnitId = u.UnitId where m.CardId = @CardId",
new { CardId = id });

            var productOperations = connection.Query<ReportProductOperationViewModel>(
@"select o.*, 1 OperationCount, p.Id ProductId, p.Code ProductCode, p.Count ProductCount, e.Name Executor 
from CRCards c 
join CRCardOwnProducts p on p.CardId = c.Id
join CRCardOwnProductOperations o on o.CardOwnProductId = p.Id
left join CRExecutors e on e.Id = o.ExecutorId
where c.Id = @CardId

union all

select o.*, 1 OperationCount, p.Id ProductId, p.Code ProductCode, p.Count ProductCount, e.Name Executor 
from CRCards c 
join CRCardPurchasedProducts p on p.CardId = c.Id
join CRCardPurchasedProductOperations o on o.CardPurchasedProductId = p.Id
left join CRExecutors e on e.Id = o.ExecutorId
where c.Id = @CardId

union all

select o.Id, o.CardOwnProductId, o.Code, o.Name, o.Labor, null Type, o.Date, o.Department, o.ExecutorId, 
o.RowVersion, o.ConfirmUserId, o.ConfirmUserName, o.Count OperationCount, p.Id ProductId, p.Code ProductCode, p.Count ProductCount, e.Name Executor 
from CRCards c 
join CRCardOwnProducts p on p.CardId = c.Id
join CRCardOwnProductRepairOperations o on o.CardOwnProductId = p.Id
left join CRExecutors e on e.Id = o.ExecutorId
where c.Id = @CardId",
new { CardId = id });

            var extractedCards = connection.Query<Card, RepairType, CardStatus, Card>(
@"select c.*, 
(select top 1 Number from CRCards p where p.Id = c.ParentId) ParentNumber,
t.*,
s.*
from CRCards c
left join CRRepairTypes t on c.RepairTypeId = t.Id
left join CRCardStatuses s on c.CardStatusId = s.Id
where c.ParentId = @Id
order by c.Id desc",
(c, t, s) => { c.RepairType = t; c.CardStatus = s; return c; },
new { Id = id });

            var installedCards = connection.Query<Card, RepairType, CardStatus, Card>(
@"select c.*, 
t.*,
s.*
from CRCards c
left join CRRepairTypes t on c.RepairTypeId = t.Id
left join CRCardStatuses s on c.CardStatusId = s.Id
where c.ParentId2 = @Id
order by c.Id desc",
(c, t, s) => { c.RepairType = t; c.CardStatus = s; return c; },
new { Id = id });

            var documents = connection.Query<CardDocument>(
@"select Name from CRCardDocuments
where CardId = @CardId
", new { CardId = id });

            var otkCardConfirmationUsername = connection.QueryFirstOrDefault<string?>(
@"select UserName from CRCardConfirmations 
where CardId = @CardId and CardConfirmationObjectId = 1 and UserRoleId = 6",
new { CardId = id });

            var innerDefectsConfirmationUsername = connection.QueryFirstOrDefault<string?>(
@"select UserName from CRCardConfirmations 
where CardId = @CardId and CardConfirmationObjectId = 2 and UserRoleId = 9",
new { CardId = id });

            var malfunctionsConfirmationUsername = connection.QueryFirstOrDefault<string?>(
@"select UserName from CRCardConfirmations 
where CardId = @CardId and CardConfirmationObjectId = 3 and UserRoleId = 9",
new { CardId = id });

            var externalDefectsConfirmationUsername = connection.QueryFirstOrDefault<string?>(
@"select UserName from CRCardConfirmations 
where CardId = @CardId and CardConfirmationObjectId = 4 and UserRoleId = 9",
new { CardId = id });

            var otkInnerDefectsUsername = connection.QueryFirstOrDefault<string?>(
@"select UserName from CRCardConfirmations 
where CardId = @CardId and CardConfirmationObjectId = 2 and UserRoleId = 6",
new { CardId = id });

            var otkMalfunctionsUsername = connection.QueryFirstOrDefault<string?>(
@"select UserName from CRCardConfirmations 
where CardId = @CardId and CardConfirmationObjectId = 3 and UserRoleId = 6",
new { CardId = id });

            var otkExternalDefectsUsername = connection.QueryFirstOrDefault<string?>(
@"select UserName from CRCardConfirmations 
where CardId = @CardId and CardConfirmationObjectId = 4 and UserRoleId = 6",
new { CardId = id });

            var parameters = new ReportParameterCollection
            {
                new ReportParameter("otkCardConfirmationUsername", otkCardConfirmationUsername),
                new ReportParameter("innerDefectsConfirmationUsername", innerDefectsConfirmationUsername),
                new ReportParameter("malfunctionsConfirmationUsername", malfunctionsConfirmationUsername),
                new ReportParameter("externalDefectsConfirmationUsername", externalDefectsConfirmationUsername),
                new ReportParameter("otkInnerDefectsUsername", otkInnerDefectsUsername),
                new ReportParameter("otkMalfunctionsUsername", otkMalfunctionsUsername),
                new ReportParameter("otkExternalDefectsUsername", otkExternalDefectsUsername),
            };

            string path = Path.Combine(_environment.ContentRootPath, "Reports\\CivilianRepairWithoutOrderReport.rdlc");

            using TextReader stream = System.IO.File.OpenText(path);
            using var report = new LocalReport();
            report.LoadReportDefinition(stream);

            report.DataSources.Add(new ReportDataSource("Card", card));
            report.DataSources.Add(new ReportDataSource("CardDetails", cardDetails));
            report.DataSources.Add(new ReportDataSource("PlanOperations", planOperations));
            report.DataSources.Add(new ReportDataSource("FactOperations", factOperations));
            report.DataSources.Add(new ReportDataSource("ProductsAndMaterials", productsAndMaterials));
            report.DataSources.Add(new ReportDataSource("ProductOperations", productOperations));
            report.DataSources.Add(new ReportDataSource("Documents", documents));
            report.DataSources.Add(new ReportDataSource("DismantledProductCards", extractedCards));
            report.DataSources.Add(new ReportDataSource("InstalledProductCards", installedCards));
            report.SetParameters(parameters);

            var file = GetFile(report, format);

            if (file == null)
                return BadRequest();

            return file;
        }

        public IActionResult GetCertificateOfRepairReport(int id, string format)
        {
            var card = _db.Cards.Find(id);

            var items = GetAllChildrenCards(card);
            items.RemoveAll(x => x.HasNotBeenRepaired || x.Id == card.Id);

            string path = Path.Combine(_environment.ContentRootPath, "Reports\\CertificateOfRepairReport.rdlc");

            TextReader stream = System.IO.File.OpenText(path);
            using var report = new LocalReport();
            report.LoadReportDefinition(stream);

            report.DataSources.Add(new ReportDataSource("Card", new List<Card> { card }));
            report.DataSources.Add(new ReportDataSource("Items", items));

            var file = GetFile(report, format);

            if (file == null)
                return BadRequest();

            return file;
        }

        public IActionResult GetExecutedWorksReport(int id, string format)
        {
            using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnectionString"));
            connection.Open();

            var card = connection.Query<ReportCardViewModel>(
@"select c.*, 
(select Number from CRCards p where p.Id = c.ParentId) ParentNumber,
(select ProductCode from CRCards p where p.Id = c.ParentId) ParentProductCode,
(select SpecificationType from ref_TypeOfContract toc where toc.Name = c.Direction) SpecificationType,
t.Name RepairType 
from CRCards c
left join CRRepairTypes t on c.RepairTypeId = t.Id
where c.Id = @Id", new { Id = id });

            var factOperations = connection.Query<FactCardOperationViewModel>(
@"select o.*, e.Name Executor from CRCardOperations o
left join CRExecutors e on e.Id = o.ExecutorId
where o.CardId = @CardId and o.[Type] = @Type",
new { CardId = id, Type = 1 });

            var productOperations = connection.Query<ReportProductOperationViewModel>(
@"select o.*, 1 OperationCount, p.Id ProductId, p.Code ProductCode, p.Count ProductCount, e.Name Executor 
from CRCards c 
join CRCardOwnProducts p on p.CardId = c.Id
join CRCardOwnProductOperations o on o.CardOwnProductId = p.Id
left join CRExecutors e on e.Id = o.ExecutorId
where c.Id = @CardId

union all

select o.*, 1 OperationCount, p.Id ProductId, p.Code ProductCode, p.Count ProductCount, e.Name Executor 
from CRCards c 
join CRCardPurchasedProducts p on p.CardId = c.Id
join CRCardPurchasedProductOperations o on o.CardPurchasedProductId = p.Id
left join CRExecutors e on e.Id = o.ExecutorId
where c.Id = @CardId

union all

select o.Id, o.CardOwnProductId, o.Code, o.Name, o.Labor, null Type, o.Date, o.Department, o.ExecutorId, 
o.RowVersion, o.ConfirmUserId, o.ConfirmUserName, o.Count OperationCount, p.Id ProductId, p.Code ProductCode, p.Count ProductCount, e.Name Executor 
from CRCards c 
join CRCardOwnProducts p on p.CardId = c.Id
join CRCardOwnProductRepairOperations o on o.CardOwnProductId = p.Id
left join CRExecutors e on e.Id = o.ExecutorId
where c.Id = @CardId",
new { CardId = id });

            string path = Path.Combine(_environment.ContentRootPath, "Reports\\ExecutedWorksReport.rdlc");

            using TextReader stream = System.IO.File.OpenText(path);
            using var report = new LocalReport();
            report.LoadReportDefinition(stream);

            report.DataSources.Add(new ReportDataSource("Card", card));
            report.DataSources.Add(new ReportDataSource("FactOperations", factOperations));
            report.DataSources.Add(new ReportDataSource("ProductOperations", productOperations));

            var file = GetFile(report, format);

            if (file == null)
                return BadRequest();

            return file;
        }

        public IActionResult GetExecutedWorksForAssemblyDepartmentsReport(int id, string format)
        {
            using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnectionString"));
            connection.Open();

            var card = connection.Query<ReportCardViewModel>(
@"select c.*, 
(select Number from CRCards p where p.Id = c.ParentId) ParentNumber,
(select ProductCode from CRCards p where p.Id = c.ParentId) ParentProductCode,
(select SpecificationType from ref_TypeOfContract toc where toc.Name = c.Direction) SpecificationType,
t.Name RepairType 
from CRCards c
left join CRRepairTypes t on c.RepairTypeId = t.Id
where c.Id = @Id", new { Id = id });

            var factOperations = connection.Query<FactCardOperationViewModel>(
@"select o.*, e.Name Executor from CRCardOperations o
left join CRExecutors e on e.Id = o.ExecutorId
where o.CardId = @CardId and o.[Type] = @Type",
new { CardId = id, Type = 1 });

            var productOperations = connection.Query<ReportProductOperationViewModel>(
@"select o.*, 1 OperationCount, p.Id ProductId, p.Code ProductCode, p.Count ProductCount, e.Name Executor 
from CRCards c 
join CRCardOwnProducts p on p.CardId = c.Id
join CRCardOwnProductOperations o on o.CardOwnProductId = p.Id
left join CRExecutors e on e.Id = o.ExecutorId
where c.Id = @CardId

union all

select o.*, 1 OperationCount, p.Id ProductId, p.Code ProductCode, p.Count ProductCount, e.Name Executor 
from CRCards c 
join CRCardPurchasedProducts p on p.CardId = c.Id
join CRCardPurchasedProductOperations o on o.CardPurchasedProductId = p.Id
left join CRExecutors e on e.Id = o.ExecutorId
where c.Id = @CardId

union all

select o.Id, o.CardOwnProductId, o.Code, o.Name, o.Labor, null Type, o.Date, o.Department, o.ExecutorId, 
o.RowVersion, o.ConfirmUserId, o.ConfirmUserName, o.Count OperationCount, p.Id ProductId, p.Code ProductCode, p.Count ProductCount, e.Name Executor 
from CRCards c 
join CRCardOwnProducts p on p.CardId = c.Id
join CRCardOwnProductRepairOperations o on o.CardOwnProductId = p.Id
left join CRExecutors e on e.Id = o.ExecutorId
where c.Id = @CardId",
new { CardId = id });

            string path = Path.Combine(_environment.ContentRootPath, "Reports\\ExecutedWorksForAssemblyDepartmentsReport.rdlc");

            using TextReader stream = System.IO.File.OpenText(path);
            using var report = new LocalReport();
            report.LoadReportDefinition(stream);

            report.DataSources.Add(new ReportDataSource("Card", card));
            report.DataSources.Add(new ReportDataSource("FactOperations", factOperations));
            report.DataSources.Add(new ReportDataSource("ProductOperations", productOperations));

            var file = GetFile(report, format);

            if (file == null)
                return BadRequest();

            return file;
        }

        public IActionResult GetOperationsReport(int id, string type, string format)
        {
            using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnectionString"));
            connection.Open();

            var card = connection.Query<ReportCardViewModel>(
@"select c.*, 
(select Number from CRCards p where p.Id = c.ParentId) ParentNumber,
(select ProductCode from CRCards p where p.Id = c.ParentId) ParentProductCode,
(select SpecificationType from ref_TypeOfContract toc where toc.Name = c.Direction) SpecificationType,
t.Name RepairType 
from CRCards c
left join CRRepairTypes t on c.RepairTypeId = t.Id
where c.Id = @Id", new { Id = id });

            List<ReportOperationViewModel> items = new();

            if (type == "extracted")
            {
                items = connection.Query<ReportOperationViewModel>(
@"with cte
as
(
	select * from CRCards where Id = @CardId
	union all
	select c.* from cte
	join CRCards c on c.ParentId = cte.Id
)
select
Name,
sum(cast(Labor * Count as decimal(18, 3))) Labor
from
(
select 
isnull(GroupName, Name) Name,
Labor,
Count
from
		(
			select 
			c.Number CardNumber,
			c.ProductCode,
			c.ProductName,
			c.[Order],
			(select SpecificationType from ref_TypeOfContract toc where toc.Name = c.Direction) SpecificationType,
			coalesce(Cipher, Direction) Direction,
			c.FactoryNumber,
			co.Department,
			d.Name DepartmentName,
			co.Code,
			co.Name,
			co.Labor,
			co.Count,
			co.GroupName
			from CRCards c
			join CRCardOperations co on co.CardId = c.Id
			join Departments d on d.Number = co.Department
			where c.Id in
			(
				select Id from cte
			)
			and co.Type = 1 -- фактические

			union all

			select 
			c.Number CardNumber,
			c.ProductCode,
			c.ProductName,
			c.[Order],
			(select SpecificationType from ref_TypeOfContract toc where toc.Name = c.Direction) SpecificationType,
			coalesce(Cipher, Direction) Direction,
			c.FactoryNumber,
			o.Department,
			d.Name DepartmentName,
			o.Code,
			o.Name,
			o.Labor,
			p.Count,
			null 
			from CRCards c
			join CRCardOwnProducts p on p.CardId = c.Id
			join CRCardOwnProductOperations o on o.CardOwnProductId = p.Id
			join Departments d on d.Number = o.Department
			where c.Id in
			(
				select Id from cte
			)

			union all

			select 
			c.Number CardNumber,
			c.ProductCode,
			c.ProductName,
			c.[Order],
			(select SpecificationType from ref_TypeOfContract toc where toc.Name = c.Direction) SpecificationType,
			coalesce(Cipher, Direction) Direction,
			c.FactoryNumber,
			o.Department,
			d.Name DepartmentName,
			o.Code,
			o.Name,
			o.Labor,
			p.Count * o.Count,
			null
			from CRCards c
			join CRCardOwnProducts p on p.CardId = c.Id
			join CRCardOwnProductRepairOperations o on o.CardOwnProductId = p.Id
			join Departments d on d.Number = o.Department
			where c.Id in
			(
				select Id from cte
			)

			union all

			select 
			c.Number CardNumber,
			c.ProductCode,
			c.ProductName,
			c.[Order],
			(select SpecificationType from ref_TypeOfContract toc where toc.Name = c.Direction) SpecificationType,
			coalesce(Cipher, Direction) Direction,
			c.FactoryNumber,
			o.Department,
			d.Name DepartmentName,
			o.Code,
			o.Name,
			o.Labor,
			p.Count,
			null
			from CRCards c
			join CRCardPurchasedProducts p on p.CardId = c.Id
			join CRCardPurchasedProductOperations o on o.CardPurchasedProductId = p.Id
			join Departments d on d.Number = o.Department
			where c.Id in
			(
				select Id from cte
			)
	) r
) r
group by Name
order by Name", new { CardId = id }).ToList();
            }

            if (type == "installed")
            {
                items = connection.Query<ReportOperationViewModel>(
@"with cte
as
(
	select * from CRCards where Id = @CardId
	union all
	select c.* from cte
	join CRCards c on c.ParentId2 = cte.Id
)

select 
Name,
sum(cast(Labor * Count as decimal(18, 3))) Labor
	from
	(
		select 
		isnull(GroupName, Name) Name,
		Labor,
		Count
		from
		(
			select 
			c.Number CardNumber,
			c.ProductCode,
			c.ProductName,
			c.[Order],
			(select SpecificationType from ref_TypeOfContract toc where toc.Name = c.Direction) SpecificationType,
			coalesce(Cipher, Direction) Direction,
			c.FactoryNumber,
			co.Department,
			d.Name DepartmentName,
			co.Code,
			co.Name,
			co.Labor,
			co.Count,
			co.GroupName
			from CRCards c
			join CRCardOperations co on co.CardId = c.Id
			join Departments d on d.Number = co.Department
			where c.Id in
			(
				select Id from cte
			)
			and co.Type = 1 -- фактические

			union all

			select 
			c.Number CardNumber,
			c.ProductCode,
			c.ProductName,
			c.[Order],
			(select SpecificationType from ref_TypeOfContract toc where toc.Name = c.Direction) SpecificationType,
			coalesce(Cipher, Direction) Direction,
			c.FactoryNumber,
			o.Department,
			d.Name DepartmentName,
			o.Code,
			o.Name,
			o.Labor,
			p.Count,
			null
			from CRCards c
			join CRCardOwnProducts p on p.CardId = c.Id
			join CRCardOwnProductOperations o on o.CardOwnProductId = p.Id
			join Departments d on d.Number = o.Department
			where c.Id in
			(
				select Id from cte
			)

			union all

			select 
			c.Number CardNumber,
			c.ProductCode,
			c.ProductName,
			c.[Order],
			(select SpecificationType from ref_TypeOfContract toc where toc.Name = c.Direction) SpecificationType,
			coalesce(Cipher, Direction) Direction,
			c.FactoryNumber,
			o.Department,
			d.Name DepartmentName,
			o.Code,
			o.Name,
			o.Labor,
			p.Count * o.Count,
			null
			from CRCards c
			join CRCardOwnProducts p on p.CardId = c.Id
			join CRCardOwnProductRepairOperations o on o.CardOwnProductId = p.Id
			join Departments d on d.Number = o.Department
			where c.Id in
			(
				select Id from cte
			)

			union all

			select 
			c.Number CardNumber,
			c.ProductCode,
			c.ProductName,
			c.[Order],
			(select SpecificationType from ref_TypeOfContract toc where toc.Name = c.Direction) SpecificationType,
			coalesce(Cipher, Direction) Direction,
			c.FactoryNumber,
			o.Department,
			d.Name DepartmentName,
			o.Code,
			o.Name,
			o.Labor,
			p.Count,
			null
			from CRCards c
			join CRCardPurchasedProducts p on p.CardId = c.Id
			join CRCardPurchasedProductOperations o on o.CardPurchasedProductId = p.Id
			join Departments d on d.Number = o.Department
			where c.Id in
			(
				select Id from cte
			)
		) r
) r
group by Name
order by Name", new { CardId = id }).ToList();
            }

            string path = Path.Combine(_environment.ContentRootPath, "Reports\\OperationsReport.rdlc");

            using TextReader stream = System.IO.File.OpenText(path);
            using var report = new LocalReport();
            report.LoadReportDefinition(stream);

            report.DataSources.Add(new ReportDataSource("Card", card));
            report.DataSources.Add(new ReportDataSource("Items", items));

            var file = GetFile(report, format);

            if (file == null)
                return BadRequest();

            return file;
        }

        public IActionResult GetMaterialsReport(int id, string type, string format)
        {
            using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnectionString"));
            connection.Open();

            List<ReportMaterialViewModel> items = new();

            if (type == "extracted")
            {
                items = connection.Query<ReportMaterialViewModel>(
@"with cte
as
(
	select * from CRCards where Id = @CardId
	union all
	select c.* from cte
	join CRCards c on c.ParentId = cte.Id
)


select
c.Number CardNumber,
c.Direction,
c.Cipher,
cm.Department,
cm.Code,
cm.Name,
cm.Count,
cm.Price,
u.Name ArrivalUnitName,
u2.Name CurrentUnitName
from CRCards c
join CRCardMaterials cm on cm.CardId = c.Id
left join tUnit u on u.UnitId = cm.UnitId
left join tMaterial m on m.Code = cm.Code
left join tUnit u2 on u2.UnitId = m.UnitId
where c.Id in
(
	select Id from cte
)",
new { CardId = id }).ToList();
            }

            if (type == "installed")
            {
                items = connection.Query<ReportMaterialViewModel>(
@"with cte
as
(
	select * from CRCards where Id = @CardId
	union all
	select c.* from cte
	join CRCards c on c.ParentId2 = cte.Id
)

select
c.Number CardNumber,
c.Direction,
c.Cipher,
cm.Department,
cm.Code,
cm.Name,
cm.Count,
cm.Price,
u.Name ArrivalUnitName,
u2.Name CurrentUnitName
from CRCards c
join CRCardMaterials cm on cm.CardId = c.Id
left join tUnit u on u.UnitId = cm.UnitId
left join tMaterial m on m.Code = cm.Code
left join tUnit u2 on u2.UnitId = m.UnitId
where c.Id in
(
	select Id from cte
)",
new { CardId = id }).ToList();
            }

            string path = Path.Combine(_environment.ContentRootPath, "Reports\\MaterialsReport.rdlc");

            using TextReader stream = System.IO.File.OpenText(path);
            using var report = new LocalReport();
            report.LoadReportDefinition(stream);

            report.DataSources.Add(new ReportDataSource("Items", items));

            var file = GetFile(report, format);

            if (file == null)
                return BadRequest();

            return file;
        }

        public IActionResult GetProductsReport(int id, string type, string format)
        {
            using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnectionString"));
            connection.Open();

            List<ReportOwnProductViewModel> ownProducts = new();
            List<ReportPurchasedProductViewModel> purchasedProducts = new();

            if (type == "extracted")
            {
               ownProducts = connection.Query<ReportOwnProductViewModel>(
@"with cte
as
(
	select * from CRCards where Id = @CardId
	union all
	select c.* from cte
	join CRCards c on c.ParentId = cte.Id
)

select
c.Number CardNumber,
c.Direction,
c.Cipher,
p.Code, 
p.Name, 
p.Count
from CRCards c
join CRCardOwnProducts p on p.CardId = c.Id
where c.Id in
(
	select Id from cte
)",
new { CardId = id }).ToList();

                purchasedProducts = connection.Query<ReportPurchasedProductViewModel>(
@"with cte
as
(
	select * from CRCards where Id = @CardId
	union all
	select c.* from cte
	join CRCards c on c.ParentId = cte.Id
)


select
c.Number CardNumber,
c.Direction,
c.Cipher,
p.Code,
p.Name,
p.Count
from CRCards c
join CRCardPurchasedProducts p on p.CardId = c.Id
where c.Id in
(
	select Id from cte
)",
new { CardId = id }).ToList();

            }

            if (type == "installed")
            {
                ownProducts = connection.Query<ReportOwnProductViewModel>(
@"with cte
as
(
	select * from CRCards where Id = @CardId
	union all
	select c.* from cte
	join CRCards c on c.ParentId2 = cte.Id
)

select
c.Number CardNumber,
c.Direction,
c.Cipher,
p.Code, 
p.Name, 
p.Count
from CRCards c
join CRCardOwnProducts p on p.CardId = c.Id
where c.Id in
(
	select Id from cte
)",
new { CardId = id }).ToList();

                purchasedProducts = connection.Query<ReportPurchasedProductViewModel>(
@"with cte
as
(
	select * from CRCards where Id = @CardId
	union all
	select c.* from cte
	join CRCards c on c.ParentId2 = cte.Id
)

select
c.Number CardNumber,
c.Direction,
c.Cipher,
p.Code,
p.Name,
p.Count
from CRCards c
join CRCardPurchasedProducts p on p.CardId = c.Id
where c.Id in
(
	select Id from cte
)",
new { CardId = id }).ToList();
            }

            string path = Path.Combine(_environment.ContentRootPath, "Reports\\ProductsReport.rdlc");

            using TextReader stream = System.IO.File.OpenText(path);
            using var report = new LocalReport();
            report.LoadReportDefinition(stream);

            report.DataSources.Add(new ReportDataSource("OwnProducts", ownProducts));
            report.DataSources.Add(new ReportDataSource("PurchasedProducts", purchasedProducts));

            var file = GetFile(report, format);

            if (file == null)
                return BadRequest();

            return file;
        }

        public IActionResult GetConsolidatedStatementReport(
            int id, 
            string type, 
            string coefficientId,
            decimal militaryPrice,
            decimal civilianPrice,
            string format)
        {
            using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnectionString"));
            connection.Open();

            List<ConsolidatedStatementReportViewModel> items = new();

            if (type == "extracted")
            {
                items = connection.Query<ConsolidatedStatementReportViewModel>(
@"with cte
as
(
	select * from CRCards where Id = @CardId
	union all
	select c.* from cte
	join CRCards c on c.ParentId = cte.Id
)

select *,
cast(Labor * (case when SpecificationType = 1 then @MilitaryCHTS else @CivilianCHTS end) as decimal(18, 3)) Cost,
cast(LaborWithCoefficient * (case when SpecificationType = 1 then @MilitaryCHTS else @CivilianCHTS end) as decimal(18, 3)) CostWithCoefficient
from
(
	select 
	CardNumber,
	SpecificationType,
	ProductCode,
	ProductName,
	Direction,
	[Order],
	FactoryNumber,
	Department,
	DepartmentName,
	sum(cast(Labor * Count as decimal(18, 3))) Labor,
	cast(sum(cast(Labor * Count as decimal(18, 3))) / 100 * Coefficient as decimal(18, 3)) LaborWithCoefficient
	from
	(
		select 
		*,
		(case Department
		when 4 then c.Department4 
		when 5 then c.Department5
		when 6 then c.Department6 
		when 13 then c.Department13 
		when 17 then c.Department17 
		when 80 then c.Department80 
		when 82 then c.Department82 
		when 90 then c.Department90 
		end) Coefficient
		from
		(
			select 
			c.Number CardNumber,
			c.ProductCode,
			c.ProductName,
			c.[Order],
			(select SpecificationType from ref_TypeOfContract toc where toc.Name = c.Direction) SpecificationType,
			coalesce(Cipher, Direction) Direction,
			c.FactoryNumber,
			co.Department,
			d.Name DepartmentName,
			co.Code,
			co.Name,
			co.Labor,
			co.Count
			from CRCards c
			join CRCardOperations co on co.CardId = c.Id
			join Departments d on d.Number = co.Department
			where c.Id in
			(
				select Id from cte
			)
			and co.Type = 1 -- фактические

			union all

			select 
			c.Number CardNumber,
			c.ProductCode,
			c.ProductName,
			c.[Order],
			(select SpecificationType from ref_TypeOfContract toc where toc.Name = c.Direction) SpecificationType,
			coalesce(Cipher, Direction) Direction,
			c.FactoryNumber,
			o.Department,
			d.Name DepartmentName,
			o.Code,
			o.Name,
			o.Labor,
			p.Count
			from CRCards c
			join CRCardOwnProducts p on p.CardId = c.Id
			join CRCardOwnProductOperations o on o.CardOwnProductId = p.Id
			join Departments d on d.Number = o.Department
			where c.Id in
			(
				select Id from cte
			)

			union all

			select 
			c.Number CardNumber,
			c.ProductCode,
			c.ProductName,
			c.[Order],
			(select SpecificationType from ref_TypeOfContract toc where toc.Name = c.Direction) SpecificationType,
			coalesce(Cipher, Direction) Direction,
			c.FactoryNumber,
			o.Department,
			d.Name DepartmentName,
			o.Code,
			o.Name,
			o.Labor,
			p.Count * o.Count
			from CRCards c
			join CRCardOwnProducts p on p.CardId = c.Id
			join CRCardOwnProductRepairOperations o on o.CardOwnProductId = p.Id
			join Departments d on d.Number = o.Department
			where c.Id in
			(
				select Id from cte
			)

			union all

			select 
			c.Number CardNumber,
			c.ProductCode,
			c.ProductName,
			c.[Order],
			(select SpecificationType from ref_TypeOfContract toc where toc.Name = c.Direction) SpecificationType,
			coalesce(Cipher, Direction) Direction,
			c.FactoryNumber,
			o.Department,
			d.Name DepartmentName,
			o.Code,
			o.Name,
			o.Labor,
			p.Count
			from CRCards c
			join CRCardPurchasedProducts p on p.CardId = c.Id
			join CRCardPurchasedProductOperations o on o.CardPurchasedProductId = p.Id
			join Departments d on d.Number = o.Department
			where c.Id in
			(
				select Id from cte
			)
		) r
		cross join SalariedEmployeeLaborCoefficients c where c.Id = @CoefficientId
	) r
	group by CardNumber, SpecificationType, ProductCode, ProductName, Direction, [Order], FactoryNumber, Department, DepartmentName, Coefficient
) r",
new
{
    CardId = id,
    MilitaryCHTS = militaryPrice,
    CivilianCHTS = civilianPrice,
    CoefficientId = coefficientId
}).ToList();
            }

            if (type == "installed")
            {
                items = connection.Query<ConsolidatedStatementReportViewModel>(
@"with cte
as
(
	select * from CRCards where Id = @CardId
	union all
	select c.* from cte
	join CRCards c on c.ParentId2 = cte.Id
)

select *,
cast(Labor * (case when SpecificationType = 1 then @MilitaryCHTS else @CivilianCHTS end) as decimal(18, 3)) Cost,
cast(LaborWithCoefficient * (case when SpecificationType = 1 then @MilitaryCHTS else @CivilianCHTS end) as decimal(18, 3)) CostWithCoefficient
from
(
	select 
	CardNumber,
	SpecificationType,
	ProductCode,
	ProductName,
	Direction,
	[Order],
	FactoryNumber,
	Department,
	DepartmentName,
	sum(cast(Labor * Count as decimal(18, 3))) Labor,
	cast(sum(cast(Labor * Count as decimal(18, 3))) / 100 * Coefficient as decimal(18, 3)) LaborWithCoefficient
	from
	(
		select 
		*,
		(case Department
		when 4 then c.Department4 
		when 5 then c.Department5
		when 6 then c.Department6 
		when 13 then c.Department13 
		when 17 then c.Department17 
		when 80 then c.Department80 
		when 82 then c.Department82 
		when 90 then c.Department90 
		end) Coefficient
		from
		(
			select 
			c.Number CardNumber,
			c.ProductCode,
			c.ProductName,
			c.[Order],
			(select SpecificationType from ref_TypeOfContract toc where toc.Name = c.Direction) SpecificationType,
			coalesce(Cipher, Direction) Direction,
			c.FactoryNumber,
			co.Department,
			d.Name DepartmentName,
			co.Code,
			co.Name,
			co.Labor,
			co.Count
			from CRCards c
			join CRCardOperations co on co.CardId = c.Id
			join Departments d on d.Number = co.Department
			where c.Id in
			(
				select Id from cte
			)
			and co.Type = 1 -- фактические

			union all

			select 
			c.Number CardNumber,
			c.ProductCode,
			c.ProductName,
			c.[Order],
			(select SpecificationType from ref_TypeOfContract toc where toc.Name = c.Direction) SpecificationType,
			coalesce(Cipher, Direction) Direction,
			c.FactoryNumber,
			o.Department,
			d.Name DepartmentName,
			o.Code,
			o.Name,
			o.Labor,
			p.Count
			from CRCards c
			join CRCardOwnProducts p on p.CardId = c.Id
			join CRCardOwnProductOperations o on o.CardOwnProductId = p.Id
			join Departments d on d.Number = o.Department
			where c.Id in
			(
				select Id from cte
			)

			union all

			select 
			c.Number CardNumber,
			c.ProductCode,
			c.ProductName,
			c.[Order],
			(select SpecificationType from ref_TypeOfContract toc where toc.Name = c.Direction) SpecificationType,
			coalesce(Cipher, Direction) Direction,
			c.FactoryNumber,
			o.Department,
			d.Name DepartmentName,
			o.Code,
			o.Name,
			o.Labor,
			p.Count * o.Count
			from CRCards c
			join CRCardOwnProducts p on p.CardId = c.Id
			join CRCardOwnProductRepairOperations o on o.CardOwnProductId = p.Id
			join Departments d on d.Number = o.Department
			where c.Id in
			(
				select Id from cte
			)

			union all

			select 
			c.Number CardNumber,
			c.ProductCode,
			c.ProductName,
			c.[Order],
			(select SpecificationType from ref_TypeOfContract toc where toc.Name = c.Direction) SpecificationType,
			coalesce(Cipher, Direction) Direction,
			c.FactoryNumber,
			o.Department,
			d.Name DepartmentName,
			o.Code,
			o.Name,
			o.Labor,
			p.Count
			from CRCards c
			join CRCardPurchasedProducts p on p.CardId = c.Id
			join CRCardPurchasedProductOperations o on o.CardPurchasedProductId = p.Id
			join Departments d on d.Number = o.Department
			where c.Id in
			(
				select Id from cte
			)
		) r
		cross join SalariedEmployeeLaborCoefficients c where c.Id = @CoefficientId
	) r
	group by CardNumber, SpecificationType, ProductCode, ProductName, Direction, [Order], FactoryNumber, Department, DepartmentName, Coefficient
) r",
new
{
    CardId = id,
    MilitaryCHTS = militaryPrice,
    CivilianCHTS = civilianPrice,
    CoefficientId = coefficientId
}).ToList();
            }

            string path = Path.Combine(_environment.ContentRootPath, "Reports\\ConsolidatedStatementReport.rdlc");

            using TextReader stream = System.IO.File.OpenText(path);
            using var report = new LocalReport();
            report.LoadReportDefinition(stream);

            report.DataSources.Add(new ReportDataSource("Items", items));

            var file = GetFile(report, format);

            if (file == null)
                return BadRequest();

            return file;
        }

		public IActionResult GetActReport(
			int id,
            string coefficientId,
            decimal militaryPrice,
            decimal civilianPrice,
            string format)
		{
            using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnectionString"));
            connection.Open();

            var card = connection.Query<ReportCardViewModel>(
@"select c.*, 
(select Number from CRCards p where p.Id = c.ParentId) ParentNumber,
(select ProductCode from CRCards p where p.Id = c.ParentId) ParentProductCode,
(select SpecificationType from ref_TypeOfContract toc where toc.Name = c.Direction) SpecificationType,
t.Name RepairType 
from CRCards c
left join CRRepairTypes t on c.RepairTypeId = t.Id
where c.Id = @Id", new { Id = id });

			var purchasedProducts = connection.Query<CardPurchasedProduct>(
"select * from CRCardPurchasedProducts where CardId = @CardId",
new { CardId = id });

			var materials = connection.Query<CardMaterial>(
@"select m.Id, m.Code, m.Name, m.Size, m.Type, m.UnitId, m.Count, m.Price, m.CardId, m.RowVersion, m.Department from CRCardMaterials m 
where m.CardId = @CardId",
new { CardId = id });

			var operations = connection.Query<ReportOperation2ViewModel>(
@"
select Department, Code, Name, sum(Labor) Labor, cast(sum(Labor / 100 * Coefficient) as decimal(18, 3)) LaborWithCoefficient from
(
	select Department, Code, Name, Labor,
	(case Department
	when 4 then c.Department4 
	when 5 then c.Department5
	when 6 then c.Department6 
	when 13 then c.Department13 
	when 17 then c.Department17 
	when 80 then c.Department80 
	when 82 then c.Department82 
	when 90 then c.Department90 
	end) Coefficient
	from
	(
		select o.Department, o.Code, o.Name, o.Labor * p.Count Labor 
		from CRCards c 
		join CRCardOwnProducts p on p.CardId = c.Id
		join CRCardOwnProductOperations o on o.CardOwnProductId = p.Id
		left join CRExecutors e on e.Id = o.ExecutorId
		where c.Id = @CardId

		union all

		select o.Department, o.Code, o.Name, o.Labor * p.Count Labor
		from CRCards c 
		join CRCardPurchasedProducts p on p.CardId = c.Id
		join CRCardPurchasedProductOperations o on o.CardPurchasedProductId = p.Id
		left join CRExecutors e on e.Id = o.ExecutorId
		where c.Id = @CardId

		union all

		select o.Department, o.Code, o.Name, o.Labor * o.Count * p.Count Labor
		from CRCards c 
		join CRCardOwnProducts p on p.CardId = c.Id
		join CRCardOwnProductRepairOperations o on o.CardOwnProductId = p.Id
		left join CRExecutors e on e.Id = o.ExecutorId
		where c.Id = @CardId

		union all

		select o.Department, null Code, o.GroupName Name, o.Labor * o.Count from CRCardOperations o
		left join CRExecutors e on e.Id = o.ExecutorId
		where o.CardId = @CardId and o.[Type] = 1
	) r
	cross join SalariedEmployeeLaborCoefficients c where c.Id = @CoefficientId
) r
group by Department, Code, Name",
new { CardId = id, CoefficientId = coefficientId });

            var parameters = new ReportParameterCollection
            {
                new ReportParameter("CivilianCHTS", civilianPrice.ToString()),
                new ReportParameter("MilitaryCHTS", militaryPrice.ToString()),
                new ReportParameter("SpecificationType", card.FirstOrDefault().SpecificationType?.ToString()),
            };

            string path = Path.Combine(_environment.ContentRootPath, "Reports\\ActReport.rdlc");

            using TextReader stream = System.IO.File.OpenText(path);
            using var report = new LocalReport();
            report.LoadReportDefinition(stream);

            report.DataSources.Add(new ReportDataSource("Card", card));
            report.DataSources.Add(new ReportDataSource("Products", purchasedProducts));
            report.DataSources.Add(new ReportDataSource("Materials", materials));
            report.DataSources.Add(new ReportDataSource("Operations", operations));
			report.SetParameters(parameters);

            var file = GetFile(report, format);

            if (file == null)
                return BadRequest();

            return file;
        }

        public IActionResult GetUnconfirmedCards(int id, string type)
        {
            using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnectionString"));
            connection.Open();

            List<Card> items = new();

            if (type == "extracted")
            {
                items = connection.Query<Card>(
@"with cte
as
(
	select * from CRCards where Id = @CardId
	union all
	select c.* from cte
	join CRCards c on c.ParentId = cte.Id
)
select * from
(
	select *,
	(case when (select count(*) from CRCardConfirmations a where a.CardId = cte.Id and a.UserRoleId = 6 and a.CardConfirmationObjectId = 1) > 0 then 1 else 0 end) IsConfirmed from cte
) r
where IsConfirmed = 0",
new { CardId = id }).ToList();
            }

            if (type == "installed")
            {
                items = connection.Query<Card>(
@"with cte
as
(
	select * from CRCards where Id = @CardId
	union all
	select c.* from cte
	join CRCards c on c.ParentId2 = cte.Id
)

select * from
(
	select *,
	(case when (select count(*) from CRCardConfirmations a where a.CardId = cte.Id and a.UserRoleId = 6 and a.CardConfirmationObjectId = 1) > 0 then 1 else 0 end) IsConfirmed from cte
) r
where IsConfirmed = 0",
new { CardId = id }).ToList();
            }

            string list = items.Select(x => x.Number).Aggregate((a, b) => a + ", " + b);

            return Ok("The following cards are not confirmed: " + list);
        }

        List<Card> GetAllChildrenCards(Card card)
        {
            var result = new List<Card>();

            void Build(List<Card> items)
            {
                foreach (var item in items)
                {
                    result.Add(item);
                    var children = _db.Cards.Where(x => x.ParentId2 == item.Id).ToList();
                    Build(children);
                }
            }

            Build(new List<Card> { card });

            return result;
        }

        private FileContentResult? GetFile(LocalReport report, string format)
        {
            return format switch
            {
                "pdf" => File(report.Render("PDF"), "application/pdf"),
                "docx" => File(report.Render("WORDOPENXML"), "application/vnd.openxmlformats-officedocument.wordprocessingml.document"),
                "xlsx" => File(report.Render("EXCELOPENXML"), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"),
                _ => null
            };
        }
    }
}
