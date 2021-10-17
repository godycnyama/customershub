using CustomersHub.Models;
using Microsoft.Azure.Cosmos.Table;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CustomersHub.Services
{
    public interface ICustomersService
    {
        Task<MessageResponse> CreateCustomer(CloudTable customerTable, CustomerInput customer);
        Task<MessageResponse> UpdateCustomer(CloudTable customerTable, string email, CustomerInput customer);
        Task<MessageResponse> DeleteCustomer(CloudTable customerTable, string partitionKey, string email);
        Task<Customer> GetCustomer(CloudTable customerTable, string partitionKey, string email);
        Task<List<Customer>> GetCustomers(CloudTable customerTable, string partitionKey);
    }
}
