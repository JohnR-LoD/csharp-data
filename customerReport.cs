using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using MongoDB.Driver;
using MySqlConnector;
using System.Text.Json;
using System.IO;

namespace labfiles
{
    public class customerOrdersReport
    {
        public async Task<MySqlConnection> getConnection(string host, string port, string database, string user, string password)
        {
            await Task.CompletedTask;
            throw new NotImplementedException();
        }
        public IMongoCollection<CustomerOrder> GetCollection(string host, string port, string database, string collection)
        {
            throw new NotImplementedException();
        }

        private async Task<CustomerContact> getCustomerContact(MySqlConnection connection, int customerNumber)
        {
            await Task.CompletedTask;
            throw new NotImplementedException();

        }

        private List<Order> getCustomerOrders(IMongoCollection<CustomerOrder> collection, int customerNumber)
        {
            throw new NotImplementedException();
        }

        public async Task<CustomerReport> GetCustomerReport(MySqlConnection connection, IMongoCollection<CustomerOrder> collection, int customerNumber)
        {
            await Task.CompletedTask;
            throw new NotImplementedException();

        }



        //Class end
    }

}
