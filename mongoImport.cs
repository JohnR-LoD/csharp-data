using MongoDB.Driver;
using System.IO;
using System.Collections.Generic;
using System.Text.Json;

namespace labfiles
{
    class MongoOrders
    {
        public IMongoCollection<CustomerOrder> GetCollection(string host, string port, string database, string collection)
        {
            string connectionstring = $"mongodb://{host}:{port}";
            MongoClient client = new MongoClient(connectionstring);
            IMongoDatabase db = client.GetDatabase(database);
            return db.GetCollection<CustomerOrder>(collection);
        }


        public int ImportDocuments(IMongoCollection<CustomerOrder> collection, string dataPath)
        {
            try
            {
                var customerOrders = new List<CustomerOrder>();
                foreach (var fileName in Directory.GetFiles(dataPath))
                {
                    var fileContent = File.ReadAllText(fileName);
                    var customer = JsonSerializer.Deserialize<CustomerOrder>(fileContent);
                    customer._id = customer.customerNumber.ToString();
                    customerOrders.Add(customer);
                }
                collection.InsertMany(customerOrders);
                return (int)collection.CountDocuments<CustomerOrder>(co => true);
            }
            catch (MongoDB.Driver.MongoWriteException)
            {
                return -1;
            }
            catch
            {
                return -2;
            }
        }
        public void updateCustomer(IMongoCollection<CustomerOrder> collection, int customerNumber, Order newOrder) {
            var customerOrders = collection.Find<CustomerOrder>(co => co._id==customerNumber.ToString()).First();
            customerOrders.orders.Add(newOrder);
            collection.ReplaceOne<CustomerOrder>(co => co._id == customerNumber.ToString(),customerOrders);
        }


    //End class
    }
}