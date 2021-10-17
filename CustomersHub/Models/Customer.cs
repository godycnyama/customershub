using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Azure.Cosmos.Table;

namespace CustomersHub.Models
{
    public class Customer: TableEntity
    {
        //PartionKey is "Customer"
        //RowKey is email
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string EmailAddress { get; set; }
        public string MobilePhone { get; set; }
    }
}
