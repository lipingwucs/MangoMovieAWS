using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoMovie.Models;
using MongoMovie.Infrastructure;
using Amazon.S3;
using Amazon.S3.Model;
using System.IO;
using Amazon.S3.Transfer;
using Microsoft.AspNetCore.Http;
using MongoMovie.Services;
using Microsoft.Extensions.Logging;
using Amazon;

namespace MongoMovie.Controllers
{
	[Authorize]
	public class MovieController : Controller
	{
		private IMovieRepository repository;
		public int PageSize = 10;

		// Specify your bucket region (an example region is shown).  
		private static readonly string bucketName = "lipingmovie";

		//private static readonly RegionEndpoint bucketRegion = RegionEndpoint.CACentral1;
		//private static readonly string accesskey = ConfigurationManager.AppSettings["AWSAccessKey"];
		//private static readonly string secretkey = ConfigurationManager.AppSettings["AWSSecretKey"];

		private IAmazonS3 s3Client { get; set; }
		private IDynamoDBServices dynamoSvc { get; set; }
		private readonly ILogger<MovieController> _logger;


		public MovieController(IMovieRepository repo, IAmazonS3 s3Client, IDynamoDBServices dynamoSvc, ILogger<MovieController> logger)
		{
			this.repository = repo;
			//this.s3Client = s3Client;
			this.s3Client = new AmazonS3Client("AKIA5SGL7RIC5OPACEXX", "vi9j5TIIqhDlLD3P1p8QKy0HJ9RUKF5FTxq5+ylu", RegionEndpoint.USEast1);
			this.dynamoSvc = dynamoSvc;
			this._logger = logger;
		}

		[AllowAnonymous]
		public async Task<IActionResult> List(string sortOrder,
			string currentFilter,
			string searchString,
			int? pageNumber)
		{
			ViewData["CurrentSort"] = sortOrder;
			ViewData["IDSortParm"] = String.IsNullOrEmpty(sortOrder) ? "id_desc" : "";
			ViewData["TitleSortParm"] = sortOrder == "title" ? "title_desc" : "title";
			ViewData["UpdatedSortParm"] = sortOrder == "date" ? "date_desc" : "date";

			ViewData["PriceSortParm"] = sortOrder == "price" ? "price_desc" : "price";
			ViewData["AuthorSortParm"] = sortOrder == "username" ? "username_desc" : "username";
			if (searchString != null)
			{
				pageNumber = 1;
			}
			else
			{
				searchString = currentFilter;
			}
			ViewData["CurrentFilter"] = searchString;
			var list = from s in repository.Movies
					   select s;
			// only list the recipes of the login users unless it is admin
			//if (User.Identity.IsAuthenticated && !User.IsInRole("Admin")) {
			//	list = list.Where(r => r.UserName == User.Identity.Name);
			//}

			if (!String.IsNullOrEmpty(searchString))
			{
				list = list.Where(s => s.Title.Contains(searchString));
			}
			switch (sortOrder)
			{
				case "id_desc":
					list = list.OrderByDescending(s => s.ID);
					break;
				case "title":
					list = list.OrderBy(s => s.Title);
					break;
				case "title_desc":
					list = list.OrderByDescending(s => s.Title);
					break;
				case "date":
					list = list.OrderBy(s => s.Updated);
					break;
				case "date_desc":
					list = list.OrderByDescending(s => s.Updated);
					break;
				case "price":
					list = list.OrderBy(s => s.Price);
					break;
				case "price_desc":
					list = list.OrderByDescending(s => s.Price);
					break;
				case "username":
					list = list.OrderBy(s => s.UserName);
					break;
				case "username_desc":
					list = list.OrderByDescending(s => s.UserName);
					break;
				default:
					list = list.OrderBy(s => s.ID);
					break;
			}

			var data = await PaginatedList<Movie>.CreateAsync(list, pageNumber ?? 1, PageSize);
			/*	foreach (var p in data)
				{
					p.Title = repository.GetMovie(p.ID);
				}*/
			return View(data);
		}

		// GET: Movie
		[AllowAnonymous]
		public ActionResult Index()
		{
			return RedirectToAction("List");
			//return View();
		}
		public ViewResult Add()
		{
			ViewBag.Message = "Add a movie to the movie list. ";			
			Movie formdata = new Movie();
			formdata.UserName = User.Identity.Name;
			formdata.ReleaseDate = DateTime.UtcNow;
			return View(formdata);
		}

		//POST /Movies/Add
		[HttpPost]
		public ActionResult Add(Movie formdata)
		{
			if (ModelState.IsValid)
			{
				formdata.ID = 0;
				formdata.UserName = User.Identity.Name;
				repository.SaveMovie(formdata);
				TempData["message"] = "You have added a new Movie [" + formdata.Title + "] Successfully! ";
				return RedirectToAction("List");
			}
			else
			{
				//if there is something wrong with the data values				
				return View(formdata);
			}
		}

		/*	private void PopulateCategoryDropDownList(object selectedCategory = null)
			{
				ViewBag.CategoryID = new SelectList(this.repository.Categories, "ID", "Name", selectedCategory);
			}*/

		//GET Update /Movie/Update/{ID}
		public ActionResult Update(int ID)
		{
			Movie found = repository.GetMovie(ID);
			if (!User.IsInRole("Admin") && User.Identity.Name != found.UserName)
			{
				TempData["message"] = "!!!You are not the owner, you can't Update the Movie!";
				return RedirectToAction(nameof(Details), new { id = found.ID });
			}
			ViewBag.Message = "Update Movie";
			//		this.PopulateCategoryDropDownList(found.CategoryID);
			return View(found);
		}

		//POST Update /Movie/Update/{ID}
		[HttpPost]
		public ActionResult Update(Movie formdata)
		{
			if (ModelState.IsValid)
			{
				if (!User.IsInRole("Admin") && User.Identity.Name != formdata.UserName)
				{
					TempData["message"] = "!!!You are not the owner, you can't Update the Movie!";
					return RedirectToAction(nameof(Details), new { id = formdata.ID });
				}
				repository.SaveMovie(formdata);
				TempData["message"] = "You have Updated the Movie [" + formdata.Title + "] information Successfully! ";
				return RedirectToAction("List");
			}
			else
			{
				//if there is something wrong with the data values
				//		this.PopulateCategoryDropDownList(formdata.CategoryID);
				return View(formdata);
			}
		}

		//GET delete /Movie/Delete/{ID} confirm page
		[HttpGet]
		public ActionResult Delete(int ID)
		{
			Movie found = repository.GetMovie(ID);
			if (!User.IsInRole("Admin") && User.Identity.Name != found.UserName)
			{
				TempData["message"] = "!!!You are not the owner, you can't Delete the Movie!";
				return RedirectToAction(nameof(Details), new { id = found.ID });
			}
			ViewBag.Message = "Are you sure want to delete the movie [" + found.Title + "]  ?";
			return View(found);
		}

		//POST delete /movie/Delete/{ID}
		[HttpPost, ActionName("Delete")]
		public ActionResult DeleteConfirmed(int ID)
		{
			Movie found = repository.DeleteMovie(ID);
			if (found != null)
			{
				TempData["message"] = "Movie [" + found.Title + "] was deleted successfully.";
			}
			else
			{
				TempData["message"] = "Failed to delete the movie [" + ID + "] as it is not existed. Movie id:";
			}
			return RedirectToAction("List");
		}

		// Get summary /Movie/Details/{ID}
		[AllowAnonymous]
		public async Task<ViewResult> Details(int ID)
		{
			Movie found = repository.GetMovie(ID);
			if (found == null)
			{
				ViewBag.errorMessage = string.Format("Movie {0} not found", ID);
			}
			found.MovieReviews = await dynamoSvc.GetMovieReviews(ID);
			ViewBag.Message = "Movie Summary";
			return View(found);
		}


		//Get: Add Review to Movie page /Movie/AddReview/{MovieID}
		public ViewResult AddReview(int ID)
		{
			Movie m = repository.GetMovie(ID);
			ViewBag.Message = "Add a Review to the movie [" + m.Title + "]";
			MovieReview found = new MovieReview();
			found.MovieID = ID;
			found.Movie = m;
			found.ID = null;
			return View(found);
		}

		//POST: Add Review to Movie page /Movie/AddReview/{MovieID}
		[HttpPost]
		public async Task<IActionResult> AddReview(MovieReview formdata)
		{
			if (ModelState.IsValid)
			{
				MovieReview found = await dynamoSvc.SaveMovieReview(formdata);
				found.Movie = repository.GetMovie(found.MovieID);
				TempData["message"] = "A Review from " + formdata.Email + " has been added to movie [" + found.Movie.Title + "].";
				return RedirectToAction("Details", new { id = found.MovieID });
			}
			else
			{
				var errorList = (from item in ModelState
								 where item.Value.Errors.Any()
								 select item.Value.Errors[0].ErrorMessage).ToList();
				_logger.LogError("Failed to add movie review: " + errorList.ToString());
				formdata.Movie = repository.GetMovie(formdata.MovieID);
				TempData["message"] = "Failed to add a Review from " + formdata.Email + " to movie [" + formdata.Movie.Title + "]: " + errorList.ToString();
				return View(formdata);
			}
		}

		//Get: Update Review to Movie page /Movie/UpdateReview/{MovieReviewID}
		public async Task<ViewResult> UpdateReview(String ID)
		{
			MovieReview found = await dynamoSvc.GetMovieReview(ID);
			found.Movie = repository.GetMovie(found.MovieID);
			ViewBag.Message = "Update Review of [" + found.Email + "] on movie [" + found.Movie.Title + "]";
			return View(found);
		}

		//POST: Update Review Movie page /MovieUpdateReview/{MovieReviewID}
		[HttpPost]
		public async Task<IActionResult> UpdateReview(MovieReview formdata)
		{
			if (ModelState.IsValid)
			{
				MovieReview found = await dynamoSvc.UpdateMovieReview(formdata);
				found.Movie = repository.GetMovie(found.MovieID);
				TempData["message"] = "A Review of [" + found.Email + "] has been updated to movie [" + found.Movie.Title + "] successfully.";
				return RedirectToAction(nameof(Details), new { id = found.MovieID });
			}
			else
			{
				var errorList = (from item in ModelState
								 where item.Value.Errors.Any()
								 select item.Value.Errors[0].ErrorMessage).ToList();
				_logger.LogError("Failed to update movie review: " + errorList.ToString());
				formdata.Movie = repository.GetMovie(formdata.MovieID);
				TempData["message"] = "Failed to update a Review from " + formdata.Email + " to movie [" + formdata.Movie.Title + "]: " + errorList.ToString();
				//if there is something wrong with the data values
				return View(formdata);
			}
		}

		//Get: Delete Review from Movie page /Movie/DeleteReview/{MovieReviewID}
		[HttpGet]
		public async Task<ViewResult> DeleteReview(String ID)
		{
			MovieReview found = await dynamoSvc.GetMovieReview(ID);
			found.Movie = repository.GetMovie(found.MovieID);
			ViewBag.Message = "Are you sure want to remove the review of [" + found.Email + "] from the movie [" + found.Movie.Title + "] ?";
			return View(found);
		}

		//POST: Delete Review from Movie page /Movie/DeleteReview/{MovieReviewID}
		[HttpPost, ActionName("DeleteReview")]
		public async Task<ActionResult> DeleteReviewConfirmed(String ID)
		{
			MovieReview found = await dynamoSvc.GetMovieReview(ID);
			found.Movie = repository.GetMovie(found.MovieID);
			await dynamoSvc.DeleteMovieReview(ID);
			TempData["message"] = "The Review of [" + found.Email + "] has been removed from movie list [" + found.Movie.Title + "].";
			return RedirectToAction(nameof(Details), new { id = found.MovieID });
		}

		//GET Update /Movie/Upload/{ID}
		public ActionResult Upload(int ID)
		{
			Movie found = repository.GetMovie(ID);
			if (!User.IsInRole("Admin") && User.Identity.Name != found.UserName)
			{
				TempData["message"] = "!!!You are not the owner, you can't Upload file for this movie!";
				return RedirectToAction(nameof(Details), new { id = found.ID });
			}
			ViewBag.Message = "Upload file for the movie";
			//		this.PopulateCategoryDropDownList(found.CategoryID);
			return View(found);
		}

		//POST Update /Movie/Upload/{ID}
		[HttpPost]
		public async Task<ActionResult> Upload(int ID, IFormFile file)
		{
			Movie found = repository.GetMovie(ID);

			if (!User.IsInRole("Admin") && User.Identity.Name != found.UserName)
			{
				TempData["message"] = "!!!You are not the owner, you can't Upload file for this Movie!";
				return RedirectToAction(nameof(Details), new { id = found.ID });
			}

			var fileTransferUtility = new TransferUtility(s3Client);
			Boolean succeed = true;
			try
			{
				if (file.Length > 0)
				{
					String keyName = file.FileName;
					var filePath = Path.GetTempFileName();
					using (var stream = System.IO.File.Create(filePath))
					{
						await file.CopyToAsync(stream);
					}
					var fileTransferUtilityRequest = new TransferUtilityUploadRequest
					{
						BucketName = bucketName,
						FilePath = filePath,
						StorageClass = S3StorageClass.StandardInfrequentAccess,
						PartSize = 6291456, // 6 MB.  
						Key = keyName,
						CannedACL = S3CannedACL.PublicRead
					};
					fileTransferUtilityRequest.Metadata.Add("MovieID", found.ID.ToString());
					fileTransferUtilityRequest.Metadata.Add("MovieTitle", found.Title);
					fileTransferUtilityRequest.Metadata.Add("MovieOwner", found.UserName);
					fileTransferUtilityRequest.Metadata.Add("Type", "Movie");
					await fileTransferUtility.UploadAsync(fileTransferUtilityRequest);
					//fileTransferUtility.Dispose();

					//update the upload file name into movie table
					found.Path = keyName;
					repository.SaveMovie(found);
				}
				TempData["message"] = "You have Uploaded a file for the Movie [" + found.Title + "] to S3! ";

			}

			catch (AmazonS3Exception amazonS3Exception)
			{
				if (amazonS3Exception.ErrorCode != null &&
					(amazonS3Exception.ErrorCode.Equals("InvalidAccessKeyId")
					||
					amazonS3Exception.ErrorCode.Equals("InvalidSecurity")))
				{
					TempData["message"] = "Check the provided AWS Credentials.";
				}
				else
				{
					TempData["message"] = "Error occurred: " + amazonS3Exception.Message;
				}
				ViewBag.Message = "Upload file for the movie";
				succeed = false;
			}
			if (succeed)
			{
				return RedirectToAction("List");
			}
			else
			{
				return View(found);
			}

		}

		//GET Update /Movie/Download/{ID}
		[AllowAnonymous]
		public IActionResult Download(int ID)
		{
			Movie found = repository.GetMovie(ID);
			String keyName = found.Path;
			if (keyName == null || keyName == "")
			{
				TempData["message"] = "Error occurred: no movie file to download yet";
				return RedirectToAction("List");
			}

			GetPreSignedUrlRequest request1 =
			   new GetPreSignedUrlRequest()
			   {
				   BucketName = bucketName,
				   Key = keyName,
				   Expires = DateTime.Now.AddMinutes(30)
			   };

			string url = s3Client.GetPreSignedURL(request1);
			return Redirect(url);
		}

	}
}