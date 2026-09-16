using Microsoft.AspNetCore.Mvc;
using Inmobiliaria.Models;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Inmobiliaria.Helpers; 
namespace Inmobiliaria.Controllers;

public class UsuarioController : Controller
{
    private readonly IRepositorioUsuario repo;

    public UsuarioController(IRepositorioUsuario _repo)
    {
        repo = _repo;
    }

    public ActionResult Login()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<ActionResult> Login(string email, string clave)
    {
        try
        {
            if(string.IsNullOrEmpty(email) || string.IsNullOrEmpty(clave))
            {
                ViewBag.Mensaje = "Debe ingresar email y contraseña";
                return View();
            }

            var usuario = repo.ObtenerPorEmail(email);
            
            string claveHasheada = SeguridadHelper.HashearClave(clave);

            if (usuario == null || usuario.Clave != claveHasheada)
            {
                ViewBag.Mensaje = "Email o contraseña incorrectos";
                return View();
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, usuario.IdUsuario.ToString()),
                new Claim("Id", usuario.IdUsuario.ToString()), 
                new Claim(ClaimTypes.Name, usuario.Nombre),
                new Claim(ClaimTypes.Email, usuario.Email),
                new Claim(ClaimTypes.Role, usuario.Rol) 
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, claimsPrincipal);

            return RedirectToAction("Index", "Home");
        }
        catch (Exception ex)
        {
            ViewBag.Mensaje = "Error al iniciar sesión: " + ex.Message;
            return View();
        }
    }

    public async Task<ActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction("Login", "Usuario");
    }

    public ActionResult Registrar()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult Registrar(Usuario entidad)
    {
        ModelState.Remove("Rol"); 
        if (ModelState.IsValid)
        {
            entidad.Rol = "Empleado";
            
            entidad.Clave = SeguridadHelper.HashearClave(entidad.Clave);
            
            repo.Alta(entidad);
            return RedirectToAction("Index", "Home");
        }
        return View(entidad);
    }
}