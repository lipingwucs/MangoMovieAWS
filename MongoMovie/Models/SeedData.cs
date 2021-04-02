using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using MongoMovie.Data;
using System;

namespace MongoMovie.Models
{
	public class SeedData
	{
		public static void EnsurePopulated(IApplicationBuilder app)
		{
			MovieDbContext context = app.ApplicationServices
			.GetRequiredService<MovieDbContext>();
			context.Database.Migrate();

			//add some seeddata for movie
			if (!context.Movies.Any())
			{
				context.Movies.AddRange(
					new Movie
					{
						Title = "Rio Bravo",
						ReleaseDate = DateTime.Parse("1959-4-15"),
						Genre = "Western",
						Price = 3.99M
					},
					new Movie
					{
						Title = "Ghostbusters 2",
						ReleaseDate = DateTime.Parse("1986-2-23"),
						Genre = "Comedy",
						Price = 9.99M
					},
					new Movie
					{
						Title = "Ghostbusters ",
						ReleaseDate = DateTime.Parse("1984-3-13"),
						Genre = "Comedy",
						Price = 8.99M
					}
				);
				context.SaveChanges();
			}

			//add some seeddata for moviereviews
			/*
			if (!context.MovieReviews.Any())
			{
				context.MovieReviews.AddRange(
					new MovieReview {MovieID = 1, FirstName = "Bill", LastName = "Gates", Email = "bg@hotmail.com",	Telephone = "888-888-8888",	Message = "This is my favorit movie."},
					new MovieReview { MovieID = 1, FirstName = "Michael", LastName = "Fisher", Email = "mf@hotmail.com", Telephone = "888-888-8888", Message = "This is my favorit movie." },
					new MovieReview { MovieID = 1, FirstName = "Susan", LastName = "Mayer", Email = "sm@hotmail.com", Telephone = "888-888-8888", Message = "This is my chidren's favorit movie." },

					new MovieReview { MovieID = 2, FirstName = "Bill", LastName = "Gates", Email = "bg@hotmail.com", Telephone = "888-888-8888", Message = "it makes miss my hometown" },
					new MovieReview { MovieID = 2, FirstName = "Michael", LastName = "Fisher", Email = "mf@hotmail.com", Telephone = "888-888-8888", Message = "miss my grandma" },
					new MovieReview { MovieID = 2, FirstName = "Susan", LastName = "Mayer", Email = "sm@hotmail.com", Telephone = "888-888-8888", Message = "good memory movie" },

					new MovieReview { MovieID = 3, FirstName = "Bill", LastName = "Gates", Email = "bg@hotmail.com", Telephone = "888-888-8888", Message = "it's so funny." },
					new MovieReview { MovieID = 3, FirstName = "Michael", LastName = "Fisher", Email = "mf@hotmail.com", Telephone = "888-888-8888", Message = "makes us laughing all the day" },
					new MovieReview { MovieID = 3, FirstName = "Susan", LastName = "Mayer", Email = "sm@hotmail.com", Telephone = "888-888-8888", Message = "make our day feel relax, thanks" }

				);
				context.SaveChanges();
			}
			*/

			
		}
	}
}


