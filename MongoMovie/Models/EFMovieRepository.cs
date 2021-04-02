using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel;
using Microsoft.EntityFrameworkCore;
using MongoMovie.Data;

namespace MongoMovie.Models
{
	public class EFMovieRepository:IMovieRepository
	{
		private MovieDbContext context;
		//constructor
		public EFMovieRepository(MovieDbContext ctx)
		{
			this.context = ctx;
		}
		
		public DbSet<Movie> Movies => context.Movies;

		[DatabaseGenerated(DatabaseGeneratedOption.Computed)]
		[DefaultValue("newid()")]
		public Guid MovieFileToken { get; set; }
		// load Movie with related data
		public Movie GetMovie(int ID)
		{
			var found = this.context.Movies				
				.Include(r => r.MovieReviews)				
			   .AsNoTracking()
			   .FirstOrDefault(p => p.ID == ID);
			return found;
		}

		//Movie create/update   
		public void SaveMovie(Movie movie)
		{
			if (movie.ID == 0)
			{
				context.Movies.Add(movie);    //if movie is not exist, just add
			}
			else
			{
				Movie dbEntry = context.Movies
				.FirstOrDefault(r => r.ID== movie.ID);
				if (dbEntry != null)
				{
					dbEntry.Title = movie.Title;
					dbEntry.Genre = movie.Genre;
					dbEntry.ReleaseDate = movie.ReleaseDate;
					dbEntry.Price = movie.Price;
					dbEntry.Path = movie.Path;
				}   //if recipe exist, updated the recipe
			}
			context.SaveChanges();
		}
		//Movie delete
		public Movie DeleteMovie(int movieID)
		{
			Movie dbEntry = context.Movies
			.FirstOrDefault(p => p.ID == movieID);
			if (dbEntry != null)
			{
				context.Movies.Remove(dbEntry);
				context.SaveChanges();
			}
			return dbEntry;
		}  //End of Moview Repo


		/*
		public IEnumerable<MovieReview> MovieReviews => context.MovieReviews;

		[DatabaseGenerated(DatabaseGeneratedOption.Computed)]
		[DefaultValue("newid()")]
		public Guid MovieReviewFileToken { get; set; }

		// load Review with related data
		public MovieReview GetMovieReview(int ID)
		{
			var found = this.context.MovieReviews
			   .Include(m => m.Movie)
			   .AsNoTracking()
			   .FirstOrDefault(p => p.ID == ID);
			return found;
		}

		public MovieReview SaveMovieReview(MovieReview movieReview)
		{
			if (movieReview.ID == 0)
			{
				context.MovieReviews.Add(movieReview);
			}
			else
			{
				MovieReview dbEntry = context.MovieReviews
				.FirstOrDefault(p => p.ID == movieReview.ID);
				if (dbEntry != null)
				{
					dbEntry.MovieID = movieReview.MovieID;
					dbEntry.FirstName = movieReview.FirstName;
					dbEntry.LastName = movieReview.LastName;
					dbEntry.Telephone = movieReview.Telephone;
					dbEntry.Email = movieReview.Email;
					dbEntry.Message = movieReview.Message;
				}
			}
			context.SaveChanges();
			return movieReview;
		}

		//MovieReview delete
		public MovieReview DeleteMovieReview(int ID)
		{
			MovieReview dbEntry = this.GetMovieReview(ID);
			if (dbEntry != null)
			{
				context.MovieReviews.Remove(dbEntry);
				context.SaveChanges();
			}
			return dbEntry;
		}	

		*/
	}
}
