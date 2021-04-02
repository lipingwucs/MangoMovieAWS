using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace MongoMovie.Models
{
    public  class Helpers
    {
        public static string GetRDSConnectionString()
        {
            string dbname = Environment.GetEnvironmentVariable("RDS_DB_NAME");
            // if no system env varibale setup, means not running in aws
            if (string.IsNullOrEmpty(dbname))
            { 
                //return "Server = PAMELAT480\\SQLEXPRESS; Database = mongomoviedb; Trusted_Connection = True; MultipleActiveResultSets = true";
                return "Server=mongosql.c2awwryka6ks.us-east-1.rds.amazonaws.com;Database=mongomoviedb;User Id=admin;Password=123qwertyu;";
                //return "Server = (LocalDB)\\MSSQLLocalDB; Database = MongoMovie; Trusted_Connection = True; MultipleActiveResultSets = true";
                //return "Server=mongosql.c2awwryka6ks.us-east-1.rds.amazonaws.com;Database=mongomoviedb;User Id=admin;Password=123qwertyu;";
                //return "server=mongomysql.c2awwryka6ks.us-east-1.rds.amazonaws.com;database=mongomoviedb;user=admin;password=123qwertyu";
            }
            string username = Environment.GetEnvironmentVariable("RDS_USERNAME");
            string password = Environment.GetEnvironmentVariable("RDS_PASSWORD");
            string hostname = Environment.GetEnvironmentVariable("RDS_HOSTNAME");
            string port = Environment.GetEnvironmentVariable("RDS_PORT");
            return "server=" + hostname + ";database=" + dbname + ";user=" +
            username + ";password=" + password + ";";
        }

        public static string GetIdentityRDSConnectionString()
        {
            string dbname = Environment.GetEnvironmentVariable("RDS_IDENTDB_NAME");
            // if no system env varibale setup, means not running in aws
            if (string.IsNullOrEmpty(dbname))
            { 
                //return "Server=PAMELAT480\\SQLEXPRESS;Database=MongoIdentity;Trusted_Connection=True;MultipleActiveResultSets=true";
                return "Server=mongosql.c2awwryka6ks.us-east-1.rds.amazonaws.com;Database=MongoIdentity;User Id=admin;Password=123qwertyu;";
                //return "Server=(localdb)\\mssqllocaldb;Database=YummyIdentity;Trusted_Connection=True;MultipleActiveResultSets=true";
                //return "Server=mongosql.c2awwryka6ks.us-east-1.rds.amazonaws.com;Database=MongoIdentity;User Id=admin;Password=123qwertyu;";
                //return "server=mongomysql.c2awwryka6ks.us-east-1.rds.amazonaws.com;database=MongoIdentity;user=admin;password=123qwertyu";
            }
            string username = Environment.GetEnvironmentVariable("RDS_USERNAME");
            string password = Environment.GetEnvironmentVariable("RDS_PASSWORD");
            string hostname = Environment.GetEnvironmentVariable("RDS_HOSTNAME");
            string port = Environment.GetEnvironmentVariable("RDS_PORT");
            return "server=" + hostname + ";database=" + dbname + ";user=" +
            username + ";password=" + password + ";";
        }
    }
}