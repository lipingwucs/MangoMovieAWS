using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MongoMovie.Models
{
	public interface IMovieRepository
	{
		DbSet<Movie> Movies { get; }
		void SaveMovie(Movie movie);
		Movie DeleteMovie(int ID);
		Movie GetMovie(int ID);
	}
}
