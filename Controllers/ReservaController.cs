using Inmobiliaria.Models;
using Microsoft.AspNetCore.Mvc;

namespace Inmobiliaria.Controllers;

public class ReservaController : Controller
{
    private readonly IRepositorioReserva repo;
    public ReservaController(IRepositorioReserva _repo)
    {
        repo = _repo;
    }
}