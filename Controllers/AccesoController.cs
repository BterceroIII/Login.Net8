using Microsoft.AspNetCore.Mvc;

using Data;
using Models;
using Microsoft.EntityFrameworkCore;
using Models.ViewModels;

using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace AppLogin.Controllers
{
    public class AccesoController : Controller
    {
        private readonly LoginDbContext _logincontext;

        public AccesoController(LoginDbContext loginDbContext)
        {
            _logincontext = loginDbContext;
        }
        [HttpGet]
        public IActionResult Registrarse()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Registrarse(UsuarioVM modelo)
        {
            if (modelo.Password != modelo.ConfirmPassword)
            {
                ViewData["Mensaje"] = "Las contraseñas no coninciden";
                return View();
            }

            Usuario usuario = new Usuario()
            {
                FirstName = modelo.FirstName,
                LastName = modelo.LastName,
                Email = modelo.Email,
                Password = modelo.Password,
            };

            await _logincontext.Usuarios.AddAsync(usuario);
            await _logincontext.SaveChangesAsync();

            if (usuario.IdUsuario != 0)
            {
                return RedirectToAction("Login", "Acceso");
            }

            ViewData["Mensaje"] = "No se puede crear el usuario, error fatal";

            return View();
        }

        [HttpGet]
        public IActionResult Login()
        {
            if (User.Identity!.IsAuthenticated) return RedirectToAction("index","Home");

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginVM modelo)
        {
            Usuario? usuario_encontrado = await _logincontext.Usuarios
                                            .Where(u =>
                                                    u.Email == modelo.Email &&
                                                    u.Password == modelo.Password).FirstOrDefaultAsync();

            if (usuario_encontrado == null)
            {
                ViewData["Mensaje"] = "No se encontraron coincidencias";
                return View();
            }

            List<Claim> claims = new List<Claim>()
            {
                new Claim(ClaimTypes.Name, usuario_encontrado.FirstName)
            };

            ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims,CookieAuthenticationDefaults.AuthenticationScheme);
            AuthenticationProperties properties = new AuthenticationProperties() 
            {
                AllowRefresh = true,
            };

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                properties
                );

            return RedirectToAction("Index", "Home");
        }
    }
}
