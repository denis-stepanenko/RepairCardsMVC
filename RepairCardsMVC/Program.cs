using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using RepairCardsMVC;
using RepairCardsMVC.Filters;
using RepairCardsMVC.Models;
using RepairCardsMVC.Services;
using System.Text.Encodings.Web;
using System.Text.Unicode;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnectionString")));

builder.Services.AddDefaultIdentity<User>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;
    options.Password.RequiredLength = 6;
    options.Password.RequireDigit = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireNonAlphanumeric = false;
})
.AddRoles<IdentityRole>()
.AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddControllersWithViews(options =>
{
    options.Filters.Add<ExceptionFilter>();
});

builder.Services.AddRazorPages();

builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession();

builder.Services.AddCors();

builder.Services.AddSingleton(HtmlEncoder.Create(allowedRanges: new[] { UnicodeRanges.BasicLatin, UnicodeRanges.Cyrillic }));

builder.Services.AddTransient<CardConfirmationObjectService>();
builder.Services.AddTransient<CardConfirmationService>();
builder.Services.AddTransient<CardDetailsService>();
builder.Services.AddTransient<CardDocumentService>();
builder.Services.AddTransient<CardMaterialService>();
builder.Services.AddTransient<CardOperationService>();
builder.Services.AddTransient<CardOwnProductOperation>();
builder.Services.AddTransient<CardOwnProductRepairOperationService>();
builder.Services.AddTransient<CardOwnProductService>();
builder.Services.AddTransient<CardPurchasedProductOperationService>();
builder.Services.AddTransient<CardPurchasedProductService>();
builder.Services.AddTransient<CardService>();
builder.Services.AddTransient<CardStatusService>();
builder.Services.AddTransient<ExecutorService>();
builder.Services.AddTransient<ExportRequestService>();
builder.Services.AddTransient<MaterialService>();
builder.Services.AddTransient<OperationService>();
builder.Services.AddTransient<OrderService>();
builder.Services.AddTransient<ProductionOperationService>();
builder.Services.AddTransient<ProductOperationService>();
builder.Services.AddTransient<ProductService>();
builder.Services.AddTransient<RepairTypeService>();
builder.Services.AddTransient<RequestService>();
builder.Services.AddTransient<RequestToCreateStagesIn1SService>();
builder.Services.AddTransient<RoleService>();
builder.Services.AddTransient<TemplateOperationService>();
builder.Services.AddTransient<TemplateService>();
builder.Services.AddTransient<UnlockedPeriodService>();
builder.Services.AddTransient<UserService>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseCors(x => x
.AllowAnyMethod()
.AllowAnyHeader()
.SetIsOriginAllowed(o => true)
.AllowCredentials());

app.UseRouting();

app.UseAuthentication();

app.UseAuthorization();

app.UseSession();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Card}/{action=Index}/{id?}");

app.MapRazorPages();

app.Run();
