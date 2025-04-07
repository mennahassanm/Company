using Company.MVC.DAL.Models;
using Company.MVC.PL.DTOS;
using Company.MVC.PL.Helpers;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Company.MVC.PL.Controllers
{
    public class RoleController : Controller
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<AppUser> _userManager;

        public RoleController(RoleManager<IdentityRole> roleManager , UserManager<AppUser> userManager)
        {
            _roleManager = roleManager;
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<IActionResult> Index(string? SearchInput)
        {
            IEnumerable<RoleToReturnDTO> roles;
            if (string.IsNullOrEmpty(SearchInput))
            {
                roles = _roleManager.Roles.Select(R => new RoleToReturnDTO()
                {
                    Id = R.Id,
                    Name = R.Name,
                });

            }
            else
            {
                roles = _roleManager.Roles.Select(R => new RoleToReturnDTO()
                {
                    Id = R.Id,
                    Name= R.Name,


                }).Where(R => R.Name.ToLower().Contains(SearchInput.ToLower()));
            }
            return View(roles);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(RoleToReturnDTO model)
        {
            if (ModelState.IsValid) // Server Side Validation
            {
                var role = await _roleManager.FindByNameAsync(model.Name);
                if(role is null)
                {
                    role = new IdentityRole()
                    {
                        Name = model.Name,
                    };

                    var result = await _roleManager.CreateAsync(role); 
                    if(result.Succeeded)
                    {
                        return RedirectToAction("Index");
                    }
                }

            }
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Details(string? id, string viewName = "Details")
        {
            if (id is null) return BadRequest("Invalid Id");
            var role = await _roleManager.FindByIdAsync(id);
            if (role is null) return NotFound(new { StatusCode = 404, message = $"Role With Id : {id} is not found" });

            var dto = new RoleToReturnDTO()
            {
                Id = role.Id,
                Name = role.Name,
            };

            return View(viewName, dto);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(string? id)
        {
            return await Details(id, "Edit");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit([FromRoute] string id, RoleToReturnDTO model)
        {
            if (ModelState.IsValid)
            {
                if (id != model.Id) return BadRequest($" This Id = {id} InValid");

                var role = await _roleManager.FindByIdAsync(id);

                if (role is null) return BadRequest(" InValid Operations ");

                var roleResult= await _roleManager.FindByNameAsync(model.Name);
                if (roleResult is  null)
                {
                    role.Name = model.Name;

                    var Result = await _roleManager.UpdateAsync(role);

                    if (Result.Succeeded)
                    {
                        return RedirectToAction(nameof(Index));
                    }
                }
                ModelState.AddModelError("", "InValid Operations !");

            }
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Delete(string? id)
        {
            return await Details(id, "Delete");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete([FromRoute] string? id, RoleToReturnDTO model)
        {

            if (ModelState.IsValid)
            {
                if (id != model.Id) return BadRequest($" This Id = {id} InValid");

                var role = await _roleManager.FindByIdAsync(id);

                if (role is null) return BadRequest(" InValid Operations ");
      
                    var Result = await _roleManager.DeleteAsync(role);

                    if (Result.Succeeded)
                    {
                        return RedirectToAction(nameof(Index));
                    }
                
                ModelState.AddModelError("", "InValid Operations !");

            }
            return View(model);

        }

        [HttpGet]
        public async Task<IActionResult> AddOrRemoveUser(string roleId)
        {
            var role = await _roleManager.FindByIdAsync(roleId);

            if (role is null) return NotFound();

            ViewData["RoleId"] = roleId;

            var UsersInRole = new List<UsersInRoleViewModel>();

            var users = await _userManager.Users.ToListAsync();

            foreach (var user in users)
            {
                var userInRole = new UsersInRoleViewModel()
                {
                    UserId = user.Id,
                    UserName = user.UserName
                };

                if (await _userManager.IsInRoleAsync(user, role.Name))
                {
                    userInRole.IsSelected = true;
                }
                else
                {
                    userInRole.IsSelected = false;
                }

                UsersInRole.Add(userInRole);

            }

            return View(UsersInRole);
        }

        [HttpPost]
        public async Task<IActionResult> AddOrRemoveUser(string roleId, List<UsersInRoleViewModel> users)
        {

            var role = await _roleManager.FindByIdAsync(roleId);

            if (role is null) 
                return NotFound();

            if (ModelState.IsValid)
            {
                foreach (var user in users)
                {
                    var appUser = await _userManager.FindByIdAsync(user.UserId);

                    if (appUser is not null)
                    {
                        if (user.IsSelected && ! await _userManager.IsInRoleAsync(appUser, role.Name))
                        {
                            await _userManager.AddToRoleAsync(appUser, role.Name);
                        }
                        else if (!user.IsSelected && await _userManager.IsInRoleAsync(appUser, role.Name))
                        {
                            await _userManager.RemoveFromRoleAsync(appUser, role.Name);
                        }
                    }

                }

                return RedirectToAction(nameof(Edit), new { id = roleId });
            }

            return View(users);

        }
    }
}
