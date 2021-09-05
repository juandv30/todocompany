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
            [Table("consolidated", Connection = "AzureWebJobsStorage")] CloudTable consolidatedTable,
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
            await consolidatedTable.ExecuteAsync(addOperation);

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
            [Table("time", Connection = "AzureWebJobsStorage")] CloudTable timeTable,
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
            await timeTable.ExecuteAsync(addOperation);

            string message = "New time consolidate in the table";
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
