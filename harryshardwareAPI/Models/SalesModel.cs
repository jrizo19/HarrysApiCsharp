using harryshardwareAPI.Util;
using MySqlConnector;
namespace harryshardwareAPI.Models;

public class SalesModel
{
    private readonly DatabaseConnection _dbConnection;

    public SalesModel(DatabaseConnection dbConnection)
    {
        _dbConnection = dbConnection;
    }
    public class salesData
    {
        public string FormattedSalesDate { get; set; }
        public string CustomerName { get; set; }
        public int SalesID { get; set; }
        public string Product { get; set; }
        public int Quantity { get; set; }
        public decimal TotalSales { get; set; }
    }

    public async Task<List<salesData>> FetchAll()
    {
        var dataList = new List<salesData>();

        using (var connection = _dbConnection.GetConnection())
        {
            await connection.OpenAsync();

            var query = "SELECT DATE_FORMAT(s.SalesDate, '%Y-%m-%d') as FormattedSalesDate, " +
            "c.CustomerName, s.SalesID, i.ItemName as Product, s.Quantity, SUM(i.ItemPrice * s.Quantity) as TotalSales " +
            "FROM customer c JOIN sales s ON c.CustomerID = s.CustomerID JOIN item i ON s.ItemID = i.ItemID " +
            "WHERE MONTH(s.SalesDate) = MONTH(CURDATE()) " +
            "AND YEAR(s.SalesDate) = YEAR(CURDATE()) " +
            "GROUP BY i.ItemName " +
            "ORDER BY s.SalesDate;";
            using (var command = new MySqlCommand(query, connection))
            {
                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        var sales = new salesData()
                        {
                            FormattedSalesDate = reader.GetString("FormattedSalesDate"),
                            CustomerName = reader.GetString("CustomerName"),
                            SalesID = reader.GetInt32("SalesID"),
                            Product = reader.GetString("Product"),
                            Quantity = reader.GetInt32("Quantity"),
                            TotalSales = reader.GetDecimal("TotalSales")
                        };
                        dataList.Add(sales);
                    }
                }
            }
        }

        return dataList;
    }
}