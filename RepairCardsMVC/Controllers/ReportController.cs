using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Reporting.NETCore;
using RepairCardsMVC.Models;
using RepairCardsMVC.ViewModels;
using System.Text.Json;

namespace RepairCardsMVC.Controllers
{
    public class ReportController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly IWebHostEnvironment _environment;
        private readonly IConfiguration _configuration;

        public ReportController(
            ApplicationDbContext db,
            IWebHostEnvironment environment,
            IConfiguration configuration)
        {
            _db = db;
            _environment = environment;
            _configuration = configuration;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult GetPostWarrantyRepairProtacolReport(string cards, string format)
        {
            var numbers = JsonSerializer.Deserialize<string[]>(cards);

            using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnectionString"));
            connection.Open();

            var items = connection.Query<PostWarrantyRepairReportViewModel>(
@"declare @tmp table ( ParentId int, ProductCode varchar(max), ParentCardProductCode varchar(max), FactoryNumber varchar(max), ParentFactoryNumber varchar(max), ExternalDefects varchar(max),
InternalDefects varchar(max), Malfunctions varchar(max), CauseOfProductFailure varchar(max), ScopeOfRepair varchar(max), CommisionReport varchar(max), ProductName varchar(max), ParentCardProductName varchar(max), 
MainProductId int )
insert into @tmp
select r.*, d1.Name ProductName, d2.Name ParentCardProductName, null from 
(
	select 
	c.ParentId,
	case when patindex('%-__.%/%.%', c.ProductCode) > 0 then substring(c.ProductCode, 1, patindex('%-__.%/%.%', c.ProductCode) + 2) else c.ProductCode end ProductCode,
	case when patindex('%-__.%/%.%', pc.ProductCode) > 0 then substring(pc.ProductCode, 1, patindex('%-__.%/%.%', pc.ProductCode) + 2) else pc.ProductCode end ParentCardProductCode,
	c.FactoryNumber,
	pc.FactoryNumber as ParentFactoryNumber,
	d.ExternalDefects, d.InternalDefects, d.Malfunctions, d.CauseOfProductFailure,
	d.ScopeOfRepair, d.CommissionReport
	from CRCards c
	join CRCardDetails d on d.Id = c.Id
	left join CRCards pc on pc.Id = c.ParentId
	where c.Number in @Numbers
) r
left join ref_dse d1 on d1.DecNum = r.ProductCode
left join ref_dse d2 on d2.DecNum = r.ParentCardProductCode

-- Находим первое изделие в дереве
declare csr cursor for
select ParentId from @tmp

declare @ParentId int
open csr

fetch next from csr into @ParentId

while @@FETCH_STATUS = 0 
begin
	
	with cte
	as
	(
		select * from CRCards where Id = @ParentId
		union all
		select c.* from cte
		join CRCards c on c.Id = cte.ParentId
	)
	update @tmp set MainProductId =
	(select Id from cte where ParentId is null)
	where ParentId = @ParentId

	fetch next from csr into @ParentId
end

close csr
deallocate csr

select r.*, d.Name MainProductName from 
(
	select t.*, 
	case when patindex('%-__.%/%.%', c.ProductCode) > 0 then substring(c.ProductCode, 1, patindex('%-__.%/%.%', c.ProductCode) + 2) else c.ProductCode end MainProductCode, 
	c.FactoryNumber MainProductFactoryNumber from @tmp t
	join CRCards c on c.Id = t.MainProductId 
) r
left join ref_dse d on d.DecNum = r.MainProductCode", new { Numbers = numbers }).ToList();

            string path = Path.Combine(_environment.ContentRootPath, "Reports\\PostWarrantyRepairProtocolFormReport.rdlc");

            using TextReader stream = System.IO.File.OpenText(path);
            using var report = new LocalReport();
            report.LoadReportDefinition(stream);

            report.DataSources.Add(new ReportDataSource("Items", items));

            var file = GetFile(report, format);

            if (file == null)
                return BadRequest();

            return file;
        }

        public IActionResult GetProductsMaterialsAndOperationsReport(string cards, string format)
        {
            var numbers = JsonSerializer.Deserialize<string[]>(cards);

            using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnectionString"));
            connection.Open();

            var ownProducts = connection.Query<CardMaterial, Card, CardMaterial>(
@"select m.Id, m.Code, m.Name, m.Size, m.Type, m.UnitId, m.Count, m.Price, m.CardId, m.RowVersion, m.Department, c.* from CRCardMaterials m 
            left join CRCards c on c.Id = m.CardId
            where c.Number in @Numbers
            order by c.Number",
(m, c) => { m.Card = c; return m; },
new { Numbers = numbers });

            var operations = connection.Query<CardOperation, Executor, Card, CardOperation>(
@"select * from CRCardOperations o
left join CRExecutors e on e.Id = o.ExecutorId
left join CRCards c on c.Id = o.CardId
where c.Number in @Numbers
order by c.Number",
(o, e, c) => { o.Executor = e; o.Card = c; return o; },
new { Numbers = numbers });

            var purchasedProducts = connection.Query<CardOwnProduct, Card, CardOwnProduct>(
@"select * from CRCardOwnProducts p 
join CRCards c on c.Id = p.CardId
where c.Number in @Numbers
order by c.Number",
(p, c) => { p.Card = c; return p; },
new { Numbers = numbers });

            var materials = connection.Query<CardPurchasedProduct, Card, CardPurchasedProduct>(
@"select * from CRCardPurchasedProducts p 
join CRCards c on c.Id = p.CardId
where c.Number in @Numbers
order by c.Number",
(p, c) => { p.Card = c; return p; },
new { Numbers = numbers });

            string path = Path.Combine(_environment.ContentRootPath, "Reports\\ProductsMaterialsOperationsReport.rdlc");

            using TextReader stream = System.IO.File.OpenText(path);
            using var report = new LocalReport();
            report.LoadReportDefinition(stream);

            report.DataSources.Add(new ReportDataSource("OwnProducts", ownProducts));
            report.DataSources.Add(new ReportDataSource("PurchasedProducts", purchasedProducts));
            report.DataSources.Add(new ReportDataSource("Materials", materials));
            report.DataSources.Add(new ReportDataSource("Operations", operations));

            var file = GetFile(report, format);

            if (file == null)
                return BadRequest();

            return file;
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

        [HttpPost]
        public IActionResult GetCards()
        {
            return Ok(Request.GetDataForJQueryDataTable(_db.Cards.AsQueryable(), "Number", "ProductName"));
        }
    }
}
