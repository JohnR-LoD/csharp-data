using System.Collections.Generic;
using System;
using MongoDB.Driver;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace labfiles
{
    class TestReport
    {
        private delegate (int, string) TestFunction();

        internal static int RunTest(int testNumber, bool showDisplay)
        {
            int result = 0;
            var testTitle = "";
            var display = "";
            var tests = new List<(string, TestFunction)>();
            tests.Add(("contact", TestContact));
            tests.Add(("orders", TestOrders));
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

        public static  (int, string) TestContact()
        {
            int result = 0;
            string display = "";
            var testObject = new customerOrdersReport();
            var connection = testObject.getConnection(Settings.host, Settings.mysqlPort, Settings.database, Settings.user, Settings.password).GetAwaiter().GetResult();
            var collection = testObject.GetCollection(Settings.host, Settings.mongoPort.ToString(), Settings.database, Settings.collectionName);
            var report = testObject.GetCustomerReport(connection, collection, 131).GetAwaiter().GetResult();

            if((report == null) || (report.contact==null)) {
                result = -1;
                display = "You did not return the customer contact data.";
            } else if ((report.contact.phone!="2125557818")) {
                result = -1;
                display = "You did not return the correct contact data.";
            } else {
                result = 0;
                display = $"You returned the correct customer contact data. {Program.SaveResults("contact",report)}";
            }
            return (result, display);
        }

        public static  (int, string) TestOrders()
        {
            int result = 0;
            string display = "";
            var testObject = new customerOrdersReport();
            var connection = testObject.getConnection(Settings.host, Settings.mysqlPort, Settings.database, Settings.user, Settings.password).GetAwaiter().GetResult();
            var collection = testObject.GetCollection(Settings.host, Settings.mongoPort.ToString(), Settings.database, Settings.collectionName);
            var report = testObject.GetCustomerReport(connection, collection, 131).GetAwaiter().GetResult();

            if((report == null) || (report.orders==null)) {
                result = -1;
                display = "You did not return the customer order data.";
            } else if (report.orders.FirstOrDefault(o => o.orderNumber == 10107)==null) {
                result = -1;
                display = "You did not return the correct order data.";
            } else {
                result = 0;
                display = $"You returned the correct customer order data. {Program.SaveResults("contact",report)}";
            }
            return (result, display);
        }

        //End class
    }
}