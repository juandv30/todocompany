using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Threading.Tasks;
using todocompany.Common.Models;
using todocompany.Common.Responses;
using todocompany.Functions.Entities;

namespace todocompany.Functions.Functions
{
    public static class TodoApi
    {
        [FunctionName(nameof(ConsolidateProcess))]
        public static async Task<IActionResult> ConsolidateProcess(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "consolidated")] HttpRequest req,
            [Table("consolidated", Connection = "AzureWebJobsStorage")] CloudTable companyTable,
            ILogger log)
        {
            log.LogInformation("recieved process for consolidated");


            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            Entry entry = JsonConvert.DeserializeObject<Entry>(requestBody);

            if (string.IsNullOrEmpty(entry?.EmployedId))
            {
                return new BadRequestObjectResult(new Response
                {
                    IsSuccess = false,
                    Message = "The request must have a Employed ID."
                });
            }
            TodoEntity todoEntity = new TodoEntity
            {
                Date = DateTime.UtcNow,
                ETag = "*",
               IsConsolidated = false,
                PartitionKey = "ENTRY",
                RowKey = Guid.NewGuid().ToString(),
                Type = entry.Type,
                EmployedId = entry.EmployedId
            };

            TableOperation addOperation = TableOperation.Insert(todoEntity);
            await companyTable.ExecuteAsync(addOperation);

            string message = "New proccess consolidate in the table";
            log.LogInformation(message);


            return new OkObjectResult(new Response
            {
                IsSuccess = true,
                Message = message,
                Result = todoEntity
            });
        }
        
        
        [FunctionName(nameof(CreateTime))]
        public static async Task<IActionResult> CreateTime(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "time")] HttpRequest req,
            [Table("time", Connection = "AzureWebJobsStorage")] CloudTable companyTable,
            ILogger log)
        {
            log.LogInformation("Time recieved");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            Entry entry = JsonConvert.DeserializeObject<Entry>(requestBody);

            if (string.IsNullOrEmpty(entry?.EmployedId))
            {
                return new BadRequestObjectResult(new Response
                {
                    IsSuccess = false,
                    Message = "The request must have a Employed ID."
                });
            }
            TodoEntity todoEntity = new TodoEntity
            {
                Date = DateTime.UtcNow,
                ETag = "*",
                IsConsolidated = false,
                PartitionKey = "ENTRY",
                RowKey = Guid.NewGuid().ToString(),               
                Type = entry.Type,
                EmployedId = entry.EmployedId
            };

            TableOperation addOperation = TableOperation.Insert(todoEntity);
            await companyTable.ExecuteAsync(addOperation);

            string message = "New time consolidate in the table";
            log.LogInformation(message);


            return new OkObjectResult(new Response
            {
                IsSuccess = true,
                Message = message,
                Result = todoEntity
            });
        }


        [FunctionName(nameof(UpdateTime))]
        public static async Task<IActionResult> UpdateTime(
                       [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "time/{id}")] HttpRequest req,
                       [Table("time", Connection = "AzureWebJobsStorage")] CloudTable companyTable,
                       string id,
                       ILogger log)
        {
            log.LogInformation($"Update time employed: {id}, received");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            Entry entry = JsonConvert.DeserializeObject<Entry>(requestBody);

            //validate employed id
            TableOperation findOperation = TableOperation.Retrieve<TodoEntity>("ENTRY", id);
            TableResult findResult = await companyTable.ExecuteAsync(findOperation);
            if (findResult.Result == null)
            {
                return new BadRequestObjectResult(new Response
                {
                    IsSuccess = false,
                    Message = "Employed id not found."
                });
            }

            //update employed time
            TodoEntity todoEntity = (TodoEntity)findResult.Result;
            todoEntity.IsConsolidated = entry.IsConsolidated;
            if (!string.IsNullOrEmpty(entry.EmployedId))
            {
                todoEntity.EmployedId = entry.EmployedId;
            }

            TableOperation addOperation = TableOperation.Replace(todoEntity);
            await companyTable.ExecuteAsync(addOperation);

            string message = $"Entry: {id}, update in the table.";
            log.LogInformation(message);


            return new OkObjectResult(new Response
            {
                IsSuccess = true,
                Message = message,
                Result = todoEntity
            });
        }
    }
}
