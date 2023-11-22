using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RepairCardsMVC.Services;
using RepairCardsMVC.ViewModels;

namespace RepairCardsMVC.Controllers
{
    [Authorize(Roles = "admin")]
    public class UserController : Controller
    {
        private readonly UserService _userService;
        private readonly RoleService _roleService;

        public UserController(UserService userService, RoleService roleService)
        {
            _userService = userService;
            _roleService = roleService;

        }

        public async Task<IActionResult> Index()
        {
            return View(await _userService.GetAll());
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateUserViewModel model)
        {
            if(!ModelState.IsValid)
            {
                return View(model);
            }

            await _userService.Add(model.UserName, model.Name, model.Department, model.Password);

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(string id)
        {
            var user = await _userService.Get(id);

            if (user == null)
                return NotFound();

            var userRoles = await _userService.GetRolesByUser(user);

            var roles = await _roleService.GetAll();

            ViewBag.UserRoles = userRoles;
            ViewBag.Roles = roles;

            return View(new EditUserViewModel { UserName = user.UserName, Name = user.Name, Department = user.Department });
        }

        [HttpPost]
        public async Task<IActionResult> Edit(EditUserViewModel model)
        {
            if(!ModelState.IsValid)
            {
                return View(model);
            }

            await _userService.Edit(model.Id, model.UserName, model.Name, model.Department, model.Roles);

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(string id)
        {
            await _userService.Remove(id);

            return RedirectToAction(nameof(Index));
        }
    
        public IActionResult ChangePassword(string id)
        {
            return View(new ChangePasswordViewModel { Id = id });
        }

        [HttpPost]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if(!ModelState.IsValid)
            {
                return View(model);
            }

            await _userService.ChangePassword(model.Id, model.NewPassword);

            return RedirectToAction(nameof(Index));
        }
    }
}
