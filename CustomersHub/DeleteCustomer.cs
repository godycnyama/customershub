using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using CustomersHub.Services;
using CustomersHub.Models;
using Microsoft.Azure.Cosmos.Table;

namespace CustomersHub
{
    public class DeleteCustomer
    {
        private readonly ICustomersService _customersService;
        public DeleteCustomer(ICustomersService customersService)
        {
            this._customersService = customersService;
        }

        [FunctionName("DeleteCustomer")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "delete", Route = "deleteCustomer")] HttpRequest req, [Table("Customers", Connection = "AzureWebJobsStorage")] CloudTable table,
            ILogger log)
        {
            if (string.IsNullOrEmpty(req.Query["email"]))
            {
                return new BadRequestObjectResult("Invalid email");
            }

            if (string.IsNullOrEmpty(req.Query["partitionKey"]))
            {
                return new BadRequestObjectResult("Invalid partionKey");
            }
            try
            {
                MessageResponse messageResponse = await _customersService.DeleteCustomer(table, req.Query["partionKey"], req.Query["email"]);
                return new OkObjectResult(messageResponse);
            }
            catch (Exception ex)
            {
                log.LogInformation(ex.Message);
                return new BadRequestObjectResult(ex.Message);
            }
        }
    }
}
