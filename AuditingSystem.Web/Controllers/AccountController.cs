using AuditingSystem.Entities.Setup;
using AuditingSystem.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace AuditingSystem.Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly IAccountRepository _accountRepository;
        
        public AccountController(IAccountRepository accountRepository)
        {
            _accountRepository = accountRepository;
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(User user)
        {
            (int? userId, string userName) = _accountRepository.Login(user);

            if (userId.HasValue)
            {
                HttpContext.Session.SetInt32("UserId", userId.Value);
                HttpContext.Session.SetString("UserName", userName);

                return RedirectToAction("Index", "User");
            }

            return View();
        }

        public async Task<IActionResult> Logout()
        {
            HttpContext.Session.Remove("UserId");
            return RedirectToAction("Login");
        }

    }
}
