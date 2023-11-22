using RepairCardsMVC.Exceptions;
using RepairCardsMVC.Models;
using RepairCardsMVC.ViewModels;

namespace RepairCardsMVC.Services
{
    public class TemplateService
    {
        private readonly ApplicationDbContext _db;

        public TemplateService(ApplicationDbContext db)
        {
            _db = db;
        }

        public DataHelperViewModel<Template> GetAll(HttpRequest request)
            => request.GetDataForJQueryDataTable(_db.Templates.AsQueryable(), "Name", "Description");

        public async Task<PaginatedList<Template>> GetAll(int pageNumber, string filter)
        {
            var items = _db.Templates
                .Where(x => x.Department.ToString().Contains(filter) ||
                (x.Name ?? "").Contains(filter) ||
                (x.Description ?? "").Contains(filter));

            return await PaginatedList<Template>.CreateAsync(items, pageNumber, 10);
        }

        public async Task<Template?> Get(int id)
            => await _db.Templates.FindAsync(id);

        public async Task Add(Template item)
        {
            _db.Templates.Add(item);
            await _db.SaveChangesAsync();
        }

        public async Task Edit(Template item)
        {
            _db.Templates.Update(item);
            await _db.SaveChangesAsync();
        }

        public async Task Remove(int id)
        {
            var item = await _db.Templates.FindAsync(id);

            if (item == null)
                throw new NotFoundException();

            _db.Templates.Remove(item);
            await _db.SaveChangesAsync();
        }
    }
}
