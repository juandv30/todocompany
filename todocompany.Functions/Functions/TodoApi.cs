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
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "company")] HttpRequest req,
            [Table("company", Connection = "AzureWebJobsStorage")] CloudTable companyTable,
            ILogger log)
        {
            log.LogInformation("recieved process for consolidated");

            string name = req.Query["name"];

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            Entry entry = JsonConvert.DeserializeObject<Entry>(requestBody);

            if (string.IsNullOrEmpty(entry?.TaskDescription))
            {
                return new BadRequestObjectResult(new Response
                {
                    IsSuccess = false,
                    Message = "The request must have a TaskDescription."
                });
            }
            TodoEntity todoEntity = new TodoEntity
            {
                CreatedTime = DateTime.UtcNow,
                ETag = "*",
                IsConsolidated = false,
                PartitionKey = "ENTRY",
                RowKey = Guid.NewGuid().ToString(),
                TaskDescription = entry.TaskDescription
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
    }
}