using System.Threading.Tasks;
using System.Collections.Generic;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.DynamoDBv2.DataModel;
using MongoMovie.Models;
using Amazon.DynamoDBv2.Model;
using System.Threading;
using Microsoft.Extensions.Logging;
using System;
using Amazon;

namespace MongoMovie.Services
{
    public class DynamoDBServices: IDynamoDBServices
    {
        IAmazonDynamoDB dynamoDBClient { get; set; }
        private readonly ILogger<DynamoDBServices> _logger;

        private string tableName = "MovieReview";

        public DynamoDBServices(IAmazonDynamoDB dynamoDBClient, ILogger<DynamoDBServices> logger)
        {
          //   this.dynamoDBClient = dynamoDBClient;
            this.dynamoDBClient = new AmazonDynamoDBClient("AKIA5SGL7RIC5OPACEXX", "vi9j5TIIqhDlLD3P1p8QKy0HJ9RUKF5FTxq5+ylu", RegionEndpoint.USEast1); 
            this._logger = logger;

        }

        private async void CreateTable()
        {
            //AmazonDynamoDBClient client = new AmazonDynamoDBClient();
            IAmazonDynamoDB client = dynamoDBClient;
            var tableResponse = await client.ListTablesAsync();
            if (!tableResponse.TableNames.Contains(tableName))
            {
                _logger.LogInformation("Table not found, creating table => " + tableName);
                await client.CreateTableAsync(new CreateTableRequest
                {
                    TableName = tableName,
                    ProvisionedThroughput = new ProvisionedThroughput
                    {
                        ReadCapacityUnits = 3,
                        WriteCapacityUnits = 1
                    },
                    KeySchema = new List<KeySchemaElement>
                    {
                        new KeySchemaElement
                        {
                            AttributeName = "id",
                            KeyType = KeyType.HASH
                        }
                    },
                    AttributeDefinitions = new List<AttributeDefinition>
                    {
                        new AttributeDefinition { AttributeName = "id", AttributeType=ScalarAttributeType.S }
                    }
                });

                bool isTableAvailable = false;
                while (!isTableAvailable)
                {
                    _logger.LogInformation("Waiting for table to be active...");
                    Thread.Sleep(5000);
                    var tableStatus = await client.DescribeTableAsync(tableName);
                    isTableAvailable = tableStatus.Table.TableStatus == "ACTIVE";
                }
                _logger.LogInformation("DynamoDB Table Created Successfully!");
            }
        }

        public async Task<MovieReview> SaveMovieReview(MovieReview movieReview)
        {
            DynamoDBContext context = new DynamoDBContext(dynamoDBClient);
            // Add a unique id for the primary key.
            movieReview.ID = System.Guid.NewGuid().ToString();
            movieReview.Created = DateTime.UtcNow;
            movieReview.Updated = DateTime.UtcNow;
            await context.SaveAsync(movieReview, default(System.Threading.CancellationToken));
            MovieReview newReview = await context.LoadAsync<MovieReview>(movieReview.ID, default(System.Threading.CancellationToken));
            return newReview;
        }

        

        public async Task<MovieReview> UpdateMovieReview(MovieReview movieReview)
        {
            MovieReview found = await GetMovieReview(movieReview.ID);
            found.LastName = movieReview.LastName;
            found.FirstName = movieReview.FirstName;
            found.Email = movieReview.Email;
            found.Telephone = movieReview.Telephone;
            found.Message = movieReview.Message;
            found.Updated = DateTime.UtcNow;
            DynamoDBContext context = new DynamoDBContext(dynamoDBClient);
            await context.SaveAsync(found, default(System.Threading.CancellationToken));
            MovieReview newReview = await context.LoadAsync<MovieReview>(found.ID, default(System.Threading.CancellationToken));
            return newReview;
        }
        public async Task DeleteMovieReview(string Id)
        {
            DynamoDBContext context = new DynamoDBContext(dynamoDBClient);
            await context.DeleteAsync<MovieReview>(Id, default(System.Threading.CancellationToken));
           
        }
        public async Task<MovieReview> GetMovieReview(string Id)
        {
            DynamoDBContext context = new DynamoDBContext(dynamoDBClient);
            MovieReview newReview = await context.LoadAsync<MovieReview>(Id, default(System.Threading.CancellationToken));
            return newReview;
        }

        public async Task<List<MovieReview>> GetMovieReviews(int MovieID)
        {
            CreateTable();

            ScanFilter scanFilter = new ScanFilter();
            scanFilter.AddCondition("MovieID", ScanOperator.Equal, MovieID);

            ScanOperationConfig soc = new ScanOperationConfig()
            {
                  Filter = scanFilter
            };
            DynamoDBContext context = new DynamoDBContext(dynamoDBClient);
            AsyncSearch<MovieReview> search = context.FromScanAsync<MovieReview>(soc, null);
            List<MovieReview> documentList = new List<MovieReview>();
            do
            {
                documentList = await search.GetNextSetAsync(default(System.Threading.CancellationToken));
            } while (!search.IsDone);

            return documentList;
        }
    }
}
