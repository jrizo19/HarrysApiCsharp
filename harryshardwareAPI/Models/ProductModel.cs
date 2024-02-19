using harryshardwareAPI.Util;
using MySqlConnector;
namespace harryshardwareAPI.Models;

public class ProductModel
{
    private readonly DatabaseConnection _dbConnection;

    public ProductModel(DatabaseConnection dbConnection)
    {
        _dbConnection = dbConnection;
    }
    public class ProductDataOne
    {
        public int ItemID { get; set; }
        public string ItemName { get; set; }
        public decimal ItemPrice { get; set; }
    }
    public class ProductDataAll
    {
        public int ItemID { get; set; }
        public string ItemName { get; set; }
        public decimal TotalSales { get; set; }
    }

    public class Product
    {
        public string ItemName { get; set; }
        public decimal ItemPrice { get; set; }
    }
    public async Task<ProductDataOne> FetchProduct(int id)
    {
        var productData = new ProductDataOne();
        using (var connection = _dbConnection.GetConnection())
        {
            await connection.OpenAsync();

            var query = "SELECT i.itemID, i.itemName, i.itemPrice " +
            "FROM item i " +
            "WHERE i.itemID= @ProductID";
            using (var command = new MySqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@ProductID", id);

                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        var c = new ProductDataOne()
                        {
                            ItemID = reader.GetInt32("itemID"),
                            ItemName = reader.GetString("itemName"),
                            ItemPrice = reader.GetDecimal("itemPrice")
                        };
                        productData = c;
                    }
                }
            }
        }
        return productData;
    }

    public async Task<List<ProductDataAll>> FetchAll()
    {
        var dataList = new List<ProductDataAll>();

        using (var connection = _dbConnection.GetConnection())
        {
            await connection.OpenAsync();

            var query = "SELECT i.itemID, i.itemName, COALESCE(SUM(i.ItemPrice * s.Quantity), 0) AS TotalSales " +
            "FROM item i LEFT JOIN sales s ON i.ItemID = s.ItemID " +
            "GROUP BY i.itemID, i.itemName ORDER BY TotalSales DESC ";
            using (var command = new MySqlCommand(query, connection))
            {
                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        var products = new ProductDataAll()
                        {
                            ItemID = reader.GetInt32("itemID"),
                            ItemName = reader.GetString("itemName"),
                            TotalSales = reader.GetDecimal("TotalSales")
                        };
                        dataList.Add(products);
                    }
                }
            }
        }

        return dataList;
    }
    public async Task<bool> Delete(int id)
    {
        using (var connection = _dbConnection.GetConnection())
        {
            await connection.OpenAsync();

            var query = "DELETE FROM item WHERE ItemID = @ItemID";
            using (var command = new MySqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@ItemID", id);

                int rowsAffected = await command.ExecuteNonQueryAsync();
                return rowsAffected > 0;
            }
        }
    }
    public async Task<int> Add(Product product)
    {
        using (var connection = _dbConnection.GetConnection())
        {
            await connection.OpenAsync();

            var query = "INSERT INTO item (ItemName, ItemPrice) VALUES (@ItemName, @ItemPrice)";
            using (var command = new MySqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@ItemName", product.ItemName);
                command.Parameters.AddWithValue("@ItemPrice", product.ItemPrice);

                int insertedId = Convert.ToInt32(await command.ExecuteScalarAsync());
                return insertedId;
            }
        }
    }
    public async Task<int> Edit(ProductDataOne product)
    {
        using (var connection = _dbConnection.GetConnection())
        {
            await connection.OpenAsync();

            var query = "UPDATE item set ItemName = @ItemName, ItemPrice = @ItemPrice WHERE item.ItemID = @ItemID";
            using (var command = new MySqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@ItemName", product.ItemName);
                command.Parameters.AddWithValue("@ItemPrice", product.ItemPrice);
                command.Parameters.AddWithValue("@ItemID", product.ItemID);

                int rowsAffected = await command.ExecuteNonQueryAsync();
                return rowsAffected;
            }
        }
    }
}