using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using RepairCardsMVC.Models;

namespace RepairCardsMVC
{
    public class ApplicationDbContext : IdentityDbContext<User>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Executor>()
                .ToTable("CRExecutors", t => t.ExcludeFromMigrations())
                .ToTable(x => x.HasTrigger("CRExecutorsDeleteTrigger"))
                .ToTable(x => x.HasTrigger("CRExecutorsInsertTrigger"))
                .ToTable(x => x.HasTrigger("CRExecutorsUpdateTrigger"));

            modelBuilder.Entity<Operation>()
                .ToTable("CROperations", t => t.ExcludeFromMigrations())
                .ToTable(x => x.HasTrigger("CROperationsDeleteTrigger"))
                .ToTable(x => x.HasTrigger("CROperationsInsertTrigger"))
                .ToTable(x => x.HasTrigger("CROperationsUpdateTrigger"));

            modelBuilder.Entity<ProductionOperation>()
                .ToTable(nameof(ProductionOperation), t => t.ExcludeFromMigrations())
                .HasKey(x => x.Id);

            modelBuilder.Entity<ProductionOperation>()
                .HasMany(x => x.Operations)
                .WithOne(x => x.ProductionOperation)
                .HasForeignKey(x => x.ProductionOperationCode)
                .HasPrincipalKey(x => x.Code);

            modelBuilder.Entity<RepairType>()
                .ToTable("CRRepairTypes", t => t.ExcludeFromMigrations());

            modelBuilder.Entity<CardStatus>()
                .ToTable("CRCardStatuses", t => t.ExcludeFromMigrations());

            modelBuilder.Entity<Card>()
                .ToTable("CRCards", t => t.ExcludeFromMigrations())
                .ToTable(x => x.HasTrigger("CRCardsDeleteTrigger"))
                .ToTable(x => x.HasTrigger("CRCardsInsertTrigger"))
                .ToTable(x => x.HasTrigger("CRCardsUpdateTrigger"));

            modelBuilder.Entity<Card>()
                .HasOne(x => x.Parent)
                .WithOne()
                .HasForeignKey<Card>(x => x.ParentId);

            modelBuilder.Entity<Card>()
                .HasOne(x => x.Parent2)
                .WithOne()
                .HasForeignKey<Card>(x => x.ParentId2);

            modelBuilder.Entity<Product>()
                .ToTable("ref_dse", t => t.ExcludeFromMigrations());

            modelBuilder.Entity<PurchasedProduct>()
                .ToTable("ref_purchase", t => t.ExcludeFromMigrations());

            modelBuilder.Entity<CardOperation>()
                .ToTable("CRCardOperations", t => t.ExcludeFromMigrations())
                .ToTable(x => x.HasTrigger("CRCardOperationsDeleteTrigger"))
                .ToTable(x => x.HasTrigger("CRCardOperationsInsertTrigger"))
                .ToTable(x => x.HasTrigger("CRCardOperationsUpdateTrigger"));

            modelBuilder.Entity<ExportRequest>()
                .ToTable("CRExportRequests", t => t.ExcludeFromMigrations())
                .ToTable(x => x.HasTrigger("CRCardExportRequestsDeleteTrigger"))
                .ToTable(x => x.HasTrigger("CRCardExportRequestsInsertTrigger"))
                .ToTable(x => x.HasTrigger("CRCardExportRequestsUpdateTrigger"));

            modelBuilder.Entity<RequestToCreateStagesIn1S>()
                .ToTable("CRRequestsToCreateStagesIn1S", t => t.ExcludeFromMigrations())
                .ToTable(x => x.HasTrigger("CRRequestsToCreateStagesIn1SDeleteTrigger"))
                .ToTable(x => x.HasTrigger("CRRequestsToCreateStagesIn1SInsertTrigger"))
                .ToTable(x => x.HasTrigger("CRRequestsToCreateStagesIn1SUpdateTrigger"));

            modelBuilder.Entity<Request>()
                .ToTable("CRRequests", t => t.ExcludeFromMigrations())
                .ToTable(x => x.HasTrigger("CRRequestsInsertTrigger"))
                .ToTable(x => x.HasTrigger("CRRequestsUpdateTrigger"));

            modelBuilder.Entity<CardDetails>()
                .ToTable("CRCardDetails", t => t.ExcludeFromMigrations())
                .ToTable(x => x.HasTrigger("CRCardDetailsDeleteTrigger"))
                .ToTable(x => x.HasTrigger("CRCardDetailsInsertTrigger"))
                .ToTable(x => x.HasTrigger("CRCardDetailsUpdateTrigger"));

            modelBuilder.Entity<CardMaterial>()
                .ToTable("CRCardMaterials", t => t.ExcludeFromMigrations())
                .ToTable(x => x.HasTrigger("CRCardMaterialsDeleteTrigger"))
                .ToTable(x => x.HasTrigger("CRCardMaterialsInsertTrigger"))
                .ToTable(x => x.HasTrigger("CRCardMaterialsUpdateTrigger"))

                // DO NOT DELETE IT
                .Property(x => x.Count).HasColumnType("decimal(18, 3)");

            modelBuilder.Entity<CardOwnProduct>()
                .ToTable("CRCardOwnProducts", t => t.ExcludeFromMigrations())
                .ToTable(x => x.HasTrigger("CardOwnProductsDeleteTrigger"))
                .ToTable(x => x.HasTrigger("CardOwnProductsInsertTrigger"))
                .ToTable(x => x.HasTrigger("CardOwnProductsUpdateTrigger"));

            modelBuilder.Entity<CardPurchasedProduct>()
                .ToTable("CRCardPurchasedProducts", t => t.ExcludeFromMigrations())
                .ToTable(x => x.HasTrigger("CardPurchasedProductsDeleteTrigger"))
                .ToTable(x => x.HasTrigger("CardPurchasedProductsInsertTrigger"))
                .ToTable(x => x.HasTrigger("CardPurchasedProductsUpdateTrigger"));

            modelBuilder.Entity<CardDocument>()
                .ToTable("CRCardDocuments", t => t.ExcludeFromMigrations())
                .ToTable(x => x.HasTrigger("CardDocumentsDeleteTrigger"))
                .ToTable(x => x.HasTrigger("CardDocumentsInsertTrigger"))
                .ToTable(x => x.HasTrigger("CardDocumentsUpdateTrigger"));
            
            modelBuilder.Entity<UnlockedPeriod>()
                .ToTable("CRUnlockedPeriods", t => t.ExcludeFromMigrations());

            modelBuilder.Entity<Template>()
                .ToTable("CRTemplates", t => t.ExcludeFromMigrations())
                .ToTable(x => x.HasTrigger("CardTemplatesDeleteTrigger"))
                .ToTable(x => x.HasTrigger("CardTemplatesInsertTrigger"))
                .ToTable(x => x.HasTrigger("CardTemplatesUpdateTrigger"));

            modelBuilder.Entity<TemplateOperation>()
                .ToTable("CRTemplateOperations", t => t.ExcludeFromMigrations())
                .ToTable(x => x.HasTrigger("TemplateOperationsDeleteTrigger"))
                .ToTable(x => x.HasTrigger("TemplateOperationsInsertTrigger"))
                .ToTable(x => x.HasTrigger("TemplateOperationsUpdateTrigger"));

            modelBuilder.Entity<Material>()
                .ToTable("tMaterial", t => t.ExcludeFromMigrations())
                .ToTable(x => x.HasTrigger("tr_tmaterial_d"))
                .ToTable(x => x.HasTrigger("tr_tmaterial_i"))
                .ToTable(x => x.HasTrigger("tr_tmaterial_u"));

            modelBuilder.Entity<CardOwnProductOperation>()
                .ToTable("CRCardOwnProductOperations", t => t.ExcludeFromMigrations())
                .ToTable(x => x.HasTrigger("CardOwnProductOperationsDeleteTrigger"))
                .ToTable(x => x.HasTrigger("CardOwnProductOperationsInsertTrigger"))
                .ToTable(x => x.HasTrigger("CardOwnProductOperationsUpdateTrigger"));

            modelBuilder.Entity<CardOwnProductRepairOperation>()
                .ToTable("CRCardOwnProductRepairOperations", t => t.ExcludeFromMigrations())
                .ToTable(x => x.HasTrigger("CardOwnProductRepairOperationsDeleteTrigger"))
                .ToTable(x => x.HasTrigger("CardOwnProductRepairOperationsInsertTrigger"))
                .ToTable(x => x.HasTrigger("CardOwnProductRepairOperationsUpdateTrigger"));
            
            modelBuilder.Entity<CardPurchasedProductOperation>()
                .ToTable("CRCardPurchasedProductOperations", t => t.ExcludeFromMigrations())
                .ToTable(x => x.HasTrigger("CRCardPurchasedProductOperationsDeleteTrigger"))
                .ToTable(x => x.HasTrigger("CRCardPurchasedProductOperationsInsertTrigger"))
                .ToTable(x => x.HasTrigger("CRCardPurchasedProductOperationsUpdateTrigger"))
                .Property(x => x.Labor).HasColumnType("decimal(18, 4)");

            modelBuilder.Entity<CardConfirmation>()
                .ToTable("CRCardConfirmations", t => t.ExcludeFromMigrations());

            modelBuilder.Entity<CardConfirmationObject>()
                .ToTable("CRCardConfirmationObjects", t => t.ExcludeFromMigrations());

            modelBuilder.Entity<Role>()
                .ToTable("CRRoles", t => t.ExcludeFromMigrations());

            modelBuilder.Entity<ProductionOperation>()
                .ToTable("ref_top_2", t => t.ExcludeFromMigrations());

            modelBuilder.Entity<SalariedEmployeeLaborCoefficient>()
                .ToTable("SalariedEmployeeLaborCoefficients", t => t.ExcludeFromMigrations());

            modelBuilder.Entity<Relation>()
                .ToTable(nameof(Relation), t => t.ExcludeFromMigrations());

            modelBuilder.Entity<ProductMaterial>()
                .ToTable(nameof(ProductMaterial), t => t.ExcludeFromMigrations());

            modelBuilder.Entity<ProductOperation>()
                .ToTable(nameof(ProductOperation), t => t.ExcludeFromMigrations());

            modelBuilder.Entity<User>()
                .Property(x => x.Name)
                .HasMaxLength(250);
        }

        public DbSet<Executor> Executors { get; set; }
        public DbSet<Operation> Operations { get; set; }
        public DbSet<RepairType> RepairTypes { get; set; }
        public DbSet<CardStatus> CardStatuses { get; set; }
        public DbSet<Card> Cards { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<PurchasedProduct> PurchasedProducts { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<CardOperation> CardOperations { get; set; }
        public DbSet<ExportRequest> ExportRequests { get; set; }
        public DbSet<RequestToCreateStagesIn1S> RequestsToCreateStagesIn1S { get; set; }
        public DbSet<Request> Requests { get; set; }
        public DbSet<CardDetails> CardDetails { get; set; }
        public DbSet<CardMaterial> CardMaterials { get; set; }
        public DbSet<CardOwnProduct> CardOwnProducts { get; set; }
        public DbSet<CardPurchasedProduct> CardPurchasedProducts { get; set; }
        public DbSet<CardDocument> CardDocuments { get; set; }
        public DbSet<UnlockedPeriod> UnlockedPeriods { get; set; }
        public DbSet<Template> Templates { get; set; }
        public DbSet<TemplateOperation> TemplateOperations { get; set; }
        public DbSet<Material> Materials { get; set; }
        public DbSet<Relation> Relations { get; set; }
        public DbSet<ProductOperation> ProductOperations { get; set; }
        public DbSet<CardOwnProductOperation> CardOwnProductOperations { get; set; }
        public DbSet<CardOwnProductRepairOperation> CardOwnProductRepairOperations { get; set; }
        public DbSet<CardPurchasedProductOperation> CardPurchasedProductOperations { get; set; }
        public DbSet<SalariedEmployeeLaborCoefficient> SalariedEmployeeLaborCoefficients { get; set; }
        public DbSet<ProductMaterial> ProductMaterials { get; set; }
        public DbSet<CardConfirmation> CardConfirmations { get; set; }
        public DbSet<CardConfirmationObject> CardConfirmationObjects { get; set; }
        public DbSet<ProductionOperation> ProductionOperations { get; set; }

        #region PDM

        public Task<List<Order>> GetOrdersByProductAsync(string productCode)
        {
            return Orders.FromSqlRaw("CROrders @ProductCode = @p0",
                parameters: new[] { productCode })
                .AsNoTracking()
                .ToListAsync();
        }

        public List<Relation> GetProductRelations(string code, bool isPurchased = false)
        {
            string tm = int.Parse(DateTime.Now.ToString("HHmmssfff")).ToString();

            using var tran = Database.BeginTransaction();

            int id = Database.SqlQuery<int>($"select Id from ref_dse where Decnum = {code}").ToList().FirstOrDefault();

            Database.ExecuteSql($"exec c_SelTask @Tm = {tm}");

            Database.ExecuteSql($"exec i_TaskComp @IdDse = {id}, @CountDse = 1, @IdOrder = 0, @Reference = 1, @TableWhat = 2, @Tm = {tm}");
            
            Database.ExecuteSql($"exec i_CompoundTaskDevelopmentWithTypeIn @IdentOpen = 1, @Tm = {tm}");

            var result = Relations.FromSqlRaw($"exec EVPR_sCompoundForDeptsWithTypeIn3 @IdentPurchase = {(isPurchased ? 1 : 0)}, @Tm = {tm}")
                .AsNoTracking().ToList();

            tran.Commit();

            return result;
        }

        public string? GetPurchasedProductRoute(string code)
        {
            string tm = int.Parse(DateTime.Now.ToString("HHmmssfff")).ToString();

            using var tran = Database.BeginTransaction();

            int id = Database.SqlQuery<int>($"select Id from ref_purchase where Decnum = {code}").ToList().FirstOrDefault();

            Database.ExecuteSql($"exec c_SelTask @Tm = {tm}");

            Database.ExecuteSql($"exec i_TaskComp @IdDse = {id}, @CountDse = 1, @IdOrder = 0, @Reference = 1, @TableWhat = 1, @Tm = {tm}");

            Database.ExecuteSql($"exec i_CompoundTaskDevelopmentWithTypeIn @IdentOpen = 1, @Tm = {tm}");

            var result = Relations.FromSqlRaw($"exec EVPR_sCompoundForDeptsWithTypeIn3 @IdentPurchase = 0, @Tm = {tm}")
                .AsNoTracking().ToList();

            tran.Commit();

            string? route = result.Where(x => string.IsNullOrEmpty(x.ParentCode))
                .Select(x => x.Route)
                .FirstOrDefault()
                ?.Replace("  ", " ");

            return route;
        }

        public List<ProductOperation> GetProductOperations(string code, string route)
        {
            var result = ProductOperations
                .FromSqlRaw($"exec s_TechProc____111 @Decnum = '{code}', @Marshryt = '{route}', @Key = 1, @Nol = 0, @CodeZak = '', @Dept = '000'")
               .AsNoTracking().ToList();
            return result;
        }

        public void DeleteCardOwnProductRecursively(int id)
        {
            Database.ExecuteSql(
@$"with cte
as
(
	select * from CRCardOwnProducts where Id = {id}
	union all
	select c.* from cte
	join CRCardOwnProducts c on c.ParentId = cte.Id
)
delete from CRCardOwnProducts where Id in (select Id from cte)");
        }

        public void ExportToPDM(Card card)
        {
            Database.ExecuteSql(
$@"Declare @table table (id int identity,idWhat int,[Type] int,DecNumWhat varchar(max),[Name] varchar(max),[Count] int, TableWhat int) 
if {card.ProductId} != 0 and {card.Id} != 0 and (select count(id) from Connect where IdIn = {card.ProductId})<2 
BEGIN 
	insert into @table 
	select t2.id 'idWhat',t2.Type,t2.DecNum,t2.Name,cast(t1.[Count] as decimal(18,3)),'2' 'TableWhat' from CRCardOwnProducts t1 
	left join ref_Dse t2 on t1.Code = t2.DecNum where t1.CardId = {card.Id} and (isnull(t1.HasChangedComposition, 0) = 0 and t1.ParentId is null) and t2.id is not null 
	union all 
	select t2.id 'idWhat',t2.Type,t2.DecNum,t2.Name,cast(t1.[Count] as decimal(18,3)),'1' 'TableWhat' from CRCardOwnProducts t1 
	left join ref_Purchase t2 on t1.Code = t2.DecNum where t1.CardId = {card.Id} and (isnull(t1.HasChangedComposition, 0) = 0 and t1.ParentId is null) and t2.id is not null 
	union all 
	select t2.id 'idWhat',t2.Type,t2.DecNum,t2.Name,cast(t1.[Count] as decimal(18,3)),'1' 'TableWhat' from CRCardPurchasedProducts t1 
	left join ref_Purchase t2 on t1.Code = t2.DecNum where t1.CardId = {card.Id} and t2.id is not null 
	
	insert into [Connect] 
	select {card.ProductId},0,idWhat,[Type],[Count],null,TableWhat,null,'К/Р '+{card.Number},2,0,null,null from @table 

	declare @LeadingZeroDepartment varchar(max) = isnull(replicate('0', 3 - len({card.Department})), '') + cast({card.Department} as varchar)

	if (select count(*) from ref_routing where What = {card.ProductId} and TableWhat = 2) = 0
	begin
		insert into ref_routing (What, TableWhat, Dept1, [DateTime])
		values ({card.ProductId}, 2, @LeadingZeroDepartment, getdate())
	end
END");
        }

        public bool IsExistingOrderInPDM(string order)
        {
            return Database.SqlQuery<bool>(
$@"Declare @IdDseOrder int = (select top 1 Dse from Orders where Code_+Isp+Ku like {order})
select cast(case when @IdDseOrder > 0 then 1 else 0 end as bit)").ToList().FirstOrDefault();
        }

        public bool IsPlannedOrder(string order)
        {
            return Database.SqlQuery<bool>(
@$"Declare @IdArticle int = (select top 1 ID from Article where DecNum like '%ДЕФИ.'+{order}+'.'+cast(YEAR(CURRENT_TIMESTAMP) as varchar(max))+'%')
select cast(case when @IdArticle > 0 then 1 else 0 end as bit)").ToList().FirstOrDefault();
        }

        public void DeleteOrderInPDM(Card card)
        {
            Database.ExecuteSql(
@$"Declare @IdDseOrder int = (select top 1 Dse from Orders where Code_+Isp+Ku like {card.Order} and Deleted <> 1)
Declare @Control int = (select Id from ref_Dse where DecNum like {card.ProductCode} and Id = @IdDseOrder)
If (@Control>0)
Begin
Delete from Connect where IdIn = @IdDseOrder
End
Else
Begin
	RAISERROR('Удаление не может быть выполнено. Обратитесь к разработчикам', 17, 1);
End");
        }

        public void ExportToNormaVremia(int cardId, string code, int department)
        {
            Database.ExecuteSql(
@$"declare @Operations table ( Code varchar(6), Year int, Month int, Executor varchar(max), Labor decimal(18, 3) )
insert into @Operations
select
ProductionOperationCode Code,
year(Date) Year,
month(Date) Month,
Executor,
sum(LaborAll) Labor
from
(
	select o.CardId, o.Code, o.Count, o.Date, o.Department, o.GroupName, o.Id, 
	cast(o.Labor as decimal(18, 3)) Labor, cast(o.Labor * o.Count as decimal(18, 3)) LaborAll, o.Name, 
	o.UnitName, e.Name Executor, rt.Code ProductionOperationCode, rt.Name ProductionOperationName from CRCardOperations o
	left join CRExecutors e on e.Id = o.ExecutorId
	left join Ref_top_2 rt on rt.Code = e.ProductionOperationCode and rt.forLastVerRef = 1
	where o.CardId = {cardId} and o.[Type] = 1

    --union all

	--select c.Id, o.Code, o.Count * p.Count Count, o.Date, o.Department, o.GroupName, o.Id,
	--cast(o.Labor as decimal(18, 3)) Labor, cast(o.Labor * o.Count * p.Count as decimal(18, 3)) LaborAll, o.Name, 
	--o.UnitName, e.Name Executor, rt.Code ProductionOperationCode, rt.Name ProductionOperationName from CRCards c 
	--join CRCardOwnProducts p on p.CardId = c.Id
	--join CRCardOwnProductRepairOperations o on o.CardOwnProductId = p.Id
	--left join CRExecutors e on e.Id = o.ExecutorId
	--left join Ref_top_2 rt on rt.Code = e.ProductionOperationCode and rt.forLastVerRef = 1
	--where c.Id = {cardId}
) r
where ProductionOperationCode is not null
group by ProductionOperationCode, year(Date), month(Date), Executor


delete from AccountingRateTime where decnum = {code} and convert(int, substring(CodeOp, 1, 3)) = {department}

declare @MaxNumber int = isnull((select max(Number) from AccountingRateTime where decnum = {code}), 0)

insert into AccountingRateTime (Number, Decnum, CodeOp, Dept, Labor, Rank, Rate, Cost, Nacenka, Descr, Koef, CodeZak, Date, LaborNew, CostNew, groupNew, Descr_OOIOT, PieceworkNew, Price)
select 
row_number() over(order by o.Year, o.Month, o.Code, o.Executor) + @MaxNumber Number,
{code} DecNum,
rt.Code CodeOp,
substring(rt.Code, 1, 3) Dept,
o.Labor, rt.Rank, rt.Rate,
cast(round(cast(o.Labor as decimal(18, 3)) * rt.Rate, 2) as decimal(18, 2)) Cost,
1 Nacenka,
'' Descr,
1 Koef,
null CodeZak,
getdate() Date,
o.Labor LaborNew,
cast(round(cast(o.Labor as decimal(18, 3)) * rt.Rate, 2) as decimal(18, 2)) CostNew,
1 groupNew,
null Descr_OOIOT,
cast(round(cast(o.Labor as decimal(18, 3)) * rt.piecework, 2) as decimal(18, 2)) PieceworkNew,
rt.piecework Price
from @Operations o
join ref_top_2 rt on rt.Code = o.Code and rt.forLastVerRef = 1
where convert(int, substring(rt.Code, 1, 3)) = {department}");
        }

        public Task<List<ProductMaterial>> GetMaterialsByProduct(string productCode)
        {
            return ProductMaterials.FromSqlRaw(
"s_PP_export_mat_1c_newgeneration_cardrepair2 @dse_num = @p0, @count = 1, @vr = @p1, @flag = 1, @type_det = 1, @cur_cex = 0", 
                parameters: new[] { productCode, DateTime.Now.ToString("ddMMyyhhmmssffff") })
                .AsNoTracking()
                .ToListAsync();
        }
        #endregion
    }
}
