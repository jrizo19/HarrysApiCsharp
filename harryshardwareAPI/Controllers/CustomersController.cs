using harryshardwareAPI.Models;
using harryshardwareAPI.Util;
using Microsoft.AspNetCore.Mvc;

namespace harryshardwareAPI.Controllers;

[ApiController]
[Route("[controller]")]

public class CustomersController : ControllerBase
{
    private readonly CustomerModel _customerModel;

    public CustomersController(DatabaseConnection dbConnection)
    {
        _customerModel = new CustomerModel(dbConnection);
    }
    [HttpGet("{id}")]
    public async Task<IActionResult> GetData(int id)
    {
        var customerID = await _customerModel.FetchCustomer(id);

        Console.WriteLine(customerID);
        return Ok(customerID);
    }
    [HttpGet]
    public async Task<IActionResult> GetData()
    {
        var customerData = await _customerModel.FetchAll();

        Console.WriteLine(customerData);
        return Ok(customerData);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        bool isDeleted = await _customerModel.Delete(id);

        if (isDeleted)
        {
            return Ok("Customer deleted successfully.");
        }
        else
        {
            return NotFound("Customer not found.");
        }
    }
    
    [HttpPost]
    public async Task<IActionResult> AddCustomer(CustomerModel.Customer customer)
    {
        int customerId = await _customerModel.Add(customer);
        return Ok($"Customer added with ID: {customerId}");
    }
    
    [HttpPut("{id}")]
    public async Task<IActionResult> Edit (CustomerModel.CustomerDataOne customer)
    {
        int rowsAffected = await _customerModel.Edit(customer);
        if (rowsAffected > 0)
        {
            return Ok("Customer updated successfully.");
        }
        else
        {
            return NotFound("Customer not found.");
        }
    }
}