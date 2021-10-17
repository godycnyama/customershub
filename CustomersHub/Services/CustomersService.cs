using CustomersHub.Models;
using Microsoft.Azure.Cosmos.Table;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CustomersHub.Services
{
    public class CustomersService: ICustomersService
    {
        //create customer record
        public async Task<MessageResponse> CreateCustomer(CloudTable customerTable, CustomerInput customerInput)
        {
            Customer customer = new Customer
            {
                PartitionKey = "Customers",
                RowKey = customerInput.EmailAddress,
                FirstName = customerInput.FirstName,
                LastName = customerInput.LastName,
                EmailAddress = customerInput.EmailAddress,
                MobilePhone = customerInput.MobilePhone
            };

            TableOperation insertOperation = TableOperation.InsertOrMerge(customer);

            try
            {
                await customerTable.ExecuteAsync(insertOperation);
                return new MessageResponse
                {
                    Message = "Customer created successfully!"
                };
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        //update customer record
        public async Task<MessageResponse> UpdateCustomer(CloudTable customerTable, string email,  CustomerInput customerInput)
        {
            string partitionKey = "Customers";
            TableOperation retrieveOperation = TableOperation.Retrieve<Customer>(partitionKey, email);

            try
            {
                TableResult retrievedResult = await customerTable.ExecuteAsync(retrieveOperation);
                if(retrievedResult.Result == null)
                {
                    throw new Exception("Customer record not found!");
                }
                Customer customer = retrievedResult.Result as Customer;
                customer.RowKey = customerInput.EmailAddress;
                customer.FirstName = customerInput.FirstName;
                customer.LastName = customerInput.LastName;
                customer.EmailAddress = customerInput.EmailAddress;
                customer.MobilePhone = customerInput.MobilePhone;

                TableOperation updateOperation = TableOperation.Replace(customer);
                await customerTable.ExecuteAsync(updateOperation);
                return new MessageResponse
                {
                    Message = "Customer record updated successfully!"
                };
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        //delete customer record
        public async Task<MessageResponse> DeleteCustomer(CloudTable customerTable, string partitionKey, string email)
        {
            TableOperation retrieveOperation = TableOperation.Retrieve<Customer>(partitionKey, email);

            try
            {
                TableResult retrievedResult = await customerTable.ExecuteAsync(retrieveOperation);
                if (retrievedResult.Result == null)
                {
                    throw new Exception("Customer record not found!");
                }
                
                TableOperation deleteOperation = TableOperation.Delete(retrievedResult.Result as Customer);
                await customerTable.ExecuteAsync(deleteOperation);
                return new MessageResponse
                {
                    Message = "Customer record deleted successfully!"
                };
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        //get customer record
        public async Task<Customer> GetCustomer(CloudTable customerTable, string partitionKey, string email)
        {
            TableOperation retrieveOperation = TableOperation.Retrieve<Customer>(partitionKey, email);

            try
            {
                TableResult retrievedResult = await customerTable.ExecuteAsync(retrieveOperation);
                if (retrievedResult.Result == null)
                {
                    return null;
                }
                return retrievedResult.Result as Customer;
                
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        //get customer record
        public async Task<List<Customer>> GetCustomers(CloudTable customerTable, string partitionKey)
        {
            var condition = TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, partitionKey);
            var query = new TableQuery<Customer>().Where(condition);
            TableQuerySegment<Customer> querySegment = null;
            List<Customer> customers = new List<Customer>();
            try
            {
                do
                {
                    querySegment = await customerTable.ExecuteQuerySegmentedAsync(query, querySegment?.ContinuationToken);
                    customers.AddRange(querySegment.Results);
                } while (querySegment.ContinuationToken != null);
                return customers;
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }
}
