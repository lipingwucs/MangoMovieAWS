using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MongoMovie.Data;
using MongoMovie.Models;
using MongoMovie.Models.AppUsersViewModels;

namespace MongoMovie.Controllers
{
	[Authorize(Roles = "Admin")]
	public class UserAdminController : Controller
	{
		private readonly AppIdentityDbContext _context;
		private readonly UserManager<AppUser> _userManager;
		private readonly RoleManager<IdentityRole> _roleManager;

		public UserAdminController(
		AppIdentityDbContext context,
		UserManager<AppUser> userManager,
		RoleManager<IdentityRole> roleManager
		)
		{
			_context = context;
			_userManager = userManager;
			_roleManager = roleManager;
		}

		// GET: AppUsers
		public async Task<IActionResult> Index()
		{
			var userViewModels = new List<UserViewModel>();
			var users = await _context.AppUser.ToListAsync();
			foreach (AppUser u in users)
			{
				UserViewModel _userViewModel = new UserViewModel
				{
					Id = u.Id,
					UserName = u.UserName,
					PhoneNumber = u.PhoneNumber,
					Email = u.Email,
					Roles = await _userManager.GetRolesAsync(u),
				};
				userViewModels.Add(_userViewModel);
			}
			//return View(await _context.AppUser.ToListAsync());
			return View(userViewModels);
		}

		// GET: AppUsers/Details/5
		public async Task<IActionResult> Details(string id)
		{
			if (id == null)
			{
				return NotFound();
			}

			UserViewModel userViewModel;

			var AppUser = await _context.AppUser
				.SingleOrDefaultAsync(m => m.Id == id);
			if (AppUser == null)
			{
				return NotFound();
			}
			else
			{
				userViewModel = new UserViewModel
				{
					Id = AppUser.Id,
					UserName = AppUser.UserName,
					PhoneNumber = AppUser.PhoneNumber,
					Email = AppUser.Email,
					Roles = await _userManager.GetRolesAsync(AppUser),
				};
				return View(userViewModel);
			}
		}

		// GET: AppUsers/Create
		public async Task<IActionResult> Create()
		{
			ViewBag.RolesList = new SelectList(await _roleManager.Roles.ToListAsync(), "Name", "NormalizedName");
			return View();
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Create(CreateViewModel model, string returnUrl = null, params string[] selectedRoles)
		{
			ViewData["ReturnUrl"] = returnUrl;
			if (ModelState.IsValid)
			{				
				var user = new AppUser { UserName = model.UserName, Email = model.Email, PhoneNumber = model.PhoneNumber };
				var userResult = await _userManager.CreateAsync(user, model.Password);
				if (userResult.Succeeded)
				{
					if (selectedRoles != null)
					{
						var roleResult = await _userManager.AddToRolesAsync(user, selectedRoles);
						if (roleResult.Succeeded)
						{
							return RedirectToAction(nameof(Index));
						}
						foreach (var error in roleResult.Errors)
						{
							ModelState.AddModelError(string.Empty, error.Description);
						}
					}
					//return RedirectToAction(nameof(Index));
				}

				foreach (var error in userResult.Errors)
				{
					ModelState.AddModelError(string.Empty, error.Description);
				}
			}
			// If we got this far, something failed, redisplay form
			return View(model);
		}

		// GET: AppUsers/Edit/5
		public async Task<IActionResult> Edit(string id)
		{
			if (id == null)
			{
				return NotFound();
			}

			UserViewModel userViewModel;

			var AppUser = await _context.AppUser
				.SingleOrDefaultAsync(m => m.Id == id);
			if (AppUser == null)
			{
				return NotFound();
			}
			else
			{
				userViewModel = new UserViewModel
				{
					Id = AppUser.Id,
					UserName = AppUser.UserName,
					PhoneNumber = AppUser.PhoneNumber,
					Email = AppUser.Email,
					Roles = await _userManager.GetRolesAsync(AppUser),
				};
				ViewBag.RolesList = new SelectList(await _roleManager.Roles.ToListAsync(), "Name", "NormalizedName");
				return View(userViewModel);
			}
		}

		
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Edit(string id, UserViewModel model, string returnUrl = null, params string[] selectedRoles)
		{

			if (id != model.Id)
			{
				return NotFound();
			}
			AppUser user = await _userManager.FindByIdAsync(id);
			if (user == null)
			{
				return NotFound();
			}

			if (user.PhoneNumber != model.PhoneNumber)
			{
				try
				{
					user.PhoneNumber = model.PhoneNumber;
					await _userManager.UpdateAsync(user);
				}
				catch (DbUpdateConcurrencyException)
				{
					return NotFound();
				}
			}

			if (selectedRoles != null)
			{
				var roles = await _userManager.GetRolesAsync(user);
				await _userManager.RemoveFromRolesAsync(user, roles.ToArray());

				var roleResult = await _userManager.AddToRolesAsync(user, selectedRoles);
				if (roleResult.Succeeded)
				{
					return RedirectToAction(nameof(Index));
				}
				foreach (var error in roleResult.Errors)
				{
					ModelState.AddModelError(string.Empty, error.Description);
				}
			}

			return View(model);
		}

		// GET: AppUsers/Delete/5
		public async Task<IActionResult> Delete(string id)
		{
			if (id == null)
			{
				return NotFound();
			}

			UserViewModel userViewModel;

			var AppUser = await _context.AppUser
				.SingleOrDefaultAsync(m => m.Id == id);
			if (AppUser == null)
			{
				return NotFound();
			}
			else
			{
				userViewModel = new UserViewModel
				{
					Id = AppUser.Id,
					UserName = AppUser.UserName,
					PhoneNumber = AppUser.PhoneNumber,
					Email = AppUser.Email,
					Roles = await _userManager.GetRolesAsync(AppUser),
				};
				return View(userViewModel);
			}
		}

		// POST: AppUsers/Delete/5
		[HttpPost, ActionName("Delete")]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> DeleteConfirmed(string id)
		{
			var AppUser = await _context.AppUser.SingleOrDefaultAsync(m => m.Id == id);
			var roles = await _userManager.GetRolesAsync(AppUser);
			await _userManager.RemoveFromRolesAsync(AppUser, roles.ToArray());

			// _context.AppUser.Remove(AppUser);
			await _userManager.DeleteAsync(AppUser);
			return RedirectToAction(nameof(Index));
		}

		private bool AppUserExists(string id)
		{
			return _context.AppUser.Any(e => e.Id == id);
		}
	}
}
