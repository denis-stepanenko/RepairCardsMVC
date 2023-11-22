using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using RepairCardsMVC.Exceptions;
using RepairCardsMVC.Models;

namespace RepairCardsMVC.Services
{
    public class UserService
    {
        private readonly UserManager<User> _userManager;
        private readonly IPasswordHasher<User> _passwordHasher;

        public UserService(UserManager<User> userManager, IPasswordHasher<User> passwordHasher)
        {
            _userManager = userManager;
            _passwordHasher = passwordHasher;
        }

        public async Task<IEnumerable<User>> GetAll()
            => await _userManager.Users.ToListAsync();

        public async Task<User?> Get(string id)
            => await _userManager.FindByIdAsync(id);

        public async Task Add(string userName, string name, int department, string password)
        {
            var user = new User
            {
                UserName = userName,
                Name = name,
                Department = department
            };

            var result = await _userManager.CreateAsync(user, password);
        }

        public async Task Edit(string id, string userName, string name, int department, List<string> roles)
        {
            var user = await _userManager.FindByIdAsync(id);

            if (user == null)
                throw new NotFoundException();

            user.UserName = userName;
            user.Name = name;
            user.Department = department;

            await _userManager.UpdateAsync(user);

            var userRoles = await _userManager.GetRolesAsync(user);

            var addedRoles = roles.Except(userRoles);

            var removedRoles = userRoles.Except(roles);

            await _userManager.AddToRolesAsync(user, addedRoles);

            await _userManager.RemoveFromRolesAsync(user, removedRoles);
        }

        public async Task Remove(string id)
        {
            var user = await _userManager.FindByIdAsync(id);

            if (user == null)
                throw new NotFoundException();

            await _userManager.DeleteAsync(user);
        }

        public async Task ChangePassword(string id, string newPassword)
        {
            var user = await _userManager.FindByIdAsync(id);

            if (user == null)
                throw new NotFoundException();

            user.PasswordHash = _passwordHasher.HashPassword(user, newPassword);

            await _userManager.UpdateAsync(user);
        }

        public async Task<IEnumerable<string>> GetRolesByUser(Models.User user)
            => await _userManager.GetRolesAsync(user);
    }
}
