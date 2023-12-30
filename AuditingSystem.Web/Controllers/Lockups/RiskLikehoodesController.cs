using AuditingSystem.Entities.Lockups;
using AuditingSystem.Services.Interfaces;
using AuditingSystem.Web.Controllers.api.Lockups;
using Microsoft.AspNetCore.Mvc;

namespace AuditingSystem.Web.Controllers.Lockups
{
    public class RiskLikehoodesController : Controller
    {
        private readonly IBaseRepository<RiskLikehood, int> _riskimpact;
        public RiskLikehoodesController(IBaseRepository<RiskLikehood, int> riskimpact)
        {
            _riskimpact = riskimpact;
        }
        public async Task<IActionResult> Index()
        {
            var userId = HttpContext.Session.GetString("UserId");
            if (userId != null)
            {
                var riskimpact = await _riskimpact.ListAsync(
                       u => u.IsDeleted == false,
                       q => q.OrderBy(u => u.Id));

                return View(riskimpact);
            }
            return RedirectToAction("Login", "Account");
        }
        public async Task<IActionResult> Add()
        {
            return View();
        }
        public async Task<IActionResult> Edit(int id)
        {
            return View(await _riskimpact.FindByAsync(id));
        }
    }
}
