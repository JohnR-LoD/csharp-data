using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using labfiles.Shared;
using MongoDB.Driver;
using MySqlConnector;

namespace labfiles.mysql
{
    public class customerOrdersReport
    {
        public async Task<MySqlConnection> getConnection(string host, string port, string database, string user, string password)
        {
            await Task.CompletedTask;
            throw new NotImplementedException();
        }
        public IMongoCollection<CustomerOrder> getCollection(string host, string port, string database, string collection)
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

        public async Task<CustomerReport> getCustomerReport(MySqlConnection connection, IMongoCollection<CustomerOrder> collection, int customerNumber)
        {
            await Task.CompletedTask;
            throw new NotImplementedException();

        }



        //Class end
    }

}
