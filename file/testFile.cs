using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace labfiles.file
{
    internal delegate (int, string) TestFunction();

    internal class TestFile
    {
        private const string dataPath = "d:\\labfiles\\data";
        private const string customerFile = "customer.json";
        private const string productsFile = "products.json";
        private const string orderFileSearch = "*.csv";
        private const string orderFileName = "2003-10.csv";

        private static Customer testCustomer
        {
            get
            {
                var customerData = File.ReadAllText($"{dataPath}\\{customerFile}");
                var customer = JsonSerializer.Deserialize<Customer>(customerData);
                if (customer == null) return new Customer(); 
                
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

        internal int RunTest(int testNumber, bool showDisplay)
        {
            int result = 0;
            var testTitle = string.Empty;

            var tests = new List<(string, TestFunction)>
            {
                ("readCustomerFile", TestReadCustomerFromFile),
                ("readCustomerData", TestReadCustomerProcessingCenter),
                ("readProductCount", TestReadProductCount),
                ("readProductData", TestReadProductData),
                ("monthlyOrders", TestMonthlyOrders),
                ("customerReport", TestCustomerReport)
            };

            try
            {
                string display;
                
                testTitle = tests[testNumber].Item1;
                
                (result, display) = tests[testNumber].Item2();
                
                if (showDisplay) Console.WriteLine($"{testTitle}:\t{display}");
            }
            catch (Exception ex)
            {
                result = -1;
                if (showDisplay) Console.WriteLine($"{testTitle}:\tResult = ERROR - {ex.Message}");
            }
            return result;
        }

        private (int, string) TestReadCustomerFromFile()
        {
            var result = -1;
            string display;
            var testObject = new FileCode();
            
            var customer = testObject.readCustomer(dataPath, customerFile);
            
            Console.WriteLine(customer.customerName);
            Console.WriteLine(JsonSerializer.Serialize(customer));
            
            if (customer.customerName != testCustomer.customerName)
            {
                display = "Your code did not return the correct data.";
            }
            else
            {
                result = 0;
                display = "You have successfully read in the customer data.";
            }
            return (result, display);
        }
        private (int, string) TestReadCustomerProcessingCenter()
        {
            var result = -1;
            var display = "";
            var testObject = new FileCode();
            var customer = testObject.readCustomer(dataPath, customerFile);
            if (customer == null)
            {
                display = "Your code did not return a customer.";
            }
            else if (customer.processingCenter != "LODS")
            {
                display = "Your code did not return the correct processing center data.";
            }
            else
            {
                result = 0;
                display = $"You have successfully set the processing center data. {Program.SaveResults("customer", customer)}";
            }
            return (result, display);
        }
        private (int, string) TestReadProductCount()
        {
            var result = -1;
            var display = "";
            var testObject = new FileCode();
            var products = testObject.readProducts(dataPath, productsFile);
            if (products == null)
            {
                display = "Your code did not return a List of Product objects";
            }
            else if (products.Count != 110)
            {
                display = "Your code did not return the correct number of Product objects";
            }
            else
            {
                result = 0;
                display = $"You have successfully returned the correct number of product objects.";
                
            }
            return (result, display);
        }
        private (int, string) TestReadProductData()
        {
            var result = -1;
            var display = "";
            var testObject = new FileCode();
            var products = testObject.readProducts(dataPath, productsFile);
            if (products == null)
            {
                display = "Your code did not return a List of Product objects";
            }
            else if (products[0].msrp != testProduct.msrp)
            {
                display = "Your code did not return the correct Product data";
            }
            else
            {
                result = 0;
                display = $"You have successfully returned product data. {Program.SaveResults("products", products)}";
            }
            return (result, display);
        }
        private (int, string) TestMonthlyOrders()
        {
            int result = -1;
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
                
                display = "Your code did not return any orders.";
            } else if (orderResults.Last().orderTotal!=testTotal) {
                
                display = "Your code did not return the correct data.";

            } else {
                result = 0;
                display = $"Your code returned the correct order results. {Program.SaveResults("Customer Orders",orderResults)}";
            }

            return (result, display);
        }
        
        private (int, string) TestCustomerReport()
        {
            var result = -1;
            var display = "";
            var fileName = $"{dataPath}\\{orderFileName}";
            var orderData = File.ReadAllLines(fileName);
            var testData = new List<Order>();
            var testObject = new FileCode();
            var allOrderResults = testObject.generateOrdersReport(dataPath,orderFileSearch);
            if (allOrderResults==null || allOrderResults.Count == 0)
            {
                display = "Your code did not return any data";
                return (result, display);
            } else if (allOrderResults.Count != 143)
            {
                display = $"Your code did not return the correct number of orders. The correct number is 143 and your code returned {allOrderResults.Count }.";
                return (result, display);
            } else if (allOrderResults[30].customerNumber!=382)
            {
                display = $"Your code did not return the correct order data.";
                return (result, display);
            } else {
                result = 0;
                display = $"You code returned the correct order data. {Program.SaveResults("report",allOrderResults, false)}";
                return (result, display);
            }
        }
    }
}