using System.Diagnostics;
using System.Linq.Expressions;
using IdentityModel.Client;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using WeatherMvc.Models;
using WeatherMVC.Models;
using WeatherMVC.Services;

namespace WeatherMvc.Controllers;

public class HomeController : Controller
{
  private readonly ITokenService _tokenService;
  private readonly ILogger<HomeController> _logger;

  public HomeController(ITokenService tokenService, ILogger<HomeController> logger)
  {
    _tokenService = tokenService;
    _logger = logger;
  }

  public IActionResult Index()
  {
    return View();
  }

  public IActionResult Privacy()
  {
    return View();
  }

  [Authorize]
  public async Task<IActionResult> Weather()
  {
    using var client = new HttpClient();

    // var token = await _tokenService.GetToken("weatherapi.read");
    var token = await HttpContext.GetTokenAsync("access_token");
    client.SetBearerToken(token);

    var result = await client.GetAsync("https://localhost:5445/weatherforecast");

    if (result.IsSuccessStatusCode)
    {
      var model = await result.Content.ReadAsStringAsync();
      
      var data = JsonConvert.DeserializeObject<List<WeatherData>>(model);

      return View(data);
    }

    throw new Exception("Unable to get content");
  }

  [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
  public IActionResult Error()
  {
    return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
  }
}