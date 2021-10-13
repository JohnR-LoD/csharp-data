using System;
using System.IO;
using System.Collections.Generic;
using System.Text.Json;
using MongoDB.Driver;

namespace labfiles
{
    class MongoOrders
    {
        public IMongoCollection<CustomerOrder> GetCollection(string host, string port, string database, string collection)
        {
            throw new NotImplementedException();
        }


        public int ImportDocuments(IMongoCollection<CustomerOrder> collection, string dataPath)
        {
            throw new NotImplementedException();
        }
        public void updateCustomer(IMongoCollection<CustomerOrder> collection, int customerNumber, Order newOrder) {
            throw new NotImplementedException();
        }


    //End class
    }
}