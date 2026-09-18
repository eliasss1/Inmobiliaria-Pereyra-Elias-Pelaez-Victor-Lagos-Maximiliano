using Microsoft.AspNetCore.Mvc;
using Inmobiliaria.Models;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Inmobiliaria.Helpers;
using Microsoft.AspNetCore.Authorization;
namespace Inmobiliaria.Controllers;

public class UsuarioController : Controller
{
    private readonly IRepositorioUsuario repo;
    private readonly IWebHostEnvironment env;

    public UsuarioController(IRepositorioUsuario _repo, IWebHostEnvironment _env)
{
    repo = _repo;
    env = _env;
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

    [Authorize]
    public IActionResult Perfil()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var usuario = repo.ObtenerPorId(int.Parse(userId));
        return View(usuario);
    }

    [Authorize]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Perfil(Usuario e, IFormFile? avatarFile)
    {

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        e.IdUsuario = int.Parse(userId);

        ModelState.Remove(nameof(e.Clave));

        if (ModelState.IsValid)
        {               
            if (avatarFile != null && avatarFile.Length > 0)
            {
                string uploadsFolder = Path.Combine(env.WebRootPath, "img", "avatars");
                if (!Directory.Exists(uploadsFolder)) 
                    Directory.CreateDirectory(uploadsFolder);

                string uniqueFileName = Guid.NewGuid().ToString() + "_" + avatarFile.FileName;
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await avatarFile.CopyToAsync(fileStream);
                }

                e.Avatar = "/img/avatars/" + uniqueFileName;
            }
            else
            {
                var usuarioAnterior = repo.ObtenerPorId(e.IdUsuario);
                e.Avatar = usuarioAnterior.Avatar;
            }
            repo.Modificacion(e);
            return RedirectToAction(nameof(Perfil));
        }
        return View(e);
    }

    [Authorize]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult CambiarContraseña(Usuario e)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        e.IdUsuario = int.Parse(userId);
        ModelState.Remove(nameof(e.Nombre));
        ModelState.Remove(nameof(e.Apellido));
        ModelState.Remove(nameof(e.Email));
        ModelState.Remove(nameof(e.Rol));
        ModelState.Remove(nameof(e.Avatar));
        if (ModelState.IsValid)
        {
            var usuario = repo.ObtenerPorId(e.IdUsuario);
            string claveHasheada = SeguridadHelper.HashearClave(e.Clave);
            if (usuario.Clave == claveHasheada)
            {
                ViewBag.Mensaje = "La nueva contraseña no puede ser igual a la actual";
                return View(e);
            }
            usuario.Clave = claveHasheada;
            repo.ModificarContraseña(usuario);
            ViewBag.Mensaje = "Contraseña actualizada correctamente";
            return RedirectToAction(nameof(Perfil));
        }
        return View(e);
    }
}