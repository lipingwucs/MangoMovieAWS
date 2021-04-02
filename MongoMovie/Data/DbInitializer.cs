using Microsoft.AspNetCore.Identity;
using MongoMovie.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace MongoMovie.Data
{
	public class DbInitializer
	{
		public static async Task Initialize(AppIdentityDbContext context, UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager)
		{
			context.Database.EnsureCreated();
			if (context.Users.Any())
			{
				return;   // DB has been seeded  
			}
			await CreateDefaultUserAndRole(userManager, roleManager);
			// add any initial data to the application
			context.SaveChanges();
		}
		private static async Task CreateDefaultUserAndRole(UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager)
		{
			string roleAdmin = "Admin";
			string roleUser = "User";
			// create the two default role: User, Admin
			await CreateDefaultRole(roleManager, roleAdmin);
			await CreateDefaultRole(roleManager, roleUser);
			// create the default admin user account
			var user = await CreateDefaultUser(userManager, "admin", "admin@demo.com", "admin123");
			await AddDefaultRoleToDefaultUser(userManager, roleAdmin, user);
			// create app user account for testing
			user = await CreateDefaultUser(userManager, "test001", "test001@demo.com", "test123");
			await AddDefaultRoleToDefaultUser(userManager, roleUser, user);
			user = await CreateDefaultUser(userManager, "liping", "liping@demo.com", "test123");
			await AddDefaultRoleToDefaultUser(userManager, roleUser, user);
		}

		private static async Task CreateDefaultRole(RoleManager<IdentityRole> roleManager, string role)
		{
			await roleManager.CreateAsync(new IdentityRole(role));
		}

		private static async Task<AppUser> CreateDefaultUser(UserManager<AppUser> userManager, String username, String email, String password)
		{
			var user = new AppUser { Email = email, UserName = username };

			await userManager.CreateAsync(user, password);

			var createdUser = await userManager.FindByEmailAsync(email);
			return createdUser;
		}

		private static async Task AddDefaultRoleToDefaultUser(UserManager<AppUser> userManager, string role, AppUser user)
		{
			await userManager.AddToRoleAsync(user, role);
		}
	}
}
