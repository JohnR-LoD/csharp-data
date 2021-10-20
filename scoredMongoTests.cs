using System.Collections.Generic;
using System;
using MongoDB.Driver;
using System.Linq;
using System.Text.Json;

namespace labfiles
{
    class TestMongo
    {
        private delegate (int, string) TestFunction();

        private static void clearDocuments(IMongoCollection<CustomerOrder> collection)
        {
            collection.DeleteMany<CustomerOrder>(co => co._id != "deleteme");

        }

        private static List<CustomerOrder> GetOrders(IMongoCollection<CustomerOrder> collection)
        {
            return collection.Find(co => co._id != "dm").ToList();
        }

        internal static int RunTest(int testNumber, bool showDisplay)
        {
            int result = 0;
            var testTitle = "";
            var display = "";
            var tests = new List<(string, TestFunction)>();
            tests.Add(("getCollection", TestMongoCollection));
            tests.Add(("loadDocuments", TestLoad));
            try
            {
                testTitle = tests[testNumber].Item1;
                (result, display) = tests[testNumber].Item2();
                if (showDisplay) Console.WriteLine($"{testTitle}:\t{display}");
            }
            catch (MySqlConnector.MySqlException exSql)
            {
                result = -100;
                if (showDisplay) Console.WriteLine($"{testTitle}:\tMySQL Exception: {exSql.ErrorCode}");

            }
            catch (Exception ex)
            {
                result = -100;
                if (showDisplay) Console.WriteLine($"{testTitle}:\tResult = ERROR - {ex.Message}");
            }

            return result;
        }

        public static (int, string) TestMongoCollection()
        {
            int result = 0;
            string display = "";
            var testObject = new MongoOrders();
            var collection = testObject.getCollection(Settings.host, Settings.mongoPort.ToString(), Settings.database, Settings.collectionName);
            if (collection.CollectionNamespace.CollectionName == Settings.collectionName)
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
            var testObject = new MongoOrders();
            var collection = testObject.getCollection(Settings.host, Settings.mongoPort.ToString(), Settings.database, Settings.collectionName);
            clearDocuments(collection);
            var documents = testObject.importDocuments(collection, Settings.dataPath);
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
                var co = GetOrders(collection).FirstOrDefault(co => co._id == "172");
                if ((co == null) || (co.orders[0].details.FirstOrDefault(d => d.productCode == "S10_4962") == null))
                {
                    result = -1;
                    display = "You did not load the correct documents into MongoDB.";
                }
                else
                {
                    result = 0;
                    display = "You successfully loaded the documents into MongoDB.";
                }
            }

            return (result, display);

        }
        //End class
    }
}