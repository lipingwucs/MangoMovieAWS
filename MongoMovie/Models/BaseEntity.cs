using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MongoMovie.Models
{
	public class BaseEntity
	{
		public System.Nullable<DateTime> Created { get; set; }
		public System.Nullable<DateTime> Updated { get; set; }
	}
}
