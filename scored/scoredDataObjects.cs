using System;
using System.Collections.Generic;

namespace labfiles.Shared
{
    public class CustomerOrder {
        public string _id { get; set; }
        public int customerNumber { get; set; }
        public List<Order> orders { get; set; }

        public CustomerOrder(){
            orders = new List<Order>();
        }

    }
    public class Order {
        public int orderNumber { get; set; }
        public DateTime? orderDate { get; set; }
        public DateTime? requiredDate { get; set; }
        public DateTime? shippedDate { get; set; }
        public string status { get; set; }
        public string comments { get; set; }
        public int customerNumber { get; set; }
        public List<OrderDetail> details { get; set; }

        public Order(){
            details = new List<OrderDetail>();
        }
    }
    public class OrderDetail {
        public int orderNumber { get; set; }
        public int orderLineNumber { get; set; }
        public string productName { get; set; }
        public string productCode { get; set; }
        public decimal priceEach { get; set; }
        public int quantityOrdered { get; set; }
        public decimal lineTotal { get; set; }
    }
    public class CustomerContact {
        public string customerName { get; set; }
        public string phone { get; set; }
    }

    public class CustomerReport {
        public CustomerContact contact { get; set; }
        public List<Order> orders { get; set; }
    }
}