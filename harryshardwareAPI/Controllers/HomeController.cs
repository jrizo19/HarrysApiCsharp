using harryshardwareAPI.Models;
using harryshardwareAPI.Util;
using Microsoft.AspNetCore.Mvc;

namespace harryshardwareAPI.Controllers;

[ApiController]
[Route("[controller]")]

public class HomeController : ControllerBase
{
    private readonly HomeModel _homeModel;

    public HomeController(DatabaseConnection dbConnection)
    {
        _homeModel = new HomeModel(dbConnection);
    }
    public class DataContainer
    {
        public List<HomeModel.CustomerData> customers { get; set; }
        public List<HomeModel.ProductData> products { get; set; }
        public List<HomeModel.SalesData> sales { get; set; }

        public DataContainer()
        {
            customers = new List<HomeModel.CustomerData>();
            products = new List<HomeModel.ProductData>();
            sales = new List<HomeModel.SalesData>();
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetData()
    {
        DataContainer allData = new DataContainer
        {
            customers = await _homeModel.FetchCustomer(),
            products = await _homeModel.FetchProduct(),
            sales = await _homeModel.FetchSales()
        };

        Console.WriteLine(allData);
        return Ok(allData);
    }
}