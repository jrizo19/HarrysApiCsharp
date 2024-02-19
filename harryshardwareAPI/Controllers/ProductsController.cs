using harryshardwareAPI.Models;
using harryshardwareAPI.Util;
using Microsoft.AspNetCore.Mvc;

namespace harryshardwareAPI.Controllers;

[ApiController]
[Route("[controller]")]

public class ProductsController : ControllerBase
{
    private readonly ProductModel _productModel;

    public ProductsController(DatabaseConnection dbConnection)
    {
        _productModel = new ProductModel(dbConnection);
    }
    [HttpGet("{id}")]
    public async Task<IActionResult> GetData(int id)
    {
        var productID = await _productModel.FetchProduct(id);

        Console.WriteLine(productID);
        return Ok(productID);
    }
    [HttpGet]
    public async Task<IActionResult> GetData()
    {
        var productData = await _productModel.FetchAll();

        Console.WriteLine(productData);
        return Ok(productData);
    }
    
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        bool isDeleted = await _productModel.Delete(id);

        if (isDeleted)
        {
            return Ok("Product deleted successfully.");
        }
        else
        {
            return NotFound("Product not found.");
        }
    }

    [HttpPost]
    public async Task<IActionResult> AddCustomer(ProductModel.Product product)
    {
        int productID = await _productModel.Add(product);
        return Ok($"Product added with ID: {productID}");
    }
    
    [HttpPut("{id}")]
    public async Task<IActionResult> Edit (ProductModel.ProductDataOne product)
    {
        int rowsAffected = await _productModel.Edit(product);
        if (rowsAffected > 0)
        {
            return Ok("Product updated successfully.");
        }
        else
        {
            return NotFound("Product not found.");
        }
    }
}