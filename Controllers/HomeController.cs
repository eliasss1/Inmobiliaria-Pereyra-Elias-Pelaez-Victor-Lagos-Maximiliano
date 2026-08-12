using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Inmobiliaria_Pereyra_Elias_Pelaez_Victor_Lagos_Maximiliano.Models;

namespace Inmobiliaria_Pereyra_Elias_Pelaez_Victor_Lagos_Maximiliano.Controllers;

public class HomeController : Controller
{
    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
