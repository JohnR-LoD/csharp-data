using System.Threading.Tasks;
using System.Collections.Generic;
using System;
using System.IO;
using System.Text.Json;
using System.Linq;

namespace labfiles
{
    delegate (int, string) TestFunction();

    internal class TestFile
    {
        internal const string dataPath = "d:\\labfiles\\data";
        internal const string customerFile = "customer.json";
        internal const string productsFile = "products.json";
        internal const string orderFileSearch = "*.csv";
        internal const string orderFileName = "2003-10.csv";

        private static Customer testCustomer
        {
            get
            {
                string customerData = File.ReadAllText($"{dataPath}\\{customerFile}");
                var customer = JsonSerializer.Deserialize<Customer>(customerData);
                customer.processingCenter = "LODS";
                return customer;

            }

        }

        private static Product testProduct
        {
            get
            {
                var products = File.ReadAllLines($"{dataPath}\\{productsFile}");
                return JsonSerializer.Deserialize<Product>(products[0].Substring(2));
            }
        }

        internal static int RunTest(int testNumber, bool showDisplay)
        {
            int result = 0;
            var testTitle = "";
            var display = "";
            var tests = new List<(string, TestFunction)>();
            tests.Add(("readCustomerFile", TestReadCustomerFromFile));
            tests.Add(("readCustomerData", TestReadCustomerProcessingCenter));
            tests.Add(("readProductCount", TestReadProductCount));
            tests.Add(("readProductData", TestReadProductData));
            tests.Add(("monthlyOrders", TestMonthlyOrders));
            tests.Add(("customerReport", TestCustomerReport));



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

        static (int, string) TestReadCustomerFromFile()
        {
            var result = 0;
            var display = "";
            var testObject = new FileCode();
            var customer = testObject.readCustomer(dataPath, customerFile);
            Console.WriteLine(customer.customerName);
            Console.WriteLine(JsonSerializer.Serialize(customer));
            if (customer == null)
            {
                result = -1;
                display = "Your code did not return a customer.";
            }
            else if (customer.customerName != testCustomer.customerName)
            {
                result = -1;
                display = "Your code did not return the correct data.";
            }
            else
            {
                result = 0;
                display = "You have successfullt read in the customer data.";
            }
            return (result, display);
        }
        static (int, string) TestReadCustomerProcessingCenter()
        {
            var result = 0;
            var display = "";
            var testObject = new FileCode();
            var customer = testObject.readCustomer(dataPath, customerFile);
            if (customer == null)
            {
                result = -1;
                display = "Your code did not return a customer.";
            }
            else if (customer.processingCenter != "LODS")
            {
                result = -1;
                display = "Your code did not return the correct processing center data.";
            }
            else
            {
                result = 0;
                display = $"You have successfully set the processing center data. {Program.SaveResults("customer", customer)}";
            }
            return (result, display);
        }
        static (int, string) TestReadProductCount()
        {
            var result = 0;
            var display = "";
            var testObject = new FileCode();
            var products = testObject.readProducts(dataPath, productsFile);
            if (products == null)
            {
                display = "Your code did not return a List of Product objects";
                result = -1;
            }
            else if (products.Count != 110)
            {
                display = "Your code did not return the correct number of Product objects";
                result = -1;
            }
            else
            {
                display = $"You have successfully returned the correct number of product objects.";
                result = 0;
            }
            return (result, display);
        }
        static (int, string) TestReadProductData()
        {
            var result = 0;
            var display = "";
            var testObject = new FileCode();
            var products = testObject.readProducts(dataPath, productsFile);
            if (products == null)
            {
                display = "Your code did not return a List of Product objects";
                result = -1;
            }
            else if (products[0].MSRP != testProduct.MSRP)
            {
                display = "Your code did not return the correct Product data";
                result = -1;
            }
            else
            {
                display = $"You have successfully returned product data. {Program.SaveResults("products", products)}";
                result = 0;
            }
            return (result, display);
        }
        static (int, string) TestMonthlyOrders()
        {
            var result = 0;
            var display = "";
            var fileName = $"{dataPath}\\{orderFileName}";
            var orderData = File.ReadAllLines(fileName);
            var testData = new List<Order>();
            foreach(var orderLine in orderData) {
                var order = orderLine.Split(",");
                testData.Add(new Order
                {
                    orderNumber = int.Parse(order[0]),
                    orderDate = System.DateTime.Parse(order[1]),
                    status = order[2],
                    customerNumber = int.Parse(order[3]),
                    orderTotal = decimal.Parse(order[4])
                });
            }
            var testTotal = testData.Where(o => o.status == "Shipped").OrderByDescending(o => o.orderTotal).Take(5).Last().orderTotal;

            var testObject = new FileCode();
            var orderResults = testObject.processMonthlyOrders(orderData);
            if(orderResults==null) {
                result = -1;
                display = "Your code did not return any orders.";
            } else if (orderResults.Last().orderTotal!=testTotal) {
                result = -1;
                display = "Your code did not return the correct data.";

            } else {
                result = 0;
                display = $"Your code returned the correct order results. {Program.SaveResults("Customer Orders",orderResults)}";
            }

            return (result, display);
        }
        static (int, string) TestCustomerReport()
        {
            var result = 0;
            var display = "";
            var fileName = $"{dataPath}\\{orderFileName}";
            var orderData = File.ReadAllLines(fileName);
            var testData = new List<Order>();
            var testObject = new FileCode();
            var allOrderResults = testObject.generateOrdersReport(dataPath,orderFileSearch);
            if ((allOrderResults==null) || (allOrderResults.Count == 0)) {
                result = -1;
                display = "Your code did not return any data";
            } else if (allOrderResults.Count != 143) {
                result = -1;
                display = $"Your code did not return the correct number of orders. The correct number is 143 and your code returned {allOrderResults.Count }.";
            } else if (allOrderResults[30].customerNumber!=382) {
                result = -1;
                display = $"Your code did not return the correct order data.";
            } else {
                result = 0;
                display = $"You code returned the correct order data. {Program.SaveResults("report",allOrderResults, false)}";

            }

            return (result, display);
        }
    }



}