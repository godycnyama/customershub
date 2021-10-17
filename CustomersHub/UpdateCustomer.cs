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
    public class UpdateCustomer
    {
        private readonly ICustomersService _customersService;
        public UpdateCustomer(ICustomersService customersService)
        {
            this._customersService = customersService;
        }

        [FunctionName("UpdateCustomer")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "put", Route = "updateCustomer/{email}")] HttpRequest req, string email, [Table("Customers", Connection = "AzureWebJobsStorage")] CloudTable table,
            ILogger log)
        {
            if (string.IsNullOrEmpty(email))
            {
                return new BadRequestObjectResult("Invalid email");
            }
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            CustomerInput customerInput = JsonConvert.DeserializeObject<CustomerInput>(requestBody);

            if (customerInput == null)
            {
                return new BadRequestObjectResult("Invalid data");
            }
            try
            {
                MessageResponse messageResponse = await _customersService.UpdateCustomer(table, email, customerInput);
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
