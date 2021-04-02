using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MongoMovie.Models
{	
	public class Movie : BaseEntity
	{ 
		public int ID { get; set; } //movie id
		public string UserName { get; set; } //FK the registered login name
		public string Title { get; set; } //movie title
		[DataType(DataType.Date)]
		public DateTime ReleaseDate { get; set; }
		public string Genre { get; set; } // movie type: action, love, Comedy, Western
		public decimal Price { get; set; } // watch price
		public string Path { get; set; }
		public ICollection<MovieReview> MovieReviews { get; set; } // movie-review one-many
		public Dictionary<string, string> Dict
		{
			get
			{
				return this.formfields;
			}
		}
		private Dictionary<string, string> formfields;
		private void initDict()
		{
			this.formfields = new Dictionary<string, string>();
			
		}
		//movie constructor	
	
		public Movie() {
			this.initDict(); 
		}
	}//end of Movie class	
}