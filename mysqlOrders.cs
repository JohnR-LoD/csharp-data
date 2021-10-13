using System;
using System.Threading.Tasks;
using System.Collections.Generic;

using MySqlConnector;
using System.Text.Json;
using System.IO;

namespace labfiles
{
    public class MySqlCode
    {
        public async Task<MySqlConnection> getConnection(string host, string port, string database, string user, string password)
        {
            var builder = new MySqlConnectionStringBuilder
            {
                Server = host,
                Database = database,
                UserID = user,
                Password = password,
            };
            var conn = new MySqlConnection(builder.ConnectionString);
            await conn.OpenAsync();
            return conn;
        }
        public async Task<int> insertOrder(MySqlConnection connection, Order newOrder)
        {
            var insertSQL = @"INSERT INTO orders(orderNumber,orderDate,requiredDate,shippedDate,status,customerNumber) VALUES (@orderNumber,@orderDate,@requiredDate,@shippedDate,@status,@customerNumber)";
            int rowCount = 0;
            try
            {
                using (var cmd = connection.CreateCommand())
                {
                    cmd.CommandText = insertSQL;
                    cmd.Parameters.AddWithValue("@orderNumber", newOrder.orderNumber);
                    cmd.Parameters.AddWithValue("@orderDate", newOrder.orderDate);
                    cmd.Parameters.AddWithValue("@requiredDate", newOrder.requiredDate);
                    cmd.Parameters.AddWithValue("@shippedDate", newOrder.shippedDate);
                    cmd.Parameters.AddWithValue("@status", newOrder.status);
                    cmd.Parameters.AddWithValue("@customerNumber", newOrder.customerNumber);
                    rowCount = await cmd.ExecuteNonQueryAsync();
                }
            }
            catch (MySqlConnector.MySqlException ex)
            {
               if (ex.ErrorCode == MySqlErrorCode.DuplicateKeyEntry)
                {
                    return -1;
                }
                else
                {
                    return -2;
                }
            }
            return rowCount;
        }

        public async Task<int> insertOrderDetails(MySqlConnection connection, List<OrderDetail> details)
        {
            var insertSQL = @"INSERT INTO orderdetails(orderNumber,productCode,quantityOrdered,priceEach,orderLineNumber) VALUES (@orderNumber,@productCode,@quantityOrdered,@priceEach,@orderLineNumber)";
            int rowCount = 0;
            try
            {
                using (var cmd = connection.CreateCommand())
                {
                    cmd.CommandText = insertSQL;
                    cmd.Parameters.Add("@orderNumber", System.Data.DbType.Int32);
                    cmd.Parameters.Add("@productCode", System.Data.DbType.String);
                    cmd.Parameters.Add("@quantityOrdered", System.Data.DbType.Int32);
                    cmd.Parameters.Add("@priceEach", System.Data.DbType.Decimal);
                    cmd.Parameters.Add("@orderLineNumber", System.Data.DbType.Int16);
                    foreach (var item in details)
                    {
                        cmd.Parameters["orderNumber"].Value = item.orderNumber;
                        cmd.Parameters["productCode"].Value = item.productCode;
                        cmd.Parameters["quantityOrdered"].Value = item.quantityOrdered;
                        cmd.Parameters["priceEach"].Value = item.priceEach;
                        cmd.Parameters["orderLineNumber"].Value = item.orderLineNumber;
                        rowCount += await cmd.ExecuteNonQueryAsync();
                    }
                }
            }
            catch (MySqlConnector.MySqlException ex)
            {
                if (ex.ErrorCode == MySqlErrorCode.DuplicateKey)
                {
                    return -1;
                }
                else
                {
                    return -2;
                }
            }
            return rowCount;
        }

        public async Task exportCustomerOrders(MySqlConnection connection, string dataPath)
        {
            var SQL = @"SELECT customerNumber FROM customers;";
            List<int> customerNumbers = new List<int>();
            using (var cmd = connection.CreateCommand())
            {
                cmd.CommandText = SQL;
                using (var rdr = await cmd.ExecuteReaderAsync())
                {
                    while (await rdr.ReadAsync())
                    {
                        customerNumbers.Add(rdr.GetInt32(0));
                    }
                }
            }
            foreach (var customerNumber in customerNumbers)
            {
                await saveCustomerOrdersDocument(connection, customerNumber, dataPath);
            }

        }

        public async Task saveCustomerOrdersDocument(MySqlConnection connection, int customerNumber, string dataPath)
        {
            var orders = new List<Order>();
            var orderSQL = $"SELECT * FROM orders WHERE customerNumber = {customerNumber}";
            using (var cmd = connection.CreateCommand())
            {
                cmd.CommandText = orderSQL;
                using (var rdr = await cmd.ExecuteReaderAsync())
                {
                    while (await rdr.ReadAsync())
                    {
                        orders.Add(new Order
                        {
                            orderNumber = rdr.GetInt32("orderNumber"),
                            orderDate = rdr["orderDate"] is DBNull ? null : (DateTime?) rdr["orderDate"],
                            requiredDate = rdr["requiredDate"] is DBNull ? null : (DateTime?)rdr["requiredDate"],
                            shippedDate = rdr["shippedDate"] is DBNull ? null : (DateTime?)rdr["shippedDate"],
                            customerNumber = rdr.GetInt32("customerNumber"),
                            status = rdr.GetString("status")
                        });
                    }
                }
            }
            foreach (var order in orders)
            {
                var detailSQL = $"SELECT * FROM orderdetails WHERE orderNumber = {order.orderNumber}";
                using (var cmd = connection.CreateCommand())
                {
                    cmd.CommandText = detailSQL;
                    using (var rdr = await cmd.ExecuteReaderAsync())
                    {
                        while (await rdr.ReadAsync())
                        {
                            order.details.Add(new OrderDetail
                            {
                                orderNumber = rdr.GetInt32("orderNumber"),
                                orderLineNumber = rdr.GetInt32("orderLineNumber"),
                                productCode = rdr.GetString("productCode"),
                                priceEach = rdr.GetDecimal("priceEach"),
                                quantityOrdered = rdr.GetInt32("quantityOrdered"),
                                lineTotal = rdr.GetDecimal("priceEach")* rdr.GetInt32("quantityOrdered")
                            });
                        }
                    }
                }
            }
            if(orders.Count>0) {
                var customer = new CustomerOrder {
                    _id = customerNumber.ToString(),
                    customerNumber = customerNumber,
                    orders = orders
                };
                var filePath = $"{dataPath}\\{customerNumber}.json";
                await File.WriteAllTextAsync(filePath, JsonSerializer.Serialize(customer,options: new JsonSerializerOptions{
                    WriteIndented=true
                }));
            }
        }


        //Class end
    }

}