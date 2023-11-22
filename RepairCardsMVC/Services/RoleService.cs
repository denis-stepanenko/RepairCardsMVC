using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using RepairCardsMVC.Exceptions;

namespace RepairCardsMVC.Services
{
    public class RoleService
    {
        private readonly RoleManager<IdentityRole> _roleManager;

        public RoleService(RoleManager<IdentityRole> roleManager)
        {
            _roleManager = roleManager;
        }

        public async Task<IEnumerable<IdentityRole>> GetAll()
            => await _roleManager.Roles.ToListAsync();

        public async Task Add(string name)
            => await _roleManager.CreateAsync(new IdentityRole { Name = name });

        public async Task Remove(string id)
        {
            var role = await _roleManager.FindByIdAsync(id);

            if (role == null)
                throw new NotFoundException();

            await _roleManager.DeleteAsync(role);
        }
    }
}
