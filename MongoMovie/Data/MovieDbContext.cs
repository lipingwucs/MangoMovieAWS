using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Linq;
using MongoMovie.Models;


namespace MongoMovie.Data
{
	public class MovieDbContext : DbContext
	{		
		public MovieDbContext(DbContextOptions<MovieDbContext> options) : base(options)
		{ }
	
		public DbSet<Movie> Movies { get; set; }
		public DbSet<MovieReview> MovieReviews { get; set; }  //bind table MovieReview
		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			// Customize the ASP.NET Identity model and override the defaults if needed.
			// For example, you can rename the ASP.NET Identity table names and more.
			// Add your customizations after calling base.OnModelCreating(builder);
			base.OnModelCreating(modelBuilder);			
		}

		
		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		{
			// optionsBuilder.UseSqlServer(@"Server=(LocalDB)\MSSQLLocalDB;Database=MongoMovie;Trusted_Connection=True;MultipleActiveResultSets=true");
			optionsBuilder.UseSqlServer(Helpers.GetRDSConnectionString());
			//optionsBuilder.UseMySql(Helpers.GetRDSConnectionString());
		}

		public override int SaveChanges()
		{
			var entries = ChangeTracker
				.Entries()
				.Where(e => e.Entity is BaseEntity && (
						e.State == EntityState.Added || e.State == EntityState.Modified));

			foreach (var entityEntry in entries)
			{
				((BaseEntity)entityEntry.Entity).Updated = DateTime.Now;

				if (entityEntry.State == EntityState.Added)
				{
					((BaseEntity)entityEntry.Entity).Created = DateTime.Now;
				}
			}

			return base.SaveChanges();
		}
	}
}


