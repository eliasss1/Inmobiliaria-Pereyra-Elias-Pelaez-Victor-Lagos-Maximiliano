
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
            try
            {
                var usuario = repo.ObtenerPorEmail(email);
                
                // ESTA COMPPROBACION LA HAGO PORQUE TUVE MANSOS PROBLEMAS, YA SE QUE QUIZA ALGUN USUARIO HAGA UNA CONTRASEÑA VACIA
                // PERO ESTO SE SOLUCIONA OBLIGANDO A QUE EL USUARIO AGREGUE UNA CONTRA CON REQUISITOS MINIMOS.
                if(usuario.Email == null || usuario.Clave == null || usuario.Clave == "" || usuario.Email == "")
                {
                    var mensaje = "No se reciben el Email o la contraseña desde el repositorio";
                    Console.WriteLine("Error al iniciar sesión: " + mensaje);
                }

                if (usuario == null || usuario.Clave != clave)
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
            }catch (Exception ex)
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
            repo.Alta(entidad);
            return RedirectToAction("Index", "Home");
        }
        return View(entidad);
    }
}




