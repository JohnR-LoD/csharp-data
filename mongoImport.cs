using System;
using System.Collections.Generic;
using MongoDB.Driver;

namespace labfiles
{
    class MongoOrders
    {
        public IMongoCollection<CustomerOrder> getCollection(string host, string port, string database, string collection)
        {
            throw new NotImplementedException();
        }


        public int importDocuments(IMongoCollection<CustomerOrder> collection, string dataPath)
        {
            throw new NotImplementedException();
        }
        public void updateCustomer(IMongoCollection<CustomerOrder> collection, int customerNumber, Order newOrder) {
            throw new NotImplementedException();
        }


    //End class
    }
}