using AuditingSystem.Entities.Lockups;
using AuditingSystem.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace AuditingSystem.Web.Controllers.Lockups
{
    public class RiskCategorysscsController : Controller
    {
        private readonly IBaseRepository<RiskCategory, int> _riskcategory;
        private readonly ILogger<RiskCategorysscsController> _logger;
        public RiskCategorysscsController(IBaseRepository<RiskCategory, int> riskcategory)
        {
            _riskcategory = riskcategory;
        }
      
        public async Task<IActionResult> Index()
        {
            var userId = HttpContext.Session.GetString("UserId");
            if (userId != null)
            {
                var controls = await _riskcategory.ListAsync(
                       u => u.IsDeleted == false,
                       q => q.OrderBy(u => u.Id));

                return View(controls);
            }
            return RedirectToAction("Login", "Account");
        }
        public async Task<IActionResult> Add()
        {
            return View();
        }
        public async Task<IActionResult> Edit(int id)
        {
            return View(await _riskcategory.FindByAsync(id));
        }
    }
}


