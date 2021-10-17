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
using System.Collections.Generic;
using CustomersHub.Models;

namespace CustomersHub
{
    public class GetCustomers
    {
        private readonly ICustomersService _customersService;
        public GetCustomers(ICustomersService customersService)
        {
            this._customersService = customersService;
        }

        [FunctionName("GetCustomers")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "getCustomers")] HttpRequest req, [Table("Customers", Connection = "AzureWebJobsStorage")] CloudTable table,
            ILogger log)
        {
            if (string.IsNullOrEmpty(req.Query["partitionKey"]))
            {
                return new BadRequestObjectResult("Invalid partionKey");
            }
            try
            {
                List<Customer> customers = await _customersService.GetCustomers(table, req.Query["partionKey"]);
                if (customers == null)
                {
                    return new NotFoundObjectResult("No customer records found!");
                }
                return new OkObjectResult(customers);
            }
            catch (Exception ex)
            {
                log.LogInformation(ex.Message);
                return new BadRequestObjectResult(ex.Message);
            }
        }
    }
}
