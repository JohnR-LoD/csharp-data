using System.Collections.Generic;
using System;

namespace labfiles
{
    class MongoTests
    {
        private delegate (int, string) TestFunction();

        internal static int RunTest(int testNumber, bool showDisplay)
        {
            int result = 0;
            var testTitle = "";
            var display = "";
            var tests = new List<(string, TestFunction)>();
            tests.Add(("getCollection", TestMongoCollection));
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
            var collection = testObject.GetCollection(Settings.host, Settings.mongoPort.ToString(), Settings.database, Settings.collectionName);
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


        //End class
    }
}