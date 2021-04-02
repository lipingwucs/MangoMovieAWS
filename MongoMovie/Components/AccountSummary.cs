using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MongoMovie.Models;
using MongoMovie.Models.AccountViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MongoMovie.Components
{
	public class AccountSummary : ViewComponent
	{
		private readonly UserManager<AppUser> _userManager;

		public AccountSummary(UserManager<AppUser> userManager)
		{
			_userManager = userManager;
		}

		private AppUser _user;

		public IViewComponentResult Invoke()
		{
			GetUser().Wait();

			if (_user != null)
			{
				var model = new AccountSummaryModel
				{
					Email = _user.Email,
					Name = _user.UserName
				};

				return View(model);
			}
			return View();
		}

		private async Task GetUser()
		{
			_user = await _userManager.FindByNameAsync(User.Identity.Name);
		}
	}
}
