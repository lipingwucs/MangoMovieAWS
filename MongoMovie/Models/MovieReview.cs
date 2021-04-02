using Amazon.DynamoDBv2.DataModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MongoMovie.Models
{
	[DynamoDBTable("MovieReview")]
	public class MovieReview : BaseEntity
	{
		//[Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)] //this line
		[DynamoDBProperty("id")]
		[DynamoDBHashKey]
		public string ID { get; set; }  //MovieReview Id

		[DynamoDBProperty("MovieID")]
		public int MovieID { get; set; } //connect to movie
		
		public Movie Movie { get; set; }

		[DynamoDBProperty("FirstName")]
		public string FirstName { get; set; }
		[DynamoDBProperty("LastName")]
		public string LastName { get; set; }
		[DynamoDBProperty("Email")]
		public string Email { get; set; }
		[DynamoDBProperty("Telephone")]
		public string Telephone { get; set; }
		[Required]
		[DataType(DataType.MultilineText)]
		[DynamoDBProperty("Message")]
		public string Message { get; set; }
	}


}