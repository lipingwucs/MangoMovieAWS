using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MongoMovie.Models;
using System;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;

namespace MongoMovie.Data
{
	public class AppIdentityDbContext : IdentityDbContext<AppUser>
	{
		public DbSet<AppUser> AppUser { get; set; }

		public AppIdentityDbContext(DbContextOptions<AppIdentityDbContext> options)
			: base(options) { }

		public static async Task CreateAdminAccount(IServiceProvider serviceProvider,
			IConfiguration configuration)
		{

			UserManager<AppUser> userManager =
				serviceProvider.GetRequiredService<UserManager<AppUser>>();
			RoleManager<IdentityRole> roleManager =
				serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

			string username = configuration["Data:AdminUser:Name"];
			string email = configuration["Data:AdminUser:Email"];
			string password = configuration["Data:AdminUser:Password"];
			string role = configuration["Data:AdminUser:Role"];

			if (await userManager.FindByNameAsync(username) == null)
			{
				if (await roleManager.FindByNameAsync(role) == null)
				{
					await roleManager.CreateAsync(new IdentityRole(role));
				}

				AppUser user = new AppUser
				{
					UserName = username,
					Email = email
				};

				IdentityResult result = await userManager
					.CreateAsync(user, password);
				if (result.Succeeded)
				{
					await userManager.AddToRoleAsync(user, role);
				}
			}
		}

		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		{
			//optionsBuilder.UseSqlServer(@"Server=(LocalDB)\MSSQLLocalDB;Database=YummyIdentity;Trusted_Connection=True;MultipleActiveResultSets=true");
			//optionsBuilder.UseMySql(Helpers.GetIdentityRDSConnectionString());
			optionsBuilder.UseSqlServer(Helpers.GetIdentityRDSConnectionString());
		}

	}
}
