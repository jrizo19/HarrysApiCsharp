using harryshardwareAPI.Util;
using MySqlConnector;

namespace harryshardwareAPI.Models;

public class CustomerModel
{
    private readonly DatabaseConnection _dbConnection;

    public CustomerModel(DatabaseConnection dbConnection)
    {
        _dbConnection = dbConnection;
    }
    public class CustomerData
    {
        public int CustomerID { get; set; }
        public string CustomerName { get; set; }
        public string CustomerEmail { get; set; }
        public decimal TotalSales { get; set; }
    }
    public class CustomerDataOne
    {
        public int CustomerID { get; set; }
        public string CustomerName { get; set; }
        public string CustomerEmail { get; set; }
    }
    public class Customer
    {
        public string CustomerName { get; set; }
        public string CustomerEmail { get; set; }
    }
    public async Task<CustomerData> FetchCustomer(int id)
    {
        var customerData = new CustomerData();
        using (var connection = _dbConnection.GetConnection())
        {
            await connection.OpenAsync();

            var query = "select * from customer where CustomerID = @CustomerID";
            using (var command = new MySqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@CustomerID", id);

                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        var c = new CustomerData()
                        {
                            CustomerID = reader.GetInt32("CustomerID"),
                            CustomerName = reader.GetString("CustomerName"),
                            CustomerEmail = reader.GetString("CustomerEmail")
                        };
                        customerData = c;
                    }
                }
            }
        }
        return customerData;
    }

    public async Task<List<CustomerData>> FetchAll()
    {
        var dataList = new List<CustomerData>();

        using (var connection = _dbConnection.GetConnection())
        {
            await connection.OpenAsync();

            var query = "SELECT c.CustomerID, c.CustomerName, c.CustomerEmail, " +
                        "COALESCE(SUM(i.ItemPrice * s.Quantity), 0) AS TotalSales " +
                        "FROM customer c " +
                        "LEFT JOIN sales s ON c.CustomerID = s.CustomerID " +
                        "LEFT JOIN item i ON s.ItemID = i.ItemID " +
                        "GROUP BY c.CustomerID, c.CustomerName, c.CustomerEmail " +
                        "ORDER BY TotalSales DESC";
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
                            CustomerEmail = reader.GetString("CustomerEmail"),
                            TotalSales = reader.GetDecimal("TotalSales")
                        };
                        dataList.Add(customers);
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

            var query = "DELETE FROM customer WHERE CustomerID = @CustomerID";
            using (var command = new MySqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@CustomerID", id);

                int rowsAffected = await command.ExecuteNonQueryAsync();
                return rowsAffected > 0;
            }
        }
    }
    public async Task<int> Add(Customer customer)
    {
        using (var connection = _dbConnection.GetConnection())
        {
            await connection.OpenAsync();

            var query = "INSERT INTO customer (CustomerName, CustomerEmail) VALUES (@CustomerName, @CustomerEmail)";
            using (var command = new MySqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@CustomerName", customer.CustomerName);
                command.Parameters.AddWithValue("@CustomerEmail", customer.CustomerEmail);

                int insertedId = Convert.ToInt32(await command.ExecuteScalarAsync());
                return insertedId;
            }
        }
    }
    public async Task<int> Edit(CustomerDataOne customer)
    {
        using (var connection = _dbConnection.GetConnection())
        {
            await connection.OpenAsync();

            var query = "UPDATE customer set CustomerName = @CustomerName, CustomerEmail = @CustomerEmail WHERE customer.CustomerID = @CustomerID";
            using (var command = new MySqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@CustomerName", customer.CustomerName);
                command.Parameters.AddWithValue("@CustomerEmail", customer.CustomerEmail);
                command.Parameters.AddWithValue("@CustomerID", customer.CustomerID);

                int rowsAffected = await command.ExecuteNonQueryAsync();
                return rowsAffected;
            }
        }
    }
}