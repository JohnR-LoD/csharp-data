using System;
using System.Threading.Tasks;
using System.Text;
using MySqlConnector;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.IO;

namespace csharp_data
{
    delegate Task<(int, string)> TestFunction();

    class TestRelational
    {


        internal static async Task<int> RunTest(int testNumber, bool showDisplay)
        {
            int result = 0;
            var testTitle = "";
            var display = "";
            var tests = new List<(string, TestFunction)>();
            tests.Add(("getConnection", TestConnect));
            tests.Add(("retrieveCustomerByNumber", TestGetCustomerByCustomerNumber));
            tests.Add(("retrieveCustomersByState", TestGetCustomersByState));
            tests.Add(("insertProductLine", TestInsertProductLine));
            tests.Add(("updateProductLine", TestUpdateProductLine));
            tests.Add(("deleteProductLine", TestDeleteProductLine));
            tests.Add(("Connection exception", TestConnectionExceptionHandling));
            tests.Add(("No results", TestGetCustomerByCustomerNumberEmpty));
            tests.Add(("Insert exception", TestInsertExceptionHandling));
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
        private const string testCustomerNumber = "161";
        private const string testCustomerName = "Technics Stores Inc.";
        private const string testProductLine = "Graphene Cars";
        private const string testProductLineDescription = "Graphene Cars are literally made of the strongest material we can get our hands on.";
        private const string testProductLineHtml = "<div class'productline'>Graphene Cars are literally <b>made of the strongest material we can get our hands on</b>.</div>";


        internal static async Task<ProductLine> GetProductLineAsync()
        {
            var testObject = new mysqlCode();
            var connection = await testObject.getConnection(Settings.mysqlHost, Settings.mysqlPort, Settings.database, Settings.user, Settings.password);
            var SQL = $"SELECT * FROM productlines WHERE productLine = '{testProductLine}'";
            ProductLine productLine = null;
            using (var cmd = connection.CreateCommand())
            {
                cmd.CommandText = SQL;
                using (var rdr = await cmd.ExecuteReaderAsync())
                {
                    if (await rdr.ReadAsync())
                    {
                        productLine = new ProductLine
                        {
                            productLine = rdr.GetString("productLine"),
                            textDescription = rdr["textDescription"].ToString(),
                            htmlDescription = rdr["htmlDescription"].ToString()
                        };
                    }
                }
            }
            return productLine;
        }

        internal static async Task<int> deleteProductLineAsync()
        {
            var testObject = new mysqlCode();
            var connection = await testObject.getConnection(Settings.mysqlHost, Settings.mysqlPort, Settings.database, Settings.user, Settings.password);
            var deleteSQL = @"DELETE FROM productlines WHERE productLine = @productLine";
            int rowCount = 0;
            using (var cmd = connection.CreateCommand())
            {
                cmd.CommandText = deleteSQL;
                cmd.Parameters.AddWithValue("@productLine", testProductLine);
                rowCount = await cmd.ExecuteNonQueryAsync();
            }
            return rowCount;
        }

        static async Task<(int, string)> TestConnect()
        {
            var result = 0;
            var testObject = new mysqlCode();
            var conn = await testObject.getConnection(Settings.mysqlHost, Settings.mysqlPort, Settings.database, Settings.user, Settings.password);
            result = conn.Database == Settings.database ? 0 : 1;
            var display = (result == 0) ? "You have successfully connected to the MariaDB database." : "You have not connected to the MariaDB database.";
            return (result, display);
        }

        static async Task<(int, string)> TestGetCustomerByCustomerNumber()
        {
            var result = 0;
            var testObject = new mysqlCode();
            var conn = await testObject.getConnection(Settings.mysqlHost, Settings.mysqlPort, Settings.database, Settings.user, Settings.password);
            string customerName, phone;
            //Read a single 
            (customerName, phone) = await testObject.retrieveCustomerByNumber(conn, testCustomerNumber);
            var correct = customerName == testCustomerName;
            result += correct ? 0 : 1;
            var display = correct ? $"You have successfully returned a customer record for {customerName} with a phone number of {phone}" : "You have not returned the correct customer information.";
            return (result, display);
        }

        static async Task<(int, string)> TestGetCustomersByState()
        {
            var result = 0;
            var display = "";
            var testObject = new mysqlCode();
            var conn = await testObject.getConnection(Settings.mysqlHost, Settings.mysqlPort, Settings.database, Settings.user, Settings.password);
            var customers = await testObject.retrieveCustomersByState(conn, "NY");
            if (customers.Count == 0)
            {
                result = 1;
                display = "You did not return any customers.";
            }
            else if (customers.Count != 6)
            {
                result = 1;
                display = $"You did not return the correct number of customers. You returned {customers.Count}";
            }
            else
            {
                result = 0;
                display = $"You successfully returned customers based on their state. {Program.SaveResults("customers", customers)}\n";
            }

            return (result, display);
        }

        static async Task<(int, string)> TestInsertProductLine()
        {
            var result = 0;
            var display = "";
            var testObject = new mysqlCode();
            var conn = await testObject.getConnection(Settings.mysqlHost, Settings.mysqlPort, Settings.database, Settings.user, Settings.password);
            var rowCount = await testObject.insertProductLine(conn, testProductLine, testProductLineDescription);
            if (rowCount == 1)
            {
                var productLine = await GetProductLineAsync();
                if ((productLine != null) && (productLine.textDescription == testProductLineDescription))
                {
                    result = 0;
                    display = $"You have successfully inserted a product line record. {Program.SaveResults("insert", productLine)}";
                }
                else
                {
                    result = 0;
                    display = "You have not properly inserted a product line record.";
                }
            }
            else
            {
                result = 1;
                display = "You have not properly inserted a product line record.";
            }

            return (result, display);

        }

        static async Task<(int, string)> TestUpdateProductLine()
        {
            var result = 0;
            var display = "";
            var testObject = new mysqlCode();
            var conn = await testObject.getConnection(Settings.mysqlHost, Settings.mysqlPort, Settings.database, Settings.user, Settings.password);
            var rowCount = await testObject.updateProductLine(conn, testProductLine, testProductLineHtml);
            if (rowCount == 1)
            {
                var productLine = await GetProductLineAsync();
                if ((productLine != null) && (productLine.htmlDescription == testProductLineHtml))
                {
                    result = 0;
                    display = $"You have successfully updated a product line record. {Program.SaveResults("update", productLine)}";
                }
                else
                {
                    result = 0;
                    display = "You have not properly updated a product line record.";
                }
            }
            else
            {
                result = 1;
                display = "You have not updated a product line record.";
            }

            return (result, display);
        }

        static async Task<(int, string)> TestDeleteProductLine()
        {
            var result = 0;
            var display = "";
            var testObject = new mysqlCode();
            var conn = await testObject.getConnection(Settings.mysqlHost, Settings.mysqlPort, Settings.database, Settings.user, Settings.password);
            var rowCount = await testObject.deleteProductLine(conn, testProductLine);
            if (rowCount == 1)
            {
                var productLine = await GetProductLineAsync();
                if (productLine == null)
                {
                    result = 0;
                    display = $"You have successfully deleted a product line record.";
                }
                else
                {
                    result = 0;
                    display = "You have not properly updated a product line record.";
                }
            }
            else
            {
                result = 1;
                display = "You have not deleted a product line record.";
            }

            return (result, display);
        }
        static async Task<(int, string)> TestConnectionExceptionHandling()
        {
            var result = 0;
            var display = "";
            var testObject = new mysqlCode();
            var conn = await testObject.getConnection(Settings.mysqlHost, Settings.mysqlPort, Settings.database, $"bad{Settings.user}", Settings.password);
            display = "You have successfully handled a bad MySQL connection string";
            return (result, display);
        }
        static async Task<(int, string)> TestGetCustomerByCustomerNumberEmpty()
        {
            var result = 0;
            var testObject = new mysqlCode();
            var conn = await testObject.getConnection(Settings.mysqlHost, Settings.mysqlPort, Settings.database, Settings.user, Settings.password);
            //Read a single 
            var customer = await testObject.retrieveCustomerByNumber(conn, testCustomerNumber + 1000000);
            var correct = customer.Item1 == null;
            result += correct ? 0 : 1;
            var display = correct ? $"\tYou have successfully handled an empty result set." : "\tYou have not handled an empty result set.";
            return (result, display);
        }
        static async Task<(int, string)> TestInsertExceptionHandling()
        {
            var result = 0;
            var display = "";
            var testObject = new mysqlCode();
            var conn = await testObject.getConnection(Settings.mysqlHost, Settings.mysqlPort, Settings.database, Settings.user, Settings.password);
            var rowCount = await testObject.insertProductLine(conn, null, testProductLineDescription);
            await testObject.insertProductLine(conn, testProductLine, testProductLineDescription);
            rowCount += await testObject.insertProductLine(conn, testProductLine, testProductLineDescription);
            display = rowCount == -1 ? "You have successfully implemented exception handling for a DuplicateKeyEntry exception." : "You have not implemented exception handling specifically for a DuplicateKeyEntry exception.";
            return (result, display);
        }



    }
}