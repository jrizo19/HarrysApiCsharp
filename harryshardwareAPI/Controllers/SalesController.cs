using harryshardwareAPI.Models;
using harryshardwareAPI.Util;
using Microsoft.AspNetCore.Mvc;

namespace harryshardwareAPI.Controllers;

[ApiController]
[Route("[controller]")]

public class SalesController : ControllerBase
{
    private readonly SalesModel _salesModel;

    public SalesController(DatabaseConnection dbConnection)
    {
        _salesModel = new SalesModel(dbConnection);
    }
    [HttpGet]
    public async Task<IActionResult> GetData()
    {
        var salesData = await _salesModel.FetchAll();

        Console.WriteLine(salesData);
        return Ok(salesData);
    }
}