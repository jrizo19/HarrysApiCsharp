using harryshardwareAPI.Util;
using MySqlConnector;

namespace harryshardwareAPI.Models;

public class HomeModel
{
    private readonly DatabaseConnection _dbConnection;

    public HomeModel(DatabaseConnection dbConnection)
    {
        _dbConnection = dbConnection;
    }
    public class CustomerData
    {
        public int CustomerID { get; set; }
        public string CustomerName { get; set; }
        public decimal TotalSales { get; set; }
    }

    public class SalesData
    {
        public string Month { get; set; }
        public int Year { get; set; }
        public int SalesID { get; set; }
        public decimal TotalSales { get; set; }
    }
    public class ProductData
    {
        public int ProductID { get; set; }
        public string ProductName { get; set; }
        public decimal TotalSales { get; set; }
    }
    public async Task<List<CustomerData>> FetchCustomer()
    {
        var dataList = new List<CustomerData>();

        using (var connection = _dbConnection.GetConnection())
        {
            await connection.OpenAsync();

            var query = "SELECT c.CustomerID, c.CustomerName, SUM(i.itemprice * s.quantity) AS Total_Sales " +
            "FROM customer c JOIN sales s ON c.customerid = s.customerid " +
            "JOIN item i ON s.itemid = i.itemid " +
            "GROUP BY c.customerid " +
            "ORDER BY Total_Sales DESC " +
            "LIMIT 5;";
            using (var command = new MySqlCommand(query, connection))
            {
                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        var customers = new CustomerData
                        {
                            CustomerID = reader.GetInt32("CustomerID"),
                            CustomerName = reader.GetString("CustomerName"),
                            TotalSales = reader.GetDecimal("Total_Sales")
                        };
                        dataList.Add(customers);
                    }
                }
            }
        }

        return dataList;
    }
    public async Task<List<ProductData>> FetchProduct()
    {
        var dataList = new List<ProductData>();

        using (var connection = _dbConnection.GetConnection())
        {
            await connection.OpenAsync();

            var query = "SELECT s.itemid, i.itemname, SUM(i.itemprice * s.quantity) AS Total_Sales " +
            "FROM item i JOIN sales s ON i.itemid = s.ItemID " +
            "GROUP BY s.ItemID " +
            "ORDER BY Total_Sales DESC " +
            "LIMIT 5;";
            using (var command = new MySqlCommand(query, connection))
            {
                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        var products = new ProductData()
                        {
                            ProductID = reader.GetInt32("itemid"),
                            ProductName = reader.GetString("itemname"),
                            TotalSales = reader.GetDecimal("Total_Sales")
                        };
                        dataList.Add(products);
                    }
                }
            }
        }

        return dataList;
    }
    public async Task<List<SalesData>> FetchSales()
    {
        var dataList = new List<SalesData>();

        using (var connection = _dbConnection.GetConnection())
        {
            await connection.OpenAsync();

            var query = "SELECT s.salesid, YEAR(s.SalesDate) AS Year, MONTHNAME(s.SalesDate) AS Month, SUM(i.ItemPrice * s.Quantity) AS Total_Sales " +
            "FROM customer c JOIN sales s ON c.CustomerID = s.CustomerID JOIN item i ON s.ItemID = i.ItemID " +
            "WHERE s.SalesDate >= DATE_FORMAT(CURRENT_DATE - INTERVAL 4 MONTH, '%Y-%m-01') " +
            "AND s.SalesDate <= LAST_DAY(CURRENT_DATE) " +
            "GROUP BY Year, Month " +
            "ORDER BY Year DESC, MONTH(s.SalesDate) DESC;";
            using (var command = new MySqlCommand(query, connection))
            {
                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        var sales = new SalesData()
                        {
                            SalesID = reader.GetInt32("salesid"),
                            Month = reader.GetString("Month"),
                            Year = reader.GetInt32("Year"),
                            TotalSales = reader.GetDecimal("Total_Sales")
                        };
                        dataList.Add(sales);
                    }
                }
            }
        }

        return dataList;
    }
}