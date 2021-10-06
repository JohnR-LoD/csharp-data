using System.Threading.Tasks;
using System.Collections.Generic;
using System;
using System.IO;
using System.Text.Json;
using System.Linq;
using MongoDB.Driver;

namespace labfiles
{
    delegate (int, string) TestFunction();

    internal class TestMongo
    {
        internal const string dataPath = "d:\\labfiles\\orders";
        internal const string host = "localhost";
        internal const string port = "27017";
        internal const string database = "classicmodels";
        internal const string collectionName = "orders";
        const int customerNumber = 471;

        //Test setup

        const int newCustomerNumber = 1000000;
        static CustomerOrder newCustomerOrder
        {
            get
            {
                var newCO = new CustomerOrder
                {
                    _id = newCustomerNumber.ToString(),
                    customerNumber = newCustomerNumber

                };
                var order = new Order
                {
                    customerNumber = newCustomerNumber,
                    status = "Shipped",
                    orderNumber = 1000000,
                    orderDate = DateTime.Parse("2021-05-20"),
                    requiredDate = DateTime.Parse("2021-05-20"),
                    shippedDate = DateTime.Parse("2021-05-20"),
                };
                order.details.Add(new OrderDetail
                {
                    orderLineNumber = 1,
                    productName = "1966 Shelby Cobra 427 S/C",
                    productCode = "S24_1628",
                    priceEach = 43.27M,
                    quantityOrdered = 50,
                    lineTotal = 2163.50M
                });
                newCO.orders.Add(order);
                return newCO;
            }
        }
        static Order newOrder
        {
            get
            {
                var order = new Order
                {
                    customerNumber = newCustomerNumber,
                    status = "Shipped",
                    orderNumber = 1000000,
                    orderDate = DateTime.Parse("2021-05-20"),
                    requiredDate = DateTime.Parse("2021-05-20"),
                    shippedDate = DateTime.Parse("2021-05-20"),
                };
                order.details.Add(new OrderDetail
                {
                    orderLineNumber = 1,
                    productName = "1966 Shelby Cobra 427 S/C",
                    productCode = "S24_1628",
                    priceEach = 43.27M,
                    quantityOrdered = 50,
                    lineTotal = 2163.50M
                });
                return order;
            }
        }

        private static void deleteTestDocument(IMongoCollection<CustomerOrder> collection) {
            collection.DeleteOne<CustomerOrder>(co => co._id == newCustomerNumber.ToString());
        }
        private static CustomerOrder getTestDocument(IMongoCollection<CustomerOrder> collection) {
            return collection.Find<CustomerOrder>(co => co._id == newCustomerNumber.ToString()).FirstOrDefault();
        }

        internal static void clearDocuments(IMongoCollection<CustomerOrder> collection)
        {
            collection.DeleteMany<CustomerOrder>(co => co._id != "deleteme");
        }

        internal static int RunTest(int testNumber, bool showDisplay)
        {
            int result = 0;
            var testTitle = "";
            var display = "";
            var tests = new List<(string, TestFunction)>();
            tests.Add(("testGetCollection", TestGetCollection));
            tests.Add(("testLoad", TestLoad));
            tests.Add(("testGetCustomerOrders", TestGetustomerOrders));
            tests.Add(("testGetProductOrders", TestGetProductOrders));
            tests.Add(("testInsert", TestInsert));
            tests.Add(("testUpdate", TestUpdate));
            tests.Add(("testDelete", TestDelete));
            tests.Add(("testNoResult", TestNoDocument));
            tests.Add(("testInsertException", TestInsertError));



            try
            {
                testTitle = tests[testNumber].Item1;
                (result, display) = tests[testNumber].Item2();
                if (showDisplay) Console.WriteLine($"{testTitle}:\t{display}");
            }
            catch (Exception ex)
            {
                result = -100;
                if (showDisplay) Console.WriteLine($"{testTitle}:\tResult = ERROR - {ex.Message}");
            }
            return result;
        }
        static (int, string) TestGetCollection()
        {
            int result = 0;
            string display = "";
            var testObject = new MongoCode();
            var collection = testObject.GetCollection(host, port, database, TestMongo.collectionName);
            if (collection.CollectionNamespace.CollectionName == TestMongo.collectionName)
            {
                result = 0;
                display = "You have successfully connected to the MongoDB collection.";
            }
            else
            {
                result = -1;
                display = "You have not connected to the MongoDB collection.";
            }
            return (result, display);
        }
        static (int, string) TestLoad()
        {
            int result = 0;
            string display = "";
            var testObject = new MongoCode();
            var collection = testObject.GetCollection(host, port, database, TestMongo.collectionName);
            clearDocuments(collection);
            var documents = testObject.LoadData(collection, dataPath);
            if (documents == 0)
            {
                result = -1;
                display = "You did not load any documents.";

            }
            else if (documents != 98)
            {
                result = -1;
                display = $"You loaded the wrong number of documents. You loaded {documents} documents, 98 were expected.";
            }
            else
            {
                result = 0;
                display = "You successfully loaded the documents into MongoDB.";
            }

            return (result, display);

        }
        static (int, string) TestGetustomerOrders()
        {
            int result = 0;
            string display = "";
            var testObject = new MongoCode();
            var collection = testObject.GetCollection(host, port, database, TestMongo.collectionName);
            var customerOrder = testObject.getCustomerOrders(collection, customerNumber);
            if (customerOrder == null)
            {
                result = -1;
                display = "You did not return a customer order document.";
            }
            else if ((customerOrder.orders.Count != 3) || (customerOrder.orders[1].details[0].productCode != "S18_3482"))
            {
                result = -1;
                display = "You did not return the correct customer order document.";
            }
            else
            {
                result = 0;
                display = $"You have successfully returned a customer orders document. {Program.SaveResults("customer", customerOrder)}";

            }
            //Console.WriteLine(customerOrder.orders.Count);
            //Console.WriteLine(customerOrder.orders[1].details[0].productCode);
            return (result, display);
        }
        static (int, string) TestGetProductOrders()
        {
            string testCode = "S18_2957";
            int result = 0;
            string display = "";
            var testObject = new MongoCode();
            var collection = testObject.GetCollection(host, port, database, TestMongo.collectionName);
            var documents = testObject.getProductOrders(collection, testCode);
            if ((documents == null) || (documents.Count == 0))
            {
                result = -1;
                display = "You did not return any product order documents.";
            }
            else if ((documents.Count != 27) || (documents.OrderBy(d => d.customerNumber).Last().customerNumber != 475))
            {
                result = -1;
                display = "You did not return the correct set of documents.";
            }
            else
            {
                result = 0;
                display = $"You have successfully returned the customer order documents for the product code. {Program.SaveResults("product", documents.Take(3))} Note that the file only contains 3 of 27 documents for productCode {testCode}.";
            }

            return (result, display);
        }
        static (int, string) TestInsert(){
            int result = 0;
            string display = "";
            var testObject = new MongoCode();
            var collection = testObject.GetCollection(host, port, database, TestMongo.collectionName);
            testObject.insertCustomer(collection,newCustomerOrder);
            var verify = getTestDocument(collection);

            if(verify == null) {
                result = -1;
                display = "You did not insert a customer orders document.";
            } else if (verify.customerNumber!=newCustomerNumber) {
                result = -1;
                display = "You did not insert the correct customer order data.";
            } else {
                result = 0;
                display = $"You have successfully inserted a customer order document.{Program.SaveResults("insert",verify)}";
            }

            return (result, display);
        }
        static (int, string) TestUpdate(){
            int result = 0;
            string display = "";
            var testObject = new MongoCode();
            var collection = testObject.GetCollection(host, port, database, TestMongo.collectionName);
            testObject.updateCustomer(collection,newCustomerNumber, newOrder);
            var verify = getTestDocument(collection);

            if(verify == null) {
                result = -1;
                display = "The customer order document does not exist.";
            } else if (verify.orders.Count!=2) {
                result = -1;
                display = "You did not correctly update the customer order document.";
            } else {
                result = 0;
                display = $"You have successfully updated a customer order document.{Program.SaveResults("update",verify)}";
            }

            return (result,display);

        }
        static (int, string) TestDelete(){
            int result = 0;
            string display = "You have deleted a document from the collection";
            var testObject = new MongoCode();
            var collection = testObject.GetCollection(host, port, database, TestMongo.collectionName);
            testObject.deleteCustomer(collection, newCustomerNumber);
            var verify = getTestDocument(collection);
            if(verify!=null) {
                result = -1;
                display = "You have not deleted the document from the collection.";
            }
            return (result,display);
        }
       static (int, string) TestNoDocument(){
            int result = 0;
            string display = "";
            var testObject = new MongoCode();
            var collection = testObject.GetCollection(host, port, database, TestMongo.collectionName);
            var verify = testObject.getCustomerOrders(collection,newCustomerNumber+100);
            if(verify==null) {
                result = 0;
                display = "You have successfully handled an empty result from a MongoDB query.";
            } else {
                result = -1;
                display = "You have retrieved a customer orders document in error. This should be a null";
            }
            return (result,display);
        }
        static (int, string) TestInsertError(){
            int result = 0;
            string display = "";
            var testObject = new MongoCode();
            var collection = testObject.GetCollection(host, port, database, TestMongo.collectionName);
            testObject.insertCustomer(collection,newCustomerOrder);
            testObject.insertCustomer(collection,newCustomerOrder);
            var verify = getTestDocument(collection);
            if(verify == null) {
                result = -1;
                display = "You did not insert a customer orders document.";
            } else {
                result = 0;
                display = $"You have successfully handled the xxx exception.";
            }

            return (result, display);
        }
    }
    
}
