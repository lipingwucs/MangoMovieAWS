using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoMovie.Models;

namespace MongoMovie.Services
{
    public interface IDynamoDBServices
    {
          Task<MovieReview> SaveMovieReview(MovieReview movieReview);
          Task<MovieReview> UpdateMovieReview(MovieReview movieReview);
          Task DeleteMovieReview(string Id);
          Task<MovieReview> GetMovieReview(string Id);
          Task<List<MovieReview>> GetMovieReviews(int MovieID);
    }

}
