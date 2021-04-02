using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoMovie.Models;
using MongoMovie.Data;
using MongoMovie.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using Amazon.DynamoDBv2;
using Amazon.S3;

namespace MongoMovie
{
	public class Startup
	{
		public Startup(IConfiguration configuration)
		{
			Configuration = configuration;
		}

		public IConfiguration Configuration { get; }

		// This method gets called by the runtime. Use this method to add services to the container.
		public void ConfigureServices(IServiceCollection services)
		{
			services.AddDbContext<MovieDbContext>(options =>
			{
				//options.UseSqlServer(Configuration["Data:ConnectionStrings:MovieDbContext"]);
				//options.UseMySql(Helpers.GetRDSConnectionString());
				options.UseSqlServer(Helpers.GetRDSConnectionString());
			});
			services.AddTransient<IMovieRepository,EFMovieRepository>();
			//services.AddTransient<IMovieRepository,FakeMovieRepository>();

			/***one database contain domain data and app indentity ***
			services.AddIdentity<AppUser, IdentityRole>()
				.AddEntityFrameworkStores<MovieDbContext>()
				.AddDefaultTokenProviders();
			*/
			// split domain data and app indentity into two database
			services.AddDbContext<AppIdentityDbContext>(options =>
			{
				//options.UseSqlServer(Configuration["Data:ConnectionStrings:AppIdentityDbContext"]);
				//options.UseMySql(Helpers.GetIdentityRDSConnectionString());
				options.UseSqlServer(Helpers.GetIdentityRDSConnectionString());
			});
			services.AddIdentity<AppUser, IdentityRole>()
				.AddEntityFrameworkStores<AppIdentityDbContext>()
				.AddDefaultTokenProviders();
			// Add google login
			services.AddAuthentication().AddGoogle(opts => {
				opts.ClientId = "<enter client id here>";
				opts.ClientSecret = "<enter client secret here>";
			});

			services.Configure<IdentityOptions>(options =>
			{
				// Password settings  
				options.Password.RequireDigit = false;
				options.Password.RequiredLength = 6;
				options.Password.RequireNonAlphanumeric = false;
				options.Password.RequireUppercase = false;
				options.Password.RequireLowercase = false;
				options.Password.RequiredUniqueChars = 6;

				// Lockout settings  
				options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(30);
				options.Lockout.MaxFailedAccessAttempts = 5;
				options.Lockout.AllowedForNewUsers = false;

				// User settings  
				options.User.RequireUniqueEmail = false;
			});

			services.ConfigureApplicationCookie(options =>
			{
				// Cookie settings  
				options.Cookie.HttpOnly = true;
				//options.Cookie.Expiration = TimeSpan.FromDays(150);
				options.LoginPath = "/Account/Login"; // If the LoginPath is not set here, ASP.NET Core will default to /Account/Login  
				options.LogoutPath = "/Account/Logout"; // If the LogoutPath is not set here, ASP.NET Core will default to /Account/Logout  
				options.AccessDeniedPath = "/Account/AccessDenied"; // If the AccessDeniedPath is not set here, ASP.NET Core will default to /Account/AccessDenied  
				//options.SlidingExpiration = true;
				options.ExpireTimeSpan = TimeSpan.FromMinutes(5);
			});

			// Add application services.
			services.AddTransient<IEmailSender, EmailSender>();

		//	services.AddTransient<IMenu, MenuService>();
			services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
		//	services.AddScoped(sp => Cart.GetCart(sp));

			// configure the required Session services
			services.AddSession(options =>
			{
				// Set a short timeout for easy testing.
				options.IdleTimeout = TimeSpan.FromMinutes(5);
				options.Cookie.HttpOnly = true;
			});

			services.AddMvc(); //.SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
			//inject AWS services 
			services.AddDefaultAWSOptions(Configuration.GetAWSOptions());
			services.AddAWSService<IAmazonS3>();
			services.AddAWSService<IAmazonDynamoDB>();

			services.AddTransient<IDynamoDBServices, DynamoDBServices>();

		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IHostingEnvironment env)
		{
			app.UseDeveloperExceptionPage();
			app.UseStatusCodePages();

			if (env.IsDevelopment())
			{
				app.UseBrowserLink();
				app.UseDeveloperExceptionPage();
			}
			else
			{
				app.UseExceptionHandler("/Home/Error");
			}

			app.UseStaticFiles();
			// app.UseCookiePolicy();
			app.UseAuthentication();

			//enable session before MVC
			app.UseSession();

			app.UseMvc(routes =>
			{
				routes.MapRoute(
					name: "default",
					template: "{controller=Home}/{action=Index}/{id?}");
			});

			// TODO: need to comment out NEXT line when you generate database migration version
			//SeedData.EnsurePopulated(app);  // need to comment out this line when you generate database migration version

		}		
	}
}
