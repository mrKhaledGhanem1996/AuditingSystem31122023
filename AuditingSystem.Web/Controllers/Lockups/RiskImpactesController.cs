using AuditingSystem.Entities.Lockups;
using AuditingSystem.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace AuditingSystem.Web.Controllers.Lockups
{
    public class RiskImpactesController : Controller
    {
        private readonly IBaseRepository<RiskImpact, int> _riskimapct;
        public RiskImpactesController(IBaseRepository<RiskImpact, int> riskimapct)
        {
            _riskimapct = riskimapct;
        }
        public async Task<IActionResult> Index()
        {
            var userId = HttpContext.Session.GetString("UserId");
            if (userId != null)
            {
                var riskimapct = await _riskimapct.ListAsync(
                       u => u.IsDeleted == false,
                       q => q.OrderBy(u => u.Id));

                return View(riskimapct);
            }
            return RedirectToAction("Login", "Account");
        }
        public async Task<IActionResult> Add()
        {
            return View();
        }
        public async Task<IActionResult> Edit(int id)
        {
            return View(await _riskimapct.FindByAsync(id));
        }
    }
}
