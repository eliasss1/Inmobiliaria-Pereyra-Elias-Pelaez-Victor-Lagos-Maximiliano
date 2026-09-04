
using Microsoft.AspNetCore.Mvc;
using Inmobiliaria.Models;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
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
            var usuario = repo.ObtenerPorEmail(email);
            
            if (usuario == null || usuario.Contraseña != clave)
            {
                ViewBag.Mensaje = "Email o contraseña incorrectos";
                return View();
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, usuario.IdUsuario.ToString()),
                new Claim(ClaimTypes.Name, usuario.Nombre),
                new Claim(ClaimTypes.Email, usuario.Email),
                new Claim(ClaimTypes.Role, usuario.Rol)
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, claimsPrincipal);

            return RedirectToAction("Index", "Home");
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
            repo.Alta(entidad);
            return RedirectToAction("Index", "Home");
        }
        return View(entidad);
    }
}


