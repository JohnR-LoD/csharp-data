using System;
using MongoDB.Driver;

namespace labfiles.mongo
{
    class MongoOrders
    {
        public IMongoCollection<Shared.CustomerOrder> getCollection(string host, string port, string database, string collection)
        {
            throw new NotImplementedException();
        }


        public int importDocuments(IMongoCollection<Shared.CustomerOrder> collection, string dataPath)
        {
            throw new NotImplementedException();
        }
        public void updateCustomer(IMongoCollection<Shared.CustomerOrder> collection, int customerNumber, Order newOrder) {
            throw new NotImplementedException();
        }


    //End class
    }
}