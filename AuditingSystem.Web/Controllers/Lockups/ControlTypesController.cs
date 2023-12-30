using AuditingSystem.Entities.Lockups;
using AuditingSystem.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace AuditingSystem.Web.Controllers.Lockups
{
    public class ControlTypesController : Controller
    {
        private readonly IBaseRepository<ControlType, int> _controletype;
        private readonly ILogger<ControlTypesController> _logger;
        public ControlTypesController(IBaseRepository<ControlType, int> controltype)
        {
            _controletype = controltype;
        }
        public async Task<IActionResult> Index()
        {
            var userId = HttpContext.Session.GetString("UserId");
            if (userId != null)
            {
                var controls = await _controletype.ListAsync(
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
            return View(await _controletype.FindByAsync(id));
        }
    }

}
    

