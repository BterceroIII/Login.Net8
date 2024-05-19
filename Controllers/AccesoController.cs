using Microsoft.AspNetCore.Mvc;

using Data;
using Models;
using Microsoft.EntityFrameworkCore;

namespace AppLogin.Controllers
{
    public class AccesoController : Controller
    {
        private readonly LoginDbContext _logincontext;

        public AccesoController(LoginDbContext loginDbContext)
        {
            _logincontext = loginDbContext;
        }

        public IActionResult Registrarse()
        {
            return View();
        }
    }
}
