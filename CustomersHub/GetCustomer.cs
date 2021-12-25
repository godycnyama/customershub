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
using Microsoft.Azure.Cosmos.Table;
using CustomersHub.Models;

namespace CustomersHub
{
    public class GetCustomer
    {
        private readonly ICustomersService _customersService;
        public GetCustomer(ICustomersService customersService)
        {
            this._customersService = customersService;
        }

        [FunctionName("GetCustomer")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "getCustomer")] HttpRequest req, [Table("Customers", Connection = "AzureWebJobsStorage")] CloudTable table,
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
                Customer customer = await _customersService.GetCustomer(table, req.Query["partionKey"], req.Query["email"]);
                if(customer == null)
                {
                    return new BadRequestObjectResult("Customer record not found!");
                }
                return new OkObjectResult(customer);
            }
            catch (Exception ex)
            {
                log.LogInformation(ex.Message);
                return new BadRequestObjectResult(ex.Message);
            }
        }
    }
}
