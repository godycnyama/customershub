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
    public class CreateCustomer
    {
        private readonly ICustomersService _customersService;
        public CreateCustomer(ICustomersService customersService)
        {
            this._customersService = customersService;
        }

        [FunctionName("CreateCustomer")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "createCustomer")] HttpRequest req, [Table("Customers", Connection = "AzureWebJobsStorage")] CloudTable table,
            ILogger log)
        {
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            CustomerInput customerInput = JsonConvert.DeserializeObject<CustomerInput>(requestBody);
            
            if(customerInput == null)
            {
                return new BadRequestObjectResult("Invalid data");
            }
            try
            {
                MessageResponse messageResponse = await _customersService.CreateCustomer(table, customerInput);
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
