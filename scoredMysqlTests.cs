using System;
using System.Threading.Tasks;
using System.IO;
using System.Text.Json;
using System.Linq;
using MySqlConnector;
using System.Collections.Generic;


namespace labfiles
{
    public class TestRelational
    {

        #region Test Values
        private static int newOrderNumber = 1000000;
        private static int testCustomerNumber = 240;
        private static int testOrderNumber = 10232;
        private static int testOrderCount = 3;
        private static int testOrderDetailCount = 8;
        private static string testFirstProductCode = "S700_3505";
        private static string testSecondProductCode = "S700_3962";

        private static Order testOrder
        {
            get
            {
                var order = new Order
                {
                    orderNumber = newOrderNumber,
                    orderDate = DateTime.Parse("2020-12-5"),
                    requiredDate = DateTime.Parse("2020-12-15"),
                    shippedDate = DateTime.Parse("2020-12-6"),
                    customerNumber = testCustomerNumber,
                    status = "Shipped"
                };
                order.details.Add(new OrderDetail
                {
                    orderNumber = newOrderNumber,
                    orderLineNumber = 1,
                    productCode = testFirstProductCode,
                    priceEach = 86.15M,
                    quantityOrdered = 48
                });
                order.details.Add(new OrderDetail
                {
                    orderNumber = newOrderNumber,
                    orderLineNumber = 2,
                    productCode = testSecondProductCode,
                    priceEach = 81.43M,
                    quantityOrdered = 35
                });
                return order;
            }
        }
        #endregion


        private static void clearTestOrder(MySqlConnection connection)
        {
            var sqlOrder = $"DELETE FROM orders WHERE orderNumber = {newOrderNumber}";
            var sqlOrderDetails = $"DELETE FROM orderdetails WHERE orderNumber = {newOrderNumber}";
            using (var cmd = connection.CreateCommand())
            {
                cmd.CommandText = sqlOrderDetails;
                cmd.ExecuteNonQuery();
                cmd.CommandText = sqlOrder;
                cmd.ExecuteNonQuery();
            }

        }

        private static Order getTestOrder(MySqlConnection connection)
        {
            var sqlOrder = $"SELECT * FROM orders where orderNumber = {newOrderNumber}";
            var sqlOrderDetails = $"SELECT * FROM orderdetails where orderNumber = {newOrderNumber}";
            var order = new Order();
            using (var cmd = connection.CreateCommand())
            {
                cmd.CommandText = sqlOrder;
                using (var rdr = cmd.ExecuteReader())
                {
                    if (rdr.Read())
                    {
                        order = new Order
                        {
                            orderNumber = rdr.GetInt32("orderNumber"),
                            orderDate = rdr.GetDateTime("orderDate"),
                            requiredDate = rdr.GetDateTime("requiredDate"),
                            shippedDate = rdr.GetDateTime("shippedDate"),
                            customerNumber = rdr.GetInt32("customerNumber"),
                            status = rdr.GetString("status")
                        };
                    }
                }
            }
            using (var cmd = connection.CreateCommand())
            {
                cmd.CommandText = sqlOrderDetails;
                using (var rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        order.details.Add(new OrderDetail
                        {
                            orderNumber = rdr.GetInt32("orderNumber"),
                            orderLineNumber = rdr.GetInt32("orderLineNumber"),
                            productCode = rdr.GetString("productCode"),
                            priceEach = rdr.GetDecimal("priceEach"),
                            quantityOrdered = rdr.GetInt32("quantityOrdered"),
                            lineTotal = rdr.GetDecimal("priceEach") * rdr.GetInt32("quantityOrdered")
                        });
                    }
                }
            }

            return order;
        }


        public delegate Task<(int,string)> TestFunction();

        internal static async Task<int> RunTest(int testNumber, bool showDisplay)
        {
            int result = 0;
            var testTitle = "";
            var display = "";
            var tests = new List<(string, TestFunction)>();
            tests.Add(("getConnection", TestMysqlConnection));
            tests.Add(("orderInsert", TestMysqlOrderInsert));
            tests.Add(("orderDetailsInsert", TestMysqlOrderDetailsInsert));
            tests.Add(("duplicateOrder", TestDuplicateOrder));
            tests.Add(("customerOrderCount", TestCustomerOrderCount));
            tests.Add(("customerOrderData", TestCustomerOrderData));
            try
            {
                testTitle = tests[testNumber].Item1;
                (result, display) = await tests[testNumber].Item2();
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

        public static async Task<(int, string)> TestMysqlConnection()
        {
            int result = 0;
            string display = "";
            var testObject = new MySqlCode();
            var conn = await testObject.getConnection(Settings.host, Settings.mysqlPort, Settings.database, Settings.user, Settings.password);
            if (conn.Database == Settings.database)
            {
                result = 0;
                display = "You have successfully connected to the database.";
            }
            else
            {
                result = 1;
                display = "You have not connected to the database.";
            }
            return (result, display);
        }
        public static async Task<(int, string)> TestMysqlOrderInsert()
        {
            int result = 0;
            string display = "";
            var testObject = new MySqlCode();
            var conn = await testObject.getConnection(Settings.host, Settings.mysqlPort, Settings.database, Settings.user, Settings.password);
            clearTestOrder(conn);
            var rowCount = await testObject.insertOrder(conn, testOrder);
            var testResult = getTestOrder(conn);
            if ((rowCount != 1) || (testResult.orderNumber != newOrderNumber))
            {
                result = -1;
                display = "You did not insert an order record.";
            }
            else
            {
                result = 0;
                display = "You have successfully inserted an order.";
            }
            return (result, display);
        }
        public static async Task<(int, string)> TestMysqlOrderDetailsInsert()
        {
            int result = 0;
            string display = "";
            var testObject = new MySqlCode();
            var conn = await testObject.getConnection(Settings.host, Settings.mysqlPort, Settings.database, Settings.user, Settings.password);
            clearTestOrder(conn);
            var orderRowCount = await testObject.insertOrder(conn, testOrder);
            var rowCount = 0;
            if (orderRowCount > 0)
            {
                rowCount = await testObject.insertOrderDetails(conn, testOrder.details);
            }
            var testResult = getTestOrder(conn);
            if ((rowCount != 2) || (testResult.orderNumber != newOrderNumber))
            {
                result = -1;
                display = "You did not insert an order record.";
            }
            else if (testResult.details.Count != 2)
            {
                result = -2;
                display = "You did not insert the order detail records properly.";
            }
            else
            {
                result = 0;
                display = "You have successfully inserted order detail records.";
            }
            return (result, display);
        }

       public static async Task<(int, string)> TestDuplicateOrder()
        {
            int result = 0;
            string display = "";
            var testObject = new MySqlCode();
            var conn = await testObject.getConnection(Settings.host, Settings.mysqlPort, Settings.database, Settings.user, Settings.password);
            clearTestOrder(conn);
            var rowCount = await testObject.insertOrder(conn, testOrder);
            var duplicateResultCode = await testObject.insertOrder(conn, testOrder);
            testOrder.customerNumber = testCustomerNumber + 1000000;
            var badDataResultCode = await testObject.insertOrder(conn, testOrder);
            if (badDataResultCode!=-1)
            {
                result = -1;
                display = "You did not handle the duplicate insert exception properly.";
            } else {
                result = 0;
                display = "You have successfully handled MySQL exceptions.";
            }
            return (result, display);
        }



        public static async Task<(int, string)> TestCustomerOrderCount()
        {
            int result = 0;
            string display = "";
            var testObject = new MySqlCode();
            if(Directory.Exists(Settings.dataPath)) {
                Directory.Delete(Settings.dataPath,true);
            }
            Directory.CreateDirectory(Settings.dataPath);
            var conn = await testObject.getConnection(Settings.host, Settings.mysqlPort, Settings.database, Settings.user, Settings.password);
            await testObject.exportCustomerOrders(conn,Settings.dataPath);
            if(Directory.GetFiles(Settings.dataPath).Length==98) {
                result = 0;
                display = "You have exported the correct number of customer orders.";
            } else {
                result = -1;
                display = "You have not exported the correct number of customer orders.";

            }
            return (result, display);
        }




        public static async Task<(int, string)> TestCustomerOrderData()
        {
            int result = 0;
            string display = "";
            var testObject = new MySqlCode();
            if(Directory.Exists(Settings.dataPath)) {
                Directory.Delete(Settings.dataPath,true);
            }
            Directory.CreateDirectory(Settings.dataPath);
            var conn = await testObject.getConnection(Settings.host, Settings.mysqlPort, Settings.database, Settings.user, Settings.password);
            await testObject.exportCustomerOrders(conn,Settings.dataPath);
            string filePath = $"{Settings.dataPath}\\{testCustomerNumber}.json";
            if(!File.Exists(filePath)) {
                result = -1;
                display = "You did not export the correct files.";
            } else {
                var customer = JsonSerializer.Deserialize<CustomerOrder>(File.ReadAllText(filePath));
                if(customer._id!=testCustomerNumber.ToString()) {
                    result = -2;
                    display = "You did not export the '_id' property correctly.";
                } else if(customer.orders.Count!=testOrderCount) {
                    result = -3;
                    display = "You did not export the order data correctly.";
                } else if(customer.orders.First(o => o.orderNumber==testOrderNumber).details.Count!=testOrderDetailCount) {
                    result = -4;
                    display = "You did not export the order details data correctly.";
                } else {
                    result = 0;
                    display = "You have successfully exported the customer order data.";
                }
            }
            return (result, display);
        }




    }
}